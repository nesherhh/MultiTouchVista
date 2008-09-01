using System;

namespace WindowsFormsApplication1
{
	partial class RemoveBackground
	{
		private System.Windows.Forms.TrackBar trackBarThreshold;
		private void InitializeComponent()
		{
			this.trackBarThreshold = new System.Windows.Forms.TrackBar();
			this.labelThreshold = new System.Windows.Forms.Label();
			this.groupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackBarThreshold)).BeginInit();
			this.SuspendLayout();
			// 
			// propertyGrid1
			// 
			this.propertyGrid1.Size = new System.Drawing.Size(177, 128);
			// 
			// groupBox
			// 
			this.groupBox.Controls.Add(this.trackBarThreshold);
			this.groupBox.Controls.Add(this.labelThreshold);
			this.groupBox.Controls.SetChildIndex(this.labelThreshold, 0);
			this.groupBox.Controls.SetChildIndex(this.propertyGrid1, 0);
			this.groupBox.Controls.SetChildIndex(this.trackBarThreshold, 0);
			this.groupBox.Controls.SetChildIndex(this.panelPreview, 0);
			this.groupBox.Controls.SetChildIndex(this.histogram, 0);
			// 
			// trackBarThreshold
			// 
			this.trackBarThreshold.AutoSize = false;
			this.trackBarThreshold.Location = new System.Drawing.Point(248, 153);
			this.trackBarThreshold.Maximum = 255;
			this.trackBarThreshold.Name = "trackBarThreshold";
			this.trackBarThreshold.Size = new System.Drawing.Size(154, 26);
			this.trackBarThreshold.TabIndex = 5;
			this.trackBarThreshold.TickStyle = System.Windows.Forms.TickStyle.None;
			this.trackBarThreshold.Value = 8;
			this.trackBarThreshold.ValueChanged += new System.EventHandler(this.trackBarThreshold_ValueChanged);
			// 
			// labelThreshold
			// 
			this.labelThreshold.AutoSize = true;
			this.labelThreshold.Location = new System.Drawing.Point(225, 150);
			this.labelThreshold.Name = "labelThreshold";
			this.labelThreshold.Size = new System.Drawing.Size(17, 13);
			this.labelThreshold.TabIndex = 6;
			this.labelThreshold.Text = "xx";
			// 
			// RemoveBackground
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.Name = "RemoveBackground";
			this.groupBox.ResumeLayout(false);
			this.groupBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackBarThreshold)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private System.Windows.Forms.Label labelThreshold;
	}
}
