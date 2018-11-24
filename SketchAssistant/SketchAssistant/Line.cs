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

        public Line(List<Point> points)
        {
            linePoints = new List<Point>(points);
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
            //canvas.DrawLine(thePen, linePoints[linePoints.Count-1], linePoints[linePoints.Count]);
            return canvas;
        }
    }
}
