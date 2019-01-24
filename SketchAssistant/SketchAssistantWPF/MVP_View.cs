﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SketchAssistantWPF
{
    public interface MVP_View
    {

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
        /// Displays an image in the left Picture box.
        /// </summary>
        /// <param name="img">The new image.</param>
        void DisplayInLeftPictureBox(Image img);

        /// <summary>
        /// Displays an image in the right Picture box.
        /// </summary>
        /// <param name="img">The new image.</param>
        void DisplayInRightPictureBox(Image img);

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
