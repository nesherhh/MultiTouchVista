namespace Multitouch.Service.Views
{
	partial class DebugConsole
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DebugConsole));
			this.richTextBoxConsole = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// richTextBoxConsole
			// 
			this.richTextBoxConsole.BackColor = System.Drawing.Color.Black;
			this.richTextBoxConsole.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richTextBoxConsole.ForeColor = System.Drawing.Color.White;
			this.richTextBoxConsole.Location = new System.Drawing.Point(0, 0);
			this.richTextBoxConsole.Name = "richTextBoxConsole";
			this.richTextBoxConsole.ReadOnly = true;
			this.richTextBoxConsole.Size = new System.Drawing.Size(600, 345);
			this.richTextBoxConsole.TabIndex = 0;
			this.richTextBoxConsole.Text = "";
			// 
			// DebugConsole
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(600, 345);
			this.Controls.Add(this.richTextBoxConsole);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "DebugConsole";
			this.ShowIcon = false;
			this.Text = "Debug Console";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.RichTextBox richTextBoxConsole;

	}
}