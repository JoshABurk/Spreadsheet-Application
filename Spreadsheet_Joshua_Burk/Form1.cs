// <copyright file="Form1.cs" company="Joshua Burk ID: 011731347">
// Copyright (c) Joshua Burk ID: 011731347. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Diagnostics;
using SpreadsheetEngine;

namespace Spreadsheet_Joshua_Burk
{
    /// <summary>
    /// The Main class for the WinForm.
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// The spreadsheet instance.
        /// </summary>
        private readonly SpreadsheetEngine.Spreadsheet spreadsheet;

        /// <summary>
        /// Initializes a new instance of the <see cref="Form1"/> class.
        /// Constructor for Form1. initializes datagrid, creates the spreadsheet, and subsribes to change events.
        /// </summary>
        public Form1()
        {
            this.InitializeComponent();
            this.InitializeDataGrid();

            // initialize the SpreadSheet with 26 columns and 50 rows
            this.spreadsheet = new SpreadsheetEngine.Spreadsheet(50, 26);

            // Subscribe to the CellPropertyChanged event
            this.spreadsheet.CellPropertyChanged += this.Spreadsheet_CellPropertyChanged;

            // Subscribe to DataGridView events for cell editing
            this.dataGridView1.CellBeginEdit += this.DataGridView1_CellBeginEdit;
            this.dataGridView1.CellEndEdit += this.DataGridView1_CellEndEdit;

            // Subscribe to changeBGColorToolStripMenuItem events for cell color editing
            this.changeBGColorToolStripMenuItem.Click += this.ChangeBgColorToolStripMenuItem_Click;

            this.undoToolStripMenuItem.Enabled = false;
            this.redoToolStripMenuItem.Enabled = false;

            this.saveToolStripMenuItem.Click += this.SaveMenuItem_Click; // Create the event handler
            this.loadToolStripMenuItem.Click += this.LoadMenuItem_Click; // Create the event handler
        }

