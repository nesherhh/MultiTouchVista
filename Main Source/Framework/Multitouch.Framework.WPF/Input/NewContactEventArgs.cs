using System;

namespace Multitouch.Framework.WPF.Input
{
	/// <summary>
	/// Provides information about a new contact.
	/// </summary>
	public class NewContactEventArgs : ContactEventArgs
	{
		internal NewContactEventArgs(MultitouchDevice device, RawMultitouchReport raw, int timestamp)
			: base(device, raw, timestamp)
		{ }

		internal NewContactEventArgs(MultitouchDevice device, Contact contact, int timestamp)
			: base(device, contact, timestamp)
		{ }

		/// <summary>
		/// Gets the tap count.
		/// </summary>
		/// <value>The tap count.</value>
		public int TapCount
		{
			get { return MultitouchDevice.TapCount; }
		}

		/// <summary>
		/// Invokes the event handler.
		/// </summary>
		/// <param name="genericHandler">The generic handler.</param>
		/// <param name="genericTarget">The generic target.</param>
		protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
		{
			NewContactEventHandler handler = (NewContactEventHandler)genericHandler;
			handler(genericTarget, this);
		}
	}
}