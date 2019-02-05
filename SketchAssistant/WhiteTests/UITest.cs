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

        /*[TestMethod]
         public void DeleteLineTest()
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
             mainWindow.Get<Button>(SearchCriteria.ByAutomationId("DeleteButton")).Click();
             Thread.Sleep(20);
             mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("EditMenuButton")).Click();
             Thread.Sleep(20);
             mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugMode")).Click();
             Thread.Sleep(20);
             mainWindow.Get<Menu>(SearchCriteria.ByAutomationId("DebugThree")).Click();
             Thread.Sleep(24000);
             Assert.AreEqual("Last Action: Line number 0 was deleted", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
             mainWindow.Close();
         }*/

        [TestMethod]
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
    }
}
