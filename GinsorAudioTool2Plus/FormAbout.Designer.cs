using System.Drawing;

namespace GinsorAudioTool2Plus
{
	public partial class FormAbout : global::System.Windows.Forms.Form
	{
		protected override void Dispose(bool disposing)
		{
			bool flag = disposing && this.components != null;
			bool flag2 = flag;
			if (flag2)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this.label8 = new System.Windows.Forms.Label();
      this.label9 = new System.Windows.Forms.Label();
      this.label10 = new System.Windows.Forms.Label();
      this.label11 = new System.Windows.Forms.Label();
      this.label12 = new System.Windows.Forms.Label();
      this.label13 = new System.Windows.Forms.Label();
      this.label15 = new System.Windows.Forms.Label();
      this.linkLabel1 = new System.Windows.Forms.LinkLabel();
      this.label16 = new System.Windows.Forms.Label();
      this.pictureBox2 = new System.Windows.Forms.PictureBox();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
      this.SuspendLayout();
      // 
      // pictureBox1
      // 
      this.pictureBox1.Image = global::GinsorAudioTool2Plus.Properties.Resources.BigIcon;
      this.pictureBox1.InitialImage = null;
      this.pictureBox1.Location = new System.Drawing.Point(181, 82);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(95, 103);
      this.pictureBox1.TabIndex = 0;
      this.pictureBox1.TabStop = false;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.ForeColor = System.Drawing.Color.Silver;
      this.label1.Location = new System.Drawing.Point(105, 9);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(274, 25);
      this.label1.TabIndex = 1;
      this.label1.Text = "Ginsor Audio Tool 2 Plus";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.ForeColor = System.Drawing.Color.Silver;
      this.label2.Location = new System.Drawing.Point(159, 43);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(138, 20);
      this.label2.TabIndex = 2;
      this.label2.Text = "- For Destiny 2 -";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.ForeColor = System.Drawing.Color.Silver;
      this.label3.Location = new System.Drawing.Point(40, 198);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(52, 16);
      this.label3.TabIndex = 3;
      this.label3.Text = "v2.0.2.0";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label4.ForeColor = System.Drawing.Color.Silver;
      this.label4.Location = new System.Drawing.Point(32, 215);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(418, 16);
      this.label4.TabIndex = 4;
      this.label4.Text = "▪ Loads all dialogue text and corresponding audio from the game files";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label5.ForeColor = System.Drawing.Color.Silver;
      this.label5.Location = new System.Drawing.Point(32, 231);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(198, 16);
      this.label5.TabIndex = 5;
      this.label5.Text = "▪ Audio Playback and Extraction";
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label6.ForeColor = System.Drawing.Color.Silver;
      this.label6.Location = new System.Drawing.Point(32, 281);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(275, 16);
      this.label6.TabIndex = 6;
      this.label6.Text = "▪ Searchable by content, narrator, or file hash.";
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label7.ForeColor = System.Drawing.Color.Silver;
      this.label7.Location = new System.Drawing.Point(32, 297);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(158, 16);
      this.label7.TabIndex = 7;
      this.label7.Text = "▪ Fixed playback crackle.";
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label8.ForeColor = System.Drawing.Color.Silver;
      this.label8.Location = new System.Drawing.Point(32, 313);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(360, 16);
      this.label8.TabIndex = 8;
      this.label8.Text = "▪ Does not access the running game\'s memory (no ban risk)";
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label9.ForeColor = System.Drawing.Color.Silver;
      this.label9.Location = new System.Drawing.Point(32, 329);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(358, 16);
      this.label9.TabIndex = 9;
      this.label9.Text = "▪ Easy Database update in case of a new Destiny 2 version";
      // 
      // label10
      // 
      this.label10.AutoSize = true;
      this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label10.ForeColor = System.Drawing.Color.Silver;
      this.label10.Location = new System.Drawing.Point(32, 247);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(329, 16);
      this.label10.TabIndex = 10;
      this.label10.Text = "▪ Transcripts inside a collection are in the correct order";
      // 
      // label11
      // 
      this.label11.AutoSize = true;
      this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label11.ForeColor = System.Drawing.Color.Silver;
      this.label11.Location = new System.Drawing.Point(33, 263);
      this.label11.Name = "label11";
      this.label11.Size = new System.Drawing.Size(265, 16);
      this.label11.TabIndex = 11;
      this.label11.Text = "   according to how they are shown in-game.";
      // 
      // label12
      // 
      this.label12.AutoSize = true;
      this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label12.ForeColor = System.Drawing.Color.Silver;
      this.label12.Location = new System.Drawing.Point(32, 345);
      this.label12.Name = "label12";
      this.label12.Size = new System.Drawing.Size(297, 16);
      this.label12.TabIndex = 12;
      this.label12.Text = "▪ Excludes audio and transcripts from cinematics,";
      // 
      // label13
      // 
      this.label13.AutoSize = true;
      this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label13.ForeColor = System.Drawing.Color.Silver;
      this.label13.Location = new System.Drawing.Point(32, 361);
      this.label13.Name = "label13";
      this.label13.Size = new System.Drawing.Size(223, 16);
      this.label13.TabIndex = 13;
      this.label13.Text = "▪ Some clips lack Narrator metadata";
      // 
      // label15
      // 
      this.label15.AutoSize = true;
      this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label15.ForeColor = System.Drawing.Color.Silver;
      this.label15.Location = new System.Drawing.Point(7, 473);
      this.label15.Name = "label15";
      this.label15.Size = new System.Drawing.Size(443, 15);
      this.label15.TabIndex = 15;
      this.label15.Text = "Destiny 2, and all accompanying resources/data © 2020 Bungie, Inc.";
      // 
      // linkLabel1
      // 
      this.linkLabel1.AccessibleRole = System.Windows.Forms.AccessibleRole.Link;
      this.linkLabel1.AutoSize = true;
      this.linkLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.linkLabel1.LinkColor = System.Drawing.Color.DodgerBlue;
      this.linkLabel1.Location = new System.Drawing.Point(88, 428);
      this.linkLabel1.Name = "linkLabel1";
      this.linkLabel1.Size = new System.Drawing.Size(170, 16);
      this.linkLabel1.TabIndex = 16;
      this.linkLabel1.TabStop = true;
      this.linkLabel1.Text = "https://twitter.com/GinsorKR";
      // 
      // label16
      // 
      this.label16.AutoSize = true;
      this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label16.ForeColor = System.Drawing.Color.Silver;
      this.label16.Location = new System.Drawing.Point(88, 412);
      this.label16.Name = "label16";
      this.label16.Size = new System.Drawing.Size(272, 16);
      this.label16.TabIndex = 17;
      this.label16.Text = "Invented by Ginsor, Improved by pinky";
      // 
      // pictureBox2
      // 
      this.pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
      this.pictureBox2.Image = global::GinsorAudioTool2Plus.Properties.Resources.GhostIcon;
      this.pictureBox2.Location = new System.Drawing.Point(43, 398);
      this.pictureBox2.Name = "pictureBox2";
      this.pictureBox2.Size = new System.Drawing.Size(45, 46);
      this.pictureBox2.TabIndex = 18;
      this.pictureBox2.TabStop = false;
      // 
      // FormAbout
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
      this.ClientSize = new System.Drawing.Size(457, 499);
      this.Controls.Add(this.linkLabel1);
      this.Controls.Add(this.label16);
      this.Controls.Add(this.pictureBox2);
      this.Controls.Add(this.label15);
      this.Controls.Add(this.label13);
      this.Controls.Add(this.label12);
      this.Controls.Add(this.label11);
      this.Controls.Add(this.label10);
      this.Controls.Add(this.label9);
      this.Controls.Add(this.label8);
      this.Controls.Add(this.label7);
      this.Controls.Add(this.label6);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.pictureBox1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.Name = "FormAbout";
      this.ShowInTaskbar = false;
      this.Text = "About";
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

		}

		private global::System.ComponentModel.IContainer components;

		private global::System.Windows.Forms.PictureBox pictureBox1;

		private global::System.Windows.Forms.Label label1;

		private global::System.Windows.Forms.Label label2;

		private global::System.Windows.Forms.Label label3;

		private global::System.Windows.Forms.Label label4;

		private global::System.Windows.Forms.Label label5;

		private global::System.Windows.Forms.Label label6;

		private global::System.Windows.Forms.Label label7;

		private global::System.Windows.Forms.Label label8;

		private global::System.Windows.Forms.Label label9;

		private global::System.Windows.Forms.Label label10;

		private global::System.Windows.Forms.Label label11;

		private global::System.Windows.Forms.Label label12;

		private global::System.Windows.Forms.Label label13;

		private global::System.Windows.Forms.Label label15;

		private global::System.Windows.Forms.LinkLabel linkLabel1;

		private global::System.Windows.Forms.Label label16;

		private global::System.Windows.Forms.PictureBox pictureBox2;
	}
}
