using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using MultipleMice.Native;

namespace MultipleMice
{
	class DeviceState
	{
		readonly RawDevice device;
		DebugCursor debugCursor;
		int x;
		int y;

		public DeviceState(RawDevice device)
		{
			debugCursor = new DebugCursor();
			this.device = device;
			Win32.POINT position = Win32.GetCursorPosition();
			X = position.x;
			Y = position.y;

			Thread t = new Thread(ThreadWorker);
			t.SetApartmentState(ApartmentState.STA);
			t.IsBackground = true;
			t.Start();
		}

		void ThreadWorker()
		{
			//if (debugCursor.InvokeRequired)
			//    debugCursor.Invoke((Action)ThreadWorker);
			debugCursor.Show(new Point(X, Y));
			Application.Run();
		}

		public IntPtr Handle
		{
			get { return device.Handle; }
		}

		public int X
		{
			get { return x; }
			set
			{
				x = value;
				UpdateLocation();
			}
		}

		public int Y
		{
			get { return y; }
			set
			{
				y = value;
				UpdateLocation();
			}
		}

		void UpdateLocation()
		{
			if (debugCursor.InvokeRequired)
				debugCursor.Invoke((Action)(() => debugCursor.Location = new Point(X, Y)));
			else
				debugCursor.Location = new Point(X, Y);
		}

		public MouseButtonState ButtonState { get; set; }
	}
}
