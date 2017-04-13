namespace DyeSkinFaker
{
	partial class FrmConfig
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
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.comboBox2 = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.comboBox3 = new System.Windows.Forms.ComboBox();
			this.button1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// comboBox1
			// 
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Location = new System.Drawing.Point(92, 12);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(402, 24);
			this.comboBox1.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(74, 17);
			this.label1.TabIndex = 1;
			this.label1.Text = "Large Dye";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(15, 49);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(71, 17);
			this.label2.TabIndex = 2;
			this.label2.Text = "Small Dye";
			// 
			// comboBox2
			// 
			this.comboBox2.FormattingEnabled = true;
			this.comboBox2.Location = new System.Drawing.Point(92, 42);
			this.comboBox2.Name = "comboBox2";
			this.comboBox2.Size = new System.Drawing.Size(402, 24);
			this.comboBox2.TabIndex = 3;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(51, 82);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(35, 17);
			this.label3.TabIndex = 4;
			this.label3.Text = "Skin";
			// 
			// comboBox3
			// 
			this.comboBox3.FormattingEnabled = true;
			this.comboBox3.Location = new System.Drawing.Point(92, 75);
			this.comboBox3.Name = "comboBox3";
			this.comboBox3.Size = new System.Drawing.Size(402, 24);
			this.comboBox3.TabIndex = 5;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(202, 114);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(113, 23);
			this.button1.TabIndex = 6;
			this.button1.Text = "Save";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// FrmConfig
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(506, 149);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.comboBox3);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.comboBox2);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.comboBox1);
			this.Name = "FrmConfig";
			this.Text = "DyeSkinFaker Config";
			this.Load += new System.EventHandler(this.Config_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox comboBox2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox comboBox3;
		private System.Windows.Forms.Button button1;
	}
}