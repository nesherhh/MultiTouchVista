using System;
using System.Runtime.Serialization;
using Multitouch.Contracts;

namespace Multitouch.Service.Logic.ExternalInterfaces
{
	[DataContract]
	public class ImageData
	{
		internal ImageData(Image image)
		{
			Width = image.Width;
			Height = image.Height;
			Stride = image.Stride;
			BitsPerPixel = image.BitsPerPixel;
			Type = image.Type;
			Data = image.Data;
		}

		[DataMember]
		public int Width { get; private set; }
		[DataMember]
		public int Height { get; private set; }
		[DataMember]
		public int Stride { get; private set; }
		[DataMember]
		public int BitsPerPixel { get; private set; }
		[DataMember]
		public ImageType Type { get; private set; }
		[DataMember]
		public byte[] Data { get; private set; }
	}
}
