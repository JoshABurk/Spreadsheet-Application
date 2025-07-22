// <copyright file="Spreadsheet.cs" company="Joshua Burk ID: 011731347">
// Copyright (c) Joshua Burk ID: 011731347. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Transactions;
using System.Xml;
using ExpressionTreeEngine;

namespace SpreadsheetEngine
{
    /// <summary>
    /// Class for the Spreadsheet.
    /// </summary>
    public class Spreadsheet
    {
        /// <summary>
        /// The Spreadsheet.
        /// </summary>
        private Cell[,] spreadsheetArray;

        /// <summary>
        /// The Stack to hold undo's.
        /// </summary>
        private Stack<UndoRedo> undoStack;

        /// <summary>
        /// The Stack to hold undo's.
        /// </summary>
        private Stack<UndoRedo> redoStack;

        /// <summary>
        /// Initializes a new instance of the <see cref="Spreadsheet"/> class.
        /// </summary>
        /// <param name="rows">the number of rows in the spreadsheet.</param>
        /// <param name="columns">the number of columns in the spreadsheet.</param>
        public Spreadsheet(int rows, int columns)
        {
            this.spreadsheetArray = new Cell[rows, columns];

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    this.spreadsheetArray[r, c] = new ConcreteCell(r, c);
                    this.spreadsheetArray[r, c].PropertyChanged += this.OnCellPropertyChanged;
                }
            }

