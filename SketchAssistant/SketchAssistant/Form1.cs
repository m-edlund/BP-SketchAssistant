using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


// This is the code for your desktop app.
// Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.

namespace SketchAssistant
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /**********************************/
        /*** CLASS VARIABLES START HERE ***/
        /**********************************/

        //Different Program States
        public enum ProgramState
        {
            Idle,
            Draw,
            Delete
        }
        //Current Program State
        private ProgramState currentState;
        //Dialog to select a file.
        OpenFileDialog openFileDialogLeft = new OpenFileDialog();
        //Image loaded on the left
        Image leftImage = null;
        //Image on the right
        Image rightImage = null;
        //Current Line being Drawn
        List<Point> currentLine;
        //All Lines in the current session
        List<Tuple<bool,Line>> lineList = new List<Tuple<bool, Line>>();
        //Whether the Mouse is currently pressed in the rightPictureBox
        bool mousePressed = false;
        //The Position of the Cursor in the right picture box
        Point currentCursorPosition;
        //The Previous Cursor Position in the right picture box
        Point previousCursorPosition;
        //Queue for the cursorPositions
        Queue<Point> cursorPositions = new Queue<Point>();
        //The graphic representation of the right image
        Graphics graph = null;
        //Deletion Matrixes for checking postions of lines in the image
        bool[,] isFilledMatrix;
        HashSet<int>[,] linesMatrix;
        //Size of deletion area
        uint deletionSize = 2;
        //History of Actions
        ActionHistory historyOfActions;

        /******************************************/
        /*** FORM SPECIFIC FUNCTIONS START HERE ***/
        /******************************************/

        private void Form1_Load(object sender, EventArgs e)
        {
            currentState = ProgramState.Idle;
            this.DoubleBuffered = true;
            historyOfActions = new ActionHistory();
        }

        //Resize Function connected to the form resize event, will refresh the form when it is resized
        private void Form1_Resize(object sender, System.EventArgs e)
        {
            this.Refresh();
        }
        
        //Load button, will open an OpenFileDialog
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialogLeft.Filter = "Image|*.jpg;*.png;*.jpeg";
            if(openFileDialogLeft.ShowDialog() == DialogResult.OK)
            {
                toolStripLoadStatus.Text = openFileDialogLeft.SafeFileName;
                leftImage = Image.FromFile(openFileDialogLeft.FileName);
                pictureBoxLeft.Image = leftImage;
                //Refresh the left image box when the content is changed
                this.Refresh();
            }
        }

        //Changes the state of the program to drawing
        private void drawButton_Click(object sender, EventArgs e)
        {
            if(rightImage != null)
            {
                if (currentState.Equals(ProgramState.Draw))
                {
                    ChangeState(ProgramState.Idle);
                }
                else
                {
                    ChangeState(ProgramState.Draw);
                }
            }
        }

        //Changes the state of the program to deletion
        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (rightImage != null)
            {
                if (currentState.Equals(ProgramState.Delete))
                {
                    ChangeState(ProgramState.Idle);
                }
                else
                {
                    ChangeState(ProgramState.Delete);
                }
            }
        }

        //get current Mouse positon within the right picture box
        private void pictureBoxRight_MouseMove(object sender, MouseEventArgs e)
        {
            currentCursorPosition = ConvertCoordinates(new Point(e.X, e.Y));
        }

        //hold left mouse button to draw.
        private void pictureBoxRight_MouseDown(object sender, MouseEventArgs e)
        {
            mousePressed = true;
            if (currentState.Equals(ProgramState.Draw))
            {
                currentLine = new List<Point>();
            }
        }

        //Lift left mouse button to stop drawing and add a new Line.
        private void pictureBoxRight_MouseUp(object sender, MouseEventArgs e)
        {
            mousePressed = false;
            if (currentState.Equals(ProgramState.Draw) && currentLine.Count > 0)
            {
                Line newLine = new Line(currentLine, lineList.Count);
                lineList.Add(new Tuple<bool, Line>(true, newLine));
                newLine.PopulateMatrixes(isFilledMatrix, linesMatrix);
                historyOfActions.AddNewAction(new Action(Action.ActionType.Draw, newLine.GetID()));
            }
        }

        //Button to create a new Canvas. Will create an empty image 
        //which is the size of the left image, if there is one.
        //If there is no image loaded the canvas will be the size of the right picture box
        private void canvasButton_Click(object sender, EventArgs e)
        {
            if (!historyOfActions.IsEmpty())
            {
                if (MessageBox.Show("You have unsaved changes, creating a new canvas will discard these.", 
                    "Attention", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    historyOfActions = new ActionHistory();
                    DrawEmptyCanvas();
                    //The following lines cannot be in DrawEmptyCanvas()
                    isFilledMatrix = new bool[rightImage.Width, rightImage.Height];
                    linesMatrix = new HashSet<int>[rightImage.Width, rightImage.Height];
                    lineList = new List<Tuple<bool, Line>>();
                }
            }
            else
            {
                DrawEmptyCanvas();
                //The following lines cannot be in DrawEmptyCanvas()
                isFilledMatrix = new bool[rightImage.Width, rightImage.Height];
                linesMatrix = new HashSet<int>[rightImage.Width, rightImage.Height];
                lineList = new List<Tuple<bool, Line>>();
            }
        }

        //add a Point on every tick to the Drawpath
        private void mouseTimer_Tick(object sender, EventArgs e)
        {
            cursorPositions.Enqueue(currentCursorPosition);
            previousCursorPosition = cursorPositions.Dequeue();

            if (currentState.Equals(ProgramState.Draw) && mousePressed)
            {
                currentLine.Add(currentCursorPosition);
                Line drawline = new Line(currentLine);
                drawline.DrawLine(graph);
                pictureBoxRight.Image = rightImage;
            }
            if (currentState.Equals(ProgramState.Delete) && mousePressed)
            {
                List<Point> uncheckedPoints = Line.BresenhamLineAlgorithm(previousCursorPosition, currentCursorPosition);
                foreach (Point currPoint in uncheckedPoints)
                {
                    HashSet<int> linesToDelete = CheckDeletionMatrixesAroundPoint(currPoint, deletionSize);
                    historyOfActions.AddNewAction(new Action(Action.ActionType.Delete, linesToDelete));
                    if (linesToDelete.Count > 0)
                    {
                        foreach (int lineID in linesToDelete)
                        {
                            lineList[lineID] = new Tuple<bool, Line>(false, lineList[lineID].Item2);
                        }
                        RepopulateDeletionMatrixes();
                        RedrawRightImage();
                    }
                }
            }
        }

        /***********************************/
        /*** HELPER FUNCTIONS START HERE ***/
        /***********************************/

        /// <summary>
        /// Creates an empty Canvas
        /// </summary>
        private void DrawEmptyCanvas()
        {
            if (leftImage == null)
            {
                rightImage = new Bitmap(pictureBoxRight.Width, pictureBoxRight.Height);
                graph = Graphics.FromImage(rightImage);
                graph.FillRectangle(Brushes.White, 0, 0, pictureBoxRight.Width + 10, pictureBoxRight.Height + 10);
                pictureBoxRight.Image = rightImage;
            }
            else
            {
                rightImage = new Bitmap(leftImage.Width, leftImage.Height);
                graph = Graphics.FromImage(rightImage);
                graph.FillRectangle(Brushes.White, 0, 0, leftImage.Width + 10, leftImage.Height + 10);
                pictureBoxRight.Image = rightImage;
            }
            this.Refresh();
            pictureBoxRight.Refresh();
        }

        /// <summary>
        /// Redraws all lines in lineList, for which their associated boolean value equals true.
        /// </summary>
        private void RedrawRightImage()
        {
            DrawEmptyCanvas();
            foreach (Tuple<bool, Line> lineBoolTuple in lineList)
            {
                if (lineBoolTuple.Item1)
                {
                    lineBoolTuple.Item2.DrawLine(graph);
                }
            }
            pictureBoxRight.Refresh();
        }

        /// <summary>
        /// A helper function which handles tasks associated witch changing states, 
        /// such as checking and unchecking buttons and changing the state.
        /// </summary>
        /// <param name="newState">The new state of the program</param>
        private void ChangeState(ProgramState newState)
        {
            switch (currentState)
            {
                case ProgramState.Draw:
                    drawButton.CheckState = CheckState.Unchecked;
                    mouseTimer.Enabled = false;
                    break;
                case ProgramState.Delete:
                    deleteButton.CheckState = CheckState.Unchecked;
                    mouseTimer.Enabled = false;
                    break;
                default:
                    break;
            }
            switch (newState)
            {
                case ProgramState.Draw:
                    drawButton.CheckState = CheckState.Checked;
                    mouseTimer.Enabled = true;
                    break;
                case ProgramState.Delete:
                    deleteButton.CheckState = CheckState.Checked;
                    mouseTimer.Enabled = true;
                    break;
                default:
                    break;
            }
            currentState = newState;
            pictureBoxRight.Refresh();
        }

        /// <summary>
        /// A function that calculates the coordinates of a point on a zoomed in image.
        /// </summary>
        /// <param name="">The position of the mouse cursor</param>
        /// <returns>The real coordinates of the mouse cursor on the image</returns>
        private Point ConvertCoordinates(Point cursorPosition)
        {
            Point realCoordinates = new Point(5,3);
            if(pictureBoxRight.Image == null)
            {
                return cursorPosition;
            }

            int widthImage = pictureBoxRight.Image.Width;
            int heightImage = pictureBoxRight.Image.Height;
            int widthBox = pictureBoxRight.Width;
            int heightBox = pictureBoxRight.Height;

            float imageRatio = (float)widthImage / (float)heightImage;
            float containerRatio = (float)widthBox / (float)heightBox;

            if (imageRatio >= containerRatio)
            {
                //Image is wider than it is high
                float zoomFactor = (float)widthImage / (float)widthBox;
                float scaledHeight = heightImage / zoomFactor;
                float filler = (heightBox - scaledHeight) / 2;
                realCoordinates.X = (int)(cursorPosition.X * zoomFactor);
                realCoordinates.Y = (int)((cursorPosition.Y - filler) * zoomFactor);
            }
            else
            {
                //Image is higher than it is wide
                float zoomFactor = (float)heightImage / (float)heightBox;
                float scaledWidth = widthImage / zoomFactor;
                float filler = (widthBox - scaledWidth) / 2;
                realCoordinates.X = (int)((cursorPosition.X - filler) * zoomFactor);
                realCoordinates.Y = (int)(cursorPosition.Y * zoomFactor);
            }
            return realCoordinates;
        }

        /// <summary>
        /// A function that populates the matrixes needed for deletion detection with line data.
        /// </summary>
        private void RepopulateDeletionMatrixes()
        {
            if(rightImage != null)
            {
                isFilledMatrix = new bool[rightImage.Width,rightImage.Height];
                linesMatrix = new HashSet<int>[rightImage.Width, rightImage.Height];
                foreach(Tuple<bool,Line> lineTuple in lineList)
                {
                    if (lineTuple.Item1)
                    {
                        lineTuple.Item2.PopulateMatrixes(isFilledMatrix, linesMatrix);
                    }
                }
            }
        }

        /// <summary>
        /// A function that checks the deletion matrixes at a certain point 
        /// and returns all Line ids at that point and in a square around it in a certain range.
        /// </summary>
        /// <param name="p">The point around which to check.</param>
        /// <param name="range">The range around the point. If range is 0, only the point is checked.</param>
        /// <returns>A List of all lines.</returns>
        private HashSet<int> CheckDeletionMatrixesAroundPoint(Point p, uint range)
        {
            HashSet<int> returnSet = new HashSet<int>();

            if (p.X >= 0 && p.Y >= 0 && p.X < rightImage.Width && p.Y < rightImage.Height)
            {
                if (isFilledMatrix[p.X, p.Y])
                {
                    returnSet.UnionWith(linesMatrix[p.X, p.Y]);
                }
            }
            for (int x_mod = (int)range*(-1); x_mod < range; x_mod++)
            {
                for (int y_mod = (int)range * (-1); y_mod < range; y_mod++)
                {
                    if (p.X + x_mod >= 0 && p.Y + y_mod >= 0 && p.X + x_mod < rightImage.Width && p.Y + y_mod < rightImage.Height)
                    {
                        if (isFilledMatrix[p.X + x_mod, p.Y + y_mod])
                        {
                            returnSet.UnionWith(linesMatrix[p.X + x_mod, p.Y + y_mod]);
                        }
                    }
                }
            }
            return returnSet;
        }
    }
}
