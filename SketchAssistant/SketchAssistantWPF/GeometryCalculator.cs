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
        public static double CalculateAverageCosineSimilarity(InternalLine l0, InternalLine l1)
        {
            List<Point> points0 = l0.GetPoints();
            List<Point> points1 = l1.GetPoints();

            if (points0.Count == points1.Count)
            {
                double sum = 0; int i = 0;
                List<Point> shortL = points0; List<Point> longL = points1;
                for (; i < shortL.Count - 1; i++)
                {
                    if (i + 1 == shortL.Count || i + 1 == longL.Count) break;
                    Vector v0 = new Vector(shortL[i + 1].X - shortL[i].X, shortL[i + 1].Y - shortL[i].Y);
                    Vector v1 = new Vector(longL[i + 1].X - longL[i].X, longL[i + 1].Y - longL[i].Y);
                    sum += CosineSimilarity(v0, v1);
                }
                return sum / i;
            }
            else
            {
                List<Point> shortL = points0; List<Point> longL = points0;
                if (points0.Count < points1.Count) { longL = points1; }
                if (points0.Count > points1.Count) { shortL = points1;}
                double dif = (longL.Count - 1) / (shortL.Count - 1);
                if(dif > 1)
                {
                    double sum = 0; int adds = 0;

                    for (int i = 0; i < shortL.Count - 1; i++)
                    {
                        if (i + 1 == shortL.Count || i + dif == longL.Count) break;
                        for (int j = 0; j <= dif; j++)
                        {
                            var k = i + j;
                            Vector v0 = new Vector(shortL[i + 1].X - shortL[i].X, shortL[i + 1].Y - shortL[i].Y);
                            Vector v1 = new Vector(longL[k + 1].X - longL[k].X, longL[k + 1].Y - longL[k].Y);
                            sum += CosineSimilarity(v0, v1); adds++;
                        }
                    }
                    return sum / adds;
                }
                else
                {
                    double sum = 0; int i = 0;
                    for (; i < shortL.Count - 1; i++)
                    {
                        if (i + 1 == shortL.Count || i + 1 == longL.Count) break;
                        Vector v0 = new Vector(shortL[i + 1].X - shortL[i].X, shortL[i + 1].Y - shortL[i].Y);
                        Vector v1 = new Vector(longL[i + 1].X - longL[i].X, longL[i + 1].Y - longL[i].Y);
                        sum += CosineSimilarity(v0, v1);
                    }
                    return sum / i;
                }
            }
        }

        public static double CalculateSimilarity(InternalLine l0, InternalLine l1)
        {
            double CosSim = Math.Abs(CalculateAverageCosineSimilarity(l0, l1));
            double LenSim = CalculateLengthSimilarity(l0, l1);
            double AvDist = CalculateAverageDistance(l0, l1);
            double DistSim = (50 - AvDist)/ 50;
            if (DistSim < 0) DistSim = 0;

            return (CosSim + LenSim + DistSim)/3;
        }

        private static double CalculateLengthSimilarity(InternalLine l0, InternalLine l1)
        {
            double len0 = l0.GetLength(); double len1 = l1.GetLength();
            var dif = Math.Abs(len1 - len0);
            double shorter;
            if (len1 > len0) shorter = len0;
            else shorter = len1;
            if (dif >= shorter) return 0;
            return (shorter - dif )/shorter;
        }

        private static double CalculateAverageDistance(InternalLine l0, InternalLine l1)
        {
            List<Point> points0 = l0.GetPoints();
            List<Point> points1 = l1.GetPoints();
            double distfirstfirst = Math.Sqrt(Math.Pow((points0[0].X - points1[0].X) , 2) + Math.Pow((points0[0].Y - points1[0].Y) , 2));
            double distlastlast = Math.Sqrt(Math.Pow((points0.Last().X - points1.Last().X), 2) + Math.Pow((points0.Last().Y - points1.Last().Y), 2));
            double distfirstlast = Math.Sqrt(Math.Pow((points0[0].X - points1.Last().X), 2) + Math.Pow((points0[0].Y - points1.Last().Y), 2));
            double distlastfirst = Math.Sqrt(Math.Pow((points0.Last().X - points1[0].X), 2) + Math.Pow((points0.Last().Y - points1[0].Y), 2));
            if ((distfirstfirst + distlastlast) / 2 < (distfirstlast + distlastfirst) / 2) return (distfirstfirst + distlastlast) / 2;
            else return (distfirstlast + distlastfirst) / 2;
        }

        public static double CosineSimilarity(Vector v0, Vector v1)
        {
            return (v0.X * v1.X + v0.Y * v1.Y) / (Math.Sqrt(v0.X * v0.X + v0.Y * v0.Y) * Math.Sqrt(v1.X * v1.X + v1.Y * v1.Y));
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
