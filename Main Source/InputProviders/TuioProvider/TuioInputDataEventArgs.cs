using System;
using System.Windows;
using System.Windows.Forms;
using Multitouch.Contracts;
using TUIO;

namespace TuioProvider
{
	class TuioInputDataEventArgs : InputDataEventArgs
	{
		static System.Drawing.Size monitorSize;
		object data;

		static TuioInputDataEventArgs()
		{
			monitorSize = SystemInformation.PrimaryMonitorSize;
		}

		public TuioInputDataEventArgs(TuioCursor cursor, ContactState state)
		{
			Size size = new Size(10, 10);
			float x = cursor.getX() * monitorSize.Width;
			float y = cursor.getY() * monitorSize.Height;
			Rect bounds = new Rect(x,y, size.Width, size.Height);
			data = new TuioContact(cursor.getFingerID(), x, y, size.Width, size.Height, 0, bounds, state);
		}

		public override InputType Type
		{
			get { return InputType.Contact; }
		}

		public override object Data
		{
			get { return data; }
		}
	}
}