using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Multitouch.Framework.Input;

namespace TestConsoleApplication
{
	public partial class Form1 : Form
	{
		Bitmap bitmap;
		ContactHandler contactHandler;

		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			contactHandler = new ContactHandler(Handle);
			contactHandler.NewContact += HandleContact;
			contactHandler.ContactRemoved += HandleContact;
			contactHandler.ContactMoved += HandleContact;
			contactHandler.Frame += HandleFrame;
			contactHandler.ReceiveImageType(ImageType.Normalized, true);
			contactHandler.ReceiveImageType(ImageType.Binarized, true);
			contactHandler.ReceiveEmptyFrames = true;
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			contactHandler.Dispose();
			base.OnClosing(e);
		}

		void HandleContact(object sender, ContactEventArgs e)
		{
			Console.WriteLine(e.Contact);
		}

		void HandleFrame(object sender, FrameEventArgs e)
		{
			UpdateImage(e, ImageType.Normalized, groupBoxNormalized);
			UpdateImage(e, ImageType.Binarized, groupBoxBinarized);
		}

		void UpdateImage(FrameEventArgs e, ImageType imageType, IWin32Window panel)
		{
			ImageData data;
			if (e.TryGetImage(imageType, 0, 0, 320, 240, out data))
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

				DrawImage(panel.Handle, bitmap);
			}
			else
				Console.WriteLine("no image received");
		}

		void DrawImage(IntPtr handle, Image image)
		{
			using (Graphics graphics = Graphics.FromHwnd(handle))
			{
				graphics.DrawImage(image, 0, 0);
			}
		}
	}
}