        /// <summary>
        /// Event Handler anytime a cell's text is changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A PropertyChangedEventArgs that contains the event data.</param>
        private void Spreadsheet_CellPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is SpreadsheetEngine.Cell cell)
            {
                // Check if the Value property has changed
                if (e.PropertyName == nameof(SpreadsheetEngine.Cell.Value))
                {
                    this.dataGridView1.Rows[cell.RowIndex].Cells[cell.ColumnIndex].Value = cell.Value;
                }

                // Extend handling for BGColor changes
                else if (e.PropertyName == nameof(SpreadsheetEngine.Cell.BGColor))
                {
                    // Convert the uint BGColor to a System.Drawing.Color object
                    var color = System.Drawing.Color.FromArgb((int)cell.BGColor);
                    this.dataGridView1.Rows[cell.RowIndex].Cells[cell.ColumnIndex].Style.BackColor = color;
                }
            }

            if (this.spreadsheet.GetUndoStackSize() == 0)
            {
                this.undoToolStripMenuItem.Enabled = false;
            }
            else
            {
                this.undoToolStripMenuItem.Text = "Undo " + this.spreadsheet.PeepUndo();
                this.undoToolStripMenuItem.Enabled = true;
            }

            if (this.spreadsheet.GetRedoStackSize() == 0)
            {
                this.redoToolStripMenuItem.Enabled = false;
            }
            else
            {
                this.redoToolStripMenuItem.Text = "Redo " + this.spreadsheet.PeepRedo();
                this.redoToolStripMenuItem.Enabled = true;
            }
        }

        /// <summary>
        /// Handles when a cell in beginning to be editted.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A PropertyChangedEventArgs that contains the event data.</param>
        private void DataGridView1_CellBeginEdit(object? sender, DataGridViewCellCancelEventArgs e)
        {
            var cell = this.spreadsheet.GetCell(e.RowIndex, e.ColumnIndex);
            if (cell != null)
            {
                this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = cell.Text;
            }
        }

        /// <summary>
        /// Handles the event when a cell finishes editing.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A PropertyChangedEventArgs that contains the event data.</param>
        private void DataGridView1_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {
            var editedValue = this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString() ?? string.Empty;
            var cell = this.spreadsheet.GetCell(e.RowIndex, e.ColumnIndex);
            if (cell != null)
            {
                // Update the Text and Value properties.
                if (editedValue != cell.Text)
                {
                    this.spreadsheet.AddUndo(cell.CreateCopy(), "cell text change");
                    this.spreadsheet.ClearRedo();
                }

                cell.Text = editedValue;
                this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = cell.Value;
            }
        }

        /// <summary>
        /// Handles the event when the edit background color menu is clicked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A PropertyChangedEventArgs that contains the event data.</param>
        private void ChangeBgColorToolStripMenuItem_Click(object? sender, EventArgs e)
        {
            if (this.dataGridView1.SelectedCells.Count == 0)
            {
                return;
            }

            // Initialize the color dialog
            using (ColorDialog colorDialog = new ColorDialog())
            {
                // Show the color dialog and check if the user clicked OK
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    // Convert System.Drawing.Color to uint
                    uint colorValue = (uint)((colorDialog.Color.A << 24) | (colorDialog.Color.R << 16) |
                                             (colorDialog.Color.G << 8) | (colorDialog.Color.B << 0));

                    // Apply the selected color to the background of the selected cell(s)
                    foreach (DataGridViewCell cell in this.dataGridView1.SelectedCells)
                    {
                        // Get the corresponding cell from your spreadsheet engine
                        Cell? engineCell = this.spreadsheet.GetCell(cell.RowIndex, cell.ColumnIndex);
                        if (engineCell != null)
                        {
                            this.spreadsheet.AddUndo(engineCell.CreateCopy(), "changing cell background color");
                        }

                        this.spreadsheet.ClearRedo();
                        if (engineCell != null)
                        {
                            // Set the BGColor property, which should trigger the PropertyChanged event
                            engineCell.BGColor = colorValue;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the event when the Undo menu button is clicked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A PropertyChangedEventArgs that contains the event data.</param>
        private void UndoTextChangeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Perform the undo operation
            string? message = this.spreadsheet.ExecuteUndo();
            this.undoToolStripMenuItem.Text = "Undo " + this.spreadsheet.PeepUndo();
        }

        /// <summary>
        /// Handles the event when the Redo menu button is clicked.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A PropertyChangedEventArgs that contains the event data.</param>
        private void RedoTextChangeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Perform the undo operation
            string? message = this.spreadsheet.ExecuteRedo();
            this.redoToolStripMenuItem.Text = "Redo " + this.spreadsheet.PeepRedo();
        }

        /// <summary>
        /// Loads the Form.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A PropertyChangedEventArgs that contains the event data.</param>
        private void Form1_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Saves the Spreadsheet to an Xml File .
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A PropertyChangedEventArgs that contains the event data.</param>
        private void SaveMenuItem_Click(object? sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                // Opens file explorer and filters files for varients of xml.
                saveFileDialog.Filter = "XML Files (*.xml)|*.xml|All files (*.*)|*.*";
                saveFileDialog.DefaultExt = "xml"; // sets default file option to xml.
                saveFileDialog.AddExtension = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // shows the file explorer and sends chosen file to the spreadsheet engine.
                    using (Stream stream = File.Open(saveFileDialog.FileName, FileMode.Create))
                    {
                        this.spreadsheet.Save(stream);
                    }
                }
            }
        }

        /// <summary>
        /// Loads the Xml file chosen.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A PropertyChangedEventArgs that contains the event data.</param>
        private void LoadMenuItem_Click(object? sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // filters file explorer to only show xml files.
                openFileDialog.Filter = "XML Files (*.xml)|*.xml|All files (*.*)|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // shows file explorer and sends chosen file to spreadsheet engine.
                    using (Stream stream = File.Open(openFileDialog.FileName, FileMode.Open))
                    {
                        this.spreadsheet.Load(stream);
                    }
                }
            }
        }
    }
}
