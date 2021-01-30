namespace GinsorAudioTool2Plus
{
	public partial class Form1 : global::System.Windows.Forms.Form
	{
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
      this.menuStrip1 = new System.Windows.Forms.MenuStrip();
      this.topMenu_loadDefault = new System.Windows.Forms.ToolStripMenuItem();
      this.topMenu_reloadDatabase = new System.Windows.Forms.ToolStripMenuItem();
      this.topMenu_about = new System.Windows.Forms.ToolStripMenuItem();
      this.splitContainer1 = new System.Windows.Forms.SplitContainer();
      this.panel_dataViewer = new System.Windows.Forms.Panel();
      this.treeView1 = new System.Windows.Forms.TreeView();
      this.tb_EventLogger = new System.Windows.Forms.TextBox();
      this.panel1 = new System.Windows.Forms.Panel();
      this.cb_narratorFilter = new System.Windows.Forms.ComboBox();
      this.tb_searchBox = new System.Windows.Forms.TextBox();
      this.statusLabel = new System.Windows.Forms.Label();
      this.panel7 = new System.Windows.Forms.Panel();
      this.panel2 = new System.Windows.Forms.Panel();
      this.btn_stopMusic = new System.Windows.Forms.Button();
      this.btn_expandTreeview = new System.Windows.Forms.Button();
      this.label6 = new System.Windows.Forms.Label();
      this.btn_collapseTreeview = new System.Windows.Forms.Button();
      this.cb_saveAudio = new System.Windows.Forms.CheckBox();
      this.menuStrip1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
      this.splitContainer1.Panel1.SuspendLayout();
      this.splitContainer1.Panel2.SuspendLayout();
      this.splitContainer1.SuspendLayout();
      this.panel_dataViewer.SuspendLayout();
      this.panel1.SuspendLayout();
      this.panel7.SuspendLayout();
      this.SuspendLayout();
      // 
      // menuStrip1
      // 
      this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.topMenu_loadDefault,
            this.topMenu_reloadDatabase,
            this.topMenu_about});
      this.menuStrip1.Location = new System.Drawing.Point(0, 0);
      this.menuStrip1.Name = "menuStrip1";
      this.menuStrip1.Size = new System.Drawing.Size(995, 24);
      this.menuStrip1.TabIndex = 21;
      this.menuStrip1.Text = "menuStrip1";
      // 
      // topMenu_loadDefault
      // 
      this.topMenu_loadDefault.Name = "topMenu_loadDefault";
      this.topMenu_loadDefault.Size = new System.Drawing.Size(114, 20);
      this.topMenu_loadDefault.Text = "Reload Transcripts";
      this.topMenu_loadDefault.Click += new System.EventHandler(this.TopMenu_loadDefault_Click);
      // 
      // topMenu_reloadDatabase
      // 
      this.topMenu_reloadDatabase.Name = "topMenu_reloadDatabase";
      this.topMenu_reloadDatabase.Size = new System.Drawing.Size(113, 20);
      this.topMenu_reloadDatabase.Text = "Update Databases";
      this.topMenu_reloadDatabase.Click += new System.EventHandler(this.TopMenu_reloadDatabase_Click);
      // 
      // topMenu_about
      // 
      this.topMenu_about.Name = "topMenu_about";
      this.topMenu_about.Size = new System.Drawing.Size(52, 20);
      this.topMenu_about.Text = "About";
      this.topMenu_about.Click += new System.EventHandler(this.TopMenu_about_Click);
      // 
      // splitContainer1
      // 
      this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.splitContainer1.Location = new System.Drawing.Point(0, 92);
      this.splitContainer1.Name = "splitContainer1";
      this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
      // 
      // splitContainer1.Panel1
      // 
      this.splitContainer1.Panel1.Controls.Add(this.panel_dataViewer);
      // 
      // splitContainer1.Panel2
      // 
      this.splitContainer1.Panel2.Controls.Add(this.tb_EventLogger);
      this.splitContainer1.Size = new System.Drawing.Size(995, 510);
      this.splitContainer1.SplitterDistance = 384;
      this.splitContainer1.TabIndex = 22;
      // 
      // panel_dataViewer
      // 
      this.panel_dataViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.panel_dataViewer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(41)))));
      this.panel_dataViewer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.panel_dataViewer.Controls.Add(this.treeView1);
      this.panel_dataViewer.ForeColor = System.Drawing.SystemColors.ControlLight;
      this.panel_dataViewer.Location = new System.Drawing.Point(0, 0);
      this.panel_dataViewer.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
      this.panel_dataViewer.Name = "panel_dataViewer";
      this.panel_dataViewer.Size = new System.Drawing.Size(995, 381);
      this.panel_dataViewer.TabIndex = 16;
      // 
      // treeView1
      // 
      this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.treeView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(41)))));
      this.treeView1.ForeColor = System.Drawing.SystemColors.ControlLight;
      this.treeView1.LineColor = System.Drawing.Color.Silver;
      this.treeView1.Location = new System.Drawing.Point(-1, -1);
      this.treeView1.Name = "treeView1";
      this.treeView1.ShowNodeToolTips = true;
      this.treeView1.Size = new System.Drawing.Size(995, 381);
      this.treeView1.TabIndex = 4;
      this.treeView1.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TreeView1_NodeMouseDoubleClick);
      // 
      // tb_EventLogger
      // 
      this.tb_EventLogger.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tb_EventLogger.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(41)))));
      this.tb_EventLogger.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.tb_EventLogger.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.tb_EventLogger.ForeColor = System.Drawing.SystemColors.ControlLight;
      this.tb_EventLogger.Location = new System.Drawing.Point(0, 3);
      this.tb_EventLogger.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
      this.tb_EventLogger.Multiline = true;
      this.tb_EventLogger.Name = "tb_EventLogger";
      this.tb_EventLogger.ReadOnly = true;
      this.tb_EventLogger.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.tb_EventLogger.Size = new System.Drawing.Size(995, 119);
      this.tb_EventLogger.TabIndex = 23;
      // 
      // panel1
      // 
      this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(69)))), ((int)(((byte)(69)))));
      this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.panel1.Controls.Add(this.cb_narratorFilter);
      this.panel1.Controls.Add(this.tb_searchBox);
      this.panel1.Location = new System.Drawing.Point(0, 27);
      this.panel1.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(995, 31);
      this.panel1.TabIndex = 24;
      // 
      // cb_narratorFilter
      // 
      this.cb_narratorFilter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.cb_narratorFilter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(41)))));
      this.cb_narratorFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cb_narratorFilter.ForeColor = System.Drawing.Color.White;
      this.cb_narratorFilter.Location = new System.Drawing.Point(772, 4);
      this.cb_narratorFilter.Name = "cb_narratorFilter";
      this.cb_narratorFilter.Size = new System.Drawing.Size(215, 21);
      this.cb_narratorFilter.TabIndex = 5;
      this.cb_narratorFilter.DropDownClosed += new System.EventHandler(this.Cb_narratorFilter_DropDownClosed);
      // 
      // tb_searchBox
      // 
      this.tb_searchBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tb_searchBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(41)))));
      this.tb_searchBox.ForeColor = System.Drawing.Color.White;
      this.tb_searchBox.Location = new System.Drawing.Point(4, 4);
      this.tb_searchBox.Name = "tb_searchBox";
      this.tb_searchBox.Size = new System.Drawing.Size(762, 20);
      this.tb_searchBox.TabIndex = 3;
      this.tb_searchBox.Text = "Search...";
      this.tb_searchBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Tb_searchBox_MouseClick);
      this.tb_searchBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Tb_searchBox_KeyDown);
      // 
      // statusLabel
      // 
      this.statusLabel.BackColor = System.Drawing.SystemColors.ControlLight;
      this.statusLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.statusLabel.Location = new System.Drawing.Point(0, 608);
      this.statusLabel.Name = "statusLabel";
      this.statusLabel.Size = new System.Drawing.Size(995, 20);
      this.statusLabel.TabIndex = 25;
      this.statusLabel.Text = "v2.0.2.0 | Invented by Ginsor, Improved by pinky | 2021";
      this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      // 
      // panel7
      // 
      this.panel7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.panel7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(69)))), ((int)(((byte)(69)))));
      this.panel7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.panel7.Controls.Add(this.panel2);
      this.panel7.Controls.Add(this.btn_stopMusic);
      this.panel7.Controls.Add(this.btn_expandTreeview);
      this.panel7.Controls.Add(this.label6);
      this.panel7.Controls.Add(this.btn_collapseTreeview);
      this.panel7.Location = new System.Drawing.Point(0, 63);
      this.panel7.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
      this.panel7.Name = "panel7";
      this.panel7.Size = new System.Drawing.Size(995, 29);
      this.panel7.TabIndex = 23;
      // 
      // panel2
      // 
      this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.panel2.BackColor = System.Drawing.Color.Silver;
      this.panel2.Location = new System.Drawing.Point(920, -1);
      this.panel2.Name = "panel2";
      this.panel2.Size = new System.Drawing.Size(2, 29);
      this.panel2.TabIndex = 5;
      // 
      // btn_stopMusic
      // 
      this.btn_stopMusic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btn_stopMusic.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
      this.btn_stopMusic.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btn_stopMusic.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btn_stopMusic.ForeColor = System.Drawing.Color.White;
      this.btn_stopMusic.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btn_stopMusic.Location = new System.Drawing.Point(882, 2);
      this.btn_stopMusic.Margin = new System.Windows.Forms.Padding(0);
      this.btn_stopMusic.Name = "btn_stopMusic";
      this.btn_stopMusic.Size = new System.Drawing.Size(23, 22);
      this.btn_stopMusic.TabIndex = 26;
      this.btn_stopMusic.Text = "| |  ";
      this.btn_stopMusic.UseCompatibleTextRendering = true;
      this.btn_stopMusic.UseVisualStyleBackColor = false;
      this.btn_stopMusic.Click += new System.EventHandler(this.Btn_stopMusic_Click);
      // 
      // btn_expandTreeview
      // 
      this.btn_expandTreeview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btn_expandTreeview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
      this.btn_expandTreeview.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btn_expandTreeview.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btn_expandTreeview.ForeColor = System.Drawing.Color.White;
      this.btn_expandTreeview.Location = new System.Drawing.Point(935, 2);
      this.btn_expandTreeview.Margin = new System.Windows.Forms.Padding(0);
      this.btn_expandTreeview.Name = "btn_expandTreeview";
      this.btn_expandTreeview.Size = new System.Drawing.Size(22, 22);
      this.btn_expandTreeview.TabIndex = 25;
      this.btn_expandTreeview.Text = "+";
      this.btn_expandTreeview.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.btn_expandTreeview.UseCompatibleTextRendering = true;
      this.btn_expandTreeview.UseVisualStyleBackColor = false;
      this.btn_expandTreeview.Click += new System.EventHandler(this.Btn_expandTreeview_Click);
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label6.ForeColor = System.Drawing.Color.White;
      this.label6.Location = new System.Drawing.Point(3, 6);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(52, 13);
      this.label6.TabIndex = 1;
      this.label6.Text = "Browser";
      // 
      // btn_collapseTreeview
      // 
      this.btn_collapseTreeview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btn_collapseTreeview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
      this.btn_collapseTreeview.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btn_collapseTreeview.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btn_collapseTreeview.ForeColor = System.Drawing.Color.White;
      this.btn_collapseTreeview.Location = new System.Drawing.Point(964, 2);
      this.btn_collapseTreeview.Name = "btn_collapseTreeview";
      this.btn_collapseTreeview.Size = new System.Drawing.Size(23, 22);
      this.btn_collapseTreeview.TabIndex = 24;
      this.btn_collapseTreeview.Text = "— ";
      this.btn_collapseTreeview.UseVisualStyleBackColor = false;
      this.btn_collapseTreeview.Click += new System.EventHandler(this.Btn_collapseTreeview_Click);
      // 
      // cb_saveAudio
      // 
      this.cb_saveAudio.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.cb_saveAudio.AutoSize = true;
      this.cb_saveAudio.BackColor = System.Drawing.SystemColors.ButtonFace;
      this.cb_saveAudio.Location = new System.Drawing.Point(899, 4);
      this.cb_saveAudio.Name = "cb_saveAudio";
      this.cb_saveAudio.Size = new System.Drawing.Size(81, 17);
      this.cb_saveAudio.TabIndex = 26;
      this.cb_saveAudio.Text = "Save Audio";
      this.cb_saveAudio.UseVisualStyleBackColor = false;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
      this.ClientSize = new System.Drawing.Size(995, 628);
      this.Controls.Add(this.cb_saveAudio);
      this.Controls.Add(this.splitContainer1);
      this.Controls.Add(this.panel1);
      this.Controls.Add(this.statusLabel);
      this.Controls.Add(this.panel7);
      this.Controls.Add(this.menuStrip1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MainMenuStrip = this.menuStrip1;
      this.MinimumSize = new System.Drawing.Size(1011, 667);
      this.Name = "Form1";
      this.Text = "Ginsor Audio Tool 2 Plus";
      this.Load += new System.EventHandler(this.Form1_Load);
      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
      this.splitContainer1.Panel1.ResumeLayout(false);
      this.splitContainer1.Panel2.ResumeLayout(false);
      this.splitContainer1.Panel2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
      this.splitContainer1.ResumeLayout(false);
      this.panel_dataViewer.ResumeLayout(false);
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.panel7.ResumeLayout(false);
      this.panel7.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

		}

		private global::System.ComponentModel.IContainer components;

		private global::System.Windows.Forms.SplitContainer splitContainer1;

		public global::System.Windows.Forms.Panel panel_dataViewer;

		private global::System.Windows.Forms.TreeView treeView1;

		private global::System.Windows.Forms.TextBox tb_EventLogger;

		private global::System.Windows.Forms.Panel panel1;

		private global::System.Windows.Forms.ComboBox cb_narratorFilter;

		private global::System.Windows.Forms.TextBox tb_searchBox;

		private global::System.Windows.Forms.Label statusLabel;

		private global::System.Windows.Forms.Panel panel7;

		private global::System.Windows.Forms.Button btn_expandTreeview;

		private global::System.Windows.Forms.Label label6;

		private global::System.Windows.Forms.Button btn_collapseTreeview;

		private global::System.Windows.Forms.CheckBox cb_saveAudio;

		private global::System.Windows.Forms.ToolStripMenuItem topMenu_reloadDatabase;

		private global::System.Windows.Forms.Button btn_stopMusic;

		private global::System.Windows.Forms.Panel panel2;

		private global::System.Windows.Forms.ToolStripMenuItem topMenu_about;

		private global::System.Windows.Forms.MenuStrip menuStrip1;

		public global::System.Windows.Forms.ToolStripMenuItem topMenu_loadDefault;
	}
}
