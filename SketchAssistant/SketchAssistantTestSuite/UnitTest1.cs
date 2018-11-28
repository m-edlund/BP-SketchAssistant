using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using SketchAssistant;

namespace SketchAssistantTestSuite
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
            List<Point> actualResult = SketchAssistant.Line.BresenhamLineAlgorithm(new Point(1, 2), new Point(1, 2));
            Assert.AreEqual(1, actualResult.Count);
            for(int i = 0; i < actualResult.Count; i++)
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
            List<Point> actualResult = SketchAssistant.Line.BresenhamLineAlgorithm(new Point(1, 2), new Point(6, 2));
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
            List<Point> actualResult = SketchAssistant.Line.BresenhamLineAlgorithm(new Point(6, 2), new Point(1, 2));
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
            List<Point> actualResult = SketchAssistant.Line.BresenhamLineAlgorithm(new Point(7, 5), new Point(7, 25));
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
            List<Point> actualResult = SketchAssistant.Line.BresenhamLineAlgorithm(new Point(7, 25), new Point(7, 5));
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
            List<Point> actualResult = SketchAssistant.Line.BresenhamLineAlgorithm(new Point(7, 5), new Point(27, 25));
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
            List<int>[,] resultLineMatrix = new List<int>[5, 5];
            Line testLine = new Line(thePoints);
            testLine.PopulateMatrixes(resultBoolMatrix, resultLineMatrix);
            for(int i = 0; i < 5; i++)
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
            List<int>[,] testLineMatrix = new List<int>[5, 5];
            for (int i = 1; i <= 3; i++)
            {
                testBoolMatrix[i, i] = true;
                List<int> temp = new List<int>();
                temp.Add(5);
                testLineMatrix[i, i] = temp;
            }
            bool[,] resultBoolMatrix = new bool[5, 5];
            List<int>[,] resultLineMatrix = new List<int>[5, 5];
            Line testLine = new Line(thePoints, 5);
            testLine.PopulateMatrixes(resultBoolMatrix, resultLineMatrix);
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    Assert.AreEqual(testBoolMatrix[i, j], resultBoolMatrix[i, j]);
                    if(testLineMatrix[i, j] != null && resultLineMatrix[i, j] != null)
                    {
                        for (int k = 0; k < resultLineMatrix[i, j].Count; k++)
                        {
                            Assert.AreEqual(testLineMatrix[i, j][k], resultLineMatrix[i, j][k]);
                        }
                    }
                }
            }
        }
    }
}
