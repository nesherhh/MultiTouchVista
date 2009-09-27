namespace OpenCVTest.Filters
{
	partial class RemoveBackgroundVisualization
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
			this.trackBarTreshold = new System.Windows.Forms.TrackBar();
			this.labelThreshold = new System.Windows.Forms.Label();
			this.groupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackBarTreshold)).BeginInit();
			this.SuspendLayout();
			// 
			// propertyGrid1
			// 
			this.propertyGrid1.Size = new System.Drawing.Size(177, 131);
			// 
			// groupBox
			// 
			this.groupBox.Controls.Add(this.trackBarTreshold);
			this.groupBox.Controls.Add(this.labelThreshold);
			this.groupBox.Controls.SetChildIndex(this.labelThreshold, 0);
			this.groupBox.Controls.SetChildIndex(this.trackBarTreshold, 0);
			this.groupBox.Controls.SetChildIndex(this.propertyGrid1, 0);
			this.groupBox.Controls.SetChildIndex(this.panelPreview, 0);
			this.groupBox.Controls.SetChildIndex(this.histogram, 0);
			// 
			// trackBarTreshold
			// 
			this.trackBarTreshold.AutoSize = false;
			this.trackBarTreshold.Location = new System.Drawing.Point(248, 156);
			this.trackBarTreshold.Maximum = 255;
			this.trackBarTreshold.Name = "trackBarTreshold";
			this.trackBarTreshold.Size = new System.Drawing.Size(154, 23);
			this.trackBarTreshold.TabIndex = 5;
			this.trackBarTreshold.TickStyle = System.Windows.Forms.TickStyle.None;
			this.trackBarTreshold.Value = 8;
			this.trackBarTreshold.ValueChanged += new System.EventHandler(this.trackBarTreshold_ValueChanged);
			// 
			// labelThreshold
			// 
			this.labelThreshold.AutoSize = true;
			this.labelThreshold.Location = new System.Drawing.Point(225, 156);
			this.labelThreshold.Name = "labelThreshold";
			this.labelThreshold.Size = new System.Drawing.Size(17, 13);
			this.labelThreshold.TabIndex = 6;
			this.labelThreshold.Text = "xx";
			// 
			// RemoveBackgroundVisualization
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Name = "RemoveBackgroundVisualization";
			this.groupBox.ResumeLayout(false);
			this.groupBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackBarTreshold)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TrackBar trackBarTreshold;
		private System.Windows.Forms.Label labelThreshold;
	}
}