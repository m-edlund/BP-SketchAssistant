using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SketchAssistant
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
        /// The assistant responsible for the redraw mode
        /// </summary>
        RedrawAssistant redrawAss;

        /*******************/
        /*** ENUMERATORS ***/
        /*******************/

        /***********************/
        /*** CLASS VARIABLES ***/
        /***********************/

        /// <summary>
        /// If the program is in drawing mode.
        /// </summary>
        bool inDrawingMode;
        /// <summary>
        /// If the mouse is currently pressed or not.
        /// </summary>
        bool mousePressed;
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

        //Images
        Image leftImage;

        List<Line> leftLineList;

        Image rightImageWithoutOverlay;

        Image rightImageWithOverlay;

        List<Tuple<bool, Line>> rightLineList;

        List<Point> currentLine;



        public MVP_Model(MVP_Presenter presenter)
        {
            programPresenter = presenter;
            historyOfActions = new ActionHistory();
            redrawAss = new RedrawAssistant();
            rightLineList = new List<Tuple<bool, Line>>();
            overlayItems = new List<Tuple<bool, HashSet<Point>>>();
        }

        /**************************/
        /*** INTERNAL FUNCTIONS ***/
        /**************************/

        /// <summary>
        /// A function that returns a white canvas for a given width and height.
        /// </summary>
        /// <param name="width">The width of the canvas in pixels</param>
        /// <param name="height">The height of the canvas in pixels</param>
        /// <returns>The new canvas</returns>
        private Image GetEmptyCanvas(int width, int height)
        {
            Image image;
            try
            {
                image = new Bitmap(width, height);
            }
            catch (ArgumentException e)
            {
                programPresenter.PassMessageToView("The requested canvas size caused an error: \n" 
                    + e.ToString() + "\n The Canvas will be set to match your window.");
                image = new Bitmap(leftImageBoxWidth, leftImageBoxHeight);
            }
            Graphics graph = Graphics.FromImage(image);
            graph.FillRectangle(Brushes.White, 0, 0, width + 10, height + 10);
            return image;
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
                leftImage = GetEmptyCanvas(leftImageBoxWidth, leftImageBoxHeight);
            }
            else
            {
                leftImage = GetEmptyCanvas(width, height);
            }
            programPresenter.UpdateLeftImage(leftImage);
        }

        /// <summary>
        /// Redraws all lines in rightLineList, for which their associated boolean value equals true and calls RedrawRightOverlay.
        /// </summary>
        private void RedrawRightImage()
        {
            var workingCanvas = GetEmptyCanvas(rightImageWithoutOverlay.Width, rightImageWithoutOverlay.Height);
            var workingGraph = Graphics.FromImage(workingCanvas);
            //Lines
            foreach (Tuple<bool, Line> lineBoolTuple in rightLineList)
            {
                if (lineBoolTuple.Item1)
                {
                    lineBoolTuple.Item2.DrawLine(workingGraph);
                }
            }
            //The Line being currently drawn
            if (currentLine != null && currentLine.Count > 0 && inDrawingMode && mousePressed)
            {
                var currLine = new Line(currentLine);
                currLine.DrawLine(workingGraph);
            }
            rightImageWithoutOverlay = workingCanvas;
            //Redraw the Overlay if needed
            if (leftImage != null)
            {
                RedrawRightOverlay();
            }
            else
            {
                programPresenter.UpdateRightImage(rightImageWithoutOverlay);
            }
        }

        /// <summary>
        /// Redraws all elements in the overlay items for which the respective boolean value is true.
        /// </summary>
        private void RedrawRightOverlay()
        {
            var workingCanvas = rightImageWithoutOverlay;
            var workingGraph = Graphics.FromImage(workingCanvas);
            foreach (Tuple<bool, HashSet<Point>> tup in overlayItems)
            {
                if (tup.Item1)
                {
                    foreach (Point p in tup.Item2)
                    {
                        workingGraph.FillRectangle(Brushes.Green, p.X, p.Y, 1, 1);
                    }
                }
            }
            rightImageWithOverlay = workingCanvas;
            programPresenter.UpdateRightImage(rightImageWithOverlay);
        }
        
        /// <summary>
        /// Change the status of whether or not the lines are shown.
        /// </summary>
        /// <param name="lines">The HashSet containing the affected Line IDs.</param>
        /// <param name="shown">True if the lines should be shown, false if they should be hidden.</param>
        private void ChangeLines(HashSet<int> lines, bool shown)
        {
            var changed = false;
            foreach (int lineId in lines)
            {
                if (lineId <= rightLineList.Count - 1 && lineId >= 0)
                {
                    rightLineList[lineId] = new Tuple<bool, Line>(shown, rightLineList[lineId].Item2);
                    changed = true;
                }
            }
            if (changed) { RedrawRightImage(); }
        }
        
        /// <summary>
        /// A function that populates the matrixes needed for deletion detection with line data.
        /// </summary>
        private void RepopulateDeletionMatrixes()
        {
            if (rightImageWithoutOverlay != null)
            {
                isFilledMatrix = new bool[rightImageWithoutOverlay.Width, rightImageWithoutOverlay.Height];
                linesMatrix = new HashSet<int>[rightImageWithoutOverlay.Width, rightImageWithoutOverlay.Height];
                foreach (Tuple<bool, Line> lineTuple in rightLineList)
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

            foreach (Point pnt in GeometryCalculator.FilledCircleAlgorithm(p, (int)range))
            {
                if (pnt.X >= 0 && pnt.Y >= 0 && pnt.X < rightImageWithoutOverlay.Width && pnt.Y < rightImageWithoutOverlay.Height)
                {
                    if (isFilledMatrix[pnt.X, pnt.Y])
                    {
                        returnSet.UnionWith(linesMatrix[pnt.X, pnt.Y]);
                    }
                }
            }
            return returnSet;
        }

        /*
        /// <summary>
        /// Will calculate the start and endpoints of the given line on the right canvas.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="size">The size of the circle with which the endpoints of the line are marked.</param>
        private Tuple<HashSet<Point>, HashSet<Point>> CalculateStartAndEnd(Line line, int size)
        {
            var circle0 = GeometryCalculator.FilledCircleAlgorithm(line.GetStartPoint(), size);
            var circle1 = GeometryCalculator.FilledCircleAlgorithm(line.GetEndPoint(), size);
            var currentLineEndings = new Tuple<HashSet<Point>, HashSet<Point>>(circle0, circle1);
            return currentLineEndings;
        }
        */



        /// <summary>
        /// Tells the Presenter to Update the UI
        /// </summary>
        private void UpdateUI()
        {
            programPresenter.UpdateUIState(inDrawingMode, historyOfActions.CanUndo(), historyOfActions.CanRedo(), (rightImageWithoutOverlay != null));
        }


        /********************************************/
        /*** FUNCTIONS TO INTERACT WITH PRESENTER ***/
        /********************************************/

        /// <summary>
        /// Creates an empty Canvas
        /// </summary>
        public void DrawEmptyCanvasRight()
        {
            if (leftImage == null)
            {
                rightImageWithoutOverlay = GetEmptyCanvas(leftImageBoxWidth, leftImageBoxHeight);
            }
            else
            {
                rightImageWithoutOverlay = GetEmptyCanvas(leftImage.Width, leftImage.Height);
            }
            RepopulateDeletionMatrixes();
            rightImageWithOverlay = rightImageWithoutOverlay;
            programPresenter.UpdateRightImage(rightImageWithOverlay);
        }

        /// <summary>
        /// The function to set the left image.
        /// </summary>
        /// <param name="width">The width of the left image.</param>
        /// <param name="height">The height of the left image.</param>
        /// <param name="listOfLines">The List of Lines to be displayed in the left image.</param>
        public void SetLeftLineList(int width, int height, List<Line> listOfLines)
        {
            var workingCanvas = GetEmptyCanvas(width,height);
            var workingGraph = Graphics.FromImage(workingCanvas);
            leftLineList = listOfLines;
            redrawAss = new RedrawAssistant(leftLineList);
            overlayItems = redrawAss.Initialize(markerRadius);
            //Lines
            foreach (Line line in leftLineList)
            {
                line.DrawLine(workingGraph);
            }
            leftImage = workingCanvas;
            programPresenter.UpdateLeftImage(leftImage);
            //Set right image to same size as left image and delete linelist
            DrawEmptyCanvasRight();
            rightLineList = new List<Tuple<bool, Line>>();
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
                if(leftImage != null)
                {
                    //overlayItems = redrawAss.Tick(currentCursorPosition, rightLineList, -1, false);
                }
                RedrawRightImage();
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
                if (leftImage != null)
                {
                    //overlayItems = redrawAss.Tick(currentCursorPosition, rightLineList, -1, false);
                }
                RedrawRightImage();
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
        /// A method to get the dimensions of the right image.
        /// </summary>
        /// <returns>A tuple containing the width and height of the right image.</returns>
        public Tuple<int, int> GetRightImageDimensions()
        {
            if (rightImageWithoutOverlay != null)
            {
                return new Tuple<int, int>(rightImageWithoutOverlay.Width, rightImageWithoutOverlay.Height);
            }
            else
            {
                return new Tuple<int, int>(0, 0);
            }
        }

        /// <summary>
        /// Updates the current cursor position of the model.
        /// </summary>
        /// <param name="p">The new cursor position</param>
        public void SetCurrentCursorPosition(Point p)
        {
            currentCursorPosition = p;
        }

        /// <summary>
        /// Start a new Line, when the Mouse is pressed down.
        /// </summary>
        public void MouseDown()
        {
            mousePressed = true;
            if (inDrawingMode)
            {
                currentLine = new List<Point>();
            }
        }

        /// <summary>
        /// Finish the current Line, when the pressed Mouse is released.
        /// </summary>
        public void MouseUp()
        {
            mousePressed = false;
            if (inDrawingMode && currentLine.Count > 0)
            {
                Line newLine = new Line(currentLine, rightLineList.Count);
                rightLineList.Add(new Tuple<bool, Line>(true, newLine));
                newLine.PopulateMatrixes(isFilledMatrix, linesMatrix);
                programPresenter.PassLastActionTaken(historyOfActions.AddNewAction(new SketchAction(SketchAction.ActionType.Draw, newLine.GetID())));
                if(leftImage != null)
                {
                    //Execute a RedrawAssistant tick with the currently finished Line
                    //overlayItems = redrawAss.Tick(currentCursorPosition, rightLineList, newLine.GetID(), true);
                }
                RedrawRightImage();
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
            cursorPositions.Enqueue(currentCursorPosition);
            //Drawing
            if (inDrawingMode && mousePressed)
            {
                var rightGraph = Graphics.FromImage(rightImageWithoutOverlay);
                currentLine.Add(currentCursorPosition);
                Line drawline = new Line(currentLine);
                drawline.DrawLine(rightGraph);
                RedrawRightOverlay();
            }
            //Deleting
            if (!inDrawingMode && mousePressed)
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
                            rightLineList[lineID] = new Tuple<bool, Line>(false, rightLineList[lineID].Item2);
                        }
                        RepopulateDeletionMatrixes();
                        if(leftImage != null)
                        {
                            //Redraw overlay gets ticked
                            //overlayItems = redrawAss.Tick(currentCursorPosition, rightLineList, -1, false);
                        }
                        RedrawRightImage();
                    }
                }
            }
        }

        /// <summary>
        /// A helper Function that updates the markerRadius & deletionRadius, considering the size of the canvas.
        /// </summary>
        public void UpdateSizes()
        {
            if (rightImageWithoutOverlay != null)
            {
                int widthImage = rightImageWithoutOverlay.Width;
                int heightImage = rightImageWithoutOverlay.Height;
                int widthBox = rightImageBoxWidth;
                int heightBox = rightImageBoxHeight;

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
                redrawAss.SetMarkerRadius(markerRadius);
                deletionRadius = (int)(5 * zoomFactor);
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
