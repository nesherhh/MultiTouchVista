using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Multitouch.Service.Logic.ExternalInterfaces
{
	[DataContract]
	public class FrameData
	{
		[DataMember]
		public ImageData[] Images { get; private set; }
		[DataMember]
		public ContactData[] Contacts { get; private set; }
		[DataMember]
		public long Timestamp { get; private set; }

		public FrameData(long timestamp, IEnumerable<ContactData> contacts, IEnumerable<ImageData> images)
		{
			if (contacts == null)
				throw new ArgumentNullException("contacts");
			if (images == null)
				images = Enumerable.Empty<ImageData>();

			Timestamp = timestamp;
			Contacts = contacts.ToArray();
			Images = images.ToArray();
		}
	}
}