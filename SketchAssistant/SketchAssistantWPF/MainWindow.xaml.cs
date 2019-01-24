using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        }

        public enum ButtonState
        {
            Enabled,
            Disabled,
            Active
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

        /********************************************/
        /*** WINDOW SPECIFIC FUNCTIONS START HERE ***/
        /********************************************/

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

        /// <summary>
        /// Button to create a new Canvas. Will create an empty image 
        /// which is the size of the left image, if there is one.
        /// If there is no image loaded the canvas will be the size of the right picture box
        /// </summary>
        private void CanvasButton_Click(object sender, RoutedEventArgs e)
        {
            ProgramPresenter.NewCanvas();
        }

        /*************************/
        /*** PRESENTER -> VIEW ***/
        /*************************/

        public void DisplayInLeftPictureBox(Image img)
        {
            throw new NotImplementedException();
        }

        public void DisplayInRightPictureBox(Image img)
        {
            throw new NotImplementedException();
        }

        public void EnableTimer()
        {
            throw new NotImplementedException();
        }

        public Tuple<string, string> openNewDialog(string Filter)
        {
            throw new NotImplementedException();
        }

        public void SetLastActionTakenText(string message)
        {
            throw new NotImplementedException();
        }

        public void SetToolStripButtonStatus(string buttonName, MainWindow.ButtonState state)
        {
            throw new NotImplementedException();
        }

        public void SetToolStripLoadStatus(string message)
        {
            throw new NotImplementedException();
        }

        public void ShowInfoMessage(string message)
        {
            throw new NotImplementedException();
        }

        public bool ShowWarning(string message)
        {
            throw new NotImplementedException();
        }
    }
}
