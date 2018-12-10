using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SketchAssistant
{
    /// <summary>
    /// A class that contains all algorithms related to geometry.
    /// </summary>
    public static class GeometryCalculator
    {

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
                if (p0.X > p1.X)
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
            if (dy < 0)
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
