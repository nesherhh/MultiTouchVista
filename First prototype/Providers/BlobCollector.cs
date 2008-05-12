using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows;
using AForge.Imaging;

namespace Danilins.Multitouch.Providers
{
	class BlobCollector
	{
		private BlobCounter counter;
		private double heightRatio;
		private double widthRatio;
		private double screenHeight;
		private double screenWidth;

		public BlobCollector()
		{
			counter = new BlobCounter();
			screenHeight = SystemParameters.FullPrimaryScreenHeight;
			screenWidth = SystemParameters.FullPrimaryScreenWidth;
		}

		public void Process(int width, int height, byte[] pixels)
		{
			widthRatio = screenWidth / width;
			heightRatio = screenHeight / height;

			Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
			Rectangle rect = new Rectangle(0, 0, width, height);
			BitmapData bits = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);
			Marshal.Copy(pixels, 0, bits.Scan0, pixels.Length);
			counter.ProcessImage(bits);
			bitmap.UnlockBits(bits);
		}

		public List<Rect> GetBlobs()
		{
			List<Rect> result = new List<Rect>();
			foreach (Rectangle rectangle in counter.GetObjectRectangles())
				result.Add(TransformCoordinates(rectangle));
			return result;
		}

		private Rect TransformCoordinates(Rectangle rectangle)
		{
			double x = rectangle.X * widthRatio;
			double y = rectangle.Y * heightRatio;
			double width = rectangle.Width * widthRatio;
			double height = rectangle.Height * heightRatio;
			return new Rect(x, y, width, height);
		}
	}
}