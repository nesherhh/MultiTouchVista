using System;
using System.Runtime.Serialization;

namespace Danilins.Multitouch.Common
{
	[DataContract(Namespace = "http://Danilins.Multitouch")]
	public class ImageData
	{
		[DataMember]
		int width;
		[DataMember]
		int height;
		[DataMember]
		int bitPerPixel;
		[DataMember]
		byte[] pixels;

		public ImageData(int width, int height, int bitPerPixel, byte[] pixels)
		{
			this.width = width;
			this.pixels = pixels;
			this.bitPerPixel = bitPerPixel;
			this.height = height;
		}

		public int Width
		{
			get { return width; }
		}

		public int Height
		{
			get { return height; }
		}

		public int BitPerPixel
		{
			get { return bitPerPixel; }
		}

		public byte[] Pixels
		{
			get { return pixels; }
		}
	}
}
