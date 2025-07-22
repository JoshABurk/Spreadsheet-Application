// <copyright file="ConcreteCell.cs" company="Joshua Burk ID: 011731347">
// Copyright (c) Joshua Burk ID: 011731347. All rights reserved.
// </copyright>

namespace SpreadsheetEngine
{
    /// <summary>
    /// The ConcreteCell class.
    /// </summary>
    public class ConcreteCell : Cell
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConcreteCell"/> class.
        /// </summary>
        /// <param name="row">The row index of the cell.</param>
        /// <param name="column">The column index of the cell.</param>
        public ConcreteCell(int row = 0, int column = 0)
            : base(row, column)
        {
        }
    }
}
