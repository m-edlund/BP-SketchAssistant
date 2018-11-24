using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


// This is the code for your desktop app.
// Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.

namespace SketchAssistant
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //Different Program States
        public enum ProgramState
        {
            Idle,
            Draw
        }
        //Current Program State
        private ProgramState currentState;
        //Dialog to select a file.
        OpenFileDialog openFileDialogLeft = new OpenFileDialog();
        //Image loaded on the left
        Image leftImage = null;
        //Image on the right
        Image rightImage = null;

        private void Form1_Load(object sender, EventArgs e)
        {
            currentState = ProgramState.Idle;
            this.DoubleBuffered = true;
            //Connect the Paint event of the left picture box to the event handler method.
            pictureBoxLeft.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxLeft_Paint);
            pictureBoxRight.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxRight_Paint);
        }

        //Resize Function connected to the form resize event, will refresh the form when it is resized
        private void Form1_Resize(object sender, System.EventArgs e)
        {
            this.Refresh();
        }

        private void pictureBoxLeft_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            //Draw the left image
            if(leftImage != null)
            {
                //pictureBoxLeft.Image = leftImage;
            }
        }

        private void pictureBoxRight_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            //Draw the right image
            if (rightImage != null)
            {
                //pictureBoxRight.Image = rightImage;
            }
        }

        // A Table Layout with one row and two columns. 
        // Columns are 50% so that the window is evenly split.
        // The size is manually set relative to the window size. 
        // TODO: Maybe change this to automatically be the size of a parent container...
        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        
        //Load button, will open an OpenFileDialog
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialogLeft.Filter = "Image|*.jpg;*.png;*.jpeg";
            if(openFileDialogLeft.ShowDialog() == DialogResult.OK)
            {
                toolStripLoadStatus.Text = openFileDialogLeft.SafeFileName;
                leftImage = Image.FromFile(openFileDialogLeft.FileName);
                pictureBoxLeft.Image = leftImage;
                //Refresh the left image box when the content is changed
                this.Refresh();
                //pictureBoxLeft.Refresh();
            }
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void pictureBoxLeft_Click(object sender, EventArgs e)
        {

        }


        //Beginn userstory4
        //Bitmap skizze = null;
        Graphics graph = null;
        Point[] points = new Point[10]; //array Mousepositons
        int i = 0;

        Point p;// = new PointF(x, y);
        bool mousedown = false;


        List<Point> currentLine = new List<Point>();

        //Create an image relative to the mouse positions, which the method gets from pictureBoxRight_MouseMove
        public void addPath(Point p)
        {
            Pen pen = new Pen(Color.White);
            Point first;
            Point second;
            points[i] = p;
            graph = Graphics.FromImage(rightImage);
            first = points[0];
            currentLine.Add(p);

            if (i == 1)
            {
                second = points[1];

                graph.DrawLine(pen, first, second);
                points[0] = second;
                i = 0;
            }

        }

        //Changes The State of the Program to drawing
        private void drawButton_Click(object sender, EventArgs e)
        {
            if (currentState.Equals(ProgramState.Draw))
            {
                changeState(ProgramState.Idle);
            }
            else
            {
                changeState(ProgramState.Draw);
            }
        }

        //get current Mouse positon
        private void pictureBoxRight_MouseMove(object sender, MouseEventArgs e)
        {
            p = convertCoordinates(new Point(e.X, e.Y));
        }

        //hold left mouse button to draw.
        private void pictureBoxRight_MouseDown(object sender, MouseEventArgs e)
        {
            mousedown = true;
        }

        //Lift left mouse button to stop drawing.
        private void pictureBoxRight_MouseUp(object sender, MouseEventArgs e)
        {
            mousedown = false;
            Line linecs = new Line(currentLine);
            linecs.DrawLine(graph);
        }

        //Ende userstory4

        //Button to create a new Canvas. Will create an empty image 
        //which is the size of the left image, if there is one.
        //If there is no image loaded the canvas will be the size of the right picture box
        private void canvasButton_Click(object sender, EventArgs e)
        {
            if (leftImage == null)
            {
                rightImage = new Bitmap(pictureBoxRight.Width, pictureBoxRight.Height);
                graph = Graphics.FromImage(rightImage);
                graph.FillRectangle(Brushes.White, 0, 0, pictureBoxRight.Width + 10, pictureBoxRight.Height + 10);
                pictureBoxRight.Image = rightImage;
            }
            else
            {
                rightImage = new Bitmap(leftImage.Width, leftImage.Height);
                graph = Graphics.FromImage(rightImage);
                graph.FillRectangle(Brushes.White, 0, 0, leftImage.Width + 10, leftImage.Height + 10);
                pictureBoxRight.Image = rightImage;
            }
            this.Refresh();
            pictureBoxRight.Refresh();
        }

        //add a Point on every tick to the Drawpath
        private void drawTimer_Tick(object sender, EventArgs e)
        {
            if (currentState.Equals(ProgramState.Draw) && mousedown)
            {
                addPath(p);
                pictureBoxRight.Image = rightImage;
                i++;
            }
            if (!mousedown)
            {
                points[0] = p;
            }
        }

        /// <summary>
        /// A helper function which handles tasks associated witch changing states, 
        /// such as checking and unchecking buttons and changing the state.
        /// </summary>
        /// <param name="newState">The new state of the program</param>
        private void changeState(ProgramState newState)
        {
            switch (currentState)
            {
                case ProgramState.Draw:
                    drawButton.CheckState = CheckState.Unchecked;
                    drawTimer.Enabled = false;
                    break;
                default:
                    break;
            }
            switch (newState)
            {
                case ProgramState.Draw:
                    drawButton.CheckState = CheckState.Checked;
                    drawTimer.Enabled = true;
                    break;
                default:
                    break;
            }
            currentState = newState;
        }

        /// <summary>
        /// A function that calculates the coordinates of a point on a zoomed in image.
        /// </summary>
        /// <param name="">The position of the mouse cursor</param>
        /// <returns>The real coordinates of the mouse cursor on the image</returns>
        private Point convertCoordinates(Point cursorPosition)
        {
            Point realCoordinates = new Point(5,3);
            if(pictureBoxRight.Image == null)
            {
                return cursorPosition;
            }

            int widthImage = pictureBoxRight.Image.Width;
            int heightImage = pictureBoxRight.Image.Height;
            int widthBox = pictureBoxRight.Width;
            int heightBox = pictureBoxRight.Height;

            float imageRatio = (float)widthImage / (float)heightImage;
            float containerRatio = (float)widthBox / (float)heightBox;

            if (imageRatio >= containerRatio)
            {
                //Image is wider than it is high
                float zoomFactor = (float)widthImage / (float)widthBox;
                float scaledHeight = heightImage / zoomFactor;
                float filler = (heightBox - scaledHeight) / 2;
                realCoordinates.X = (int)(cursorPosition.X * zoomFactor);
                realCoordinates.Y = (int)((cursorPosition.Y - filler) * zoomFactor);
            }
            else
            {
                //Image is higher than it is wide
                float zoomFactor = (float)heightImage / (float)heightBox;
                float scaledWidth = widthImage / zoomFactor;
                float filler = (widthBox - scaledWidth) / 2;
                realCoordinates.X = (int)((cursorPosition.X - filler) * zoomFactor);
                realCoordinates.Y = (int)(cursorPosition.Y * zoomFactor);
            }
            return realCoordinates;
        }
    }
}
