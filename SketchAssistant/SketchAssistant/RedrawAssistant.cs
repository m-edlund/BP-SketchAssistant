﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SketchAssistant
{
    public class RedrawAssistant
    {
        /// <summary>
        /// The lines of the left image, with a boolean indicating if they have been redrawn
        /// and an integer that is the same as the line id of the respective line in the right image.
        /// </summary>
        List<Tuple<Line, bool, int>> linesToRedraw;
        /// <summary>
        /// The Start and End points of all lines in linesToRedraw in the same order.
        /// </summary>
        List<Tuple<HashSet<Point>, HashSet<Point>>> startAndEndPoints;
        /// <summary>
        /// A Hashtable for quick lookup for a line id and its respective tuple in linesToRedraw
        /// </summary>
        Hashtable redrawnLineLookupTable;
        /// <summary>
        /// The position of the line currently being redrawn in the startAndEndPoints 
        /// & linesToRedraw lists. -1 if no line is being redrawn.
        /// </summary>
        int lineBeingRedrawn;
        /// <summary>
        /// The id of the line being drawn on the right side. -1 if no line is being drawn.
        /// </summary>
        int currentLineID;
        /// <summary>
        /// Whether or not the user is currently redrawing a line.
        /// </summary>
        bool currentlyRedrawing;
        /// <summary>
        /// Whether or not the RedrawAssistant is active.
        /// </summary>
        bool isActive;
        /// <summary>
        /// The radius of the markers for redrawing.
        /// </summary>
        int markerRadius = 5;

        /// <summary>
        /// The Constructor for an inactive RedrawAssistant.
        /// </summary>
        public RedrawAssistant()
        {
            isActive = false;
        }

        /// <summary>
        /// The constructor for an active RedrawAssistant
        /// </summary>
        /// <param name="redrawItem">The lines that shall be redrawn</param>
        public RedrawAssistant(List<Line> redrawItem)
        {
            linesToRedraw = new List<Tuple<Line, bool, int>>();
            startAndEndPoints = new List<Tuple<HashSet<Point>, HashSet<Point>>>();
            isActive = true;
            currentlyRedrawing = false;
            lineBeingRedrawn = -1;
            redrawnLineLookupTable = new Hashtable();
            foreach (Line line in redrawItem)
            {
                linesToRedraw.Add(new Tuple<Line, bool, int>(line, false, -1));
            }
            SetMarkerRadius(5);
        }

        /// <summary>
        /// Initialization function that returns the initial list of overlay points.
        /// </summary>
        /// <param name="mRad">The radius of the points.</param>
        /// <returns>The list of overlay points.</returns>
        public List<Tuple<bool, HashSet<Point>>> Initialize(int mRad)
        {
            if (isActive)
            {
                List<Tuple<bool, HashSet<Point>>> retList = new List<Tuple<bool, HashSet<Point>>>();
                SetMarkerRadius(mRad);
                foreach(Tuple<HashSet<Point>, HashSet<Point>> tup in startAndEndPoints)
                {
                    retList.Add(new Tuple<bool, HashSet<Point>>(false, tup.Item1));
                    retList.Add(new Tuple<bool, HashSet<Point>>(false, tup.Item2));
                }
                return retList;
            }
            return null;
        }

        /// <summary>
        /// The main functionality of the RedrawAssistant, which updates the Assistant according to the inputs given.
        /// </summary>
        /// <param name="currentPoint">The current position of the cursor, as a point</param>
        /// <param name="rightLines">The lines on the right canvas</param>
        /// <param name="currLineID">The id of the line currently being drawn.</param>
        /// <param name="lineFinished">A boolean to indicate that the line is finished</param>
        /// <param name="overlayItems">A list containing the overlay items and if they should be drawn.</param>
        /// <returns>The updated List of overlay items, or the same list if no changes need to be done, 
        /// along with a boolean indicating if something was changed</returns>
        public Tuple<bool, List<Tuple<bool, HashSet<Point>>>> Tick(Point currentPoint, List<Tuple<bool, Line>> rightLines, int currLineID, bool lineFinished, 
            List<Tuple<bool, HashSet<Point>>> overlayItems)
        {
            if (!isActive) { return new Tuple<bool, List<Tuple<bool, HashSet<Point>>>>(false, overlayItems); }

            if (!currentlyRedrawing)
            {

            }
            /*
            Tuple<Line, bool, int> newLineTuple = null;
            var returnAllStartPoints = true;
            CheckForUndrawnLines(rightLines);
            
            // Checking if a startpoint is intersected
            if (!currentlyRedrawing)
            {
                for (int i = 0; i < linesToRedraw.Count; i++)
                {
                    Tuple<Line, bool, int> tup = linesToRedraw[i];
                    if (!tup.Item2)
                    {
                        if (startAndEndPoints[i].Item1.Contains(currentPoint))
                        {
                            currentlyRedrawing = true;
                            lineBeingRedrawn = i;
                            currentLineID = currLineID;
                            returnList.Add(startAndEndPoints[i].Item1);
                            returnList.Add(startAndEndPoints[i].Item2);
                            returnAllStartPoints = false;
                        }
                    }
                }
            }
            //Currently redrawing a line, but a line hasn't been finished drawing.
            else if (!lineFinished)
            {
                returnList.Add(startAndEndPoints[lineBeingRedrawn].Item1);
                returnList.Add(startAndEndPoints[lineBeingRedrawn].Item2);
                returnAllStartPoints = false;
            }
            //Line is finished, check if it is in the correct endpoint
            else if (currLineID == currentLineID && startAndEndPoints[lineBeingRedrawn].Item2.Contains(currentPoint))
            {
                newLineTuple = new Tuple<Line, bool, int>(linesToRedraw[lineBeingRedrawn].Item1, true, currLineID);
                currentlyRedrawing = false;
                lineBeingRedrawn = -1;
                currentLineID = -1;
            }
            //Line is finished, but not in the correct endpoint
            else
            {
                currentlyRedrawing = false;
                lineBeingRedrawn = -1;
                currentLineID = -1;
            }

            //Replace the changed line tuple in linesToRedraw
            if(newLineTuple != null)
            {
                var newLine = newLineTuple.Item1;
                for (int i = 0; i < linesToRedraw.Count; i++)
                {
                    var redrawLine = linesToRedraw[i].Item1;
                    if (redrawLine.GetID() == newLine.GetID() 
                        && redrawLine.GetStartPoint().Equals(newLine.GetStartPoint())
                        && redrawLine.GetEndPoint().Equals(newLine.GetEndPoint()))
                    {
                        redrawnLineLookupTable.Add(currLineID, i);
                        linesToRedraw[i] = newLineTuple;
                    }
                }
            }

            //Add all the startpoints to the list being returned
            if (returnAllStartPoints)
            {
                for (int i = 0; i < linesToRedraw.Count; i++)
                {
                    if (!linesToRedraw[i].Item2)
                    {
                        returnList.Add(startAndEndPoints[i].Item1);
                    }
                }
            }
            return returnList;
            */
            return new Tuple<bool, List<Tuple<bool, HashSet<Point>>>>(false, overlayItems);
        }

        /// <summary>
        /// A helping function which checks for lines where previously redrawn, but were removed from the image again.
        /// </summary>
        /// <param name="rightLines">The lines in the right image.</param>
        private void CheckForUndrawnLines(List<Tuple<bool, Line>> rightLines)
        {
            for (int i = 0; i < rightLines.Count; i++)
            {
                if (redrawnLineLookupTable.ContainsKey(rightLines[i].Item2.GetID()))
                {
                    if (!rightLines[i].Item1)
                    {
                        int listPos = (int)redrawnLineLookupTable[rightLines[i].Item2.GetID()];
                        var oldTup = linesToRedraw[listPos];
                        linesToRedraw[listPos] = new Tuple<Line, bool, int>(oldTup.Item1, false, -1);
                    }
                    else
                    {
                        int listPos = (int)redrawnLineLookupTable[rightLines[i].Item2.GetID()];
                        var oldTup = linesToRedraw[listPos];
                        linesToRedraw[listPos] = new Tuple<Line, bool, int>(oldTup.Item1, true, rightLines[i].Item2.GetID());
                    }
                }
            }
        }

        /// <summary>
        /// A function to set the marker radius for the markers returned by the RedrawAssistant
        /// </summary>
        /// <param name="markerRad">The Radius of the markers.</param>
        public void SetMarkerRadius(int markerRad)
        {
            markerRadius = markerRad;
            if (isActive)
            {
                startAndEndPoints = new List<Tuple<HashSet<Point>, HashSet<Point>>>();
                foreach (Tuple<Line, bool, int> tup in linesToRedraw)
                {
                    startAndEndPoints.Add(CalculateStartAndEnd(tup.Item1));
                }
            }
        }

        /// <summary>
        /// Will calculate the start and endpoints of the given line.
        /// </summary>
        /// <param name="line">The given line.</param>
        private Tuple<HashSet<Point>, HashSet<Point>> CalculateStartAndEnd(Line line)
        {
            var circle0 = GeometryCalculator.FilledCircleAlgorithm(line.GetStartPoint(), markerRadius);
            var circle1 = GeometryCalculator.FilledCircleAlgorithm(line.GetEndPoint(), markerRadius);
            var currentLineEndings = new Tuple<HashSet<Point>, HashSet<Point>>(circle0, circle1);
            return currentLineEndings;
        }
    }
}
