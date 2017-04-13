namespace TraderBot
{
	partial class FrmTraderBot
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
			this.radioButton1 = new System.Windows.Forms.RadioButton();
			this.radioButton2 = new System.Windows.Forms.RadioButton();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.button1 = new System.Windows.Forms.Button();
			this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
			this.listView1 = new System.Windows.Forms.ListView();
			this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.listView2 = new System.Windows.Forms.ListView();
			this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.label1 = new System.Windows.Forms.Label();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.button2 = new System.Windows.Forms.Button();
			this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
			this.SuspendLayout();
			// 
			// radioButton1
			// 
			this.radioButton1.AutoSize = true;
			this.radioButton1.Location = new System.Drawing.Point(12, 12);
			this.radioButton1.Name = "radioButton1";
			this.radioButton1.Size = new System.Drawing.Size(52, 21);
			this.radioButton1.TabIndex = 0;
			this.radioButton1.TabStop = true;
			this.radioButton1.Text = "Sell";
			this.radioButton1.UseVisualStyleBackColor = true;
			// 
			// radioButton2
			// 
			this.radioButton2.AutoSize = true;
			this.radioButton2.Location = new System.Drawing.Point(70, 12);
			this.radioButton2.Name = "radioButton2";
			this.radioButton2.Size = new System.Drawing.Size(53, 21);
			this.radioButton2.TabIndex = 1;
			this.radioButton2.TabStop = true;
			this.radioButton2.Text = "Buy";
			this.radioButton2.UseVisualStyleBackColor = true;
			// 
			// comboBox1
			// 
			this.comboBox1.FormattingEnabled = true;
			this.comboBox1.Location = new System.Drawing.Point(129, 12);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(325, 24);
			this.comboBox1.TabIndex = 2;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(510, 12);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(86, 24);
			this.button1.TabIndex = 3;
			this.button1.Text = "Add";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// numericUpDown1
			// 
			this.numericUpDown1.Location = new System.Drawing.Point(460, 14);
			this.numericUpDown1.Maximum = new decimal(new int[] {
            8,
            0,
            0,
            0});
			this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.numericUpDown1.Name = "numericUpDown1";
			this.numericUpDown1.Size = new System.Drawing.Size(44, 22);
			this.numericUpDown1.TabIndex = 4;
			this.numericUpDown1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// listView1
			// 
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
			this.listView1.FullRowSelect = true;
			this.listView1.GridLines = true;
			this.listView1.HideSelection = false;
			this.listView1.Location = new System.Drawing.Point(12, 42);
			this.listView1.MultiSelect = false;
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(289, 339);
			this.listView1.TabIndex = 5;
			this.listView1.UseCompatibleStateImageBehavior = false;
			this.listView1.View = System.Windows.Forms.View.Details;
			this.listView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listView1_KeyDown);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Qty";
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Item";
			this.columnHeader2.Width = 160;
			// 
			// listView2
			// 
			this.listView2.Activation = System.Windows.Forms.ItemActivation.OneClick;
			this.listView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
			this.listView2.FullRowSelect = true;
			this.listView2.GridLines = true;
			this.listView2.HideSelection = false;
			this.listView2.HoverSelection = true;
			this.listView2.Location = new System.Drawing.Point(307, 42);
			this.listView2.MultiSelect = false;
			this.listView2.Name = "listView2";
			this.listView2.Size = new System.Drawing.Size(289, 339);
			this.listView2.TabIndex = 6;
			this.listView2.UseCompatibleStateImageBehavior = false;
			this.listView2.View = System.Windows.Forms.View.Details;
			this.listView2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listView2_KeyDown);
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Qty";
			this.columnHeader3.Width = 40;
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "Item";
			this.columnHeader4.Width = 100;
			// 
			// columnHeader5
			// 
			this.columnHeader5.Text = "Alt Qty";
			this.columnHeader5.Width = 40;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 384);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(105, 17);
			this.label1.TabIndex = 7;
			this.label1.Text = "Spam Message";
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(123, 384);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(381, 22);
			this.textBox1.TabIndex = 8;
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(510, 383);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(86, 24);
			this.button2.TabIndex = 9;
			this.button2.Text = "Save All";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// columnHeader6
			// 
			this.columnHeader6.Text = "Atl Item";
			this.columnHeader6.Width = 100;
			// 
			// FrmTraderBot
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(610, 413);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listView2);
			this.Controls.Add(this.listView1);
			this.Controls.Add(this.numericUpDown1);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.comboBox1);
			this.Controls.Add(this.radioButton2);
			this.Controls.Add(this.radioButton1);
			this.Name = "FrmTraderBot";
			this.Text = "FrmTraderBot";
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.RadioButton radioButton1;
		private System.Windows.Forms.RadioButton radioButton2;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.NumericUpDown numericUpDown1;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ListView listView2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.ColumnHeader columnHeader6;
	}
}