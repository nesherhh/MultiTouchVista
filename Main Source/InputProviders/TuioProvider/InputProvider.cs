using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Forms;
using Multitouch.Contracts;
using TUIO;
using Size=System.Drawing.Size;

namespace TuioProvider
{
	[AddIn("Tuio", Publisher = "Daniel Danilin", Description = "Provides input from TUIO server.", Version = VERSION)]
	[Export(typeof(IProvider))]
	public class InputProvider : IProvider
	{
		internal const string VERSION = "3.0.0.0";

		Size monitorSize;

		public class CursorState
		{
			public CursorState(TuioCursor cursor, ContactState state)
			{
				Cursor = cursor;
				State = state;
			}

			public TuioCursor Cursor { get; private set; }
			public ContactState State { get; private set; }
		}

		TuioClient client;
		Listener listener;
		readonly Queue<Contact> contactsQueue;

		public event EventHandler<NewFrameEventArgs> NewFrame;

		public InputProvider()
		{
			contactsQueue = new Queue<Contact>();
			monitorSize = SystemInformation.PrimaryMonitorSize;
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

		internal void EnqueueContact(TuioCursor cursor, ContactState state)
		{
			lock (contactsQueue)
			{
				contactsQueue.Enqueue(new TuioContact(cursor, state, monitorSize));
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
						eventHandler(this, new NewFrameEventArgs(timestamp, contactsQueue, null));
					contactsQueue.Clear();
				}
			}
		}
	}
}