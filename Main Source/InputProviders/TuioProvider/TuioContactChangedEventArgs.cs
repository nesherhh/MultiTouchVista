using System;
using System.Drawing;
using System.Windows.Forms;
using Multitouch.Contracts;
using TUIO;

namespace TuioProvider
{
	class TuioContactChangedEventArgs : ContactChangedEventArgs
	{
		IContact contact;
		static Size monitorSize;

		static TuioContactChangedEventArgs()
		{
			monitorSize = SystemInformation.PrimaryMonitorSize;
		}

		public TuioContactChangedEventArgs(TuioCursor cursor, ContactState state)
		{
			contact = new TuioContact(cursor.getFingerID(), cursor.getX() * monitorSize.Width, cursor.getY() * monitorSize.Height, 10, 10, state);
		}

		public override IContact Contact
		{
			get { return contact; }
		}
	}
}