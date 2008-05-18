// Debug Console By Dean North 23/04/08
// This class needs optimising significantly, it should only be appending text 
// to the text box rather than replacing the entire console output every time 
// like it does now

using System;
using System.Windows.Forms;

namespace Multitouch.Service.Views
{
	public partial class DebugConsole : Form
	{
		public DebugConsole()
		{
			InitializeComponent();

			OutputStream outputStream = new OutputStream();
			outputStream.Updated += Updated;
			Console.SetOut(outputStream);
		}

		void Updated(object sender, EventArgs<string> e)
		{
			if (InvokeRequired)
				Invoke((Action<string>)UpdateDebugConsole, e.Item);
			else
				UpdateDebugConsole(e.Item);
		}

		void UpdateDebugConsole(string line)
		{
			richTextBoxConsole.AppendText(line + Environment.NewLine);
		}
	}
}