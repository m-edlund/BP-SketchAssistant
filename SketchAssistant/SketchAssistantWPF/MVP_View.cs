using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace SketchAssistantWPF
{
    public interface MVP_View
    {

        /// <summary>
        /// Remove the current line.
        /// </summary>
        void RemoveCurrLine();

        /// <summary>
        /// Display the current line.
        /// </summary>
        /// <param name="line">The current line to display</param>
        void DisplayCurrLine(Polyline line);

        /// <summary>
        /// Removes all Lines from the left canvas.
        /// </summary>
        void RemoveAllLeftLines();

        /// <summary>
        /// Removes all lines in the right canvas.
        /// </summary>
        void RemoveAllRightLines();

        /// <summary>
        /// Adds another Line that will be displayed in the left display.
        /// </summary>
        /// <param name="newLine">The new Polyline to be added displayed.</param>
        void AddNewLineLeft(Polyline newLine);

        /// <summary>
        /// Adds another Line that will be displayed in the right display.
        /// </summary>
        /// <param name="newLine">The new Polyline to be added displayed.</param>
        void AddNewLineRight(Polyline newLine);

        /// <summary>
        /// Enables the timer of the View, which will tick the Presenter.
        /// </summary>
        void EnableTimer();

        /// <summary>
        /// A function that opens a file dialog and returns the filename.
        /// </summary>
        /// <param name="Filter">The filter that should be applied to the new Dialog.</param>
        /// <returns>Returns the FileName and the SafeFileName if the user correctly selects a file, 
        /// else returns a tuple with empty strigns</returns>
        Tuple<String, String> openNewDialog(String Filter);

        /// <summary>
        /// Sets the contents of the load status indicator label.
        /// </summary>
        /// <param name="message">The new contents</param>
        void SetToolStripLoadStatus(String message);

        /// <summary>
        /// Sets the contents of the last action taken indicator label.
        /// </summary>
        /// <param name="message">The new contents</param>
        void SetLastActionTakenText(String message);

        /// <summary>
        /// Changes the states of a tool strip button.
        /// </summary>
        /// <param name="buttonName">The name of the button.</param>
        /// <param name="state">The new state of the button.</param>
        void SetToolStripButtonStatus(String buttonName, MainWindow.ButtonState state);

        /// <summary>
        /// shows the given info message in a popup and asks the user to aknowledge it
        /// </summary>
        /// <param name="message">the message to show</param>
        void ShowInfoMessage(String message);

        /// <summary>
        /// Shows a warning box with the given message (Yes/No Buttons)and returns true if the user aknowledges it.
        /// </summary>
        /// <param name="message">The message of the warning.</param>
        /// <returns>True if the user confirms (Yes), negative if he doesn't (No)</returns>
        bool ShowWarning(String message);
    }
}
