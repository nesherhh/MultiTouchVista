using System;
using System.Collections.Generic;
using Multitouch.Contracts;

namespace MultipleMice
{
	class MouseContactChangedEventArgs : NewFrameEventArgs
	{
		List<IImageData> images;
		long timestamp;
		IList<IContactData> contacts;

		public MouseContactChangedEventArgs(IEnumerable<MouseContact> mouseContacts, long timestamp)
		{
			images = new List<IImageData>();

			contacts = new List<IContactData>();
			foreach (MouseContact contact in mouseContacts)
				contacts.Add(contact);

			this.timestamp = timestamp;
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
