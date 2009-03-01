using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Multitouch.Contracts;

namespace TuioProvider
{
	class TuioInputDataEventArgs : NewFrameEventArgs
	{
		static readonly System.Drawing.Size monitorSize;
		readonly IList<IImageData> images;
		readonly IList<IContactData> contacts;
		readonly long timestamp;

		static TuioInputDataEventArgs()
		{
			monitorSize = SystemInformation.VirtualScreen.Size;
		}

		public TuioInputDataEventArgs(Queue<InputProvider.CursorState> cursors, long timestamp)
		{
			images = new List<IImageData>();
			contacts = new List<IContactData>();
			this.timestamp = timestamp;

			foreach (InputProvider.CursorState cursor in cursors)
				contacts.Add(new TuioContact(cursor.Cursor, cursor.State, monitorSize));
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