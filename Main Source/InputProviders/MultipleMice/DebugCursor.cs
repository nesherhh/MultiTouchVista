using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using MultipleMice.Native;
using MultipleMice.Properties;
using Form=System.Windows.Forms.Form;
using FormBorderStyle=System.Windows.Forms.FormBorderStyle;

namespace MultipleMice
{
	class DebugCursor : Form
	{
		private bool isClosing;
		private Bitmap backgroundImage;
		private Bitmap feedbackImage;
		private Bitmap actionImage;
		private int xCenter;
		private int yCenter;
		
		public DebugCursor()
		{
			FormBorderStyle = FormBorderStyle.None;
			ShowInTaskbar = false;
			TopMost = true;
			backgroundImage = Resources.GlassDiscImage;
			actionImage = null;
			feedbackImage = new Bitmap(backgroundImage.Width, backgroundImage.Height);
			Width = feedbackImage.Width;
			Height = feedbackImage.Height;
			xCenter = Width / 2;
			yCenter = Height / 2;

			Cursor cursor = Cursors.Default;
			Bitmap defaultImage = new Bitmap(Cursors.Default.Size.Width, cursor.Size.Height);

			Graphics g = Graphics.FromImage(defaultImage);
			cursor.Draw(g, new Rectangle(0, 0, defaultImage.Width, defaultImage.Height));

			for (int y = 0; y < defaultImage.Height; y++)
			{
				for (int x = 0; x < defaultImage.Width; x++)
				{
					Color pixel = defaultImage.GetPixel(x, y);
					if (pixel.GetBrightness() < 0.01)
						defaultImage.SetPixel(x, y, Color.Red);
				}
			}

			ActionImage = defaultImage;
		}

		private void Render(Graphics g)
		{
			g.Clear(Color.Transparent);
			g.SmoothingMode = SmoothingMode.AntiAlias;
			try
			{
				g.DrawImage(actionImage, 0, 0);
			}
			catch
			{}
			g.SmoothingMode = SmoothingMode.None;
		}

		public void Show(Point point)
		{
			Location = new Point(point.X - xCenter, point.Y - yCenter);
			UpdateLayered();
			Show();
		}

		protected void UpdateLayered()
		{
			IntPtr dC = Win32Interop.GetDC(IntPtr.Zero);
			IntPtr hDC = Win32Interop.CreateCompatibleDC(dC);
			IntPtr zero = IntPtr.Zero;
			IntPtr hObject = IntPtr.Zero;
			Render(Graphics.FromImage(feedbackImage));
			try
			{
				zero = feedbackImage.GetHbitmap(Color.FromArgb(0));
				hObject = Win32Interop.SelectObject(hDC, zero);
				Win32Interop.SIZE psize = new Win32Interop.SIZE(Width, Height);
				Win32Interop.POINT pprSrc = new Win32Interop.POINT(0, 0);
				Win32Interop.POINT pptDst = new Win32Interop.POINT(Left, Top);
				Win32Interop.BLENDFUNCTION pblend = new Win32Interop.BLENDFUNCTION();
				pblend.BlendOp = 0;
				pblend.BlendFlags = 0;
				pblend.SourceConstantAlpha = 255;
				pblend.AlphaFormat = 1;
				Win32Interop.UpdateLayeredWindow(Handle, dC, ref pptDst, ref psize, hDC, ref pprSrc, 0, ref pblend, 2);
			}
			finally
			{
				Win32Interop.ReleaseDC(IntPtr.Zero, dC);
				if (zero != IntPtr.Zero)
				{
					Win32Interop.SelectObject(hDC, hObject);
					Win32Interop.DeleteObject(zero);
				}
				Win32Interop.DeleteDC(hDC);
			}
		}

		protected override void WndProc(ref Message msg)
		{
			if (msg.Msg == 33)
				msg.Result = (IntPtr)3;
			else
				base.WndProc(ref msg);
		}

		public Bitmap ActionImage
		{
			get { return actionImage; }
			set { actionImage = value; }
		}

		public new Bitmap BackgroundImage
		{
			get { return backgroundImage; }
			set { backgroundImage = value; }
		}

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle = (cp.ExStyle | 134742176);
				return cp;
			}
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			isClosing = true;
			base.OnClosing(e);
		}

		public bool IsClosing
		{
			get { return isClosing; }
		}
	}
}