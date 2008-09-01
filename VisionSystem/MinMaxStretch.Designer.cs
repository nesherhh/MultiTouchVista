namespace WindowsFormsApplication1
{
	partial class MinMaxStretch
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.trackBarMin = new System.Windows.Forms.TrackBar();
			this.trackBarMax = new System.Windows.Forms.TrackBar();
			this.labelMin = new System.Windows.Forms.Label();
			this.labelMax = new System.Windows.Forms.Label();
			this.groupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackBarMin)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBarMax)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox
			// 
			this.groupBox.Controls.Add(this.labelMax);
			this.groupBox.Controls.Add(this.labelMin);
			this.groupBox.Controls.Add(this.trackBarMax);
			this.groupBox.Controls.Add(this.trackBarMin);
			this.groupBox.Controls.SetChildIndex(this.trackBarMin, 0);
			this.groupBox.Controls.SetChildIndex(this.trackBarMax, 0);
			this.groupBox.Controls.SetChildIndex(this.propertyGrid1, 0);
			this.groupBox.Controls.SetChildIndex(this.panelPreview, 0);
			this.groupBox.Controls.SetChildIndex(this.histogram, 0);
			this.groupBox.Controls.SetChildIndex(this.labelMin, 0);
			this.groupBox.Controls.SetChildIndex(this.labelMax, 0);
			// 
			// histogram
			// 
			this.histogram.Size = new System.Drawing.Size(174, 98);
			// 
			// trackBarMin
			// 
			this.trackBarMin.AutoSize = false;
			this.trackBarMin.Location = new System.Drawing.Point(396, 123);
			this.trackBarMin.Maximum = 255;
			this.trackBarMin.Name = "trackBarMin";
			this.trackBarMin.Size = new System.Drawing.Size(194, 26);
			this.trackBarMin.TabIndex = 5;
			this.trackBarMin.TickStyle = System.Windows.Forms.TickStyle.None;
			this.trackBarMin.ValueChanged += new System.EventHandler(this.trackBarMin_ValueChanged);
			// 
			// trackBarMax
			// 
			this.trackBarMax.AutoSize = false;
			this.trackBarMax.LargeChange = 1;
			this.trackBarMax.Location = new System.Drawing.Point(401, 155);
			this.trackBarMax.Maximum = 255;
			this.trackBarMax.Name = "trackBarMax";
			this.trackBarMax.Size = new System.Drawing.Size(189, 24);
			this.trackBarMax.TabIndex = 6;
			this.trackBarMax.TickStyle = System.Windows.Forms.TickStyle.None;
			this.trackBarMax.Value = 255;
			this.trackBarMax.ValueChanged += new System.EventHandler(this.trackBarMax_ValueChanged);
			// 
			// labelMin
			// 
			this.labelMin.AutoSize = true;
			this.labelMin.Location = new System.Drawing.Point(408, 19);
			this.labelMin.Name = "labelMin";
			this.labelMin.Size = new System.Drawing.Size(17, 13);
			this.labelMin.TabIndex = 7;
			this.labelMin.Text = "xx";
			// 
			// labelMax
			// 
			this.labelMax.AutoSize = true;
			this.labelMax.Location = new System.Drawing.Point(547, 19);
			this.labelMax.Name = "labelMax";
			this.labelMax.Size = new System.Drawing.Size(17, 13);
			this.labelMax.TabIndex = 8;
			this.labelMax.Text = "xx";
			// 
			// MinMaxStretch
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Name = "MinMaxStretch";
			this.groupBox.ResumeLayout(false);
			this.groupBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackBarMin)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBarMax)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TrackBar trackBarMax;
		private System.Windows.Forms.TrackBar trackBarMin;
		private System.Windows.Forms.Label labelMax;
		private System.Windows.Forms.Label labelMin;
	}
}
