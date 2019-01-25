using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SketchAssistantWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, MVP_View
    {
        public MainWindow()
        {
            InitializeComponent();
            ProgramPresenter = new MVP_Presenter(this);
            //  DispatcherTimer setup
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 33);
        }

        public enum ButtonState
        {
            Enabled,
            Disabled,
            Active
        }

        DispatcherTimer dispatcherTimer;
        /// <summary>
        /// Dialog to select a file.
        /// </summary>
        OpenFileDialog openFileDialog = new OpenFileDialog();
        /// <summary>
        /// All Lines in the current session
        /// </summary>
        List<Tuple<bool, InternalLine>> rightLineList = new List<Tuple<bool, InternalLine>>();
        /// <summary>
        /// Queue for the cursorPositions
        /// </summary>
        Queue<Point> cursorPositions = new Queue<Point>();
        /// <summary>
        /// The Presenter Component of the MVP-Model
        /// </summary>
        MVP_Presenter ProgramPresenter;

        /********************************************/
        /*** WINDOW SPECIFIC FUNCTIONS START HERE ***/
        /********************************************/

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void RedoButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DrawButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RightCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void RightCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void RightCanvas_MouseMove(object sender, MouseEventArgs e)
        {

        }

        /// <summary>
        /// Button to create a new Canvas. Will create an empty image 
        /// which is the size of the left image, if there is one.
        /// If there is no image loaded the canvas will be the size of the right picture box
        /// </summary>
        private void CanvasButton_Click(object sender, RoutedEventArgs e)
        {
            ProgramPresenter.NewCanvas();
        }

        /// <summary>
        /// Ticks the Presenter.
        /// </summary>
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            ProgramPresenter.Tick();
        }

        /*************************/
        /*** PRESENTER -> VIEW ***/
        /*************************/

        /// <summary>
        /// Displays a list of lines in the left Picture box.
        /// </summary>
        /// <param name="img">The new image.</param>
        public void DisplayInLeftPictureBox(List<InternalLine> lineList)
        {
            foreach (InternalLine line in lineList)
            {
                Polyline newPolyLine = new Polyline();
                newPolyLine.Stroke = Brushes.Black;
                newPolyLine.Points = line.GetPointCollection();
                LeftCanvas.Children.Add(newPolyLine);
            }
        }

        /// <summary>
        /// Displays a list of lines in the right Picture box.
        /// </summary>
        /// <param name="img">The new image.</param>
        public void DisplayInRightPictureBox(List<InternalLine> lineList)
        {
            foreach (InternalLine line in lineList)
            {
                Polyline newPolyLine = new Polyline();
                newPolyLine.Stroke = Brushes.Black;
                newPolyLine.Points = line.GetPointCollection();
                LeftCanvas.Children.Add(newPolyLine);
            }
        }

        /// <summary>
        /// Enables the timer of the View, which will tick the Presenter.
        /// </summary>
        public void EnableTimer()
        {
            dispatcherTimer.Start();
        }

        /// <summary>
        /// A function that opens a file dialog and returns the filename.
        /// </summary>
        /// <param name="Filter">The filter that should be applied to the new Dialog.</param>
        /// <returns>Returns the FileName and the SafeFileName if the user correctly selects a file, 
        /// else returns a tuple with empty strigns</returns>
        public Tuple<string, string> openNewDialog(string Filter)
        {
            openFileDialog.Filter = Filter;
            if (openFileDialog.ShowDialog() == true)
            {
                return new Tuple<string, string>(openFileDialog.FileName, openFileDialog.SafeFileName);
            }
            else
            {
                return new Tuple<string, string>("", "");
            }
        }

        /// <summary>
        /// Sets the contents of the last action taken indicator label.
        /// </summary>
        /// <param name="message">The new contents</param>
        public void SetLastActionTakenText(string message)
        {
            LastActionBox.Text = message;
        }

        /// <summary>
        /// Changes the states of a tool strip button.
        /// </summary>
        /// <param name="buttonName">The name of the button.</param>
        /// <param name="state">The new state of the button.</param>
        public void SetToolStripButtonStatus(string buttonName, MainWindow.ButtonState state)
        {
            ButtonBase buttonToChange;
            bool isToggleable = false;
            switch (buttonName)
            {
                case "canvasButton":
                    buttonToChange = CanvasButton;
                    break;
                case "drawButton":
                    buttonToChange = DrawButton;
                    isToggleable = true;
                    break;
                case "deleteButton":
                    buttonToChange = DeleteButton;
                    isToggleable = true;
                    break;
                case "undoButton":
                    buttonToChange = UndoButton;
                    break;
                case "redoButton":
                    buttonToChange = RedoButton;
                    break;
                default:
                    Console.WriteLine("Invalid Button was given to SetToolStripButton. \nMaybe you forgot to add a case?");
                    return;
            }
            if (isToggleable)
            {
                switch (state)
                {
                    case ButtonState.Active:
                        ((ToggleButton)buttonToChange).IsEnabled = true;
                        ((ToggleButton)buttonToChange).IsChecked = true;
                        break;
                    case ButtonState.Disabled:
                        ((ToggleButton)buttonToChange).IsEnabled = false;
                        ((ToggleButton)buttonToChange).IsChecked = false;
                        break;
                    case ButtonState.Enabled:
                        ((ToggleButton)buttonToChange).IsEnabled = true;
                        ((ToggleButton)buttonToChange).IsChecked = false;
                        break;
                }
            }
            else
            {
                switch (state)
                {
                    case ButtonState.Disabled:
                        ((Button)buttonToChange).IsEnabled = false;
                        break;
                    default:
                        ((Button)buttonToChange).IsEnabled = true;
                        break;
                }
            }
        }

        /// <summary>
        /// Sets the contents of the load status indicator label.
        /// </summary>
        /// <param name="message">The new contents</param>
        public void SetToolStripLoadStatus(string message)
        {
            LoadStatusBox.Text = message;
        }

        /// <summary>
        /// shows the given info message in a popup and asks the user to aknowledge it
        /// </summary>
        /// <param name="message">the message to show</param>
        public void ShowInfoMessage(string message)
        {
            MessageBox.Show(message);
        }

        /// <summary>
        /// Shows a warning box with the given message (Yes/No Buttons)and returns true if the user aknowledges it.
        /// </summary>
        /// <param name="message">The message of the warning.</param>
        /// <returns>True if the user confirms (Yes), negative if he doesn't (No)</returns>
        public bool ShowWarning(string message)
        {
            MessageBoxResult result = MessageBox.Show(message, "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            return (result.Equals(MessageBoxResult.Yes));
        }
    }
}
