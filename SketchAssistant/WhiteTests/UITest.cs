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
            application = Application.Launch(app_path);
            Window mainWindow = application.GetWindow("Sketch Assistant");
        }

        [TestMethod]
        public void CreateCanvasTest()
        {
            setupapp();
            Window mainWindow = application.GetWindow("Sketch Assistant");
            Thread.Sleep(100);
            Assert.AreEqual("none", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Get<Button>(SearchCriteria.ByAutomationId("CanvasButton")).Click();
            Thread.Sleep(100);
            Assert.AreEqual("Last Action: A new canvas was created.", mainWindow.Get<TextBox>(SearchCriteria.ByAutomationId("LastActionBox")).Text.ToString());
            mainWindow.Close();
        }
    }
}
