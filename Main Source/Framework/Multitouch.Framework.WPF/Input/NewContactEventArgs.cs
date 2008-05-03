using System;

namespace Multitouch.Framework.WPF.Input
{
	public class NewContactEventArgs : ContactEventArgs
	{
		internal NewContactEventArgs(MultitouchDevice device, RawMultitouchReport raw, int timestamp)
			: base(device, raw, timestamp)
		{ }

		internal NewContactEventArgs(MultitouchDevice device, Contact contact, int timestamp)
			: base(device, contact, timestamp)
		{ }

		public int TapCount
		{
			get { return MultitouchDevice.TapCount; }
		}

		protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
		{
			NewContactEventHandler handler = (NewContactEventHandler)genericHandler;
			handler(genericTarget, this);
		}
	}
}