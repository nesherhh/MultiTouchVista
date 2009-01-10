using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Multitouch.Contracts;
using TUIO;

namespace TuioProvider
{
	class TuioInputDataEventArgs : NewFrameEventArgs
	{
		static System.Drawing.Size monitorSize;
		IList<IImageData> images;
		IList<IContactData> contacts;
		long timestamp;

		static TuioInputDataEventArgs()
		{
			monitorSize = SystemInformation.PrimaryMonitorSize;
		}

		public TuioInputDataEventArgs(IEnumerable<TuioCursor> cursors, long timestamp)
		{
			images = new List<IImageData>();
			contacts = new List<IContactData>();
			this.timestamp = timestamp;

			foreach (TuioCursor cursor in cursors)
			{
				if(cursor.getState() == TuioCursor.UNDEFINED)
					continue;
				contacts.Add(new TuioContact(cursor, monitorSize));
			}
		}

		public override IList<IImageData> Images
		{
			get { return images; }
		}

		public override IList<IContactData> Contacts
		{
			get { return contacts; }
		}

		public override long Timestamp
		{
			get { return timestamp; }
		}
	}
}