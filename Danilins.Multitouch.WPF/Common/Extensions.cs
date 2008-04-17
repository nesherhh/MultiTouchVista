using System;
using System.Windows.Media;

namespace Danilins.Multitouch.Common
{
	public static class Extensions
	{
		public static int BytePerPixel(this PixelFormat format)
		{
			return format.BitsPerPixel / 8;
		}

		public static int GetStride(this PixelFormat format, int width)
		{
			return format.BytePerPixel() * width;
		}
	}
}