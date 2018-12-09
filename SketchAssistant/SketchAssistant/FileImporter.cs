using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SketchAssistant
{
    public class FileImporter
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
        /// parses a drawing consisting of line objects, given as a file in the application specific .isad format
        /// </summary>
        /// <param name="fileName">the path of the input file</param>
        /// <returns>the width and height of the left canvas and the parsed picture as a list of lines</returns>
        public (int, int, List<Line>) ParseISADInputFile(String fileName)
        {
            return ParseISADInput(System.IO.File.ReadAllLines(fileName));
        }

        /// <summary>
        /// parses a drawing consisting of line objects, given as the content of a .isad file, seperated into lines
        /// </summary>
        /// <param name="allLines">an array holding all lines of the input file</param>
        /// <returns>the width and height of the left canvas and the parsed picture as a list of lines</returns>
        private (int, int, List<Line>) ParseISADInput(String[] allLines)
        {

            if (allLines.Length == 0)
            {
                throw new FileImporterException("file is empty", "", -1);
            }
            if (!"drawing".Equals(allLines[0]))
            {
                throw new FileImporterException("file is not an interactive sketch assistant drawing", ".isad files have to start with the 'drawing' token", 1);
            }
            if (!"enddrawing".Equals(allLines[allLines.Length - 1]))
            {
                throw new FileImporterException("unterminated drawing definition", ".isad files have to end with the 'enddrawing' token", allLines.Length);
            }

            (int, int) dimensions = parseISADHeader(allLines);
            List<Line> picture = parseISADBody(allLines, dimensions.Item1, dimensions.Item2);

            return (dimensions.Item1, dimensions.Item2, picture);
        }



        /// <summary>
        /// parses the first two lines of an input file in .isad format
        /// </summary>
        /// <param name="allLines">the input file as an array of lines</param>
        /// <returns>the width and height of the left canvas</returns>
        private (int, int) parseISADHeader(String[] allLines)
        {
            int width;
            int height;
            if (!(allLines.Length > 1) || !Regex.Match(allLines[1], @"^\d+x?\d+$", RegexOptions.None).Success)
            {
                throw new FileImporterException("invalid or missing canvas size definition", "format: [width]x[heigth]", 2);
            }
            String[] size = allLines[1].Split('x');
            width = Convert.ToInt32(size[0]);
            height = Convert.ToInt32(size[1]);
            return (width, height);
        }

        /// <summary>
        /// parses all line entries of an input file in .isad format
        /// </summary>
        /// <param name="allLines">the input file as an array of lines</param>
        /// <returns>the parsed picture as a list of lines</returns>
        private List<Line> parseISADBody(String[] allLines, int width, int height)
        {

            String lineStartString = "line";
            String lineEndString = "endline";

            List<Line> drawing = new List<Line>();

            //number of the line currently being parsed, enumeration starting at 0, body starts at the third line, therefore lin number 2
            int i = 2;
            //parse 'line' token and complete line definition
            int lineStartPointer = i;
            //holds the line number of the next expected beginning of a line definition, or of the enddrawing token
            while (lineStartString.Equals(allLines[i]))
            {
                //start parsing next line
                i++;
                List<Point> newLine = new List<Point>();
                while (!lineEndString.Equals(allLines[i]))
                {
                    if (i == allLines.Length)
                    {
                        throw new FileImporterException("unterminated line definition", null, (i + 1));
                    }
                    //parse single point definition
                    if (!Regex.Match(allLines[i], @"^\d+;\d+$", RegexOptions.None).Success)
                    {
                        throw new FileImporterException("invalid Point definition: wrong format", "format: [xCoordinate];[yCoordinate]", (i + 1) );
                    }
                    String[] coordinates = allLines[i].Split(';');
                    //no errors possible, convertability to int already checked above
                    int xCoordinate = Convert.ToInt32(coordinates[0]);
                    int yCoordinate = Convert.ToInt32(coordinates[1]);
                    if (xCoordinate < 0 || yCoordinate < 0 || xCoordinate > width - 1 || yCoordinate > height - 1)
                    {
                        throw new FileImporterException("invalid Point definition: point out of bounds", null, (i + 1) );
                    }
                    newLine.Add(new Point(xCoordinate, yCoordinate));
                    //start parsing next line
                    i++;
                }
                //"parse" 'endline' token, syntax already checked at the beginning,  and start parsing next line
                i++;
                //add line to drawing
                drawing.Add(new Line(newLine));
                //update lineStartPointer to the presumable start of the next line
                lineStartPointer = i;
            }
            //check if end of body is reached after there are no more line definitions
            if(i != allLines.Length - 1)
            {
                throw new FileImporterException("missing or invalid line definition token", "line definitions start with the 'line' token", (i + 1));
            }
            //return parsed picture
            return drawing;
        }

        /// <summary>
        /// connection point for testing use only: calls ParseISADInput(String[] allLines) and directly passes the given argument (effectively bypassing the File Input functionality)
        /// </summary>
        /// <param name="allLines">an array holding all lines of the input file</param>
        /// <returns>the width and height of the left canvas and the parsed picture as a list of lines</returns>
        public (int, int, List<Line>) ParseISADInputForTesting(String[] allLines)
        {
            return ParseISADInput(allLines);
        }

    }
}
