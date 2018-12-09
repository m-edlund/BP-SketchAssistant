using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SketchAssistant
{
    public class Line
    {
        /// <summary>
        /// list saving all the points of the line in the order of the path from start to end point
        /// </summary>
        private List<Point> linePoints;
        /// <summary>
        /// unique identifier of this Line object
        /// </summary>
        private int identifier;
        /// <summary>
        /// flag showing if this is only a temporary line
        /// </summary>
        private bool isTemporary;

        /// <summary>
        /// The constructor for lines which are only temporary.
        /// If you want nice lines use the other constructor.
        /// </summary>
        /// <param name="points">The points of the line</param>
        public Line(List<Point> points)
        {
            linePoints = new List<Point>(points);
            isTemporary = true;
        }

        /// <summary>
        /// The constructor for lines, which will be more resource efficient 
        /// and have the ability to populate deletion matrixes.
        /// </summary>
        /// <param name="points">The points of the line</param>
        /// <param name="id">The identifier of the line</param>
        public Line(List<Point> points, int id)
        {
            linePoints = new List<Point>(points);
            identifier = id;
            CleanPoints();
            isTemporary = false;
        }

        public Point GetStartPoint()
        {
            return linePoints.First();
        }

        public Point GetEndPoint()
        {
            return linePoints.Last();
        }

        public List<Point> GetPoints()
        {
            return linePoints;
        }

        public int GetID()
        {
            return identifier;
        }

        /// <summary>
        /// A function that takes a Graphics element and returns it with
        /// the line drawn on it.
        /// </summary>
        /// <param name="canvas">The Graphics element on which the line shall be drawn</param>
        /// <returns>The given Graphics element with the additional line</returns>
        public Graphics DrawLine(Graphics canvas)
        {
            Pen thePen = new Pen(Color.Black);
            for(int i = 0; i < linePoints.Count - 1 ; i++)
            {
                canvas.DrawLine(thePen, linePoints[i], linePoints[i + 1]);
            }
            //If there is only one point
            if(linePoints.Count == 1){ canvas.FillRectangle(Brushes.Black, linePoints[0].X, linePoints[0].Y, 1, 1); }
            return canvas;
        }

        /// <summary>
        /// A function that will take to matrixes and populate the with the line data of this line object
        /// </summary>
        /// <param name="boolMatrix">The Matrix of booleans, in which is saved wether there is a line at this position.</param>
        /// <param name="listMatrix">The Matrix of Lists of integers, in which is saved which lines are at this position</param>
        public void PopulateMatrixes(bool[,] boolMatrix, HashSet<int>[,] listMatrix)
        {
            if(!isTemporary)
            {
                foreach (Point currPoint in linePoints)
                {
                    if (currPoint.X >= 0 && currPoint.Y >= 0 && 
                        currPoint.X < boolMatrix.GetLength(0) && currPoint.Y < boolMatrix.GetLength(1))
                    {
                        boolMatrix[currPoint.X, currPoint.Y] = true;
                        if (listMatrix[currPoint.X, currPoint.Y] == null)
                        {
                            listMatrix[currPoint.X, currPoint.Y] = new HashSet<int>();
                        }
                        listMatrix[currPoint.X, currPoint.Y].Add(identifier);
                    }
                }
            }
        }

        /// <summary>
        /// Removes duplicate points from the line object
        /// </summary>
        private void CleanPoints()
        {
            if (linePoints.Count > 1)
            {
                List<Point> newList = new List<Point>();
                List<Point> tempList = new List<Point>();
                //Since Point is non-nullable, we must ensure the nullPoints, 
                //which we remove can not possibly be points of the original given line.
                int nullValue = linePoints[0].X + 1;
                //Fill the gaps between points
                for (int i = 0; i < linePoints.Count - 1; i++)
                {
                    nullValue += linePoints[i + 1].X;
                    List<Point> partialList = BresenhamLineAlgorithm(linePoints[i], linePoints[i + 1]);
                    tempList.AddRange(partialList);
                }
                Point nullPoint = new Point(nullValue, 0);
                //Set duplicate points to the null point
                for (int i = 1; i < tempList.Count; i++)
                {
                    if ((tempList[i].X == tempList[i - 1].X) && (tempList[i].Y == tempList[i - 1].Y))
                    {
                        tempList[i - 1] = nullPoint;
                    }
                }
                //remove the null points
                foreach (Point tempPoint in tempList)
                {
                    if (tempPoint.X != nullValue)
                    {
                        newList.Add(tempPoint);
                    }
                }
                linePoints = new List<Point>(newList);
            }
        }

        /// <summary>
        /// An implementation of the Bresenham Line Algorithm, 
        /// which calculates all points between two points in a straight line.
        /// Implemented using the pseudocode on Wikipedia.
        /// </summary>
        /// <param name="p0">The start point</param>
        /// <param name="p1">The end point</param>
        /// <returns>All points between p0 and p1 (including p0 and p1)</returns>
        public static List<Point> BresenhamLineAlgorithm(Point p0, Point p1)
        {
            int deltaX = p1.X - p0.X;
            int deltaY = p1.Y - p0.Y;
            List<Point> returnList;

            if (Math.Abs(deltaY) < Math.Abs(deltaX))
            {
                if(p0.X > p1.X)
                {
                    returnList = GetLineLow(p1.X, p1.Y, p0.X, p0.Y);
                    returnList.Reverse();
                }
                else
                {
                    returnList = GetLineLow(p0.X, p0.Y, p1.X, p1.Y);
                }
            }
            else
            {
                if (p0.Y > p1.Y)
                {
                    returnList = GetLineHigh(p1.X, p1.Y, p0.X, p0.Y);
                    returnList.Reverse();
                }
                else
                {
                    returnList = GetLineHigh(p0.X, p0.Y, p1.X, p1.Y);
                }
            }
            return returnList;
        }

        /// <summary>
        /// Helping function of the Bresenham Line algorithm,
        /// under the assumption that abs(deltaY) is smaller than abs(deltX)
        /// and x0 is smaller than x1
        /// </summary>
        /// <param name="x0">x value of point 0</param>
        /// <param name="y0">y value of point 0</param>
        /// <param name="x1">x value of point 1</param>
        /// <param name="y1">y value of point 1</param>
        /// <returns>All points on the line between the two points</returns>
        private static List<Point> GetLineLow(int x0, int y0, int x1, int y1)
        {
            List<Point> returnList = new List<Point>();
            int dx = x1 - x0;
            int dy = y1 - y0;
            int yi = 1;
            if(dy < 0)
            {
                yi = -1;
                dy = -dy;
            }
            int D = 2 * dy - dx;
            int y = y0;
            for (int x = x0; x <= x1; x++)
            {
                returnList.Add(new Point(x, y));
                if (D > 0)
                {
                    y = y + yi;
                    D = D - 2 * dx;
                }
                D = D + 2 * dy;
            }
            return returnList;
        }

        /// <summary>
        /// Helping function of the Bresenham Line algorithm,
        /// under the assumption that abs(deltaY) is larger or equal than abs(deltX)
        /// and y0 is smaller than y1
        /// </summary>
        /// <param name="x0">x value of point 0</param>
        /// <param name="y0">y value of point 0</param>
        /// <param name="x1">x value of point 1</param>
        /// <param name="y1">y value of point 1</param>
        /// <returns>All points on the line between the two points</returns>
        private static List<Point> GetLineHigh(int x0, int y0, int x1, int y1)
        {
            List<Point> returnList = new List<Point>();
            int dx = x1 - x0;
            int dy = y1 - y0;
            int xi = 1;
            if (dx < 0)
            {
                xi = -1;
                dx = -dx;
            }
            int D = 2 * dx - dy;
            int x = x0;
            for (int y = y0; y <= y1; y++)
            {
                returnList.Add(new Point(x, y));
                if (D > 0)
                {
                    x = x + xi;
                    D = D - 2 * dy;
                }
                D = D + 2 * dx;
            }
            return returnList;
        }
    }
}
