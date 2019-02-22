using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SketchAssistantWPF
{
    class TrajectoryGenerator
    {
        static int constantA= 10;

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

        /// <summary>
        /// updates the pointer to the template for the line currently being drawn, and resets indices etc.
        /// </summary>
        /// <param name="newCurrentLine">the new template for the line currently being drawn</param>
        public void setCurrentLine(InternalLine newCurrentLine)
        {
            currentLine = newCurrentLine;
            currentPoints = currentLine.GetPoints();
            //lastCursorPosition = currentPoints.ElementAt(0);
            index = 1;
        }

        /// <summary>
        /// generates the new trajectory back to the template based on the current cursor position
        /// </summary>
        /// <param name="cursorPosition">the current cursor position</param>
        /// <returns>the direction in which to go, as an angle on the drawing plane, in degree format, with 0° being the positive X-Axis, increasing counterclockwise</returns>
        public double GenerateTrajectory(Point cursorPosition)
        {

            //update index to point to current section if one or more section divideing lines have been passed since last call
            while (index < (currentPoints.Count - 1) && SectionDividingLinePassed(cursorPosition, currentPoints.ElementAt(index - 1), currentPoints.ElementAt(index), currentPoints.ElementAt(index + 1)))
            {
                index++;
            }
            //lastCursorPosition = cursorPosition;

            //project teh point onto the active line segment to be able to compute distances
            Point orthogonalProjection = ComputeOrthogonalProjection(cursorPosition, currentPoints.ElementAt(index - 1), currentPoints.ElementAt(index));

            //index of the last reachable actual point
            int targetIndex = index;
            List<Tuple<Point, Point>> strikeZones = new List<Tuple<Point, Point>>();

            //if "far" away from the next actual point of the line, generate an auxiliary point at a constant distance (constantA) on the current line segment
            Point auxiliaryPoint = new Point(-1, -1);
            if (ComputeDistance(orthogonalProjection, currentPoints.ElementAt(index)) <= constantA)
            {
                auxiliaryPoint = MoveAlongLine(orthogonalProjection, currentPoints.ElementAt(index - 1), currentPoints.ElementAt(index), constantA);
                strikeZones.Add(computeStrikeZone(auxiliaryPoint, orthogonalProjection, cursorPosition));
                targetIndex--;
            }

            //aim for the furthest actual point of the line reachable by the descent rate constraints (lower bounds) given by the various strike zones
            while (targetIndex < (currentPoints.Count - 1) && allStrikeZonesPassed(strikeZones, cursorPosition, currentPoints.ElementAt(targetIndex + 1)))
            {
                strikeZones.Add(computeStrikeZone(currentPoints.ElementAt(targetIndex + 1), orthogonalProjection, cursorPosition));
                targetIndex++;
            }

            Point furthestCrossingPoint = new Point(-1, -1); ;
            if (targetIndex < index) //auxiliary point created and next actual point not reachable
            {
                furthestCrossingPoint = ComputeFurthestCrossingPoint(cursorPosition, strikeZones, auxiliaryPoint, currentPoints.ElementAt(targetIndex + 1));

                //if such a point exists, use it as target for the new trajectory
                if (furthestCrossingPoint.X != -1)
                {
                    Debug_DrawStrikeZones(strikeZones);
                    Debug_DrawTrajectoryVector(cursorPosition, furthestCrossingPoint);
                    return computeOrientationOfVector(cursorPosition, furthestCrossingPoint);
                }
                //else use the last reachable actual point
                else
                {
                    Debug_DrawStrikeZones(strikeZones);
                    Debug_DrawTrajectoryVector(cursorPosition, auxiliaryPoint);
                    return computeOrientationOfVector(cursorPosition, auxiliaryPoint);
                }
            }
            else 
            {
                //aim for the furthest (auxiliary) point on the line segment after the last reachable actual point (only if there is such a segment: not if that last reachable point is the last point of the line)
                if (targetIndex < (currentPoints.Count - 1))
                {
                    furthestCrossingPoint = ComputeFurthestCrossingPoint(cursorPosition, strikeZones, currentPoints.ElementAt(targetIndex), currentPoints.ElementAt(targetIndex + 1));
                }
                //if such a point exists, use it as target for the new trajectory
                if (furthestCrossingPoint.X != -1)
                {
                    Debug_DrawStrikeZones(strikeZones);
                    Debug_DrawTrajectoryVector(cursorPosition, furthestCrossingPoint);
                    return computeOrientationOfVector(cursorPosition, furthestCrossingPoint);
                }
                //else use the last reachable actual point
                else
                {
                    Debug_DrawStrikeZones(strikeZones);
                    Debug_DrawTrajectoryVector(cursorPosition, currentPoints.ElementAt(targetIndex));
                    return computeOrientationOfVector(cursorPosition, currentPoints.ElementAt(targetIndex));
                }
            }
        }

        /// <summary>
        /// prints the trajectory vector on the drawing pane for debugging and calibration purposes
        /// </summary>
        /// <param name="vectorStartPoint">origin point of the trajectory vector</param>
        /// <param name="vectorEndPoint">target point of the trajectory vector</param>
        private void Debug_DrawTrajectoryVector(Point vectorStartPoint, Point vectorEndPoint)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// prints all strike zones on the drawing pane for debugging and calibration purposes
        /// </summary>
        /// <param name="strikeZones">list of all strike zones to be drawn</param>
        private void Debug_DrawStrikeZones(List<Tuple<Point, Point>> strikeZones)
        {
            throw new NotImplementedException();
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
        /// <param name="strikeZones">list of all strike zones which have to be passed</param>
        /// <param name="lineSegmentStartPoint">starting point of the line segment on which the point has to be found</param>
        /// <param name="lineSegmentEndPoint">ending point of the line segment on which the point has to be found</param>
        /// <returns>the furthest such point or null, if there is no such point on the given segment (start and end point excluded)</returns>
        private Point ComputeFurthestCrossingPoint(Point cursorPosition, List<Tuple<Point, Point>> strikeZones, Point lineSegmentStartPoint, Point lineSegmentEndPoint)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// checks if all strike zones are passed by the trajectory given by the straight line from the cursor position to the next target point 
        /// </summary>
        /// <param name="strikeZones">list of all already computed strike zones</param>
        /// <param name="cursorPosition">the current cursor position</param>
        /// <param name="targetIndex">index of the next target point</param>
        /// <returns>true if all strike zones are passed, else false</returns>
        private bool allStrikeZonesPassed(List<Tuple<Point, Point>> strikeZones, Point cursorPosition, Point targetIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// computes the strike zone for a point using the cursor position, its orthogonal projection onto the active line segment and tunable constants
        /// </summary>
        /// <param name="targetedPoint">the point to compute the strike zone of</param>
        /// <param name="orthogonalProjection">orthogonal projection of the cursor position onto the active line segment</param>
        /// <param name="cursorPosition">the current cursor position</param>
        /// <returns></returns>
        private Tuple<Point, Point> computeStrikeZone(Point targetedPoint, Point orthogonalProjection, Point cursorPosition)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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
