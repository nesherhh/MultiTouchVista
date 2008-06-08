using System;

namespace DWMaxxAddIn
{
	class AccelerometerDataEventArgs : EventArgs
	{
		public int X { get; private set; }
		public int Y { get; private set; }

		public AccelerometerDataEventArgs(int x, int y)
		{
			X = x;
			Y = y;
		}
	}
}
