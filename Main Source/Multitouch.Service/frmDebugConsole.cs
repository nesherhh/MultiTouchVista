// Debug Console By Dean North 23/04/08
// This class needs optimising significantly, it should only be appending text 
// to the text box rather than replacing the entire console output every time 
// like it does now

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Multitouch.Service
{
	public partial class frmDebugConsole : Form
	{
		public frmDebugConsole()
		{
			InitializeComponent();
			OutputStringWriter sw = new OutputStringWriter();
			Console.SetOut(sw);
			sw.AfterOutput += new OutputStringWriter.AfterOutputHandler(OnOutput); 
		}

		private void OnOutput(Object sender, EventArgs e)
		{
			//This causes threadding issues but i dont know the correct c# syntax to invoke this on the gui thread yet.
			this.textBox1.Text = sender.ToString();
			this.textBox1.Select(this.textBox1.Text.Length, 0);
		}

		void frmDebugConsole_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
		{
			e.Cancel = true;
			this.Hide();
		}

	}

	class OutputStringWriter : System.IO.StringWriter
	{
		public delegate void AfterOutputHandler(Object sender, EventArgs e);
		public event AfterOutputHandler AfterOutput;
		public override void WriteLine(string value)
		{
			base.WriteLine(value);
			AfterOutput(this, null);
		}
	}
}
