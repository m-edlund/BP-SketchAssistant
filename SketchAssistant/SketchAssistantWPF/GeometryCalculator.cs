using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SketchAssistantWPF
{
    /// <summary>
    /// A class that contains all algorithms related to geometry.
    /// </summary>
    public static class GeometryCalculator
    {
        /// <summary>
        /// An implementation of the Bresenham Line Algorithm,
        /// which calculates the points of a circle in a radius around a center point.
        /// Implemented with the help of code examples on Wikipedia.
        /// </summary>
        /// <param name="center">The center of the circle.</param>
        /// <param name="radius">The radius of the circle, 
        /// when it is zero or less, only the midpoint is returned</param>
        /// <returns>The HashSet containing all Points on the circle.</returns>
        public static HashSet<Point> BresenhamCircleAlgorithm(Point center, int radius)
        {
            if (radius <= 0) { return new HashSet<Point> { center }; }

            int x = radius - 1;
            int y = 0;
            int dx = 1;
            int dy = 1;
            int err = dx - (radius * 2);
            HashSet<Point> returnSet = new HashSet<Point>();

            while (x >= y)
            {
                returnSet.Add(new Point(center.X + x, center.Y + y));
                returnSet.Add(new Point(center.X + y, center.Y + x));
                returnSet.Add(new Point(center.X - y, center.Y + x));
                returnSet.Add(new Point(center.X - x, center.Y + y));
                returnSet.Add(new Point(center.X - x, center.Y - y));
                returnSet.Add(new Point(center.X - y, center.Y - x));
                returnSet.Add(new Point(center.X + y, center.Y - x));
                returnSet.Add(new Point(center.X + x, center.Y - y));

                if (err <= 0)
                {
                    y++;
                    err += dy;
                    dy += 2;
                }

                if (err > 0)
                {
                    x--;
                    dx += 2;
                    err += dx - (radius * 2);
                }
            }
            return returnSet;
        }

        /// <summary>
        /// A simple algorithm that returns a filled circle with a radius and a center point.
        /// </summary>
        /// <param name="center">The center point of the alorithm </param>
        /// <param name="radius">The radius of the circle, if its less or equal to 1 
        /// only the center point is returned. </param>
        /// <returns>All the points in or on the circle.</returns>
        public static HashSet<Point> FilledCircleAlgorithm(Point center, int radius)
        {
            HashSet<Point> returnSet = new HashSet<Point> { center };
            //Fill the circle
            for (int x = 0; x < radius; x++)
            {
                for (int y = 0; y < radius; y++)
                {
                    //Check if point is on or in the circle
                    if ((x * x + y * y - radius * radius) <= 0)
                    {
                        returnSet.Add(new Point(center.X + x, center.Y + y));
                        returnSet.Add(new Point(center.X - x, center.Y + y));
                        returnSet.Add(new Point(center.X + x, center.Y - y));
                        returnSet.Add(new Point(center.X - x, center.Y - y));
                    }
                }
            }
            return returnSet;
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
            int p1x = (int)p1.X;
            int p1y = (int)p1.Y;
            int p0x = (int)p0.X;
            int p0y = (int)p0.Y;

            int deltaX = p1x - p0x;
            int deltaY = p1y - p0y;
            List<Point> returnList;

            if (Math.Abs(deltaY) < Math.Abs(deltaX))
            {
                if (p0.X > p1.X)
                {
                    returnList = GetLineLow(p1x, p1y, p0x, p0y);
                    returnList.Reverse();
                }
                else
                {
                    returnList = GetLineLow(p0x, p0y, p1x, p1y);
                }
            }
            else
            {
                if (p0.Y > p1.Y)
                {
                    returnList = GetLineHigh(p1x, p1y, p0x, p0y);
                    returnList.Reverse();
                }
                else
                {
                    returnList = GetLineHigh(p0x, p0y, p1x, p1y);
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
