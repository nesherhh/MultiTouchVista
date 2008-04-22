using System;
using System.AddIn;
using Multitouch.Contracts;
using TouchlibWrapper;

namespace TouchLib
{
	[AddIn("TouchLib", Publisher = "Daniel Danilin", Description = "Provides input from touchLib (http://nuigroup.com/touchlib)", Version = VERSION)]
	public class InputProvider : IProvider
	{
		CsTI instance;
		internal const string VERSION = "1.0.0.0";

		public event EventHandler<ContactChangedEventArgs> ContactChanged;

		public void Start()
		{
			instance = CsTI.Instance();
			instance.fingerDown += new fingerDownHandler(instance_fingerDown);
			instance.fingerUp += new fingerUpHandler(instance_fingerUp);
			instance.fingerUpdate += new fingerUpdateHandler(instance_fingerUpdate);
			instance.startScreen();
			IsRunning = true;
		}

		public void Stop()
		{
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
			OnContactChanged(new TouchLibContactChangedEventArgs(new TouchLibContact(id, x, y, width, height, ContactState.Moved)));
		}

		void instance_fingerUp(int id, int tagID, float x, float y, float angle, float area, float height, float width, float dX, float dY)
		{
			OnContactChanged(new TouchLibContactChangedEventArgs(new TouchLibContact(id, x, y, width, height, ContactState.New)));
		}

		void instance_fingerDown(int id, int tagID, float x, float y, float angle, float area, float height, float width, float dX, float dY)
		{
			OnContactChanged(new TouchLibContactChangedEventArgs(new TouchLibContact(id, x, y, width, height, ContactState.Removed)));
		}
	}
}