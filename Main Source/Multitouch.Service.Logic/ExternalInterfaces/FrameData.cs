using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Multitouch.Contracts;

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

		public FrameData(long timestamp, IEnumerable<ContactData> contacts, IEnumerable<IImageData> images)
		{
			Timestamp = timestamp;
			Contacts = contacts.ToArray();
			Images = images.Select(i => new ImageData(i)).ToArray();
		}
	}
}