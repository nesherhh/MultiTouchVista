using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MultiTouchVista
{
	public partial class Form1 : Form
	{
		WindowManager windowManager = new WindowManager();

		public Form1()
		{
			InitializeComponent();
			windowManager.WindowCreated += new WindowManager.WindowCreatedEventHandler(windowManager_WindowCreated);
			windowManager.WindowDestroyed += new WindowManager.WindowDestroyedEventHandler(windowManager_WindowDestroyed);
			windowManager.ParentForm = this;
			windowManager.Register(this.Handle);
		}

		void windowManager_WindowCreated(Window window)
		{
			this.listBox1.Items.Add(window.Handle.ToString() + " " + window.Text);
		}

		void windowManager_WindowDestroyed(Window window)
		{
			this.listBox1.Items.Remove(window.Handle.ToString() + " " + window.Text);
		}

		private void btnRotate_Click(object sender, EventArgs e)
		{
			MessageBox.Show("I havnt added the DWMAXX Reference yet, this is just an example of the WindowManager in action");
		}
	}
}
