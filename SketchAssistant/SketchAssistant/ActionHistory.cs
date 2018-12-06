using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SketchAssistant
{
    public class ActionHistory
    {
        //History of Actions taken
        List<SketchAction> actionHistory;
        //The current position in the actionHistory
        Tuple<int, SketchAction> currentAction;

        public ActionHistory()
        {
            actionHistory = new List<SketchAction>();
            currentAction = new Tuple<int, SketchAction>(-1, null);
            AddNewAction(new SketchAction(SketchAction.ActionType.Start, -1));
        }

        /// <summary>
        /// Adds a new action to the action history.
        /// </summary>
        /// <param name="newAction">The newly added action.</param>
        public void AddNewAction(SketchAction newAction)
        {
            //The current Action is before the last action taken, delete everything after the current action.
            if (currentAction.Item1 < actionHistory.Count - 1)
            {
                actionHistory.RemoveRange(currentAction.Item1 + 1, actionHistory.Count - currentAction.Item1 + 1);
            }
            actionHistory.Add(newAction);
            currentAction = new Tuple<int, SketchAction>(actionHistory.Count - 1, newAction);
        }

        /// <summary>
        /// Returns whether or not the history is empty.
        /// </summary>
        /// <returns>true if the history is empty, otherwise false</returns>
        public bool IsEmpty()
        {
            if (actionHistory.Count == 1) { return true; }
            else { return false; }
        }
    }
}
