using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using AForge.Video;
using AForge.Video.VFW;
using Danilins.Multitouch.Common.Providers;
using Danilins.Multitouch.Logic;
using Danilins.Multitouch.Providers.Configuration;

namespace Danilins.Multitouch.Providers
{
	[InputProvider("{2AF2D31F-A13B-4993-B4C3-D0B846BC68E8}", "Video file", true)]
	public class VideoFileProvider : BitmapProvider
	{
		AVIFileVideoSource source;
		private byte[] lastPixels;
		private object lockObject = new object();
		private Timer timer;
		private int width;
		private int bitPerPixel;
		private int height;
		private string sourceFile;

		public VideoFileProvider()
		{
			bitPerPixel = 24;

			VideoFileProviderSection section = MainLogic.Instance.ConfigurationLogic.GetSection<VideoFileProviderSection>(sectionName);
			Filters.LoadFrom(section);
			if(section != null)
				sourceFile = section.VideoFile;
		}

		public override void Start()
		{
			source = new AVIFileVideoSource();
			source.FrameIntervalFromSource = true;
			//source.FrameInterval = 200;
			source.Source = sourceFile;
			source.NewFrame += source_NewFrame;
			timer = new Timer(timer_Callback);
			timer.Change(1000, 500);
			source.Start();

			isRunning = true;
		}

		public override Guid Id
		{
			get { return new Guid("{2AF2D31F-A13B-4993-B4C3-D0B846BC68E8}"); }
		}

		public override string Name
		{
			get { return "Video file"; }
		}

		private void timer_Callback(object state)
		{
			if (!source.IsRunning)
				source.Start();
		}

		void source_NewFrame(object sender, NewFrameEventArgs e)
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
				if (source != null)
					source.SignalToStop();
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
			VideoFileProviderSection section = configurationLogic.GetSection<VideoFileProviderSection>(sectionName);
			if (section == null)
			{
				section = new VideoFileProviderSection();
				configurationLogic.AddSection(sectionName, section);
			}
			section.VideoFile = sourceFile;
			Filters.SaveTo(section);
		}
	}
}