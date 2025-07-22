// <copyright file="SpreadsheetClassTests.cs" company="Joshua Burk ID: 011731347">
// Copyright (c) Joshua Burk ID: 011731347. All rights reserved.
// </copyright>

using NUnit.Framework;
using NUnit.Framework.Legacy;
using SpreadsheetEngine;

namespace SpreadsheetTests
{
    /// <summary>
    /// Tests for the Spreadsheet Class.
    /// </summary>
    public class SpreadsheetClassTests
    {
        /// <summary>
        /// A Spreadsheet object.
        /// </summary>
        private Spreadsheet spreadsheet;

        /// <summary>
        /// Constructs a Spreadsheet to test on.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            // Assuming your Spreadsheet class takes two parameters: rows and columns
            this.spreadsheet = new Spreadsheet(50, 26);
        }

        /// <summary>
        /// Tests that if the text doesn't start with '=' then value equals text.
        /// </summary>
        [Test]
        public void TextUpdatePropagatesToValue()
        {
            var cell = this.spreadsheet.GetCell(0, 0);
            if (cell != null)
            {
                string expectedText = "Hello, World!";
                cell.Text = expectedText;
                Assert.That(cell.Value, Is.EqualTo(expectedText), "Cell value did not update to match text.");
            }
            else
            {
                Assert.Fail("Expected cell to be non-null.");
            }
        }

