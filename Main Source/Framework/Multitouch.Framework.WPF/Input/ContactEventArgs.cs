using System;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using Multitouch.Framework.Collections;

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
		public Contact Contact { get; private set; }
		internal MultitouchDevice MultitouchDevice { get; private set; }

		internal ContactEventArgs(MultitouchDevice device, RawMultitouchReport raw, int timestamp)
			: this(device, raw.Contact, timestamp)
		{ }

		internal ContactEventArgs(MultitouchDevice device, Contact contact, int timestamp)
			: base(device, timestamp)
		{
			Contact = contact;
			MultitouchDevice = device;
		}

		/// <summary>
		/// Invokes event handlers in a type-specific way, which can increase event system efficiency.
		/// </summary>
		/// <param name="genericHandler">The generic handler to call in a type-specific way.</param>
		/// <param name="genericTarget">The target to call the handler on.</param>
		protected override void InvokeEventHandler(Delegate genericHandler, object genericTarget)
		{
			ContactEventHandler handler = (ContactEventHandler)genericHandler;
			handler(genericTarget, this);
		}

		/// <summary>
		/// Gets the position relative to <see cref="IInputElement"/>.
		/// </summary>
		/// <param name="relativeTo">The relative to.</param>
		/// <returns></returns>
		public Point GetPosition(IInputElement relativeTo)
		{
			return MultitouchDevice.GetPosition(relativeTo);
		}

		/// <summary>
		/// Gets the captured <see cref="IInputElement"/>.
		/// </summary>
		/// <value>The captured.</value>
		public IInputElement Captured
		{
			get { return MultitouchDevice.Captured; }
		}

		/// <summary>
		/// Gets all contacts.
		/// </summary>
		/// <value>All contacts.</value>
		public IDictionary<int, Contact> AllContacts
		{
			get { return new ReadOnlyDictionary<int, Contact>(MultitouchDevice.AllContacts); }
		}

		/// <summary>
		/// Gets the contacts for specified element.
		/// </summary>
		/// <param name="forElement">For element.</param>
		/// <param name="criteria">The criteria.</param>
		/// <returns></returns>
		public IDictionary<int, Contact> GetContacts(UIElement forElement, MatchCriteria criteria)
		{
			return new ReadOnlyDictionary<int, Contact>(MultitouchDevice.GetContacts(forElement, criteria));
		}

		/// <summary>
		/// Returns all contacts that are over specified element
		/// </summary>
		/// <param name="forElement"></param>
		/// <returns></returns>
		public IDictionary<int, Contact> GetContacts(UIElement forElement)
		{
			return GetContacts(forElement, MatchCriteria.LogicalParent);  // CHANGED: Roberto Sonnino 09/11/2008
		}

		/// <summary>
		/// When overridden in a derived class, provides a notification callback entry point whenever the value of the <see cref="P:System.Windows.RoutedEventArgs.Source"/> property of an instance changes.
		/// </summary>
		/// <param name="source">The new value that <see cref="P:System.Windows.RoutedEventArgs.Source"/> is being set to.</param>
		protected override void OnSetSource(object source)
		{
			Contact.SetElement((UIElement)source);
		}
	}
}