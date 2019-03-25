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
        /// <summary>
        /// The connector class used to get frames from the Optitrack system.
        /// </summary>
        OptiTrackConnector connector;



        /***********************/
        /*** CLASS VARIABLES ***/
        /***********************/

        /// <summary>
        /// this is a variable used for detecting whether the tracker is in the warning zone (0 +- variable), no drawing zone (0 +- 2 * variable) or normal drawing zone
        /// </summary>
        readonly double WARNING_ZONE_BOUNDARY = 0.10; //10cm
        /// <summary>
        /// If the program is in drawing mode.
        /// </summary>
        public bool inDrawingMode { get; private set; }
        /// <summary>
        /// if the program is using OptiTrack
        /// </summary>
        public bool optiTrackInUse { get; private set; }
        /// <summary>
        /// Size of deletion area
        /// </summary>
        int deletionRadius = 5;
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
        /// The Position of the Cursor of opti track
        /// </summary>
        Point currentOptiCursorPosition;
        /// <summary>
        /// The Previous Cursor Position of opti track
        /// </summary>
        Point previousOptiCursorPosition;
        /// <summary>
        /// Queue for the cursorPositions of opti track
        /// </summary>
        Queue<Point> optiCursorPositions = new Queue<Point>();
        /// <summary>
        /// Lookup Matrix for checking postions of lines in the image
        /// </summary>
        bool[,] isFilledMatrix;
        /// <summary>
        /// Lookup Matrix for getting line ids at a certain postions of the image
        /// </summary>
        HashSet<int>[,] linesMatrix;
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
        /// <summary>
        /// The size of the right canvas.
        /// </summary>
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
        /// <summary>
        /// x coordinate in real world. one unit is one meter. If standing in front of video wall facing it, moving left results in incrementation of x.
        /// </summary>
        public float optiTrackX;
        /// <summary>
        /// y coordinate in real world. one unit is one meter. If standing in front of video wall, moving up results in incrementation of y.
        /// </summary>
        public float optiTrackY;
        /// <summary>
        /// z coordinate in real world. one unit is one meter. If standing in front of video wall, moving back results in incrementation of y.
        /// </summary>
        public float optiTrackZ;
        /// <summary>
        /// keeps track of whether last tick the trackable was inside drawing zone or not.
        /// </summary>
        private bool optiTrackInsideDrawingZone = false;
        /// <summary>
        /// object of class wristband used for controlling the vibrotactile wristband
        /// </summary>
        private Wristband wristband;
        /// <summary>
        /// Is set to true when the trackable has passed to the backside of the drawing surface, 
        /// invalidating all inputs on its way back.
        /// </summary>
        bool OptiMovingBack = false;
        /// <summary>
        /// The Layer in which the optitrack system was. 0 is drawing layer, -1 is in front, 1 is behind
        /// </summary>
        int OptiLayer = 0;
        /// <summary>
        /// The path traveled since the last tick
        /// </summary>
        double PathTraveled = 0;
        /// <summary>
        /// Whether or not the mouse is pressed.
        /// </summary>
        private bool mouseDown;
        /// <summary>
        /// A List of lines in the left canvas.
        /// </summary>
        List<InternalLine> leftLineList;
        /// <summary>
        /// A list of lines in the right canvas along with a boolean indicating if they should be drawn.
        /// </summary>
        List<Tuple<bool, InternalLine>> rightLineList;
        /// <summary>
        /// The line currently being drawin with optitrack.
        /// </summary>
        List<Point> currentLine = new List<Point>();

        public MVP_Model(MVP_Presenter presenter)
        {
            programPresenter = presenter;
            historyOfActions = new ActionHistory();
            rightLineList = new List<Tuple<bool, InternalLine>>();
            canvasActive = false;
            UpdateUI();
            rightImageSize = new ImageDimension(0, 0);
            connector = new OptiTrackConnector();
            wristband = new Wristband();

            //Set up Optitrack
            optitrackAvailable = false;
            if (File.Exists(@"C:\Users\videowall-pc-user\Documents\BP-SketchAssistant\SketchAssistant\optitrack_setup.ttp"))
            {
                if (connector.Init(@"C:\Users\videowall-pc-user\Documents\BP-SketchAssistant\SketchAssistant\optitrack_setup.ttp"))
                {
                    optitrackAvailable = true;
                    connector.StartTracking(GetOptiTrackPosition);
                }
            }
        }

        /**************************/
        /*** INTERNAL FUNCTIONS ***/
        /**************************/


        /// <summary>
        /// Check if the Optitrack trackable is in the drawing plane.
        /// </summary>
        /// <param name="optiTrackZ">The real world z coordinates of the trackable.</param>
        /// <returns>If the trackable is in front of the drawing plane</returns>
        private bool CheckInsideDrawingZone(float optiTrackZ)
        {
            if (Math.Abs(optiTrackZ) > WARNING_ZONE_BOUNDARY * 2) return false;
            return true;
        }

        /// <summary>
        /// Function that is called by the OptitrackController to pass frames to the model.
        /// </summary>
        /// <param name="frame">An Optitrack Frame</param>
        void GetOptiTrackPosition(OptiTrack.Frame frame)
        {
            if (frame.Trackables.Length >= 1)
            {
                optiTrackX = frame.Trackables[0].X;
                optiTrackY = frame.Trackables[0].Y;
                optiTrackZ = frame.Trackables[0].Z;
            }
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
                    rightLineList[lineId] = new Tuple<bool, InternalLine>(shown, rightLineList[lineId].Item2);
                }
            }
        }


        /// <summary>
        /// Check if enough distance has been travelled to warrant a vibration.
        /// </summary>
        private void CheckPathTraveled()
        {
            var a = Math.Abs(previousOptiCursorPosition.X - currentOptiCursorPosition.X);
            var b = Math.Abs(previousOptiCursorPosition.Y - currentOptiCursorPosition.Y);
            PathTraveled += Math.Sqrt(Math.Pow(a,2) + Math.Pow(b,2));
            //Set the Interval of vibrations here
            if(PathTraveled > 2)
            {
                PathTraveled = 0; 
                //TODO: Activate vibration here
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

        /// <summary>
        /// Converts given point to device-independent pixel.
        /// </summary>
        /// <param name="p">real world coordinate</param>
        /// <returns>The given Point converted to device-independent pixel </returns>
        private Point ConvertTo96thsOfInch(Point p)
        {
            //The position of the optitrack coordinate system
            // and sizes of the optitrack tracking area relative to the canvas.
            double OPTITRACK_X_OFFSET = -0.4854;
            double OPTITRACK_Y_OFFSET = 0.9;
            double OPTITRACK_CANVAS_HEIGHT = 1.2;
            double OPTITRACK_X_SCALE = 0.21 * (((1.8316/*size of canvas*/ / 0.0254) * 96) / (1.8316));
            double OPTITRACK_Y_SCALE = 0.205 * (((1.2 / 0.0254) * 96) / (1.2));
            //The coordinates on the display
            double xCoordinate = (p.X - OPTITRACK_X_OFFSET) * OPTITRACK_X_SCALE;
            double yCoordinate = (OPTITRACK_CANVAS_HEIGHT - (p.Y - OPTITRACK_Y_OFFSET)) * OPTITRACK_Y_SCALE;
            return new Point(xCoordinate, yCoordinate);
        }

        /// <summary>
        /// Updates the Optitrack coordiantes, aswell as the marker for optitrack.
        /// </summary>
        /// <param name="p">The new cursor position</param>
        private void SetCurrentFingerPosition(Point p)
        {
            Point correctedPoint = ConvertTo96thsOfInch(p);


            if (optiTrackZ < -2.2 * WARNING_ZONE_BOUNDARY && OptiLayer > -1)
            {
                OptiLayer = -1; OptiMovingBack = false;
                programPresenter.SetOverlayColor("optipoint", Brushes.Yellow);
            }
            else if (optiTrackZ > 2 * WARNING_ZONE_BOUNDARY && OptiLayer < 1)
            {
                programPresenter.SetOverlayColor("optipoint", Brushes.Red);
                OptiLayer = 1;
            }
            else if(optiTrackZ <= 2 * WARNING_ZONE_BOUNDARY && optiTrackZ >= -2.2 * WARNING_ZONE_BOUNDARY){
                if(OptiLayer > 0)
                    OptiMovingBack = true;
                programPresenter.SetOverlayColor("optipoint", Brushes.Green);
                OptiLayer = 0;
            }

            currentOptiCursorPosition = correctedPoint;
            programPresenter.MoveOptiPoint(currentOptiCursorPosition);

            if (optiCursorPositions.Count > 0) { previousOptiCursorPosition = optiCursorPositions.Dequeue(); }
            else { previousOptiCursorPosition = currentOptiCursorPosition; }
            optiCursorPositions.Enqueue(currentOptiCursorPosition);
        }

        /********************************************/
        /*** FUNCTIONS TO INTERACT WITH PRESENTER ***/
        /********************************************/

        /// <summary>
        /// A function to update the dimensions of the left and right canvas when the window is resized.
        /// </summary>
        /// <param name="RightCanvas">The size of the right canvas.</param>
        public void ResizeEvent(ImageDimension RightCanvas)
        {
            if (RightCanvas.Height >= 0 && RightCanvas.Width >= 0) { rightImageSize = RightCanvas; }
            RepopulateDeletionMatrixes();
        }

        /// <summary>
        /// A function to reset the right image.
        /// </summary>
        public void ResetRightImage()
        {
            if(currentLine.Count > 0)
                FinishCurrentLine(true);
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
            rightImageSize = new ImageDimension(width, height);
            leftLineList = listOfLines;
            graphicLoaded = true;
            programPresenter.UpdateLeftLines(leftLineList);
            CanvasActivated();
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
            if(inDrawingMode && !nowDrawing && currentLine.Count > 0 && optiTrackInUse)
                FinishCurrentLine(true);
            inDrawingMode = nowDrawing;
            UpdateUI();
        }

        /// <summary>
        /// The function called by the Presenter to set a variable which describes if OptiTrack is in use
        /// </summary>
        /// <param name="usingOptiTrack">The status of optitrack button</param>
        public void SetOptiTrack(bool usingOptiTrack)
        {
            optiTrackInUse = usingOptiTrack;
            if (usingOptiTrack && optiTrackX == 0 && optiTrackY == 0 && optiTrackZ == 0)
            {
                programPresenter.PassWarning("Trackable not detected, please check if OptiTrack is activated and Trackable is recognized");
                optiTrackInUse = false;
                //Disable optipoint
                programPresenter.SetOverlayStatus("optipoint", false, currentCursorPosition);
            }
            else
            {
                //Enable optipoint
                programPresenter.SetOverlayStatus("optipoint", true, currentCursorPosition);
            }
        }

        /// <summary>
        /// Updates the current cursor position of the model.
        /// </summary>
        /// <param name="p">The new cursor position</param>
        public void SetCurrentCursorPosition(Point p)
        {
            currentCursorPosition = p;
            mouseDown = programPresenter.IsMousePressed();
        }

        /// <summary>
        /// Start a new Line, when the Mouse is pressed down.
        /// </summary>
        public void StartNewLine()
        {
            mouseDown = true;
            if (inDrawingMode)
            {
                if(optiTrackInUse)
                {
                    currentLine.Clear();
                    currentLine.Add(currentOptiCursorPosition);
                }
                else if (programPresenter.IsMousePressed())
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

            mouseDown = false;
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

        /// <summary>
        /// Finish the current Line, when the pressed Mouse is released.
        /// Overload that is used to pass a list of points to be used when one is available.
        /// </summary>
        /// <param name="p">The list of points</param>
        public void FinishCurrentLine(List<Point> p)
        {
            mouseDown = false;
            if (inDrawingMode && currentLine.Count > 0)
            {
                InternalLine newLine = new InternalLine(p, rightLineList.Count);
                rightLineList.Add(new Tuple<bool, InternalLine>(true, newLine));
                newLine.PopulateMatrixes(isFilledMatrix, linesMatrix);
                programPresenter.PassLastActionTaken(historyOfActions.AddNewAction(new SketchAction(SketchAction.ActionType.Draw, newLine.GetID())));
                programPresenter.UpdateRightLines(rightLineList);
                currentLine.Clear();
            }
            UpdateUI();
        }

        /// <summary>
        /// Method to be called every tick. Updates the current Line, or checks for Lines to delete, depending on the drawing mode.
        /// </summary>
        public void Tick()
        {
            if (cursorPositions.Count > 0) { previousCursorPosition = cursorPositions.Dequeue(); }
            else { previousCursorPosition = currentCursorPosition; }

            if(optitrackAvailable)
                SetCurrentFingerPosition(new Point(optiTrackX, optiTrackY));
            

            if (optiTrackInUse && inDrawingMode && !OptiMovingBack) // optitrack is being used
            {
                //outside of drawing zone
                if (!CheckInsideDrawingZone(optiTrackZ))
                {
                    //Check if trackable was in drawing zone last tick & program is in drawing mode-> finish line
                    if (optiTrackInsideDrawingZone && inDrawingMode) 
                    {
                        optiTrackInsideDrawingZone = false;
                        FinishCurrentLine(true);
                    }
                }
                else //Draw with optitrack, when in drawing zone
                {
                    //Optitrack wasn't in the drawing zone last tick -> start a new line
                    if (!optiTrackInsideDrawingZone)
                    {
                        optiTrackInsideDrawingZone = true;
                        StartNewLine();
                    }
                    else currentLine.Add(currentOptiCursorPosition);
                    programPresenter.UpdateCurrentLine(currentLine);
                    if (optiTrackZ > WARNING_ZONE_BOUNDARY)
                    {
                        wristband.PushForward();
                    }
                    else if (optiTrackZ < -1 * WARNING_ZONE_BOUNDARY)
                    {
                        wristband.PushBackward();
                    }
                }
            }
            else if( !optiTrackInUse && inDrawingMode)
            {
                //drawing without optitrack
                cursorPositions.Enqueue(currentCursorPosition);
                if (inDrawingMode && programPresenter.IsMousePressed())
                {
                    currentLine.Add(currentCursorPosition);
                    //programPresenter.UpdateCurrentLine(currentLine);
                }

            }

            //Deletion mode for optitrack and regular use
            if (!inDrawingMode)
            {
                List<Point> uncheckedPoints = new List<Point>();
                if (programPresenter.IsMousePressed() && !optiTrackInUse) //without optitrack
                    uncheckedPoints = GeometryCalculator.BresenhamLineAlgorithm(previousCursorPosition, currentCursorPosition);
                if(optiTrackInUse && CheckInsideDrawingZone(optiTrackZ)  && !OptiMovingBack) //with optitrack
                    uncheckedPoints = GeometryCalculator.BresenhamLineAlgorithm(previousOptiCursorPosition, currentOptiCursorPosition);

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
                        programPresenter.UpdateRightLines(rightLineList);
                    }
                }
            }
        }

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
