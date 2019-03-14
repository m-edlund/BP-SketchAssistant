using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Media;

namespace SketchAssistantWPF
{
    class TrajectoryGenerator
    {
        static readonly int CONSTANT_A = 50;
        static readonly double DEADZONE = 3;

        MVP_View programView_debug;

        List<Polyline> previousLines;

        /// <summary>
        /// the template for the line currently being drawn
        /// </summary>
        InternalLine currentLine;
        /// <summary>
        /// the points of the current line template, as an ordered list
        /// </summary>
        List<Point> currentPoints;

        //Point lastCursorPosition;

        /// <summary>
        /// pointer to the active section of the line template, indexing the ending point of the active section
        /// </summary>
        int index;

        public TrajectoryGenerator(MVP_View viewForDebug)
        {
            programView_debug = viewForDebug;
            previousLines = new List<Polyline>();
        }

        /// <summary>
        /// updates the pointer to the template for the line currently being drawn, and resets indices etc.
        /// </summary>
        /// <param name="newCurrentLine">the new template for the line currently being drawn</param>
        public void setCurrentLine(InternalLine newCurrentLine, List<InternalLine> leftLineList)
        {
            currentLine = newCurrentLine;
            if (currentLine != null)
            {
                currentPoints = currentLine.GetPoints();
            }
            else
            {
                currentPoints = null;
            }
            //lastCursorPosition = currentPoints.ElementAt(0);
            index = 1;

            if (leftLineList != null)
            {
                foreach (InternalLine l in leftLineList)
                {
                    Polyline temp = new Polyline();
                    PointCollection pointCollection = new PointCollection();
                    foreach (Point p in l.GetPoints())
                    {
                        pointCollection.Add(p);
                    }
                    temp.Points = pointCollection;
                    temp.Stroke = System.Windows.Media.Brushes.Red;
                    temp.StrokeThickness = 1;
                    programView_debug.AddNewLineRight(temp);
                    previousLines.Add(temp);
                }
            }
        }

