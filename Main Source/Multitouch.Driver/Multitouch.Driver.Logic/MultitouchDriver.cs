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
				switch (contact.State)
				{
					case ContactState.New:
						communicator.AddContact(new HidContactInfo(true, true, contact));
						break;
					case ContactState.Moved:
						communicator.UpdateContact(new HidContactInfo(true, true, contact));
						break;
					case ContactState.Removed:
						communicator.RemoveContact(new HidContactInfo(false, false, contact));
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}
	}
}