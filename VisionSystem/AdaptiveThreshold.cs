using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using AForge.Imaging.Filters;

namespace WindowsFormsApplication1
{
	class AdaptiveThreshold : FilterGrayToGray
	{
		public int Size { get; set; }
		public int Constant { get; set; }
		private Bitmap Destination { get; set; }

		public AdaptiveThreshold()
		{
			Size = 7;
			Constant = 7;
		}

		protected override unsafe void ProcessFilter(BitmapData imageData)
		{
			Destination = AForge.Imaging.Image.CreateGrayscaleImage(imageData.Width, imageData.Height);
			BitmapData dstData = Destination.LockBits(new Rectangle(0, 0, Destination.Width, Destination.Height), ImageLockMode.ReadWrite, Destination.PixelFormat);

			byte* srcPtr = (byte*)imageData.Scan0.ToPointer();
			byte* dstPtr = (byte*)dstData.Scan0.ToPointer();
			for (int y = 0; y < imageData.Height; y++)
			{
				for (int x = 0; x < imageData.Width; x++)
				{
					int mean = 0;
					int max = srcPtr[GetOffset(imageData.Stride, x, y)];
					int min = srcPtr[GetOffset(imageData.Stride, x, y)];


					for (int k = Size / 2; k < Size; k++)
					{
						for (int l = Size / 2; l < Size; l++)
						{
							byte pixel = srcPtr[GetOffset(imageData.Stride, x - Size / 2 + k, y - Size / 2 + l)];
							if (pixel > max)
								max = pixel;
							if (pixel < min)
								min = pixel;
						}
					}

					int tmp = max + min;
					tmp = tmp / 2;
					mean = tmp - Constant;
					if (srcPtr[GetOffset(imageData.Stride, x, y)] >= mean)
						dstPtr[GetOffset(dstData.Stride, x, y)] = 255;
					else
						dstPtr[GetOffset(dstData.Stride, x, y)] = 0;
				}
			}

			byte[] buffer = new byte[dstData.Width * dstData.Height];
			Marshal.Copy(dstData.Scan0, buffer, 0, buffer.Length);
			Marshal.Copy(buffer, 0, imageData.Scan0, buffer.Length);

			Destination.UnlockBits(dstData);
		}

		int GetOffset(int stride, int x, int y)
		{
			return y * stride + x;
		}
	}
}