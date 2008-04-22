using System;
using System.ServiceModel;
using Multitouch.Contracts;

namespace Multitouch.Service.ExternalInterfaces
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
	public class ApplicationInterfaceService : IApplicationInterface
	{
		IApplicationInterfaceCallback callback;
		EventHandler<ContactChangedEventArgs> contactChangedHandler;
		static event EventHandler<ContactChangedEventArgs> ContactChangedEvent;

		public void Subscribe()
		{
			callback = OperationContext.Current.GetCallbackChannel<IApplicationInterfaceCallback>();
			contactChangedHandler = OnContactChanged;
			ContactChangedEvent += contactChangedHandler;
		}

		public void Unsubscribe()
		{
			ContactChangedEvent -= contactChangedHandler;
		}

		void OnContactChanged(object sender, ContactChangedEventArgs e)
		{
			IContact contact = e.Contact;
			callback.ContactChanged(contact.Id, contact.X, contact.Y, contact.Width, contact.Height, contact.State);
		}

		public static void ContactChanged(ContactChangedEventArgs e)
		{
			if (ContactChangedEvent != null)
				ContactChangedEvent(null, e);
		}
	}
}
