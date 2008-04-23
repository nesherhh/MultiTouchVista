using System;
using System.Windows.Forms;

namespace Multitouch.Service
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			//ServiceBase[] ServicesToRun;
			//ServicesToRun = new ServiceBase[] 
			//{ 
			//    new MultitouchService() 
			//};
			//ServiceBase.Run(ServicesToRun);

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			debug = new frmDebugConsole();

			// 
			// contextMenu
			//
			ContextMenuStrip contextMenu = new ContextMenuStrip();
			contextMenu.Items.Add("Debug Console", null, Debug_Click);
			contextMenu.Items.Add("Options", null, Options_Click);
			contextMenu.Items.Add(new ToolStripSeparator());
			contextMenu.Items.Add("Exit", null, Exit_Click);
			// 
			// notifyIcon
			// 
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmOptions));
			NotifyIcon notifyIcon = new NotifyIcon();
			notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
			notifyIcon.Text = "MultiTouch Service";
			notifyIcon.ContextMenuStrip = contextMenu;
			notifyIcon.Visible = true;

			MultitouchInput input = new MultitouchInput();
			//input.Start(); //For some reason starting the input prevents context menu mouse events
			Console.WriteLine("Multitouch service is running.");
			Application.Run();
			notifyIcon.Visible = false;
			input.Stop();

		}

		static frmDebugConsole debug;
		static void Debug_Click(Object sender, EventArgs e)
		{
			debug.Show();
		}

		static void Options_Click(Object sender, EventArgs e)
		{
			frmOptions options = new frmOptions();
			options.Show();
		}

		static void Exit_Click(Object sender, EventArgs e)
		{
			// Manualy dispose the debug console because it is set to cancel standard form closing
			debug.Dispose();
			Application.Exit();
		}

	}
}
