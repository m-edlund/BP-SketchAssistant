﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SketchAssistantWPF
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
            for (int i = 0; i < linePoints.Count - 1; i++)
            {
                canvas.DrawLine(Pens.Black, linePoints[i], linePoints[i + 1]);
            }
            //If there is only one point
            if (linePoints.Count == 1) { canvas.FillRectangle(Brushes.Black, linePoints[0].X, linePoints[0].Y, 1, 1); }
            return canvas;
        }

        /// <summary>
        /// A function that will take to matrixes and populate the with the line data of this line object
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
        }
    }
}
