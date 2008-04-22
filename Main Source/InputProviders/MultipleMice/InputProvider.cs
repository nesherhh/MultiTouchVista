using System;
using System.AddIn;
using Multitouch.Contracts;

namespace MultipleMice
{
	[AddIn("MultipleMice", Publisher = "Daniel Danilin", Description = "Provides input from multiple mice", Version = VERSION)]
	public class InputProvider : IProvider
	{
		internal const string VERSION = "1.0.0.0";

		public event EventHandler<ContactChangedEventArgs> ContactChanged;

		RawDevicesManager deviceManager;

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

		internal void OnContactChanged(ContactChangedEventArgs e)
		{
			if (ContactChanged != null)
				ContactChanged(this, e);
		}
	}
}
