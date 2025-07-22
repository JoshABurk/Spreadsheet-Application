// <copyright file="Cell.cs" company="Joshua Burk ID: 011731347">
// Copyright (c) Joshua Burk ID: 011731347. All rights reserved.
// </copyright>

using System.ComponentModel;
using System.Data;

namespace SpreadsheetEngine
{
    /// <summary>
    /// The Cell Class.
    /// </summary>
    public abstract class Cell : INotifyPropertyChanged
    {
        /// <summary>
        /// The Text of the cell.
        /// </summary>
        private string? text;

        /// <summary>
        /// The Value of the cell.
        /// </summary>
        private string? value;

        /// <summary>
        /// The color of the cell defualted to white.
        /// </summary>
        private uint bgColor;

        /// <summary>
        /// Initializes a new instance of the <see cref="Cell"/> class.
        /// </summary>
        /// <param name="row">The index of the row.</param>
        /// <param name="column">The index of the column.</param>
        protected Cell(int row = 0, int column = 0)
        {
            this.RowIndex = row;
            this.ColumnIndex = column;
            this.text = string.Empty;
            this.value = string.Empty;
            this.bgColor = 0xFFFFFFFF;
        }

        /// <summary>
        ///  The Event Handler.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets or sets property to hold dependencies.
        /// </summary>
        public List<Cell> Dependents { get; set; } = new List<Cell>();

        /// <summary>
        /// Gets or sets property to hold dependencies.
        /// </summary>
        public List<Cell> DependsOn { get; set; } = new List<Cell>();

        /// <summary>
        /// Gets or sets the row index of the cell.
        /// </summary>
        public int RowIndex { get; set; }

        /// <summary>
        /// Gets or sets the column index of the cell.
        /// </summary>
        public int ColumnIndex { get; set; }

        /// <summary>
        /// Gets the error message of the cell.
        /// </summary>
        public string? ErrorMessage { get; private set; }

        /// <summary>
        /// Gets or sets property for the text that is typed in the cell.
        /// </summary>
        public string? Text
        {
            get
            {
                return this.text;
            }

            set
            {
                this.text = value ?? string.Empty;
                this.OnPropertyChanged(nameof(this.Text));
            }
        }

        /// <summary>
        /// Gets property for the result of the evaluated text in the cell.
        /// (This is just text, unless the first character is '=')
        /// Only allows the Spreadsheet Class to set value.
        /// </summary>
        public string? Value
        {
            get
            {
                if (!string.IsNullOrEmpty(this.ErrorMessage))
                {
                    return this.ErrorMessage; // Return the error message if there is one.
                }

                if (!string.IsNullOrEmpty(this.text) && this.text[0] == '=')
                {
                    return this.value;
                }
                else
                {
                    return this.text;
                }
            }

            internal set
            {
                if (Environment.StackTrace.Contains("Spreadsheet"))
                {
                    this.ErrorMessage = null; // Clear any existing error message when a new value is set.
                    this.value = value;
                    this.OnPropertyChanged(nameof(this.Value));
                }
                else
                {
                    throw new MemberAccessException("Only the Spreadsheet class can access this property.");
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the cell and invokes property changed event.
        /// </summary>
        public uint BGColor
        {
            get => this.bgColor;
            set
            {
                if (this.bgColor != value)
                {
                    this.bgColor = value;
                    this.OnPropertyChanged(nameof(this.BGColor));
                }
            }
        }

        /// <summary>
        /// Causes an event to triger making the cells recalculate their value.
        /// </summary>
        public void RecalculateValue()
        {
            this.Text = this.Text;
        }

        /// <summary>
        /// Creates a copy of a cell.
        /// </summary>
        /// <returns>the copied cell.</returns>
        public Cell CreateCopy()
        {
            Cell newCell = new ConcreteCell
            {
                Text = this.Text,
                BGColor = this.BGColor,
                RowIndex = this.RowIndex,
                ColumnIndex = this.ColumnIndex,
            };

            return newCell;
        }

        /// <summary>
        /// Sets the error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        internal void SetError(string? message)
        {
            this.ErrorMessage = message;
            this.OnPropertyChanged(nameof(this.Value));
        }

        /// <summary>
        /// Raises PropertyChanged event when a Cell's property changes (Text or Value).
        /// </summary>
        /// <param name="name">The name of the property that has changed.</param>
        protected virtual void OnPropertyChanged(string name)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            if (name == "Value")
            {
                var dependentsCopy = new List<Cell>(this.Dependents);
                foreach (var dependent in dependentsCopy)
                {
                    dependent.RecalculateValue();
                }
            }
        }
    }
}