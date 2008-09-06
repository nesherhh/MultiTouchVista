namespace OpenCVTest.Filters
{
	partial class BlobsVisualization
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
			this.label1 = new System.Windows.Forms.Label();
			this.labelBlobsCount = new System.Windows.Forms.Label();
			this.groupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox
			// 
			this.groupBox.Controls.Add(this.label1);
			this.groupBox.Controls.Add(this.labelBlobsCount);
			this.groupBox.Controls.SetChildIndex(this.labelBlobsCount, 0);
			this.groupBox.Controls.SetChildIndex(this.label1, 0);
			this.groupBox.Controls.SetChildIndex(this.propertyGrid1, 0);
			this.groupBox.Controls.SetChildIndex(this.panelPreview, 0);
			this.groupBox.Controls.SetChildIndex(this.histogram, 0);
			// 
			// histogram
			// 
			this.histogram.Size = new System.Drawing.Size(182, 144);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(405, 166);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(66, 13);
			this.label1.TabIndex = 5;
			this.label1.Text = "Blobs count:";
			// 
			// labelBlobsCount
			// 
			this.labelBlobsCount.AutoSize = true;
			this.labelBlobsCount.Location = new System.Drawing.Point(477, 166);
			this.labelBlobsCount.Name = "labelBlobsCount";
			this.labelBlobsCount.Size = new System.Drawing.Size(17, 13);
			this.labelBlobsCount.TabIndex = 6;
			this.labelBlobsCount.Text = "xx";
			// 
			// BlobsVisualization
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Name = "BlobsVisualization";
			this.groupBox.ResumeLayout(false);
			this.groupBox.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labelBlobsCount;
	}
}
