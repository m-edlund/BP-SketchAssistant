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

        bool inDrawingMode;

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
            historyOfActions = new ActionHistory(null);
            redrawAss = new RedrawAssistant();
        }
        
        public void Undo()
        {

        }

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
        /// Creates an empty Canvas
        /// </summary>
        private void DrawEmptyCanvasRight()
        {
            if (leftImage == null)
            {
                rightImageWithoutOverlay = GetEmptyCanvas(leftImageBoxWidth, leftImageBoxHeight);
            }
            else
            {
                rightImageWithoutOverlay = GetEmptyCanvas(leftImage.Width, leftImage.Height);
            }
            RefreshRightImage();
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
            RefreshLeftImage();
        }

        /// <summary>
        /// A function to refresh the image being displayed in the right picture box, with the current rightImageWithOverlay.
        /// </summary>
        /// <param name="image">The new Image</param>
        private void RefreshRightImage()
        {
            programPresenter.UpdateRightImage(rightImageWithOverlay);
        }

        /// <summary>
        /// A function to refresh the image being displayed in the left picture box, with the current leftImage.
        /// </summary>
        /// <param name="image">The new Image</param>
        private void RefreshLeftImage()
        {
            programPresenter.UpdateLeftImage(leftImage);
        }

        /// <summary>
        /// Redraws all lines in lineList, for which their associated boolean value equals true and calls RedrawRightOverlay.
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
            //Redraw the Overlay
            RedrawRightOverlay();
            RefreshRightImage();
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
            RefreshRightImage();
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
        /*
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
                if (pnt.X >= 0 && pnt.Y >= 0 && pnt.X < rightImage.Width && pnt.Y < rightImage.Height)
                {
                    if (isFilledMatrix[pnt.X, pnt.Y])
                    {
                        returnSet.UnionWith(linesMatrix[pnt.X, pnt.Y]);
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
            foreach (Line l in leftLineList)
            {
                l.DrawLine(Graphics.FromImage(leftImage));
            }
        }
        
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
        /// Tells the Presenter to Update the UI
        /// </summary>
        private void UpdateUI()
        {
            programPresenter.UpdateUIState(inDrawingMode, historyOfActions.CanUndo(), historyOfActions.CanRedo(), (rightImageWithoutOverlay != null));
        }

        /// <summary>
        /// A method to get the dimensions of the right image.
        /// </summary>
        /// <returns>A tuple containing the width and height of the right image.</returns>
        public Tuple<int, int> GetRightImageDimensions()
        {
            if(rightImageWithoutOverlay != null)
            {
                return new Tuple<int, int>(rightImageWithoutOverlay.Width, rightImageWithoutOverlay.Height);
            }
            else
            {
                return new Tuple<int, int>(0, 0);
            }
        }

        public void SetCurrentCursorPosition(Point p)
        {

        }
        /*
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
        */
    }
}
