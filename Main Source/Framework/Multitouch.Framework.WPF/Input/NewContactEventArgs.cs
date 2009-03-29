using System;

namespace Multitouch.Framework.WPF.Input
{
	/// <summary>
	/// Provides information about a new contact.
	/// </summary>
	public class NewContactEventArgs : ContactEventArgs
	{
		internal NewContactEventArgs(Contact contact, int timestamp)
			: base(contact, timestamp)
		{ }

		/// <summary>
		/// Invokes the event handler.
		/// </summary>
		/// <param name="genericHandler">The generic handler.</param>
		/// <param name="genericTarget">The generic target.</param>
		protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
		{
			((NewContactEventHandler)genericHandler)(genericTarget, this);
		}
	}
}