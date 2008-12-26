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

		public void SetImages(IEnumerable<IImageData> images)
		{
			Images = images.Select(i => new ImageData(i)).ToArray();
		}
	}
}