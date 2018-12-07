using NUnit.Framework;
using System.Drawing;
using System;
using System.Collections.Generic;
using SketchAssistant;

namespace Tests
{
    class LineTests
    {
        //========================//
        //= Bresenham Line Tests =//
        //========================//

        [Test]
        public void BresenhamLineTest1()
        {
            //Test point
            List<Point> expectedResult = new List<Point>();
            expectedResult.Add(new Point(1, 2));
            List<Point> actualResult = SketchAssistant.Line.BresenhamLineAlgorithm(new Point(1, 2), new Point(1, 2));
            Assert.AreEqual(1, actualResult.Count);
            for (int i = 0; i < actualResult.Count; i++)
            {
                Assert.AreEqual(expectedResult[i], actualResult[i]);
            }
        }

        [Test]
        public void BresenhamLineTest2()
        {
            //Test line going from left to right
            List<Point> expectedResult = new List<Point>();
            for (int i = 1; i <= 6; i++) { expectedResult.Add(new Point(i, 2)); }
            List<Point> actualResult = SketchAssistant.Line.BresenhamLineAlgorithm(new Point(1, 2), new Point(6, 2));
            Assert.AreEqual(expectedResult.Count, actualResult.Count);
            for (int i = 0; i < actualResult.Count; i++)
            {
                Assert.AreEqual(expectedResult[i], actualResult[i]);
            }
        }

        [Test]
        public void BresenhamLineTest3()
        {
            //Test line going from right to left
            List<Point> expectedResult = new List<Point>();
            for (int i = 6; i >= 1; i--) { expectedResult.Add(new Point(i, 2)); }
            List<Point> actualResult = SketchAssistant.Line.BresenhamLineAlgorithm(new Point(6, 2), new Point(1, 2));
            Assert.AreEqual(expectedResult.Count, actualResult.Count);
            for (int i = 0; i < actualResult.Count; i++)
            {
                Assert.AreEqual(expectedResult[i], actualResult[i]);
            }
        }

        [Test]
        public void BresenhamLineTest4()
        {
            //Test line going from top to bottom
            List<Point> expectedResult = new List<Point>();
            for (int i = 5; i <= 25; i++) { expectedResult.Add(new Point(7, i)); }
            List<Point> actualResult = SketchAssistant.Line.BresenhamLineAlgorithm(new Point(7, 5), new Point(7, 25));
            Assert.AreEqual(expectedResult.Count, actualResult.Count);
            for (int i = 0; i < actualResult.Count; i++)
            {
                Assert.AreEqual(expectedResult[i], actualResult[i]);
            }
        }

        [Test]
        public void BresenhamLineTest5()
        {
            //Test line going from bottom to top
            List<Point> expectedResult = new List<Point>();
            for (int i = 25; i >= 5; i--) { expectedResult.Add(new Point(7, i)); }
            List<Point> actualResult = SketchAssistant.Line.BresenhamLineAlgorithm(new Point(7, 25), new Point(7, 5));
            Assert.AreEqual(expectedResult.Count, actualResult.Count);
            for (int i = 0; i < actualResult.Count; i++)
            {
                Assert.AreEqual(expectedResult[i], actualResult[i]);
            }
        }

        [Test]
        public void BresenhamLineTest6()
        {
            //Test exactly diagonal line from top left to bottom right
            List<Point> expectedResult = new List<Point>();
            for (int i = 5; i <= 25; i++) { expectedResult.Add(new Point(i + 2, i)); }
            List<Point> actualResult = SketchAssistant.Line.BresenhamLineAlgorithm(new Point(7, 5), new Point(27, 25));
            Assert.AreEqual(expectedResult.Count, actualResult.Count);
            for (int i = 0; i < actualResult.Count; i++)
            {
                Assert.AreEqual(expectedResult[i], actualResult[i]);
            }
        }

        [Test]
        public void BresenhamLineTest7()
        {
            //Test exactly diagonal line from bottom right to top left
            List<Point> expectedResult = new List<Point>();
            for (int i = 25; i >= 5; i--) { expectedResult.Add(new Point(i + 2, i)); }
            List<Point> actualResult = SketchAssistant.Line.BresenhamLineAlgorithm(new Point(27, 25), new Point(7, 5));
            Assert.AreEqual(expectedResult.Count, actualResult.Count);
            for (int i = 0; i < actualResult.Count; i++)
            {
                Assert.AreEqual(expectedResult[i], actualResult[i]);
            }
        }

        //===========================//
        //= Matrix Population Tests =//
        //===========================//

        [Test]
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

        [Test]
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

        [Test]
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

    class ActionHistoryTests
    {
        [TestCase(SketchAction.ActionType.Start, 5, -1, "A new canvas was created.")]
        [TestCase(SketchAction.ActionType.Draw, 5, 5, "Line number 5 was drawn.")]
        [TestCase(SketchAction.ActionType.Delete, 10, 10, "Line number 10 was deleted.")]
        public void ScetchActionTest1(SketchAction.ActionType type, int id, int exit, String response)
        {
            HashSet<int> actualResult = new HashSet<int>();
            if(!type.Equals(SketchAction.ActionType.Start)) { actualResult.Add(id); }
            SketchAction testAction = new SketchAction(type, id);
            Assert.AreEqual(type, testAction.GetActionType());
            Assert.AreEqual(true, actualResult.SetEquals(testAction.GetLineIDs()));
            Assert.AreEqual(response, testAction.GetActionInformation());
        }

        [TestCase(SketchAction.ActionType.Start, 1, 2, 3, "A new canvas was created.")]
        [TestCase(SketchAction.ActionType.Draw, 3, 3, 3, "Line number 3 was drawn.")]
        [TestCase(SketchAction.ActionType.Delete, 20, 30, 40, "Several Lines were deleted.")]
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

        [TestCase(SketchAction.ActionType.Start, SketchAction.ActionType.Start, true)]
        [TestCase(SketchAction.ActionType.Draw, SketchAction.ActionType.Delete, false)]
        public void ActionHistoryTest1(SketchAction.ActionType action1, SketchAction.ActionType action2, bool isEmpty)
        {
            ActionHistory testHistory = new ActionHistory();
            if(action1 != SketchAction.ActionType.Start) { testHistory.AddNewAction(new SketchAction(action1, 5)); }
            if(action2 != SketchAction.ActionType.Start) { testHistory.AddNewAction(new SketchAction(action2, 5)); }
            Assert.AreEqual(isEmpty, testHistory.IsEmpty());
        }
    }
}