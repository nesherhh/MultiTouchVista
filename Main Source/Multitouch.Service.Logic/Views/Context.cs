using System;
using System.Windows.Forms;
using Multitouch.Service.Logic.Properties;
using Multitouch.Service.Views;

namespace Multitouch.Service.Logic.Views
{
	class Context : ApplicationContext
	{
		NotifyIcon notifyIcon;
		MultitouchInput input;

		public Context()
		{
			ContextMenuStrip contextMenu = new ContextMenuStrip();
			contextMenu.Items.Add("&Debug Console", null, Debug_Click);
			contextMenu.Items.Add("&Options", null, Options_Click);
			contextMenu.Items.Add(new ToolStripSeparator());
			contextMenu.Items.Add("E&xit", null, Exit_Click);

			notifyIcon = new NotifyIcon();
			notifyIcon.Icon = Resources.MultitouchIcon;
			notifyIcon.Text = "MultiTouch Service";
			notifyIcon.ContextMenuStrip = contextMenu;
			notifyIcon.Visible = true;

			input = new MultitouchInput();
			//input.Start(); //For some reason starting the input prevents context menu mouse events
		}

		void Exit_Click(object sender, EventArgs e)
		{
			if(input != null)
				input.Stop();
			if (notifyIcon != null)
				notifyIcon.Visible = false;
			Application.Exit();
		}

		void Options_Click(object sender, EventArgs e)
		{
			new Options().Show();
		}

		void Debug_Click(object sender, EventArgs e)
		{
			new DebugConsole().Show();
		}
	}
}