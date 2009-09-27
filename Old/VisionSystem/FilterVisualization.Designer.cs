using AForge.Controls;

namespace WindowsFormsApplication1
{
	partial class FilterVisualization
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
			this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
			this.groupBox = new System.Windows.Forms.GroupBox();
			this.histogram = new Histogram();
			this.panelPreview = new System.Windows.Forms.Panel();
			this.groupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// propertyGrid1
			// 
			this.propertyGrid1.HelpVisible = false;
			this.propertyGrid1.Location = new System.Drawing.Point(225, 19);
			this.propertyGrid1.Name = "propertyGrid1";
			this.propertyGrid1.Size = new System.Drawing.Size(177, 160);
			this.propertyGrid1.TabIndex = 1;
			this.propertyGrid1.ToolbarVisible = false;
			// 
			// groupBox
			// 
			this.groupBox.AutoSize = true;
			this.groupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.groupBox.Controls.Add(this.histogram);
			this.groupBox.Controls.Add(this.panelPreview);
			this.groupBox.Controls.Add(this.propertyGrid1);
			this.groupBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox.Location = new System.Drawing.Point(0, 0);
			this.groupBox.Name = "groupBox";
			this.groupBox.Size = new System.Drawing.Size(596, 198);
			this.groupBox.TabIndex = 3;
			this.groupBox.TabStop = false;
			this.groupBox.Text = "Filter name";
			// 
			// histogram
			// 
			this.histogram.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.histogram.Location = new System.Drawing.Point(408, 19);
			this.histogram.Name = "histogram";
			this.histogram.Size = new System.Drawing.Size(182, 160);
			this.histogram.TabIndex = 4;
			// 
			// panelPreview
			// 
			this.panelPreview.Location = new System.Drawing.Point(6, 19);
			this.panelPreview.Name = "panelPreview";
			this.panelPreview.Size = new System.Drawing.Size(213, 160);
			this.panelPreview.TabIndex = 3;
			// 
			// FilterVisualization
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this.groupBox);
			this.Name = "FilterVisualization";
			this.Size = new System.Drawing.Size(596, 198);
			this.groupBox.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		protected System.Windows.Forms.PropertyGrid propertyGrid1;
		protected System.Windows.Forms.Panel panelPreview;
		protected System.Windows.Forms.GroupBox groupBox;
		protected AForge.Controls.Histogram histogram;
	}
}
