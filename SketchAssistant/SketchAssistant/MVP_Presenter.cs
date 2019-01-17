using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace SketchAssistant
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

        public void Resize(Tuple<int, int> leftPBS,Tuple<int, int> rightPBS)
        {
            programModel.leftImageBoxWidth = leftPBS.Item1;
            programModel.leftImageBoxHeight = leftPBS.Item2;
            programModel.rightImageBoxWidth = rightPBS.Item1;
            programModel.rightImageBoxHeight = rightPBS.Item2;
            programModel.UpdateSizes();
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
                    (int, int, List<Line>) values = fileImporter.ParseISADInputFile(fileNameTup.Item1);
                    programModel.SetLeftLineList(values.Item1, values.Item2, values.Item3);
                    programModel.ChangeState(true);
                    programView.EnableTimer();
                }
            }
        }

        public void ChangeState(bool NowDrawing)
        {
            programModel.ChangeState(NowDrawing);
        }

        public void Undo()
        {
            programModel.Undo();
        }

        public void Redo()
        {
            programModel.Redo();
        }

        public void NewCanvas()
        {
            var okToContinue = true;
            if (programModel.HasUnsavedProgress())
            {
                okToContinue = programView.ShowWarning("You have unsaved progress. Continue?");
            }
            if (okToContinue)
            {
                programModel.DrawEmptyCanvasRight();
                programModel.ChangeState(true);
                programView.EnableTimer();
            }
        }

        public void Tick()
        {
            programModel.Tick();
        }

        public void MouseEvent(MouseAction mouseAction, MouseEventArgs e)
        {
            switch (mouseAction)
            {
                case MouseAction.Click:
                    break;
                case MouseAction.Down:
                    programModel.MouseDown();
                    break;
                case MouseAction.Up:
                    programModel.MouseUp();
                    break;
                case MouseAction.Move:
                    programModel.SetCurrentCursorPosition(ConvertCoordinates(new Point(e.X, e.Y)));
                    break;
            }
        }

        /************************************/
        /*** FUNCTIONS MODEL -> PRESENTER ***/
        /************************************/

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
            Dictionary<String, Form1.ButtonState> dict = new Dictionary<String, Form1.ButtonState> {
                {"canvasButton", Form1.ButtonState.Enabled }, {"drawButton", Form1.ButtonState.Disabled}, {"deleteButton",Form1.ButtonState.Disabled },
                {"undoButton", Form1.ButtonState.Disabled },{"redoButton",  Form1.ButtonState.Disabled}};

            if (imageLoaded)
            {
                if (inDrawingMode)
                {
                    dict["drawButton"] = Form1.ButtonState.Active;
                    dict["deleteButton"] = Form1.ButtonState.Enabled;
                }
                else
                {
                    dict["drawButton"] = Form1.ButtonState.Enabled;
                    dict["deleteButton"] = Form1.ButtonState.Active;
                }
                if (canUndo){dict["undoButton"] = Form1.ButtonState.Enabled;}
                if (canRedo){dict["redoButton"] = Form1.ButtonState.Enabled;}
            }
            foreach(KeyValuePair<String, Form1.ButtonState> entry in dict)
            {
                programView.SetToolStripButtonStatus(entry.Key, entry.Value);
            }
        }

        /// <summary>
        /// Is called by the model when the left image is changed.
        /// </summary>
        /// <param name="img">The new image.</param>
        public void UpdateLeftImage(Image img)
        {
            programView.DisplayInLeftPictureBox(img);
        }

        /// <summary>
        /// Is called by the model when the right image is changed.
        /// </summary>
        /// <param name="img">The new image.</param>
        public void UpdateRightImage(Image img)
        {
            programView.DisplayInRightPictureBox(img);
        }

        public void PassMessageToView(String msg)
        {
            programView.ShowInfoMessage(msg);
        }

        public void PassLastActionTaken(String msg)
        {
            programView.SetLastActionTakenText(msg);
        }

        /*************************/
        /*** HELPING FUNCTIONS ***/
        /*************************/

        /// <summary>
        /// A function that calculates the coordinates of a point on a zoomed in image.
        /// </summary>
        /// <param name="">The position of the mouse cursor</param>
        /// <returns>The real coordinates of the mouse cursor on the image</returns>
        private Point ConvertCoordinates(Point cursorPosition)
        {
            var rightImageDimensions = programModel.GetRightImageDimensions();
            Point realCoordinates = new Point(0, 0);

            int widthImage = rightImageDimensions.Item1;
            int heightImage = rightImageDimensions.Item2;
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
