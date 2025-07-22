using System.Windows.Forms;

namespace Spreadsheet_Joshua_Burk
{
    /// <summary>
    ///  The Class for the WinForm Form1.
    /// </summary>
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            dataGridView1 = new DataGridView();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            editToolStripMenuItem = new ToolStripMenuItem();
            undoToolStripMenuItem = new ToolStripMenuItem();
            redoToolStripMenuItem = new ToolStripMenuItem();
            cellToolStripMenuItem = new ToolStripMenuItem();
            changeBGColorToolStripMenuItem = new ToolStripMenuItem();
            saveToolStripMenuItem = new ToolStripMenuItem();
            loadToolStripMenuItem = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(12, 26);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(813, 529);
            dataGridView1.TabIndex = 0;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, cellToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(837, 24);
            menuStrip1.TabIndex = 2;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { saveToolStripMenuItem, loadToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { undoToolStripMenuItem, redoToolStripMenuItem });
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new Size(39, 20);
            editToolStripMenuItem.Text = "Edit";
            // 
            // undoToolStripMenuItem
            // 
            undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            undoToolStripMenuItem.Size = new Size(103, 22);
            undoToolStripMenuItem.Text = "Undo";
            undoToolStripMenuItem.Click += UndoTextChangeToolStripMenuItem_Click;
            // 
            // redoToolStripMenuItem
            // 
            redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            redoToolStripMenuItem.Size = new Size(103, 22);
            redoToolStripMenuItem.Text = "Redo";
            redoToolStripMenuItem.Click += RedoTextChangeToolStripMenuItem_Click;
            // 
            // cellToolStripMenuItem
            // 
            cellToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { changeBGColorToolStripMenuItem });
            cellToolStripMenuItem.Name = "cellToolStripMenuItem";
            cellToolStripMenuItem.Size = new Size(39, 20);
            cellToolStripMenuItem.Text = "Cell";
            // 
            // changeBGColorToolStripMenuItem
            // 
            changeBGColorToolStripMenuItem.Name = "changeBGColorToolStripMenuItem";
            changeBGColorToolStripMenuItem.Size = new Size(165, 22);
            changeBGColorToolStripMenuItem.Text = "Change BG Color";
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.Size = new Size(180, 22);
            saveToolStripMenuItem.Text = "Save";
            // 
            // loadToolStripMenuItem
            // 
            loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            loadToolStripMenuItem.Size = new Size(180, 22);
            loadToolStripMenuItem.Text = "Load";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(837, 567);
            Controls.Add(dataGridView1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            Click += UndoTextChangeToolStripMenuItem_Click;
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        /// <summary>
        ///  Initializes the dataGridView1 to have columns A-Z and 50 rows
        /// </summary>
        private void InitializeDataGrid()
        {
            // clears all columns
            dataGridView1.Columns.Clear();

            // Iterates through A-Z using their ascii values
            for (int i = 65; i <= 90; i++)
            {
                // Convert ascii to char
                char columnLetter = (char) i;

                // Convert the char to a string
                string columnName = columnLetter.ToString();

                // Add a new column with the letter as both the column's name and header text
                dataGridView1.Columns.Add(columnName, columnName);
            }

            // clears all rows
            dataGridView1.Rows.Clear();

            // Iterates through 1-50
            for (int i = 1; i <= 50; i++)
            {
                // Add a new row
                dataGridView1.Rows.Add();

                // Add the row number to the header
                dataGridView1.Rows[i-1].HeaderCell.Value = i.ToString();
            }

            // increase row width manually so that all numbers can be seen
            // for some reason I couldn't get auto resizing to work.
            dataGridView1.RowHeadersWidth = 50;
        }

        #endregion

        private DataGridView dataGridView1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem cellToolStripMenuItem;
        private ToolStripMenuItem changeBGColorToolStripMenuItem;
        private ToolStripMenuItem undoToolStripMenuItem;
        private ToolStripMenuItem redoToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem loadToolStripMenuItem;
    }
}
