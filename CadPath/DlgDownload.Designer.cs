namespace CadPath
{
    partial class DlgDownload
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
            tableLayoutPanel1 = new TableLayoutPanel();
            txtIP = new TextBox();
            label2 = new Label();
            label1 = new Label();
            txtRack = new TextBox();
            label3 = new Label();
            label4 = new Label();
            txtSlot = new TextBox();
            txtXName = new TextBox();
            btnDonwload = new Button();
            label5 = new Label();
            txtYName = new TextBox();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(txtIP, 1, 0);
            tableLayoutPanel1.Controls.Add(label2, 0, 1);
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(txtRack, 1, 1);
            tableLayoutPanel1.Controls.Add(label3, 0, 2);
            tableLayoutPanel1.Controls.Add(label4, 0, 3);
            tableLayoutPanel1.Controls.Add(txtSlot, 1, 2);
            tableLayoutPanel1.Controls.Add(txtXName, 1, 3);
            tableLayoutPanel1.Controls.Add(btnDonwload, 1, 5);
            tableLayoutPanel1.Controls.Add(label5, 0, 4);
            tableLayoutPanel1.Controls.Add(txtYName, 1, 4);
            tableLayoutPanel1.Location = new Point(80, 12);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 6;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new Size(602, 371);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // txtIP
            // 
            txtIP.Dock = DockStyle.Fill;
            txtIP.Location = new Point(103, 3);
            txtIP.Name = "txtIP";
            txtIP.Size = new Size(496, 23);
            txtIP.TabIndex = 1;
            txtIP.Text = "192.168.1.100";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(3, 40);
            label2.Name = "label2";
            label2.Size = new Size(32, 15);
            label2.TabIndex = 2;
            label2.Text = "Rack";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(17, 15);
            label1.TabIndex = 3;
            label1.Text = "IP";
            // 
            // txtRack
            // 
            txtRack.Dock = DockStyle.Fill;
            txtRack.Location = new Point(103, 43);
            txtRack.Name = "txtRack";
            txtRack.Size = new Size(496, 23);
            txtRack.TabIndex = 4;
            txtRack.Text = "0";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(3, 80);
            label3.Name = "label3";
            label3.Size = new Size(27, 15);
            label3.TabIndex = 5;
            label3.Text = "Slot";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(3, 120);
            label4.Name = "label4";
            label4.Size = new Size(69, 15);
            label4.TabIndex = 6;
            label4.Text = "X 坐标地址";
            // 
            // txtSlot
            // 
            txtSlot.Dock = DockStyle.Fill;
            txtSlot.Location = new Point(103, 83);
            txtSlot.Name = "txtSlot";
            txtSlot.Size = new Size(496, 23);
            txtSlot.TabIndex = 7;
            txtSlot.Text = "1";
            // 
            // txtXName
            // 
            txtXName.Dock = DockStyle.Fill;
            txtXName.Location = new Point(103, 123);
            txtXName.Name = "txtXName";
            txtXName.Size = new Size(496, 23);
            txtXName.TabIndex = 8;
            // 
            // btnDonwload
            // 
            btnDonwload.Location = new Point(103, 203);
            btnDonwload.Name = "btnDonwload";
            btnDonwload.Size = new Size(75, 23);
            btnDonwload.TabIndex = 9;
            btnDonwload.Text = "Download";
            btnDonwload.UseVisualStyleBackColor = true;
            btnDonwload.Click += btnDonwload_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(3, 160);
            label5.Name = "label5";
            label5.Size = new Size(69, 15);
            label5.TabIndex = 10;
            label5.Text = "Y 坐标地址";
            // 
            // txtYName
            // 
            txtYName.Dock = DockStyle.Fill;
            txtYName.Location = new Point(103, 163);
            txtYName.Name = "txtYName";
            txtYName.Size = new Size(496, 23);
            txtYName.TabIndex = 11;
            // 
            // DlgDownload
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 561);
            Controls.Add(tableLayoutPanel1);
            Name = "DlgDownload";
            StartPosition = FormStartPosition.CenterParent;
            Text = "DlgDownload";
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private TextBox txtIP;
        private Label label2;
        private Label label1;
        private TextBox txtRack;
        private Label label3;
        private Label label4;
        private TextBox txtSlot;
        private TextBox txtXName;
        private Button btnDonwload;
        private Label label5;
        private TextBox txtYName;
    }
}