using System;
using System.Windows;
using System.Windows.Input;

namespace Multitouch.Framework.WPF.Input
{
	/// <summary>
	/// Information about contacts.
	/// </summary>
	public class ContactEventArgs : InputEventArgs
	{
		/// <summary>
		/// Gets the contact.
		/// </summary>
		/// <value>The contact.</value>
		public Contact Contact { get { return (Contact)Device; } }

			internal ContactEventArgs(Contact contact, long timestamp)
			: base(contact, (int)timestamp)
		{ }

		/// <summary>
		/// Invokes event handlers in a type-specific way, which can increase event system efficiency.
		/// </summary>
		/// <param name="genericHandler">The generic handler to call in a type-specific way.</param>
		/// <param name="genericTarget">The target to call the handler on.</param>
		protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
		{
			((ContactEventHandler)genericHandler)(genericTarget, this);
		}

		/// <summary>
		/// Gets the position relative to <see cref="IInputElement"/>.
		/// </summary>
		/// <param name="relativeTo">The relative to.</param>
		/// <returns></returns>
		public Point GetPosition(UIElement relativeTo)
		{
			return Contact.GetPosition(relativeTo);
		}
	}
}