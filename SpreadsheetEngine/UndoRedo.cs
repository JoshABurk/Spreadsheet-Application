// <copyright file="UndoRedo.cs" company="Joshua Burk ID: 011731347">
// Copyright (c) Joshua Burk ID: 011731347. All rights reserved.
// </copyright>

using System.ComponentModel;

namespace SpreadsheetEngine
{
    /// <summary>
    /// Class for the Spreadsheet.
    /// </summary>
    public class UndoRedo
    {
        /// <summary>
        /// Cell that was changed.
        /// </summary>
        private Cell oldCell;

        /// <summary>
        /// Description of what was changed about it.
        /// </summary>
        private string changedPropertyName;

        /// <summary>
        /// Initializes a new instance of the <see cref="UndoRedo"/> class.
        /// </summary>
        /// <param name="description">What changed.</param>
        /// <param name="undoRedoCell">The cell that was changed.</param>
        public UndoRedo(Cell undoRedoCell, string description = "")
        {
            this.oldCell = undoRedoCell;
            this.changedPropertyName = description;
        }

        /// <summary>
        /// returns the old cell.
        /// </summary>
        /// <returns>The Cell.</returns>
        public Cell GetOldCell()
        {
            return this.oldCell;
        }

        /// <summary>
        /// returns the old cell.
        /// </summary>
        /// <returns>The Cell.</returns>
        public string GetDescription()
        {
            return this.changedPropertyName;
        }

        /// <summary>
        /// get the coordinates of the undo cell.
        /// </summary>
        /// <returns>indices of the cell.</returns>
        public Tuple<int, int> GetCoordinates()
        {
            return new Tuple<int, int>(this.oldCell.RowIndex, this.oldCell.ColumnIndex);
        }
    }
}
