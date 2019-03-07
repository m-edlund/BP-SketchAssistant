using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SketchAssistantWPF
{
    public class RedrawLine
    {
        InternalLine LineToRedraw;

        Point[] points;

        private int radius;

        private HashSet<Point>[] detectionZones;

        /// <summary>
        /// Constructor of the RedrawLine.
        /// </summary>
        /// <param name="rad">The radius around each point of the line input will be accepted.</param>
        public RedrawLine(int rad)
        {
            radius = rad;
        }

        /// <summary>
        /// A function to intialize the redraw line.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public bool Init(InternalLine line)
        {
            HashSet<Point> initialZone = GeometryCalculator.FilledCircleAlgorithm(new Point(0, 0), radius);
            LineToRedraw = line;
            points = line.GetPoints().ToArray();

            List<HashSet<Point>> dZones = new List<HashSet<Point>>();

            foreach(Point p in points)
            {
                HashSet<Point> newZone = new HashSet<Point>();
                foreach(Point i_p in initialZone)
                {
                    newZone.Add(new Point(i_p.X + p.X, i_p.Y + p.Y));
                }
                dZones.Add(newZone);
            }
            detectionZones = dZones.ToArray();

            Console.WriteLine("This line has {0} points ", line.GetPoints().Count());
            return false;
        }

        public Point GetOverlayPosition()
        {
            return points[0];
        }

        public Angle GetDirection(Point p)
        {
            //TODO: Calculate angles between p and the next n points of the line
            // Take average and return it.
            return null;
        }

    }
}
