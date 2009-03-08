using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Timers;
using System.Windows;
using Multitouch.Contracts;

namespace MultipleMice
{
	[AddIn("MultipleMice", Publisher = "Daniel Danilin", Description = "Provides input from multiple mice", Version = VERSION)]
	[Export(typeof(IProvider))]
	public class InputProvider : IProvider
	{
		internal const string VERSION = "3.0.0.0";

		public event EventHandler<NewFrameEventArgs> NewFrame;

		Timer timer;
		RawDevicesManager deviceManager;
		Queue<Contact> contactsQueue;

		public InputProvider()
		{
			timer = new Timer(1000 / 60d);
			timer.Elapsed += timer_Elapsed;

			contactsQueue = new Queue<Contact>();

			SendEmptyFrames = false;
		}

		public bool SendImageType(ImageType imageType, bool isEnable)
		{
			return false;
		}

		public void Start()
		{
			deviceManager = new RawDevicesManager(this);
			timer.Start();
		}

		public void Stop()
		{
			if (deviceManager != null)
			{
				deviceManager.Dispose();
				deviceManager = null;
			}
			timer.Stop();
		}

		public bool IsRunning { get; internal set; }

		void timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			lock (contactsQueue)
			{
				if (SendEmptyFrames || contactsQueue.Count > 0)
				{
					EventHandler<NewFrameEventArgs> eventHandler = NewFrame;
					if (eventHandler != null)
						eventHandler(this, new NewFrameEventArgs(Stopwatch.GetTimestamp(), contactsQueue, null));
					contactsQueue.Clear();
				}
			}
		}

		internal void EnqueueContact(MouseContact contact)
		{
			lock (contactsQueue)
			{
				contactsQueue.Enqueue((MouseContact)contact.Clone());
			}
		}

		public UIElement GetConfiguration()
		{
			return new ConfigurationWindow();
		}

		public bool HasConfiguration
		{
			get { return true; }
		}

		public bool SendEmptyFrames { get; set; }
	}
}