        /// <summary>
        /// generates the new trajectory back to the template based on the current cursor position
        /// </summary>
        /// <param name="cursorPosition">the current cursor position</param>
        /// <returns>the direction in which to go, as an angle on the drawing plane, in degree format, with 0° being the positive X-Axis, increasing counterclockwise</returns>
        public double GenerateTrajectory(Point cursorPosition)
        {
            if (currentLine != null && currentPoints.Count > 1)
            {
                Console.WriteLine(cursorPosition);
                Console.WriteLine(index + ":" + currentPoints.Count);
                Console.WriteLine(currentPoints.ElementAt(index));

                //update index to point to current section if one or more section divideing lines have been passed since last call
                 while (index < (currentPoints.Count - 1) && (SectionDividingLinePassed(cursorPosition, currentPoints.ElementAt(index - 1), currentPoints.ElementAt(index), currentPoints.ElementAt(index + 1)) || ComputeDistance(cursorPosition, currentPoints.ElementAt(index)) < 3))
                 {
                     index++;
                 }
                /*if (ComputeDistance(cursorPosition, currentPoints.ElementAt(index)) < 5 && index < (currentPoints.Count - 1))
                {
                    index++;
                    Console.WriteLine(index);
                }*/
                //lastCursorPosition = cursorPosition;

                //project teh point onto the active line segment to be able to compute distances
                Point orthogonalProjection = ComputeOrthogonalProjection(cursorPosition, currentPoints.ElementAt(index - 1), currentPoints.ElementAt(index));


                //index of the last reachable actual point
                int targetIndex = index - 1;
                List<Tuple<Point, Point>> strikeZones = new List<Tuple<Point, Point>>();

                //if "far" away from the next actual point of the line, generate an auxiliary point at a constant distance (constantA) on the current line segment
                Point auxiliaryPoint = new Point(-1, -1);
                if (ComputeDistance(orthogonalProjection, currentPoints.ElementAt(index)) > CONSTANT_A)
                {
                    auxiliaryPoint = MoveAlongLine(orthogonalProjection, currentPoints.ElementAt(index - 1), currentPoints.ElementAt(index), CONSTANT_A);
                    strikeZones.Add(computeStrikeZone(currentPoints.ElementAt(index - 1), auxiliaryPoint, currentPoints.ElementAt(index), orthogonalProjection, cursorPosition));
                    //targetIndex--;
                }

                //aim for the furthest actual point of the line reachable by the descent rate constraints (lower bounds) given by the various strike zones
                while (targetIndex < (currentPoints.Count - 1) && allStrikeZonesPassed(strikeZones, cursorPosition, orthogonalProjection, currentPoints.ElementAt(targetIndex + 1)))
                {
                    strikeZones.Add(computeStrikeZone((targetIndex >= 0 ? currentPoints.ElementAt(targetIndex) : new Point(-1, -1)), currentPoints.ElementAt(targetIndex + 1), (targetIndex < currentPoints.Count - 1 ? currentPoints.ElementAt(targetIndex + 1) : new Point(-1, -1)), orthogonalProjection, cursorPosition));
                    targetIndex++;
                }


                Console.WriteLine(index + "," + targetIndex);
                Point furthestCrossingPoint = new Point(-1, -1); ;
                if (targetIndex < index) //auxiliary point created and next actual point not reachable
                {
                    furthestCrossingPoint = ComputeFurthestCrossingPoint(cursorPosition, orthogonalProjection, strikeZones, auxiliaryPoint, currentPoints.ElementAt(targetIndex + 1));

                    //if such a point exists, use it as target for the new trajectory
                    if (furthestCrossingPoint.X != -1)
                    {
                        Debug_DrawStrikeZones(strikeZones);
                        Debug_DrawTrajectoryVector(cursorPosition, furthestCrossingPoint);
                        Debug_DrawOrthogonalProjection(cursorPosition, orthogonalProjection);
                        Debug_DrawCurrentSection(currentPoints.ElementAt(index - 1), currentPoints.ElementAt(index));
                        return computeOrientationOfVector(cursorPosition, furthestCrossingPoint);
                    }
                    //else use the last reachable actual point
                    else
                    {
                        Debug_DrawStrikeZones(strikeZones);
                        Debug_DrawTrajectoryVector(cursorPosition, auxiliaryPoint);
                        Debug_DrawOrthogonalProjection(cursorPosition, orthogonalProjection);
                        Debug_DrawCurrentSection(currentPoints.ElementAt(index - 1), currentPoints.ElementAt(index));
                        return computeOrientationOfVector(cursorPosition, auxiliaryPoint);
                    }
                }
                else
                {
                    //aim for the furthest (auxiliary) point on the line segment after the last reachable actual point (only if there is such a segment: not if that last reachable point is the last point of the line)
                    if (targetIndex < (currentPoints.Count - 1))
                    {
                        furthestCrossingPoint = ComputeFurthestCrossingPoint(cursorPosition, orthogonalProjection, strikeZones, currentPoints.ElementAt(targetIndex), currentPoints.ElementAt(targetIndex + 1));
                    }
                    //if such a point exists, use it as target for the new trajectory
                    if (furthestCrossingPoint.X != -1)
                    {
                        Debug_DrawStrikeZones(strikeZones);
                        Debug_DrawTrajectoryVector(cursorPosition, furthestCrossingPoint);
                        Debug_DrawOrthogonalProjection(cursorPosition, orthogonalProjection);
                        Debug_DrawCurrentSection(currentPoints.ElementAt(index - 1), currentPoints.ElementAt(index));
                        return computeOrientationOfVector(cursorPosition, furthestCrossingPoint);
                    }
                    //else use the last reachable actual point
                    else
                    {
                        Debug_DrawStrikeZones(strikeZones);
                        Debug_DrawTrajectoryVector(cursorPosition, currentPoints.ElementAt(targetIndex));
                        Debug_DrawOrthogonalProjection(cursorPosition, orthogonalProjection);
                        Debug_DrawCurrentSection(currentPoints.ElementAt(index - 1), currentPoints.ElementAt(index));
                        return computeOrientationOfVector(cursorPosition, currentPoints.ElementAt(targetIndex));
                    }
                }
            }
            else return -1;
        }

