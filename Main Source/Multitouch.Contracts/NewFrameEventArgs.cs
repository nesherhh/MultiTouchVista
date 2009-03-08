using System;
using System.Collections.Generic;
using System.Linq;

namespace Multitouch.Contracts
{
	public class NewFrameEventArgs : EventArgs
	{
		public IEnumerable<Image> Images { get; private set; }
		public IEnumerable<Contact> Contacts { get; private set; }

		public NewFrameEventArgs(long timestamp, IEnumerable<Contact> contacts, IEnumerable<Image> images)
		{
			if (contacts == null)
				throw new ArgumentNullException("contacts");
			if (images == null)
				images = Enumerable.Empty<Image>();

			Images = images;
			Contacts = contacts;
			Timestamp = timestamp;
		}

		public long Timestamp { get; private set; }
	}
}