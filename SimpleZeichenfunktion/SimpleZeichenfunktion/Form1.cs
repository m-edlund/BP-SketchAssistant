using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleZeichenfunktion
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        int x = 0;
        int y = 0;
        //List<PointF> points = new List<PointF> { };
        //PointF init = new PointF(10, 10);
        PointF[] points = new PointF[10];
        int i = 0;
        PointF first;
        PointF second;
        bool clicked = false;
        PointF p;// = new PointF(x, y);



        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            //points[0] = init;
            // label1.Text = "X=" + e.X + " Y=" + e.Y;
            /*
             x = e.X;
             y = e.Y;
             PointF p = new PointF(x, y);
             addPath(p);
             i++;
             */
            timer1.Enabled = !clicked;
            clicked = !clicked;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {

                x = e.X;
                y = e.Y;
                p = new PointF(x, y);
          /*  if (clicked)
            {
                addPath(p);
                i++;
            }*/
        }

        public void addPath(PointF p)
        {
           // listBox1.Items.Add(p);
            //Aus den gegebenen Punkten eine Grafik erstellen
            points[i] = p;
            listBox1.Items.Add(points[i]);
            Pen pen = new Pen(Color.Black);
            Graphics g = CreateGraphics();
            first = points[0];


            if (i == 1)
            {
                second = points[1];

                g.DrawLine(pen, first, second);
                points[0] = second;
                i = 0;
            }

        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //Do Nothing
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Interval = 100;
            if (clicked)
            {
                addPath(p);
                i++;
            }
        }
    }
}
