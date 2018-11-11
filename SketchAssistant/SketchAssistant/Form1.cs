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
        OpenFileDialog ofd = new OpenFileDialog();
        //Image loaded on the left
        Image leftImage = null;

        private void Form1_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
            //Connect the Paint event of the left picture box to the event handler method.
            pictureBoxLeft.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBoxLeft_Paint);
        }

        private void pictureBoxLeft_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            //Create a local version of the graphics object
            //Graphics g = e.Graphics;

            //Draw something
            if(leftImage != null)
            {
                pictureBoxLeft.Image = leftImage;
            }

        }

        //TODO: Remove this placeholde when real buttons are in place
        private void toolStripLabel1_Click(object sender, EventArgs e)
        {

        }

        // A Table Layout with one row and two columns. 
        // Columns are 50% so that the window is evenly split.
        // The size is manually set relative to the window size. 
        // Maybe change this to automatically be the size of a parent container...
        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        
        //Load button, will open an OpenFileDialog
        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ofd.Filter = "SVG|*.svg;*.jpg";
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                toolStripLoadStatus.Text = ofd.SafeFileName;
                Svg.SvgDocument svgDoc = Svg.SvgDocument.Open(ofd.FileName);
                leftImage = new Bitmap(svgDoc.Draw());
                //leftImage = Image.FromFile(ofd.FileName);
                //The following line is needed, as else on first image load
                //the image will only be shown after resizing the window.
                this.Refresh();
            }
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            pictureBoxLeft.Update();
        }
    }
}
