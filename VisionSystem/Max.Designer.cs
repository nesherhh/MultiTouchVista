namespace WindowsFormsApplication1
{
	partial class Max
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
			this.labelMin = new System.Windows.Forms.Label();
			this.groupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackBarMin)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox
			// 
			this.groupBox.Controls.Add(this.labelMin);
			this.groupBox.Controls.Add(this.trackBarMin);
			this.groupBox.Controls.SetChildIndex(this.trackBarMin, 0);
			this.groupBox.Controls.SetChildIndex(this.propertyGrid1, 0);
			this.groupBox.Controls.SetChildIndex(this.panelPreview, 0);
			this.groupBox.Controls.SetChildIndex(this.histogram, 0);
			this.groupBox.Controls.SetChildIndex(this.labelMin, 0);
			// 
			// histogram
			// 
			this.histogram.Size = new System.Drawing.Size(173, 128);
			// 
			// trackBarMin
			// 
			this.trackBarMin.AutoSize = false;
			this.trackBarMin.LargeChange = 1;
			this.trackBarMin.Location = new System.Drawing.Point(397, 153);
			this.trackBarMin.Maximum = 255;
			this.trackBarMin.Name = "trackBarMin";
			this.trackBarMin.Size = new System.Drawing.Size(193, 26);
			this.trackBarMin.TabIndex = 5;
			this.trackBarMin.TickStyle = System.Windows.Forms.TickStyle.None;
			this.trackBarMin.Value = 255;
			this.trackBarMin.ValueChanged += new System.EventHandler(this.trackBarMin_ValueChanged);
			// 
			// labelMin
			// 
			this.labelMin.AutoSize = true;
			this.labelMin.Location = new System.Drawing.Point(408, 19);
			this.labelMin.Name = "labelMin";
			this.labelMin.Size = new System.Drawing.Size(17, 13);
			this.labelMin.TabIndex = 6;
			this.labelMin.Text = "xx";
			// 
			// Max
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Name = "Max";
			this.groupBox.ResumeLayout(false);
			this.groupBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackBarMin)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TrackBar trackBarMin;
		private System.Windows.Forms.Label labelMin;
	}
}
