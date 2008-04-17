using System;
using System.Drawing;
using System.Windows.Forms;

namespace Danilins.Multitouch.Providers
{
	internal class MousePointer : Logic.LegacySupport.Cursors.Cursor
	{
		// Methods
		public MousePointer()
		{
			Cursor cursor = Cursors.Default;
			Bitmap defaultImage = new Bitmap(Cursors.Default.Size.Width, cursor.Size.Height);
			Graphics g = Graphics.FromImage(defaultImage);
			cursor.Draw(g, new Rectangle(0, 0, defaultImage.Width, defaultImage.Height));

			ActionImage = defaultImage;
		}
	}
}