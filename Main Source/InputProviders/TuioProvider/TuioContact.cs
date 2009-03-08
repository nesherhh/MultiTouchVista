using System;
using System.Windows;
using Multitouch.Contracts;
using TUIO;

namespace TuioProvider
{
	class TuioContact : Contact
	{
		public TuioContact(TuioCursor cursor, ContactState state, System.Drawing.Size monitorSize)
			: base(cursor.getFingerID(), state, new Point(0, 0), 20, 20)
		{
			float x = cursor.getScreenX(monitorSize.Width);
			float y = cursor.getScreenY(monitorSize.Height);

			Position = new Point(x, y);
			Orientation = 0;
		}
	}
}