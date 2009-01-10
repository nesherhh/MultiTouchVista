using System;
using System.Windows;
using Multitouch.Contracts;
using TUIO;

namespace TuioProvider
{
	class TuioContact : IContactData
	{
		public TuioContact(TuioCursor cursor, System.Drawing.Size monitorSize)
		{
			Size size = new Size(20, 20);
			float x = cursor.getScreenX(monitorSize.Width);
			float y = cursor.getScreenY(monitorSize.Height);
			Rect bounds = new Rect(x - size.Width / 2, y - size.Height / 2, size.Width, size.Height);

			ContactState contactState;
			int state = cursor.getState();
			if (state == TuioCursor.ADDED)
				contactState = ContactState.New;
			else if (state == TuioCursor.REMOVED)
				contactState = ContactState.Removed;
			else if (state == TuioCursor.UPDATED)
				contactState = ContactState.Moved;
			else
				throw new ArgumentException("invalid state", "cursor");

			Id = cursor.getFingerID();
			Bounds = bounds;
			Position = new Point(x, y);
			Orientation = 0;
			Area = bounds.Width * bounds.Height;
			MajorAxis = bounds.Width;
			MinorAxis = bounds.Height;
			State = contactState;
		}

		public int Id { get; private set; }
		public Rect Bounds { get; private set; }
		public Point Position { get; private set; }
		public double Orientation { get; private set; }
		public double Area { get; private set; }
		public double MajorAxis { get; private set; }
		public double MinorAxis { get; private set; }
		public ContactState State { get; private set; }
	}
}