using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SketchAssistantWPF
{
    public class MVP_Presenter
    {
        /// <summary>
        /// The View of the MVP-Model, in this case Form1.
        /// </summary>
        MVP_View programView;
        /// <summary>
        /// The Model of the MVP-Model.
        /// </summary>
        MVP_Model programModel;
        /// <summary>
        /// A dictionary connecting the id of an InternalLine with the respective Polyline in the right canvas.
        /// </summary>
        Dictionary<int, Polyline> rightPolyLines;
        /// <summary>
        /// A dictionary connecting the id of an InternalLine with the respective Polyline in the left canvas.
        /// </summary>
        Dictionary<int, Polyline> leftPolyLines;

        ImageDimension CanvasSizeLeft = new ImageDimension(0,0);

        ImageDimension CanvasSizeRight = new ImageDimension(0, 0);

        ImageDimension ImageSizeLeft = new ImageDimension(0, 0);

        ImageDimension ImageSizeRight = new ImageDimension(0, 0);

        /*******************/
        /*** ENUMERATORS ***/
        /*******************/

        public enum MouseAction
        {
            Click,
            Down,
            Up,
            Move
        }

        /***********************/
        /*** CLASS VARIABLES ***/
        /***********************/

        /// <summary>
        /// Instance of FileImporter to handle drawing imports.
        /// </summary>
        private FileImporter fileImporter;


        public MVP_Presenter(MVP_View form)
        {
            programView = form;
            programModel = new MVP_Model(this);
            //Initialize Class Variables
            fileImporter = new FileImporter();
        }

        /***********************************/
        /*** FUNCTIONS VIEW -> PRESENTER ***/
        /***********************************/

        /// <summary>
        /// Pass-trough function to update the appropriate information of the model, when the window is resized.
        /// </summary>
        /// <param name="leftPBS">The new size of the left picture box.</param>
        /// <param name="rightPBS">The new size of the left picture box.</param>
        public void Resize(Tuple<int, int> leftPBS, Tuple<int, int> rightPBS)
        {
            CanvasSizeLeft.ChangeDimension(leftPBS.Item1, leftPBS.Item2);
            CanvasSizeRight.ChangeDimension(rightPBS.Item1, rightPBS.Item2);
            /*
            programModel.leftImageBoxWidth = leftPBS.Item1;
            programModel.leftImageBoxHeight = leftPBS.Item2;
            programModel.rightImageBoxWidth = rightPBS.Item1;
            programModel.rightImageBoxHeight = rightPBS.Item2;
            */
            programModel.UpdateSizes(CanvasSizeRight);
        }

        /// <summary>
        /// Display a new FileDialog to load a collection of lines.
        /// </summary>
        public void ExamplePictureToolStripMenuItemClick()
        {
            var okToContinue = true;
            if (programModel.HasUnsavedProgress())
            {
                okToContinue = programView.ShowWarning("You have unsaved progress. Continue?");
            }
            if (okToContinue)
            {
                var fileNameTup = programView.openNewDialog("Interactive Sketch-Assistant Drawing|*.isad");
                if (!fileNameTup.Item1.Equals("") && !fileNameTup.Item2.Equals(""))
                {
                    programView.SetToolStripLoadStatus(fileNameTup.Item2);
                    Tuple<int, int, List<InternalLine>> values = fileImporter.ParseISADInputFile(fileNameTup.Item1);
                    programModel.SetLeftLineList(values.Item1, values.Item2, values.Item3);
                    programModel.ChangeState(true);
                    programView.EnableTimer();
                }
            }
        }

        /// <summary>
        /// Pass-trough function to change the drawing state of the model.
        /// </summary>
        /// <param name="NowDrawing">Indicates if the program is in drawing (true) or deletion (false) mode.</param>
        public void ChangeState(bool NowDrawing)
        {
            programModel.ChangeState(NowDrawing);
        }

        /// <summary>
        /// Pass-trough function to undo an action.
        /// </summary>
        public void Undo()
        {
            programModel.Undo();
        }

        /// <summary>
        /// Pass-trough function to redo an action.
        /// </summary>
        public void Redo()
        {
            programModel.Redo();
        }

        /// <summary>
        /// Pass-trough function for ticking the model.
        /// </summary>
        public void Tick()
        {
            programModel.Tick();
        }

        /// <summary>
        /// Checks if there is unsaved progress, and promts the model to generate a new canvas if not.
        /// </summary>
        public void NewCanvas()
        {
            var okToContinue = true;
            if (programModel.HasUnsavedProgress())
            {
                okToContinue = programView.ShowWarning("You have unsaved progress. Continue?");
            }
            if (okToContinue)
            {
                programModel.canvasActive = true;
                programModel.ChangeState(true);
                programView.EnableTimer();
            }
        }

        /// <summary>
        /// Pass-trough when the mouse is moved.
        /// </summary>
        /// <param name="mouseAction">The action which is sent by the View.</param>
        /// <param name="e">The Mouse event arguments.</param>
        public void MouseEvent(MouseAction mouseAction, Point position)
        {
            switch (mouseAction)
            {
                case MouseAction.Move:
                    programModel.SetCurrentCursorPosition(ConvertCoordinates(position));
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Pass-trough function that calls the correct Mouse event of the model, when the mouse is clicked.
        /// </summary>
        /// <param name="mouseAction">The action which is sent by the View.</param>
        /// <param name="e">The Mouse event arguments.</param>
        public void MouseEvent(MouseAction mouseAction)
        {
            switch (mouseAction)
            {
                case MouseAction.Click:
                    programModel.MouseDown();
                    programModel.Tick();
                    programModel.MouseUp();
                    break;
                case MouseAction.Down:
                    programModel.MouseDown();
                    break;
                case MouseAction.Up:
                    programModel.MouseUp();
                    break;
                default:
                    break;
            }
        }

        /************************************/
        /*** FUNCTIONS MODEL -> PRESENTER ***/
        /************************************/

        /// <summary>
        /// Updates the currentline
        /// </summary>
        /// <param name="linepoints">The points of the current line.</param>
        public void UpdateCurrentLine(List<Point> linepoints)
        {
            Polyline currentLine = new Polyline();
            currentLine.Stroke = Brushes.Black;
            currentLine.Points = new PointCollection(linepoints);
            programView.DisplayCurrLine(currentLine);
        }

        /// <summary>
        /// Clears all Lines in the right canvas.
        /// </summary>
        public void ClearRightLines()
        {
            programView.RemoveAllRightLines();
            rightPolyLines = new Dictionary<int, Polyline>();
        }

        /// <summary>
        /// A function to update the displayed lines in the right canvas.
        /// </summary>
        public void UpdateRightLines(List<Tuple<bool, InternalLine>> lines)
        {
            foreach(Tuple<bool, InternalLine> tup in lines)
            {
                var status = tup.Item1;
                var line = tup.Item2;
                if (!rightPolyLines.ContainsKey(line.GetID()))
                {
                    Polyline newLine = new Polyline();
                    newLine.Stroke = Brushes.Black;
                    newLine.Points = line.GetPointCollection();
                    rightPolyLines.Add(line.GetID(), newLine);
                    programView.AddNewLineRight(newLine);
                }
                SetVisibility(rightPolyLines[line.GetID()], status);
            }
        }

        /// <summary>
        /// A function to update the displayed lines in the left canvas.
        /// </summary>
        public void UpdateLeftLines(List<InternalLine> lines)
        {
            programView.RemoveAllLeftLines();
            leftPolyLines = new Dictionary<int, Polyline>();
            foreach(InternalLine line in lines)
            {
                Polyline newLine = new Polyline();
                newLine.Stroke = Brushes.Black;
                newLine.Points = line.GetPointCollection();
                leftPolyLines.Add(line.GetID(), newLine);
                programView.AddNewLineLeft(newLine);
            }
        }

        /// <summary>
        /// Called by the model when the state of the Program changes. 
        /// Changes the look of the UI according to the current state of the model.
        /// </summary>
        /// <param name="inDrawingMode">If the model is in Drawing Mode</param>
        /// <param name="canUndo">If actions in the model can be undone</param>
        /// <param name="canRedo">If actions in the model can be redone</param>
        /// <param name="imageLoaded">If an image is loaded in the model</param>
        public void UpdateUIState(bool inDrawingMode, bool canUndo, bool canRedo, bool imageLoaded)
        {
            Dictionary<String, MainWindow.ButtonState> dict = new Dictionary<String, MainWindow.ButtonState> {
                {"canvasButton", MainWindow.ButtonState.Enabled }, {"drawButton", MainWindow.ButtonState.Disabled}, {"deleteButton",MainWindow.ButtonState.Disabled },
                {"undoButton", MainWindow.ButtonState.Disabled },{"redoButton",  MainWindow.ButtonState.Disabled}};

            if (imageLoaded)
            {
                if (inDrawingMode)
                {
                    dict["drawButton"] = MainWindow.ButtonState.Active;
                    dict["deleteButton"] = MainWindow.ButtonState.Enabled;
                }
                else
                {
                    dict["drawButton"] = MainWindow.ButtonState.Enabled;
                    dict["deleteButton"] = MainWindow.ButtonState.Active;
                }
                if (canUndo) { dict["undoButton"] = MainWindow.ButtonState.Enabled; }
                if (canRedo) { dict["redoButton"] = MainWindow.ButtonState.Enabled; }
            }
            foreach (KeyValuePair<String, MainWindow.ButtonState> entry in dict)
            {
                programView.SetToolStripButtonStatus(entry.Key, entry.Value);
            }
        }


        /*
        /// <summary>
        /// Is called by the model when the left image is changed.
        /// </summary>
        /// <param name="lineList">The new image.</param>
        public void UpdateLeftImage(List<InternalLine> lineList)
        {
            programView.DisplayInLeftPictureBox(lineList);
        }

        /// <summary>
        /// Is called by the model when the right image is changed.
        /// </summary>
        /// <param name="lineList">The new image.</param>
        public void UpdateRightImage(List<InternalLine> lineList)
        {
            programView.DisplayInRightPictureBox(lineList);
        }*/

        /// <summary>
        /// Pass-trough function to display an info message in the view.
        /// </summary>
        /// <param name="msg">The message.</param>
        public void PassMessageToView(String msg)
        {
            programView.ShowInfoMessage(msg);
        }

        /// <summary>
        /// Pass-trough function to update the display of the last action taken.
        /// </summary>
        /// <param name="msg">The new last action taken.</param>
        public void PassLastActionTaken(String msg)
        {
            programView.SetLastActionTakenText(msg);
        }

        /*************************/
        /*** HELPING FUNCTIONS ***/
        /*************************/

        /// <summary>
        /// Sets the visibility of a polyline.
        /// </summary>
        /// <param name="line">The polyline</param>
        /// <param name="visible">Whether or not it should be visible.</param>
        private void SetVisibility(Polyline line, bool visible)
        {
            if (visible)
            {
                line.Opacity = 0.00001;
            }
            else
            {
                line.Opacity = 1;
            }
        }

        /// <summary>
        /// A function that calculates the coordinates of a point on a zoomed in image.
        /// </summary>
        /// <param name="">The position of the mouse cursor</param>
        /// <returns>The real coordinates of the mouse cursor on the image</returns>
        private Point ConvertCoordinates(Point cursorPosition)
        {
            if (!programModel.canvasActive) { return cursorPosition; }
            ImageDimension rightImageDimensions = programModel.rightImageSize;
            Point realCoordinates = new Point(0, 0);

            int widthImage = rightImageDimensions.Width;
            int heightImage = rightImageDimensions.Height;
            int widthBox = programModel.rightImageBoxWidth;
            int heightBox = programModel.rightImageBoxHeight;

            if (heightImage == 0 && widthImage == 0)
            {
                return cursorPosition;
            }

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
    }
}