        /// <summary>
        /// prints the trajectory vector on the drawing pane for debugging and calibration purposes
        /// </summary>
        /// <param name="vectorStartPoint">origin point of the trajectory vector</param>
        /// <param name="vectorEndPoint">target point of the trajectory vector</param>
        private void Debug_DrawTrajectoryVector(Point vectorStartPoint, Point vectorEndPoint)
        {
            Polyline temp = new Polyline();
            PointCollection pointCollection = new PointCollection();
            pointCollection.Add(vectorStartPoint);
            pointCollection.Add(vectorEndPoint);
            temp.Points = pointCollection;
            temp.Stroke = System.Windows.Media.Brushes.Red;
            temp.StrokeDashArray = new DoubleCollection { 2, 2 }; //TODO
            temp.StrokeThickness = 1;
            programView_debug.AddNewLineRight(temp);
            previousLines.Add(temp);
        }

        /// <summary>
        /// prints the trajectory vector on the drawing pane for debugging and calibration purposes
        /// </summary>
        /// <param name="vectorStartPoint">origin point of the trajectory vector</param>
        /// <param name="vectorEndPoint">target point of the trajectory vector</param>
        private void Debug_DrawOrthogonalProjection(Point cursorPosition, Point orthogonalProjection)
        {
            Polyline temp = new Polyline();
            PointCollection pointCollection = new PointCollection();
            pointCollection.Add(cursorPosition);
            pointCollection.Add(orthogonalProjection);
            temp.Points = pointCollection;
            temp.Stroke = System.Windows.Media.Brushes.Red;
            temp.StrokeDashArray = new DoubleCollection { 1, 2 }; //TODO
            temp.StrokeThickness = 1;
            programView_debug.AddNewLineRight(temp);
            previousLines.Add(temp);
        }

        /// <summary>
        /// prints the trajectory vector on the drawing pane for debugging and calibration purposes
        /// </summary>
        /// <param name="vectorStartPoint">origin point of the trajectory vector</param>
        /// <param name="vectorEndPoint">target point of the trajectory vector</param>
        private void Debug_DrawCurrentSection(Point sectionStartPoint, Point sectionEndPoint)
        {
            Polyline temp = new Polyline();
            PointCollection pointCollection = new PointCollection();
            pointCollection.Add(sectionStartPoint);
            pointCollection.Add(sectionEndPoint);
            temp.Points = pointCollection;
            temp.Stroke = System.Windows.Media.Brushes.Red;
            temp.StrokeThickness = 2;
            programView_debug.AddNewLineRight(temp);
            previousLines.Add(temp);
        }

        /// <summary>
        /// first deletes all debug lines from thte last interaction and then prints all strike zones on the drawing pane for debugging and calibration purposes
        /// </summary>
        /// <param name="strikeZones">list of all strike zones to be drawn</param>
        private void Debug_DrawStrikeZones(List<Tuple<Point, Point>> strikeZones)
        {
            foreach(Polyline l in previousLines)
            {
                programView_debug.RemoveSpecificLine(l);
            }
            previousLines.Clear();
            foreach (Tuple<Point, Point> t in strikeZones)
            {
                Polyline temp = new Polyline();
                PointCollection pointCollection= new PointCollection();
                pointCollection.Add(t.Item1);
                pointCollection.Add(t.Item2);
                temp.Points = pointCollection;
                temp.Stroke = System.Windows.Media.Brushes.DarkGreen;
                temp.StrokeDashArray = new DoubleCollection{ 2, 2 }; //TODO
                temp.StrokeThickness = 2;
                programView_debug.AddNewLineRight(temp);
                previousLines.Add(temp);
            }
        }

        /// <summary>
        /// computes the orientation of the given vector on the drawing plane
        /// </summary>
        /// <param name="vectorStartPoint">origin point of the direction vector</param>
        /// <param name="vectorEndPoint">target point of the direction vector</param>
        /// <returns>the orientation angle, in degree format</returns>
        private double computeOrientationOfVector(Point vectorStartPoint, Point vectorEndPoint)
        {
            double x = vectorEndPoint.X - vectorStartPoint.X;
            double y = vectorEndPoint.Y - vectorStartPoint.Y;
            return Math.Atan( y / x );
        }

