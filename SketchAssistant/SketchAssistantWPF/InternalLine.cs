using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SketchAssistantWPF
{
    public class InternalLine
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
        /// A collection of the original Points defining the line.
        /// </summary>
        private PointCollection pointColl;
        /// <summary>
        /// Indicates if this is a single point.
        /// </summary>
        public bool isPoint { get; private set; }
        /// <summary>
        /// The location of the point, if this is a point
        /// </summary>
        public Point point { get; private set; }

        /// <summary>
        /// The constructor for lines which are only temporary.
        /// If you want nice lines use the other constructor.
        /// </summary>
        /// <param name="points">The points of the line</param>
        public InternalLine(List<Point> points)
        {
            linePoints = new List<Point>(points);
            pointColl = new PointCollection(linePoints);
            isTemporary = true;
        }

        /// <summary>
        /// The constructor for lines, which will be more resource efficient 
        /// and have the ability to populate deletion matrixes.
        /// </summary>
        /// <param name="points">The points of the line</param>
        /// <param name="id">The identifier of the line</param>
        public InternalLine(List<Point> points, int id)
        {
            linePoints = new List<Point>(points);
            pointColl = new PointCollection(linePoints);
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

        public PointCollection GetPointCollection()
        {
            return pointColl;
        }

        /// <summary>
        /// A function that will take two matrixes and populate them with the line data of this line object
        /// </summary>
        /// <param name="boolMatrix">The Matrix of booleans, in which is saved wether there is a line at this position.</param>
        /// <param name="listMatrix">The Matrix of Lists of integers, in which is saved which lines are at this position</param>
        public void PopulateMatrixes(bool[,] boolMatrix, HashSet<int>[,] listMatrix)
        {
            if (!isTemporary)
            {
                foreach (Point currPoint in linePoints)
                {
                    if (currPoint.X >= 0 && currPoint.Y >= 0 &&
                        currPoint.X < boolMatrix.GetLength(0) && currPoint.Y < boolMatrix.GetLength(1))
                    {
                        boolMatrix[(int) currPoint.X, (int) currPoint.Y] = true;
                        if (listMatrix[(int) currPoint.X, (int) currPoint.Y] == null)
                        {
                            listMatrix[(int) currPoint.X, (int) currPoint.Y] = new HashSet<int>();
                        }
                        listMatrix[(int) currPoint.X, (int) currPoint.Y].Add(identifier);
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
                //if this is a point or not
                var localIsPoint = true;
                //check if its a point
                foreach(Point p in linePoints)
                {
                    if (p.X != linePoints[0].X || p.Y != linePoints[0].Y)
                        localIsPoint = false;
                }
                if (!localIsPoint) {
                    List<Point> newList = new List<Point>();
                    List<Point> tempList = new List<Point>();
                    //Since Point is non-nullable, we must ensure the nullPoints, 
                    //which we remove can not possibly be points of the original given line.
                    int nullValue = (int) linePoints[0].X + 1;
                    //Fill the gaps between points
                    for (int i = 0; i < linePoints.Count - 1; i++)
                    {
                        nullValue += (int) linePoints[i + 1].X;
                        List<Point> partialList = GeometryCalculator.BresenhamLineAlgorithm(linePoints[i], linePoints[i + 1]);
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
                else
                {
                    isPoint = true;
                    point = linePoints[0];
                    linePoints = new List<Point>();
                    linePoints.Add(point);
                }
            }
        }
    }
}
