using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SketchAssistantWPF
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
        /// <returns>The message to be displayed</returns>
        public String AddNewAction(SketchAction newAction)
        {
            //The current Action is before the last action taken, delete everything after the current action.
            if (currentAction.Item1 < actionHistory.Count - 1)
            {
                actionHistory.RemoveRange(currentAction.Item1 + 1, actionHistory.Count - (currentAction.Item1 + 1));
            }
            actionHistory.Add(newAction);
            currentAction = new Tuple<int, SketchAction>(actionHistory.Count - 1, newAction);
            return UpdateStatusLabel();
        }

        /// <summary>
        /// Changes the currentAction.
        /// </summary>
        /// <param name="moveBack">If True, moves the current action back one slot, if False, moves it forward.</param>
        /// <returns>The message to be displayed</returns>
        public String MoveAction(bool moveBack)
        {
            if (moveBack && CanUndo())
            {
                currentAction = new Tuple<int, SketchAction>(currentAction.Item1 - 1, actionHistory[currentAction.Item1 - 1]);
            }
            if (!moveBack && CanRedo())
            {
                currentAction = new Tuple<int, SketchAction>(currentAction.Item1 + 1, actionHistory[currentAction.Item1 + 1]);
            }
            return UpdateStatusLabel();
        }

        /// <summary>
        /// Returns the current action.
        /// </summary>
        /// <returns>The current action.</returns>
        public SketchAction GetCurrentAction()
        {
            return currentAction.Item2;
        }

        /// <summary>
        /// Return whether or not an action can be undone.
        /// </summary>
        /// <returns>True if an action can be undone.</returns>
        public bool CanUndo()
        {
            if (currentAction.Item1 > 0) { return true; }
            else { return false; }
        }

        /// <summary>
        /// Return whether or not an action can be redone.
        /// </summary>
        /// <returns>True if an action can be redone.</returns>
        public bool CanRedo()
        {
            if (currentAction.Item1 < actionHistory.Count - 1) { return true; }
            else { return false; }
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

        /// <summary>
        /// Updates the status label if there is one given.
        /// </summary>
        /// <returns>The message to be displayed</returns>
        private String UpdateStatusLabel()
        {
            return "Last Action: " + currentAction.Item2.GetActionInformation();
        }
    }
}
