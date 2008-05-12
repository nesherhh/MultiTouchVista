using System;
using System.AddIn;
using ApplicationSelector.Native;
using Multitouch;
using Multitouch.Contracts;

namespace ApplicationSelector
{
	[AddIn("ApplicationSelector", Publisher="Daniel Danilin", Description="Returns target window from coordinates", Version=VERSION)]
	public class InputPreviewHandler : IInputPreviewHandler
	{
		internal const string VERSION = "1.0.0.0";

		public IPreviewResult Handle(IContact contact)
		{
			IntPtr hWnd = Win32.WindowFromPoint(new Win32.POINT((int)contact.X, (int)contact.Y));
			return new Result(hWnd, contact);
		}
	}
}
