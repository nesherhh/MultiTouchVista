using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Multitouch.Framework.Input;

namespace TestConsoleApplication
{
	public partial class Form1 : Form, IContactHandler
	{
		Bitmap bitmap;

		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			CommunicationLogic logic = CommunicationLogic.Instance;

			logic.RegisterContactHandler(this);
			logic.Connect(Handle);
			if (logic.ShouldReceiveImage(ImageType.Normalized, true))
			{
				logic.EnableFrameEvent = true;
				logic.Frame += Instance_Frame;
			}
		}

		void Instance_Frame(object sender, FrameEventArgs e)
		{
			ImageData data;
			if (e.TryGetImage(ImageType.Normalized, 0, 0, 320, 240, out data))
			{
				if (bitmap == null)
				{
					bitmap = new Bitmap(data.Width, data.Height, PixelFormat.Format8bppIndexed);
					ColorPalette palette = bitmap.Palette;
					for (int i = 0; i < palette.Entries.Length; i++)
						palette.Entries[i] = Color.FromArgb(i, i, i);
					bitmap.Palette = palette;
				}

				BitmapData bits = bitmap.LockBits(new Rectangle(0, 0, data.Width, data.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
				Marshal.Copy(data.Data, 0, bits.Scan0, data.Data.Length);
				bitmap.UnlockBits(bits);

				using(Graphics graphics = Graphics.FromHwnd(Handle))
				{
					graphics.DrawImage(bitmap, 0, 0);
				}
			}
			else
				Console.WriteLine("no image received");
		}

		public void ProcessContactChange(int id, double x, double y, double width, double height, ContactState state)
		{

		}
	}
}