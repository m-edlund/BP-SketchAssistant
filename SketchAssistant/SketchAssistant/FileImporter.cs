using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SketchAssistant
{
    class FileImporter
    {

        /// <summary>
        /// pointer to the running instance of main program
        /// </summary>
        Form1 program;

        public FileImporter(Form1 newProgram)
        {
            program = newProgram;
        }

        /// <summary>
        /// parses a drawing consisting of line objects, given in the proprietary .isad format
        /// </summary>
        /// <param name="allLines">the input file as an array of all lines in the .isad file</param>
        public void ParseISADInput(String[] allLines)
        {


            if (allLines.Length == 0)
            {
                program.ShowInfoMessage("Could not import file:\n file is empty");
                return;
            }
            if (!"drawing".Equals(allLines[0]))
            {
                program.ShowInfoMessage("Could not import file:\n file is not an interactive sketch assistant drawing\n (Hint: .isad files ave to start with the 'drawing' token)");
                return;
            }
            if (!"enddrawing".Equals(allLines[allLines.Length - 1]))
            {
                program.ShowInfoMessage("Could not import file:\n unterminated drawing definition\n (Hint: .isad files ave to end with the 'enddrawing' token)");
                return;
            }

            if (!parseISADHeader(allLines))
            {
                return;
            }

            if (!parseISADBody(allLines))
            {
                return;
            }



        }

        /// <summary>
        /// parses the first two lines of an input file .isad format
        /// </summary>
        /// <param name="allLines">the input file as an array of lines</param>
        /// <returns></returns>
        private bool parseISADHeader(String[] allLines)
        {

            int width;
            int height;

            if (!(allLines.Length > 1) || !Regex.Match(allLines[1], @"(\d?\d*x?\d?\d*?)?", RegexOptions.None).Success)
            {
                program.ShowInfoMessage("Could not import file:\n invalid or missing canvas size definition\n (Line: 2)");
                return false;
            }
            String[] size = allLines[1].Split('x');
            width = Convert.ToInt32(size[0]);
            height = Convert.ToInt32(size[1]);

            program.DrawEmptyCanvasLeft(width, height);

            return true;
        }

        /// <summary>
        /// parses all line entries of an input file .isad format
        /// </summary>
        /// <param name="allLines">the input file as an array of lines</param>
        /// <returns></returns>
        private bool parseISADBody(String[] allLines)
        {

            String lineStartString = "line";
            String lineEndString = "endline";

            List<Line> drawing = new List<Line>();

            int i = 2;
            //parse 'line' token and complete line definition
            while (lineStartString.Equals(allLines[i]))
            {
                i++;
                List<Point> newLine = new List<Point>();
                while (!lineEndString.Equals(allLines[i]))
                {
                    if (i == allLines.Length)
                    {
                        program.ShowInfoMessage("Could not import file:\n unterminated line definition\n (Line: " + (i + 1) + ")");
                        return false;
                    }
                    //parse single point definition
                    if (!Regex.Match(allLines[i], @"(\d?\d*;?\d?\d*?)?", RegexOptions.None).Success)
                    {
                        program.ShowInfoMessage("Could not import file:\n invalid Point definition: wrong format\n (Line: " + (i + 1) + ")");
                        return false;
                    }
                    String[] coordinates = allLines[i].Split(';');
                    //no errors possible, convertability to string already checked above
                    int xCoordinate = Convert.ToInt32(coordinates[0]);
                    int yCoordinate = Convert.ToInt32(coordinates[1]);
                    if (xCoordinate < 0 || yCoordinate < 0 || xCoordinate > program.leftImage.Width - 1 || yCoordinate > program.leftImage.Height - 1)
                    {
                        program.ShowInfoMessage("Could not import file:\n invalid Point definition: point out of bounds\n (Line: " + (i + 1) + ")");
                        return false;
                    }
                    newLine.Add(new Point(xCoordinate, yCoordinate));
                    i++;
                }
                //parse 'endline' token
                i++;
                //add line to drawing
                drawing.Add(new Line(newLine));
            }

            //save parsed drawing to instance variable and draw it
            program.BindTemplatePicture(drawing);

            return true;
        }

    }
}
