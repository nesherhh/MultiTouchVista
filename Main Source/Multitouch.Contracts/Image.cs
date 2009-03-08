using System;

namespace Multitouch.Contracts
{
	public class Image
	{
		public Image(ImageType type, int width, int height, int stride, int bitsPerPixel, byte[] data)
		{
			Width = width;
			Height = height;
			Stride = stride;
			BitsPerPixel = bitsPerPixel;
			Type = type;
			Data = data;
		}

		public int Width { get; private set; }
		public int Height { get; private set; }
		public int Stride { get; private set; }
		public int BitsPerPixel { get; private set; }
		public ImageType Type { get; private set; }
		public byte[] Data { get; private set; }
	}
}