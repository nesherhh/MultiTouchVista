using System;
using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color=System.Windows.Media.Color;
using Point=System.Windows.Point;
using WFColor = System.Drawing.Color;
using WFBitmap = System.Drawing.Bitmap;
using WFImageFormat = System.Drawing.Imaging.ImageFormat;
using WFCursor = System.Windows.Forms.Cursor;
using WFCursors = System.Windows.Forms.Cursors;
using WFRectangle = System.Drawing.Rectangle;

namespace Danilins.Multitouch.Providers
{
	internal class MousePointerWPF : Logic.LegacySupport.Cursors.CursorWPF
	{
		// Fields
		private Color cursorColor;
		private BitmapImage cursorImage;
		internal BitmapImage defaultImage;
		private bool defaultImageUsed;

		private WFBitmap wfBitmap;

		// Methods
		public MousePointerWPF(Point location)
			: base(location, 31, 27)
		{
			InitializeMouse();
		}

		private void InitializeMouse()
		{
			defaultImageUsed = true;
			MemoryStream stream = new MemoryStream();

			WFCursor cursor = WFCursors.Default;
			wfBitmap = new WFBitmap(WFCursors.Default.Size.Width, cursor.Size.Height);
			Graphics g = Graphics.FromImage(wfBitmap);
			cursor.Draw(g, new WFRectangle(0, 0, wfBitmap.Width, wfBitmap.Height));

			WFBitmap bitmap = wfBitmap;
			bitmap.MakeTransparent(WFColor.Magenta);
			bitmap.Save(stream, WFImageFormat.Png);
			stream.Position = 0L;
			defaultImage = new BitmapImage();
			defaultImage.BeginInit();
			defaultImage.StreamSource = stream;
			defaultImage.EndInit();
			image.Source = defaultImage;
		}

		internal void ChangeCursorColor(Color newColor)
		{
			WFBitmap bitmap = wfBitmap;
			bitmap.MakeTransparent(WFColor.Magenta);
			WFColor color = WFColor.FromArgb(newColor.A, newColor.R, newColor.G, newColor.B);
			for (int i = 0; i < (bitmap.Width - 1); i++)
			{
				for (int j = 0; j < (bitmap.Height - 1); j++)
				{
					WFColor pixel = bitmap.GetPixel(i, j);
					if (((pixel.A == WFColor.Black.A) && (pixel.R == WFColor.Black.R)) && ((pixel.B == WFColor.Black.B) && (pixel.G == WFColor.Black.G)))
						bitmap.SetPixel(i, j, color);
				}
			}
			MemoryStream stream = new MemoryStream();
			bitmap.Save(stream, WFImageFormat.Png);
			stream.Position = 0L;

			BitmapImage bitmapImage = new BitmapImage();
			bitmapImage.BeginInit();
			bitmapImage.StreamSource = stream;
			bitmapImage.EndInit();
			image.Source = bitmapImage;
		}

		internal void ChangeCursorImage(BitmapImage newCursorImage)
		{
			image.Source = newCursorImage;
		}

		// Properties
		internal Color CursorColor
		{
			get
			{
				if (cursorImage == null)
					return cursorColor;
				return Colors.Transparent;
			}
			set
			{
				cursorColor = value;
				defaultImageUsed = true;
				ChangeCursorColor(cursorColor);
			}
		}

		internal BitmapImage CursorImage
		{
			get
			{
				if (cursorImage != null)
					return cursorImage;
				return defaultImage;
			}
			set
			{
				cursorImage = value;
				defaultImageUsed = false;
				ChangeCursorImage(cursorImage);
				cursorColor = Colors.Transparent;
			}
		}

		internal bool DefaultImageUsed
		{
			get { return defaultImageUsed; }
		}
	}
}