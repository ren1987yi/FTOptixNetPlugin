namespace CADImportConfigure
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
            mainMenu = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            MenuItem_NewProject = new ToolStripMenuItem();
            MenuItem_OpenProject = new ToolStripMenuItem();
            openToolStripMenuItem = new ToolStripMenuItem();
            MenuItem_OpenDxf = new ToolStripMenuItem();
            MenuItem_OpenDwg = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            MenuItem_SaveProject = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            closeToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            mainStatusStrip = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            lbCadFilePath = new ToolStripStatusLabel();
            toolStripStatusLabel2 = new ToolStripStatusLabel();
            lbProjectPath = new ToolStripStatusLabel();
            tableLayoutPanel1 = new TableLayoutPanel();
            label1 = new Label();
            label2 = new Label();
            dgBlocks = new DataGridView();
            Column1 = new DataGridViewTextBoxColumn();
            Column2 = new DataGridViewTextBoxColumn();
            dgEntites = new DataGridView();
            Column3 = new DataGridViewTextBoxColumn();
            Column4 = new DataGridViewTextBoxColumn();
            label3 = new Label();
            label4 = new Label();
            flowLayoutPanel1 = new FlowLayoutPanel();
            label5 = new Label();
            txtXmin = new TextBox();
            label6 = new Label();
            txtYmin = new TextBox();
            label7 = new Label();
            txtXmax = new TextBox();
            label8 = new Label();
            txtYmax = new TextBox();
            chbIgnoreNoBindBlock = new CheckBox();
            chbLine = new CheckBox();
            chbText = new CheckBox();
            chbBlock = new CheckBox();
            mainMenu.SuspendLayout();
            mainStatusStrip.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgBlocks).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgEntites).BeginInit();
            flowLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // mainMenu
            // 
            mainMenu.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
            mainMenu.Location = new Point(0, 0);
            mainMenu.Name = "mainMenu";
            mainMenu.Size = new Size(1264, 24);
            mainMenu.TabIndex = 0;
            mainMenu.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { MenuItem_NewProject, MenuItem_OpenProject, openToolStripMenuItem, toolStripSeparator3, MenuItem_SaveProject, toolStripSeparator1, closeToolStripMenuItem, toolStripSeparator2, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "&File";
            // 
            // MenuItem_NewProject
            // 
            MenuItem_NewProject.Name = "MenuItem_NewProject";
            MenuItem_NewProject.Size = new Size(143, 22);
            MenuItem_NewProject.Text = "New";
            // 
            // MenuItem_OpenProject
            // 
            MenuItem_OpenProject.Name = "MenuItem_OpenProject";
            MenuItem_OpenProject.Size = new Size(143, 22);
            MenuItem_OpenProject.Text = "Open Project";
            MenuItem_OpenProject.Click += MenuItem_OpenProject_Click;
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { MenuItem_OpenDxf, MenuItem_OpenDwg });
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.Size = new Size(143, 22);
            openToolStripMenuItem.Text = "Open CAD";
            // 
            // MenuItem_OpenDxf
            // 
            MenuItem_OpenDxf.Name = "MenuItem_OpenDxf";
            MenuItem_OpenDxf.Size = new Size(122, 22);
            MenuItem_OpenDxf.Text = "DXF File";
            MenuItem_OpenDxf.Click += MenuItem_OpenDxf_Click;
            // 
            // MenuItem_OpenDwg
            // 
            MenuItem_OpenDwg.Name = "MenuItem_OpenDwg";
            MenuItem_OpenDwg.Size = new Size(122, 22);
            MenuItem_OpenDwg.Text = "DWG File";
            MenuItem_OpenDwg.Click += MenuItem_OpenDwg_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(140, 6);
            // 
            // MenuItem_SaveProject
            // 
            MenuItem_SaveProject.Name = "MenuItem_SaveProject";
            MenuItem_SaveProject.Size = new Size(143, 22);
            MenuItem_SaveProject.Text = "&Save Project";
            MenuItem_SaveProject.Click += MenuItem_SaveProject_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(140, 6);
            // 
            // closeToolStripMenuItem
            // 
            closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            closeToolStripMenuItem.Size = new Size(143, 22);
            closeToolStripMenuItem.Text = "Close";
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(140, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(143, 22);
            exitToolStripMenuItem.Text = "&Exit";
            // 
            // mainStatusStrip
            // 
            mainStatusStrip.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1, lbCadFilePath, toolStripStatusLabel2, lbProjectPath });
            mainStatusStrip.Location = new Point(0, 739);
            mainStatusStrip.Name = "mainStatusStrip";
            mainStatusStrip.Size = new Size(1264, 22);
            mainStatusStrip.TabIndex = 1;
            mainStatusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(55, 17);
            toolStripStatusLabel1.Text = "CAD File:";
            // 
            // lbCadFilePath
            // 
            lbCadFilePath.Name = "lbCadFilePath";
            lbCadFilePath.Size = new Size(19, 17);
            lbCadFilePath.Text = "    ";
            // 
            // toolStripStatusLabel2
            // 
            toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            toolStripStatusLabel2.Size = new Size(68, 17);
            toolStripStatusLabel2.Text = "Project File:";
            // 
            // lbProjectPath
            // 
            lbProjectPath.Name = "lbProjectPath";
            lbProjectPath.Size = new Size(13, 17);
            lbProjectPath.Text = "  ";
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(label2, 0, 2);
            tableLayoutPanel1.Controls.Add(dgBlocks, 0, 1);
            tableLayoutPanel1.Controls.Add(dgEntites, 0, 3);
            tableLayoutPanel1.Controls.Add(label3, 1, 0);
            tableLayoutPanel1.Controls.Add(label4, 1, 2);
            tableLayoutPanel1.Controls.Add(flowLayoutPanel1, 1, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 24);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new Size(1264, 715);
            tableLayoutPanel1.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(41, 15);
            label1.TabIndex = 0;
            label1.Text = "Blocks";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(3, 357);
            label2.Name = "label2";
            label2.Size = new Size(42, 15);
            label2.TabIndex = 1;
            label2.Text = "Entites";
            // 
            // dgBlocks
            // 
            dgBlocks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgBlocks.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgBlocks.Columns.AddRange(new DataGridViewColumn[] { Column1, Column2 });
            dgBlocks.Dock = DockStyle.Fill;
            dgBlocks.Location = new Point(3, 23);
            dgBlocks.Name = "dgBlocks";
            dgBlocks.Size = new Size(626, 331);
            dgBlocks.TabIndex = 3;
            dgBlocks.CellMouseDoubleClick += Block_DoubleClick;
            // 
            // Column1
            // 
            Column1.DataPropertyName = "Name";
            Column1.HeaderText = "Name";
            Column1.Name = "Column1";
            Column1.ReadOnly = true;
            // 
            // Column2
            // 
            Column2.DataPropertyName = "BindName";
            Column2.HeaderText = "BindName";
            Column2.Name = "Column2";
            Column2.ReadOnly = true;
            // 
            // dgEntites
            // 
            dgEntites.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgEntites.Columns.AddRange(new DataGridViewColumn[] { Column3, Column4 });
            dgEntites.Dock = DockStyle.Fill;
            dgEntites.Location = new Point(3, 380);
            dgEntites.Name = "dgEntites";
            dgEntites.Size = new Size(626, 332);
            dgEntites.TabIndex = 4;
            dgEntites.CellContentDoubleClick += Entites_DoubleClick;
            // 
            // Column3
            // 
            Column3.DataPropertyName = "ElementTypeName";
            Column3.HeaderText = "Type";
            Column3.Name = "Column3";
            Column3.ReadOnly = true;
            Column3.Width = 160;
            // 
            // Column4
            // 
            Column4.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Column4.DataPropertyName = "Summary";
            Column4.HeaderText = "Detail";
            Column4.Name = "Column4";
            Column4.ReadOnly = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(635, 0);
            label3.Name = "label3";
            label3.Size = new Size(44, 15);
            label3.TabIndex = 5;
            label3.Text = "Option";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(635, 357);
            label4.Name = "label4";
            label4.Size = new Size(42, 15);
            label4.TabIndex = 6;
            label4.Text = "Viewer";
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(label5);
            flowLayoutPanel1.Controls.Add(txtXmin);
            flowLayoutPanel1.Controls.Add(label6);
            flowLayoutPanel1.Controls.Add(txtYmin);
            flowLayoutPanel1.Controls.Add(label7);
            flowLayoutPanel1.Controls.Add(txtXmax);
            flowLayoutPanel1.Controls.Add(label8);
            flowLayoutPanel1.Controls.Add(txtYmax);
            flowLayoutPanel1.Controls.Add(chbIgnoreNoBindBlock);
            flowLayoutPanel1.Controls.Add(chbLine);
            flowLayoutPanel1.Controls.Add(chbText);
            flowLayoutPanel1.Controls.Add(chbBlock);
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.FlowDirection = FlowDirection.TopDown;
            flowLayoutPanel1.Location = new Point(635, 23);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(626, 331);
            flowLayoutPanel1.TabIndex = 7;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(3, 0);
            label5.Name = "label5";
            label5.Size = new Size(38, 15);
            label5.TabIndex = 0;
            label5.Text = "X min";
            // 
            // txtXmin
            // 
            txtXmin.Location = new Point(3, 18);
            txtXmin.Name = "txtXmin";
            txtXmin.Size = new Size(183, 23);
            txtXmin.TabIndex = 1;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(3, 44);
            label6.Name = "label6";
            label6.Size = new Size(38, 15);
            label6.TabIndex = 2;
            label6.Text = "Y min";
            // 
            // txtYmin
            // 
            txtYmin.Location = new Point(3, 62);
            txtYmin.Name = "txtYmin";
            txtYmin.Size = new Size(183, 23);
            txtYmin.TabIndex = 3;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(3, 88);
            label7.Name = "label7";
            label7.Size = new Size(39, 15);
            label7.TabIndex = 4;
            label7.Text = "X max";
            // 
            // txtXmax
            // 
            txtXmax.Location = new Point(3, 106);
            txtXmax.Name = "txtXmax";
            txtXmax.Size = new Size(183, 23);
            txtXmax.TabIndex = 5;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(3, 132);
            label8.Name = "label8";
            label8.Size = new Size(39, 15);
            label8.TabIndex = 6;
            label8.Text = "Y max";
            // 
            // txtYmax
            // 
            txtYmax.Location = new Point(3, 150);
            txtYmax.Name = "txtYmax";
            txtYmax.Size = new Size(183, 23);
            txtYmax.TabIndex = 7;
            // 
            // chbIgnoreNoBindBlock
            // 
            chbIgnoreNoBindBlock.AutoSize = true;
            chbIgnoreNoBindBlock.Location = new Point(3, 179);
            chbIgnoreNoBindBlock.Name = "chbIgnoreNoBindBlock";
            chbIgnoreNoBindBlock.Size = new Size(138, 19);
            chbIgnoreNoBindBlock.TabIndex = 8;
            chbIgnoreNoBindBlock.Text = "Ignore No Bind Block";
            chbIgnoreNoBindBlock.UseVisualStyleBackColor = true;
            // 
            // chbLine
            // 
            chbLine.AutoSize = true;
            chbLine.Location = new Point(3, 204);
            chbLine.Name = "chbLine";
            chbLine.Size = new Size(78, 19);
            chbLine.TabIndex = 9;
            chbLine.Text = "Draw Line";
            chbLine.UseVisualStyleBackColor = true;
            // 
            // chbText
            // 
            chbText.AutoSize = true;
            chbText.Location = new Point(3, 229);
            chbText.Name = "chbText";
            chbText.Size = new Size(77, 19);
            chbText.TabIndex = 10;
            chbText.Text = "Draw Text";
            chbText.UseVisualStyleBackColor = true;
            // 
            // chbBlock
            // 
            chbBlock.AutoSize = true;
            chbBlock.Location = new Point(3, 254);
            chbBlock.Name = "chbBlock";
            chbBlock.Size = new Size(85, 19);
            chbBlock.TabIndex = 11;
            chbBlock.Text = "Draw Block";
            chbBlock.UseVisualStyleBackColor = true;
            // 
            // mainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1264, 761);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(mainStatusStrip);
            Controls.Add(mainMenu);
            MainMenuStrip = mainMenu;
            Name = "mainForm";
            Text = "CAD Import Configure";
            Load += mainForm_Load;
            mainMenu.ResumeLayout(false);
            mainMenu.PerformLayout();
            mainStatusStrip.ResumeLayout(false);
            mainStatusStrip.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgBlocks).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgEntites).EndInit();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip mainMenu;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem MenuItem_OpenDxf;
        private ToolStripMenuItem MenuItem_OpenDwg;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem closeToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem MenuItem_OpenProject;
        private StatusStrip mainStatusStrip;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripStatusLabel lbCadFilePath;
        private ToolStripStatusLabel toolStripStatusLabel2;
        private ToolStripStatusLabel lbProjectPath;
        private ToolStripMenuItem MenuItem_NewProject;
        private TableLayoutPanel tableLayoutPanel1;
        private Label label1;
        private Label label2;
        private DataGridView dgBlocks;
        private DataGridView dgEntites;
        private DataGridViewTextBoxColumn Column3;
        private DataGridViewTextBoxColumn Column4;
        private DataGridViewTextBoxColumn Column1;
        private DataGridViewTextBoxColumn Column2;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem MenuItem_SaveProject;
        private Label label3;
        private Label label4;
        private FlowLayoutPanel flowLayoutPanel1;
        private Label label5;
        private TextBox txtXmin;
        private Label label6;
        private TextBox txtYmin;
        private Label label7;
        private TextBox txtXmax;
        private Label label8;
        private TextBox txtYmax;
        private CheckBox chbIgnoreNoBindBlock;
        private CheckBox chbLine;
        private CheckBox chbText;
        private CheckBox chbBlock;
    }
}
