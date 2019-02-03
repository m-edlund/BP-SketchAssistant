using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

// This is the code for your desktop app.
// Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.

namespace SketchAssistant
{
    public partial class Form1 : Form, MVP_View
    {
        public Form1()
        {
            InitializeComponent();
            ProgramPresenter = new MVP_Presenter(this);
        }

        /**********************************/
        /*** CLASS VARIABLES START HERE ***/
        /**********************************/

        public enum ButtonState
        {
            Enabled,
            Disabled,
            Active
        }

        /// <summary>
        /// Different Program States
        /// </summary>
        public enum ProgramState
        {
            Idle,
            Draw,
            Delete
        }

        /// <summary>
        /// Dialog to select a file.
        /// </summary>
        OpenFileDialog openFileDialog = new OpenFileDialog();
        /// <summary>
        /// All Lines in the current session
        /// </summary>
        List<Tuple<bool, Line>> rightLineList = new List<Tuple<bool, Line>>();
        /// <summary>
        /// Queue for the cursorPositions
        /// </summary>
        Queue<Point> cursorPositions = new Queue<Point>();
        /// <summary>
        /// The Presenter Component of the MVP-Model
        /// </summary>
        MVP_Presenter ProgramPresenter;

        /******************************************/
        /*** FORM SPECIFIC FUNCTIONS START HERE ***/
        /******************************************/

        private void Form1_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
        }

        /// <summary>
        /// Resize Function connected to the form resize event, will refresh the form when it is resized
        /// </summary>
        private void Form1_Resize(object sender, System.EventArgs e)
        {
            ProgramPresenter.Resize(new Tuple<int, int>(pictureBoxLeft.Width, pictureBoxLeft.Height), 
                new Tuple<int, int>(pictureBoxRight.Width, pictureBoxRight.Height));
            this.Refresh();
        }

