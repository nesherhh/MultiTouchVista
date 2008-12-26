using System;
using System.Runtime.Serialization;
using Multitouch.Contracts;

namespace Multitouch.Service.Logic.ExternalInterfaces
{
	[DataContract]
	public class ImageData : IImageData
	{
		public ImageData(IImageData imageData)
		{
			Width = imageData.Width;
			Height = imageData.Height;
			Stride = imageData.Stride;
			BitsPerPixel = imageData.BitsPerPixel;
			Type = imageData.Type;
			Data = imageData.Data;
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