// <copyright file="CellClassTests.cs" company="Joshua Burk ID: 011731347">
// Copyright (c) Joshua Burk ID: 011731347. All rights reserved.
// </copyright>

using NUnit.Framework;
using SpreadsheetEngine;

namespace SpreadsheetTests
{
    /// <summary>
    /// Tests for the Cell Class.
    /// </summary>
    public class CellClassTests
    {
        /// <summary>
        /// Tests the behavior of the construction of the ExpressionTree when
        /// an invalid expression with mismatched parenthesis is given.
        /// </summary>
        [Test]
        public void PropertyChanged_Fires_On_Text_Update()
        {
            var cell = new ConcreteCell(0, 0);
            bool eventFired = false;
            cell.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(cell.Text))
                {
                    eventFired = true;
                }
            };

            cell.Text = "New Value";
            Assert.That(eventFired, Is.True, "PropertyChanged event did not fire for Text property update.");
        }

        /// <summary>
        /// Tests that the value of the cell is not different from text when the text
        /// doesnt start with '='.
        /// </summary>
        [Test]
        public void Value_Equals_Text_When_Not_Starting_With_Equal_Sign()
        {
            var cell = new ConcreteCell(0, 0)
            {
                Text = "Sample Text",
            };
            Assert.That(cell.Value, Is.EqualTo(cell.Text), "Value does not equal Text when Text does not start with '='.");
        }

        /// <summary>
        /// Tests that the Cell constructor works properly.
        [Test]
        public void Constructor_Initializes_Properties_Correctly()
        {
            int rowIndex = 5;
            int columnIndex = 10;
            var cell = new ConcreteCell(rowIndex, columnIndex);
            Assert.Multiple(() =>
            {
                Assert.That(cell.RowIndex, Is.EqualTo(rowIndex), "RowIndex not initialized correctly.");
                Assert.That(cell.ColumnIndex, Is.EqualTo(columnIndex), "ColumnIndex not initialized correctly.");
            });
        }

        /// <summary>
        /// that a Cell's text can be set to null.
        /// </summary>
        [Test]
        public void Setting_Text_To_Null_Handles_Correctly()
        {
            var cell = new ConcreteCell(0, 0)
            {
                Text = null,
            };

            Assert.That(cell.Text, Is.EqualTo(string.Empty), "Text property does not handle null correctly.");
        }

        /// <summary>
        /// Tests that if the same value is entered into a cell it does not fire the propertyChanged event.
        /// </summary>
        // [Test]
        // public void Repeated_Value_Does_Not_Fire_PropertyChanged()
        // {
        //    var cell = new ConcreteCell(0, 0);
        //    bool eventFired = false;
        //    cell.Text = "Initial Value";

        // cell.PropertyChanged += (sender, e) => eventFired = true;
        //    cell.Text = "Initial Value";

        // Assert.That(eventFired, Is.False, "PropertyChanged event fired on setting Text to the same value.");
        // }
    }
}