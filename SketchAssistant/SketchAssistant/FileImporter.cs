using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
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

        /// <summary>
        /// scale factor for coordinates of svg file
        /// </summary>
        double scale;

        /// <summary>
        /// line pointer for the current svg document
        /// </summary>
        int i;

        /// <summary>
        /// array containing all characters interpreted as whitespaces which seperate words/tokens in the input file
        /// </summary>
        readonly char[] whitespaces = new char[] { ' ' , ',' };

        /// <summary>
        /// number of points to create along the outline of an ellipse, divisible by 4
        /// </summary>
        readonly int samplingRateEllipse = 24;

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

            (int, int) dimensions = ParseISADHeader(allLines);
            List<Line> picture = ParseISADBody(allLines, dimensions.Item1, dimensions.Item2);

            return (dimensions.Item1, dimensions.Item2, picture);
        }



        /// <summary>
        /// parses the first two lines of an input file in .isad format
        /// </summary>
        /// <param name="allLines">the input file as an array of lines</param>
        /// <returns>the width and height of the left canvas</returns>
        private (int, int) ParseISADHeader(String[] allLines)
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
        private List<Line> ParseISADBody(String[] allLines, int width, int height)
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

        /// <summary>
        /// parses a svg drawing, given as a .svg file
        /// </summary>
        /// <param name="fileName">the path of the input file</param>
        /// <returns>the width and height of the left canvas and the parsed picture as a list of lines</returns>
        public (int, int, List<Line>) ParseSVGInputFile(String fileName, int windowWidth, int windowHeight)
        {
            return ParseSVGInput(System.IO.File.ReadAllLines(fileName), windowWidth, windowHeight);
        }

        /// <summary>
        /// parses a svg drawing, given as the content of a .svg file, seperated into lines
        /// </summary>
        /// <param name="allLines">an array holding all lines of the input file</param>
        /// <returns>the width and height of the left canvas and the parsed picture as a list of lines</returns>
        private (int, int, List<Line>) ParseSVGInput(String[] allLines, double windowWidth, double windowHeight)
        {
            i = 0; //reset line pointer
            if (allLines.Length == 0) //check for empty file
            {
                throw new FileImporterException("file is empty", "", -1);
            }
            (int, int) sizedef = ParseSVGHeader(allLines); //parse svg file header and get internal coordinate range
            i++;
            int width; //width of the resulting picture in pixels
            int height; //height of the resulting picture in pixels
            if (windowWidth / windowHeight > sizedef.Item1 / sizedef.Item2) //height dominant, width has to be smaller than drawing window to preserve xy-scale
            {
                scale = windowHeight / sizedef.Item2;
                Console.WriteLine("scale: (heights) " + windowHeight + "/" + sizedef.Item2);
                Console.WriteLine("widths: " + windowWidth + "/" + sizedef.Item1);
                height = (int)Math.Round(windowHeight);
                width = (int) Math.Round(scale * sizedef.Item1);
                Console.WriteLine(width + "x" + height + " (" + scale + ")");
            }
            else //width dominant, height has to be smaller than drawing window to preserve xy-scale
            {
                scale = windowWidth / sizedef.Item1;
                Console.WriteLine("scale: (widths) " + windowWidth + "/" + sizedef.Item1);
                Console.WriteLine("heights: " + windowHeight + "/" + sizedef.Item2);
                width = (int)Math.Round(windowWidth);
                height = (int)Math.Round(scale * sizedef.Item2);
                Console.WriteLine(width + "x" + height + " (" + scale + ")");
            }
            for(int j=0; j < allLines.Length; j++)
            {
                allLines[j] = allLines[j].Trim(whitespaces);
            }
            List<Line> picture = ParseSVGBody(allLines); //parse whole svg drawing into list of lines
            return (width, height, picture);
        }

        /// <summary>
        /// parses the svg file header and returns the internal coordinate range of this drawing, and iterates i to point to the start of svg element definitions
        /// </summary>
        /// <param name="allLines">an array holding all lines of the input file</param>
        /// <returns>the internal coordinate range of this drawing</returns>
        private (int, int) ParseSVGHeader(String[] allLines)
        {
            while (!allLines[i].StartsWith("<svg")) //skip non-relevant metadata at start of svg file
            {
                i++;
            }
            String[] currentLine = allLines[i].Split(' ');
            int width= -1;
            int height= -1;
            for(int j= 0; j < currentLine.Length; j++)
            {
                if (currentLine[j].StartsWith("width"))
                {
                    width = Convert.ToInt32(ParseSingleSVGAttribute(currentLine[j]));
                }
                else if (currentLine[j].StartsWith("height"))
                {
                    height = Convert.ToInt32(ParseSingleSVGAttribute(currentLine[j]));
                }
            }
            if(width == -1)
            {
                throw new FileImporterException("missing width definition in SVG header", "the header should contain the \"width=...\" attribute", i+1);
            }
            if (height == -1)
            {
                throw new FileImporterException("missing height definition in SVG header", "the header should contain the \"height=...\" attribute", i + 1);
            }
            return (width, height);
        }

        /// <summary>
        /// parses all relevant svg element definitions and skips the ones not representable by the sketch assistant
        /// </summary>
        /// <param name="allLines">an array holding all lines of the input file</param>
        /// <returns>the parsed picture as a list of lines</returns>
        private List<Line> ParseSVGBody(String[] allLines)
        {
            List<Line> picture = new List<Line>();
            while (!allLines[i].StartsWith("</svg"))
            {
                List<Line> element = ParseSingleSVGElement(allLines);
                if (element != null)
                {
                    picture.AddRange(element);
                }
                i++;
            }
            return picture;
        }

        /// <summary>
        /// parses one toplevel svg element
        /// </summary>
        /// <param name="allLines">an array holding all lines of the input file</param>
        /// <returns>the parsed Element as a list of lines</returns>
        private List<Line> ParseSingleSVGElement(string[] allLines)
        {
            String[] currentElement = GetCurrentElement(allLines);
            if (currentElement[currentElement.Length - 1].EndsWith("/>")) //single line element
            {
                return ParseSingleLineSVGElement(currentElement);
            }
            else //element containing sub-elements
            {
                return ParseMultiLineSVGElement(currentElement, allLines);
            }
        }

        /// <summary>
        /// parses a single toplevel svg element only taking one line
        /// </summary>
        /// <param name="allLines">an array holding all lines of the input file</param>
        /// <returns>the parsed element as a Line object, or null if the element is not supported</returns>
        private List<Line> ParseSingleLineSVGElement(string[] currentElement)
        {
            List<Point> points= null;
            List<Line> element = null;
            switch (currentElement[0])
            {
                case "<rect":
                    points = parseRect(currentElement);
                    break;
                case "<circle":
                    points = parseCircle(currentElement);
                    break;
                case "<ellipse":
                    points = parseEllipse(currentElement);
                    break;
                case "<line":
                    points = parseLine(currentElement);
                    break;
                case "<polyline":
                    points = parsePolyline(currentElement);
                    break;
                case "<polygon":
                    points = parsePolygon(currentElement);
                    break;
                case "<path":
                    element = parsePath(currentElement);
                    break;
                default: //unsupported svg element
                    Console.WriteLine("unsupported element: " + currentElement[0] + currentElement[0].Length);
                    return null;
            }
            if (element == null)
            {
                element = new List<Line>();
                element.Add(new Line(points));
            }
            return element;
        }

        /// <summary>
        /// parses a rectangle definition into a List of Points representing a single line around the rectangle (in clockwise direction)
        /// </summary>
        /// <param name="currentElement">the definition of the element as whitespace seperated String[]</param>
        /// <returns>the parsed element as a List of Points</returns>
        private List<Point> parseRect(string[] currentElement)
        {
            double x = 0;
            double y = 0;
            double w = 0;
            double h = 0;
            double rx = 0;
            double ry = 0;
            for(int j= 0; j < currentElement.Length; j++)
            {
                if (currentElement[j].StartsWith("x="))
                {
                    x = Convert.ToDouble(ParseSingleSVGAttribute(currentElement[j]), CultureInfo.InvariantCulture);
                }
                else if (currentElement[j].StartsWith("y="))
                {
                    y = Convert.ToDouble(ParseSingleSVGAttribute(currentElement[j]), CultureInfo.InvariantCulture);
                }
                else if (currentElement[j].StartsWith("width="))
                {
                    w = Convert.ToDouble(ParseSingleSVGAttribute(currentElement[j]), CultureInfo.InvariantCulture);
                }
                else if (currentElement[j].StartsWith("height="))
                {
                    h = Convert.ToDouble(ParseSingleSVGAttribute(currentElement[j]), CultureInfo.InvariantCulture);
                }
                else if (currentElement[j].StartsWith("rx="))
                {
                    rx = Convert.ToDouble(ParseSingleSVGAttribute(currentElement[j]), CultureInfo.InvariantCulture);
                }
                else if (currentElement[j].StartsWith("ry="))
                {
                    ry = Convert.ToDouble(ParseSingleSVGAttribute(currentElement[j]), CultureInfo.InvariantCulture);
                }
            }
            List<Point> rect = new List<Point>();
            rect.Add(ScaleAndCreatePoint(x, y));
            rect.Add(ScaleAndCreatePoint(x + w, y));
            rect.Add(ScaleAndCreatePoint(x + w, y + h));
            rect.Add(ScaleAndCreatePoint(x, y + h));
            rect.Add(ScaleAndCreatePoint(x, y));
            Console.WriteLine("parsed point: " + x + ";" + y);
            return rect;
        }

        /// <summary>
        /// parses a circle definition into a List of Points
        /// </summary>
        /// <param name="currentElement">the definition of the element as whitespace seperated String[]</param>
        /// <returns>the parsed element as a List of Points</returns>
        private List<Point> parseCircle(string[] currentElement)
        {
            double x = 0;
            double y = 0;
            double r = 0;
            for (int j = 0; j < currentElement.Length; j++)
            {
                if (currentElement[j].StartsWith("cx="))
                {
                    x = Convert.ToDouble(ParseSingleSVGAttribute(currentElement[j]), CultureInfo.InvariantCulture);
                }
                else if (currentElement[j].StartsWith("cy="))
                {
                    y = Convert.ToDouble(ParseSingleSVGAttribute(currentElement[j]), CultureInfo.InvariantCulture);
                }
                else if (currentElement[j].StartsWith("r="))
                {
                    r = Convert.ToDouble(ParseSingleSVGAttribute(currentElement[j]), CultureInfo.InvariantCulture);
                }
            }
            return SampleEllipse(x, y, r, r);
        }

        /// <summary>
        /// parses a ellipse definition into a List of Points
        /// </summary>
        /// <param name="currentElement">the definition of the element as whitespace seperated String[]</param>
        /// <returns>the parsed element as a List of Points</returns>
        private List<Point> parseEllipse(string[] currentElement)
        {
            double x = 0;
            double y = 0;
            double rx = 0;
            double ry = 0;
            for (int j = 0; j < currentElement.Length; j++)
            {
                if (currentElement[j].StartsWith("cx="))
                {
                    x = Convert.ToDouble(ParseSingleSVGAttribute(currentElement[j]), CultureInfo.InvariantCulture);
                }
                else if (currentElement[j].StartsWith("cy="))
                {
                    y = Convert.ToDouble(ParseSingleSVGAttribute(currentElement[j]), CultureInfo.InvariantCulture);
                }
                else if (currentElement[j].StartsWith("rx="))
                {
                    rx = Convert.ToDouble(ParseSingleSVGAttribute(currentElement[j]), CultureInfo.InvariantCulture);
                }
                else if (currentElement[j].StartsWith("ry="))
                {
                    ry = Convert.ToDouble(ParseSingleSVGAttribute(currentElement[j]), CultureInfo.InvariantCulture);
                }
            }
            return SampleEllipse(x, y, rx, ry);
        }

        /// <summary>
        /// parses a line definition into a List of two Points
        /// </summary>
        /// <param name="currentElement">the definition of the element as whitespace seperated String[]</param>
        /// <returns>the parsed element as a List of Points</returns>
        private List<Point> parseLine(string[] currentElement)
        {
            double x1 = 0;
            double y1 = 0;
            double x2 = 0;
            double y2 = 0;
            for (int j = 0; j < currentElement.Length; j++)
            {
                if (currentElement[j].StartsWith("x1="))
                {
                    x1 = Convert.ToDouble(ParseSingleSVGAttribute(currentElement[j]), CultureInfo.InvariantCulture);
                }
                else if (currentElement[j].StartsWith("y1="))
                {
                    y1 = Convert.ToDouble(ParseSingleSVGAttribute(currentElement[j]), CultureInfo.InvariantCulture);
                }
                else if (currentElement[j].StartsWith("x2="))
                {
                    x2 = Convert.ToDouble(ParseSingleSVGAttribute(currentElement[j]), CultureInfo.InvariantCulture);
                }
                else if (currentElement[j].StartsWith("y2="))
                {
                    y2 = Convert.ToDouble(ParseSingleSVGAttribute(currentElement[j]), CultureInfo.InvariantCulture);
                }
            }
            List<Point> line = new List<Point>();
            line.Add(ScaleAndCreatePoint(x1, y1));
            line.Add(ScaleAndCreatePoint(x2, y2));
            return line;
        }

        /// <summary>
        /// parses a polyline definition into a List of Points
        /// </summary>
        /// <param name="currentElement">the definition of the element as whitespace seperated String[]</param>
        /// <returns>the parsed element as a List of Points</returns>
        private List<Point> parsePolyline(string[] currentElement)
        {
            String[] points = null;
            for (int j = 0; j < currentElement.Length; j++)
            {
                if (currentElement[j].StartsWith("points="))
                {
                    List<String> pointDefs = new List<string>();
                    pointDefs.Add(currentElement[j].Substring(8)); //parse first point coordinates by removing 'points="'
                    j++;
                    while (!currentElement[j].EndsWith("\""))
                    {
                        pointDefs.Add(currentElement[j]); //parse intermediate point coordinates
                        j++;
                    }
                    pointDefs.Add(currentElement[j].Trim('"')); //parse last point coordinates by removing '"'
                    points = pointDefs.ToArray();
                }
            }
            List<Point> polyline = new List<Point>();
            for (int k = 0; k < points.Length - 1; k += 2)
            {
                polyline.Add(ScaleAndCreatePoint(Convert.ToDouble(points[k], CultureInfo.InvariantCulture), Convert.ToDouble(points[k + 1], CultureInfo.InvariantCulture)));
            }
            return polyline;
        }

        /// <summary>
        /// parses a polygon definition into a List of Points
        /// </summary>
        /// <param name="currentElement">the definition of the element as whitespace seperated String[]</param>
        /// <returns>the parsed element as a List of Points</returns>
        private List<Point> parsePolygon(string[] currentElement)
        {
            String[] points = null;
            for (int j = 0; j < currentElement.Length; j++)
            {
                if (currentElement[j].StartsWith("points="))
                {
                    List<String> pointDefs = new List<string>();
                    pointDefs.Add(currentElement[j].Substring(8)); //parse first point coordinates by removing 'points="'
                    j++;
                    while (!currentElement[j].EndsWith("\""))
                    {
                        pointDefs.Add(currentElement[j]); //parse intermediate point coordinates
                        j++;
                    }
                    pointDefs.Add(currentElement[j].Trim('"')); //parse last point coordinates by removing '"'
                    points = pointDefs.ToArray();
                }
            }
            List<Point> polygon = new List<Point>();
            for (int k = 0; k < points.Length - 1; k+=2)
            {
                polygon.Add(ScaleAndCreatePoint(Convert.ToDouble(points[k], CultureInfo.InvariantCulture), Convert.ToDouble(points[k+1], CultureInfo.InvariantCulture)));
                Console.WriteLine("parsed point: " + points[k] + ";" + points[k + 1]);
            }
            polygon.Add(ScaleAndCreatePoint(Convert.ToDouble(points[0], CultureInfo.InvariantCulture), Convert.ToDouble(points[1], CultureInfo.InvariantCulture))); //close polygon
            Console.WriteLine("parsed point: " + points[0] + ";" + points[1]);
            return polygon;
        }

        /// <summary>
        /// parses a path definition into a List of Points
        /// </summary>
        /// <param name="currentElement">the definition of the element as whitespace seperated String[]</param>
        /// <returns>the parsed element as a List of Points</returns>
        private List<Line> parsePath(string[] currentElement)
        {
            List<String> pathElements = new List<string>();
            /*
            for (int j = 0; j < currentElement.Length; j++)
            {
                if (currentElement[j].StartsWith("d="))
                {
                    pathElements.Add(currentElement[j].Substring(3)); //parse first path element by removing 'd="'
                    j++;
                    while (!currentElement[j].EndsWith("\""))
                    {
                        pathElements.Add(currentElement[j]); //parse intermediate path element
                        j++;
                    }
                    pathElements.Add(currentElement[j].Trim('"')); //parse last path element by removing '"'
                }
            }
            List<Line> element = new List<Line>();
            List<Point> currentLine = new List<Point>();
            Point mirroredBezierPoint;
            pathElements = PreparePathElements(pathElements); //split pathElement list objects until every object is atomar (single character or single number (coordinate))
            //int k = 0; //index of active element in pathElements is always 0
            currentLine = parse_M(pathElements);
            while(!(pathElements.Count == 0){
                if (pathElements.First().Equals("M"))
                {
                    element.Add(new Line(currentLine));
                    currentLine = parse_M(pathElements);
                }
                else if (pathElements.First().Equals(""))
                {

                }
            }
            return element;
            */
            return null;
        }

        /// <summary>
        /// parses a hierarchical svg element and all its sub-elements
        /// </summary>
        /// <param name="currentElement">the definition of the top level element as whitespace seperated String[]</param>
        /// <param name="allLines">an array holding all lines of the input file</param>
        /// <returns>the parsed element as a Line object, or null if the element is not supported</returns>
        private List<Line> ParseMultiLineSVGElement(string[] currentElement, string[] allLines)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// removes the name of the attribute aswell as the '="' at the beginning and the '"' or '">' at the end of an attribute definition
        /// </summary>
        /// <param name="definition">the definition from the svg file</param>
        /// <returns>the value of the attribute, as String (the part of definition contained between '"'s)</returns>
        private String ParseSingleSVGAttribute(String definition)
        {
            return definition.Split('"')[1];
        }

        /// <summary>
        /// fetches a single svg element definition that may extend ovr several lines of the input file, iterates i to point to the last line of the element definition
        /// </summary>
        /// <param name="allLines">an array holding all lines of the input file</param>
        /// <returns>the definition of the current svg element, as String[] split by whitespaces</returns>
        private String[] GetCurrentElement(String[] allLines)
        {
            List<String> currentElementTemp = allLines[i].Split(whitespaces).ToList();
            while (!currentElementTemp.Last().EndsWith(">"))
            {
                i++;
                currentElementTemp.AddRange(allLines[i].Split(whitespaces).ToList());
            }
            return currentElementTemp.ToArray();
        }

        /// <summary>
        /// applies the scale factor to the coordinates and creates a new Point
        /// </summary>
        /// <param name="x">unscaled x coordinate</param>
        /// <param name="y">unscaled y coordinate</param>
        /// <returns>new Point with scaled coordinates</returns>
        private Point ScaleAndCreatePoint(double x, double y)
        {
            return new Point((int)Math.Round(x * scale), (int)Math.Round(y * scale));
        }

        /// <summary>
        /// creates a representation of an ellipse as a List of Points by sampling the outline of the ellipse
        /// </summary>
        /// <param name="x">x coordinate of the center of the ellipse</param>
        /// <param name="y">y coordinate of the center of the ellipse</param>
        /// <param name="rx">x radius of the ellipse</param>
        /// <param name="ry">y radius of the ellipse</param>
        /// <returns>the parsed element as a List of Points</returns>
        private List<Point> SampleEllipse(double x, double y, double rx, double ry)
        {
            List<Point> ellipse = new List<Point>();
            double angle = ((double)2 * Math.PI) / (double)samplingRateEllipse;
            double yScale = ry / rx;
            Console.WriteLine("parsing ellipse: " + x + ";" + y + "(" + rx + "x" + ry + ")" + " " + yScale + ":" + angle);
            double[] xValues = new double[samplingRateEllipse / 4];
            double[] yValues = new double[samplingRateEllipse / 4];
            for (int j = 0; j < samplingRateEllipse / 4; j++) //compute offset values of points for one quadrant
            {
                xValues[j] = Math.Sin((double)j * angle) * rx;
                yValues[j] = Math.Cos((double)j * angle) * rx;
                Console.WriteLine("parsed ellipse value: " + xValues[j] + ";" + yValues[j]);
            }
            for (int j = 0; j < samplingRateEllipse / 4; j++) //create actual points for first quadrant
            {
                int xCoord = Convert.ToInt32(Math.Round(x + xValues[j]));
                int yCoord = Convert.ToInt32(Math.Round(y - yValues[j] * yScale));
                ellipse.Add(ScaleAndCreatePoint(xCoord, yCoord));
                Console.WriteLine("parsed ellipse point: " + xCoord + ";" + yCoord + " pointCount: " + (samplingRateEllipse / 4));
            }
            for (int j = 0; j < samplingRateEllipse / 4; j++) //create actual points for second quadrant
            {
                int xCoord = Convert.ToInt32(Math.Round(x + yValues[j]));
                int yCoord = Convert.ToInt32(Math.Round(y + xValues[j] * yScale));
                ellipse.Add(ScaleAndCreatePoint(xCoord, yCoord));
                Console.WriteLine("parsed ellipse point: " + xCoord + ";" + yCoord + " pointCount: " + (samplingRateEllipse / 4));
            }
            for (int j = 0; j < samplingRateEllipse / 4; j++) //create actual points for third quadrant
            {
                int xCoord = Convert.ToInt32(Math.Round(x - xValues[j]));
                int yCoord = Convert.ToInt32(Math.Round(y + yValues[j] * yScale));
                ellipse.Add(ScaleAndCreatePoint(xCoord, yCoord));
                Console.WriteLine("parsed ellipse point: " + xCoord + ";" + yCoord + " pointCount: " + (samplingRateEllipse / 4));
            }
            for (int j = 0; j < samplingRateEllipse / 4; j++) //create actual points for fourth quadrant
            {
                int xCoord = Convert.ToInt32(Math.Round(x - yValues[j]));
                int yCoord = Convert.ToInt32(Math.Round(y - xValues[j] * yScale));
                ellipse.Add(ScaleAndCreatePoint(xCoord, yCoord));
                Console.WriteLine("parsed ellipse point: " + xCoord + ";" + yCoord + " pointCount: " + (samplingRateEllipse / 4));
            }
            ellipse.Add(ScaleAndCreatePoint(Convert.ToInt32(Math.Round(x + 0)), Convert.ToInt32(Math.Round(y - rx * yScale)))); //close ellipse
            return ellipse;
        }
    }
}
