﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
            dispatcherTimer = new DispatcherTimer(DispatcherPriority.Render);
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 10);
            ProgramPresenter.Resize(new Tuple<int, int>((int)LeftCanvas.Width, (int)LeftCanvas.Height),
                new Tuple<int, int>((int)RightCanvas.Width, (int)RightCanvas.Height));
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
        /// <summary>
        /// The line currently being drawn
        /// </summary>
        Polyline currentLine;

        /********************************************/
        /*** WINDOW SPECIFIC FUNCTIONS START HERE ***/
        /********************************************/

        /// <summary>
        /// Resize Function connected to the form resize event, will refresh the form when it is resized
        /// </summary>
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ProgramPresenter.Resize(new Tuple<int, int>((int) LeftCanvas.Width, (int) LeftCanvas.Height),
                new Tuple<int, int>((int) RightCanvas.Width, (int) RightCanvas.Height));
        }

        /// <summary>
        /// Redo an Action.
        /// </summary>
        private void RedoButton_Click(object sender, RoutedEventArgs e)
        {
            ProgramPresenter.Redo();
        }

        /// <summary>
        /// Undo an Action.
        /// </summary>
        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            ProgramPresenter.Undo();
        }

        /// <summary>
        /// Changes the state of the program to deletion
        /// </summary>
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            ProgramPresenter.ChangeState(false);
        }

        /// <summary>
        /// Changes the state of the program to drawing
        /// </summary>
        private void DrawButton_Click(object sender, RoutedEventArgs e)
        {
            ProgramPresenter.ChangeState(true);
        }

        /// <summary>
        /// Hold left mouse button to start drawing.
        /// </summary>
        private void RightCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ProgramPresenter.MouseEvent(MVP_Presenter.MouseAction.Down);
        }

        /// <summary>
        /// Lift left mouse button to stop drawing and add a new Line.
        /// </summary>
        private void RightCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ProgramPresenter.MouseEvent(MVP_Presenter.MouseAction.Up);
        }

        /// <summary>
        /// Get current Mouse positon within the right picture box.
        /// </summary>
        private void RightCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            ProgramPresenter.MouseEvent(MVP_Presenter.MouseAction.Move, e.GetPosition(RightCanvas));
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

        /// <summary>
        /// Import button, will open an OpenFileDialog
        /// </summary>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            ProgramPresenter.ExamplePictureToolStripMenuItemClick();
        }

        /*************************/
        /*** PRESENTER -> VIEW ***/
        /*************************/

        /*
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
    }*/

        /// <summary>
        /// Remove the current line.
        /// </summary>
        public void RemoveCurrLine()
        {
            RightCanvas.Children.Remove(currentLine);
        }

        /// <summary>
        /// Display the current line.
        /// </summary>
        /// <param name="line">The current line to display</param>
        public void DisplayCurrLine(Polyline line)
        {
            if (RightCanvas.Children.Contains(currentLine))
            {
                RemoveCurrLine();
            }
            RightCanvas.Children.Add(line);
            currentLine = line;
        }

        /// <summary>
        /// Removes all Lines from the left canvas.
        /// </summary>
        public void RemoveAllLeftLines()
        {
            LeftCanvas.Children.Clear();
        }

        /// <summary>
        /// Removes all lines in the right canvas.
        /// </summary>
        public void RemoveAllRightLines()
        {
            RightCanvas.Children.Clear();
        }

        /// <summary>
        /// Adds another Line that will be displayed in the left display.
        /// </summary>
        /// <param name="newLine">The new Polyline to be added displayed.</param>
        public void AddNewLineLeft(Polyline newLine)
        {
            newLine.Stroke = Brushes.Black;
            newLine.StrokeThickness = 2;
            LeftCanvas.Children.Add(newLine);
        }

        /// <summary>
        /// Adds another Line that will be displayed in the right display.
        /// </summary>
        /// <param name="newLine">The new Polyline to be added displayed.</param>
        public void AddNewLineRight(Polyline newLine)
        {
            newLine.Stroke = Brushes.Black;
            newLine.StrokeThickness = 2;
            RightCanvas.Children.Add(newLine);
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
                        ((ToggleButton)buttonToChange).Opacity = 1;
                        ((ToggleButton)buttonToChange).Background = Brushes.SkyBlue;
                        break;
                    case ButtonState.Disabled:
                        ((ToggleButton)buttonToChange).IsEnabled = false;
                        ((ToggleButton)buttonToChange).IsChecked = false;
                        ((ToggleButton)buttonToChange).Opacity = 0.5;
                        ((ToggleButton)buttonToChange).Background = Brushes.LightGray;
                        break;
                    case ButtonState.Enabled:
                        ((ToggleButton)buttonToChange).IsEnabled = true;
                        ((ToggleButton)buttonToChange).IsChecked = false;
                        ((ToggleButton)buttonToChange).Opacity = 1;
                        ((ToggleButton)buttonToChange).Background = Brushes.LightGray;
                        break;
                }
            }
            else
            {
                switch (state)
                {
                    case ButtonState.Disabled:
                        ((Button)buttonToChange).IsEnabled = false;
                        ((Button)buttonToChange).Opacity = 0.5;
                        break;
                    default:
                        ((Button)buttonToChange).IsEnabled = true;
                        ((Button)buttonToChange).Opacity = 1;
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

        /// <summary>
        /// Updates the colour of a canvas.
        /// </summary>
        /// <param name="canvasName">The name of the canvas to be updated.</param>
        /// <param name="active">Whether or not the canvas is active.</param>
        public void SetCanvasState(string canvasName, bool active)
        {
            Canvas canvas;
            switch (canvasName){
                case ("LeftCanvas"):
                    canvas = LeftCanvas;
                    break;
                case ("RightCanvas"):
                    canvas = RightCanvas;
                    break;
                default:
                    throw new InvalidOperationException("Unknown canvas name, Check that the canvas passed is either LeftCanvas or RightCanvas");
            }
            if (active)
            {
                canvas.Background = Brushes.White;
            }
            else
            {
                canvas.Background = Brushes.SlateGray;
            }
        }
    }
}
