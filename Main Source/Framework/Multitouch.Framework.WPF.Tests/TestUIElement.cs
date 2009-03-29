using System;
using System.Windows.Controls;
using Multitouch.Framework.WPF.Input;

namespace Multitouch.Framework.WPF.Tests
{
	class TestUIElement : Button
	{
		Action<Contact> executeOnNextContact;

		public event NewContactEventHandler NewContact;
		public event ContactEventHandler ContactRemoved;
		public event ContactEventHandler ContactMoved;

		public event NewContactEventHandler PreviewNewContact;
		public event ContactEventHandler PreviewContactRemoved;
		public event ContactEventHandler PreviewContactMoved;

		public event ContactEventHandler ContactEnter;
		public event ContactEventHandler ContactLeave;

		public event ContactEventHandler GotContactCapture;
		public event ContactEventHandler LostContactCapture;

		public TestUIElement()
		{
			AddHandler(MultitouchScreen.NewContactEvent, (NewContactEventHandler)OnNewContact);
			AddHandler(MultitouchScreen.ContactRemovedEvent, (ContactEventHandler)OnContactRemoved);
			AddHandler(MultitouchScreen.ContactMovedEvent, (ContactEventHandler)OnContactMoved);

			AddHandler(MultitouchScreen.PreviewNewContactEvent, (NewContactEventHandler)OnPreviewNewContact);
			AddHandler(MultitouchScreen.PreviewContactRemovedEvent, (ContactEventHandler)OnPreviewContactRemoved);
			AddHandler(MultitouchScreen.PreviewContactMovedEvent, (ContactEventHandler)OnPreviewContactMoved);

			AddHandler(MultitouchScreen.ContactEnterEvent, (ContactEventHandler)OnContactEnter);
			AddHandler(MultitouchScreen.ContactLeaveEvent, (ContactEventHandler)OnContactLeave);

			AddHandler(MultitouchScreen.GotContactCaptureEvent, (ContactEventHandler)OnGotContactCapture);
			AddHandler(MultitouchScreen.LostContactCaptureEvent, (ContactEventHandler)OnLostContactCapture);
		}

		void InvokeHandler(ContactEventHandler handler, ContactEventArgs e)
		{
			if (handler != null)
			{
				handler(this, e);

				if(executeOnNextContact != null)
				{
					Action<Contact> action = executeOnNextContact;
					executeOnNextContact = null;
					action(e.Contact);
				}
			}
		}

		void InvokeHandler(NewContactEventHandler handler, NewContactEventArgs e)
		{
			if (handler != null)
			{
				handler(this, e);

				if (executeOnNextContact != null)
				{
					Action<Contact> action = executeOnNextContact;
					executeOnNextContact = null;
					action(e.Contact);
				}
			}
		}

		void OnLostContactCapture(object sender, ContactEventArgs e)
		{
			InvokeHandler(LostContactCapture, e);
		}

		void OnGotContactCapture(object sender, ContactEventArgs e)
		{
			InvokeHandler(GotContactCapture, e);
		}

		void OnContactLeave(object sender, ContactEventArgs e)
		{
			InvokeHandler(ContactLeave, e);
		}

		void OnContactEnter(object sender, ContactEventArgs e)
		{
			InvokeHandler(ContactEnter, e);
		}

		void OnContactMoved(object sender, ContactEventArgs e)
		{
			InvokeHandler(ContactMoved, e);
		}

		void OnContactRemoved(object sender, ContactEventArgs e)
		{
			InvokeHandler(ContactRemoved, e);
		}

		void OnNewContact(object sender, NewContactEventArgs e)
		{
			InvokeHandler(NewContact, e);
		}

		void OnPreviewContactMoved(object sender, ContactEventArgs e)
		{
			InvokeHandler(PreviewContactMoved, e);
		}

		void OnPreviewContactRemoved(object sender, ContactEventArgs e)
		{
			InvokeHandler(PreviewContactRemoved, e);
		}

		void OnPreviewNewContact(object sender, NewContactEventArgs e)
		{
			InvokeHandler(PreviewNewContact, e);
		}

		public void ExecuteOnNextContact(Action<Contact> action)
		{
			executeOnNextContact = action;
		}
	}
}