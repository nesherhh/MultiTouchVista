using System;
using Multitouch;
using Multitouch.Contracts;

namespace ApplicationSelector
{
	class Result : IPreviewResult
	{
		IntPtr hWnd;
		IContact contact;

		public Result(IntPtr hWnd, IContact contact)
		{
			this.hWnd = hWnd;
			this.contact = contact;
		}

		public IContact Contact
		{
			get { return contact; }
		}

		public IntPtr HWnd
		{
			get { return hWnd; }
		}

		public bool Handled
		{
			get { return false; }
		}
	}
}
