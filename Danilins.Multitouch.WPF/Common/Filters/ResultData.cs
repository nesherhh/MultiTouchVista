using System;

namespace Danilins.Multitouch.Common.Filters
{
	public class ResultData
	{
		public byte[] Pixels { get; private set; }
		public int Width { get; private set; }
		public int Height { get; private set; }
		public int BitPerPixel { get; private set; }

		public ResultData(int width, int height, int bitPerPixel, byte[] pixels)
		{
			Width = width;
			Height = height;
			BitPerPixel = bitPerPixel;
			Pixels = pixels;
		}
	}
}
