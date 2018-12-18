using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using System.Collections.Generic;
using SketchAssistant;
using System.Windows.Forms;

namespace Tests
{
    [TestClass]
    public class LineTests
    {
        //========================//
        //= Bresenham Line Tests =//
        //========================//

        [TestMethod]
        public void BresenhamLineTest1()
        {
            //Test point
            List<Point> expectedResult = new List<Point>();
            expectedResult.Add(new Point(1, 2));
            List<Point> actualResult = SketchAssistant.GeometryCalculator.BresenhamLineAlgorithm(new Point(1, 2), new Point(1, 2));
            Assert.AreEqual(1, actualResult.Count);
            for (int i = 0; i < actualResult.Count; i++)
            {
                Assert.AreEqual(expectedResult[i], actualResult[i]);
            }
        }

        [TestMethod]
        public void BresenhamLineTest2()
        {
            //Test line going from left to right
            List<Point> expectedResult = new List<Point>();
            for (int i = 1; i <= 6; i++) { expectedResult.Add(new Point(i, 2)); }
            List<Point> actualResult = SketchAssistant.GeometryCalculator.BresenhamLineAlgorithm(new Point(1, 2), new Point(6, 2));
            Assert.AreEqual(expectedResult.Count, actualResult.Count);
            for (int i = 0; i < actualResult.Count; i++)
            {
                Assert.AreEqual(expectedResult[i], actualResult[i]);
            }
        }

        [TestMethod]
        public void BresenhamLineTest3()
        {
            //Test line going from right to left
            List<Point> expectedResult = new List<Point>();
            for (int i = 6; i >= 1; i--) { expectedResult.Add(new Point(i, 2)); }
            List<Point> actualResult = SketchAssistant.GeometryCalculator.BresenhamLineAlgorithm(new Point(6, 2), new Point(1, 2));
            Assert.AreEqual(expectedResult.Count, actualResult.Count);
            for (int i = 0; i < actualResult.Count; i++)
            {
                Assert.AreEqual(expectedResult[i], actualResult[i]);
            }
        }

        [TestMethod]
        public void BresenhamLineTest4()
        {
            //Test line going from top to bottom
            List<Point> expectedResult = new List<Point>();
            for (int i = 5; i <= 25; i++) { expectedResult.Add(new Point(7, i)); }
            List<Point> actualResult = SketchAssistant.GeometryCalculator.BresenhamLineAlgorithm(new Point(7, 5), new Point(7, 25));
            Assert.AreEqual(expectedResult.Count, actualResult.Count);
            for (int i = 0; i < actualResult.Count; i++)
            {
                Assert.AreEqual(expectedResult[i], actualResult[i]);
            }
        }

        [TestMethod]
        public void BresenhamLineTest5()
        {
            //Test line going from bottom to top
            List<Point> expectedResult = new List<Point>();
            for (int i = 25; i >= 5; i--) { expectedResult.Add(new Point(7, i)); }
            List<Point> actualResult = SketchAssistant.GeometryCalculator.BresenhamLineAlgorithm(new Point(7, 25), new Point(7, 5));
            Assert.AreEqual(expectedResult.Count, actualResult.Count);
            for (int i = 0; i < actualResult.Count; i++)
            {
                Assert.AreEqual(expectedResult[i], actualResult[i]);
            }
        }

        [TestMethod]
        public void BresenhamLineTest6()
        {
            //Test exactly diagonal line from top left to bottom right
            List<Point> expectedResult = new List<Point>();
            for (int i = 5; i <= 25; i++) { expectedResult.Add(new Point(i + 2, i)); }
            List<Point> actualResult = SketchAssistant.GeometryCalculator.BresenhamLineAlgorithm(new Point(7, 5), new Point(27, 25));
            Assert.AreEqual(expectedResult.Count, actualResult.Count);
            for (int i = 0; i < actualResult.Count; i++)
            {
                Assert.AreEqual(expectedResult[i], actualResult[i]);
            }
        }

