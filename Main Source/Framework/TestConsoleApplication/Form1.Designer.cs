namespace TestConsoleApplication
{
	partial class Form1
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
			this.groupBoxNormalized = new System.Windows.Forms.GroupBox();
			this.groupBoxBinarized = new System.Windows.Forms.GroupBox();
			this.SuspendLayout();
			// 
			// groupBoxNormalized
			// 
			this.groupBoxNormalized.Location = new System.Drawing.Point(0, 0);
			this.groupBoxNormalized.Name = "groupBoxNormalized";
			this.groupBoxNormalized.Size = new System.Drawing.Size(320, 240);
			this.groupBoxNormalized.TabIndex = 0;
			this.groupBoxNormalized.TabStop = false;
			this.groupBoxNormalized.Text = "Normalized";
			// 
			// groupBoxBinarized
			// 
			this.groupBoxBinarized.Location = new System.Drawing.Point(326, 0);
			this.groupBoxBinarized.Name = "groupBoxBinarized";
			this.groupBoxBinarized.Size = new System.Drawing.Size(320, 240);
			this.groupBoxBinarized.TabIndex = 1;
			this.groupBoxBinarized.TabStop = false;
			this.groupBoxBinarized.Text = "Binarized";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(659, 257);
			this.Controls.Add(this.groupBoxBinarized);
			this.Controls.Add(this.groupBoxNormalized);
			this.Name = "Form1";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBoxNormalized;
		private System.Windows.Forms.GroupBox groupBoxBinarized;

	}
}