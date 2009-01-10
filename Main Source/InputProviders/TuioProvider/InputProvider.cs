using System;
using System.AddIn;
using System.Collections.Generic;
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
		Queue<TuioCursor> contactsQueue;

		internal const string VERSION = "2.0.0.0";

		public event EventHandler<NewFrameEventArgs> NewFrame;

		public InputProvider()
		{
			contactsQueue = new Queue<TuioCursor>();
			SendEmptyFrames = false;
		}

		public bool SendImageType(ImageType imageType, bool isEnable)
		{
			return false;
		}

		public bool SendEmptyFrames { get; set; }

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

		internal void EnqueueContact(TuioCursor cursor)
		{
			lock (contactsQueue)
			{
				contactsQueue.Enqueue(cursor);
			}
		}

		internal void RaiseNewFrame(long timestamp)
		{
			lock (contactsQueue)
			{
				if (SendEmptyFrames || contactsQueue.Count > 0)
				{
					EventHandler<NewFrameEventArgs> eventHandler = NewFrame;
					if (eventHandler != null)
						eventHandler(this, new TuioInputDataEventArgs(contactsQueue, timestamp));
					contactsQueue.Clear();
				}
			}
		}
	}
}