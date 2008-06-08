using System;
using System.AddIn;
using System.Drawing;
using System.Windows.Forms;
using ManagedWinapi.Windows;
using Multitouch;
using Multitouch.Contracts;

namespace DWMaxxAddIn
{
	[AddIn("DWMaxxAddIn", Publisher = "Daniel Danilin", Description = "Returns target window from coordinates", Version = VERSION)]
	public class InputPreviewHandler : IInputPreviewHandler
	{
		public const string VERSION = "1.0.0.0";

		Desktop desktop;

		public InputPreviewHandler()
		{
			desktop = new Desktop();
		}

		public void Stop()
		{
			if(desktop != null)
			{
				desktop.Dispose();
				desktop = null;
			}
		}

		public void Start()
		{}

		public IPreviewResult Handle(IContact contact)
		{
			Point transformedCoordinates;
			IntPtr hWnd = desktop.GetWindowAt(new Point((int)contact.X, (int)contact.Y), out transformedCoordinates);

			if (new SystemWindow(hWnd).WindowState == FormWindowState.Normal)
			{
				switch (contact.State)
				{
					case ContactState.New:
						OnNewContact(hWnd, contact);
						break;
					case ContactState.Removed:
						OnContactRemoved(hWnd, contact);
						break;
					case ContactState.Moved:
						OnContactMoved(hWnd, contact);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			TransformedContact newContact = new TransformedContact(contact);
			newContact.X = transformedCoordinates.X;
			newContact.Y = transformedCoordinates.Y;
			return new Result(hWnd, newContact);
		}

		void OnNewContact(IntPtr handle, IContact contact)
		{
			desktop.OnNewContact(handle, contact);
		}

		void OnContactRemoved(IntPtr handle, IContact contact)
		{
			desktop.OnContactRemoved(handle, contact);
		}

		void OnContactMoved(IntPtr handle, IContact contact)
		{
			desktop.OnContactMoved(handle, contact);
		}

		public int Order
		{
			get { return 10; }
		}
	}
}
