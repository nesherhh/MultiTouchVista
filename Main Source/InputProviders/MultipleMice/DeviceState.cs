using System;
using MultipleMice.Native;

namespace MultipleMice
{
	class DeviceState
	{
		readonly RawDevice device;

		public DeviceState(RawDevice device)
		{
			this.device = device;
			Win32.POINT position = Win32.GetCursorPosition();
			X = position.x;
			Y = position.y;
		}

		public IntPtr Handle
		{
			get { return device.Handle; }
		}

		public int Y { get; set; }
		public int X { get; set; }
		public MouseButtonState ButtonState { get; set; }
	}
}
