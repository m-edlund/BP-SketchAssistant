﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace SketchAssistantWPF
{
    public class MVP_Presenter
    {
        /// <summary>
        /// The View of the MVP-Model, in this case Form1.
        /// </summary>
        MVP_View programView;
        /// <summary>
        /// The Model of the MVP-Model.
        /// </summary>
        MVP_Model programModel;
        /// <summary>
        /// A dictionary connecting the id of an InternalLine with the respective Polyline in the right canvas.
        /// </summary>
        Dictionary<int, Shape> rightPolyLines;

        ImageDimension CanvasSizeLeft = new ImageDimension(0,0);

        ImageDimension CanvasSizeRight = new ImageDimension(0, 0);

        ImageDimension ImageSizeLeft = new ImageDimension(0, 0);

        ImageDimension ImageSizeRight = new ImageDimension(0, 0);

        List<double> ImageSimilarity = new List<double>();

        List<InternalLine> LeftLines = new List<InternalLine>();

        /*******************/
        /*** ENUMERATORS ***/
        /*******************/

        public enum MouseAction
        {
            Click,
            Down,
            Up,
            Up_Invalid,
            Move
        }

        /***********************/
        /*** CLASS VARIABLES ***/
        /***********************/

        /// <summary>
        /// Instance of FileImporter to handle drawing imports.
        /// </summary>
        private FileImporter fileImporter;


        public MVP_Presenter(MVP_View form)
        {
            programView = form;
            programModel = new MVP_Model(this);
            //Initialize Class Variables
            fileImporter = new FileImporter();
        }

        /***********************************/
        /*** FUNCTIONS VIEW -> PRESENTER ***/
        /***********************************/

        /// <summary>
        /// Pass-trough function to update the appropriate information of the model, when the window is resized.
        /// </summary>
        /// <param name="leftPBS">The new size of the left picture box.</param>
        /// <param name="rightPBS">The new size of the left picture box.</param>
        public void Resize(Tuple<int, int> leftPBS, Tuple<int, int> rightPBS)
        {
            CanvasSizeLeft.ChangeDimension(leftPBS.Item1, leftPBS.Item2);
            CanvasSizeRight.ChangeDimension(rightPBS.Item1, rightPBS.Item2);
            //programModel.UpdateSizes(CanvasSizeRight);
            programModel.ResizeEvent(CanvasSizeLeft, CanvasSizeRight);
        }

        /// <summary>
        /// Display a new FileDialog to load a collection of lines.
        /// </summary>
        /// <returns>True if loading was a success</returns>
        public bool ExamplePictureToolStripMenuItemClick()
        {
            var okToContinue = true; bool returnval = false;
            if (programModel.HasUnsavedProgress())
            {
                okToContinue = programView.ShowWarning("You have unsaved progress. Continue?");
            }
            if (okToContinue)
            {
                var fileNameTup = programView.openNewDialog("Interactive Sketch-Assistant Drawing|*.isad");
                if (!fileNameTup.Item1.Equals("") && !fileNameTup.Item2.Equals(""))
                {
                    programView.SetToolStripLoadStatus(fileNameTup.Item2);
                    try
                    {
                        Tuple<int, int, List<InternalLine>> values = fileImporter.ParseISADInputFile(fileNameTup.Item1);
                        values.Item3.ForEach(line => line.MakePermanent(0)); //Make all lines permanent
                        programModel.SetLeftLineList(values.Item1, values.Item2, values.Item3);

                        programModel.ResetRightImage();
                        programModel.CanvasActivated();
                        programModel.ChangeState(true);
                        programView.EnableTimer();
                        ClearRightLines();
                        returnval = true;
                    }
                    catch (FileImporterException ex)
                    {
                        programView.ShowInfoMessage(ex.ToString());
                    }
                    catch (Exception ex)
                    {
                        programView.ShowInfoMessage("exception occured while trying to load interactive sketch-assistant drawing file:\n\n" + ex.ToString() + "\n\n" + ex.StackTrace);
                    }
                }
            }
            return returnval;
        }

        /// <summary>
        /// Display a new FileDialog to a svg drawing.
        /// </summary>
        /// <returns>True if loading was a success</returns>
        public bool SVGToolStripMenuItemClick()
        {
            var okToContinue = true; bool returnval = false;
            if (programModel.HasUnsavedProgress())
            {
                okToContinue = programView.ShowWarning("You have unsaved progress. Continue?");
            }
            if (okToContinue)
            {
                var fileNameTup = programView.openNewDialog("Scalable Vector Graphics|*.svg");
                if (!fileNameTup.Item1.Equals("") && !fileNameTup.Item2.Equals(""))
                {
                    programView.SetToolStripLoadStatus(fileNameTup.Item2);
                    try
                    {
                        Tuple<int, int, List<InternalLine>> values = fileImporter.ParseSVGInputFile(fileNameTup.Item1, programModel.leftImageBoxWidth, programModel.leftImageBoxHeight);
                        values.Item3.ForEach(line => line.MakePermanent(0)); //Make all lines permanent
                        programModel.SetLeftLineList(values.Item1, values.Item2, values.Item3);
                        programModel.ResetRightImage();
                        programModel.CanvasActivated();
                        programModel.ChangeState(true);
                        programView.EnableTimer();
                        ClearRightLines();
                        returnval = true;
                    }
                    catch (FileImporterException ex)
                    {
                        programView.ShowInfoMessage(ex.ToString());
                    }
                    catch (Exception ex)
                    {
                        programView.ShowInfoMessage("exception occured while trying to parse svg file:\n\n" + ex.ToString() + "\n\n" + ex.StackTrace);
                    }
                }
            }
            return returnval;
        }

        /// <summary>
        /// Pass-trough function to change the drawing state of the model.
        /// </summary>
        /// <param name="NowDrawing">Indicates if the program is in drawing (true) or deletion (false) mode.</param>
        public void ChangeState(bool NowDrawing)
        {
            programModel.ChangeState(NowDrawing);
        }

        /// <summary>
        /// Pass-trough function to undo an action.
        /// </summary>
        public void Undo()
        {
            programModel.Undo();
        }

        /// <summary>
        /// Pass-trough function to redo an action.
        /// </summary>
        public void Redo()
        {
            programModel.Redo();
        }

        /// <summary>
        /// Pass-trough function for ticking the model.
        /// </summary>
        public void Tick()
        {
            programModel.Tick();
        }

        /// <summary>
        /// Checks if there is unsaved progress, and promts the model to generate a new canvas if not.
        /// </summary>
        public void NewCanvas()
        {
            var okToContinue = true;
            if (programModel.HasUnsavedProgress())
            {
                okToContinue = programView.ShowWarning("You have unsaved progress. Continue?");
            }
            if (okToContinue)
            {
                programModel.ResizeEvent(CanvasSizeLeft, CanvasSizeRight);
                programModel.ResetRightImage();
                programModel.CanvasActivated();
                programModel.ChangeState(true);
                programView.EnableTimer();
                ClearRightLines();
            }
        }

        /// <summary>
        /// Pass-trough when the mouse is moved.
        /// </summary>
        /// <param name="mouseAction">The action which is sent by the View.</param>
        /// <param name="e">The Mouse event arguments.</param>
        public void MouseEvent(MouseAction mouseAction, Point position)
        {
            switch (mouseAction)
            {
                case MouseAction.Move:
                    programModel.SetCurrentCursorPosition(position);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Pass-trough function that calls the correct Mouse event of the model, when the mouse is clicked.
        /// </summary>
        /// <param name="mouseAction">The action which is sent by the View.</param>
        /// <param name="e">The Mouse event arguments.</param>
        public void MouseEvent(MouseAction mouseAction)
        {
            switch (mouseAction)
            {
                case MouseAction.Click:
                    programModel.MouseDown();
                    programModel.Tick();
                    programModel.MouseUp(true);
                    break;
                case MouseAction.Down:
                    programModel.MouseDown();
                    break;
                case MouseAction.Up:
                    programModel.MouseUp(true);
                    break;
                case MouseAction.Up_Invalid:
                    programModel.MouseUp(false);
                    break;
                default:
                    break;
            }
        }

        /************************************/
        /*** FUNCTIONS MODEL -> PRESENTER ***/
        /************************************/

        /// <summary>
        /// Return the position of the cursor
        /// </summary>
        /// <returns>The position of the cursor</returns>
        public Point GetCursorPosition()
        {
            return programView.GetCursorPosition();
        }

        /// <summary>
        /// Clears all Lines in the right canvas.
        /// </summary>
        public void ClearRightLines()
        {
            programView.RemoveAllRightLines();
            rightPolyLines = new Dictionary<int, Shape>();
            //Reset the similarity display
            UpdateSimilarityScore(Double.NaN);
        }

        /// <summary>
        /// A function to update the displayed lines in the right canvas.
        /// </summary>
        public void UpdateRightLines(List<Tuple<bool, InternalLine>> lines)
        {
            foreach(Tuple<bool, InternalLine> tup in lines)
            {
                var status = tup.Item1;
                var line = tup.Item2;
                if (!rightPolyLines.ContainsKey(line.GetID()))
                {
                    if (!line.isPoint)
                    {
                        Polyline newLine = new Polyline();
                        newLine.Points = line.GetPointCollection();
                        rightPolyLines.Add(line.GetID(), newLine);
                        programView.AddNewLineRight(newLine);
                    }
                    else
                    {
                        Ellipse newPoint = new Ellipse();
                        rightPolyLines.Add(line.GetID(), newPoint);
                        programView.AddNewPointRight(newPoint, line);
                    }
                }
                SetVisibility(rightPolyLines[line.GetID()], status);
            }
            //Calculate similarity scores, maybe change later to compare to the 
            UpdateSimilarityScore(Double.NaN); var templist = lines.Where(tup => tup.Item1).ToList();
            if(LeftLines.Count > 0)
            {
                for(int i = 0; i < LeftLines.Count; i++)
                {
                    if (templist.Count == i) break;
                    UpdateSimilarityScore(GeometryCalculator.CalculateSimilarity(templist[i].Item2, LeftLines[i]));
                }
            }
            else
            {
                for (int i = 0; i < templist.Count - 1; i++)
                    UpdateSimilarityScore(GeometryCalculator.CalculateSimilarity(templist[i].Item2, templist[i + 1].Item2));
            }
        }

        /*
        /// <summary>
        /// Updates the currentline
        /// </summary>
        /// <param name="linepoints">The points of the current line.</param>
        public void UpdateCurrentLine(List<Point> linepoints)
        {
            Polyline currentLine = new Polyline();
            currentLine.Stroke = Brushes.Black;
            currentLine.Points = new PointCollection(linepoints);
            programView.DisplayCurrLine(currentLine);
        }
        */
        /// <summary>
        /// A function to update the displayed lines in the left canvas.
        /// </summary>
        public void UpdateLeftLines(List<InternalLine> lines)
        {
            programView.RemoveAllLeftLines();
            foreach (InternalLine line in lines)
            {
                Polyline newLine = new Polyline();
                newLine.Stroke = Brushes.Black;
                newLine.Points = line.GetPointCollection();
                programView.AddNewLineLeft(newLine);
            }
            programView.SetCanvasState("LeftCanvas", true);
            programView.SetCanvasState("RightCanvas", true);

            LeftLines = lines;
        }

        /// <summary>
        /// Called by the model when the state of the Program changes. 
        /// Changes the look of the UI according to the current state of the model.
        /// </summary>
        /// <param name="inDrawingMode">If the model is in Drawing Mode</param>
        /// <param name="canUndo">If actions in the model can be undone</param>
        /// <param name="canRedo">If actions in the model can be redone</param>
        /// <param name="canvasActive">If the right canvas is active</param>
        /// <param name="graphicLoaded">If an image is loaded in the model</param>
        public void UpdateUIState(bool inDrawingMode, bool canUndo, bool canRedo, bool canvasActive, bool graphicLoaded)
        {
            Dictionary<String, MainWindow.ButtonState> dict = new Dictionary<String, MainWindow.ButtonState> {
                {"canvasButton", MainWindow.ButtonState.Enabled }, {"drawButton", MainWindow.ButtonState.Disabled}, {"deleteButton",MainWindow.ButtonState.Disabled },
                {"undoButton", MainWindow.ButtonState.Disabled },{"redoButton",  MainWindow.ButtonState.Disabled}};

            if (canvasActive)
            {
                if (inDrawingMode)
                {
                    dict["drawButton"] = MainWindow.ButtonState.Active;
                    dict["deleteButton"] = MainWindow.ButtonState.Enabled;
                }
                else
                {
                    dict["drawButton"] = MainWindow.ButtonState.Enabled;
                    dict["deleteButton"] = MainWindow.ButtonState.Active;
                }
                if (canUndo) { dict["undoButton"] = MainWindow.ButtonState.Enabled; }
                if (canRedo) { dict["redoButton"] = MainWindow.ButtonState.Enabled; }
            }
            foreach (KeyValuePair<String, MainWindow.ButtonState> entry in dict)
            {
                programView.SetToolStripButtonStatus(entry.Key, entry.Value);
            }
            programView.SetCanvasState("RightCanvas", canvasActive);
            programView.SetCanvasState("LeftCanvas", graphicLoaded);
        }


        /// <summary>
        /// Pass-trough function to display an info message in the view.
        /// </summary>
        /// <param name="msg">The message.</param>
        public void PassMessageToView(String msg)
        {
            programView.ShowInfoMessage(msg);
        }

        /// <summary>
        /// Pass-trough function to update the display of the last action taken.
        /// </summary>
        /// <param name="msg">The new last action taken.</param>
        public void PassLastActionTaken(String msg)
        {
            programView.SetLastActionTakenText(msg);
        }

        /// <summary>
        /// Passes whether or not the mouse is pressed.
        /// </summary>
        /// <returns>Whether or not the mouse is pressed</returns>
        public bool IsMousePressed()
        {
            return programView.IsMousePressed();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="score">Score will be reset if NaN is passed, 
        /// will be ignored if the score is not between 0 and 1</param>
        public void UpdateSimilarityScore(double score)
        {
            if (Double.IsNaN(score))
            {
                ImageSimilarity.Clear();
                programView.SetImageSimilarityText("");
            }
            else
            {
                if (score >= 0 && score <= 1) ImageSimilarity.Add(score);
                programView.SetImageSimilarityText((ImageSimilarity.Sum() / ImageSimilarity.Count).ToString());
            }
        }

        /*************************/
        /*** HELPING FUNCTIONS ***/
        /*************************/

        /// <summary>
        /// Sets the visibility of a polyline.
        /// </summary>
        /// <param name="line">The polyline</param>
        /// <param name="visible">Whether or not it should be visible.</param>
        private void SetVisibility(Shape line, bool visible)
        {
            if (!visible)
            {
                line.Opacity = 0.00001;
            }
            else
            {
                line.Opacity = 1;
            }
        }
    }
}