        /// <summary>
        /// Import example picture button, will open an OpenFileDialog
        /// </summary>
        private void examplePictureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProgramPresenter.ExamplePictureToolStripMenuItemClick();
        }

        /// <summary>
        /// Import svg drawing button, will open an OpenFileDialog
        /// </summary>
        private void SVGDrawingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProgramPresenter.SVGToolStripMenuItemClick();
        }

        /// <summary>
        /// Changes the state of the program to drawing
        /// </summary>
        private void drawButton_Click(object sender, EventArgs e)
        {
            ProgramPresenter.ChangeState(true);
        }

        /// <summary>
        /// Changes the state of the program to deletion
        /// </summary>
        private void deleteButton_Click(object sender, EventArgs e)
        {
            ProgramPresenter.ChangeState(false);
        }

        /// <summary>
        /// Undo an Action.
        /// </summary>
        private void undoButton_Click(object sender, EventArgs e)
        {
            ProgramPresenter.Undo();
        }

        /// <summary>
        /// Redo an Action.
        /// </summary>
        private void redoButton_Click(object sender, EventArgs e)
        {
            ProgramPresenter.Redo();
        }

        /// <summary>
        /// Detect Keyboard Shortcuts.
        /// </summary>
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Z)
            {
                ProgramPresenter.Undo();
            }
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.Y)
            {
                ProgramPresenter.Redo();
            }
        }

        /// <summary>
        /// The Picture box is clicked.
        /// </summary>
        private void pictureBoxRight_Click(object sender, EventArgs e)
        {
            ProgramPresenter.MouseEvent(MVP_Presenter.MouseAction.Click);
        }

        /// <summary>
        /// Get current Mouse positon within the right picture box.
        /// </summary>
        private void pictureBoxRight_MouseMove(object sender, MouseEventArgs e)
        {
            ProgramPresenter.MouseEvent(MVP_Presenter.MouseAction.Move, e);
        }
        
        /// <summary>
        /// Hold left mouse button to start drawing.
        /// </summary>
        private void pictureBoxRight_MouseDown(object sender, MouseEventArgs e)
        {
            ProgramPresenter.MouseEvent(MVP_Presenter.MouseAction.Down);
        }
        
        /// <summary>
        /// Lift left mouse button to stop drawing and add a new Line.
        /// </summary>
        private void pictureBoxRight_MouseUp(object sender, MouseEventArgs e)
        {
            ProgramPresenter.MouseEvent(MVP_Presenter.MouseAction.Up);
        }
        
        /// <summary>
        /// Button to create a new Canvas. Will create an empty image 
        /// which is the size of the left image, if there is one.
        /// If there is no image loaded the canvas will be the size of the right picture box
        /// </summary>
        private void canvasButton_Click(object sender, EventArgs e)
        {
            ProgramPresenter.NewCanvas();
        }

        /// <summary>
        /// Add a Point on every tick to the Drawpath.
        /// Or detect lines for deletion on every tick
        /// </summary>
        private void mouseTimer_Tick(object sender, EventArgs e)
        {
            ProgramPresenter.Tick();
        }

        /*************************/
        /*** PRESENTER -> VIEW ***/
        /*************************/

        /// <summary>
        /// Enables the timer of the View, which will tick the Presenter.
        /// </summary>
        public void EnableTimer()
        {
            mouseTimer.Enabled = true;
        }

        /// <summary>
        /// A function that opens a file dialog and returns the filename.
        /// </summary>
        /// <param name="Filter">The filter that should be applied to the new Dialog.</param>
        /// <returns>Returns the FileName and the SafeFileName if the user correctly selects a file, 
        /// else returns a tuple with empty strigns</returns>
        public Tuple<String, String> openNewDialog(String Filter)
        {
            openFileDialog.Filter = Filter;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                return new Tuple<string, string>(openFileDialog.FileName, openFileDialog.SafeFileName);
            }
            else
            {
                return new Tuple<string, string>("", "");
            }
        }

        /// <summary>
        /// Sets the contents of the load status indicator label.
        /// </summary>
        /// <param name="message">The new contents</param>
        public void SetToolStripLoadStatus(String message)
        {
            toolStripLoadStatus.Text = message;
        }

        /// <summary>
        /// Sets the contents of the last action taken indicator label.
        /// </summary>
        /// <param name="message">The new contents</param>
        public void SetLastActionTakenText(String message)
        {
            lastActionTakenLabel.Text = message;
        }

        /// <summary>
        /// Changes the states of a tool strip button.
        /// </summary>
        /// <param name="buttonName">The name of the button.</param>
        /// <param name="state">The new state of the button.</param>
        public void SetToolStripButtonStatus(String buttonName, ButtonState state)
        {
            ToolStripButton buttonToChange;
            switch (buttonName)
            {
                case "canvasButton":
                    buttonToChange = canvasButton;
                    break;
                case "drawButton":
                    buttonToChange = drawButton;
                    break;
                case "deleteButton":
                    buttonToChange = deleteButton;
                    break;
                case "undoButton":
                    buttonToChange = undoButton;
                    break;
                case "redoButton":
                    buttonToChange = redoButton;
                    break;
                default:
                    Console.WriteLine("Invalid Button was given to SetToolStripButton. \nMaybe you forgot to add a case?");
                    return;
            }
            switch (state)
            {
                case ButtonState.Active:
                    buttonToChange.Checked = true;
                    break;
                case ButtonState.Disabled:
                    buttonToChange.Checked = false;
                    buttonToChange.Enabled = false;
                    break;
                case ButtonState.Enabled:
                    buttonToChange.Checked = false;
                    buttonToChange.Enabled = true;
                    break;
            }
        }

        /// <summary>
        /// Displays an image in the left Picture box.
        /// </summary>
        /// <param name="img">The new image.</param>
        public void DisplayInLeftPictureBox(Image img)
        {
            pictureBoxLeft.Image = img;
            pictureBoxLeft.Refresh();
        }

        /// <summary>
        /// Displays an image in the right Picture box.
        /// </summary>
        /// <param name="img">The new image.</param>
        public void DisplayInRightPictureBox(Image img)
        {
            pictureBoxRight.Image = img;
            pictureBoxRight.Refresh();
        }

        /// <summary>
        /// shows the given info message in a popup and asks the user to aknowledge it
        /// </summary>
        /// <param name="message">the message to show</param>
        public void ShowInfoMessage(String message)
        {
            MessageBox.Show(message);
        }

        /// <summary>
        /// Shows a warning box with the given message (Yes/No Buttons)and returns true if the user aknowledges it.
        /// </summary>
        /// <param name="message">The message of the warning.</param>
        /// <returns>True if the user confirms (Yes), negative if he doesn't (No)</returns>
        public bool ShowWarning(String message)
        {
            return (MessageBox.Show(message, "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes);
        }

    }
}
