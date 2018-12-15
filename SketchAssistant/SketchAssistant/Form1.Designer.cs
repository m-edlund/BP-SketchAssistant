namespace SketchAssistant
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.pictureBoxRight = new System.Windows.Forms.PictureBox();
			this.pictureBoxLeft = new System.Windows.Forms.PictureBox();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.examplePictureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.canvasButton = new System.Windows.Forms.ToolStripButton();
			this.drawButton = new System.Windows.Forms.ToolStripButton();
			this.deleteButton = new System.Windows.Forms.ToolStripButton();
			this.undoButton = new System.Windows.Forms.ToolStripButton();
			this.redoButton = new System.Windows.Forms.ToolStripButton();
			this.statusStrip1 = new System.Windows.Forms.StatusStrip();
			this.toolStripLoadStatus = new System.Windows.Forms.ToolStripStatusLabel();
			this.lastActionTakenLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();
			this.mouseTimer = new System.Windows.Forms.Timer(this.components);
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxRight)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxLeft)).BeginInit();
			this.menuStrip1.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this.statusStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.BackColor = System.Drawing.SystemColors.InactiveCaption;
			this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.OutsetDouble;
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.pictureBoxRight, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.pictureBoxLeft, 0, 0);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 52);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(696, 440);
			this.tableLayoutPanel1.TabIndex = 1;
			// 
			// pictureBoxRight
			// 
			this.pictureBoxRight.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBoxRight.Location = new System.Drawing.Point(349, 3);
			this.pictureBoxRight.Margin = new System.Windows.Forms.Padding(0);
			this.pictureBoxRight.Name = "pictureBoxRight";
			this.pictureBoxRight.Size = new System.Drawing.Size(344, 434);
			this.pictureBoxRight.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBoxRight.TabIndex = 6;
			this.pictureBoxRight.TabStop = false;
			this.pictureBoxRight.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBoxRight_MouseDown);
			this.pictureBoxRight.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBoxRight_MouseMove);
			this.pictureBoxRight.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBoxRight_MouseUp);
			// 
			// pictureBoxLeft
			// 
			this.pictureBoxLeft.BackColor = System.Drawing.SystemColors.InactiveCaption;
			this.pictureBoxLeft.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBoxLeft.Location = new System.Drawing.Point(3, 3);
			this.pictureBoxLeft.Margin = new System.Windows.Forms.Padding(0);
			this.pictureBoxLeft.Name = "pictureBoxLeft";
			this.pictureBoxLeft.Size = new System.Drawing.Size(343, 434);
			this.pictureBoxLeft.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
			this.pictureBoxLeft.TabIndex = 5;
			this.pictureBoxLeft.TabStop = false;
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.MaximumSize = new System.Drawing.Size(1000, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(696, 24);
			this.menuStrip1.TabIndex = 2;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadToolStripMenuItem,
            this.importToolStripMenuItem,
            this.saveToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// loadToolStripMenuItem
			// 
			this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
			this.loadToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.loadToolStripMenuItem.Text = "Load...";
			this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
			// 
			// importToolStripMenuItem
			// 
			this.importToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.examplePictureToolStripMenuItem});
			this.importToolStripMenuItem.Name = "importToolStripMenuItem";
			this.importToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.importToolStripMenuItem.Text = "Import...";
			// 
			// examplePictureToolStripMenuItem
			// 
			this.examplePictureToolStripMenuItem.Name = "examplePictureToolStripMenuItem";
			this.examplePictureToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
			this.examplePictureToolStripMenuItem.Text = "Example picture";
			this.examplePictureToolStripMenuItem.Click += new System.EventHandler(this.examplePictureToolStripMenuItem_Click);
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.canvasButton,
            this.drawButton,
            this.deleteButton,
            this.undoButton,
            this.redoButton});
			this.toolStrip1.Location = new System.Drawing.Point(0, 24);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(696, 25);
			this.toolStrip1.TabIndex = 3;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// canvasButton
			// 
			this.canvasButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.canvasButton.Image = ((System.Drawing.Image)(resources.GetObject("canvasButton.Image")));
			this.canvasButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.canvasButton.Name = "canvasButton";
			this.canvasButton.Size = new System.Drawing.Size(76, 22);
			this.canvasButton.Text = "New Canvas";
			this.canvasButton.Click += new System.EventHandler(this.canvasButton_Click);
			// 
			// drawButton
			// 
			this.drawButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.drawButton.Image = ((System.Drawing.Image)(resources.GetObject("drawButton.Image")));
			this.drawButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.drawButton.Name = "drawButton";
			this.drawButton.Size = new System.Drawing.Size(38, 22);
			this.drawButton.Text = "Draw";
			this.drawButton.Click += new System.EventHandler(this.drawButton_Click);
			// 
			// deleteButton
			// 
			this.deleteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.deleteButton.Image = ((System.Drawing.Image)(resources.GetObject("deleteButton.Image")));
			this.deleteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.deleteButton.Name = "deleteButton";
			this.deleteButton.Size = new System.Drawing.Size(44, 22);
			this.deleteButton.Text = "Delete";
			this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
			// 
			// undoButton
			// 
			this.undoButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.undoButton.Image = ((System.Drawing.Image)(resources.GetObject("undoButton.Image")));
			this.undoButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.undoButton.Name = "undoButton";
			this.undoButton.Size = new System.Drawing.Size(40, 22);
			this.undoButton.Text = "Undo";
			this.undoButton.Click += new System.EventHandler(this.undoButton_Click);
			// 
			// redoButton
			// 
			this.redoButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.redoButton.Image = ((System.Drawing.Image)(resources.GetObject("redoButton.Image")));
			this.redoButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.redoButton.Name = "redoButton";
			this.redoButton.Size = new System.Drawing.Size(38, 22);
			this.redoButton.Text = "Redo";
			this.redoButton.Click += new System.EventHandler(this.redoButton_Click);
			// 
			// statusStrip1
			// 
			this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLoadStatus,
            this.lastActionTakenLabel});
			this.statusStrip1.Location = new System.Drawing.Point(0, 491);
			this.statusStrip1.Name = "statusStrip1";
			this.statusStrip1.Size = new System.Drawing.Size(696, 24);
			this.statusStrip1.TabIndex = 4;
			this.statusStrip1.Text = "statusStrip1";
			// 
			// toolStripLoadStatus
			// 
			this.toolStripLoadStatus.Name = "toolStripLoadStatus";
			this.toolStripLoadStatus.Size = new System.Drawing.Size(40, 19);
			this.toolStripLoadStatus.Text = "no file";
			// 
			// lastActionTakenLabel
			// 
			this.lastActionTakenLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
			this.lastActionTakenLabel.Name = "lastActionTakenLabel";
			this.lastActionTakenLabel.Size = new System.Drawing.Size(38, 19);
			this.lastActionTakenLabel.Text = "none";
			this.lastActionTakenLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// mouseTimer
			// 
			this.mouseTimer.Interval = 1;
			this.mouseTimer.Tick += new System.EventHandler(this.mouseTimer_Tick);
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.saveToolStripMenuItem.Text = "Save...";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click_1);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.MenuBar;
			this.ClientSize = new System.Drawing.Size(696, 515);
			this.Controls.Add(this.statusStrip1);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.menuStrip1);
			this.KeyPreview = true;
			this.MainMenuStrip = this.menuStrip1;
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Sketch Assistant";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Load += new System.EventHandler(this.Form1_Load);
			this.SizeChanged += new System.EventHandler(this.Form1_Resize);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
			this.tableLayoutPanel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxRight)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBoxLeft)).EndInit();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.statusStrip1.ResumeLayout(false);
			this.statusStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripLoadStatus;
        private System.ComponentModel.BackgroundWorker backgroundWorker2;
        private System.Windows.Forms.PictureBox pictureBoxRight;
        private System.Windows.Forms.PictureBox pictureBoxLeft;
        private System.Windows.Forms.Timer mouseTimer;
        private System.Windows.Forms.ToolStripButton canvasButton;
        private System.Windows.Forms.ToolStripButton drawButton;
        private System.Windows.Forms.ToolStripButton deleteButton;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem examplePictureToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel lastActionTakenLabel;
        private System.Windows.Forms.ToolStripButton undoButton;
        private System.Windows.Forms.ToolStripButton redoButton;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
	}
}

