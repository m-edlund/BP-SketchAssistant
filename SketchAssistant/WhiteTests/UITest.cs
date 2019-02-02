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

namespace WhiteTests
{
    [TestClass]
    public class UITest
    {
        private TestStack.White.Application application;

        public void setupapp()
        {
            string outputDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string editedDir = outputDir.Replace("WhiteTests", "SketchAssistantWPF");
            string app_path = editedDir + @"\SketchAssistantWPF.exe";
            ProcessStartInfo processStart = new ProcessStartInfo(app_path, "-debug");
            application = Application.Launch(processStart);
            Window mainWindow = application.GetWindow("Sketch Assistant");
        }

        [TestMethod]
        public void CreateCanvasTest()
        {
            setupapp();
            Window mainWindow = application.GetWindow("Sketch Assistant");
            Thread.Sleep(20);
            Assert.AreEqual("none", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("CanvasButton")).Click();
            Thread.Sleep(20);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Close();
        }

        [TestMethod]
        public void DrawLineTest()
        {
            setupapp();
            Window mainWindow = application.GetWindow("Sketch Assistant");
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
    }
}
