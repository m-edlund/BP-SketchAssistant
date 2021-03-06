﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SketchAssistantWPF
{
    public class SketchAction
    {
        //Types of possible actions
        public enum ActionType
        {
            Draw,
            Delete,
            Start
        }
        //Type of this action
        private ActionType thisAction;
        //ID of the Line affected
        private HashSet<int> lineIDs;

        /// <summary>
        /// Constructor for a new action with multiple lines affected.
        /// </summary>
        /// <param name="theAction">The type of action, if it is ActionType.Start the affectedIDs will be ignored.</param>
        /// <param name="affectedID">The IDs of the lines affected.</param>
        public SketchAction(ActionType theAction, HashSet<int> affectedIDs)
        {
            thisAction = theAction;
            if (theAction.Equals(ActionType.Start)) { lineIDs = new HashSet<int>(); }
            else { lineIDs = new HashSet<int>(affectedIDs); }
        }

        /// <summary>
        /// Constructor for a new action with one line affected.
        /// </summary>
        /// <param name="theAction">The type of action, if it is ActionType.Start the affectedID will be ignored.</param>
        /// <param name="affectedID">The ID of the affected line.</param>
        public SketchAction(ActionType theAction, int affectedID)
        {
            thisAction = theAction;
            if (theAction.Equals(ActionType.Start)) { lineIDs = new HashSet<int>(); }
            else
            {
                lineIDs = new HashSet<int>();
                lineIDs.Add(affectedID);
            }
        }

        /// <summary>
        /// Fetches the type of this action.
        /// </summary>
        /// <returns>The type of this action.</returns>
        public ActionType GetActionType()
        {
            return thisAction;
        }

        /// <summary>
        /// Fetches the IDs of the lines affected by this action.
        /// </summary>
        /// <returns>The IDs of the lines affected by this action. An empty set if there is no line affected.</returns>
        public HashSet<int> GetLineIDs()
        {
            return lineIDs;
        }

        /// <summary>
        /// Get the information about this action.
        /// </summary>
        /// <returns>A String describing what happend at this action.</returns>
        public String GetActionInformation()
        {
            String returnString = "";
            switch (thisAction)
            {
                case ActionType.Start:
                    returnString = "A new canvas was created.";
                    break;
                case ActionType.Draw:
                    returnString = "Line number " + lineIDs.First().ToString() + " was drawn.";
                    break;
                case ActionType.Delete:
                    if (lineIDs.Count == 1) { returnString = "Line number " + lineIDs.First().ToString() + " was deleted."; }
                    else
                    {
                        returnString = "Several Lines were deleted.";
                    }
                    break;
            }
            return returnString;
        }
    }
}
