using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;


// This is the code for your desktop app.
// Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.

namespace SketchAssistant
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            fileImporter = new FileImporter(this);
        }

        /**********************************/
        /*** CLASS VARIABLES START HERE ***/
        /**********************************/

        //important: add new variables only at the end of the list to keep the order of definition consistent with the order in which they are returned by GetAllVariables()

        /// <summary>
        /// Different Program States
        /// </summary>
        public enum ProgramState
        {
            Idle,
            Draw,
            Delete
        }
        /// <summary>
        /// Current Program State
        /// </summary>
        private ProgramState currentState;
        /// <summary>
        /// instance of FileImporter to handle drawing imports
        /// </summary>
        private FileImporter fileImporter;
        /// <summary>
        /// Dialog to select a file.
        /// </summary>
        OpenFileDialog openFileDialog = new OpenFileDialog();
        /// <summary>
        /// Image loaded on the left
        /// </summary>
        private Image leftImage = null;
        /// <summary>
        /// the graphic shown in the left window, represented as a list of polylines
        /// </summary>
        private List<Line> leftLineList;
        /// <summary>
        /// Image on the right
        /// </summary>
        Image rightImage = null;
        /// <summary>
        /// Current Line being Drawn
        /// </summary>
        List<Point> currentLine;
        /// <summary>
        /// All Lines in the current session
        /// </summary>
        List<Tuple<bool,Line>> rightLineList = new List<Tuple<bool, Line>>();
        /// <summary>
        /// Whether the Mouse is currently pressed in the rightPictureBox
        /// </summary>
        bool mousePressed = false;
        /// <summary>
        /// The Position of the Cursor in the right picture box
        /// </summary>
        Point currentCursorPosition;
        /// <summary>
        /// The Previous Cursor Position in the right picture box
        /// </summary>
        Point previousCursorPosition;
        /// <summary>
        /// Queue for the cursorPositions
        /// </summary>
        Queue<Point> cursorPositions = new Queue<Point>();
        /// <summary>
        /// The graphic representation of the right image
        /// </summary>
        Graphics rightGraph = null;
        /// <summary>
        /// Lookup Matrix for checking postions of lines in the image
        /// </summary>
        bool[,] isFilledMatrix;
        /// <summary>
        /// Lookup Matrix for getting line ids at a certain postions of the image
        /// </summary>
        HashSet<int>[,] linesMatrix;
        /// <summary>
        /// Size of deletion area
        /// </summary>
        int deletionRadius = 2;
        /// <summary>
        /// History of Actions
        /// </summary>
        ActionHistory historyOfActions;
        /// <summary>
        /// A boolean value indicating whether or not the redraw mode is active.
        /// </summary>
        bool redrawModeActive = false;
        /// <summary>
        /// A tuple containing the areas around the end points of the current line in the left graphic.
        /// </summary>
        Tuple<HashSet<Point>, HashSet<Point>> currentLineEndings;
        /// <summary>
        /// Size of areas marking endpoints of lines in the redraw mode.
        /// </summary>
        int markerRadius = 10;

        /******************************************/
        /*** FORM SPECIFIC FUNCTIONS START HERE ***/
        /******************************************/

        private void Form1_Load(object sender, EventArgs e)
        {
            currentState = ProgramState.Idle;
            this.DoubleBuffered = true;
            historyOfActions = new ActionHistory(null);
            UpdateButtonStatus();
        }

        //Resize Function connected to the form resize event, will refresh the form when it is resized
        private void Form1_Resize(object sender, System.EventArgs e)
        {
            this.Refresh();
            UpdateSizes();
        }
        
        //Load button, will open an OpenFileDialog
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog.Filter = "Image|*.jpg;*.png;*.jpeg";
            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                toolStripLoadStatus.Text = openFileDialog.SafeFileName;
                leftImage = Image.FromFile(openFileDialog.FileName);
                pictureBoxLeft.Image = leftImage;
                //Refresh the left image box when the content is changed
                this.Refresh();
            }
            UpdateButtonStatus();
        }

        /// <summary>
        /// Import button, will open an OpenFileDialog
        /// </summary>
        private void examplePictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CheckSavedStatus())
            {
                openFileDialog.Filter = "Interactive Sketch-Assistant Drawing|*.isad";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    toolStripLoadStatus.Text = openFileDialog.SafeFileName;
                    try
                    {
                        (int, int, List<Line>) values = fileImporter.ParseISADInputFile(openFileDialog.FileName);
                        DrawEmptyCanvasLeft(values.Item1, values.Item2);
                        BindAndDrawLeftImage(values.Item3);

                        //Match The right canvas to the left
                        historyOfActions = new ActionHistory(lastActionTakenLabel);
                        DrawEmptyCanvasRight();
                        isFilledMatrix = new bool[rightImage.Width, rightImage.Height];
                        linesMatrix = new HashSet<int>[rightImage.Width, rightImage.Height];
                        rightLineList = new List<Tuple<bool, Line>>();
                        //Show the start and ends of lines
                        CalculateAndDisplayStartAndEnd(leftLineList[0], markerRadius);

                        this.Refresh();
                    }
                    catch (FileImporterException ex)
                    {
                        ShowInfoMessage(ex.ToString());
                    }
                }
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
            UpdateButtonStatus();
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
            UpdateButtonStatus();
        }

        //Undo an action
        private void undoButton_Click(object sender, EventArgs e)
        {
            if (historyOfActions.CanUndo())
            {
                HashSet<int> affectedLines = historyOfActions.GetCurrentAction().GetLineIDs();
                SketchAction.ActionType  undoAction = historyOfActions.GetCurrentAction().GetActionType();
                switch (undoAction)
                {
                    case SketchAction.ActionType.Delete:
                        //Deleted Lines need to be shown
                        ChangeLines(affectedLines, true);
                        break;
                    case SketchAction.ActionType.Draw:
                        //Drawn lines need to be hidden
                        ChangeLines(affectedLines, false);
                        break;
                    default:
                        break;
                }
            }
            historyOfActions.MoveAction(true);
            UpdateButtonStatus();
        }

        //Redo an action
        private void redoButton_Click(object sender, EventArgs e)
        {
            if (historyOfActions.CanRedo())
            {
                historyOfActions.MoveAction(false);
                HashSet<int> affectedLines = historyOfActions.GetCurrentAction().GetLineIDs();
                SketchAction.ActionType redoAction = historyOfActions.GetCurrentAction().GetActionType();
                switch (redoAction)
                {
                    case SketchAction.ActionType.Delete:
                        //Deleted Lines need to be redeleted
                        ChangeLines(affectedLines, false);
                        break;
                    case SketchAction.ActionType.Draw:
                        //Drawn lines need to be redrawn
                        ChangeLines(affectedLines, true);
                        break;
                    default:
                        break;
                }
            }
            UpdateButtonStatus();
        }

        //Detect Keyboard Shortcuts
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Z)
            {
                undoButton_Click(sender, e);
            }
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Y)
            {
                redoButton_Click(sender, e);
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
                Line newLine = new Line(currentLine, rightLineList.Count);
                rightLineList.Add(new Tuple<bool, Line>(true, newLine));
                newLine.PopulateMatrixes(isFilledMatrix, linesMatrix);
                historyOfActions.AddNewAction(new SketchAction(SketchAction.ActionType.Draw, newLine.GetID()));
            }
            UpdateButtonStatus();
        }

        //Button to create a new Canvas. Will create an empty image 
        //which is the size of the left image, if there is one.
        //If there is no image loaded the canvas will be the size of the right picture box
        private void canvasButton_Click(object sender, EventArgs e)
        {
            if (CheckSavedStatus())
            {
                historyOfActions = new ActionHistory(lastActionTakenLabel);
                DrawEmptyCanvasRight();
                //The following lines cannot be in DrawEmptyCanvas()
                isFilledMatrix = new bool[rightImage.Width, rightImage.Height];
                linesMatrix = new HashSet<int>[rightImage.Width, rightImage.Height];
                rightLineList = new List<Tuple<bool, Line>>();
            }
            UpdateButtonStatus();
            UpdateSizes();
        }

        //add a Point on every tick to the Drawpath
        private void mouseTimer_Tick(object sender, EventArgs e)
        {
            if(cursorPositions.Count > 0) { previousCursorPosition = cursorPositions.Dequeue(); }
            else { previousCursorPosition = currentCursorPosition; }
            cursorPositions.Enqueue(currentCursorPosition);
            //Drawing
            if (currentState.Equals(ProgramState.Draw) && mousePressed)
            {
                rightGraph = Graphics.FromImage(rightImage);
                currentLine.Add(currentCursorPosition);
                Line drawline = new Line(currentLine);
                drawline.DrawLine(rightGraph);
                pictureBoxRight.Image = rightImage;
            }
            //Deleting
            if (currentState.Equals(ProgramState.Delete) && mousePressed)
            {
                List<Point> uncheckedPoints = GeometryCalculator.BresenhamLineAlgorithm(previousCursorPosition, currentCursorPosition);
                foreach (Point currPoint in uncheckedPoints)
                {
                    HashSet<int> linesToDelete = CheckDeletionMatrixesAroundPoint(currPoint, deletionRadius);
                    if (linesToDelete.Count > 0)
                    {
                        historyOfActions.AddNewAction(new SketchAction(SketchAction.ActionType.Delete, linesToDelete));
                        foreach (int lineID in linesToDelete)
                        {
                            rightLineList[lineID] = new Tuple<bool, Line>(false, rightLineList[lineID].Item2);
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
        /// A function that returns a white canvas for a given width and height.
        /// </summary>
        /// <param name="width">The width of the canvas in pixels</param>
        /// <param name="height">The height of the canvas in pixels</param>
        /// <returns>The new canvas</returns>
        private Image GetEmptyCanvas(int width, int height)
        {
            Image image = new Bitmap(width, height);
            Graphics graph = Graphics.FromImage(image);
            graph.FillRectangle(Brushes.White, 0, 0, width + 10, height + 10);
            return image;
        }

        /// <summary>
        /// Creates an empty Canvas
        /// </summary>
        private void DrawEmptyCanvasRight()
        {
            if (leftImage == null)
            {
                SetAndRefreshRightImage(GetEmptyCanvas(pictureBoxRight.Width, pictureBoxRight.Height));
            }
            else
            {
                SetAndRefreshRightImage(GetEmptyCanvas(leftImage.Width, leftImage.Height));
            }
        }

        /// <summary>
        /// Creates an empty Canvas on the left
        /// </summary>
        /// <param name="width"> width of the new canvas in pixels </param>
        /// <param name="height"> height of the new canvas in pixels </param>
        private void DrawEmptyCanvasLeft(int width, int height)
        {
            if (width == 0)
            {
                SetAndRefreshLeftImage(GetEmptyCanvas(pictureBoxLeft.Width, pictureBoxLeft.Height));
            }
            else
            {
                SetAndRefreshLeftImage(GetEmptyCanvas(width, height));
            }
        }

        /// <summary>
        /// Redraws all lines in lineList, for which their associated boolean value equals true.
        /// </summary>
        private void RedrawRightImage()
        {
            var workingCanvas = GetEmptyCanvas(rightImage.Width, rightImage.Height);
            var workingGraph = Graphics.FromImage(workingCanvas);
            foreach (Tuple<bool, Line> lineBoolTuple in rightLineList)
            {
                if (lineBoolTuple.Item1)
                {
                    lineBoolTuple.Item2.DrawLine(workingGraph);
                }
            }
            SetAndRefreshRightImage(workingCanvas);
        }

        /// <summary>
        /// A function to set rightImage and to refresh the respective PictureBox with it.
        /// </summary>
        /// <param name="image">The new Image</param>
        private void SetAndRefreshRightImage(Image image)
        {
            rightImage = image;
            pictureBoxRight.Image = rightImage;
            pictureBoxRight.Refresh();
        }

        /// <summary>
        /// A function to set leftImage and to refresh the respective PictureBox with it.
        /// </summary>
        /// <param name="image">The new Image</param>
        private void SetAndRefreshLeftImage(Image image)
        {
            leftImage = image;
            pictureBoxLeft.Image = leftImage;
            pictureBoxLeft.Refresh();
        }

        /// <summary>
        /// Change the status of whether or not the lines are shown.
        /// </summary>
        /// <param name="lines">The HashSet containing the affected Line IDs.</param>
        /// <param name="shown">True if the lines should be shown, false if they should be hidden.</param>
        private void ChangeLines(HashSet<int> lines, bool shown)
        {
            foreach (int lineId in lines)
            {
                if (lineId <= rightLineList.Count - 1 && lineId >= 0)
                {
                    rightLineList[lineId] = new Tuple<bool, Line>(shown, rightLineList[lineId].Item2);
                }
            }
            RedrawRightImage();
        }

        /// <summary>
        /// Updates the active status of buttons. Currently draw, delete, undo and redo button.
        /// </summary>
        private void UpdateButtonStatus()
        {
            undoButton.Enabled = historyOfActions.CanUndo();
            redoButton.Enabled = historyOfActions.CanRedo();
            drawButton.Enabled = (rightImage != null);
            deleteButton.Enabled = (rightImage != null);
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
                foreach(Tuple<bool,Line> lineTuple in rightLineList)
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
        private HashSet<int> CheckDeletionMatrixesAroundPoint(Point p, int range)
        {
            HashSet<int> returnSet = new HashSet<int>();

            foreach(Point pnt in GeometryCalculator.FilledCircleAlgorithm(p, (int)range))
            {
                if (isFilledMatrix[pnt.X, pnt.Y])
                {
                    returnSet.UnionWith(linesMatrix[pnt.X, pnt.Y]);
                }
            }
            return returnSet;
        }

        /// <summary>
        /// binds the given picture to templatePicture and draws it
        /// </summary>
        /// <param name="newTemplatePicture"> the new template picture, represented as a list of polylines </param>
        /// <returns></returns>
        private void BindAndDrawLeftImage(List<Line> newTemplatePicture)
        {
            leftLineList = newTemplatePicture;
            foreach(Line l in leftLineList)
            {
                l.DrawLine(Graphics.FromImage(leftImage));
            }
        }

        /// <summary>
        /// shows the given info message in a popup and asks the user to aknowledge it
        /// </summary>
        /// <param name="message">the message to show</param>
        private void ShowInfoMessage(String message)
        {
            MessageBox.Show(message);
        }
        
        /// <summary>
        /// Will calculate and mark the start and endpoints of the given line on the right canvas.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="size">The size of the circle with which the endpoints of the line are marked.</param>
        private void CalculateAndDisplayStartAndEnd(Line line, int size)
        {
            var circle0 = GeometryCalculator.FilledCircleAlgorithm(line.GetStartPoint(), size);
            var circle1 = GeometryCalculator.BresenhamCircleAlgorithm(line.GetEndPoint(), size);
            currentLineEndings = new Tuple<HashSet<Point>, HashSet<Point>>(circle0, circle1);

            rightGraph = Graphics.FromImage(rightImage);
            foreach(Point p in circle0) { rightGraph.FillRectangle(Brushes.Red, p.X, p.Y, 1, 1); }
            foreach(Point p in circle1) { rightGraph.FillRectangle(Brushes.Blue, p.X, p.Y, 1, 1); }

            SetAndRefreshRightImage(rightImage);
        }

        /// <summary>
        /// A helper Function that updates the markerRadius & deletionRadius, considering the size of the canvas.
        /// </summary>
        private void UpdateSizes()
        {
            if (rightImage != null)
            {
                double widthProp = pictureBoxRight.Width / rightImage.Width;
                double heightProp = pictureBoxRight.Height / rightImage.Height;
                double scaling = (widthProp + heightProp) / 2;
                markerRadius = (int)(10 / scaling);
                deletionRadius = (int)(5 / scaling);
            }
        }

        /// <summary>
        /// Checks if there is unsaved progess, and warns the user. Returns True if it safe to continue.
        /// </summary>
        /// <returns>true if there is none, or the user wishes to continue without saving.
        /// false if there is progress, and the user doesn't wish to continue.</returns>
        private bool CheckSavedStatus()
        {
            if (!historyOfActions.IsEmpty())
            {
                return (MessageBox.Show("You have unsaved changes, do you wish to continue?",
                    "Attention", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes);
            }
            return true;
        }

        /********************************************/
        /*** TESTING RELATED FUNCTIONS START HERE ***/
        /********************************************/

        /// <summary>
        /// returns all instance variables in the order of their definition for testing
        /// </summary>
        /// <returns>A list of tuples containing names of variables and the variable themselves. 
        /// Cast according to the Type definitions in the class variable section.</returns>
        public List<Tuple<String, Object>>GetAllVariables()
        {
            var objArr = new Object[] { currentState, fileImporter, openFileDialog, leftImage, leftLineList, rightImage, currentLine, rightLineList, mousePressed, currentCursorPosition, previousCursorPosition, cursorPositions, rightGraph, isFilledMatrix, linesMatrix, deletionRadius, historyOfActions };
            var varArr = new List<Tuple<String, Object>>();
            foreach(Object obj in objArr)
            {
                varArr.Add(new Tuple<string, object>(nameof(obj), obj));
            }
            return varArr;
        }

        /// <summary>
        /// public method wrapper for testing purposes, invoking DrawEmptyCanvas(...) and BindAndDrawLeftImage(...)
        /// </summary>
        /// <param name="width">width of the parsed image</param>
        /// <param name="height">height of the parsed image</param>
        /// <param name="newImage">the parsed image</param>
        public void CreateCanvasAndSetPictureForTesting(int width, int height, List<Line> newImage)
        {
            DrawEmptyCanvasLeft(width, height);
            BindAndDrawLeftImage(newImage);
        }
    }
}
