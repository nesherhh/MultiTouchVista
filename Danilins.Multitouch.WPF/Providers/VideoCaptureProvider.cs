using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using AForge.Video;
using AForge.Video.DirectShow;
using Danilins.Multitouch.Common.Providers;
using Danilins.Multitouch.Logic;
using Danilins.Multitouch.Providers.Configuration;

namespace Danilins.Multitouch.Providers
{
	[InputProvider("{46ECBE52-839C-4448-9C45-6A25DE1CC311}", "Video capture", true)]
	public class VideoCaptureProvider:BitmapProvider
	{
		private VideoCaptureDevice device;
		private object lockObject = new object();
		private byte[] lastPixels;
		private int width;
		private int bitPerPixel;
		private int height;

		public VideoCaptureProvider()
		{
			bitPerPixel = 24;
			nextFrame = new AutoResetEvent(false);

			FiltersSection section = MainLogic.Instance.ConfigurationLogic.GetSection<FiltersSection>(sectionName);
			Filters.LoadFrom(section);
		}

		public override void Start()
		{
			FilterInfoCollection info = new FilterInfoCollection(FilterCategory.VideoInputDevice);
			device = new VideoCaptureDevice(info[1].MonikerString);
			device.NewFrame += device_NewFrame;
			device.Start();
			isRunning = true;
		}

		public override Guid Id
		{
			get { return new Guid("{46ECBE52-839C-4448-9C45-6A25DE1CC311}"); }
		}

		public override string Name
		{
			get { return "Video capture"; }
		}

		void device_NewFrame(object sender, NewFrameEventArgs e)
		{
			Bitmap frame = e.Frame;
			width = frame.Width;
			height = frame.Height;

			lock (lockObject)
			{
				BitmapData bits = frame.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, frame.PixelFormat);
				bitPerPixel = Bitmap.GetPixelFormatSize(frame.PixelFormat);
				lastPixels = new byte[height * width * (bitPerPixel / 8)];
				Marshal.Copy(bits.Scan0, lastPixels, 0, lastPixels.Length);
				frame.UnlockBits(bits);
				frame.Dispose();
				nextFrame.Set();
			}
		}

		protected override byte[] GetData(out int width, out int height, out int bitPerPixel)
		{
			lock (lockObject)
			{
				width = this.width;
				height = this.height;
				bitPerPixel = this.bitPerPixel;
				return lastPixels;
			}
		}

		public override void Dispose()
		{
			Stop();
		}

		public override void Stop()
		{
			try
			{
				if (device != null)
					device.SignalToStop();
				isRunning = false;
			}
			catch (Exception)
			{
				isRunning = false;
			}
		}

		public override void Save()
		{
			ConfigurationLogic configurationLogic = MainLogic.Instance.ConfigurationLogic;
			FiltersSection section = configurationLogic.GetSection<FiltersSection>(sectionName);
			if (section == null)
			{
				section = new FiltersSection();
				configurationLogic.AddSection(sectionName, section);
			}
			Filters.SaveTo(section);
		}
	}
}