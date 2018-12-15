using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;


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
        /// Deletion Matrixes for checking postions of lines in the image
        /// </summary>
        bool[,] isFilledMatrix;
        HashSet<int>[,] linesMatrix;
        /// <summary>
        /// Size of deletion area
        /// </summary>
        uint deletionSize = 2;
        /// <summary>
        /// History of Actions
        /// </summary>
        ActionHistory historyOfActions;
		/// <summary>
		///Dialog to save a file
		/// </summary>
		SaveFileDialog saveFileDialogRight = new SaveFileDialog();

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
            openFileDialog.Filter = "Interactive Sketch-Assistant Drawing|*.isad";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                toolStripLoadStatus.Text = openFileDialog.SafeFileName;
                try
                {
                    (int, int, List<Line>) values = fileImporter.ParseISADInputFile(openFileDialog.FileName);
                    DrawEmptyCanvasLeft(values.Item1, values.Item2);
                    BindAndDrawLeftImage(values.Item3);
                    this.Refresh();
                }
                catch(FileImporterException ex)
                {
                    ShowInfoMessage(ex.ToString());
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
            if (!historyOfActions.IsEmpty())
            {
                if (MessageBox.Show("You have unsaved changes, creating a new canvas will discard these.", 
                    "Attention", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    historyOfActions = new ActionHistory(lastActionTakenLabel);
                    DrawEmptyCanvasRight();
                    //The following lines cannot be in DrawEmptyCanvas()
                    isFilledMatrix = new bool[rightImage.Width, rightImage.Height];
                    linesMatrix = new HashSet<int>[rightImage.Width, rightImage.Height];
                    rightLineList = new List<Tuple<bool, Line>>();
                }
            }
            else
            {
                historyOfActions = new ActionHistory(lastActionTakenLabel);
                DrawEmptyCanvasRight();
                //The following lines cannot be in DrawEmptyCanvas()
                isFilledMatrix = new bool[rightImage.Width, rightImage.Height];
                linesMatrix = new HashSet<int>[rightImage.Width, rightImage.Height];
                rightLineList = new List<Tuple<bool, Line>>();
            }
            UpdateButtonStatus();
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
                drawline.DrawLine(rightGraph);
                pictureBoxRight.Image = rightImage;
            }
            if (currentState.Equals(ProgramState.Delete) && mousePressed)
            {
                List<Point> uncheckedPoints = Line.BresenhamLineAlgorithm(previousCursorPosition, currentCursorPosition);
                foreach (Point currPoint in uncheckedPoints)
                {
                    HashSet<int> linesToDelete = CheckDeletionMatrixesAroundPoint(currPoint, deletionSize);
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


		//Save button, will open an SaveFileDialog
		private void saveToolStripMenuItem_Click_1(object sender, EventArgs e)
		{
			if (saveFileDialogRight.ShowDialog() == DialogResult.OK)
			{
				StreamWriter sw = new StreamWriter(File.OpenWrite(saveFileDialogRight.FileName));
				sw.Write("hello");
				sw.Close();
			}
			
			//if (rightImage != null)
			//{
			//	saveFileDialogRight.Filter = "Image|*.jpg;*.png;*.jpeg|" + "Vector Graphics|*.txt|" + "All files (*.*)|*.*";
			//	ImageFormat format = ImageFormat.Jpeg;
			//	if (saveFileDialogRight.ShowDialog() == DialogResult.OK)
			//	{

			//		switch (saveFileDialogRight.Filter)
			//		{
			//			case ".txt":
							







			//			/*String newReturnString = createSvgTxt();
			//			File.WriteAllText(saveFileDialogRight.FileName, newReturnString);
			//			return;
			//			*/

			//			/*sw.WriteLine(String.Format("<svg viewBox = \"0 0 {0} {1}\" xmlns = \"http://www.w3.org/2000/svg\">", rightImage.Width, rightImage.Height));
			//			foreach (Tuple<bool, Line> lineTuple in rightLineList)
			//			{
			//				if (lineTuple.Item1 == true)
			//				{
			//						sw.WriteLine("\n" + "<line x1 = \"{0}\" y1 = \"{1}\" x2 = \"{2}\" y2 = \"{3}\" stroke = \"black\">",
			//														lineTuple.Item2.GetStartPoint().X, lineTuple.Item2.GetStartPoint().Y, lineTuple.Item2.GetEndPoint().X, lineTuple.Item2.GetEndPoint().X);
			//					//sw.WriteLine("1");
			//				}
			//			}*/


			//			/*using (FileStream fs = File.Create(saveFileDialogRight.FileName))
			//			{
			//				fs.Write(Encoding.UTF8.GetBytes(String.Format("<svg viewBox = \"0 0 {0} {1}\" xmlns = \"http://www.w3.org/2000/svg\">", rightImage.Width, rightImage.Height)),0,int.MaxValue);
			//				foreach (Tuple<bool, Line> lineTuple in rightLineList)
			//				{
			//					if (lineTuple.Item1 == true)
			//					{
			//						fs.Write(Encoding.UTF8.GetBytes(String.Format("\n" + "<line x1 = \"{0}\" y1 = \"{1}\" x2 = \"{2}\" y2 = \"{3}\" stroke = \"black\">",
			//														lineTuple.Item2.GetStartPoint().X, lineTuple.Item2.GetStartPoint().Y, lineTuple.Item2.GetEndPoint().X, lineTuple.Item2.GetEndPoint().X)),0,int.MaxValue);
			//					}
			//				}
			//			}
			//			return;
			//			*/


			//			case ".png":

			//				format = ImageFormat.Png;
			//				pictureBoxRight.Image.Save(saveFileDialogRight.FileName, format);
			//				break;

			//			case ".bmp":

			//				format = ImageFormat.Bmp;
			//				pictureBoxRight.Image.Save(saveFileDialogRight.FileName, format);
			//				break;

			//			default:
			//				pictureBoxRight.Image.Save(saveFileDialogRight.FileName, format);
			//				break;
			//		}


			//	}
			//}
			//else
			//{
			//	MessageBox.Show("The right picture box can't be empty");
			//}


		}

		/***********************************/
		/*** HELPER FUNCTIONS START HERE ***/
		/***********************************/

		/// <summary>
		/// Creates an empty Canvas
		/// </summary>
		private void DrawEmptyCanvasRight()
        {
            if (leftImage == null)
            {
                rightImage = new Bitmap(pictureBoxRight.Width, pictureBoxRight.Height);
                rightGraph = Graphics.FromImage(rightImage);
                rightGraph.FillRectangle(Brushes.White, 0, 0, pictureBoxRight.Width + 10, pictureBoxRight.Height + 10);
                pictureBoxRight.Image = rightImage;
            }
            else
            {
                rightImage = new Bitmap(leftImage.Width, leftImage.Height);
                rightGraph = Graphics.FromImage(rightImage);
                rightGraph.FillRectangle(Brushes.White, 0, 0, leftImage.Width + 10, leftImage.Height + 10);
                pictureBoxRight.Image = rightImage;
            }
            this.Refresh();
            pictureBoxRight.Refresh();
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
                leftImage = new Bitmap(pictureBoxLeft.Width, pictureBoxLeft.Height);
            }
            else
            {
                leftImage = new Bitmap(width, height);
            }
            Graphics.FromImage(leftImage).FillRectangle(Brushes.White, 0, 0, pictureBoxLeft.Width + 10, pictureBoxLeft.Height + 10);
            pictureBoxLeft.Image = leftImage;
            
            this.Refresh();
            pictureBoxLeft.Refresh();
        }

        /// <summary>
        /// Redraws all lines in lineList, for which their associated boolean value equals true.
        /// </summary>
        private void RedrawRightImage()
        {
            DrawEmptyCanvasRight();
            foreach (Tuple<bool, Line> lineBoolTuple in rightLineList)
            {
                if (lineBoolTuple.Item1)
                {
                    lineBoolTuple.Item2.DrawLine(rightGraph);
                }
            }
            pictureBoxRight.Refresh();
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
        /// returns all instance variables in the order of their definition for testing
        /// </summary>
        /// <returns>all instance variables in the order of their definition</returns>
        public Object[]/*(ProgramState, FileImporter, OpenFileDialog, Image, List<Line>, Image, List<Point>, List<Tuple<bool, Line>>, bool, Point, Point, Queue<Point>, Graphics, bool[,], HashSet<int>[,], uint, ActionHistory)*/ GetAllVariables()
        {
            return new Object[] { currentState, fileImporter, openFileDialog, leftImage, leftLineList, rightImage, currentLine, rightLineList, mousePressed, currentCursorPosition, previousCursorPosition, cursorPositions, rightGraph, isFilledMatrix, linesMatrix, deletionSize, historyOfActions };
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

		private String createSvgTxt()
		{
			String newString = String.Format("<svg viewBox = \"0 0 {0} {1}\" xmlns = \"http://www.w3.org/2000/svg\">", rightImage.Width, rightImage.Height);
			foreach (Tuple<bool, Line> lineTuple in rightLineList)
			{
				if (lineTuple.Item1 == true)
				{
					String nextLine = String.Format("\n" + "<line x1 = \"{0}\" y1 = \"{1}\" x2 = \"{2}\" y2 = \"{3}\" stroke = \"black\">",
													lineTuple.Item2.GetStartPoint().X, lineTuple.Item2.GetStartPoint().Y, lineTuple.Item2.GetEndPoint().X, lineTuple.Item2.GetEndPoint().X);
					newString += nextLine;
				}
			}
			return newString;
		}

	}
}