        /// <summary>
        /// computes the furthest point on the given line segment that will still pass all previous strike zones when connecting it with the current cursor position in a straight line
        /// </summary>
        /// <param name="cursorPosition">the current cursor position</param>
        /// <param name="orthogonalProjection">the orthogonal projection of the current cursor position onto the active line segment</param>
        /// <param name="strikeZones">list of all strike zones which have to be passed</param>
        /// <param name="lineSegmentStartPoint">starting point of the line segment on which the point has to be found</param>
        /// <param name="lineSegmentEndPoint">ending point of the line segment on which the point has to be found</param>
        /// <returns>the furthest such point or a point with negative coordinates, if there is no such point on the given segment (start and end point excluded)</returns>
        private Point ComputeFurthestCrossingPoint(Point cursorPosition, Point orthogonalProjection, List<Tuple<Point, Point>> strikeZones, Point lineSegmentStartPoint, Point lineSegmentEndPoint)
        {
            Tuple<Point, Point> bsf= new Tuple<Point, Point>(new Point(-1, -1), new Point(-1, -1));
            Double bsfAngle = 180;
            for (int j = 0; j < strikeZones.Count; j++)
            {
                double currentAngle = ComputeAngle(orthogonalProjection, cursorPosition, strikeZones.ElementAt(j).Item2);
                if (currentAngle < bsfAngle)
                {
                    bsfAngle = currentAngle;
                    bsf = strikeZones.ElementAt(j);
                }
            }
            Point furthestCrossingPoint = ComputeCrossingPoint(cursorPosition, bsf.Item2, lineSegmentStartPoint, lineSegmentEndPoint);
            return furthestCrossingPoint;
        }

        private double ComputeAngle(Point point1, Point centerPoint, Point point2)
        {
            return (Math.Abs(computeOrientationOfVector(centerPoint, point1) - computeOrientationOfVector(centerPoint, point2)) % 180);
        }

        private Point ComputeCrossingPoint(Point line1Point1, Point line1Point2, Point line2Point1, Point line2Point2)
        {
            if (line2Point1.Equals(line2Point2)) return new Point(-1, -1);
            double xCoordinateOfCrossingPoint = ( (line1Point1.X * line1Point2.Y - line1Point1.Y * line1Point2.X) * (line2Point1.X - line2Point2.X) - (line1Point1.X - line1Point2.X) * (line2Point1.X * line2Point2.Y - line2Point1.Y * line2Point2.X) ) / ( (line1Point1.X - line1Point2.X) * (line2Point1.Y - line2Point2.Y) - (line1Point1.Y - line1Point2.Y) * (line2Point1.X - line2Point2.X) );
            double yCoordinateOfCrossingPoint = ( (line1Point1.X * line1Point2.Y - line1Point1.Y * line1Point2.X) * (line2Point1.Y - line2Point2.Y) - (line1Point1.Y - line1Point2.Y) * (line2Point1.X * line2Point2.Y - line2Point1.Y * line2Point2.X) ) / ( (line1Point1.X - line1Point2.X) * (line2Point1.Y - line2Point2.Y) - (line1Point1.Y - line1Point2.Y) * (line2Point1.X - line2Point2.X) );
            Point crossingPointOfLines = new Point(xCoordinateOfCrossingPoint, yCoordinateOfCrossingPoint);
            if (BeyondSection(crossingPointOfLines, line1Point1, line1Point2)) return new Point(-1, -1);
            if (BeyondSection(crossingPointOfLines, line1Point2, line1Point1)) return new Point(-1, -1);
            if (BeyondSection(crossingPointOfLines, line2Point1, line2Point2)) return new Point(-1, -1);
            if (BeyondSection(crossingPointOfLines, line2Point2, line2Point1)) return new Point(-1, -1);
            else return crossingPointOfLines;
        }

        /// <summary>
        /// checks if all strike zones are passed by the trajectory given by the straight line from the cursor position to the next target point 
        /// </summary>
        /// <param name="strikeZones">list of all already computed strike zones</param>
        /// <param name="cursorPosition">the current cursor position</param>
        /// <param name="targetPoint">index of the next target point</param>
        /// <returns>true if all strike zones are passed, else false</returns>
        private bool allStrikeZonesPassed(List<Tuple<Point, Point>> strikeZones, Point cursorPosition, Point orthogonalProjection, Point targetPoint)
        {
            Point lastPoint = orthogonalProjection;
            foreach (Tuple<Point, Point> s in strikeZones )
            {
                if (!SectionsCrossing(cursorPosition, targetPoint, s.Item1, s.Item2)) return false; //strike zone not passed
                if (SectionsCrossing(cursorPosition, targetPoint, lastPoint, s.Item1)) return false; //line crossed
                lastPoint = s.Item1;
            }
            return true;
        }

