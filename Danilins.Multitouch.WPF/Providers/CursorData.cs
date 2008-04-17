using System;
using System.Windows;
using Danilins.Multitouch.Providers.Native;

namespace Danilins.Multitouch.Providers
{
	class CursorData
	{
		private double left;
		private double top;

		private readonly MousePointer pointer;
		private MouseButtonState buttonState;
		private readonly double screenWidth;
		private readonly double screenHeight;

		public CursorData()
		{
			buttonState = MouseButtonState.None;
			pointer = new MousePointer();
			pointer.Show(new System.Drawing.Point((int)X, (int)Y));
			screenWidth = SystemParameters.PrimaryScreenWidth;
			screenHeight = SystemParameters.PrimaryScreenHeight;
			SetPosition((int)(screenWidth / 2), (int)(screenHeight / 2));
		}

		public MouseButtonState ButtonState
		{
			get { return buttonState; }
			set { buttonState = value; }
		}

		public double Y
		{
			get { return top; }
		}

		public double X
		{
			get { return left; }
		}

		public void SetPosition(double x, double y)
		{
			double newLeft = x;
			double newTop = y;
			ValidateNewCursorPosition(ref newLeft, ref newTop);
			pointer.Location = new System.Drawing.Point((int)x, (int)y);
			//Win32.SetWindowPos(pointer.Handle, IntPtr.Zero, (int)newLeft, (int)newTop, 0, 0, 17693);
			left = newLeft;
			top = newTop;
		}

		internal void ValidateNewCursorPosition(ref double newLeft, ref double newTop)
		{
			if (newLeft <= 0.0)
				newLeft = 0.0;
			if (newTop <= 0.0)
				newTop = 0.0;

			if (newLeft >= screenWidth)
				newLeft = screenWidth;
			if (newTop >= screenHeight)
				newTop = screenHeight;
		}

		internal void CloseCursor()
		{
			pointer.Close();
		}
	}
}