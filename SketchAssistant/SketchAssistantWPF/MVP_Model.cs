using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using OptiTrack;
using System.Runtime.InteropServices;
using System.IO;

namespace SketchAssistantWPF
{
    public class MVP_Model
    {
        /// <summary>
        /// The Presenter of the MVP-Model.
        /// </summary>
        MVP_Presenter programPresenter;
        /// <summary>
        /// History of Actions
        /// </summary>
        ActionHistory historyOfActions;

        OptiTrackConnector connector;

        /***********************/
        /*** CLASS VARIABLES ***/
        /***********************/

        /// <summary>
        /// If the program is in drawing mode.
        /// </summary>
        bool inDrawingMode;
        /// <summary>
        /// if the program is using OptiTrack
        /// </summary>
        public bool optiTrackInUse { get; private set; }
        /// <summary>
        /// Size of deletion area
        /// </summary>
        int deletionRadius = 5;
        /// <summary>
        /// Size of areas marking endpoints of lines in the redraw mode.
        /// </summary>
        int markerRadius = 10;
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
        /// Lookup Matrix for checking postions of lines in the image
        /// </summary>
        bool[,] isFilledMatrix;
        /// <summary>
        /// Lookup Matrix for getting line ids at a certain postions of the image
        /// </summary>
        HashSet<int>[,] linesMatrix;
        /// <summary>
        /// List of items which will be overlayed over the right canvas.
        /// </summary>
        List<Tuple<bool, HashSet<Point>>> overlayItems;
        /// <summary>
        /// Width of the LeftImageBox.
        /// </summary>
        public int leftImageBoxWidth;
        /// <summary>
        /// Height of the LeftImageBox.
        /// </summary>
        public int leftImageBoxHeight;
        /// <summary>
        /// Width of the RightImageBox.
        /// </summary>
        public int rightImageBoxWidth;
        /// <summary>
        /// Height of the RightImageBox.
        /// </summary>
        public int rightImageBoxHeight;

        public ImageDimension leftImageSize { get; private set; }

        public ImageDimension rightImageSize { get; private set; }
        /// <summary>
        /// Indicates whether or not the canvas on the right side is active.
        /// </summary>
        public bool canvasActive { get; set; }
        /// <summary>
        /// Indicates if there is a graphic loaded in the left canvas.
        /// </summary>
        public bool graphicLoaded { get; set; }
        /// <summary>
        /// Whether or not an optitrack system is avaiable.
        /// </summary>
        public bool optitrackAvailable { get; private set; }

        //TODO: calibrate
        double OPTITRACK_X_OFFSET = 0.7878;
        double OPTITRACK_Y_OFFSET = 0.7977;
        double OPTITRACK_CANVAS_HEIGHT = 1.29;
        double OPTITRACK_X_SCALE = -0.254 * (((1.816 / 0.0254) * 96) / (1.816));
        double OPTITRACK_Y_SCALE = 0.254 * (((1.360 / 0.0254) * 96) / (1.360));


        Image rightImageWithoutOverlay;
        /// Whether or not the mouse is pressed.
        /// </summary>
        private bool mouseDown;

        List<InternalLine> leftLineList;

        List<Tuple<bool, InternalLine>> rightLineList;

        List<Point> currentLine = new List<Point>();

        public MVP_Model(MVP_Presenter presenter)
        {
            programPresenter = presenter;
            historyOfActions = new ActionHistory();
            //redrawAss = new RedrawAssistant();
            //overlayItems = new List<Tuple<bool, HashSet<Point>>>();
            rightLineList = new List<Tuple<bool, InternalLine>>();
            canvasActive = false;
            UpdateUI();
            rightImageSize = new ImageDimension(0, 0);
            leftImageSize = new ImageDimension(0, 0);
            connector = new OptiTrackConnector();
            armband = new Armband();

            optitrackAvailable = false;
            if (File.Exists(@"C:\Users\videowall-pc-user\Documents\BP-SketchAssistant\SketchAssistant\optitrack_setup.ttp"))
            {
                if (connector.Init(@"C:\Users\videowall-pc-user\Documents\BP-SketchAssistant\SketchAssistant\optitrack_setup.ttp"))
                {
                    optitrackAvailable = true;
                    connector.StartTracking(getOptiTrackPosition);
                }
            }
        }

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        private static extern bool SetCursorPos(int X, int Y);

