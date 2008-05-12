using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using MultipleMice.Native;

namespace MultipleMice
{
	class DeviceStatus
	{
		readonly RawDevice device;
		DebugCursor debugCursor;
		SynchronizationContext syncContext;
		Point location;

		public DeviceStatus(RawDevice device)
		{
			this.device = device;

			syncContext = SynchronizationContext.Current;

			if (syncContext == null)
				syncContext = new SynchronizationContext();

			debugCursor = new DebugCursor();
			debugCursor.Name = "DebugCursor";
			Win32.POINT position = Win32.GetCursorPosition();
			Location = new Point(position.x, position.y);

			Thread t = new Thread(ThreadWorker);
			t.Name = "Cursor for device: " + device.Handle;
			t.SetApartmentState(ApartmentState.STA);
			t.IsBackground = true;
			t.Start();
		}

		void ThreadWorker()
		{
			debugCursor.Show(Location);
			Application.Run(debugCursor);
		}

		public Point Location
		{
			get { return location; }
			set
			{
				if (location != value)
				{
					location = value;
					UpdateLocation();
				}
			}
		}

		public IntPtr Handle
		{
			get { return device.Handle; }
		}

		void UpdateLocation()
		{
			SynchronizationContext current = SynchronizationContext.Current;
			current.Send(SyncUpdateLocation, null);
		}

		void SyncUpdateLocation(object state)
		{
			debugCursor.Location = Location;
		}

		public DeviceState ButtonState { get; set; }
	}
}
