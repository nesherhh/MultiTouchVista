using System;

namespace Multitouch.Framework.Input
{
	/// <summary>
	/// Image data
	/// </summary>
	public class ImageData
	{
		internal ImageData(int width, int height, int stride, int bitsPerPixel, byte[] pixels)
		{
			Width = width;
			Height = height;
			Stride = stride;
			BitsPerPixel = bitsPerPixel;
			Data = pixels;
		}

		/// <summary>
		/// Image width
		/// </summary>
		public int Width { get; private set; }
		/// <summary>
		/// Image height
		/// </summary>
		public int Height { get; private set; }
		/// <summary>
		/// Image stride
		/// </summary>
		public int Stride { get; private set; }
		/// <summary>
		/// Image bits per pixel
		/// </summary>
		public int BitsPerPixel { get; private set; }
		/// <summary>
		/// Data
		/// </summary>
		public byte[] Data { get; private set; }
	}
}