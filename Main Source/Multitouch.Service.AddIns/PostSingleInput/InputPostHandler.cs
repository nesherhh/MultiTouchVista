using System;
using System.AddIn;
using System.Drawing;
using Multitouch.Contracts;
using PostSingleInput.Native;

namespace PostSingleInput
{
	[AddIn("PostSingleInput", Publisher = "Daniel Danilin", Description = "Returns target window from coordinates", Version = VERSION)]
	public class InputPostHandler : IInputPostHandler
	{
		public const string VERSION = "1.0.0.0";

		public void Handle(IntPtr hWnd, IContact contact)
		{
			if (contact.State == ContactState.New)
				SendDown(hWnd, contact);
			else if (contact.State == ContactState.Moved)
				SendMove(hWnd, contact);
			else if (contact.State == ContactState.Removed)
				SendUp(hWnd, contact);
		}

		public void Start()
		{}

		public void Stop()
		{}

		void SendUp(IntPtr hWnd, IContact contact)
		{
			//SendMouse(hWnd, contact, NativeMethods.SendMouseUp);
			MouseEmulationHelper.LeftUp(new Point((int)contact.X, (int)contact.Y));
		}

		void SendMove(IntPtr hWnd, IContact contact)
		{
			//SendMouse(hWnd, contact, NativeMethods.SendMouseMove);
			MouseEmulationHelper.Drag(new Point((int)contact.X, (int)contact.Y));
		}

		void SendDown(IntPtr hWnd, IContact contact)
		{
			//SendMouse(hWnd, contact, NativeMethods.SendMouseDown);
			MouseEmulationHelper.LeftDown(new Point((int)contact.X, (int)contact.Y));
		}

		void SendMouse(IntPtr hWnd, IContact contact, Action<IntPtr, POINT> action)
		{
			POINT point = new POINT((int)contact.X, (int)contact.Y);
			if(hWnd.Equals(IntPtr.Zero))
				hWnd = GetWindowAtPoint(point);

			NativeMethods.MapWindowPoints(IntPtr.Zero, hWnd, ref point, 1);

			if(hWnd != IntPtr.Zero)
				action(hWnd, point);
		}

		IntPtr GetWindowAtPoint(POINT point)
		{
			return NativeMethods.WindowFromPoint(point);
		}

		public int Order
		{
			get { return 20; }
		}
	}
}
