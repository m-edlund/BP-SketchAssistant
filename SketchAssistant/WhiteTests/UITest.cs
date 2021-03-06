﻿using System;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestStack.White;
using TestStack.White.UIItems;
using TestStack.White.UIItems.WindowItems;
using TestStack.White.UIItems.Finders;
using System.Threading;
using SketchAssistantWPF;
using System.Windows;
using System.Diagnostics;
using TestStack.White.UIItems.WindowStripControls;
using TestStack.White.UIItems.MenuItems;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using WindowsInput;
using WindowsInput.Native;
using System.Threading.Tasks;
using System.Linq;
using Application = TestStack.White.Application;
using Window = TestStack.White.UIItems.WindowItems.Window;

namespace WhiteTests
{

    [TestClass]
    public class UITest
    {
        private TestStack.White.Application application;

        /// <summary>
        /// The directory of the input files, saved for repeated use
        /// </summary>
        private String input_file_dir = null;
        /// <summary>
        /// instance of TestContext to be able to access deployed files
        /// </summary>
        private TestContext testContextInstance;
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        /// <summary>
        /// A function that returns the path to the test_input_files folder. 
        /// Do with it what you want.
        /// </summary>
        /// <returns>the path to the test_input_files folder</returns>
        public String getSketchAssistantDirectory()
        {
            Regex rx = new Regex(@"^(.*\\SketchAssistant\\)");
            Match match = rx.Match(TestContext.DeploymentDirectory);
            String SketchAssistDir = match.Groups[1].Value;
            if (input_file_dir == null)
            {
                if (Directory.Exists(SketchAssistDir + @"WhiteTests\test_input_files\"))
                {
                    input_file_dir = SketchAssistDir + @"WhiteTests\test_input_files\";
                }
                else if (Directory.Exists(SketchAssistDir + @"WhiteTests\bin\Debug\test_input_files\"))
                {
                    input_file_dir = SketchAssistDir + @"WhiteTests\bin\Debug\test_input_files\";
                }
                else
                {
                    Regex rx_0 = new Regex(@"^(.*\\projects\\)");
                    Match match_0 = rx_0.Match(TestContext.DeploymentDirectory);
                    String ProjectsDir = match_0.Groups[1].Value;
                    var dirs = Directory.GetDirectories(ProjectsDir, "test_input_files", SearchOption.AllDirectories);
                    input_file_dir = dirs[0];
                }
            }
            return input_file_dir;
        }

        public Window setupapp()
        {
            string[] files;
            Regex rx = new Regex(@"^(.*\\SketchAssistant\\)");
            Match match = rx.Match(TestContext.DeploymentDirectory);
            String SketchAssistDir = match.Groups[1].Value;
            try
            {
                files = Directory.GetFiles(SketchAssistDir + @"SketchAssistantWPF\bin\", "SketchAssistantWPF.exe", SearchOption.AllDirectories);
            }
            catch
            {
                Regex rx_0 = new Regex(@"^(.*\\projects\\)");
                Match match_0 = rx_0.Match(TestContext.DeploymentDirectory);
                String ProjectsDir = match_0.Groups[1].Value;
                files = Directory.GetFiles(ProjectsDir, "SketchAssistantWPF.exe", SearchOption.AllDirectories);
            }

            ProcessStartInfo processStart = new ProcessStartInfo(files[0], "-debug");
            /*
            string outputDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string editedDir = outputDir.Replace("WhiteTests", "SketchAssistantWPF");
            string app_path = editedDir + @"\SketchAssistantWPF.exe";
            ProcessStartInfo processStart = new ProcessStartInfo(app_path, "-debug");*/
            application = Application.Launch(processStart);
            return application.GetWindow("Sketch Assistant");
        }

        [DataTestMethod]
        [TestCategory("FileIO")]
        [DataRow("line")]
        public void LoadSVGFileTest(String filename)
        {
            Window mainWindow = setupapp();
            InputSimulator inputSimulator = new InputSimulator();
            Thread.Sleep(30);
            string[] files = Directory.GetFiles(getSketchAssistantDirectory() + @"\whitelisted", "*.svg", SearchOption.AllDirectories);
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("LoadMenuButton")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("SVGMenuButton")).Click();
            Thread.Sleep(1000);
            inputSimulator.Keyboard.TextEntry(getSketchAssistantDirectory() + @"whitelisted\" + filename + ".svg");
            Thread.Sleep(1000);
            inputSimulator.Keyboard.KeyPress(VirtualKeyCode.RETURN);
            Thread.Sleep(1000);
            //Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Close();
        }

        [TestMethod]
        [TestCategory("DirectInput")]
        public void DrawLineOnCanvasTest()
        {
            Window mainWindow = setupapp();
            Thread.Sleep(30);
            InputSimulator inputSimulator = new InputSimulator();
            MouseSimulator mouseSimulator = new MouseSimulator(inputSimulator);
            Assert.AreEqual("none", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("CanvasButton")).Click();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            inputSimulator.Mouse.MoveMouseBy(100, 100);
            inputSimulator.Mouse.LeftButtonDown();
            Thread.Sleep(30);
            inputSimulator.Mouse.MoveMouseBy(100, 100);
            Thread.Sleep(30);
            inputSimulator.Mouse.LeftButtonUp();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Close();
        }

        [TestMethod]
        [TestCategory("DirectInput")]
        public void UndoLineOnCanvasTest()
        {
            Window mainWindow = setupapp();
            Thread.Sleep(30);
            InputSimulator inputSimulator = new InputSimulator();
            MouseSimulator mouseSimulator = new MouseSimulator(inputSimulator);
            Assert.AreEqual("none", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("CanvasButton")).Click();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            inputSimulator.Mouse.MoveMouseBy(0, 200);
            inputSimulator.Mouse.LeftButtonDown();
            Thread.Sleep(30);
            inputSimulator.Mouse.MoveMouseBy(500, 300);
            Thread.Sleep(30);
            inputSimulator.Mouse.LeftButtonUp();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("UndoButton")).Click();
            Thread.Sleep(100);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Close();
        }

        [TestMethod]
        [TestCategory("DirectInput")]
        public void InvalidLineTest()
        {
            Window mainWindow = setupapp();
            Thread.Sleep(30);
            InputSimulator inputSimulator = new InputSimulator();
            MouseSimulator mouseSimulator = new MouseSimulator(inputSimulator);
            Assert.AreEqual("none", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("CanvasButton")).Click();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("DrawButton")).Click();
            Thread.Sleep(30);
            inputSimulator.Mouse.LeftButtonDown();
            inputSimulator.Mouse.MoveMouseBy(0, 200);
            Thread.Sleep(30);
            inputSimulator.Mouse.MoveMouseBy(500, 300);
            Thread.Sleep(30);
            inputSimulator.Mouse.LeftButtonUp();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            inputSimulator.Mouse.MoveMouseBy(-1000, 0);
            Thread.Sleep(30);
            inputSimulator.Mouse.LeftButtonDown();
            inputSimulator.Mouse.MoveMouseBy(1000, 0);
            inputSimulator.Mouse.LeftButtonUp();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Close();
        }

        [TestMethod]
        [TestCategory("DirectInput")]
        public void PointsOnCanvasSimilarityTest()
        {
            Window mainWindow = setupapp();
            Thread.Sleep(30);
            InputSimulator inputSimulator = new InputSimulator();
            MouseSimulator mouseSimulator = new MouseSimulator(inputSimulator);
            Assert.AreEqual("none", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Assert.AreEqual("-", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LineSimilarityBox")).Text.ToString());
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("CanvasButton")).Click();
            inputSimulator.Mouse.MoveMouseBy(0, 200);
            inputSimulator.Mouse.LeftButtonDown();
            Thread.Sleep(30);
            inputSimulator.Mouse.LeftButtonUp();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Assert.AreEqual("-", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LineSimilarityBox")).Text.ToString());
            Thread.Sleep(30);
            inputSimulator.Mouse.LeftButtonDown();
            Thread.Sleep(30);
            inputSimulator.Mouse.LeftButtonUp();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: Line number 1 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Assert.AreEqual("1", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LineSimilarityBox")).Text.ToString());
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("UndoButton")).Click();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Assert.AreEqual("-", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LineSimilarityBox")).Text.ToString());
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("RedoButton")).Click();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: Line number 1 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Assert.AreEqual("1", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LineSimilarityBox")).Text.ToString());
            mainWindow.Close();
        }

        [TestMethod]
        [TestCategory("DebugInput")]
        public void CreateCanvasTest()
        {
            Window mainWindow = setupapp();
            Thread.Sleep(30);
            Assert.AreEqual("none", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("CanvasButton")).Click();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Close();
        }

        [TestMethod]
        [TestCategory("DebugInput")]
        public void DrawLineTest()
        {
            Window mainWindow = setupapp();
            Thread.Sleep(30);
            Assert.AreEqual("none", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("CanvasButton")).Click();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugOne")).Click();
            Thread.Sleep(7000);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Close();
        }

        [TestMethod]
        [TestCategory("DebugInput")]
        public void DeleteLineTest()
        {
            Window mainWindow = setupapp();
            Thread.Sleep(30);
            Assert.AreEqual("none", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("CanvasButton")).Click();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("DrawButton")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugOne")).Click();
            Thread.Sleep(7000);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("DeleteButton")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugOne")).Click();
            Thread.Sleep(7000);
            Assert.AreEqual("Last Action: Line number 0 was deleted.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Close();
        }

        [TestMethod]
        [TestCategory("DebugInput")]
        public void UndoTest()
        {
            Window mainWindow = setupapp();
            Thread.Sleep(30);
            Assert.AreEqual("none", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("CanvasButton")).Click();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugOne")).Click();
            Thread.Sleep(7000);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("UndoButton")).Click();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Close();
        }

        [TestMethod]
        [TestCategory("DebugInput")]
        public void RedoTest()
        {
            Window mainWindow = setupapp();
            Thread.Sleep(30);
            Assert.AreEqual("none", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("CanvasButton")).Click();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugOne")).Click();
            Thread.Sleep(7000);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("UndoButton")).Click();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("RedoButton")).Click();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Close();
        }

        [TestMethod]
        [TestCategory("DebugInput")]
        public void DrawSeveralLinesTest()
        {
            Window mainWindow = setupapp();
            Thread.Sleep(30);
            Assert.AreEqual("none", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("CanvasButton")).Click();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugOne")).Click();
            Thread.Sleep(7000);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugTwo")).Click();
            Thread.Sleep(30000);
            Assert.AreEqual("Last Action: Line number 1 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            mainWindow.Close();
        }

        [TestMethod]
        [TestCategory("DebugInput")]
        public void DeleteSeveralLinesTest()
        {
            Window mainWindow = setupapp();
            Thread.Sleep(30);
            Assert.AreEqual("none", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("CanvasButton")).Click();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugOne")).Click();
            Thread.Sleep(7000);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugTwo")).Click();
            Thread.Sleep(24000);
            Assert.AreEqual("Last Action: Line number 1 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("DeleteButton")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugThree")).Click();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: Line number 1 was deleted.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(10000);
            Assert.AreEqual("Last Action: Line number 0 was deleted.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Close();
        }

        [TestMethod]
        [TestCategory("DebugInput")]
        public void UndoSeveralLinesTest()
        {
            Window mainWindow = setupapp();
            Thread.Sleep(30);
            Assert.AreEqual("none", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("CanvasButton")).Click();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugOne")).Click();
            Thread.Sleep(7000);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugTwo")).Click();
            Thread.Sleep(30000);
            Assert.AreEqual("Last Action: Line number 1 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("UndoButton")).Click();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("UndoButton")).Click();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            mainWindow.Close();
        }

        [TestMethod]
        [TestCategory("DebugInput")]
        public void RedoSeveralLinesTest()
        {
            Window mainWindow = setupapp();
            Thread.Sleep(30);
            Assert.AreEqual("none", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("CanvasButton")).Click();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugOne")).Click();
            Thread.Sleep(7000);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugTwo")).Click();
            Thread.Sleep(24000);
            Assert.AreEqual("Last Action: Line number 1 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("UndoButton")).Click();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("UndoButton")).Click();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("RedoButton")).Click();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("RedoButton")).Click();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: Line number 1 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            mainWindow.Close();
        }

        [TestMethod]
        [TestCategory("DebugInput")]
        public void UndoAndRedoTest()
        {
            Window mainWindow = setupapp();
            Thread.Sleep(30);
            Assert.AreEqual("none", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("CanvasButton")).Click();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugOne")).Click();
            Thread.Sleep(7000);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugTwo")).Click();
            Thread.Sleep(24000);
            Assert.AreEqual("Last Action: Line number 1 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("UndoButton")).Click();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("UndoButton")).Click();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("RedoButton")).Click();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("RedoButton")).Click();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: Line number 1 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("DeleteButton")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugThree")).Click();
            Thread.Sleep(7000);
            Assert.AreEqual("Last Action: Line number 0 was deleted.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("UndoButton")).Click();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: Line number 1 was deleted.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            mainWindow.Close();
        }

        [TestMethod]
        [TestCategory("DebugInput")]
        public void UndoAndDrawTest()
        {
            Window mainWindow = setupapp();
            Thread.Sleep(30);
            Assert.AreEqual("none", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("CanvasButton")).Click();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugFour")).Click();
            Thread.Sleep(2000);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("UndoButton")).Click();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugFour")).Click();
            Thread.Sleep(2000);
            Assert.AreEqual("Last Action: Line number 1 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            mainWindow.Close();
        }

        [TestMethod]
        [TestCategory("DebugInput")]
        public void DeleteSeveralLinesAtOnceTest()
        {
            Window mainWindow = setupapp();
            Thread.Sleep(30);
            Assert.AreEqual("none", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("CanvasButton")).Click();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugFour")).Click();
            Thread.Sleep(2000);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugFour")).Click();
            Thread.Sleep(2000);
            Assert.AreEqual("Last Action: Line number 1 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("DeleteButton")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugFour")).Click();
            Thread.Sleep(2000);
            Assert.AreEqual("Last Action: Several Lines were deleted.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            mainWindow.Close();
        }

        [TestMethod]
        [TestCategory("DebugInput")]
        public void PointDrawTest()
        {
            Window mainWindow = setupapp();
            Thread.Sleep(30);
            Assert.AreEqual("none", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("CanvasButton")).Click();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugFour")).Click();
            Thread.Sleep(4000);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            mainWindow.Close();
        }

        [TestMethod]
        [TestCategory("DebugInput")]
        public void NewCanvasAfterDrawTest()
        {
            Window mainWindow = setupapp();
            Thread.Sleep(30);
            Assert.AreEqual("none", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("CanvasButton")).Click();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(30);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugThree")).Click();
            Thread.Sleep(4000);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("CanvasButton")).Click();
            Thread.Sleep(30);
            // Click on No button in warning
            Window messageBox0 = mainWindow.MessageBox("Warning");
            messageBox0.Get<Button>(SearchCriteria.ByText("No")).Click();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("CanvasButton")).Click();
            Thread.Sleep(30);
            // close warning
            Window messageBox1 = mainWindow.MessageBox("Warning");
            messageBox1.Close();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("CanvasButton")).Click();
            Thread.Sleep(30);
            // click yes button on warning
            Window messageBox2 = mainWindow.MessageBox("Warning");
            messageBox2.Get<Button>(SearchCriteria.ByText("Yes")).Click();
            Thread.Sleep(30);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(30);
            mainWindow.Close();
        }
    }

    [TestClass]
    public class FileImporterTests
    {
        /// <summary>
        /// The directory of the input files, saved for repeated use
        /// </summary>
        private String input_file_dir = null;
        /// <summary>
        /// instance of TestContext to be able to access deployed files
        /// </summary>
        private TestContext testContextInstance;
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        /// <summary>
        /// A function that returns the path to the test_input_files folder. 
        /// Do with it what you want.
        /// </summary>
        /// <returns>the path to the test_input_files folder</returns>
        public String getSketchAssistantDirectory()
        {
            Regex rx = new Regex(@"^(.*\\SketchAssistant\\)");
            Match match = rx.Match(TestContext.DeploymentDirectory);
            String SketchAssistDir = match.Groups[1].Value;
            if (input_file_dir == null)
            {
                if (Directory.Exists(SketchAssistDir + @"\WhiteTests\test_input_files\"))
                {
                    input_file_dir = SketchAssistDir + @"\WhiteTests\test_input_files\";
                }
                else if (Directory.Exists(SketchAssistDir + @"\WhiteTests\bin\Debug\test_input_files\"))
                {
                    input_file_dir = SketchAssistDir + @"\WhiteTests\bin\Debug\test_input_files\";
                }
                else
                {
                    Regex rx_0 = new Regex(@"^(.*\\projects\\)");
                    Match match_0 = rx_0.Match(TestContext.DeploymentDirectory);
                    String ProjectsDir = match_0.Groups[1].Value;
                    var dirs = Directory.GetDirectories(ProjectsDir, "test_input_files", SearchOption.AllDirectories);
                    input_file_dir = dirs[0];
                }
            }
            return input_file_dir;
        }

        /// <summary>
        /// creates a valid .isad file from the given sets of coordinates (number divisible by 3) by creating a line for every three consecutive points, parses the file and verifies that all lines and their points have been parsed correctly
        /// </summary>
        /// <param name="xCoordinates">an array containing the x coordinates of the points that will be created (length divisible by 3)</param>
        /// <param name="yCoordinates">an array containing the y coordinates of the points that will be created (length divisible by 3)</param>
        [DataTestMethod]
        [TestCategory("FileIO")]
        [DataRow(new int[] { 54, 43, 57, 11, 145, 34, 113, 299, 0 }, new int[] { 33, 42, 140, 30, 30, 30, 32, 145, 2 })]
        [DataRow(new int[] { 33, 42, 140, 30, 30, 30, 32, 145, 2 }, new int[] { 33, 42, 140, 30, 30, 30, 32, 145, 2 })]
        [DataRow(new int[] { 33, 42, 140, 30, 30, 30, 32, 145, 2 }, new int[] { 54, 43, 57, 11, 145, 34, 113, 199, 0 })]
        public void ParseISADInputSuccessfulTest(int[] xCoordinates, int[] yCoordinates)
        {
            FileImporter uut = new FileImporter();

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

            Tuple<int, int, List<InternalLine>> values = uut.ParseISADInputForTesting(file.ToArray());

            Assert.AreEqual(xCoordinates.Length / 3, values.Item3.Count);
            InternalLine[] lines = values.Item3.ToArray();
            for (int i = 0; i < xCoordinates.Length - 2; i += 3)
            {
                Point[] currentLine = lines[i / 3].GetPoints().ToArray();
                Assert.AreEqual(3, currentLine.Length);
                for (int j = 0; j < 3; j++)
                {
                    Assert.IsTrue(currentLine[j].X == xCoordinates[i + j] && currentLine[j].Y == yCoordinates[i + j]);
                }
            }
        }

        /// <summary>
        /// parses teh given invalid .isad files and verifies that a FileImporterException is thrown, but no other exception
        /// </summary>
        /// <param name="file">the input file represented as an array of lines</param>
        [DataTestMethod]
        [TestCategory("FileIO")]
        [DataRow(new String[] { })]
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
            bool correctExceptionThrown = false;

            FileImporter uut = new FileImporter();

            try
            {
                //try to initialize the left image with an invalid isad drawing
                Tuple<int, int, List<InternalLine>> values1 = uut.ParseISADInputForTesting(file);
            }
            catch (FileImporterException e)
            {
                //save the occurence of an exception
                correctExceptionThrown = true;
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }
            catch (Exception)
            {
                //don't set success flag
            }
            //check that an exception has been thrown
            Assert.IsTrue(correctExceptionThrown);
        }

        /// <summary>
        /// parses all whitelisted files and ensures no exceptions are thrown (parsing abortion, e.g. due to corrupted input files, are realized by throwing a FileImporterException)
        /// </summary>
        [TestMethod]
        [TestCategory("FileIO")]
        public void ParseSVGInputNoErrorForWhitelistedFilesTest()
        {
            FileImporter uut = new FileImporter();
            string[] files = Directory.GetFiles(getSketchAssistantDirectory() + @"\whitelisted", "*.svg", SearchOption.AllDirectories);

            Assert.IsTrue(files.Length > 0);

            foreach (string s in files) //parse each of the whitelisted files
            {
                Console.WriteLine(s);
                bool noExceptionThrown = true;
                try
                {
                    uut.ParseSVGInputFile(s, 10000, 10000);
                }
                catch (Exception)
                {
                    noExceptionThrown = false;
                }
                Assert.IsTrue(noExceptionThrown);
            }
        }

        /// <summary>
        /// parses all blacklisted files and ensures an instance of FileIporterException is thrown for each file, but no other exceptions occur
        /// </summary>
        [TestMethod]
        [TestCategory("FileIO")]
        public void ParseSVGInputNoErrorForBlacklistedFilesTest()
        {
            FileImporter uut = new FileImporter();

            string[] files = Directory.GetFiles(getSketchAssistantDirectory() + @"\blacklisted", "*.svg", SearchOption.AllDirectories);
            Assert.IsTrue(files.Length > 0);
            foreach (string s in files) //parse each of the blacklisted files
            {
                bool correctExceptionThrown = false;
                try
                {
                    uut.ParseSVGInputFile(s, 10000, 10000);
                }
                catch (FileImporterException e)
                {
                    System.Diagnostics.Debug.WriteLine(e.ToString());
                    correctExceptionThrown = true;
                }
                catch (Exception)
                {
                }
                Assert.IsTrue(correctExceptionThrown);
            }
        }
    }

    [TestClass]
    public class SimilarityCalculationTests
    {
        /// <summary>
        /// The debug data element used to generate random lines.
        /// </summary>
        private DebugData DebugData = new DebugData();

        /// <summary>
        /// Generates random lines and tests how similar they are. 
        /// To test the similarity score always stays between 0 and 1.
        /// </summary>
        [TestMethod]
        [TestCategory("UnitTest")]
        public void StaysWithinParameters()
        {
            Parallel.For(1, 100,
                i =>
                {
                    InternalLine l0 = DebugData.GetRandomLine(1, (uint)i);
                    InternalLine l1 = DebugData.GetRandomLine(1, (uint)i);
                    var sim = GeometryCalculator.CalculateSimilarity(l0, l1);
                    Assert.IsTrue((sim >= 0));
                    Assert.IsTrue((sim <= 1));
                });
        }

        [TestMethod]
        [TestCategory("UnitTest")]
        public void CorrectSimilarity()
        {
            Parallel.ForEach(DebugData.GetSimilarityTestData(),
                tup =>
                {
                    InternalLine l0 = tup.Item1;
                    InternalLine l1 = tup.Item2;
                    var sim = GeometryCalculator.CalculateSimilarity(l0, l1);
                    Assert.AreEqual(tup.Item3, sim, 0.00000001);
                });
        }

    }

    [TestClass]
    public class InternalLineUnitTests
    {
        /// <summary>
        /// The debug data element used to generate random lines.
        /// </summary>
        private DebugData DebugData = new DebugData();

        [TestMethod]
        [TestCategory("UnitTest")]
        public void MakePermanentTest()
        {

            List<Point> points = new List<Point>();
            points.AddRange(DebugData.debugPoints4);
            InternalLine uut = new InternalLine(points);
            Assert.AreEqual(false, uut.isPoint);
            uut.MakePermanent(5);
            Assert.AreEqual(true, uut.isPoint);
            Assert.AreEqual(5, uut.GetID());
            Assert.AreEqual(0, uut.GetLength());
        }

        [DataTestMethod]
        [TestCategory("UnitTest")]
        [DataRow(new int[] { 1, 1, 3, 3 }, new int[] { 1, 1, 2, 2, 3, 3 }, false, 2.828427125)]
        [DataRow(new int[] { 1, 1, 3, 3 }, new int[] { 1, 1, 2, 2, 3, 3 }, true, 2.828427125)]
        [DataRow(new int[] { 1, 1, 1, 4, 3, 4 }, new int[] { 1, 1, 1, 2, 1, 3, 1, 4, 2, 4, 3, 4 }, false, 5)]
        [DataRow(new int[] { 1, 1, 1, 4, 3, 4 }, new int[] { 1, 1, 1, 2, 1, 3, 1, 4, 2, 4, 3, 4 }, true, 5)]
        public void PermanentLineTest(int[] inPoints, int[] outPoints, bool isTemp, double len)
        {
            List<Point> inLine = new List<Point>(); List<Point> outLine = new List<Point>();
            for (int i = 0; i < inPoints.Length; i += 2) inLine.Add(new Point(inPoints[i], inPoints[i + 1]));
            for (int i = 0; i < outPoints.Length; i += 2) outLine.Add(new Point(outPoints[i], outPoints[i + 1]));
            InternalLine uut;
            if (isTemp)
            {
                uut = new InternalLine(inLine);
                var zip = inLine.Zip(uut.GetPoints(), (a, b) => new Tuple<Point, Point>(a, b));
                foreach (Tuple<Point, Point> tup in zip)
                {
                    Assert.AreEqual(tup.Item1, tup.Item2);
                }
            }
            else
            {
                uut = new InternalLine(inLine, 0);
                var zip = outLine.Zip(uut.GetPoints(), (a, b) => new Tuple<Point, Point>(a, b));
                foreach (Tuple<Point, Point> tup in zip)
                {
                    Assert.AreEqual(tup.Item1, tup.Item2);
                }
            }
            Assert.AreEqual(len, uut.GetLength(), 0.000001);
        }
    }
}
