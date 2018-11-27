using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SketchAssistant
{
    class Line
    {
        private List<Point> linePoints;
        private int identifier;

        public Line(List<Point> points)
        {
            linePoints = new List<Point>(points);
        }

        public Line(List<Point> points, int id)
        {
            linePoints = new List<Point>(points);
            identifier = id;
            CleanPoints();
        }

        public Point GetStartPoint()
        {
            return linePoints.First();
        }

        public Point GetEndPoint()
        {
            return linePoints.Last();
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
        /// Exceptions:
        ///     Will throw IndexOutOfRangeException if any of the points of this line are 
        ///     outside of the given matrixes.
        /// </summary>
        /// <param name="boolMatrix">The Matrix of booleans, in which is saved wether there is a line at this position.</param>
        /// <param name="listMatrix">The Matrix of Lists of integers, in which is saved which lines are at this position</param>
        public void PopulateMatrixes(bool[,] boolMatrix, List<int>[,] listMatrix)
        {
            foreach(Point currPoint in linePoints)
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
                catch(IndexOutOfRangeException e)
                {

                }
            }
        }

        /// <summary>
        /// Removes duplicate points from the line object
        /// </summary>
        private void CleanPoints()
        {
            List<Point> newList = new List<Point>();
            Point nullPoint = new Point(-1, -1);
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
                    newList.Add(linepoint);
                }
            }
            linePoints = newList;
        }
    }
}
