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
        private List<Point> linePoints;
        private int identifier;
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
            return canvas;
        }

        /// <summary>
        /// A function that will take to matrixes and populate the with the line data of this line object
        /// </summary>
        /// <param name="boolMatrix">The Matrix of booleans, in which is saved wether there is a line at this position.</param>
        /// <param name="listMatrix">The Matrix of Lists of integers, in which is saved which lines are at this position</param>
        public void PopulateMatrixes(bool[,] boolMatrix, List<int>[,] listMatrix)
        {
            if(!isTemporary)
            {
                foreach (Point currPoint in linePoints)
                {
                    try
                    {
                        boolMatrix[currPoint.X, currPoint.Y] = true;
                        if (listMatrix[currPoint.X, currPoint.Y] == null)
                        {
                            listMatrix[currPoint.X, currPoint.Y] = new List<int>();
                        }
                        listMatrix[currPoint.X, currPoint.Y].Add(identifier);
                    }
                    catch (IndexOutOfRangeException e)
                    {

                    }
                }
            }
        }

        /// <summary>
        /// Removes duplicate points from the line object
        /// </summary>
        private void CleanPoints()
        {
            List<Point> newList = new List<Point>();
            List<Point> tempList = new List<Point>();
            Point nullPoint = new Point(-1, -1);
            //Remove duplicate points
            for (int i = 1; i < linePoints.Count; i++)
            {
                if ((linePoints[i].X == linePoints[i - 1].X) && (linePoints[i].Y == linePoints[i - 1].Y))
                {
                    linePoints[i - 1] = nullPoint;
                }
            }
            foreach(Point linepoint in linePoints)
            {
                if (!(linepoint.X == -1))
                {
                    tempList.Add(linepoint);
                }
            }
            //Fill the gaps between points
            for (int i = 0; i < tempList.Count - 1; i++)
            {
                List<Point> partialList = BresenhamLineAlgorithm(tempList[i], tempList[i + 1]);
                partialList.RemoveAt(partialList.Count - 1);
                newList.AddRange(partialList);
            }
            newList.Add(tempList.Last<Point>());
            linePoints = newList;
        }

        /// <summary>
        /// An implementation of the Bresenham Line Algorithm, 
        /// which calculates all points between two points in a straight line.
        /// </summary>
        /// <param name="p0">The start point</param>
        /// <param name="p1">The end point</param>
        /// <returns>All points between p0 and p1 (including p0 and p1)</returns>
        public static List<Point> BresenhamLineAlgorithm(Point p0, Point p1)
        {
            List<Point> returnList = new List<Point>();
            int deltaX = p1.X - p0.X;
            int deltaY = p1.Y - p0.Y;
            if(deltaX != 0 && deltaY != 0)
            {
                //line is not horizontal or vertical
                //Bresenham Implementation taken from Wikipedia
                float deltaErr = Math.Abs(deltaY / deltaX);
                float error = 0;
                int y = p0.Y;
                if (deltaX > 0)
                {
                    for (int x = p0.X; x <= p1.X; x++)
                    {
                        returnList.Add(new Point(x, y));
                        error += deltaErr;
                        if (error >= 0.5)
                        {
                            y = y + Math.Sign(deltaY) * 1;
                            error -= 1;
                        }
                    }
                }
                else if(deltaX < 0)
                {
                    for (int x = p0.X; x >= p1.X; x--)
                    {
                        returnList.Add(new Point(x, y));
                        error += deltaErr;
                        if (error >= 0.5)
                        {
                            y = y + Math.Sign(deltaY) * 1;
                            error -= 1;
                        }
                    }
                }
                return returnList;
            }
            else if(deltaX == 0 && deltaY != 0)
            {
                //line is vertical
                if (deltaY < 0)
                {
                    //p1 is above of p0
                    for (int i = p0.Y; i >= p1.Y; i--)
                    {
                        returnList.Add(new Point(p0.X, i));
                    }
                    return returnList;
                }
                else
                {
                    //p1 is below of p0
                    for (int i = p0.Y; i <= p1.Y; i++)
                    {
                        returnList.Add(new Point(p0.X, i));
                    }
                    return returnList;
                }
            }
            else if(deltaX != 0 && deltaY == 0)
            {
                //line is horizontal
                if(deltaX < 0)
                {
                    //p1 is left of p0
                    for(int i = p0.X; i >= p1.X; i--)
                    {
                        returnList.Add(new Point(i, p0.Y));
                    }
                    return returnList;
                }
                else
                {
                    //p1 is right of p0
                    for (int i = p0.X; i <= p1.X; i++)
                    {
                        returnList.Add(new Point(i, p0.Y));
                    }
                    return returnList;
                }
            }
            else
            {
                //both points are the same
                returnList.Add(p0);
                return returnList;
            }
        }
    }
}
