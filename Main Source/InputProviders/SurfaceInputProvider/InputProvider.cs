using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Windows;
using Multitouch.Contracts;

namespace SurfaceInputProvider
{
	[AddIn("Microsoft Surface", Publisher = "Daniel Danilin", Description = "Provides input from Microsoft Surface", Version = "1.0.0.0"), Export(typeof(IProvider))]
	public class InputProvider : IProvider
	{
		public void Start()
		{
			ThreadPool.QueueUserWorkItem(state => System.Windows.Forms.Application.Run(new InputContext(this)));
			IsRunning = true;
		}

		public void Stop()
		{
			System.Windows.Forms.Application.Exit();
			IsRunning = false;
		}

		public UIElement GetConfiguration()
		{
			return null;
		}

		public bool SendImageType(ImageType imageType, bool value)
		{
			return false;
		}

		public bool IsRunning { get; private set; }

		public bool HasConfiguration
		{
			get { return false; }
		}

		public bool SendEmptyFrames { get; set; }

		public event EventHandler<NewFrameEventArgs> NewFrame;

		internal void OnNewFrame(NewFrameEventArgs e)
		{
			EventHandler<NewFrameEventArgs> handler = NewFrame;
			if (handler != null)
				handler(this, e);
		}
	}
}
