namespace Torrent_Parser
{
    partial class SearchForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.searchTextBox = new System.Windows.Forms.TextBox();
            this.searchBut = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.TitleColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SeedersColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LeechersColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UploadedColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MagnetColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.URLColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openOnTPBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openMagnetBut = new System.Windows.Forms.Button();
            this.openTPBBut = new System.Windows.Forms.Button();
            this.settingsBut = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // searchTextBox
            // 
            this.searchTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.searchTextBox.Location = new System.Drawing.Point(13, 13);
            this.searchTextBox.Name = "searchTextBox";
            this.searchTextBox.Size = new System.Drawing.Size(461, 20);
            this.searchTextBox.TabIndex = 0;
            this.searchTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.searchTextBox_KeyDown);
            // 
            // searchBut
            // 
            this.searchBut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.searchBut.Location = new System.Drawing.Point(480, 12);
            this.searchBut.Name = "searchBut";
            this.searchBut.Size = new System.Drawing.Size(75, 23);
            this.searchBut.TabIndex = 1;
            this.searchBut.Text = "&Search";
            this.searchBut.UseVisualStyleBackColor = true;
            this.searchBut.Click += new System.EventHandler(this.searchBut_Click);
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TitleColumn,
            this.SeedersColumn,
            this.LeechersColumn,
            this.UploadedColumn,
            this.MagnetColumn,
            this.URLColumn});
            this.dataGridView.Location = new System.Drawing.Point(12, 39);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersWidth = 25;
            this.dataGridView.RowTemplate.ContextMenuStrip = this.contextMenuStrip1;
            this.dataGridView.Size = new System.Drawing.Size(543, 379);
            this.dataGridView.TabIndex = 2;
            this.dataGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellDoubleClick);
            this.dataGridView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView_KeyDown);
            this.dataGridView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGridView_MouseDown);
            // 
            // TitleColumn
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.TitleColumn.DefaultCellStyle = dataGridViewCellStyle1;
            this.TitleColumn.HeaderText = "Title";
            this.TitleColumn.Name = "TitleColumn";
            this.TitleColumn.ReadOnly = true;
            this.TitleColumn.Width = 315;
            // 
            // SeedersColumn
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.SeedersColumn.DefaultCellStyle = dataGridViewCellStyle2;
            this.SeedersColumn.HeaderText = "Seeders";
            this.SeedersColumn.Name = "SeedersColumn";
            this.SeedersColumn.ReadOnly = true;
            this.SeedersColumn.Width = 60;
            // 
            // LeechersColumn
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.LeechersColumn.DefaultCellStyle = dataGridViewCellStyle3;
            this.LeechersColumn.HeaderText = "Leechers";
            this.LeechersColumn.Name = "LeechersColumn";
            this.LeechersColumn.ReadOnly = true;
            this.LeechersColumn.Width = 60;
            // 
            // UploadedColumn
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.UploadedColumn.DefaultCellStyle = dataGridViewCellStyle4;
            this.UploadedColumn.HeaderText = "Uploaded";
            this.UploadedColumn.Name = "UploadedColumn";
            this.UploadedColumn.ReadOnly = true;
            this.UploadedColumn.Width = 60;
            // 
            // MagnetColumn
            // 
            this.MagnetColumn.HeaderText = "Magnet";
            this.MagnetColumn.Name = "MagnetColumn";
            this.MagnetColumn.ReadOnly = true;
            this.MagnetColumn.Visible = false;
            // 
            // URLColumn
            // 
            this.URLColumn.HeaderText = "URL";
            this.URLColumn.Name = "URLColumn";
            this.URLColumn.ReadOnly = true;
            this.URLColumn.Visible = false;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openOnTPBToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(145, 26);
            // 
            // openOnTPBToolStripMenuItem
            // 
            this.openOnTPBToolStripMenuItem.Name = "openOnTPBToolStripMenuItem";
            this.openOnTPBToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.openOnTPBToolStripMenuItem.Text = "Open on &TPB";
            this.openOnTPBToolStripMenuItem.Click += new System.EventHandler(this.openOnTPBToolStripMenuItem_Click);
            // 
            // openMagnetBut
            // 
            this.openMagnetBut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.openMagnetBut.Location = new System.Drawing.Point(462, 424);
            this.openMagnetBut.Name = "openMagnetBut";
            this.openMagnetBut.Size = new System.Drawing.Size(93, 23);
            this.openMagnetBut.TabIndex = 3;
            this.openMagnetBut.Text = "Open &Magnet";
            this.openMagnetBut.UseVisualStyleBackColor = true;
            this.openMagnetBut.Click += new System.EventHandler(this.openMagnetBut_Click);
            // 
            // openTPBBut
            // 
            this.openTPBBut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.openTPBBut.Location = new System.Drawing.Point(363, 424);
            this.openTPBBut.Name = "openTPBBut";
            this.openTPBBut.Size = new System.Drawing.Size(93, 23);
            this.openTPBBut.TabIndex = 4;
            this.openTPBBut.Text = "Open on &TPB";
            this.openTPBBut.UseVisualStyleBackColor = true;
            this.openTPBBut.Click += new System.EventHandler(this.openTPBBut_Click);
            // 
            // settingsBut
            // 
            this.settingsBut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.settingsBut.Location = new System.Drawing.Point(264, 424);
            this.settingsBut.Name = "settingsBut";
            this.settingsBut.Size = new System.Drawing.Size(93, 23);
            this.settingsBut.TabIndex = 6;
            this.settingsBut.Text = "S&ettings";
            this.settingsBut.UseVisualStyleBackColor = true;
            this.settingsBut.Click += new System.EventHandler(this.settingsBut_Click);
            // 
            // SearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(567, 459);
            this.Controls.Add(this.settingsBut);
            this.Controls.Add(this.openTPBBut);
            this.Controls.Add(this.openMagnetBut);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.searchBut);
            this.Controls.Add(this.searchTextBox);
            this.MinimumSize = new System.Drawing.Size(583, 498);
            this.Name = "SearchForm";
            this.Text = "TPB Search";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox searchTextBox;
        private System.Windows.Forms.Button searchBut;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.Button openMagnetBut;
        private System.Windows.Forms.Button openTPBBut;
        private System.Windows.Forms.DataGridViewTextBoxColumn TitleColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn SeedersColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn LeechersColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn UploadedColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn MagnetColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn URLColumn;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem openOnTPBToolStripMenuItem;
        private System.Windows.Forms.Button settingsBut;
    }
}

