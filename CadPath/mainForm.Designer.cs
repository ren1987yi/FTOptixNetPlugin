namespace CadPath
{
    partial class mainForm
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
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            openDwgToolStripMenuItem = new ToolStripMenuItem();
            splitContainer1 = new SplitContainer();
            tableLayoutPanel1 = new TableLayoutPanel();
            label1 = new Label();
            txtZoom = new TextBox();
            btnZoom = new Button();
            label2 = new Label();
            dvPath = new DataGridView();
            Column1 = new DataGridViewTextBoxColumn();
            Column2 = new DataGridViewTextBoxColumn();
            Column3 = new DataGridViewTextBoxColumn();
            flowLayoutPanel1 = new FlowLayoutPanel();
            btnSortLine = new Button();
            dvPoints = new DataGridView();
            Column4 = new DataGridViewTextBoxColumn();
            Column5 = new DataGridViewTextBoxColumn();
            btnDownload = new Button();
            btnZoomOut = new Button();
            btnZoomIn = new Button();
            cadView = new PictureBox();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dvPath).BeginInit();
            flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dvPoints).BeginInit();
            ((System.ComponentModel.ISupportInitialize)cadView).BeginInit();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1264, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { openDwgToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // openDwgToolStripMenuItem
            // 
            openDwgToolStripMenuItem.Name = "openDwgToolStripMenuItem";
            openDwgToolStripMenuItem.Size = new Size(130, 22);
            openDwgToolStripMenuItem.Text = "Open Dwg";
            openDwgToolStripMenuItem.Click += openDwgToolStripMenuItem_Click;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 24);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(tableLayoutPanel1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(btnZoomOut);
            splitContainer1.Panel2.Controls.Add(btnZoomIn);
            splitContainer1.Panel2.Controls.Add(cadView);
            splitContainer1.Size = new Size(1264, 737);
            splitContainer1.SplitterDistance = 421;
            splitContainer1.TabIndex = 1;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25.89074F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 74.10926F));
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(txtZoom, 0, 1);
            tableLayoutPanel1.Controls.Add(btnZoom, 1, 1);
            tableLayoutPanel1.Controls.Add(label2, 0, 2);
            tableLayoutPanel1.Controls.Add(dvPath, 0, 3);
            tableLayoutPanel1.Controls.Add(flowLayoutPanel1, 1, 4);
            tableLayoutPanel1.Controls.Add(dvPoints, 0, 6);
            tableLayoutPanel1.Controls.Add(btnDownload, 1, 7);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 10;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 19F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 41F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.RowStyles.Add(new RowStyle());
            tableLayoutPanel1.Size = new Size(421, 737);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(39, 15);
            label1.TabIndex = 0;
            label1.Text = "Zoom";
            // 
            // txtZoom
            // 
            txtZoom.Location = new Point(3, 22);
            txtZoom.Name = "txtZoom";
            txtZoom.Size = new Size(103, 23);
            txtZoom.TabIndex = 1;
            txtZoom.Text = "100";
            // 
            // btnZoom
            // 
            btnZoom.Location = new Point(112, 22);
            btnZoom.Name = "btnZoom";
            btnZoom.Size = new Size(75, 23);
            btnZoom.TabIndex = 2;
            btnZoom.Text = "OK";
            btnZoom.UseVisualStyleBackColor = true;
            btnZoom.Click += btnZoom_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(3, 60);
            label2.Name = "label2";
            label2.Size = new Size(35, 15);
            label2.TabIndex = 4;
            label2.Text = "Layer";
            // 
            // dvPath
            // 
            dvPath.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dvPath.Columns.AddRange(new DataGridViewColumn[] { Column1, Column2, Column3 });
            tableLayoutPanel1.SetColumnSpan(dvPath, 2);
            dvPath.Dock = DockStyle.Fill;
            dvPath.Location = new Point(3, 103);
            dvPath.Name = "dvPath";
            dvPath.Size = new Size(415, 252);
            dvPath.TabIndex = 5;
            // 
            // Column1
            // 
            Column1.DataPropertyName = "Name";
            Column1.HeaderText = "图层";
            Column1.Name = "Column1";
            Column1.ReadOnly = true;
            // 
            // Column2
            // 
            Column2.DataPropertyName = "Index";
            Column2.HeaderText = "运行顺序";
            Column2.Name = "Column2";
            // 
            // Column3
            // 
            Column3.DataPropertyName = "Count";
            Column3.HeaderText = "运行次数";
            Column3.Name = "Column3";
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(btnSortLine);
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.Location = new Point(112, 361);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(306, 34);
            flowLayoutPanel1.TabIndex = 6;
            // 
            // btnSortLine
            // 
            btnSortLine.Location = new Point(3, 3);
            btnSortLine.Name = "btnSortLine";
            btnSortLine.Size = new Size(75, 23);
            btnSortLine.TabIndex = 0;
            btnSortLine.Text = "排序";
            btnSortLine.UseVisualStyleBackColor = true;
            btnSortLine.Click += btnSortLine_Click;
            // 
            // dvPoints
            // 
            dvPoints.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dvPoints.Columns.AddRange(new DataGridViewColumn[] { Column4, Column5 });
            tableLayoutPanel1.SetColumnSpan(dvPoints, 2);
            dvPoints.Dock = DockStyle.Fill;
            dvPoints.Location = new Point(3, 441);
            dvPoints.Name = "dvPoints";
            dvPoints.Size = new Size(415, 252);
            dvPoints.TabIndex = 7;
            // 
            // Column4
            // 
            Column4.DataPropertyName = "X";
            Column4.HeaderText = "X";
            Column4.Name = "Column4";
            Column4.ReadOnly = true;
            // 
            // Column5
            // 
            Column5.DataPropertyName = "Y";
            Column5.HeaderText = "Y";
            Column5.Name = "Column5";
            Column5.ReadOnly = true;
            // 
            // btnDownload
            // 
            btnDownload.Location = new Point(112, 699);
            btnDownload.Name = "btnDownload";
            btnDownload.Size = new Size(75, 23);
            btnDownload.TabIndex = 8;
            btnDownload.Text = "Download";
            btnDownload.UseVisualStyleBackColor = true;
            btnDownload.Click += btnDownload_Click;
            // 
            // btnZoomOut
            // 
            btnZoomOut.Location = new Point(84, 3);
            btnZoomOut.Name = "btnZoomOut";
            btnZoomOut.Size = new Size(75, 23);
            btnZoomOut.TabIndex = 2;
            btnZoomOut.Text = "-";
            btnZoomOut.UseVisualStyleBackColor = true;
            btnZoomOut.Click += btnZoomOut_Click;
            // 
            // btnZoomIn
            // 
            btnZoomIn.Location = new Point(3, 3);
            btnZoomIn.Name = "btnZoomIn";
            btnZoomIn.Size = new Size(75, 23);
            btnZoomIn.TabIndex = 1;
            btnZoomIn.Text = "+";
            btnZoomIn.UseVisualStyleBackColor = true;
            btnZoomIn.Click += btnZoomIn_Click;
            // 
            // cadView
            // 
            cadView.Dock = DockStyle.Fill;
            cadView.Location = new Point(0, 0);
            cadView.Name = "cadView";
            cadView.Size = new Size(839, 737);
            cadView.TabIndex = 0;
            cadView.TabStop = false;
            cadView.Paint += cadView_OnPaint;
            cadView.Resize += cadView_Resize;
            // 
            // mainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1264, 761);
            Controls.Add(splitContainer1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "mainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "CAD Path";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dvPath).EndInit();
            flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dvPoints).EndInit();
            ((System.ComponentModel.ISupportInitialize)cadView).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openDwgToolStripMenuItem;
        private SplitContainer splitContainer1;
        private TableLayoutPanel tableLayoutPanel1;
        private PictureBox cadView;
        private Label label1;
        private TextBox txtZoom;
        private Button btnZoom;
        private Button btnZoomOut;
        private Button btnZoomIn;
        private Label label2;
        private DataGridView dvPath;
        private FlowLayoutPanel flowLayoutPanel1;
        private Button btnSortLine;
        private DataGridView dvPoints;
        private DataGridViewTextBoxColumn Column4;
        private DataGridViewTextBoxColumn Column5;
        private DataGridViewTextBoxColumn Column1;
        private DataGridViewTextBoxColumn Column2;
        private DataGridViewTextBoxColumn Column3;
        private Button btnDownload;
    }
}
