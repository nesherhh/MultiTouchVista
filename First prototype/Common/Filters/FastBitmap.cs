using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Danilins.Multitouch.Common.Filters
{
	public unsafe class FastBitmap
	{
		public struct PixelData
		{
			public byte gray;
		}

		Bitmap bitmap;
		BitmapData bitmapData = null;
		Byte* pBase = null;
		bool isLocked = false;
		private int stride;
		private int width;
		private int height;

		public FastBitmap(Bitmap bitmap)
		{
			this.bitmap = bitmap;
			width = bitmap.Width;
			height = bitmap.Height;
		}

		public FastBitmap(int width, int height, int bitPerPixel, byte[] pixels)
		{
			this.width = width;
			this.height = height;
			PixelFormat format;
			if (bitPerPixel == 32)
				format = PixelFormat.Format32bppRgb;
			else if (bitPerPixel == 24)
				format = PixelFormat.Format24bppRgb;
			else
				format = PixelFormat.Format8bppIndexed;
			bitmap = new Bitmap(width, height, format);
			LockBits();
			Marshal.Copy(pixels, 0, Data().Scan0, pixels.Length);
			Release();
		}

		public void Release()
		{
			try
			{
				UnlockBits();
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		public Bitmap Bitmap
		{
			get
			{
				return bitmap;
			}
		}

		public byte this[int x, int y]
		{
			get { return this[GetIndex(x, y)]; }
			set { this[GetIndex(x,y)] = value; }
		}

		public byte this[int index]
		{
			get { return pBase[index]; }
			set { pBase[index] = value; }
		}

		public bool IsLocked() { return isLocked; }
		public BitmapData Data() { return bitmapData; }

		public void LockBits()
		{
			if (isLocked) return;
			try
			{
				bitmapData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
				stride = bitmapData.Stride;
				pBase = (byte*) bitmapData.Scan0.ToPointer();
			}
			finally
			{
				isLocked = true;
			}
		}

		public int GetIndex(int x, int y)
		{
			return y * stride + x;
		}

		private void UnlockBits()
		{
			if (bitmapData == null)
				return;
			bitmap.UnlockBits(bitmapData);
			bitmapData = null;
			pBase = null;
			isLocked = false;
		}

		public byte[] GetPixels()
		{
			byte[] result = new byte[bitmapData.Height * bitmapData.Stride];
			Marshal.Copy(bitmapData.Scan0, result, 0, result.Length);
			return result;
		}
	}
}