        private bool SectionsCrossing(Point section1StartingPoint, Point section1EndingPoint, Point section2StartingPoint, Point section2EndingPoint)
        {
            Point crossingPoint = ComputeCrossingPoint(section1StartingPoint, section1EndingPoint, section2StartingPoint, section2EndingPoint);
            if (BeyondSection(crossingPoint, section1StartingPoint, section1EndingPoint)) return false;
            if (BeyondSection(crossingPoint, section1EndingPoint, section1StartingPoint)) return false;
            if (BeyondSection(crossingPoint, section2StartingPoint, section2EndingPoint)) return false;
            if (BeyondSection(crossingPoint, section2EndingPoint, section2StartingPoint)) return false;
            return true;
        }

        /// <summary>
        /// computes the strike zone for a point using the cursor position, its orthogonal projection onto the active line segment and tunable constants
        /// </summary>
        /// <param name="targetedPoint">the point to compute the strike zone of</param>
        /// <param name="orthogonalProjection">orthogonal projection of the cursor position onto the active line segment</param>
        /// <param name="cursorPosition">the current cursor position</param>
        /// <returns></returns>
        private Tuple<Point, Point> computeStrikeZone(Point lastPoint, Point targetedPoint, Point nextPoint, Point orthogonalProjection, Point cursorPosition)
        {
            if (lastPoint.X == -1 || nextPoint.X == -1 || ComputeDistance(cursorPosition, orthogonalProjection) < DEADZONE)
            {
                return new Tuple<Point, Point>(targetedPoint, targetedPoint);
            }
            double size = ComputeStrikeZoneSize(ComputeDistance(cursorPosition, orthogonalProjection), ComputeDistance(orthogonalProjection, targetedPoint)); //TODO correct distance to targetPoint
            double v_x = (lastPoint.X - targetedPoint.X) / ComputeDistance(lastPoint, targetedPoint);
            double v_y = (lastPoint.Y - targetedPoint.Y) / ComputeDistance(lastPoint, targetedPoint);
            //double u_x = (nextPoint.X - targetedPoint.X) / ComputeDistance(nextPoint, targetedPoint);
            //double u_y = (nextPoint.Y - targetedPoint.Y) / ComputeDistance(nextPoint, targetedPoint);
            //double tmp_x = v_x + u_x;
            //double tmp_y = v_y + u_y;
            double tmp_x = v_y;
            double tmp_y = v_x;
            double tmp_length = Math.Sqrt(tmp_x * tmp_x + tmp_y * tmp_y);
            tmp_x = ((tmp_x / tmp_length) * size) + targetedPoint.X;
            tmp_y = ((tmp_y / tmp_length) * size) + targetedPoint.Y;
            return new Tuple<Point, Point>(targetedPoint, new Point(tmp_x, tmp_y)); //TODO right direction of vector
        }

        private double ComputeStrikeZoneSize(double v1, double v2)
        {
            return 25; //TODO
        }

        /// <summary>
        /// moves a point a given distance along a vector defined by two points
        /// </summary>
        /// <param name="pointToBeMoved">the point to be moved along the line</param>
        /// <param name="lineStartPoint">origin point of the direction vector</param>
        /// <param name="lineEndPoint">target point of the direction vector</param>
        /// <param name="distance">distance by which to move the point</param>
        /// <returns>a new point that is located distance away from pointToBeMoved in the direction of the given vector</returns>
        private Point MoveAlongLine(Point pointToBeMoved, Point lineStartPoint, Point lineEndPoint, double distance)
        {
            double xOffset = lineEndPoint.X - lineStartPoint.X;
            double yOffset = lineEndPoint.Y - lineStartPoint.Y;
            double vectorLength = ComputeDistance(lineStartPoint, lineEndPoint);
            xOffset /= vectorLength;
            xOffset *= distance;
            yOffset /= vectorLength;
            yOffset *= distance;
            return new Point(pointToBeMoved.X + xOffset, pointToBeMoved.Y + yOffset);
        }

