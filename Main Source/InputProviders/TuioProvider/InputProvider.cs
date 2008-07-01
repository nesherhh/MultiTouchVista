using System;
using System.AddIn;
using Multitouch.Contracts;
using TUIO;

namespace TuioProvider
{
	[AddIn("Tuio", Publisher = "Daniel Danilin", Description = "Provides input from TUIO server.", Version = VERSION)]
	public class InputProvider : IProvider
	{
		TuioClient client;
		Listener listener;
		internal const string VERSION = "1.0.0.0";

		public event EventHandler<ContactChangedEventArgs> ContactChanged;

		public void Start()
		{
			client = new TuioClient();
			listener = new Listener(this);
			client.addTuioListener(listener);
			client.connect();
			IsRunning = true;
		}

		public void Stop()
		{
			if (client != null)
			{
				client.disconnect();
				if (listener != null)
					client.removeTuioListener(listener);
				IsRunning = false;
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