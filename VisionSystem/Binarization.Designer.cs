using System;

namespace WindowsFormsApplication1
{
	partial class Binarization
	{
		private System.Windows.Forms.TrackBar trackBarThreshold;

		private void InitializeComponent()
		{
			this.trackBarThreshold = new System.Windows.Forms.TrackBar();
			this.labelThreshold = new System.Windows.Forms.Label();
			this.checkBoxAutoThreshold = new System.Windows.Forms.CheckBox();
			this.groupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackBarThreshold)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox
			// 
			this.groupBox.Controls.Add(this.checkBoxAutoThreshold);
			this.groupBox.Controls.Add(this.labelThreshold);
			this.groupBox.Controls.Add(this.trackBarThreshold);
			this.groupBox.Size = new System.Drawing.Size(599, 198);
			this.groupBox.Controls.SetChildIndex(this.trackBarThreshold, 0);
			this.groupBox.Controls.SetChildIndex(this.propertyGrid1, 0);
			this.groupBox.Controls.SetChildIndex(this.histogram, 0);
			this.groupBox.Controls.SetChildIndex(this.panelPreview, 0);
			this.groupBox.Controls.SetChildIndex(this.labelThreshold, 0);
			this.groupBox.Controls.SetChildIndex(this.checkBoxAutoThreshold, 0);
			// 
			// histogram
			// 
			this.histogram.Location = new System.Drawing.Point(408, 41);
			this.histogram.LogarithmicView = true;
			this.histogram.Size = new System.Drawing.Size(177, 108);
			// 
			// trackBarThreshold
			// 
			this.trackBarThreshold.AutoSize = false;
			this.trackBarThreshold.LargeChange = 1;
			this.trackBarThreshold.Location = new System.Drawing.Point(397, 155);
			this.trackBarThreshold.Maximum = 255;
			this.trackBarThreshold.Name = "trackBarThreshold";
			this.trackBarThreshold.Size = new System.Drawing.Size(196, 24);
			this.trackBarThreshold.TabIndex = 4;
			this.trackBarThreshold.TickStyle = System.Windows.Forms.TickStyle.None;
			this.trackBarThreshold.ValueChanged += new System.EventHandler(this.trackBarThreshold_ValueChanged);
			// 
			// labelThreshold
			// 
			this.labelThreshold.AutoSize = true;
			this.labelThreshold.Location = new System.Drawing.Point(408, 19);
			this.labelThreshold.Name = "labelThreshold";
			this.labelThreshold.Size = new System.Drawing.Size(17, 13);
			this.labelThreshold.TabIndex = 5;
			this.labelThreshold.Text = "xx";
			// 
			// checkBoxAutoThreshold
			// 
			this.checkBoxAutoThreshold.AutoSize = true;
			this.checkBoxAutoThreshold.Checked = true;
			this.checkBoxAutoThreshold.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxAutoThreshold.Location = new System.Drawing.Point(537, 18);
			this.checkBoxAutoThreshold.Name = "checkBoxAutoThreshold";
			this.checkBoxAutoThreshold.Size = new System.Drawing.Size(48, 17);
			this.checkBoxAutoThreshold.TabIndex = 6;
			this.checkBoxAutoThreshold.Text = "Auto";
			this.checkBoxAutoThreshold.UseVisualStyleBackColor = true;
			this.checkBoxAutoThreshold.CheckedChanged += new System.EventHandler(this.checkBoxAutoThreshold_CheckedChanged);
			// 
			// Binarization
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.Name = "Binarization";
			this.Size = new System.Drawing.Size(599, 198);
			this.groupBox.ResumeLayout(false);
			this.groupBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.trackBarThreshold)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		private System.Windows.Forms.Label labelThreshold;
		private System.Windows.Forms.CheckBox checkBoxAutoThreshold;
	}
}