        //[DllImport("user32.dll")]
        //public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public enum MouseEventType : int
        {
            LeftDown = 0x02,
            LeftUp = 0x04,
            RightDown = 0x08,
            RightUp = 0x10
        }


        /**************************/
        /*** INTERNAL FUNCTIONS ***/
        /**************************/


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
                    rightLineList[lineId] = new Tuple<bool, InternalLine>(shown, rightLineList[lineId].Item2);
                }
            }
        }

        /// <summary>
        /// A function that populates the matrixes needed for deletion detection with line data.
        /// </summary>
        private void RepopulateDeletionMatrixes()
        {
            if (canvasActive)
            {
                isFilledMatrix = new bool[rightImageSize.Width, rightImageSize.Height];
                linesMatrix = new HashSet<int>[rightImageSize.Width, rightImageSize.Height];
                foreach (Tuple<bool, InternalLine> lineTuple in rightLineList)
                {
                    if (lineTuple.Item1)
                    {
                        lineTuple.Item2.PopulateMatrixes(isFilledMatrix, linesMatrix);
                    }
                }
            }
        }

        /// <summary>
        /// Tells the Presenter to Update the UI
        /// </summary>
        private void UpdateUI()
        {
            programPresenter.UpdateUIState(inDrawingMode, historyOfActions.CanUndo(), historyOfActions.CanRedo(), canvasActive, graphicLoaded, optitrackAvailable, optiTrackInUse);
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

            foreach (Point pnt in GeometryCalculator.FilledCircleAlgorithm(p, (int)range))
            {
                if (pnt.X >= 0 && pnt.Y >= 0 && pnt.X < rightImageSize.Width && pnt.Y < rightImageSize.Height)
                {
                    if (isFilledMatrix[(int)pnt.X, (int)pnt.Y])
                    {
                        returnSet.UnionWith(linesMatrix[(int)pnt.X, (int)pnt.Y]);
                    }
                }
            }
            return returnSet;
        }

        /********************************************/
        /*** FUNCTIONS TO INTERACT WITH PRESENTER ***/
        /********************************************/

        /// <summary>
        /// A function to update the dimensions of the left and right canvas when the window is resized.
        /// </summary>
        /// <param name="LeftCanvas">The size of the left canvas.</param>
        /// <param name="RightCanvas">The size of the right canvas.</param>
        public void ResizeEvent(ImageDimension LeftCanvas, ImageDimension RightCanvas)
        {
            if (LeftCanvas.Height >= 0 && LeftCanvas.Width >= 0) { leftImageSize = LeftCanvas; }
            if (RightCanvas.Height >= 0 && RightCanvas.Width >= 0) { rightImageSize = RightCanvas; }

            RepopulateDeletionMatrixes();
        }

        /// <summary>
        /// A function to reset the right image.
        /// </summary>
        public void ResetRightImage()
        {
            rightLineList.Clear();
            programPresenter.PassLastActionTaken(historyOfActions.Reset());
            programPresenter.ClearRightLines();
        }

        /// <summary>
        /// The function to set the left image.
        /// </summary>
        /// <param name="width">The width of the left image.</param>
        /// <param name="height">The height of the left image.</param>
        /// <param name="listOfLines">The List of Lines to be displayed in the left image.</param>
        public void SetLeftLineList(int width, int height, List<InternalLine> listOfLines)
        {
            leftImageSize = new ImageDimension(width, height);
            rightImageSize = new ImageDimension(width, height);
            leftLineList = listOfLines;
            graphicLoaded = true;
            programPresenter.UpdateLeftLines(leftLineList);
            CanvasActivated();
            /*
            var workingCanvas = GetEmptyCanvas(width, height);
            var workingGraph = Graphics.FromImage(workingCanvas);
            leftLineList = listOfLines;
            //redrawAss = new RedrawAssistant(leftLineList);
            //overlayItems = redrawAss.Initialize(markerRadius);
            //Lines
            foreach (InternalLine line in leftLineList)
            {
                line.DrawLine(workingGraph);
            }
            leftImage = workingCanvas;
            programPresenter.UpdateLeftImage(leftImage);
            //Set right image to same size as left image and delete linelist
            DrawEmptyCanvasRight();
            rightLineList = new List<Tuple<bool, InternalLine>>();
            */
        }

        /// <summary>
        /// A function to tell the model a new canvas was activated.
        /// </summary>
        public void CanvasActivated()
        {
            canvasActive = true;
            RepopulateDeletionMatrixes();
            UpdateUI();
        }

        /// <summary>
        /// Will undo the last action taken, if the action history allows it.
        /// </summary>
        public void Undo()
        {
            if (historyOfActions.CanUndo())
            {
                HashSet<int> affectedLines = historyOfActions.GetCurrentAction().GetLineIDs();
                SketchAction.ActionType undoAction = historyOfActions.GetCurrentAction().GetActionType();
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
                //TODO: For the person implementing overlay: Add check if overlay needs to be added
                programPresenter.UpdateRightLines(rightLineList);
            }
            RepopulateDeletionMatrixes();
            programPresenter.PassLastActionTaken(historyOfActions.MoveAction(true));
            UpdateUI();
        }

        /// <summary>
        /// Will redo the last action undone, if the action history allows it.
        /// </summary>
        public void Redo()
        {
            if (historyOfActions.CanRedo())
            {
                programPresenter.PassLastActionTaken(historyOfActions.MoveAction(false));
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
                //TODO: For the person implementing overlay: Add check if overlay needs to be added
                programPresenter.UpdateRightLines(rightLineList);
                RepopulateDeletionMatrixes();
            }
            UpdateUI();
        }

        /// <summary>
        /// The function called by the Presenter to change the drawing state of the program.
        /// </summary>
        /// <param name="nowDrawing">The new drawingstate of the program</param>
        public void ChangeState(bool nowDrawing)
        {
            inDrawingMode = nowDrawing;
            UpdateUI();
        }

        /// <summary>
        /// The function called by the Presenter to set a variable which describes if OptiTrack is in use
        /// </summary>
        /// <param name="usingOptiTrack"></param>
        public void SetOptiTrack(bool usingOptiTrack)
        {
            optiTrackInUse = usingOptiTrack;
            if (usingOptiTrack && optiTrackX == 0 && optiTrackY == 0 && optiTrackZ == 0)
            {
                programPresenter.PassMessageToView("Trackable not detected, please check if OptiTrack is activated and Trackable is recognized", "Warning");
            }
        }

        /// <summary>
        /// Updates the current cursor position of the model.
        /// </summary>
        /// <param name="p">The new cursor position</param>
        public void SetCurrentCursorPosition(Point p)
        {
            if (!optiTrackInUse) currentCursorPosition = p;
            mouseDown = programPresenter.IsMousePressed();
        }

        /// <summary>
        /// Updates the current cursor position of the model.
        /// </summary>
        /// <param name="p">The new cursor position</param>
        public void SetCurrentFingerPosition(Point p)
        {
            Console.WriteLine("raw coordinates: " + p.X + ";" + p.Y);
            Point correctedPoint = ConvertTo96thsOfInch(p);
            Console.WriteLine(correctedPoint.X + "," + correctedPoint.Y);
            currentCursorPosition = correctedPoint;
        }


        private Point ConvertTo96thsOfInch(Point p)
        {
            double xCoordinate = (p.X - OPTITRACK_X_OFFSET) * OPTITRACK_X_SCALE;
            double yCoordinate = (OPTITRACK_CANVAS_HEIGHT - (p.Y - OPTITRACK_Y_OFFSET)) * OPTITRACK_Y_SCALE;
            return new Point(xCoordinate, yCoordinate);
        }

        private Point ConvertToPixel(Point p)
        {
            double xCoordinate = (p.X - OPTITRACK_X_OFFSET) * -1 * (/*Anzahl Pixel X-Richtung*/0 / (1.816)) + 0/*1/2 * x-richtung pixel*/; //TODO
            double yCoordinate = (((OPTITRACK_CANVAS_HEIGHT + 0/*meter von oberer Rand Leinwand zu oberer Rand Bildschirm*/) - (p.Y - OPTITRACK_Y_OFFSET))) * (/*Anzahl Pixel Y-Richtung*/0 / (1.360));
            return new Point(xCoordinate, yCoordinate);
        }

        /// <summary>
        /// Start a new Line, when the Mouse is pressed down.
        /// </summary>
        public void StartNewLine()
        {
            if (optiTrackInUse || programPresenter.IsMousePressed())
            {
                if (inDrawingMode)
                {
                    currentLine.Clear();
                    currentLine.Add(currentCursorPosition);
                }
            }
        }

        /// <summary>
        /// Finish the current Line, when the pressed Mouse is released.
        /// </summary>
        /// <param name="valid">Whether the up event is valid or not</param>
        public void FinishCurrentLine(bool valid)
        {
            foreach (Point p in currentLine)
            {
                Console.WriteLine(p.X + ";" + p.Y);
            }
            if (valid)
            {
                if (inDrawingMode && currentLine.Count > 0)
                {
                    InternalLine newLine = new InternalLine(currentLine, rightLineList.Count);
                    rightLineList.Add(new Tuple<bool, InternalLine>(true, newLine));
                    newLine.PopulateMatrixes(isFilledMatrix, linesMatrix);
                    programPresenter.PassLastActionTaken(historyOfActions.AddNewAction(new SketchAction(SketchAction.ActionType.Draw, newLine.GetID())));
                    //TODO: For the person implementing overlay: Add check if overlay needs to be added
                    programPresenter.UpdateRightLines(rightLineList);
                    currentLine.Clear();
                    programPresenter.UpdateCurrentLine(currentLine);
                }
            }
            else
            {
                currentLine.Clear();
            }
            UpdateUI();
        }

        public float optiTrackX;
        public float optiTrackY;
        public float optiTrackZ;
        private bool optiTrackInsideDrawingZone = false;
        private double WARNING_ZONE_BOUNDARY = 0.10; //5cm
        private Armband armband;

        void getOptiTrackPosition(OptiTrack.Frame frame)
        {
            optiTrackX = frame.Trackables[0].X;
            optiTrackY = frame.Trackables[0].Y;
            optiTrackZ = frame.Trackables[0].Z;
        }
        /// <summary>
        /// Method to be called every tick. Updates the current Line, or checks for Lines to delete, depending on the drawing mode.
        /// </summary>
        public void Tick()
        {
            if (optiTrackInUse)
            {
                if (CheckInsideDrawingZone(optiTrackZ))
                {
                    SetCurrentFingerPosition(new Point(optiTrackX, optiTrackY));
                    if (!optiTrackInsideDrawingZone)
                    {
                        optiTrackInsideDrawingZone = true;
                        StartNewLine();
                        Console.WriteLine("new line begun");
                    }
                    if (optiTrackZ > WARNING_ZONE_BOUNDARY)
                    {
                        armband.pushForward();
                    }
                    else if (optiTrackZ < -1 * WARNING_ZONE_BOUNDARY)
                    {
                        armband.pushBackward();
                    }
                }
                else
                {
                    if (optiTrackInsideDrawingZone)
                    {
                        optiTrackInsideDrawingZone = false;
                        FinishCurrentLine(true);
                        Console.WriteLine("line finished");
                    }
                }
                if(optiTrackX != 0 && optiTrackY != 0 && optiTrackZ != 0) {
                    projectPointOntoScreen(optiTrackX, optiTrackY);
                }
            }
            if (cursorPositions.Count > 0) { previousCursorPosition = cursorPositions.Dequeue(); }
            else { previousCursorPosition = currentCursorPosition; }
            cursorPositions.Enqueue(currentCursorPosition);

            //Drawing
            if (optiTrackInUse)
            {
                if (CheckInsideDrawingZone(optiTrackZ))
                {
                    Console.WriteLine("point added");
                    currentLine.Add(currentCursorPosition);
                    programPresenter.UpdateCurrentLine(currentLine);
                }
            }
            else if (inDrawingMode && programPresenter.IsMousePressed())
            {
                currentLine.Add(currentCursorPosition);
                //programPresenter.UpdateCurrentLine(currentLine);
            }
            //Deleting
            if (!inDrawingMode && programPresenter.IsMousePressed())
            {
                List<Point> uncheckedPoints = GeometryCalculator.BresenhamLineAlgorithm(previousCursorPosition, currentCursorPosition);

                foreach (Point currPoint in uncheckedPoints)
                {
                    HashSet<int> linesToDelete = CheckDeletionMatrixesAroundPoint(currPoint, deletionRadius);
                    if (linesToDelete.Count > 0)
                    {
                        programPresenter.PassLastActionTaken(historyOfActions.AddNewAction(new SketchAction(SketchAction.ActionType.Delete, linesToDelete)));
                        foreach (int lineID in linesToDelete)
                        {
                            rightLineList[lineID] = new Tuple<bool, InternalLine>(false, rightLineList[lineID].Item2);
                        }
                        RepopulateDeletionMatrixes();
                        //TODO: For the person implementing overlay: Add check if overlay needs to be added
                        programPresenter.UpdateRightLines(rightLineList);
                    }
                }
            }
        }

        private void projectPointOntoScreen(float optiTrackX, float optiTrackY)
        {
            Point auxiliaryPoint = ConvertToPixel(new Point(optiTrackX, optiTrackY));
            SetCursorPos((int)auxiliaryPoint.X, (int)auxiliaryPoint.Y);
        }

        private bool CheckInsideDrawingZone(float optiTrackZ)
        {
            if (Math.Abs(optiTrackZ) > WARNING_ZONE_BOUNDARY * 2) return false;
            return true;
        }

        /*
        /// <summary>
        /// A helper Function that updates the markerRadius & deletionRadius, considering the size of the canvas.
        /// </summary>
        /// <param name="CanvasSize">The size of the canvas</param>
        public void UpdateSizes(ImageDimension CanvasSize)
        {
            if (rightImageWithoutOverlay != null)
            {
                int widthImage = rightImageSize.Width;
                int heightImage = rightImageSize.Height;
                int widthBox = CanvasSize.Width;
                int heightBox = CanvasSize.Height;

                float imageRatio = (float)widthImage / (float)heightImage;
                float containerRatio = (float)widthBox / (float)heightBox;
                float zoomFactor = 0;
                if (imageRatio >= containerRatio)
                {
                    //Image is wider than it is high
                    zoomFactor = (float)widthImage / (float)widthBox;
                }
                else
                {
                    //Image is higher than it is wide
                    zoomFactor = (float)heightImage / (float)heightBox;
                }
                markerRadius = (int)(10 * zoomFactor);
                deletionRadius = (int)(5 * zoomFactor);
            }
        }
        */

        /// <summary>
        /// If there is unsaved progress.
        /// </summary>
        /// <returns>True if there is progress that has not been saved.</returns>
        public bool HasUnsavedProgress()
        {
            return !historyOfActions.IsEmpty();
        }
    }
}