        /// <summary>
        /// Tests that the event fires when text is updated.
        /// </summary>
        [Test]
        public void PropertyChangedEventFiresOnTextUpdate()
        {
            var cell = this.spreadsheet.GetCell(0, 0);
            bool eventFired = false;
            if (cell != null)
            {
                cell.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == "Value")
                    {
                        eventFired = true;
                    }
                };
                cell.Text = "New Value";
            }

            Assert.That(eventFired, Is.True, "PropertyChanged event for 'Value' did not fire after updating Text.");
        }

        /// <summary>
        /// Tests that trying to access a cell that isnt in the bounds of the spreedsheet returns null.
        /// </summary>
        [Test]
        public void AccessingCellsOutsideRangeReturnsNull()
        {
            var cell = this.spreadsheet.GetCell(100, 100);
            Assert.That(cell, Is.Null, "Accessing a cell outside of the spreadsheet range should return null.");
        }

        /// <summary>
        /// Tests that updating a cell's value updates the dependent cell's value where the dependent cell contains an expression referencing the first cell.
        /// </summary>
        [Test]
        public void DependencyValuePropagationTest()
        {
            var sourceCell = this.spreadsheet.GetCell(0, 0);
            var dependentCell = this.spreadsheet.GetCell(1, 0);
            if (sourceCell != null && dependentCell != null)
            {
                sourceCell.Text = "5";
                dependentCell.Text = "=A1*2";
                Assert.That(dependentCell.Value, Is.EqualTo("10"), "Dependent cell value did not update based on expression.");

                // Update the source cell to see if the dependent cell updates correctly
                sourceCell.Text = "10";
                Assert.That(dependentCell.Value, Is.EqualTo("20"), "Dependent cell value did not update after source cell value changed.");
            }
            else
            {
                Assert.Fail("Expected cells to be non-null.");
            }
        }

        /// <summary>
        /// Tests that a cell with an expression correctly evaluates based on the current values of the cells it references.
        /// </summary>
        [Test]
        public void ExpressionEvaluationTest()
        {
            var cellA = this.spreadsheet.GetCell(0, 0); // A1
            var cellB = this.spreadsheet.GetCell(0, 1); // B1
            var resultCell = this.spreadsheet.GetCell(0, 2); // C1

            if (cellA != null && cellB != null && resultCell != null)
            {
                cellA.Text = "10";
                cellB.Text = "20";
                resultCell.Text = "=A1+B1";

                Assert.That(resultCell.Value, Is.EqualTo("30"), "Expression in cell did not evaluate correctly.");
            }
            else
            {
                Assert.Fail("Expected cells to be non-null.");
            }
        }

        /// <summary>
        /// Tests the Undo and Redo systems.
        /// </summary>
        [Test]
        public void UndoRedoStackManagementTest()
        {
            // Get a cell to test.
            Cell? cell = this.spreadsheet.GetCell(0, 0);

            // Capture the initial state of the cell.
            string initialText = "Initial";
            cell!.Text = initialText; // Initial setting does not need undo.

            // Prepare to change text by first capturing the state for undo.
            this.spreadsheet.AddUndo(cell.CreateCopy()); // Capture state before change.
            string newText = "Changed";
            cell.Text = newText; // Make the change.

            // Assert changes were made.
            Assert.That(cell.Text, Is.EqualTo(newText), "Cell text did not update to new text.");

            // Undo the changes.
            this.spreadsheet.ExecuteUndo();

            // Assert that the changes were undone.
            Assert.That(cell.Text, Is.EqualTo(initialText), "Undo did not revert the cell's text as expected.");

            // Redo the changes.
            this.spreadsheet.ExecuteRedo();

            // Assert that the changes were redone.
            Assert.That(cell.Text, Is.EqualTo(newText), "Redo did not reapply the new text as expected.");
        }

        /// <summary>
        /// Tests Changing the Cell Background and can be undo'd.
        /// </summary>
        [Test]
        public void ChangeCellBackgroundColorTest()
        {
            // Test changing a cell's background color.
            var cell = this.spreadsheet.GetCell(1, 1); // B2
            if (cell == null)
            {
                Assert.Fail("Cell B2 should not be null.");
                return;
            }

            uint originalColor = cell.BGColor; // Should be white by default
            uint newColor = 0xFF00FF00; // Green

            // Change the cell's background color
            this.spreadsheet.AddUndo(cell.CreateCopy());
            cell.BGColor = newColor;

            // Check if the color was changed
            Assert.That(cell.BGColor, Is.EqualTo(newColor), "Cell background color did not change as expected.");

            // Undo the change
            this.spreadsheet.ExecuteUndo();

            // Check if the color was reverted
            Assert.That(cell.BGColor, Is.EqualTo(originalColor), "Undo did not revert the cell's background color as expected.");
        }

        /// <summary>
        /// Tests the Save Functionality.
        /// </summary>
        [Test]
        public void SaveMethodWritesCorrectXml()
        {
            // Set up a few cells with values and background colors
            var cellA1 = this.spreadsheet.GetCell(0, 0);
            var cellB1 = this.spreadsheet.GetCell(0, 1);
            if (cellA1 == null || cellB1 == null)
            {
                Assert.Fail("Test setup error: Cells A1 or B1 are null.");
                return; // Early exit to prevent null dereference
            }

            cellA1.Text = "Hello";
            cellB1.BGColor = 0xFF00FF00; // Green

            using (var ms = new MemoryStream())
            {
                this.spreadsheet.Save(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var reader = new StreamReader(ms);
                string xmlContent = reader.ReadToEnd();

                // Check if the content of the XML contains the expected values
                Assert.That(xmlContent, Does.Contain("<cell name=\"A1\">"));
                Assert.That(xmlContent, Does.Contain("<text>Hello</text>"));
                Assert.That(xmlContent, Does.Contain("<cell name=\"B1\">"));
                Assert.That(xmlContent, Does.Contain("<bgcolor>FF00FF00</bgcolor>"));
            }
        }

        /// <summary>
        /// Tests the Load Functionality.
        /// </summary>
        [Test]
        public void LoadMethodReadsXmlCorrectly()
        {
            // Prepare XML content simulating a saved file
            string xmlContent = @"<spreadsheet>
                                    <cell name=""A1""><text>Hello</text></cell>
                                    <cell name=""B1""><bgcolor>FF00FF00</bgcolor></cell>
                                  </spreadsheet>";

            uint originalColor = 0xFF00FF00; // Green

            using (var ms = new MemoryStream())
            {
                var writer = new StreamWriter(ms);
                writer.Write(xmlContent);
                writer.Flush();
                ms.Seek(0, SeekOrigin.Begin);

                this.spreadsheet.Load(ms);
            }

            // Verify the cells are loaded correctly
            var cellA1 = this.spreadsheet.GetCell(0, 0);
            var cellB1 = this.spreadsheet.GetCell(0, 1);
            if (cellA1 == null || cellB1 == null)
            {
                Assert.Fail("Test setup error: Cells A1 or B1 are null.");
                return; // Early exit to prevent null dereference
            }

            Assert.That(cellA1.Text, Is.EqualTo("Hello"), "Undo did not revert the cell's text as expected.");

            Assert.That(cellB1.BGColor, Is.EqualTo(originalColor), "Undo did not revert the cell's background color as expected.");
        }

        /// <summary>
        /// Tests an invalid cell reference that does not exist in the spreadsheet.
        /// </summary>
        [Test]
        public void HandleInvalidCellReferenceTest()
        {
            var cell = this.spreadsheet.GetCell(0, 0); // A1
            if (cell == null)
            {
                Assert.Fail("Expected cell A1 to be non-null.");
                return;
            }

            cell.Text = "=Z100"; // Reference to a non-existent cell
            Assert.That(cell.Value, Is.EqualTo("!(bad reference)"), "Cell did not handle invalid reference correctly.");
        }

        /// <summary>
        /// Tests a cell referencing itself in an expression.
        /// </summary>
        [Test]
        public void SelfReferencingCellTest()
        {
            var cell = this.spreadsheet.GetCell(0, 0); // A1
            if (cell == null)
            {
                Assert.Fail("Expected cell A1 to be non-null.");
                return;
            }

            cell.Text = "=A1"; // Self-reference
            Assert.That(cell.Value, Is.EqualTo("!(self reference)"), "Cell did not handle self-reference correctly.");
        }

        /// <summary>
        /// Tests circular reference between cells.
        /// </summary>
        [Test]
        public void CircularReferenceTest()
        {
            var cellA1 = this.spreadsheet.GetCell(0, 0); // A1
            var cellA2 = this.spreadsheet.GetCell(1, 0); // A2

            if (cellA1 == null || cellA2 == null)
            {
                Assert.Fail("Expected cells A1 and A2 to be non-null.");
                return;
            }

            cellA1.Text = "=A2"; // A1 depends on A2
            cellA2.Text = "=A1"; // A2 depends on A1, creating a circular reference

            // A2 should identify the circular reference and set an error message.
            Assert.That(cellA2.Value, Is.EqualTo("!(Circular reference)"), "Cell A2 did not handle circular reference correctly.");
        }
    }
}
