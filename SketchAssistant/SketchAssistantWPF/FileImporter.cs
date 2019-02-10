using System;
using System.Collections.Generic;
//using System.Drawing;
using System.Globalization;
using System.Windows;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SketchAssistantWPF
{
    public class FileImporter
    {

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
        readonly int samplingRateEllipse = 12;

        /// <summary>
        /// number of points to create on a bezier curve, including start and end point (even number will result in "flat" bezier curves, uneven number in "pointed" ones)
        /// </summary>
        readonly int samplingRateBezier = 101;

        /// <summary>
        /// parses a drawing consisting of line objects, given as a file in the application specific .isad format
        /// </summary>
        /// <param name="fileName">the path of the input file</param>
        /// <returns>the width and height of the left canvas and the parsed picture as a list of lines</returns>
public Tuple<int, int, List<InternalLine>> ParseISADInputFile(String fileName)
        {
            return ParseISADInput(System.IO.File.ReadAllLines(fileName));
        }

        /// <summary>
        /// parses a drawing consisting of line objects, given as the content of a .isad file, seperated into lines
        /// </summary>
        /// <param name="allLines">an array holding all lines of the input file</param>
        /// <returns>the width and height of the left canvas and the parsed picture as a list of lines</returns>
        private Tuple<int, int, List<InternalLine>> ParseISADInput(String[] allLines)
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
            Tuple<int, int> dimensions = ParseISADHeader(allLines);
            List<InternalLine> picture = ParseISADBody(allLines, dimensions.Item1, dimensions.Item2);
            
            return new Tuple<int, int, List<InternalLine>>(dimensions.Item1, dimensions.Item2, picture);
        }



        /// <summary>
        /// parses the first two lines of an input file in .isad format
        /// </summary>
        /// <param name="allLines">the input file as an array of lines</param>
        /// <returns>the width and height of the left canvas</returns>
        private Tuple<int, int> ParseISADHeader(String[] allLines)
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
            return new Tuple<int, int>(width, height);
        }

        /// <summary>
        /// parses all line entries of an input file in .isad format
        /// </summary>
        /// <param name="allLines">the input file as an array of lines</param>
        /// <returns>the parsed picture as a list of lines</returns>
        private List<InternalLine> ParseISADBody(String[] allLines, int width, int height)
        {
            String lineStartString = "line";
            String lineEndString = "endline";
            List<InternalLine> drawing = new List<InternalLine>();
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
                        throw new FileImporterException("invalid Point definition: wrong format", "format: [xCoordinate];[yCoordinate]", (i + 1));
                    }
                    String[] coordinates = allLines[i].Split(';');
                    //no errors possible, convertability to int already checked above
                    int xCoordinate = Convert.ToInt32(coordinates[0]);
                    int yCoordinate = Convert.ToInt32(coordinates[1]);
                    if (xCoordinate < 0 || yCoordinate < 0 || xCoordinate > width - 1 || yCoordinate > height - 1)
                    {
                        throw new FileImporterException("invalid Point definition: point out of bounds", null, (i + 1));
                    }
                    newLine.Add(new Point(xCoordinate, yCoordinate));
                    //start parsing next line
                    i++;
                }
                //"parse" 'endline' token, syntax already checked at the beginning,  and start parsing next line
                i++;
                //add line to drawing
                drawing.Add(new InternalLine(newLine));
                //update lineStartPointer to the presumable start of the next line
                lineStartPointer = i;
            }
            //check if end of body is reached after there are no more line definitions
            if (i != allLines.Length - 1)
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
        public Tuple<int, int, List<InternalLine>> ParseISADInputForTesting(String[] allLines)
        {
            return ParseISADInput(allLines);
        }

        /// <summary>
        /// parses a svg drawing, given as a .svg file
        /// <para />several severe restrictions to the svg standard apply:
        /// <para /> - width and heigth values must be integers
        /// <para /> - the supported svg elements to be drawn must be placed on top level directly inside the 'svg' tag
        /// <para /> - except for the global 'svg' tag, no hierarchical elements (elements which contain other svg elements) may exist. in other words: after an opening element tag no other opening element tag may occur before the closing tag of this element.
        /// <para /> - lines in front of the (single) opening and after the (single) closing global svg tag will be ignored during parsing
        /// <para /> - unsupported svg elements on top level will be ignored during parsing
        /// <para /> - the input file must not contain empty lines
        /// <para /> - all input files have to be manually tested and approved for use with this program by a developer or otherwise entitled personnel, otherwise no guarantee about correct and error-free parsing will be given
        /// </summary>
        /// <param name="fileName">the path of the input file</param>
        /// <returns>the width and height of the left canvas and the parsed picture as a list of lines</returns>
        public Tuple<int, int, List<InternalLine>> ParseSVGInputFile(String fileName, int windowWidth, int windowHeight)
        {
            return ParseSVGInput(System.IO.File.ReadAllLines(fileName), windowWidth, windowHeight);
        }

        /// <summary>
        /// parses a svg drawing, given as the content of a .svg file, seperated into lines
        /// </summary>
        /// <param name="allLines">an array holding all lines of the input file</param>
        /// <returns>the width and height of the left canvas and the parsed picture as a list of lines</returns>
        private Tuple<int, int, List<InternalLine>> ParseSVGInput(String[] allLines, double windowWidth, double windowHeight)
        {
            i = 0; //reset line pointer
            if (allLines.Length == 0) //check for empty file
            {
                throw new FileImporterException("file is empty", "", -1);
            }
            var sizedef = ParseSVGHeader(allLines); //parse svg file header and get internal coordinate range
            i++;
            int width; //width of the resulting picture in pixels
            int height; //height of the resulting picture in pixels
            if(windowWidth != 0 && windowHeight != 0)
            {
                if (windowWidth / windowHeight > sizedef.Item1 / sizedef.Item2) //height dominant, width has to be smaller than drawing window to preserve xy-scale
                {
                    scale = windowHeight / sizedef.Item2;
                    height = (int)Math.Round(windowHeight);
                    width = (int) Math.Round(scale * sizedef.Item1);
                }
                else //width dominant, height has to be smaller than drawing window to preserve xy-scale
                {
                    scale = windowWidth / sizedef.Item1;
                    width = (int)Math.Round(windowWidth);
                    height = (int)Math.Round(scale * sizedef.Item2);
                }
            }
            else
            {
                scale = 1;
                width = sizedef.Item1;
                height = sizedef.Item2;
            }
            for (int j=0; j < allLines.Length; j++)
            {
                allLines[j] = allLines[j].Trim(whitespaces);
            }
            List<InternalLine> picture = ParseSVGBody(allLines); //parse whole svg drawing into list of lines
            return new Tuple<int, int, List<InternalLine>>(width, height, picture);
        }

        /// <summary>
        /// parses the svg file header and returns the internal coordinate range of this drawing, and iterates i to point to the start of svg element definitions
        /// </summary>
        /// <param name="allLines">an array holding all lines of the input file</param>
        /// <returns>the internal coordinate range of this drawing</returns>
        private Tuple<int, int> ParseSVGHeader(String[] allLines)
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
            return new Tuple<int, int>(width, height);
        }

        /// <summary>
        /// parses all relevant svg element definitions and skips the ones not representable by the sketch assistant
        /// </summary>
        /// <param name="allLines">an array holding all lines of the input file</param>
        /// <returns>the parsed picture as a list of lines</returns>
        private List<InternalLine> ParseSVGBody(String[] allLines)
        {
            List<InternalLine> picture = new List<InternalLine>();
            while (!allLines[i].StartsWith("</svg"))
            {
                List<InternalLine> element = ParseSingleSVGElement(allLines);
                if (element != null)
                {
                    picture.AddRange(element);
                }
                i++;
                if (i > allLines.Length - 1) throw new FileImporterException("unterminated input file: missing </svg> tag", "the file must not contain empty lines", i + 1);
            }
            return picture;
        }

        /// <summary>
        /// parses one toplevel svg element
        /// </summary>
        /// <param name="allLines">an array holding all lines of the input file</param>
        /// <returns>the parsed Element as a list of lines</returns>
        private List<InternalLine> ParseSingleSVGElement(string[] allLines)
        {
            String[] currentElement = GetCurrentElement(allLines);
            return ParseSingleLineSVGElement(currentElement);
        }

        /// <summary>
        /// parses a single toplevel svg element only taking one line
        /// </summary>
        /// <param name="allLines">an array holding all lines of the input file</param>
        /// <returns>the parsed element as a Line object, or null if the element is not supported</returns>
        private List<InternalLine> ParseSingleLineSVGElement(string[] currentElement)
        {
            List<Point> points= null;
            List<InternalLine> element = null;
            switch (currentElement[0])
            {
                case "<rect":
                    points = ParseRect(currentElement);
                    break;
                case "<circle":
                    points = ParseCircle(currentElement);
                    break;
                case "<ellipse":
                    points = ParseEllipse(currentElement);
                    break;
                case "<line":
                    points = ParseLine(currentElement);
                    break;
                case "<polyline":
                    points = ParsePolyline(currentElement);
                    break;
                case "<polygon":
                    points = ParsePolygon(currentElement);
                    break;
                case "<path":
                    element = ParsePath(currentElement);
                    break;
                default: //unsupported svg element
                    return null; //simply ignore
            }
            if (element == null)
            {
                element = new List<InternalLine>();
                element.Add(new InternalLine(points));
            }
            return element;
        }

        /// <summary>
        /// parses a rectangle definition into a List of Points representing a single line around the rectangle (in clockwise direction), ignoring rounded corners
        /// </summary>
        /// <param name="currentElement">the definition of the element as whitespace seperated String[]</param>
        /// <returns>the parsed element as a List of Points</returns>
        private List<Point> ParseRect(string[] currentElement)
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
            return rect;
        }

        /// <summary>
        /// parses a circle definition into a List of Points
        /// </summary>
        /// <param name="currentElement">the definition of the element as whitespace seperated String[]</param>
        /// <returns>the parsed element as a List of Points</returns>
        private List<Point> ParseCircle(string[] currentElement)
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
        private List<Point> ParseEllipse(string[] currentElement)
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
        private List<Point> ParseLine(string[] currentElement)
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
        private List<Point> ParsePolyline(string[] currentElement)
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
        private List<Point> ParsePolygon(string[] currentElement)
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
            }
            polygon.Add(ScaleAndCreatePoint(Convert.ToDouble(points[0], CultureInfo.InvariantCulture), Convert.ToDouble(points[1], CultureInfo.InvariantCulture))); //close polygon
            return polygon;
        }

        /// <summary>
        /// parses a path definition into a List of Points
        /// </summary>
        /// <param name="currentElement">the definition of the element as whitespace seperated String[]</param>
        /// <returns>the parsed element as a List of Points</returns>
        private List<InternalLine> ParsePath(string[] currentElement)
        {
            List<String> pathElements = new List<string>();
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
            NormalizePathDeclaration(pathElements); //expand path data to always explicitly have the command descriptor in front of the appropriate number of arguments and to seperate command descriptors, coordinates and other tokens always into seperate list elements (equivalent to seperation with spaces in the input file, but svg allows also for comma as a seperator, and for omitting seperators where possible without losing information (refer to svg grammer) to reduce file size
            List<InternalLine> element = new List<InternalLine>();
            List<Point> currentLine = new List<Point>();
            double lastBezierControlPointX= 0;
            double lastBezierControlPointY= 0;
            double lastPositionX;
            double lastPositionY;
            double initialPositionX= -1;
            double initialPositionY= -1;
            bool newSubpath = true;
            Tuple<List<Point>, double, double> valuesArc; //list of points, new values for: lastPositionX, lastPositionY
            Tuple<List<Point>, double, double, double, double> valuesBezierCurve; //list of points, new values for: lastPositionX, lastPositionY, lastBezierControlPointX, lastBezierControlPointY
            var valuesSinglePoint = Parse_M_L(pathElements); //new point, new values for: lastPositionX, lastPositionY
            currentLine = new List<Point>();
            currentLine.Add(valuesSinglePoint.Item1);
            lastPositionX = valuesSinglePoint.Item2;
            lastPositionY = valuesSinglePoint.Item3;
            String currentToken;
            while (!(pathElements.Count == 0)){
                if (newSubpath)
                {
                    initialPositionX = lastPositionX; //update buffers for coordinates of first point of active subpath
                    initialPositionY = lastPositionY;
                    newSubpath = false;
                }
                currentToken = pathElements.First();
                if (currentToken.Equals("M"))
                {
                    element.Add(new InternalLine(currentLine)); //save current line
                    valuesSinglePoint = Parse_M_L(pathElements);
                    currentLine = new List<Point>(); //create new empty line
                    currentLine.Add(valuesSinglePoint.Item1); //add point to new line
                    lastPositionX = valuesSinglePoint.Item2; //save last point coordinates
                    lastPositionY = valuesSinglePoint.Item3; //save last point coordinates
                }
                else if (currentToken.Equals("m"))
                {
                    element.Add(new InternalLine(currentLine)); //save current line
                    valuesSinglePoint = Parse_m_l(pathElements, lastPositionX, lastPositionY);
                    currentLine = new List<Point>(); //create new empty line
                    currentLine.Add(valuesSinglePoint.Item1); //add point to new line
                    lastPositionX = valuesSinglePoint.Item2; //save last point coordinates
                    lastPositionY = valuesSinglePoint.Item3; //save last point coordinates
                }
                else if (currentToken.Equals("Z") || currentToken.Equals("z"))
                {
                    valuesSinglePoint = Parse_Z(pathElements, initialPositionX, initialPositionY); //method call only used for uniform program structure... only real effect of method is to consume one token
                    newSubpath = true;
                    currentLine.Add(valuesSinglePoint.Item1); //add point to old line
                    element.Add(new InternalLine(currentLine)); //save current line
                    currentLine = new List<Point>(); //create new empty line
                    currentLine.Add(valuesSinglePoint.Item1); //add point to new line
                    lastPositionX = valuesSinglePoint.Item2; //save last point coordinates
                    lastPositionY = valuesSinglePoint.Item3; //save last point coordinates
                }
                else if (currentToken.Equals("L"))
                {
                    valuesSinglePoint = Parse_M_L(pathElements);
                    currentLine.Add(valuesSinglePoint.Item1); //add point to new line
                    lastPositionX = valuesSinglePoint.Item2; //save last point coordinates
                    lastPositionY = valuesSinglePoint.Item3; //save last point coordinates
                }
                else if (currentToken.Equals("l"))
                {
                    valuesSinglePoint = Parse_m_l(pathElements, lastPositionX, lastPositionY);
                    currentLine.Add(valuesSinglePoint.Item1); //add point to new line
                    lastPositionX = valuesSinglePoint.Item2; //save last point coordinates
                    lastPositionY = valuesSinglePoint.Item3; //save last point coordinates
                }
                else if (currentToken.Equals("H"))
                {
                    valuesSinglePoint = Parse_H(pathElements, lastPositionY);
                    currentLine.Add(valuesSinglePoint.Item1); //add point to new line
                    lastPositionX = valuesSinglePoint.Item2; //save last point coordinates
                    lastPositionY = valuesSinglePoint.Item3; //save last point coordinates
                }
                else if (currentToken.Equals("h"))
                {
                    valuesSinglePoint = Parse_h(pathElements, lastPositionX, lastPositionY);
                    currentLine.Add(valuesSinglePoint.Item1); //add point to new line
                    lastPositionX = valuesSinglePoint.Item2; //save last point coordinates
                    lastPositionY = valuesSinglePoint.Item3; //save last point coordinates
                }
                else if (currentToken.Equals("V"))
                {
                    valuesSinglePoint = Parse_V(pathElements, lastPositionX);
                    currentLine.Add(valuesSinglePoint.Item1); //add point to new line
                    lastPositionX = valuesSinglePoint.Item2; //save last point coordinates
                    lastPositionY = valuesSinglePoint.Item3; //save last point coordinates
                }
                else if (currentToken.Equals("v"))
                {
                    valuesSinglePoint = Parse_v(pathElements, lastPositionX, lastPositionY);
                    currentLine.Add(valuesSinglePoint.Item1); //add point to new line
                    lastPositionX = valuesSinglePoint.Item2; //save last point coordinates
                    lastPositionY = valuesSinglePoint.Item3; //save last point coordinates
                }
                else if (currentToken.Equals("C"))
                {
                    valuesBezierCurve = Parse_C(pathElements, lastPositionX, lastPositionY);
                    currentLine.AddRange(valuesBezierCurve.Item1); //add point to new line
                    lastPositionX = valuesBezierCurve.Item2; //save last point coordinates
                    lastPositionY = valuesBezierCurve.Item3; //save last point coordinates
                    lastBezierControlPointX = valuesBezierCurve.Item4; //save last bezier control point coordinates
                    lastBezierControlPointY = valuesBezierCurve.Item5; //save last bezier control point coordinates
                }
                else if (currentToken.Equals("c"))
                {
                    valuesBezierCurve = Parse_C(pathElements, lastPositionX, lastPositionY);
                    currentLine.AddRange(valuesBezierCurve.Item1); //add point to new line
                    lastPositionX = valuesBezierCurve.Item2; //save last point coordinates
                    lastPositionY = valuesBezierCurve.Item3; //save last point coordinates
                    lastBezierControlPointX = valuesBezierCurve.Item4; //save last bezier control point coordinates
                    lastBezierControlPointY = valuesBezierCurve.Item5; //save last bezier control point coordinates
                }
                else if (currentToken.Equals("S"))
                {
                    valuesBezierCurve = Parse_S(pathElements, lastPositionX, lastPositionY, lastBezierControlPointX, lastBezierControlPointY);
                    currentLine.AddRange(valuesBezierCurve.Item1); //add point to new line
                    lastPositionX = valuesBezierCurve.Item2; //save last point coordinates
                    lastPositionY = valuesBezierCurve.Item3; //save last point coordinates
                    lastBezierControlPointX = valuesBezierCurve.Item4; //save last bezier control point coordinates
                    lastBezierControlPointY = valuesBezierCurve.Item5; //save last bezier control point coordinates
                }
                else if (currentToken.Equals("s"))
                {
                    valuesBezierCurve = Parse_s(pathElements, lastPositionX, lastPositionY, lastBezierControlPointX, lastBezierControlPointY);
                    currentLine.AddRange(valuesBezierCurve.Item1); //add point to new line
                    lastPositionX = valuesBezierCurve.Item2; //save last point coordinates
                    lastPositionY = valuesBezierCurve.Item3; //save last point coordinates
                    lastBezierControlPointX = valuesBezierCurve.Item4; //save last bezier control point coordinates
                    lastBezierControlPointY = valuesBezierCurve.Item5; //save last bezier control point coordinates
                }
                else if (currentToken.Equals("Q"))
                {
                    valuesBezierCurve = Parse_Q(pathElements, lastPositionX, lastPositionY);
                    currentLine.AddRange(valuesBezierCurve.Item1); //add point to new line
                    lastPositionX = valuesBezierCurve.Item2; //save last point coordinates
                    lastPositionY = valuesBezierCurve.Item3; //save last point coordinates
                    lastBezierControlPointX = valuesBezierCurve.Item4; //save last bezier control point coordinates
                    lastBezierControlPointY = valuesBezierCurve.Item5; //save last bezier control point coordinates
                }
                else if (currentToken.Equals("q"))
                {
                    valuesBezierCurve = Parse_q(pathElements, lastPositionX, lastPositionY);
                    currentLine.AddRange(valuesBezierCurve.Item1); //add point to new line
                    lastPositionX = valuesBezierCurve.Item2; //save last point coordinates
                    lastPositionY = valuesBezierCurve.Item3; //save last point coordinates
                    lastBezierControlPointX = valuesBezierCurve.Item4; //save last bezier control point coordinates
                    lastBezierControlPointY = valuesBezierCurve.Item5; //save last bezier control point coordinates
                }
                else if (currentToken.Equals("T"))
                {
                    valuesBezierCurve = Parse_T(pathElements, lastPositionX, lastPositionY, lastBezierControlPointX, lastBezierControlPointY);
                    currentLine.AddRange(valuesBezierCurve.Item1); //add point to new line
                    lastPositionX = valuesBezierCurve.Item2; //save last point coordinates
                    lastPositionY = valuesBezierCurve.Item3; //save last point coordinates
                    lastBezierControlPointX = valuesBezierCurve.Item4; //save last bezier control point coordinates
                    lastBezierControlPointY = valuesBezierCurve.Item5; //save last bezier control point coordinates
                }
                else if (currentToken.Equals("t"))
                {
                    valuesBezierCurve = Parse_t(pathElements, lastPositionX, lastPositionY, lastBezierControlPointX, lastBezierControlPointY);
                    currentLine.AddRange(valuesBezierCurve.Item1); //add point to new line
                    lastPositionX = valuesBezierCurve.Item2; //save last point coordinates
                    lastPositionY = valuesBezierCurve.Item3; //save last point coordinates
                    lastBezierControlPointX = valuesBezierCurve.Item4; //save last bezier control point coordinates
                    lastBezierControlPointY = valuesBezierCurve.Item5; //save last bezier control point coordinates
                }
                else if (currentToken.Equals("A"))
                {
                    valuesArc = Parse_A(pathElements, lastPositionX, lastPositionY);
                    currentLine.AddRange(valuesArc.Item1); //add points to new line
                    lastPositionX = valuesArc.Item2; //save last point coordinates
                    lastPositionY = valuesArc.Item3; //save last point coordinates
                }
                else if (currentToken.Equals("a"))
                {
                    valuesArc = Parse_a(pathElements, lastPositionX, lastPositionY);
                    currentLine.AddRange(valuesArc.Item1); //add points to new line
                    lastPositionX = valuesArc.Item2; //save last point coordinates
                    lastPositionY = valuesArc.Item3; //save last point coordinates
                }
                else
                {
                    throw new FileImporterException("invalid path argument or path data formatting: read argument " + pathElements.First(), "valid path arguments are: {M,Z,L,H,V,C,S,Q,T,A} in upper and lower case", i + 1);
                }
            }
            if (currentLine.Count > 1)
            {
                element.Add(new InternalLine(currentLine)); //save current line
            }
            return element;
        }

        /// <summary>
        /// normalizes the declaration of the data field of a path declaration by splitting coordinates still connected by a semicolon and command descriptors which are directly attached to the following coordinate into seperate tokens, also repeats omitted command descriptor tokens when the same command is repeated multiple times
        /// </summary>
        /// <param name="pathElements">the list of tokens to normalize, by splitting up existing tokens and adding new command descriptor tokens</param>
        private void NormalizePathDeclaration(List<string> pathElements)
        {
            Char lastCommand = 'M';
            int argumentCounter = 0;
            for( int j= 0; j < pathElements.Count; j++)
            {
                String currentElement = pathElements.ElementAt(j);
                if (currentElement.Length != 1)
                {
                    if (((currentElement.First() >= 'A' && currentElement.First() <= 'Z') || (currentElement.First() >= 'a' && currentElement.First() <= 'z')) && currentElement.First() != 'e') //seperate a single command descriptor / letter
                    {
                        pathElements.RemoveAt(j);
                        pathElements.Insert(j, currentElement.First() + ""); //insert letter as seperate element
                        pathElements.Insert(j + 1, currentElement.Substring(1)); //insert rest of String at next position so it will be processed again
                        lastCommand = currentElement.First();
                        argumentCounter = 0;
                    }
                    else if ((currentElement.First() >= '0' && currentElement.First() <= '9') || currentElement.First() == '-' || currentElement.First() == '+' || currentElement.First() != 'e') //seperate a single coordinate / number
                    {
                        bool repeatCommandDescriptor = false; 
                        switch (lastCommand){ //check for reaching of next command with omitted command descriptor
                            case 'M':
                                if (argumentCounter >= 2) repeatCommandDescriptor = true;
                                break;
                            case 'm':
                                if (argumentCounter >= 2) repeatCommandDescriptor = true;
                                break;
                            case 'L':
                                if (argumentCounter >= 2) repeatCommandDescriptor = true;
                                break;
                            case 'l':
                                if (argumentCounter >= 2) repeatCommandDescriptor = true;
                                break;
                            case 'V':
                                if (argumentCounter >= 1) repeatCommandDescriptor = true;
                                break;
                            case 'v':
                                if (argumentCounter >= 1) repeatCommandDescriptor = true;
                                break;
                            case 'H':
                                if (argumentCounter >= 1) repeatCommandDescriptor = true;
                                break;
                            case 'h':
                                if (argumentCounter >= 1) repeatCommandDescriptor = true;
                                break;
                            case 'C':
                                if (argumentCounter >= 6) repeatCommandDescriptor = true;
                                break;
                            case 'c':
                                if (argumentCounter >= 6) repeatCommandDescriptor = true;
                                break;
                            case 'S':
                                if (argumentCounter >= 4) repeatCommandDescriptor = true;
                                break;
                            case 's':
                                if (argumentCounter >= 4) repeatCommandDescriptor = true;
                                break;
                            case 'Q':
                                if (argumentCounter >= 4) repeatCommandDescriptor = true;
                                break;
                            case 'q':
                                if (argumentCounter >= 4) repeatCommandDescriptor = true;
                                break;
                            case 'T':
                                if (argumentCounter >= 2) repeatCommandDescriptor = true;
                                break;
                            case 't':
                                if (argumentCounter >= 2) repeatCommandDescriptor = true;
                                break;
                            case 'A':
                                if (argumentCounter >= 7) repeatCommandDescriptor = true;
                                break;
                            case 'a':
                                if (argumentCounter >= 7) repeatCommandDescriptor = true;
                                break;
                        }
                        if (repeatCommandDescriptor)
                        {
                            pathElements.Insert(j, lastCommand + ""); //repeat command descriptor
                            j++; //skip command descriptor (was put into active position in the list
                            argumentCounter = 0; //reset argument counter
                        }
                        bool decimalPointEncountered = false;
                        for (int k = 1; k < currentElement.Length; k++)
                        {
                            if (!decimalPointEncountered && currentElement.ElementAt(k) == '.') //allow up to one decimal point in numbers
                            {
                                decimalPointEncountered = true;
                            }
                            else if (!((currentElement.ElementAt(k) >= '0' && currentElement.ElementAt(k) <= '9') || currentElement.First() == '-' || currentElement.First() == '+' || currentElement.First() != 'e'))
                            {
                                pathElements.RemoveAt(j);
                                pathElements.Insert(j, currentElement.Substring(0, k - 1)); //insert number as seperate element
                                pathElements.Insert(j + 1, currentElement.Substring(k)); //insert rest of String at next position so it will be processed again
                                break;
                            }
                        }
                        argumentCounter++; 
                    }
                    else //parse non-space seperators and skip other unsupported characters (the only other valid ones per svg standard would be weird tokens looking like format descriptors (e.g. '#xC'), these are unsopported and will likely cause an error or other inconsitencies during parsing)
                    {
                        for (int k = 1; k < currentElement.Length; k++)
                        {
                            if (((currentElement.ElementAt(k) >= '0' && currentElement.ElementAt(k) <= '9')) || currentElement.ElementAt(k) == '-' || currentElement.ElementAt(k) == '+' || (currentElement.ElementAt(k) >= 'A' && currentElement.ElementAt(k) <= 'Z') || (currentElement.ElementAt(k) >= 'a' && currentElement.ElementAt(k) <= 'z'))
                            {
                                pathElements.RemoveAt(j);
                                pathElements.Insert(j + 1, currentElement.Substring(k)); //insert rest of String at next position so it will be processed again
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if ((currentElement.First() >= 'A' && currentElement.First() <= 'Z') || (currentElement.First() >= 'a' && currentElement.First() <= 'z')) //update lastCommand buffer when reading single letter
                    {
                        lastCommand = currentElement.First();
                        argumentCounter = 0;
                    }
                    else if(!(currentElement.First() >= '0' && currentElement.First() <= '9')) //not a number
                    {
                        pathElements.RemoveAt(j); //remove element
                        j--; //decrement index pointer so next element will not be skipped (indices of all folowing elements just decreased by 1)
                    }
                    else //a single digit number
                    {
                        bool repeatCommandDescriptor = false;
                        switch (lastCommand)
                        { //check for reaching of next command with omitted command descriptor
                            case 'M':
                                if (argumentCounter >= 2) repeatCommandDescriptor = true;
                                break;
                            case 'm':
                                if (argumentCounter >= 2) repeatCommandDescriptor = true;
                                break;
                            case 'L':
                                if (argumentCounter >= 2) repeatCommandDescriptor = true;
                                break;
                            case 'l':
                                if (argumentCounter >= 2) repeatCommandDescriptor = true;
                                break;
                            case 'V':
                                if (argumentCounter >= 1) repeatCommandDescriptor = true;
                                break;
                            case 'v':
                                if (argumentCounter >= 1) repeatCommandDescriptor = true;
                                break;
                            case 'H':
                                if (argumentCounter >= 1) repeatCommandDescriptor = true;
                                break;
                            case 'h':
                                if (argumentCounter >= 1) repeatCommandDescriptor = true;
                                break;
                            case 'C':
                                if (argumentCounter >= 6) repeatCommandDescriptor = true;
                                break;
                            case 'c':
                                if (argumentCounter >= 6) repeatCommandDescriptor = true;
                                break;
                            case 'S':
                                if (argumentCounter >= 4) repeatCommandDescriptor = true;
                                break;
                            case 's':
                                if (argumentCounter >= 4) repeatCommandDescriptor = true;
                                break;
                            case 'Q':
                                if (argumentCounter >= 4) repeatCommandDescriptor = true;
                                break;
                            case 'q':
                                if (argumentCounter >= 4) repeatCommandDescriptor = true;
                                break;
                            case 'T':
                                if (argumentCounter >= 2) repeatCommandDescriptor = true;
                                break;
                            case 't':
                                if (argumentCounter >= 2) repeatCommandDescriptor = true;
                                break;
                            case 'A':
                                if (argumentCounter >= 7) repeatCommandDescriptor = true;
                                break;
                            case 'a':
                                if (argumentCounter >= 7) repeatCommandDescriptor = true;
                                break;
                        }
                        if (repeatCommandDescriptor)
                        {
                            pathElements.Insert(j, lastCommand + ""); //repeat command descriptor
                            j++; //skip command descriptor (was put into active position in the list
                            argumentCounter = 0; //reset argument counter
                        }
                        argumentCounter++;
                    }
                }
            }
        }

        /// <summary>
        /// parses a "closeloop" path element
        /// </summary>
        /// <param name="pathElements">a list of all not yet parsed path element tokens and values in correct order, starting with the element to be parsed</param>
        /// <param name="initialPositionX">absolute x coordinate of the last initial point of this subpath</param>
        /// <param name="initialPositionY">absolute y coordinate of the last initial point of this subpath</param>
        /// <returns></returns>
        private Tuple<Point, double, double> Parse_Z(List<string> pathElements, double initialPositionX, double initialPositionY)
        {
            pathElements.RemoveAt(0); //remove element descriptor token
            return new Tuple<Point, double, double>(ScaleAndCreatePoint(initialPositionX, initialPositionY), initialPositionX, initialPositionY);
        }

        /// <summary>
        /// parses a "moveto", "close loop" or "lineto" path element with absolute coordinates
        /// </summary>
        /// <param name="pathElements">a list of all not yet parsed path element tokens and values in correct order, starting with the element to be parsed</param>
        /// <returns>the point at the end of the move, close loop or line action and its exact, unscaled coordinates</returns>
        private Tuple<Point, double, double> Parse_M_L(List<string> pathElements)
        {
            pathElements.RemoveAt(0); //remove element descriptor token
            double x = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse x coordinate
            pathElements.RemoveAt(0); //remove x coordinate token
            double y = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse y coordinate
            pathElements.RemoveAt(0); //remove y coordinate token
            return new Tuple<Point, double, double>(ScaleAndCreatePoint(x, y), x, y);
        }

        /// <summary>
        /// parses a "moveto", "close loop" or "lineto" path element with relative coordinates
        /// </summary>
        /// <param name="pathElements">a list of all not yet parsed path element tokens and values in correct order, starting with the element to be parsed</param>
        /// <param name="lastPositionX">absolute x coordinate of the last active point</param>
        /// <param name="lastPositionY">absolute y coordinate of the last active point</param>
        /// <returns>the point at the end of the move, close loop or line action and its exact, unscaled coordinates</returns>
        private Tuple<Point, double, double> Parse_m_l(List<string> pathElements, double lastPositionX, double lastPositionY)
        {
            pathElements.RemoveAt(0); //remove element descriptor token
            double x = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse relative x coordinate
            pathElements.RemoveAt(0); //remove x coordinate token
            double y = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse relative y coordinate
            pathElements.RemoveAt(0); //remove y coordinate token
            x = lastPositionX + x; //compute absolute x coordinate
            y = lastPositionY + y; //compute absolute y coordinate
            return new Tuple<Point, double, double>(ScaleAndCreatePoint(x, y), x, y);
        }

        /// <summary>
        /// parses a "horizontal lineto" path element with absolute coordinates
        /// </summary>
        /// <param name="pathElements">a list of all not yet parsed path element tokens and values in correct order, starting with the element to be parsed</param>
        /// <param name="lastPositionY">absolute y coordinate of the last active point</param>
        /// <returns>the point at the end of the horizontal line action and its exact, unscaled coordinates</returns>
        private Tuple<Point, double, double> Parse_H(List<string> pathElements, double lastPositionY)
        {
            pathElements.RemoveAt(0); //remove element descriptor token
            double x = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse x coordinate
            pathElements.RemoveAt(0); //remove x coordinate token
            return new Tuple<Point, double, double>(ScaleAndCreatePoint(x, lastPositionY), x, lastPositionY);
        }

        /// <summary>
        /// parses a "horizontal lineto" path element with relative coordinates
        /// </summary>
        /// <param name="pathElements">a list of all not yet parsed path element tokens and values in correct order, starting with the element to be parsed</param>
        /// <param name="lastPositionX">absolute x coordinate of the last active point</param>
        /// <param name="lastPositionY">absolute y coordinate of the last active point</param>
        /// <returns>the point at the end of the horizontal line action and its exact, unscaled coordinates</returns>
        private Tuple<Point, double, double> Parse_h(List<string> pathElements, double lastPositionX, double lastPositionY)
        {
            pathElements.RemoveAt(0); //remove element descriptor token
            double x = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse relative x coordinate
            pathElements.RemoveAt(0); //remove x coordinate token
            x = lastPositionX + x; //compute absolute x coordinate
            return new Tuple<Point, double, double>(ScaleAndCreatePoint(x, lastPositionY), x, lastPositionY);
        }

        /// <summary>
        /// parses a "vertical lineto" path element with absolute coordinates
        /// </summary>
        /// <param name="pathElements">a list of all not yet parsed path element tokens and values in correct order, starting with the element to be parsed</param>
        /// <param name="lastPositionX">absolute x coordinate of the last active point</param>
        /// <returns>the point at the end of the vertical line action and its exact, unscaled coordinates</returns>
        private Tuple<Point, double, double> Parse_V(List<string> pathElements, double lastPositionX)
        {
            pathElements.RemoveAt(0); //remove element descriptor token
            double y = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse y coordinate
            pathElements.RemoveAt(0); //remove y coordinate token
            return new Tuple<Point, double, double>(ScaleAndCreatePoint(lastPositionX, y), lastPositionX, y);
        }

        /// <summary>
        /// parses a "vertical lineto" path element with relative coordinates
        /// </summary>
        /// <param name="pathElements">a list of all not yet parsed path element tokens and values in correct order, starting with the element to be parsed</param>
        /// <param name="lastPositionX">absolute x coordinate of the last active point</param>
        /// <param name="lastPositionY">absolute y coordinate of the last active point</param>
        /// <returns>the point at the end of the vertical line action and its exact, unscaled coordinates</returns>
        private Tuple<Point, double, double> Parse_v(List<string> pathElements, double lastPositionX, double lastPositionY)
        {
            pathElements.RemoveAt(0); //remove element descriptor token
            double y = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse relative y coordinate
            pathElements.RemoveAt(0); //remove y coordinate token
            y = lastPositionY + y; //compute absolute y coordinate
            return new Tuple<Point, double, double>(ScaleAndCreatePoint(lastPositionX, y), lastPositionX, y);
        }

        /// <summary>
        /// parses a "cubic bezier curve" path element with absolute coordinates
        /// </summary>
        /// <param name="pathElements">a list of all not yet parsed path element tokens and values in correct order, starting with the element to be parsed</param>
        /// <param name="lastPositionX">absolute x coordinate of the last active point</param>
        /// <param name="lastPositionY">absolute y coordinate of the last active point</param>
        /// <returns>a List of Points containing all sampled points on the bezier curve, aswell as the unscaled x and y coordinates of the last point of the curve and of the second bezier control point</returns>
        private Tuple<List<Point>, double, double, double, double> Parse_C(List<string> pathElements, double lastPositionX, double lastPositionY)
        {
            pathElements.RemoveAt(0); //remove element descriptor token
            double x1 = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse first control point x coordinate
            pathElements.RemoveAt(0); //remove x coordinate token
            double y1 = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse first control point y coordinate
            pathElements.RemoveAt(0); //remove y coordinate token
            double x2 = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse second control point x coordinate
            pathElements.RemoveAt(0); //remove x coordinate token
            double y2 = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse second control point y coordinate
            pathElements.RemoveAt(0); //remove y coordinate token
            double x = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse target point x coordinate
            pathElements.RemoveAt(0); //remove x coordinate token
            double y = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse target point y coordinate
            pathElements.RemoveAt(0); //remove y coordinate token
            return new Tuple<List<Point>, double, double, double, double>(SampleCubicBezier(lastPositionX, lastPositionY, x1, y1, x2, y2, x, y), x, y, x2, y2);
        }

        /// <summary>
        /// parses a "cubic bezier curve" path element with relative coordinates
        /// </summary>
        /// <param name="pathElements">a list of all not yet parsed path element tokens and values in correct order, starting with the element to be parsed</param>
        /// <param name="lastPositionX">absolute x coordinate of the last active point</param>
        /// <param name="lastPositionY">absolute y coordinate of the last active point</param>
        /// <returns>a List of Points containing all sampled points on the bezier curve, aswell as the unscaled x and y coordinates of the last point of the curve and of the second bezier control point</returns>
        private Tuple<List<Point>, double, double, double, double> Parse_c(List<string> pathElements, double lastPositionX, double lastPositionY)
        {
            pathElements.RemoveAt(0); //remove element descriptor token
            double x1 = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse first control point x coordinate
            pathElements.RemoveAt(0); //remove x coordinate token
            double y1 = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse first control point y coordinate
            pathElements.RemoveAt(0); //remove y coordinate token
            double x2 = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse second control point x coordinate
            pathElements.RemoveAt(0); //remove x coordinate token
            double y2 = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse second control point y coordinate
            pathElements.RemoveAt(0); //remove y coordinate token
            double x = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse target point x coordinate
            pathElements.RemoveAt(0); //remove x coordinate token
            double y = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse target point y coordinate
            pathElements.RemoveAt(0); //remove y coordinate token
            x1 = lastPositionX + x1; //compute absolute x coordinate
            y1 = lastPositionY + y1; //compute absolute y coordinate
            x2 = lastPositionX + x2; //compute absolute x coordinate
            y2 = lastPositionY + y2; //compute absolute y coordinate
            x = lastPositionX + x; //compute absolute x coordinate
            y = lastPositionY + y; //compute absolute y coordinate
            return new Tuple<List<Point>, double, double, double, double>(SampleCubicBezier(lastPositionX, lastPositionY, x1, y1, x2, y2, x, y), x, y, x2, y2);
        }

        /// <summary>
        /// parses a "cubic bezier curve shorthand" path element with absolute coordinates
        /// </summary>
        /// <param name="pathElements">a list of all not yet parsed path element tokens and values in correct order, starting with the element to be parsed</param>
        /// <param name="lastPositionX">absolute x coordinate of the last active point</param>
        /// <param name="lastPositionY">absolute y coordinate of the last active point</param>
        /// <param name="lastBezierControlPointX">absolute x coordinate of the last bezier control point of the previous bezier curve</param>
        /// <param name="lastBezierControlPointY">absolute y coordinate of the last bezier control point of the previous bezier curve</param>
        /// <returns>a List of Points containing all sampled points on the bezier curve, aswell as the unscaled x and y coordinates of the last point of the curve and of the second bezier control point</returns>
        private Tuple<List<Point>, double, double, double, double> Parse_S(List<string> pathElements, double lastPositionX, double lastPositionY, double lastBezierControlPointX, double lastBezierControlPointY)
        {
            pathElements.RemoveAt(0); //remove element descriptor token
            double x2 = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse second control point x coordinate
            pathElements.RemoveAt(0); //remove x coordinate token
            double y2 = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse second control point y coordinate
            pathElements.RemoveAt(0); //remove y coordinate token
            double x = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse target point x coordinate
            pathElements.RemoveAt(0); //remove x coordinate token
            double y = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse target point y coordinate
            pathElements.RemoveAt(0); //remove y coordinate token
            double x1 = lastPositionX + (lastPositionX - lastBezierControlPointX); //mirror last bezier control point at bezier start point to get first new bezier control point
            double y1 = lastPositionY + (lastPositionY - lastBezierControlPointY); //mirror last bezier control point at bezier start point to get first new bezier control point
            return new Tuple<List<Point>, double, double, double, double>(SampleCubicBezier(lastPositionX, lastPositionY, x1, y1, x2, y2, x, y), x, y, x2, y2);
        }

        /// <summary>
        /// parses a "cubic bezier curve shorthand" path element with relative coordinates
        /// </summary>
        /// <param name="pathElements">a list of all not yet parsed path element tokens and values in correct order, starting with the element to be parsed</param>
        /// <param name="lastPositionX">absolute x coordinate of the last active point</param>
        /// <param name="lastPositionY">absolute y coordinate of the last active point</param>
        /// <param name="lastBezierControlPointX">absolute x coordinate of the last bezier control point of the previous bezier curve</param>
        /// <param name="lastBezierControlPointY">absolute y coordinate of the last bezier control point of the previous bezier curve</param>
        /// <returns>a List of Points containing all sampled points on the bezier curve, aswell as the unscaled x and y coordinates of the last point of the curve and of the second bezier control point</returns>
        private Tuple<List<Point>, double, double, double, double> Parse_s(List<string> pathElements, double lastPositionX, double lastPositionY, double lastBezierControlPointX, double lastBezierControlPointY)
        {
            pathElements.RemoveAt(0); //remove element descriptor token
            double x2 = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse second control point x coordinate
            pathElements.RemoveAt(0); //remove x coordinate token
            double y2 = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse second control point y coordinate
            pathElements.RemoveAt(0); //remove y coordinate token
            double x = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse target point x coordinate
            pathElements.RemoveAt(0); //remove x coordinate token
            double y = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse target point y coordinate
            pathElements.RemoveAt(0); //remove y coordinate token
            double x1 = lastPositionX + (lastPositionX - lastBezierControlPointX); //mirror last bezier control point at bezier start point to get first new bezier control point
            double y1 = lastPositionY + (lastPositionY - lastBezierControlPointY); //mirror last bezier control point at bezier start point to get first new bezier control point
            x2 = lastPositionX + x2; //compute absolute x coordinate
            y2 = lastPositionY + y2; //compute absolute y coordinate
            x = lastPositionX + x; //compute absolute x coordinate
            y = lastPositionY + y; //compute absolute y coordinate
            return new Tuple<List<Point>, double, double, double, double>(SampleCubicBezier(lastPositionX, lastPositionY, x1, y1, x2, y2, x, y), x, y, x2, y2);
        }

        /// <summary>
        /// parses a "quadratic bezier curve" path element with absolute coordinates
        /// </summary>
        /// <param name="pathElements">a list of all not yet parsed path element tokens and values in correct order, starting with the element to be parsed</param>
        /// <param name="lastPositionX">absolute x coordinate of the last active point</param>
        /// <param name="lastPositionY">absolute y coordinate of the last active point</param>
        /// <returns>a List of Points containing all sampled points on the bezier curve, aswell as the unscaled x and y coordinates of the last point of the curve and of the bezier control point</returns>
        private Tuple<List<Point>, double, double, double, double> Parse_Q(List<string> pathElements, double lastPositionX, double lastPositionY)
        {
            pathElements.RemoveAt(0); //remove element descriptor token
            double x1 = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse control point x coordinate
            pathElements.RemoveAt(0); //remove x coordinate token
            double y1 = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse control point y coordinate
            pathElements.RemoveAt(0); //remove y coordinate token
            double x = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse target point x coordinate
            pathElements.RemoveAt(0); //remove x coordinate token
            double y = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse target point y coordinate
            pathElements.RemoveAt(0); //remove y coordinate token
            return new Tuple<List<Point>, double, double, double, double>(SampleQuadraticBezier(lastPositionX, lastPositionY, x1, y1, x, y), x, y, x1, y1);
        }

        /// <summary>
        /// parses a "quadratic bezier curve" path element with relative coordinates
        /// </summary>
        /// <param name="pathElements">a list of all not yet parsed path element tokens and values in correct order, starting with the element to be parsed</param>
        /// <param name="lastPositionX">absolute x coordinate of the last active point</param>
        /// <param name="lastPositionY">absolute y coordinate of the last active point</param>
        /// <returns>a List of Points containing all sampled points on the bezier curve, aswell as the unscaled x and y coordinates of the last point of the curve and of the bezier control point</returns>
        private Tuple<List<Point>, double, double, double, double> Parse_q(List<string> pathElements, double lastPositionX, double lastPositionY)
        {
            pathElements.RemoveAt(0); //remove element descriptor token
            double x1 = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse control point x coordinate
            pathElements.RemoveAt(0); //remove x coordinate token
            double y1 = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse control point y coordinate
            pathElements.RemoveAt(0); //remove y coordinate token
            double x = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse target point x coordinate
            pathElements.RemoveAt(0); //remove x coordinate token
            double y = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse target point y coordinate
            pathElements.RemoveAt(0); //remove y coordinate token
            x1 = lastPositionX + x1; //compute absolute x coordinate
            y1 = lastPositionY + y1; //compute absolute y coordinate
            x = lastPositionX + x; //compute absolute x coordinate
            y = lastPositionY + y; //compute absolute y coordinate
            return new Tuple<List<Point>, double, double, double, double>(SampleQuadraticBezier(lastPositionX, lastPositionY, x1, y1, x, y), x, y, x1, y1);
        }

        /// <summary>
        /// parses a "quadratic bezier curve shorthand" path element with absolute coordinates
        /// </summary>
        /// <param name="pathElements">a list of all not yet parsed path element tokens and values in correct order, starting with the element to be parsed</param>
        /// <param name="lastPositionX">absolute x coordinate of the last active point</param>
        /// <param name="lastPositionY">absolute y coordinate of the last active point</param>
        /// <param name="lastBezierControlPointX">absolute x coordinate of the last bezier control point of the previous bezier curve</param>
        /// <param name="lastBezierControlPointY">absolute y coordinate of the last bezier control point of the previous bezier curve</param>
        /// <returns>a List of Points containing all sampled points on the bezier curve, aswell as the unscaled x and y coordinates of the last point of the curve and of the bezier control point</returns>
        private Tuple<List<Point>, double, double, double, double> Parse_T(List<string> pathElements, double lastPositionX, double lastPositionY, double lastBezierControlPointX, double lastBezierControlPointY)
        {
            pathElements.RemoveAt(0); //remove element descriptor token
            double x = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse target point x coordinate
            pathElements.RemoveAt(0); //remove x coordinate token
            double y = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse target point y coordinate
            pathElements.RemoveAt(0); //remove y coordinate token
            double x1 = lastPositionX + (lastPositionX - lastBezierControlPointX); //mirror last bezier control point at bezier start point to get first new bezier control point
            double y1 = lastPositionY + (lastPositionY - lastBezierControlPointY); //mirror last bezier control point at bezier start point to get first new bezier control point
            return new Tuple<List<Point>, double, double, double, double>(SampleQuadraticBezier(lastPositionX, lastPositionY, x1, y1, x, y), x, y, x1, y1);
        }

        /// <summary>
        /// parses a "quadratic bezier curve shorthand" path element with relative coordinates
        /// </summary>
        /// <param name="pathElements">a list of all not yet parsed path element tokens and values in correct order, starting with the element to be parsed</param>
        /// <param name="lastPositionX">absolute x coordinate of the last active point</param>
        /// <param name="lastPositionY">absolute y coordinate of the last active point</param>
        /// <param name="lastBezierControlPointX">absolute x coordinate of the last bezier control point of the previous bezier curve</param>
        /// <param name="lastBezierControlPointY">absolute y coordinate of the last bezier control point of the previous bezier curve</param>
        /// <returns>a List of Points containing all sampled points on the bezier curve, aswell as the unscaled x and y coordinates of the last point of the curve and of the bezier control point</returns>
        private Tuple<List<Point>, double, double, double, double> Parse_t(List<string> pathElements, double lastPositionX, double lastPositionY, double lastBezierControlPointX, double lastBezierControlPointY)
        {
            pathElements.RemoveAt(0); //remove element descriptor token
            double x = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse target point x coordinate
            pathElements.RemoveAt(0); //remove x coordinate token
            double y = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse target point y coordinate
            pathElements.RemoveAt(0); //remove y coordinate token
            x = lastPositionX + x; //compute absolute x coordinate
            y = lastPositionY + y; //compute absolute y coordinate
            double x1 = lastPositionX + (lastPositionX - lastBezierControlPointX); //mirror last bezier control point at bezier start point to get first new bezier control point
            double y1 = lastPositionY + (lastPositionY - lastBezierControlPointY); //mirror last bezier control point at bezier start point to get first new bezier control point
            return new Tuple<List<Point>, double, double, double, double>(SampleQuadraticBezier(lastPositionX, lastPositionY, x1, y1, x, y), x, y, x1, y1);
        }

        /// <summary>
        /// parses a "elliptical arc" path element with absolute coordinates
        /// </summary>
        /// <param name="pathElements">a list of all not yet parsed path element tokens and values in correct order, starting with the element to be parsed</param>
        /// <param name="lastPositionX">absolute x coordinate of the last active point</param>
        /// <param name="lastPositionY">absolute y coordinate of the last active point</param>
        /// <returns>a List of Points containing all sampled points on the elliptic arc, aswell as the unscaled x and y coordinates of the last point of the arc<returns>
        private Tuple<List<Point>, double, double> Parse_A(List<string> pathElements, double lastPositionX, double lastPositionY)
        {
            pathElements.RemoveAt(0); //remove element descriptor token
            double rx = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse x radius
            pathElements.RemoveAt(0); //remove x radius token
            double ry = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse y radius
            pathElements.RemoveAt(0); //remove y radius token
            double thetha = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse rotation
            pathElements.RemoveAt(0); //remove rotation token
            bool largeArcFlag = Convert.ToInt16(pathElements.First()) == 1 ? true : false; //parse large arc flag
            pathElements.RemoveAt(0); //remove large arc flag token
            bool sweepFlag = Convert.ToInt16(pathElements.First()) == 1 ? true : false; //parse sweep flag
            pathElements.RemoveAt(0); //remove sweep flag token
            double x = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse target point x coordinate
            pathElements.RemoveAt(0); //remove x coordinate token
            double y = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse target point y coordinate
            pathElements.RemoveAt(0); //remove y coordinate token
            x = x - lastPositionX; //compute relative x coordinate
            y = y - lastPositionY; //compute relative y coordinate
            return new Tuple<List<Point>, double, double>(SampleArc(lastPositionX, lastPositionY, rx, ry, x, y, thetha, largeArcFlag, sweepFlag), x, y);
        }

        /// <summary>
        /// parses a "elliptical arc" path element with relative coordinates
        /// </summary>
        /// <param name="pathElements">a list of all not yet parsed path element tokens and values in correct order, starting with the element to be parsed</param>
        /// <param name="lastPositionX">absolute x coordinate of the last active point</param>
        /// <param name="lastPositionY">absolute y coordinate of the last active point</param>
        /// <returns>a List of Points containing all sampled points on the elliptic arc, aswell as the unscaled x and y coordinates of the last point of the arc</returns>
        private Tuple<List<Point>, double, double> Parse_a(List<string> pathElements, double lastPositionX, double lastPositionY)
        {
            pathElements.RemoveAt(0); //remove element descriptor token
            double rx = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse x radius
            pathElements.RemoveAt(0); //remove x radius token
            double ry = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse y radius
            pathElements.RemoveAt(0); //remove y radius token
            double thetha = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse rotation
            pathElements.RemoveAt(0); //remove rotation token
            bool largeArcFlag = Convert.ToInt16(pathElements.First()) == 1 ? true : false; //parse large arc flag
            pathElements.RemoveAt(0); //remove large arc flag token
            bool sweepFlag = Convert.ToInt16(pathElements.First()) == 1 ? true : false; //parse sweep flag
            pathElements.RemoveAt(0); //remove sweep flag token
            double x = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse target point x coordinate
            pathElements.RemoveAt(0); //remove x coordinate token
            double y = Convert.ToDouble(pathElements.First(), CultureInfo.InvariantCulture); //parse target point y coordinate
            pathElements.RemoveAt(0); //remove y coordinate token
            return new Tuple<List<Point>, double, double>(SampleArc(lastPositionX, lastPositionY, rx, ry, x, y, thetha, largeArcFlag, sweepFlag), x, y);
        }

        /// <summary>
        /// samples an arc of an ellipse into a list of points
        /// </summary>
        /// <param name="lastPositionX">x coordinate of last point</param>
        /// <param name="lastPositionY">y coordinate of last point</param>
        /// <param name="rx">x radius of the ellipse</param>
        /// <param name="ry">y radius of the ellipse</param>
        /// <param name="nextPositionXRelative">x coordinate of next point</param>
        /// <param name="nextPositionYRelative">y coordinate of next point</param>
        /// <param name="thetha">rotation of the ellipse around the x axis</param>
        /// <param name="largeArcFlag">flag determining if the large or the small arc is to be drawn</param>
        /// <param name="sweepFlag">flag determining in which direction the arc is to be drawn (false = ccw, true = cw)</param>
        /// <returns></returns>
        private List<Point> SampleArc(double lastPositionX, double lastPositionY, double rx, double ry, double nextPositionXRelative, double nextPositionYRelative, double thetha, bool largeArcFlag, bool sweepFlag)
        {
            double cos = Math.Cos(thetha / 180 * Math.PI);
            double sin = Math.Sin(thetha / 180 * Math.PI);
            double targetXTransformed = cos * nextPositionXRelative - sin * nextPositionYRelative; //rotate target point counterclockwise around the start point by [thetha] degrees, thereby practically rotating an intermediate coordinate system, which has its origin in the start point, clockwise by the same amount
            double targetYTransformed = sin * nextPositionXRelative + cos * nextPositionYRelative;
            var values = SampleEllipticArcBiasedNoRotation(rx, ry, targetXTransformed, targetYTransformed, largeArcFlag, sweepFlag);
            List<Point> result = new List<Point>();
            for (int j = 0; j < values.Item1.Length; j++)
            {
                double xCoordinateRelative = cos * values.Item1[j] + sin * values.Item2[j]; //rotate backwards so intermediate coordinate system and "real" coordinate system have the same rotation again
                double yCoordinateRelative = cos * values.Item2[j] - sin * values.Item1[j];
                double xCoordinateAbsolute = lastPositionX + xCoordinateRelative; //translate relative to absolute coordinates (intermediate coordinate system is now again aligned with the "real" one (the virtual pane on which all vectorgraphic elements are placed) (note that this "real" coordinate system is still not the same as the one actually representing pixels for drawing, as it still has to be scaled appropriately (done inside the ScaleAndCreatePoint method)))
                double yCoordinateAbsolute = lastPositionY + yCoordinateRelative;
                result.Add(ScaleAndCreatePoint(xCoordinateAbsolute, yCoordinateAbsolute));
            }
            return result;
        }

        /// <summary>
        /// samples an elliptical arc with given radii through coordinate origin and endpoint with specified properties
        /// </summary>
        /// <param name="rx">x radius</param>
        /// <param name="ry">y radius</param>
        /// <param name="targetXTransformed">x coordinate of next point</param>
        /// <param name="targetYTransformed">y coordinate of next point</param>
        /// <param name="largeArcFlag">flag determining if the large or the small arc is to be drawn</param>
        /// <param name="sweepFlag">flag determining in which direction the arc is to be drawn (false = ccw, true = cw)</param>
        /// <returns></returns>
        private Tuple<double[], double[]> SampleEllipticArcBiasedNoRotation(double rx, double ry, double targetXTransformed, double targetYTransformed, bool largeArcFlag, bool sweepFlag)
        {
            double xStretchFactor = rx / ry; //get rx to ry ratio
            var values = SampleCircleArcBiasedNoRotation(ry, targetXTransformed / xStretchFactor, targetYTransformed, largeArcFlag, sweepFlag); //get a circular arc with radius ry
            for (int j = 0; j < values.Item1.Length; j++)
            {
                values.Item1[j] = values.Item1[j] * xStretchFactor; //correct x coordinates to get an elliptical arc from a circular one
            }
            return values;
        }

        /// <summary>
        /// samples a circular arc with given radius through coordinate origin and endpoint with specified properties
        /// </summary>
        /// <param name="r">radius</param>
        /// <param name="nextPositionXRelative">x coordinate of next point</param>
        /// <param name="nextPositionYRelative">y coordinate of next point</param>
        /// <param name="largeArcFlag">flag determining if the large or the small arc is to be drawn</param>
        /// <param name="sweepFlag">flag determining in which direction the arc is to be drawn (false = ccw, true = cw)</param>
        /// <returns></returns>
        private Tuple<double[], double[]> SampleCircleArcBiasedNoRotation(double r, double nextPositionXRelative, double nextPositionYRelative, bool largeArcFlag, bool sweepFlag)
        {
            // code for center computation adapted from https://stackoverflow.com/a/36211852
            double radsq = r * r;
            double q = Math.Sqrt(((nextPositionXRelative) * (nextPositionXRelative)) + ((nextPositionYRelative) * (nextPositionYRelative))); //Math.Sqrt(((x2 - x1) * (x2 - x1)) + ((y2 - y1) * (y2 - y1)));
            double x3 = (nextPositionXRelative) / 2; //(x1 + x2) / 2;
            double y3 = (nextPositionYRelative) / 2; //(y1 + y2) / 2;
            bool xPlusFlag; //flags needed to select center point "left" of the line between origin and the endpoint (will be used to select correct one ("left" or "right" one) later together with flags passed as arguments
            bool yPlusFlag;
            if (nextPositionXRelative > 0)
            {
                yPlusFlag = true; //left point lies above line
            }
            else
            {
                yPlusFlag = false; //left point lies below line
            }
            if (nextPositionYRelative > 0)
            {
                xPlusFlag = false; //left point lies left of line
            }
            else
            {
                xPlusFlag = true; //left point lies right of line
            }
            if(sweepFlag != largeArcFlag) //need "right" center point, not "left" one (refer to svg specification, sweepFlag means going around the circle in "clockwise" direction, largeArcFlag means tracing the larger of the two possible arcs in the selected direction) 
            {
                xPlusFlag = !xPlusFlag;
                yPlusFlag = !yPlusFlag;
            }
            double xC; // coordinates of center point of circle
            double yC;
            if(xPlusFlag) xC = x3 + Math.Sqrt(radsq - ((q / 2) * (q / 2))) * ((nextPositionYRelative) / q); //x3 + Math.Sqrt(radsq - ((q / 2) * (q / 2))) * ((y1 - y2) / q);
            else xC = x3 - Math.Sqrt(radsq - ((q / 2) * (q / 2))) * ((nextPositionYRelative) / q);
            if (yPlusFlag) yC = y3 + Math.Sqrt(radsq - ((q / 2) * (q / 2))) * ((nextPositionXRelative) / q); //y3 + Math.Sqrt(radsq - ((q / 2) * (q / 2))) * ((x2-x1) / q);
            else yC = y3 - Math.Sqrt(radsq - ((q / 2) * (q / 2))) * ((nextPositionXRelative) / q);
            var values = SampleCircleArcBiasedAroundCenter(-xC, -yC, nextPositionXRelative - xC, nextPositionYRelative - yC, r, largeArcFlag, sweepFlag);
            for (int j = 0; j < values.Item1.Length; j++)
            {
                values.Item1[j] = values.Item1[j] + xC; //correct center point coordinate bias
                values.Item2[j] = values.Item2[j] + yC; 
            }
            return values;
        }

        /// <summary>
        /// samples a circular arc with given radius around the center from the startpoint to the endpoint in the specified direction
        /// </summary>
        /// <param name="xStartPoint">x coordinate of the start point</param>
        /// <param name="yStartPoint">y coordinate of the start point</param>
        /// <param name="xFinalPoint">x coordinate of the final point</param>
        /// <param name="yFinalPoint">y coordinate of the final point</param>
        /// <param name="r">radius</param>
        /// <param name="clockwise">direction</param>
        /// <returns></returns>
        private Tuple<double[], double[]> SampleCircleArcBiasedAroundCenter(double xStartPoint, double yStartPoint, double xFinalPoint, double yFinalPoint, double r, bool largeArcFlag, bool clockwise)
        {
            double phiEnd = Math.Atan2(yFinalPoint, xFinalPoint); // angles between points and origin and the positive x Axis
            double phiStart = Math.Atan2(yStartPoint, xStartPoint);
            double angle = ((double)2 * Math.PI) / (double)samplingRateEllipse; //compute angle increment (equal to the one used for ellipses)
            double angleDifference = Math.Abs(phiStart - phiEnd);
            if (angleDifference > 2 * Math.PI || angleDifference < 0) throw new Exception("angleDifference out of range: " + angleDifference); //TODO remove
            if (largeArcFlag) // get larger angleDifference
            {
                if (angleDifference < Math.PI) angleDifference = ((double)2 * Math.PI) - angleDifference;  // was smaller angleDifference
            }
            else // get smaller angleDifference
            {
                if(angleDifference > Math.PI) angleDifference = ((double)2 * Math.PI) - angleDifference;  // was larger angleDifference
            }
            int numberOfPoints = (int) Math.Ceiling(angleDifference / angle);  //compute number of points to sample
            double[] xValues = new double[numberOfPoints];
            double[] yValues = new double[numberOfPoints];
            double phiCurrent = phiStart;
            for (int j = 0; j < numberOfPoints-1; j++) //compute intermediate points
            {
                if (clockwise) phiCurrent -= angle; //get new angle
                else phiCurrent += angle;
                yValues[j] = Math.Sin(phiCurrent) * r; //angles are relative to positive x Axis!
                xValues[j] = Math.Cos(phiCurrent) * r;
            }
            xValues[numberOfPoints - 1] = xFinalPoint; //(last segment always has an angle of less than or exactly 'angle')
            yValues[numberOfPoints - 1] = yFinalPoint;
            return new Tuple<double[], double[]>(xValues, yValues);
        }

        /// <summary>
        /// samples a cubic bezier curve with a static number of steps (samplingRateBezier)
        /// </summary>
        /// <param name="lastPositionX">x coordinate of last point</param>
        /// <param name="lastPositionY">y coordinate of last point</param>
        /// <param name="controlPoint1X">x coordinate of control point 1</param>
        /// <param name="controlPoint1Y">y coordinate of control point 1</param>
        /// <param name="controlPoint2X">x coordinate of control point 2</param>
        /// <param name="controlPoint2Y">y coordinate of control point 2</param>
        /// <param name="nextPositionX">x coordinate of next point</param>
        /// <param name="nextPositionY">y coordinate of next point</param>
        /// <returns>a List of Points containing all sampled points</returns>
        private List<Point> SampleCubicBezier(double lastPositionX, double lastPositionY, double controlPoint1X, double controlPoint1Y, double controlPoint2X, double controlPoint2Y, double nextPositionX, double nextPositionY)
        {
            var line1 = CreateDiscreteLine(lastPositionX, lastPositionY, controlPoint1X, controlPoint1Y);
            var line2 = CreateDiscreteLine(controlPoint1X, controlPoint1Y, controlPoint2X, controlPoint2Y);
            var line3 = CreateDiscreteLine(controlPoint2X, controlPoint2Y, nextPositionX, nextPositionY);
            var quadraticBezier1 = ComputeBezierStep(line1.Item1, line1.Item2, line2.Item1, line2.Item2);
            var quadraticBezier2 = ComputeBezierStep(line2.Item1, line2.Item2, line3.Item1, line3.Item2);
            var values = ComputeBezierStep(quadraticBezier1.Item1, quadraticBezier1.Item2, quadraticBezier2.Item1, quadraticBezier2.Item2);
            List<Point> result = new List<Point>();
            for (int j = 0; j < samplingRateBezier; j++)
            {
                result.Add(ScaleAndCreatePoint(values.Item1[j], values.Item2[j]));
            }
            return result;
        }

        /// <summary>
        /// samples a quadratic bezier curve with a static number of steps (samplingRateBezier)
        /// </summary>
        /// <param name="lastPositionX">x coordinate of last point</param>
        /// <param name="lastPositionY">y coordinate of last point</param>
        /// <param name="controlPointX">x coordinate of control point</param>
        /// <param name="controlPointY">y coordinate of control point</param>
        /// <param name="nextPositionX">x coordinate of next point</param>
        /// <param name="nextPositionY">y coordinate of next point</param>
        /// <returns>a List of Points containing all sampled points</returns>
        private List<Point> SampleQuadraticBezier(double lastPositionX, double lastPositionY, double controlPointX, double controlPointY, double nextPositionX, double nextPositionY)
        {
            var line1 = CreateDiscreteLine(lastPositionX, lastPositionY, controlPointX, controlPointY);
            var line2 = CreateDiscreteLine(controlPointX, controlPointY, nextPositionX, nextPositionY);
            var values = ComputeBezierStep(line1.Item1, line1.Item2, line2.Item1, line2.Item2);
            List<Point> result = new List<Point>();
            for (int j = 0; j < samplingRateBezier; j++)
            {
                result.Add(ScaleAndCreatePoint(values.Item1[j], values.Item2[j]));
            }
            return result;
        }

        /// <summary>
        /// create a discrete line with [samplingRateBezier] points (including start and end point) between two points
        /// </summary>
        /// <param name="point1X">coordinate of point 1</param>
        /// <param name="point1Y">y coordinate of point 1</param>
        /// <param name="point2X">x coordinate of point 2</param>
        /// <param name="point2Y">y coordinate of point 2</param>
        /// <returns>the discrete line as arrays of x and y coordinates</returns>
        private Tuple<double[], double[]> CreateDiscreteLine(double point1X, double point1Y, double point2X, double point2Y)
        {
            double[] resultX = new double[samplingRateBezier];
            double[] resultY = new double[samplingRateBezier];
            for (int j = 0; j < samplingRateBezier; j++)
            {
                var pointResult = LinearInterpolationForBezier(point1X, point1Y, point2X, point2Y, j);
                resultX[j] = pointResult.Item1;
                resultY[j] = pointResult.Item2;
            }
            return new Tuple<double[], double[]>(resultX, resultY);
        }

        /// <summary>
        /// computes the discrete bezier curve between two given dicrete lines/curves
        /// </summary>
        /// <param name="line1X">x coordinates of all points in line 1</param>
        /// <param name="line1Y">y coordinates of all points in line 1</param>
        /// <param name="line2X">x coordinates of all points in line 2</param>
        /// <param name="line2Y">y coordinates of all points in line 2</param>
        /// <returns>the discrete bezier curve</returns>
        private Tuple<double[], double[]> ComputeBezierStep(double[] line1X, double[] line1Y, double[] line2X, double[] line2Y)
        {
            double[] resultX = new double[samplingRateBezier];
            double[] resultY = new double[samplingRateBezier];
            for (int j = 0; j < samplingRateBezier; j++)
            {
                var pointResult = LinearInterpolationForBezier(line1X[j], line1Y[j], line2X[j], line2Y[j], j);
                resultX[j] = pointResult.Item1;
                resultY[j] = pointResult.Item2;
            }
            return new Tuple<double[], double[]>(resultX, resultY);
        }

        /// <summary>
        /// creates the linearly interpolated point at j/(samplingRateBezier - 1) between point 1 and point 2
        /// </summary>
        /// <param name="point1X">x coordinate of point 1</param>
        /// <param name="point1Y">y coordinate of point 1</param>
        /// <param name="point2X">x coordinate of point 2</param>
        /// <param name="point2Y">y coordinate of point 2</param>
        /// <param name="j">number of point to be interpolated, at a total number of [samplingRateBezier] points</param>
        /// <returns>the linearly interpolated point</returns>
        private Tuple<double, double> LinearInterpolationForBezier(double point1X, double point1Y, double point2X, double point2Y, int j)
        {
            double factor = ((double)1 / (double)(samplingRateBezier - 1)) * (double)j; //factor for linear interpolation
            double x = point1X + ((point2X - point1X) * factor);
            double y = point1Y + ((point2Y - point1Y) * factor);
            return new Tuple<double, double>(x, y);
        }

        /// <summary>
        /// parses a hierarchical svg element and all its sub-elements
        /// </summary>
        /// <param name="currentElement">the definition of the top level element as whitespace seperated String[]</param>
        /// <param name="allLines">an array holding all lines of the input file</param>
        /// <returns>the parsed element as a Line object, or null if the element is not supported</returns>
        private List<InternalLine> ParseMultiLineSVGElement(string[] currentElement, string[] allLines)
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
            double[] xValues = new double[samplingRateEllipse / 4];
            double[] yValues = new double[samplingRateEllipse / 4];
            for (int j = 0; j < samplingRateEllipse / 4; j++) //compute offset values of points for one quadrant
            {
                xValues[j] = Math.Sin((double)j * angle) * rx;
                yValues[j] = Math.Cos((double)j * angle) * rx;
            }
            for (int j = 0; j < samplingRateEllipse / 4; j++) //create actual points for first quadrant
            {
                int xCoord = Convert.ToInt32(Math.Round(x + xValues[j]));
                int yCoord = Convert.ToInt32(Math.Round(y - yValues[j] * yScale));
                ellipse.Add(ScaleAndCreatePoint(xCoord, yCoord));
            }
            for (int j = 0; j < samplingRateEllipse / 4; j++) //create actual points for second quadrant
            {
                int xCoord = Convert.ToInt32(Math.Round(x + yValues[j]));
                int yCoord = Convert.ToInt32(Math.Round(y + xValues[j] * yScale));
                ellipse.Add(ScaleAndCreatePoint(xCoord, yCoord));
            }
            for (int j = 0; j < samplingRateEllipse / 4; j++) //create actual points for third quadrant
            {
                int xCoord = Convert.ToInt32(Math.Round(x - xValues[j]));
                int yCoord = Convert.ToInt32(Math.Round(y + yValues[j] * yScale));
                ellipse.Add(ScaleAndCreatePoint(xCoord, yCoord));
            }
            for (int j = 0; j < samplingRateEllipse / 4; j++) //create actual points for fourth quadrant
            {
                int xCoord = Convert.ToInt32(Math.Round(x - yValues[j]));
                int yCoord = Convert.ToInt32(Math.Round(y - xValues[j] * yScale));
                ellipse.Add(ScaleAndCreatePoint(xCoord, yCoord));
            }
            ellipse.Add(ScaleAndCreatePoint(Convert.ToInt32(Math.Round(x + 0)), Convert.ToInt32(Math.Round(y - rx * yScale)))); //close ellipse
            return ellipse;
        }
    }
}
