using System;
using System.AddIn;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Multitouch.Contracts;
using TouchlibWrapper;

namespace TouchLib
{
	[AddIn("TouchLib", Publisher = "Daniel Danilin", Description = "Provides input from touchLib (http://nuigroup.com/touchlib)", Version = VERSION)]
	public class InputProvider : IProvider
	{
		CsTI instance;
		int screenWidth;
		int screenHeight;
		internal const string VERSION = "1.0.0.0";

		public event EventHandler<ContactChangedEventArgs> ContactChanged;

		public void Start()
		{
			screenWidth = Screen.PrimaryScreen.Bounds.Width;
			screenHeight = Screen.PrimaryScreen.Bounds.Height;

			instance = CsTI.Instance();
			instance.fingerDown += new fingerDownHandler(instance_fingerDown);
			instance.fingerUp += new fingerUpHandler(instance_fingerUp);
			instance.fingerUpdate += new fingerUpdateHandler(instance_fingerUpdate);


			Thread t = new Thread(ThreadWorker);
			t.SetApartmentState(ApartmentState.MTA);
			t.IsBackground = true;
			t.Name = "touchlib thread";
			t.Start();
		}

		void ThreadWorker()
		{
			if (instance != null)
			{
				IsRunning = true;
				instance.startScreen(true);
			}
		}

		public void Stop()
		{
			if(instance != null)
				instance.stopScreen();
			instance = null;
			IsRunning = false;
		}

		public bool IsRunning { get; private set; }

		private void OnContactChanged(ContactChangedEventArgs e)
		{
			if (ContactChanged != null)
				ContactChanged(this, e);
		}

		void instance_fingerUpdate(int id, int tagID, float x, float y, float angle, float area, float height, float width, float dX, float dY)
		{
			x = x * screenWidth;
			y = y * screenHeight;
			OnContactChanged(new TouchLibContactChangedEventArgs(new TouchLibContact(id, x, y, width, height, ContactState.Moved)));
		}

		void instance_fingerUp(int id, int tagID, float x, float y, float angle, float area, float height, float width, float dX, float dY)
		{
			x = x * screenWidth;
			y = y * screenHeight;
			OnContactChanged(new TouchLibContactChangedEventArgs(new TouchLibContact(id, x, y, width, height, ContactState.Removed)));
		}

		void instance_fingerDown(int id, int tagID, float x, float y, float angle, float area, float height, float width, float dX, float dY)
		{
			x = x * screenWidth;
			y = y * screenHeight;
			OnContactChanged(new TouchLibContactChangedEventArgs(new TouchLibContact(id, x, y, width, height, ContactState.New)));
		}
	}
}