        [TestMethod]
        public void BresenhamLineTest7()
        {
            //Test exactly diagonal line from bottom right to top left
            List<Point> expectedResult = new List<Point>();
            for (int i = 25; i >= 5; i--) { expectedResult.Add(new Point(i + 2, i)); }
            List<Point> actualResult = SketchAssistant.GeometryCalculator.BresenhamLineAlgorithm(new Point(27, 25), new Point(7, 5));
            Assert.AreEqual(expectedResult.Count, actualResult.Count);
            for (int i = 0; i < actualResult.Count; i++)
            {
                Assert.AreEqual(expectedResult[i], actualResult[i]);
            }
        }

        //===========================//
        //= Matrix Population Tests =//
        //===========================//

        [TestMethod]
        public void MatrixTest1()
        {
            //Populate Matrix for temporary Line
            List<Point> thePoints = new List<Point>();
            thePoints.Add(new Point(1, 1));
            thePoints.Add(new Point(1, 2));
            bool[,] testBoolMatrix = new bool[5, 5];
            List<int>[,] testLineMatrix = new List<int>[5, 5];
            bool[,] resultBoolMatrix = new bool[5, 5];
            HashSet<int>[,] resultLineMatrix = new HashSet<int>[5, 5];
            Line testLine = new Line(thePoints);
            testLine.PopulateMatrixes(resultBoolMatrix, resultLineMatrix);
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    Assert.AreEqual(testBoolMatrix[i, j], resultBoolMatrix[i, j]);
                    Assert.AreEqual(testLineMatrix[i, j], resultLineMatrix[i, j]);
                }
            }
        }

        [TestMethod]
        public void MatrixTest2()
        {
            //Populate Matrix for non-temporary Line
            List<Point> thePoints = new List<Point>();
            thePoints.Add(new Point(1, 1));
            thePoints.Add(new Point(3, 3));
            bool[,] testBoolMatrix = new bool[5, 5];
            HashSet<int>[,] testLineMatrix = new HashSet<int>[5, 5];
            for (int i = 1; i <= 3; i++)
            {
                testBoolMatrix[i, i] = true;
                HashSet<int> temp = new HashSet<int>();
                temp.Add(5);
                testLineMatrix[i, i] = temp;
            }
            bool[,] resultBoolMatrix = new bool[5, 5];
            HashSet<int>[,] resultLineMatrix = new HashSet<int>[5, 5];
            Line testLine = new Line(thePoints, 5);
            testLine.PopulateMatrixes(resultBoolMatrix, resultLineMatrix);
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    Assert.AreEqual(testBoolMatrix[i, j], resultBoolMatrix[i, j]);
                    if (testLineMatrix[i, j] != null && resultLineMatrix[i, j] != null)
                    {
                        for (int k = 0; k < resultLineMatrix[i, j].Count; k++)
                        {
                            Assert.AreEqual(true, testLineMatrix[i, j].SetEquals(resultLineMatrix[i, j]));
                        }
                    }
                }
            }
        }

        //=========================//
        //= Line Constructor Test =//
        //=========================//

        [TestMethod]
        public void ConstructorTest()
        {
            //Create non-temporary Line and check points
            //reference Points
            List<Point> comparisonPoints = new List<Point> {new Point(2,2), new Point(3, 1), new Point(4, 1), new Point(5, 1), new Point(6, 2),
                new Point(7, 3), new Point(8, 4), new Point(9, 5), new Point(10, 6), new Point(11, 5), new Point(11, 4), new Point(11, 3),
                new Point(10, 2), new Point(9, 1), new Point(8, 2), new Point(7, 3), new Point(6, 4), new Point(5, 5), new Point(4, 5),
                new Point(3, 5), new Point(2, 5), new Point(1, 4)};
            //test Points, with intermediate points missing & duplicate points
            List<Point> testPoints = new List<Point> {new Point(2,2), new Point(3, 1), new Point(5, 1), new Point(5, 1), new Point(5, 1),
                new Point(8, 4), new Point(10, 6), new Point(11, 5), new Point(11, 3), new Point(9, 1), new Point(9, 1), new Point(9, 1),
                new Point(5, 5), new Point(2, 5), new Point(2, 5), new Point(1, 4) };
            Line testLine = new Line(testPoints, 0);
            List<Point> returnedPoints = testLine.GetPoints();
            Assert.AreEqual(comparisonPoints.Count, returnedPoints.Count);
            for (int i = 0; i < returnedPoints.Count; i++)
            {
                Assert.AreEqual(comparisonPoints[i], returnedPoints[i]);
            }
        }
    }

    [TestClass]
    public class ActionHistoryTests
    {
        ToolStripStatusLabel testLabel = new ToolStripStatusLabel();

        private ActionHistory GetActionHistory()
        {
            return new ActionHistory(testLabel);
        }

        [DataTestMethod]
        [DataRow(SketchAction.ActionType.Start, 5, -1, "A new canvas was created.")]
        [DataRow(SketchAction.ActionType.Draw, 5, 5, "Line number 5 was drawn.")]
        [DataRow(SketchAction.ActionType.Delete, 10, 10, "Line number 10 was deleted.")]
        public void ScetchActionTest1(SketchAction.ActionType type, int id, int exit, String response)
        {
            HashSet<int> actualResult = new HashSet<int>();
            if (!type.Equals(SketchAction.ActionType.Start)) { actualResult.Add(id); }
            SketchAction testAction = new SketchAction(type, id);
            Assert.AreEqual(type, testAction.GetActionType());
            Assert.AreEqual(true, actualResult.SetEquals(testAction.GetLineIDs()));
            Assert.AreEqual(response, testAction.GetActionInformation());
        }

        [DataTestMethod]
        [DataRow(SketchAction.ActionType.Start, 1, 2, 3, "A new canvas was created.")]
        [DataRow(SketchAction.ActionType.Draw, 3, 3, 3, "Line number 3 was drawn.")]
        [DataRow(SketchAction.ActionType.Delete, 20, 30, 40, "Several Lines were deleted.")]
        public void ScetchActionTest2(SketchAction.ActionType type, int id1, int id2, int id3, String response)
        {
            HashSet<int> actualResult = new HashSet<int>();
            if (!type.Equals(SketchAction.ActionType.Start))
            {
                actualResult.Add(id1);
                actualResult.Add(id2);
                actualResult.Add(id3);
            }
            SketchAction testAction = new SketchAction(type, actualResult);
            Assert.AreEqual(type, testAction.GetActionType());
            Assert.AreEqual(true, actualResult.SetEquals(testAction.GetLineIDs()));
            Assert.AreEqual(response, testAction.GetActionInformation());
        }

        [DataTestMethod]
        [DataRow(SketchAction.ActionType.Start, SketchAction.ActionType.Start, true)]
        [DataRow(SketchAction.ActionType.Draw, SketchAction.ActionType.Delete, false)]
        public void ActionHistoryTest1(SketchAction.ActionType action1, SketchAction.ActionType action2, bool isEmpty)
        {
            ActionHistory testHistory = GetActionHistory();
            if (!action1.Equals(SketchAction.ActionType.Start)) { testHistory.AddNewAction(new SketchAction(action1, 5)); }
            if (!action2.Equals(SketchAction.ActionType.Start)) { testHistory.AddNewAction(new SketchAction(action2, 5)); }
            Assert.AreEqual(isEmpty, testHistory.IsEmpty());
        }

        [DataTestMethod]
        [DataRow(SketchAction.ActionType.Draw, "Last Action: Line number 0 was drawn.")]
        [DataRow(SketchAction.ActionType.Delete, "Last Action: Line number 0 was deleted.")]
        public void ActionHistoryUndoRedoTest(SketchAction.ActionType actionType, String message)
        {
            ActionHistory testHistory = GetActionHistory();
            SketchAction testAction = new SketchAction(actionType, 0);
            testHistory.AddNewAction(testAction);
            Assert.AreEqual(true, testHistory.CanUndo());
            testHistory.MoveAction(true);
            Assert.AreEqual(true, testHistory.CanRedo());
            testHistory.MoveAction(false);
            Assert.AreEqual(actionType, testHistory.GetCurrentAction().GetActionType());
            String currLabel = testLabel.Text;
            Assert.AreEqual(currLabel, message);
        }
    }

    [TestClass]
    public class FileImporterTests
    {
        [DataTestMethod]
        [DataRow(new int[] { 54, 43, 57, 11, 145, 34, 113, 299, 0 }, new int[] { 33, 42, 140, 30, 30, 30, 32, 145, 2 })]
        [DataRow(new int[] { 33, 42, 140, 30, 30, 30, 32, 145, 2 }, new int[] { 33, 42, 140, 30, 30, 30, 32, 145, 2 })]
        [DataRow(new int[] { 33, 42, 140, 30, 30, 30, 32, 145, 2 }, new int[] { 54, 43, 57, 11, 145, 34, 113, 199, 0 })]
        public void ParseISADInputSuccessfulTest(int[] xCoordinates, int[] yCoordinates)
        {
            Form1 program = new Form1();
            FileImporter uut = new SketchAssistant.FileImporter(program);

            List<String> file = new List<string>();
            file.Add("drawing");
            file.Add("300x200");
            for (int i = 0; i < xCoordinates.Length - 2; i += 3)
            {
                file.Add("line");
                file.Add(xCoordinates[i] + ";" + yCoordinates[i]);
                file.Add(xCoordinates[i + 1] + ";" + yCoordinates[i + 1]);
                file.Add(xCoordinates[i + 2] + ";" + yCoordinates[i + 2]);
                file.Add("endline");
            }
            file.Add("enddrawing");

            (int, int, List<Line>) values = uut.ParseISADInputForTesting(file.ToArray());
            program.CreateCanvasAndSetPictureForTesting(values.Item1, values.Item2, values.Item3);

            Line[] drawing = GetLeftImage(program).ToArray();

            Assert.AreEqual(xCoordinates.Length / 3, drawing.Length);
            for (int i = 0; i < xCoordinates.Length - 2; i += 3)
            {
                Point[] currentLine = drawing[i / 3].GetPoints().ToArray();
                Assert.AreEqual(3, currentLine.Length);
                for (int j = 0; j < 3; j++)
                {
                    Assert.IsTrue(currentLine[j].X == xCoordinates[i + j] && currentLine[j].Y == yCoordinates[i + j]);
                }
            }
        }

        [DataTestMethod]
        [DataRow(new String[] {})]
        [DataRow(new String[] { "begindrawing", "300x300", "line", "50;50", "100;50", "endline", "enddrawing" })]
        [DataRow(new String[] { "drawing", "300;300", "line", "50;50", "100;50", "endline", "enddrawing" })]
        [DataRow(new String[] { "drawing", "30.5x300", "line", "50;50", "100;50", "endline", "enddrawing" })]
        [DataRow(new String[] { "drawing", "line", "50;50", "100;50", "endline", "enddrawing" })]
        [DataRow(new String[] { "drawing", "300x300", "beginline", "50;50", "100;50", "endline", "enddrawing" })]
        [DataRow(new String[] { "drawing", "300x300", "line", "500;50", "100;50", "endline", "enddrawing" })]
        [DataRow(new String[] { "drawing", "300x300", "line", "50x50", "100;50", "endline", "enddrawing" })]
        [DataRow(new String[] { "drawing", "300x300", "line", "50", "100", "endline", "enddrawing" })]
        [DataRow(new String[] { "drawing", "300x300", "line", "50;50", "line", "endline", "enddrawing" })]
        [DataRow(new String[] { "drawing", "300x300", "line", "50;50", "100;50", "stopline", "enddrawing" })]
        [DataRow(new String[] { "drawing", "300x300", "line", "50;50", "100;50", "enddrawing" })]
        [DataRow(new String[] { "drawing", "300x300", "line", "50;50", "100;50", "endline", "endrawing" })]
        [DataRow(new String[] { "drawing", "300x300", "line", "50;50", "100;50", "endline" })]
        public void ParseISADInputExceptionTest(String[] file)
        {
            bool exceptionThrown = false;
            Form1 program = new Form1();
            FileImporter uut = new SketchAssistant.FileImporter(program);
            //check that left image initially is uninitialized
            Assert.IsNull(GetLeftImage(program));
            //initialize left image with a valid isad drawing
            (int, int, List<Line>) values = uut.ParseISADInputForTesting(new string[] { "drawing", "300x205", "line", "40;40", "140;140", "endline", "enddrawing" });
            program.CreateCanvasAndSetPictureForTesting(values.Item1, values.Item2, values.Item3);
            //save left image for later comparison
            List<Line> oldLeftImage = GetLeftImage(program);
            try
            {
                //try to initialize the left image with an invalid isad drawing
                (int, int, List<Line>) values1 = uut.ParseISADInputForTesting(file);
                program.CreateCanvasAndSetPictureForTesting(values1.Item1, values1.Item2, values1.Item3);
            }
            catch(FileImporterException)
            {
                //save the occurence of an exception
                exceptionThrown = true;
            }
            //check that an exception has been thrown
            Assert.IsTrue(exceptionThrown);
            //check that the left image has not been changed by the failed image import
            Assert.AreEqual(oldLeftImage, GetLeftImage(program));
        }

        /// <summary>
        /// local helper method retrieving the left image from a Form1 instance
        /// </summary>
        /// <returns>the left image of the given Form1 instance</returns>
        private List<Line> GetLeftImage(Form1 program)
        {
            //cast is save as long as Form1#GetAllVariables() is conform to its contract
            return (List<Line>)program.GetAllVariables().Find(x => x.Item1.Equals("leftLineList")).Item2;
        }
    }

    [TestClass]
    public class RedrawAssistantTests
    {
        private RedrawAssistant GetAssistant(List<Line> input)
        {
            if(input.Count == 0)
            {
                return new RedrawAssistant();
            }
            else
            {
                return new RedrawAssistant(input);
            }
        }

        [DataTestMethod]
        [DataRow(17, 20, new int[] { 54, 43, 57, 11, 145, 34, 113, 299, 0 }, new int[] { 33, 42, 140, 30, 30, 30, 32, 145, 2 }, true)]
        [DataRow(-50, 30, new int[] { 33, 42, 140, 30, 30, 30, 32, 145, 2 }, new int[] { 33, 42, 140, 30, 30, 30, 32, 145, 2 }, false)]
        [DataRow(-70, -20, new int[] { 33, 42, 140, 30, 30, 30, 32, 145, 2 }, new int[] { 54, 43, 57, 11, 145, 34, 113, 199, 0 }, true)]
        public void InactiveRedrawAssistantTest(int x, int y, int[] xCoords, int[] yCoords, bool lineActive)
        {
            RedrawAssistant testAssistant = GetAssistant(new List<Line>());
            List<Point> testPoints = new List<Point>();
            for (int i = 0; i < xCoords.Length; i++)
            {
                testPoints.Add(new Point(xCoords[i], yCoords[i]));
            }
            List<Tuple<bool, Line>> testLines = new List<Tuple<bool, Line>> { new Tuple<bool, Line>(lineActive, new Line(testPoints, 0)) };
            List<HashSet<Point>> result = testAssistant.Tick(new Point(1, 1), testLines, -1, false);
            Assert.AreEqual(0, result.Count);
        }

        [DataTestMethod]
        [DataRow(17, 20, new int[] { 54, 43, 57, 11, 145, 34, 113, 299, 0 }, new int[] { 33, 42, 140, 30, 30, 30, 32, 145, 2 }, 1)]
        [DataRow(-50, 30, new int[] { 33, 42, 140, 30, 30, 30, 32, 145, 2 }, new int[] { 33, 42, 140, 30, 30, 30, 32, 145, 2 }, 1)]
        [DataRow(33, 54, new int[] { 33, 42, 140, 30, 30, 30, 32, 145, 2 }, new int[] { 54, 43, 57, 11, 145, 34, 113, 199, 0 }, 2)]
        public void ActiveRedrawAssistantTestStartedDrawing(int x, int y, int[] xCoords, int[] yCoords, int resultingCount)
        {
            List<Point> testPoints = new List<Point>();
            for (int i = 0; i < xCoords.Length; i++)
            {
                testPoints.Add(new Point(xCoords[i], yCoords[i]));
            }
            List<Tuple<bool, Line>> testInputLines = new List<Tuple<bool, Line>>();
            List<Line> testRedrawLines = new List<Line> { new Line(testPoints, 0) };
            RedrawAssistant testAssistant = GetAssistant(testRedrawLines);
            //Setting Marker Radius to 1 so that only the functionality of the RedrawAssistant is checked 
            //and not the functionality of the Circle algorithm.
            testAssistant.SetMarkerRadius(1);
            List<HashSet<Point>> tickResult = testAssistant.Tick(new Point(x, y), testInputLines, -1, false);


            Assert.AreEqual(resultingCount, tickResult.Count);
            if(resultingCount == 1)
            {
                foreach(Point p in tickResult[0])
                {
                    Assert.AreEqual(testPoints[0], p);
                }
            }
            if(resultingCount == 2)
            {
                foreach (Point p in tickResult[0])
                {
                    Assert.AreEqual(testPoints[0], p);
                }
                foreach (Point p in tickResult[1])
                {
                    Assert.AreEqual(testPoints[testPoints.Count-1], p);
                }
            }
        }

        [DataTestMethod]
        [DataRow(17, 20, new int[] { 54, 43, 57, 11, 145, 34, 113, 299, 0 }, new int[] { 33, 42, 140, 30, 30, 30, 32, 145, 2 },
             new int[] { 77, 20, 3, 74, 28 }, new int[] { 40, 50, 20, 77, 28}, 2, false)]
        [DataRow(33, 33, new int[] { 33, 42, 140, 30, 30, 30, 32, 145, 2 }, new int[] { 33, 42, 140, 30, 30, 30, 32, 145, 2 },
             new int[] { 42, 140, 30, 30, 30 }, new int[] { 11, 145, 34, 113, 28 }, 2, true)]
        [DataRow(33, 54, new int[] { 33, 42, 140, 30, 30, 30, 32, 145, 2 }, new int[] { 54, 43, 57, 11, 145, 34, 113, 199, 0 },
             new int[] { 43, 57, 11, 145 }, new int[] { 33, 42, 140, 30 }, 2, true)]
        public void ActiveRedrawAssistantTestMultipleLinesRedrawn(
            int x, int y, int[] xCoords_one, int[] yCoords_one, int[] xCoords_two, int[] yCoords_two, int resultingCount, bool showingStartAndEnd)
        {
            List<Point> testPoints1 = new List<Point>();
            for (int i = 0; i < xCoords_one.Length; i++)
            {
                testPoints1.Add(new Point(xCoords_one[i], yCoords_one[i]));
            }
            List<Point> testPoints2 = new List<Point>();
            for (int i = 0; i < xCoords_two.Length; i++)
            {
                testPoints2.Add(new Point(xCoords_two[i], yCoords_two[i]));
            }
            List<Tuple<bool, Line>> testInputLines = new List<Tuple<bool, Line>>();

            List<Line> testRedrawLines = new List<Line> { new Line(testPoints1, 0) , new Line(testPoints2, 1) };
            RedrawAssistant testAssistant = GetAssistant(testRedrawLines);
            //Setting Marker Radius to 1 so that only the functionality of the RedrawAssistant is checked 
            //and not the functionality of the Circle algorithm.
            testAssistant.SetMarkerRadius(1);

            List<HashSet<Point>> tickResult = testAssistant.Tick(new Point(x, y), testInputLines, -1, false);

            Assert.AreEqual(resultingCount, tickResult.Count);
            if (showingStartAndEnd)
            {
                foreach (Point p in tickResult[0])
                {
                    Assert.AreEqual(testPoints1[0], p);
                }
                foreach (Point p in tickResult[1])
                {
                    Assert.AreEqual(testPoints1[testPoints1.Count - 1], p);
                }
            }
            else
            {
                foreach (Point p in tickResult[0])
                {
                    Assert.AreEqual(testPoints1[0], p);
                }
                foreach (Point p in tickResult[1])
                {
                    Assert.AreEqual(testPoints2[0], p);
                }
            }
        }

        [DataTestMethod]
        [DataRow(17, 20, 17, 20, -1 ,false, new int[] { 54, 43, 57, 11, 145, 34, 113, 299, 0 }, new int[] { 33, 42, 140, 30, 30, 30, 32, 145, 2 },
     new int[] { 77, 20, 3, 74, 28 }, new int[] { 40, 50, 20, 77, 28 }, 2, 2, false, false)]

        [DataRow(33, 33, 2, 2, 0, true, new int[] { 33, 42, 140, 30, 30, 30, 32, 145, 2 }, new int[] { 33, 42, 140, 30, 30, 30, 32, 145, 2 },
     new int[] { 42, 140, 30, 30, 30 }, new int[] { 11, 145, 34, 113, 28 }, 2, 1, true, false)]

        [DataRow(33, 54, 17, 0, 0, true, new int[] { 33, 42, 140, 30, 30, 30, 32, 145, 2 }, new int[] { 54, 43, 57, 11, 145, 34, 113, 199, 0 },
     new int[] { 43, 57, 11, 145 }, new int[] { 33, 42, 140, 30 }, 2, 2, true, false)]
        public void ActiveRedrawAssistantTestLineFinished(
            int x_one, int y_one, int x_two, int y_two, int lineID, bool finishedDrawing, int[] xCoords_one, int[] yCoords_one, int[] xCoords_two, int[] yCoords_two, 
            int count_one, int count_two, bool showingSE_one, bool showingSE_two)
        {
            List<Point> testPoints1 = new List<Point>();
            for (int i = 0; i < xCoords_one.Length; i++)
            {
                testPoints1.Add(new Point(xCoords_one[i], yCoords_one[i]));
            }
            List<Point> testPoints2 = new List<Point>();
            for (int i = 0; i < xCoords_two.Length; i++)
            {
                testPoints2.Add(new Point(xCoords_two[i], yCoords_two[i]));
            }
            List<Tuple<bool, Line>> testInputLines = new List<Tuple<bool, Line>>();

            List<Line> testRedrawLines = new List<Line> { new Line(testPoints1, 0), new Line(testPoints2, 1) };
            RedrawAssistant testAssistant = GetAssistant(testRedrawLines);
            //Setting Marker Radius to 1 so that only the functionality of the RedrawAssistant is checked 
            //and not the functionality of the Circle algorithm.
            testAssistant.SetMarkerRadius(1);

            List<HashSet<Point>> tickResult1 = testAssistant.Tick(new Point(x_one, y_one), testInputLines, lineID, false);
            List<HashSet<Point>> tickResult2 = testAssistant.Tick(new Point(x_two, y_two), testInputLines, lineID, finishedDrawing);

            Assert.AreEqual(count_one, tickResult1.Count);
            if (showingSE_one)
            {
                foreach(Point p in tickResult1[0])
                {
                    Assert.AreEqual(testPoints1[0], p);
                }
                foreach (Point p in tickResult1[1])
                {
                    Assert.AreEqual(testPoints1[testPoints1.Count - 1], p);
                }
            }
            else
            {
                foreach (Point p in tickResult1[0])
                {
                    Assert.AreEqual(testPoints1[0], p);
                }
                foreach (Point p in tickResult1[1])
                {
                    Assert.AreEqual(testPoints2[0], p);
                }
            }

            Assert.AreEqual(count_two, tickResult2.Count);
            if(count_two == 2)
            {
                if (showingSE_two)
                {
                    foreach (Point p in tickResult2[0])
                    {
                        Assert.AreEqual(testPoints1[0], p);
                    }
                    foreach (Point p in tickResult2[1])
                    {
                        Assert.AreEqual(testPoints1[testPoints1.Count - 1], p);
                    }
                }
                else
                {
                    foreach (Point p in tickResult2[0])
                    {
                        Assert.AreEqual(testPoints1[0], p);
                    }
                    foreach (Point p in tickResult2[1])
                    {
                        Assert.AreEqual(testPoints2[0], p);
                    }
                }
            }
            if(count_two == 1)
            {
                foreach (Point p in tickResult2[0])
                {
                    Assert.AreEqual(testPoints2[0], p);
                }
            }
        }
    }
}