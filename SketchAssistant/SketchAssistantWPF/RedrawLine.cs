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

        private int finishedIndex;

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
            finishedIndex = 0;
            Console.WriteLine("This line has {0} points ", line.GetPoints().Count());
            return false;
        }

        public Point GetOverlayPosition()
        {
            return points[0];
        }

        public double GetDirection(Point p)
        {
            if(finishedIndex > points.Length - 1)
            {
                return -1;
            }

            if(detectionZones[finishedIndex + 1].Contains(p))
            {
                finishedIndex++;
            }

            double angle = 0;
            var np = points[finishedIndex + 1];
            Vector vector0 = new Vector(1,0);
            Vector vector1 = new Vector(np.X-p.X, np.Y - p.Y);
            angle = Math.Acos((vector0.X* vector1.X + vector0.Y * vector1.Y) / (vector0.Length * vector1.Length)) / Math.PI * 180;
            /*double cross_prod = np.Y - p.Y;
            double acute_angle = Math.Atan2(Math.Abs(cross_prod), np.X - p.Y)* 180 /Math.PI;
            if (cross_prod < 0) { angle = 360 - acute_angle; }
            else { angle = acute_angle; }
            */
            //TODO: Calculate angles between p and the next n points of the line
            // Take average and return it.
            return angle;
        }
    }
}