            this.undoStack = new Stack<UndoRedo>();
            this.redoStack = new Stack<UndoRedo>();
        }

        /// <summary>
        ///  The Event Handler.
        /// </summary>
        public event PropertyChangedEventHandler? CellPropertyChanged;

        /// <summary>
        /// Gets the column count.
        /// </summary>
        public int ColumnCount
        {
            get { return this.spreadsheetArray.GetLength(1); }
        }

        /// <summary>
        /// Gets the row count.
        /// </summary>
        public int RowCount
        {
            get { return this.spreadsheetArray.GetLength(0); }
        }

        /// <summary>
        /// gets the cell from SpreadsheetArray at index [row, column].
        /// </summary>
        /// <param name="row">Row number of the cell.</param>
        /// <param name="column">Column number of the cell.</param>
        /// <returns>The Cell object at index (row, column).</returns>
        public Cell? GetCell(int row, int column)
        {
            if (row >= 0 && column >= 0 && row < this.spreadsheetArray.GetLength(0) && column < this.spreadsheetArray.GetLength(1))
            {
                return this.spreadsheetArray[row, column];
            }

            return null;
        }

        /// <summary>
        /// an Overloaded GetCell that takes the Variable name as input and returns the cell.
        /// </summary>
        /// <param name="variable">Variable name.</param>
        /// <returns>The Cell object.</returns>
        public Cell? GetCell(string variable)
        {
            // Convert the variable into row and column indices
            int column = variable[0] - 'A';
            int row = int.Parse(variable.Substring(1)) - 1; // accounts for the first row being 0 not 1.

            return this.GetCell(row, column);
        }

        /// <summary>
        /// Returns undo stack size.
        /// </summary>
        /// <returns>(int) Size of stack.</returns>
        public int GetUndoStackSize()
        {
            return this.undoStack.Count;
        }

        /// <summary>
        /// Adds a cell unto the Undo Stack.
        /// </summary>
        /// <param name="undoCell">The Cell that is to be added to undo stack.</param>
        /// <param name="description">what is being changed.</param>
        public void AddUndo(Cell undoCell, string description = "")
        {
            this.undoStack.Push(new UndoRedo(undoCell, description));
        }

        /// <summary>
        /// Takes Cell off of undo stack and copies values to its original index.
        /// Adds the Old cell to redo stack.
        /// </summary>
        /// <returns>the description of the undo.</returns>
        public string? ExecuteUndo()
        {
            if (this.undoStack.Count > 0)
            {
                UndoRedo undoAction = this.undoStack.Pop();
                string message = undoAction.GetDescription();
                Tuple<int, int> coordinates = undoAction.GetCoordinates();
                Cell cellToUndo = this.spreadsheetArray[coordinates.Item1, coordinates.Item2];
                if (cellToUndo.BGColor == undoAction.GetOldCell().BGColor)
                {
                    this.AddRedo(cellToUndo.CreateCopy(), "cell text change");
                }
                else
                {
                    this.AddRedo(cellToUndo.CreateCopy(), "changing cell background color");
                }

                cellToUndo.Text = undoAction.GetOldCell().Text; // This should internally update Value and trigger PropertyChanged if necessary.
                cellToUndo.BGColor = undoAction.GetOldCell().BGColor;
                cellToUndo.Dependents = undoAction.GetOldCell().Dependents;
                cellToUndo.DependsOn = undoAction.GetOldCell().DependsOn;
                return message;
            }

            return null;
        }

        /// <summary>
        /// Returns redo stack size.
        /// </summary>
        /// <returns>(int) Size of stack.</returns>
        public int GetRedoStackSize()
        {
            return this.redoStack.Count;
        }

        /// <summary>
        /// Adds a cell onto the redo Stack.
        /// </summary>
        /// <param name="redoCell">The Cell that is to be added to redo stack.</param>
        /// <param name="description">what is being changed.</param>
        public void AddRedo(Cell redoCell, string description = "")
        {
            this.redoStack.Push(new UndoRedo(redoCell, description));
        }

        /// <summary>
        /// Gets description of cell on top of stack.
        /// </summary>
        /// <returns>the description of cell.</returns>
        public string? PeepRedo()
        {
            if (this.redoStack.Count > 0)
            {
                return this.redoStack.Peek().GetDescription();
            }

            return null;
        }

        /// <summary>
        /// Gets description of cell on top of stack.
        /// </summary>
        /// <returns>the description of cell.</returns>
        public string? PeepUndo()
        {
            if (this.undoStack.Count > 0)
            {
                return this.undoStack.Peek().GetDescription();
            }

            return null;
        }

        /// <summary>
        /// Clears the redo stack.
        /// </summary>
        public void ClearRedo()
        {
            this.redoStack.Clear();
        }

        /// <summary>
        /// Takes Cell off of redo stack and copies values to its original index.
        /// Adds the old cell to the undo stack.
        /// </summary>
        /// <returns>the description of the undo.</returns>
        public string? ExecuteRedo()
        {
            if (this.redoStack.Count > 0)
            {
                UndoRedo redoAction = this.redoStack.Pop();
                string message = redoAction.GetDescription();
                Tuple<int, int> coordinates = redoAction.GetCoordinates();
                Cell cellToRedo = this.spreadsheetArray[coordinates.Item1, coordinates.Item2];
                if (cellToRedo.BGColor == redoAction.GetOldCell().BGColor)
                {
                    this.AddUndo(cellToRedo.CreateCopy(), "cell text change");
                }
                else
                {
                    this.AddUndo(cellToRedo.CreateCopy(), "changing cell background color");
                }

                cellToRedo.Text = redoAction.GetOldCell().Text; // This should internally update Value and trigger PropertyChanged if necessary.
                cellToRedo.BGColor = redoAction.GetOldCell().BGColor;
                cellToRedo.Dependents = redoAction.GetOldCell().Dependents;
                cellToRedo.DependsOn = redoAction.GetOldCell().DependsOn;
                return message;
            }

            return null;
        }

        /// <summary>
        /// Goes through the Spreadsheet and finds cell's that are not default,
        /// then writes them to an xml file using XmlWriter.
        /// </summary>
        /// <param name="stream">The output xml file stream.</param>
        public void Save(Stream stream)
        {
            using (XmlWriter writer = XmlWriter.Create(stream, new XmlWriterSettings { Indent = true }))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("spreadsheet");

                // Parese through every Cell of the spreadsheet.
                for (int r = 0; r < this.RowCount; r++)
                {
                    for (int c = 0; c < this.ColumnCount; c++)
                    {
                        Cell cell = this.spreadsheetArray[r, c];
                        if (cell.Text != string.Empty || cell.BGColor != 0xFFFFFFFF) // Only write non-default cells
                        {
                            writer.WriteStartElement("cell");
                            writer.WriteAttributeString("name", $"{(char)('A' + c)}{r + 1}");

                            if (cell.BGColor != 0xFFFFFFFF)
                            {
                                writer.WriteElementString("bgcolor", cell.BGColor.ToString("X8"));
                            }

                            if (!string.IsNullOrEmpty(cell.Text))
                            {
                                writer.WriteElementString("text", cell.Text);
                            }

                            writer.WriteEndElement();
                        }
                    }
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        /// <summary>
        /// Clears the spreadsheet and undo/redo stacks. Then loads in the cell's from the xml file
        /// and places them in the spreadsheet.
        /// </summary>
        /// <param name="stream">The input xml file stream.</param>
        public void Load(Stream stream)
        {
            // Iterate over all cells and reset them
            for (int r = 0; r < this.spreadsheetArray.GetLength(0); r++)
            {
                for (int c = 0; c < this.spreadsheetArray.GetLength(1); c++)
                {
                    this.spreadsheetArray[r, c].Text = string.Empty;
                    this.spreadsheetArray[r, c].Value = string.Empty;
                    this.spreadsheetArray[r, c].BGColor = 0xFFFFFFFF;
                    this.spreadsheetArray[r, c].Dependents.Clear();
                    this.spreadsheetArray[r, c].DependsOn.Clear();
                }
            }

            // reset undo/redo stacks
            this.undoStack.Clear();
            this.redoStack.Clear();

            using (XmlReader reader = XmlReader.Create(stream))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement() && reader.Name == "cell") // makes sure xml data is of type ".cell".
                    {
                        string? cellName = reader["name"];
                        if (cellName != null)
                        {
                            int column = cellName[0] - 'A';
                            int row = int.Parse(cellName.Substring(1)) - 1;

                            while (reader.Read())
                            {
                                if (reader.NodeType == XmlNodeType.Element)
                                {
                                    switch (reader.Name)
                                    {
                                        case "bgcolor":
                                            string bgColor = reader.ReadElementContentAsString();
                                            if (bgColor.Length == 6)
                                            {
                                                bgColor = "FF" + bgColor;
                                            }

                                            this.spreadsheetArray[row, column].BGColor = uint.Parse(bgColor, System.Globalization.NumberStyles.HexNumber);
                                            break;
                                        case "text":
                                            string text = reader.ReadElementContentAsString();
                                            this.spreadsheetArray[row, column].Text = text;
                                            break;
                                    }
                                }

                                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "cell")
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Event Handler anytime a cell's text is changed.
        /// If the cell's text value starts with '=' grab the value
        /// from the referenced cell.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The Event data.</param>
        private void OnCellPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is Cell cell)
            {
                if (e.PropertyName == nameof(Cell.Text))
                {
                    cell.SetError(null);

                    foreach (var variable in cell.DependsOn)
                    {
                        this.GetCell(variable.RowIndex, variable.ColumnIndex)?.Dependents.Remove(cell);
                    }

                    cell.DependsOn = new List<Cell>();

                    if (!string.IsNullOrEmpty(cell.Text) && cell.Text.StartsWith("="))
                    {
                        string formula = cell.Text.Substring(1); // Remove '=' from start
                        ExpressionTree exprTree = new ExpressionTree(formula); // Build the expression tree

                        this.UpdateExpressionTreeVariables(exprTree, cell);

                        if (cell.ErrorMessage == null)
                        {
                            // Evaluate the expression
                            double result = exprTree.Evaluate();

                            // Update the cell's value with the result
                            cell.Value = result.ToString();

                            foreach (var variable in exprTree.GetVariables())
                            {
                                var cellToAdd = this.GetCell(variable);
                                if (cellToAdd != null)
                                {
                                    cell.DependsOn.Add(cellToAdd);
                                    cellToAdd.Dependents.Add(cell);
                                }
                            }
                        }
                    }
                    else
                    {
                        // If the cell text does not start with '=', just set the cell value to the text
                        if (cell.Text != null)
                        {
                            cell.Value = cell.Text;
                        }
                        else
                        {
                            // Handles the null case
                            cell.Value = string.Empty;
                        }
                    }

                    // This method is called whenever any cell's property changes.
                    this.CellPropertyChanged?.Invoke(sender, e);
                }

                this.CellPropertyChanged?.Invoke(sender, e);
            }
        }

        /// <summary>
        /// Gets the value from the Cell used as a variable.
        /// </summary>
        /// <param name="exprTree">The Constructed ExpressionTree.</param>
        /// <param name="dependentCell">The Cell thats being changed.<param>
        private void UpdateExpressionTreeVariables(ExpressionTree exprTree, Cell dependentCell)
        {
            foreach (var variable in exprTree.GetVariables())
            {
                // Check if cell coordinates are valid.
                if (!this.IsValidCellName(variable))
                {
                    dependentCell.SetError("!(bad reference)");
                    return;
                }

                // Check if cell coordinates is self referencing.
                if (!this.IsSelfReference(variable, dependentCell))
                {
                    dependentCell.SetError("!(self reference)");
                    return;
                }

                if (this.IsCyclic(variable, dependentCell))
                {
                    dependentCell.SetError("!(Circular reference)");
                    var referencedCell = this.GetCell(variable);
                    if (referencedCell != null)
                    {
                        dependentCell.DependsOn.Add(referencedCell);
                        referencedCell.DependsOn.Add(dependentCell);
                    }

                    return;
                }

                Cell? cell = this.GetCell(variable);
                if (cell != null && double.TryParse(cell.Value, out double value))
                {
                    exprTree.SetVariable(variable, value);
                }
                else
                {
                    // Set variable to 0 if the cell is empty or non-existent but was a valid cell name.
                    exprTree.SetVariable(variable, 0.0);
                }
            }
        }

        /// <summary>
        /// Checks if cellName is a valid cell.
        /// </summary>
        /// <param name="cellName">The name of the Cell.</param>
        /// <returns>Boolean if name is valid.</returns>
        private bool IsValidCellName(string cellName)
        {
            if (string.IsNullOrEmpty(cellName) || !char.IsLetter(cellName[0]) || cellName.Length < 2)
            {
                return false;
            }

            int column = cellName[0] - 'A';
            int row;
            if (!int.TryParse(cellName.Substring(1), out row))
            {
                return false;
            }

            row--; // Convert to 0-based index.

            return column >= 0 && column < this.ColumnCount && row >= 0 && row < this.RowCount;
        }

        /// <summary>
        /// Checks if the cellName is a self reference.
        /// </summary>
        /// <param name="cellName">The name of the Cell.</param>
        /// /// <param name="cell">The current Cell.</param>
        /// <returns>Boolean if name is a self reference.</returns>
        private bool IsSelfReference(string cellName, Cell cell)
        {
            if (string.IsNullOrEmpty(cellName) || !char.IsLetter(cellName[0]) || cellName.Length < 2)
            {
                return false;
            }

            int column = cellName[0] - 'A';
            int row = int.Parse(cellName.Substring(1)) - 1;

            if (column == cell.ColumnIndex && row == cell.RowIndex)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if the cellName is a making a circular reference.
        /// </summary>
        /// <param name="newCellName">The name of the Cell that going to be referenced.</param>
        /// /// <param name="cell">The current Cell.</param>
        /// <returns>Boolean if name is a circular reference.</returns>
        private bool IsCyclic(string newCellName, Cell cell)
        {
            Cell? newCell = this.GetCell(newCellName);
            if (newCell == null)
            {
                return false; // Non-existent cell cannot participate in a cycle.
            }

            return this.RecursiveCycle(newCell, cell);
        }

        /// <summary>
        /// A recursive helper funciton for IsCyclic.
        /// </summary>
        /// <param name="targetCell">The name of the Cell that going to be referenced.</param>
        /// /// <param name="currentCell">The current Cell.</param>
        /// <returns>Boolean if name is a circular reference.</returns>
        private bool RecursiveCycle(Cell targetCell, Cell currentCell)
        {
            // If the current cell is the target cell, it's a direct or indirect cycle.
            if (currentCell == targetCell)
            {
                return true;
            }

            // Iterate over each dependent of the current cell.
            foreach (Cell dependent in currentCell.Dependents)
            {
                if (this.RecursiveCycle(targetCell, dependent))
                {
                    return true;
                }
            }

            return false;
        }
    }
}