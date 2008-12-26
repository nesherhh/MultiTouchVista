using System;
using System.AddIn;
using System.Windows;
using Multitouch.Contracts;
using TUIO;

namespace TuioProvider
{
	[AddIn("Tuio", Publisher = "Daniel Danilin", Description = "Provides input from TUIO server.", Version = VERSION)]
	public class InputProvider : IProvider
	{
		TuioClient client;
		Listener listener;
		internal const string VERSION = "2.0.0.0";

		public event EventHandler<InputDataEventArgs> Input;

		public bool SendImageType(ImageType imageType, bool isEnable)
		{
			return false;
		}

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

		public UIElement GetConfiguration()
		{
			return null;
		}

		public bool IsRunning { get; internal set; }

		public bool HasConfiguration
		{
			get { return false; }
		}

		internal void RaiseInput(TuioCursor cursor, ContactState state)
		{
			EventHandler<InputDataEventArgs> eventHandler = Input;
			if (eventHandler != null)
				eventHandler(this, new TuioInputDataEventArgs(cursor, state));
		}
	}
}