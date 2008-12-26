using System;
using System.AddIn;
using System.Windows;
using Multitouch.Contracts;

namespace MultipleMice
{
	[AddIn("MultipleMice", Publisher = "Daniel Danilin", Description = "Provides input from multiple mice", Version = VERSION)]
	public class InputProvider : IProvider
	{
		internal const string VERSION = "2.0.0.0";

		public event EventHandler<InputDataEventArgs> Input;

		RawDevicesManager deviceManager;

		public bool SendImageType(ImageType imageType, bool isEnable)
		{
			return false;
		}

		public void Start()
		{
			deviceManager = new RawDevicesManager(this);
		}

		public void Stop()
		{
			if (deviceManager != null)
			{
				deviceManager.Dispose();
				deviceManager = null;
			}
		}

		public bool IsRunning { get; internal set; }

		internal void RaiseInput(MouseContact contact)
		{
			EventHandler<InputDataEventArgs> eventHandler = Input;
			if (eventHandler != null)
				eventHandler(this, new MouseContactChangedEventArgs(contact));
		}

		public UIElement GetConfiguration()
		{
			return null;
		}

		public bool HasConfiguration
		{
			get { return false; }
		}
	}
}