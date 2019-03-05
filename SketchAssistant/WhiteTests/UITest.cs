using System;
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
            string outputDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string editedDir = outputDir.Replace("WhiteTests", "SketchAssistantWPF");
            string app_path = editedDir + @"\SketchAssistantWPF.exe";
            ProcessStartInfo processStart = new ProcessStartInfo(app_path, "-debug");
            application = Application.Launch(processStart);
            return application.GetWindow("Sketch Assistant");
        }

        [TestMethod]
        [TestCategory("bla")]
        public void CreateCanvasTest()
        {
            Window mainWindow = setupapp();
            Thread.Sleep(20);
            Assert.AreEqual("none", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("CanvasButton")).Click();
            Thread.Sleep(20);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Close();
        }

        [TestMethod]
        [TestCategory("bla")]
        public void DrawLineTest()
        {
            Window mainWindow = setupapp();
            Thread.Sleep(20);
            Assert.AreEqual("none", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("CanvasButton")).Click();
            Thread.Sleep(20);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugOne")).Click();
            Thread.Sleep(7000);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Close();
        }

        [TestMethod]
        public void DeleteLineTest()
        {
            Window mainWindow = setupapp();
            Thread.Sleep(20);
            Assert.AreEqual("none", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("CanvasButton")).Click();
            Thread.Sleep(20);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("DrawButton")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugOne")).Click();
            Thread.Sleep(7000);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(20);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("DeleteButton")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugOne")).Click();
            Thread.Sleep(7000);
            Assert.AreEqual("Last Action: Line number 0 was deleted.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Close();
        }

        [TestMethod]
        [TestCategory("bla")]
        public void UndoTest()
        {
            Window mainWindow = setupapp();
            Thread.Sleep(20);
            Assert.AreEqual("none", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("CanvasButton")).Click();
            Thread.Sleep(20);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugOne")).Click();
            Thread.Sleep(7000);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(20);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("UndoButton")).Click();
            Thread.Sleep(20);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Close();
        }

        [TestMethod]
        [TestCategory("bla")]
        public void RedoTest()
        {
            Window mainWindow = setupapp();
            Thread.Sleep(20);
            Assert.AreEqual("none", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("CanvasButton")).Click();
            Thread.Sleep(20);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugOne")).Click();
            Thread.Sleep(7000);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(20);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("UndoButton")).Click();
            Thread.Sleep(20);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(20);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("RedoButton")).Click();
            Thread.Sleep(20);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Close();
        }

        [TestMethod]
        public void DrawSeveralLines()
        {
            Window mainWindow = setupapp();
            Thread.Sleep(20);
            Assert.AreEqual("none", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("CanvasButton")).Click();
            Thread.Sleep(20);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugOne")).Click();
            Thread.Sleep(7000);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugTwo")).Click();
            Thread.Sleep(30000);
            Assert.AreEqual("Last Action: Line number 1 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(20);
            mainWindow.Close();
        }

        [TestMethod]
        public void DeleteSeveralLines()
        {
            Window mainWindow = setupapp();
            Thread.Sleep(20);
            Assert.AreEqual("none", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("CanvasButton")).Click();
            Thread.Sleep(20);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugOne")).Click();
            Thread.Sleep(7000);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugTwo")).Click();
            Thread.Sleep(24000);
            Assert.AreEqual("Last Action: Line number 1 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(20);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("DeleteButton")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugThree")).Click();
            Thread.Sleep(20);
            Assert.AreEqual("Last Action: Line number 1 was deleted.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(10000);
            Assert.AreEqual("Last Action: Line number 0 was deleted.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Close();
        }

        [TestMethod]
        public void UndoSeveralLines()
        {
            Window mainWindow = setupapp();
            Thread.Sleep(20);
            Assert.AreEqual("none", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("CanvasButton")).Click();
            Thread.Sleep(20);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugOne")).Click();
            Thread.Sleep(7000);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugTwo")).Click();
            Thread.Sleep(30000);
            Assert.AreEqual("Last Action: Line number 1 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(20);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("UndoButton")).Click();
            Thread.Sleep(20);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(20);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("UndoButton")).Click();
            Thread.Sleep(20);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(20);
            mainWindow.Close();
        }

        [TestMethod]
        public void RedoSeveralLines()
        {
            Window mainWindow = setupapp();
            Thread.Sleep(20);
            Assert.AreEqual("none", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("CanvasButton")).Click();
            Thread.Sleep(20);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugOne")).Click();
            Thread.Sleep(7000);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugTwo")).Click();
            Thread.Sleep(24000);
            Assert.AreEqual("Last Action: Line number 1 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(20);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("UndoButton")).Click();
            Thread.Sleep(20);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(20);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("UndoButton")).Click();
            Thread.Sleep(20);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(20);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("RedoButton")).Click();
            Thread.Sleep(20);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(20);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("RedoButton")).Click();
            Thread.Sleep(20);
            Assert.AreEqual("Last Action: Line number 1 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(20);
            mainWindow.Close();
        }

        [TestMethod]
        public void UndoAndRedoTests()
        {
            Window mainWindow = setupapp();
            Thread.Sleep(20);
            Assert.AreEqual("none", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("CanvasButton")).Click();
            Thread.Sleep(20);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugOne")).Click();
            Thread.Sleep(7000);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugTwo")).Click();
            Thread.Sleep(24000);
            Assert.AreEqual("Last Action: Line number 1 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(20);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("UndoButton")).Click();
            Thread.Sleep(20);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(20);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("UndoButton")).Click();
            Thread.Sleep(20);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(20);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("RedoButton")).Click();
            Thread.Sleep(20);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(20);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("RedoButton")).Click();
            Thread.Sleep(20);
            Assert.AreEqual("Last Action: Line number 1 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(20);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("DeleteButton")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugThree")).Click();
            Thread.Sleep(7000);
            Assert.AreEqual("Last Action: Line number 0 was deleted.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(20);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("UndoButton")).Click();
            Thread.Sleep(20);
            Assert.AreEqual("Last Action: Line number 1 was deleted.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(20);
            mainWindow.Close();
        }

        [TestMethod]
        public void PointDraw()
        {
            Window mainWindow = setupapp();
            Thread.Sleep(20);
            Assert.AreEqual("none", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("CanvasButton")).Click();
            Thread.Sleep(20);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugFour")).Click();
            Thread.Sleep(4000);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(20);
            mainWindow.Close();
        }

        [TestMethod]
        public void NewCanvasAfterDraw()
        {
            Window mainWindow = setupapp();
            Thread.Sleep(20);
            Assert.AreEqual("none", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("CanvasButton")).Click();
            Thread.Sleep(20);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
            Thread.Sleep(20);
            mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugThree")).Click();
            Thread.Sleep(4000);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(20);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("CanvasButton")).Click();
            Thread.Sleep(20);
            // Click on No button in warning
            Window messageBox0 = mainWindow.MessageBox("Warning");
            messageBox0.Get<Button>(SearchCriteria.ByText("No")).Click();
            Thread.Sleep(20);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(20);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("CanvasButton")).Click();
            Thread.Sleep(20);
            // close warning
            Window messageBox1 = mainWindow.MessageBox("Warning");
            messageBox1.Close();
            Thread.Sleep(20);
            Assert.AreEqual("Last Action: Line number 0 was drawn.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(20);
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("CanvasButton")).Click();
            Thread.Sleep(20);
            // click yes button on warning
            Window messageBox2 = mainWindow.MessageBox("Warning");
            messageBox2.Get<Button>(SearchCriteria.ByText("Yes")).Click();
            Thread.Sleep(20);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            Thread.Sleep(20);
            mainWindow.Close();
        }
    }

    [TestClass]
    [DeploymentItem(@"WhiteTests\test_input_files\")]
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
            if(input_file_dir == null)
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
        [TestCategory("bla")]
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
        [TestCategory("bla")]
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
            bool correctExceptionThrown = false;

            FileImporter uut = new FileImporter();

            try
            {
                //try to initialize the left image with an invalid isad drawing
                Tuple<int, int, List<InternalLine>> values1 = uut.ParseISADInputForTesting(file);
            }
            catch (FileImporterException)
            {
                //save the occurence of an exception
                correctExceptionThrown = true;
            }
            catch (Exception e)
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
                catch (Exception e)
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
                    correctExceptionThrown = true;
                }
                catch (Exception e)
                {
                }
                Assert.IsTrue(correctExceptionThrown);
            }
        }
    }
}