        /// <summary>
        /// computes the euclidean distance between two points
        /// </summary>
        /// <param name="point1">point 1</param>
        /// <param name="point2">point 2</param>
        /// <returns>euclidean distance between point1 and point2</returns>
        private double ComputeDistance(Point point1, Point point2)
        {
            return Math.Sqrt( (double) ( Math.Pow((point2.X - point1.X), 2) + Math.Pow((point2.Y - point1.Y), 2) ) );
        }

        /// <summary>
        /// computes the orthogonal projection of a point onto the given line segment
        /// </summary>
        /// <param name="cursorPosition">the current cursor position</param>
        /// <param name="lastPoint">beginning point of the current section</param>
        /// <param name="currentPoint">ending point of current section</param>
        /// <returns>the orthogonal projection of a point onto the given line segment, or the respective segment end point if the orthogonal projection lies outside the specified segment</returns>
        private Point ComputeOrthogonalProjection(Point cursorPosition, Point lastPoint, Point currentPoint)
        {
            double v_x = cursorPosition.X - lastPoint.X;
            double v_y= cursorPosition.Y - lastPoint.Y;
            double u_x = currentPoint.X - lastPoint.X;
            double u_y = currentPoint.Y - lastPoint.Y;
            double factor = (v_x * u_x + v_y * u_y) / (u_x * u_x + u_y * u_y);
            double new_x = factor * u_x + lastPoint.X;
            double new_y = factor * u_y + lastPoint.Y;

            Point orthogonalProjection = new Point(new_x, new_y);
            Console.Write(orthogonalProjection);
            if (BeyondSection(orthogonalProjection, lastPoint, currentPoint))
            {
                Console.WriteLine("n");
                return currentPoint;
            }
            if (BeyondSection(orthogonalProjection, currentPoint, lastPoint))
            {
                Console.WriteLine("l");
                return lastPoint;
            }
            Console.WriteLine("o");
            return orthogonalProjection;
        }

        /// <summary>
        /// checks if a Point lies on a given line section or beyond it, works only if pointToTest lies on the line defined by sectionStartingPoint and sectionEndingPoint
        /// </summary>
        /// <param name="pointToTest">the Point to test</param>
        /// <param name="sectionStartingPoint">the starting point of the section</param>
        /// <param name="sectionEndingPoint">the ending point of the section</param>
        /// <returns>true iff pointToTest lies beyond sectionEndingPoint, on the line defined by sectionStartingPoint and sectionEndingPoint</returns>
        private bool BeyondSection(Point pointToTest, Point sectionStartingPoint, Point sectionEndingPoint)
        {
            //TODO
            if(sectionEndingPoint.X >= sectionStartingPoint.X && pointToTest.X > sectionEndingPoint.X)
            {
                return true;
            }
            else if(sectionEndingPoint.X <= sectionStartingPoint.X && pointToTest.X < sectionEndingPoint.X)
            {
                return true;
            }
            else
            {
                if (sectionEndingPoint.Y > sectionStartingPoint.Y)
                {
                    if (pointToTest.Y > sectionEndingPoint.Y) return true;
                }
                else if (sectionEndingPoint.Y <= sectionStartingPoint.Y)
                {
                    if (pointToTest.Y < sectionEndingPoint.Y) return true;
                }
            }
            return false;
        }

        /// <summary>
        /// checks if the angle dividing line between this section and the next one has been passed
        /// </summary>
        /// <param name="cursorPosition">the current cursor position</param>
        /// <param name="lastPoint">beginning point of the current section</param>
        /// <param name="currentPoint">ending point of current and beginning point of next section</param>
        /// <param name="nextPoint">ending point of next section</param>
        /// <returns>true iff cursorPosition is closer to the next section than to the current one</returns>
        private bool SectionDividingLinePassed(Point cursorPosition, Point lastPoint, Point currentPoint, Point nextPoint)
        {
            //compute a point at the same distance to the dividing line as nextPoint, but on the other side of the line
            Point auxiliaryPoint = MoveAlongLine(currentPoint, currentPoint, lastPoint, ComputeDistance(currentPoint, nextPoint));
            //line passed iff cursorPosition closer to nextPoint than to auxiliaryPoint
            return ComputeDistance(cursorPosition, nextPoint) <= ComputeDistance(cursorPosition, auxiliaryPoint);
        }
    }
}
