using System;
using System.AddIn;
using Multitouch.Contracts;
using PostSingleInput.Native;

namespace PostSingleInput
{
	[AddIn("ApplicationSelector", Publisher = "Daniel Danilin", Description = "Returns target window from coordinates", Version = VERSION)]
	public class InputPostHandler : IInputPostHandler
	{
		public const string VERSION = "1.0.0.0";

		public void Handle(IContact contact)
		{
			if (contact.State == ContactState.New)
				SendDown(contact);
			else if (contact.State == ContactState.Moved)
				SendMove(contact);
			else if (contact.State == ContactState.Removed)
				SendUp(contact);
		}

		void SendUp(IContact contact)
		{
			SendMouse(contact, NativeMethods.SendMouseUp);
		}

		void SendMove(IContact contact)
		{
			SendMouse(contact, NativeMethods.SendMouseMove);
		}

		void SendDown(IContact contact)
		{
			SendMouse(contact, NativeMethods.SendMouseDown);
		}

		void SendMouse(IContact contact, Action<IntPtr, POINT> action)
		{
			POINT point = new POINT((int)contact.X, (int)contact.Y);
			IntPtr hWnd = GetWindowAtPoint(point);

			NativeMethods.MapWindowPoints(IntPtr.Zero, hWnd, ref point, 1);

			if(hWnd != IntPtr.Zero)
				action(hWnd, point);
		}

		IntPtr GetWindowAtPoint(POINT point)
		{
			return NativeMethods.WindowFromPoint(point);
		}
	}
}
