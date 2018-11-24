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

        //Dialog to select a file.
        OpenFileDialog openFileDialogLeft = new OpenFileDialog();
        //Image loaded on the left
        Image leftImage = null;
        //Image on the right
        Image rightImage = null;

        private void Form1_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
            //Connect the Paint event of the left picture box to the event handler method.
            pictureBoxLeft.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxLeft_Paint);
            //pictureBoxRight.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxRight_Paint);
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
                pictureBoxLeft.Refresh();
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
        int x = 0;
        int y = 0;
        PointF[] points = new PointF[10]; //array Mousepositons
        int i = 0;
        PointF first;
        PointF second;
        bool clicked = false; //Button "Paint" is clicked or not
        PointF p;// = new PointF(x, y);
        bool mousedown = false;
        Pen pen = new Pen(Color.Black);


        //Create an image relative to the mouse positions, which the method gets from pictureBoxRight_MouseMove
        public void addPath(PointF p)
        {
            points[i] = p;
            graph = Graphics.FromImage(rightImage);
            first = points[0];


            if (i == 1)
            {
                second = points[1];

                graph.DrawLine(pen, first, second);
                points[0] = second;
                i = 0;
            }

        }

        // creates an empty image and prepares rightPictureBox for drawing
        private void painttoolStripMenuItem_Click(object sender, EventArgs e)
        {
            //rightImage = new Bitmap(500, 800);
            //graph = Graphics.FromImage(rightImage);
            //graph.FillRectangle(Brushes.White, 0, 0, 500, 800);
            //pictureBoxRight.Image = rightImage;
            timer2.Enabled = !clicked;
            clicked = !clicked;

            if (clicked)
            {
                painttoolStripMenuItem.BackColor = Color.Aqua;
            }
            else
                painttoolStripMenuItem.BackColor = Color.White;
        }

        //add a Point on every tick to the Drawpath
        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Interval = 100;
            if (clicked && mousedown)
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

        //get current Mouse positon
        private void pictureBoxRight_MouseMove(object sender, MouseEventArgs e)
        {
            x = e.X;
            y = e.Y;
            p = new PointF(x, y);
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
        }

        //Ende userstory4

        //Button to create a new Canvas. Will create an empty image 
        //which is the size of the left image, if there is one.
        //If there is no image loaded the canvas will be the size of the right picture box
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (leftImage == null)
            {
                rightImage = new Bitmap(pictureBoxRight.Width, pictureBoxRight.Height);
                graph = Graphics.FromImage(rightImage);
                graph.FillRectangle(Brushes.White, 0, 0, pictureBoxRight.Width + 10, pictureBoxRight.Height + 10);
                pictureBoxRight.Image = rightImage;
                /*using (Graphics grp = Graphics.FromImage(rightImage))
                {
                    grp.FillRectangle(Brushes.White, 0, 0, pictureBoxRight.Width + 10, pictureBoxRight.Height + 10);
                }*/
            }
            else
            {
                rightImage = new Bitmap(leftImage.Width, leftImage.Height);
                graph = Graphics.FromImage(rightImage);
                graph.FillRectangle(Brushes.White, 0, 0, leftImage.Width + 10, leftImage.Height + 10);
                pictureBoxRight.Image = rightImage;
                /*using (Graphics grp = Graphics.FromImage(rightImage))
                {
                    grp.FillRectangle(Brushes.White, 0, 0, leftImage.Width + 10, leftImage.Height + 10);
                }*/
            }
            this.Refresh();
            pictureBoxRight.Refresh();
        }
        

    }
}
