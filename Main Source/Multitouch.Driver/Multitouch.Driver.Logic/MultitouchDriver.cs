using System;
using Multitouch.Framework.Input;

namespace Multitouch.Driver.Logic
{
	public class MultitouchDriver
	{
		private ContactHandler contactHandler;
		private DriverCommunicator communicator;

		public void Start()
		{
			communicator = new DriverCommunicator();

			contactHandler = new ContactHandler(IntPtr.Zero);
			contactHandler.Frame += contactHandler_Frame;
		}

		public void Stop()
		{
			contactHandler.Frame -= contactHandler_Frame;
			contactHandler.Dispose();
			contactHandler = null;
		}

		void contactHandler_Frame(object sender, FrameEventArgs e)
		{
			foreach (Contact contact in e.Contacts)
			{
				HidContactInfo contactInfo;
				switch (contact.State)
				{
					case ContactState.New:
						contactInfo = new HidContactInfo(HidContactState.Adding, contact);
						break;
					case ContactState.Moved:
						contactInfo = new HidContactInfo(HidContactState.Updated, contact);
						break;
					case ContactState.Removed:
						contactInfo = new HidContactInfo(HidContactState.Removing, contact);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
				communicator.Enqueue(contactInfo);
			}
		}
	}
}