using System;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using Multitouch.Contracts;

namespace Multitouch.Service.ExternalInterfaces
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
	public class ApplicationInterfaceService : IApplicationInterface
	{
		IApplicationInterfaceCallback callback;
		IntPtr hWnd;
		EventHandler<ContactChangedEventArgs> contactChangedHandler;
		static Dictionary<IntPtr, EventHandler<ContactChangedEventArgs>> receivers;

		static ApplicationInterfaceService()
		{
			receivers = new Dictionary<IntPtr, EventHandler<ContactChangedEventArgs>>();
		}

		public void Subscribe(IntPtr windowHandle)
		{
			hWnd = windowHandle;
			callback = OperationContext.Current.GetCallbackChannel<IApplicationInterfaceCallback>();
			contactChangedHandler = OnContactChanged;
			receivers.Add(hWnd, contactChangedHandler);
		}

		public void Unsubscribe()
		{
			receivers.Remove(hWnd);
		}

		void OnContactChanged(object sender, ContactChangedEventArgs e)
		{
			IContact contact = e.Contact;
			callback.ContactChanged(contact.Id, contact.X, contact.Y, contact.Width, contact.Height, contact.State);
		}

		/// <summary>
		/// Sends contact information to specified window.
		/// </summary>
		/// <param name="hWnd">Window handle</param>
		/// <param name="e">Contact information</param>
		public static void ContactChanged(IntPtr hWnd, ContactChangedEventArgs e)
		{
			EventHandler<ContactChangedEventArgs> handler;
			if (receivers.TryGetValue(hWnd, out handler))
			{
				try
				{
					handler(null, e);
				}
				catch (Exception)
				{
					receivers.Remove(hWnd);
				}
			}
		}

		/// <summary>
		/// Sends contact information to all subscribers
		/// </summary>
		/// <param name="e">Contact information</param>
		public static void ContactChanged(ContactChangedEventArgs e)
		{
			List<KeyValuePair<IntPtr, EventHandler<ContactChangedEventArgs>>> handlers = receivers.ToList();
			foreach (KeyValuePair<IntPtr, EventHandler<ContactChangedEventArgs>> keyValuePair in handlers)
			{
				try
				{
					keyValuePair.Value(null, e);
				}
				catch(Exception)
				{
					receivers.Remove(keyValuePair.Key);
				}
			}
		}
	}
}