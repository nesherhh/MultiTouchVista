using AForge.Controls;

namespace WindowsFormsApplication1
{
	partial class FormAForge
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
			this.flowLayoutPanelFilters = new System.Windows.Forms.FlowLayoutPanel();
			this.buttonStart = new System.Windows.Forms.Button();
			this.buttonBackground = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// flowLayoutPanelFilters
			// 
			this.flowLayoutPanelFilters.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.flowLayoutPanelFilters.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.flowLayoutPanelFilters.Location = new System.Drawing.Point(12, 49);
			this.flowLayoutPanelFilters.Name = "flowLayoutPanelFilters";
			this.flowLayoutPanelFilters.Size = new System.Drawing.Size(260, 203);
			this.flowLayoutPanelFilters.TabIndex = 2;
			// 
			// buttonStart
			// 
			this.buttonStart.Location = new System.Drawing.Point(12, 12);
			this.buttonStart.Name = "buttonStart";
			this.buttonStart.Size = new System.Drawing.Size(75, 23);
			this.buttonStart.TabIndex = 0;
			this.buttonStart.Text = "Start";
			this.buttonStart.UseVisualStyleBackColor = true;
			this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
			// 
			// buttonBackground
			// 
			this.buttonBackground.Location = new System.Drawing.Point(93, 12);
			this.buttonBackground.Name = "buttonBackground";
			this.buttonBackground.Size = new System.Drawing.Size(75, 23);
			this.buttonBackground.TabIndex = 1;
			this.buttonBackground.Text = "Background";
			this.buttonBackground.UseVisualStyleBackColor = true;
			this.buttonBackground.Click += new System.EventHandler(this.buttonBackground_Click);
			// 
			// FormAForge
			// 
			this.AcceptButton = this.buttonStart;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 264);
			this.Controls.Add(this.buttonBackground);
			this.Controls.Add(this.buttonStart);
			this.Controls.Add(this.flowLayoutPanelFilters);
			this.Name = "FormAForge";
			this.Text = "FormAForge";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelFilters;
		private System.Windows.Forms.Button buttonStart;
		private System.Windows.Forms.Button buttonBackground;

	}
}