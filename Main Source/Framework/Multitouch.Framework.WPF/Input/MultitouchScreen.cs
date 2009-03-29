using System;
using System.Collections.Generic;
using System.Windows;
using Phydeaux.Utilities;

namespace Multitouch.Framework.WPF.Input
{
	/// <summary>
	/// Represents a Multitouch Screen.
	/// </summary>
	public static class MultitouchScreen
	{
		/// <summary>
		/// Identifies <see cref="PreviewNewContactEvent"/> attached event.
		/// </summary>
		public static readonly RoutedEvent PreviewNewContactEvent = EventManager.RegisterRoutedEvent("PreviewNewContact",
			RoutingStrategy.Tunnel, typeof(NewContactEventHandler), typeof(MultitouchScreen));
		/// <summary>
		/// Identifies <see cref="PreviewContactRemovedEvent"/> attached event.
		/// </summary>
		public static readonly RoutedEvent PreviewContactRemovedEvent = EventManager.RegisterRoutedEvent("PreviewContactRemoved",
			RoutingStrategy.Tunnel, typeof(ContactEventHandler), typeof(MultitouchScreen));
		/// <summary>
		/// Identifies <see cref="PreviewContactMovedEvent"/> attached event.
		/// </summary>
		public static readonly RoutedEvent PreviewContactMovedEvent = EventManager.RegisterRoutedEvent("PreviewContactMoved",
			RoutingStrategy.Tunnel, typeof(ContactEventHandler), typeof(MultitouchScreen));

		/// <summary>
		/// Identifies <see cref="NewContactEvent"/> attached event.
		/// </summary>
		public static readonly RoutedEvent NewContactEvent = EventManager.RegisterRoutedEvent("NewContact", RoutingStrategy.Bubble,
			typeof(NewContactEventHandler), typeof(MultitouchScreen));
		/// <summary>
		/// Identifies <see cref="ContactRemovedEvent"/> attached event.
		/// </summary>
		public static readonly RoutedEvent ContactRemovedEvent = EventManager.RegisterRoutedEvent("ContactRemoved", RoutingStrategy.Bubble,
			typeof(ContactEventHandler), typeof(MultitouchScreen));
		/// <summary>
		/// Identifies <see cref="ContactMovedEvent"/> attached event.
		/// </summary>
		public static readonly RoutedEvent ContactMovedEvent = EventManager.RegisterRoutedEvent("ContactMoved", RoutingStrategy.Bubble,
			typeof(ContactEventHandler), typeof(MultitouchScreen));

		/// <summary>
		/// Identifies <see cref="ContactEnterEvent"/> attached event.
		/// </summary>
		public static readonly RoutedEvent ContactEnterEvent = EventManager.RegisterRoutedEvent("ContactEnter", RoutingStrategy.Direct,
			typeof(ContactEventHandler), typeof(MultitouchScreen));
		/// <summary>
		/// Identifies <see cref="ContactLeaveEvent"/> attached event.
		/// </summary>
		public static readonly RoutedEvent ContactLeaveEvent = EventManager.RegisterRoutedEvent("ContactLeave", RoutingStrategy.Direct,
			typeof(ContactEventHandler), typeof(MultitouchScreen));

		/// <summary>
		/// Idenitfies <see cref="GotContactCaptureEvent"/> attached event.
		/// </summary>
		public static readonly RoutedEvent GotContactCaptureEvent = EventManager.RegisterRoutedEvent("GotContactCapture", RoutingStrategy.Bubble,
			typeof(ContactEventHandler), typeof(MultitouchScreen));
		/// <summary>
		/// Identifies <see cref="LostContactCaptureEvent"/> attached event.
		/// </summary>
		public static readonly RoutedEvent LostContactCaptureEvent = EventManager.RegisterRoutedEvent("LostContactCapture", RoutingStrategy.Bubble,
			typeof(ContactEventHandler), typeof(MultitouchScreen));

		static readonly StaticProc<UIElement, DependencyObject, RoutedEvent, Delegate> addHandlerMethod;
		static readonly StaticProc<UIElement, DependencyObject, RoutedEvent, Delegate> removeHandlerMethod;

		static MultitouchScreen()
		{
			addHandlerMethod = Dynamic<UIElement>.Static.Procedure.Explicit<DependencyObject, RoutedEvent, Delegate>.CreateDelegate("AddHandler");
			removeHandlerMethod = Dynamic<UIElement>.Static.Procedure.Explicit<DependencyObject, RoutedEvent, Delegate>.CreateDelegate("RemoveHandler");
			//AllowNonContactEvents = true;
		}

		/// <summary>
		/// Adds a handler for the <see cref="NewContactEvent"/> attached event.
		/// </summary>
		/// <param name="element"><see cref="UIElement"/> or <see cref="ContentElement"/> that listens to this event.</param>
		/// <param name="handler">The handler.</param>
		public static void AddNewContactHandler(DependencyObject element, NewContactEventHandler handler)
		{
			addHandlerMethod.Invoke(element, NewContactEvent, handler);
		}

		/// <summary>
		/// Removes a handler for the <see cref="NewContactEvent"/> attached event.
		/// </summary>
		/// <param name="element"><see cref="UIElement"/> or <see cref="ContentElement"/> that listens to this event.</param>
		/// <param name="handler">The handler.</param>
		public static void RemoveNewContactHandler(DependencyObject element, NewContactEventHandler handler)
		{
			removeHandlerMethod.Invoke(element, NewContactEvent, handler);
		}

		/// <summary>
		/// Adds a handler for the <see cref="ContactMovedEvent"/> attached event.
		/// </summary>
		/// <param name="element"><see cref="UIElement"/> or <see cref="ContentElement"/> that listens to this event.</param>
		/// <param name="handler">The handler.</param>
		public static void AddContactMovedHandler(DependencyObject element, ContactEventHandler handler)
		{
			addHandlerMethod.Invoke(element, ContactMovedEvent, handler);
		}

		/// <summary>
		/// Removes a handler for the <see cref="ContactMovedEvent"/> attached event.
		/// </summary>
		/// <param name="element"><see cref="UIElement"/> or <see cref="ContentElement"/> that listens to this event.</param>
		/// <param name="handler">The handler.</param>
		public static void RemoveContactMovedHandler(DependencyObject element, ContactEventHandler handler)
		{
			removeHandlerMethod.Invoke(element, ContactMovedEvent, handler);
		}

		/// <summary>
		/// Adds a handler for the <see cref="ContactRemovedEvent"/> attached event.
		/// </summary>
		/// <param name="element"><see cref="UIElement"/> or <see cref="ContentElement"/> that listens to this event.</param>
		/// <param name="handler">The handler.</param>
		public static void AddContactRemovedHandler(DependencyObject element, ContactEventHandler handler)
		{
			addHandlerMethod.Invoke(element, ContactRemovedEvent, handler);
		}

		/// <summary>
		/// Removes a handler for the <see cref="ContactRemovedEvent"/> attached event.
		/// </summary>
		/// <param name="element"><see cref="UIElement"/> or <see cref="ContentElement"/> that listens to this event.</param>
		/// <param name="handler">The handler.</param>
		public static void RemoveContactRemovedHandler(DependencyObject element, ContactEventHandler handler)
		{
			removeHandlerMethod.Invoke(element, ContactRemovedEvent, handler);
		}

		/// <summary>
		/// Adds a handler for the <see cref="ContactEnterEvent"/> attached event.
		/// </summary>
		/// <param name="element"><see cref="UIElement"/> or <see cref="ContentElement"/> that listens to this event.</param>
		/// <param name="handler">The handler.</param>
		public static void AddContactEnterHandler(DependencyObject element, ContactEventHandler handler)
		{
			addHandlerMethod.Invoke(element, ContactEnterEvent, handler);
		}

		/// <summary>
		/// Removes a handler for the <see cref="ContactEnterEvent"/> attached event.
		/// </summary>
		/// <param name="element"><see cref="UIElement"/> or <see cref="ContentElement"/> that listens to this event.</param>
		/// <param name="handler">The handler.</param>
		public static void RemoveContactEnterHandler(DependencyObject element, ContactEventHandler handler)
		{
			removeHandlerMethod.Invoke(element, ContactEnterEvent, handler);
		}

		/// <summary>
		/// Adds a handler for the <see cref="ContactLeaveEvent"/> attached event.
		/// </summary>
		/// <param name="element"><see cref="UIElement"/> or <see cref="ContentElement"/> that listens to this event.</param>
		/// <param name="handler">The handler.</param>
		public static void AddContactLeaveHandler(DependencyObject element, ContactEventHandler handler)
		{
			addHandlerMethod.Invoke(element, ContactLeaveEvent, handler);
		}

		/// <summary>
		/// Removes a handler for the <see cref="ContactLeaveEvent"/> attached event.
		/// </summary>
		/// <param name="element"><see cref="UIElement"/> or <see cref="ContentElement"/> that listens to this event.</param>
		/// <param name="handler">The handler.</param>
		public static void RemoveContactLeaveHandler(DependencyObject element, ContactEventHandler handler)
		{
			removeHandlerMethod.Invoke(element, ContactLeaveEvent, handler);
		}

		/// <summary>
		/// Adds a handler for the <see cref="GotContactCaptureEvent"/> attached event.
		/// </summary>
		/// <param name="element"><see cref="UIElement"/> or <see cref="ContentElement"/> that listens to this event.</param>
		/// <param name="handler">The handler.</param>
		public static void AddGotContactCaptureHandler(DependencyObject element, ContactEventHandler handler)
		{
			addHandlerMethod.Invoke(element, GotContactCaptureEvent, handler);
		}

		/// <summary>
		/// Removes a handler for the <see cref="GotContactCaptureEvent"/> attached event.
		/// </summary>
		/// <param name="element"><see cref="UIElement"/> or <see cref="ContentElement"/> that listens to this event.</param>
		/// <param name="handler">The handler.</param>
		public static void RemoveGotContactCaptureHandler(DependencyObject element, ContactEventHandler handler)
		{
			removeHandlerMethod.Invoke(element, GotContactCaptureEvent, handler);
		}

		/// <summary>
		/// Adds a handler for the <see cref="LostContactCaptureEvent"/> attached event.
		/// </summary>
		/// <param name="element"><see cref="UIElement"/> or <see cref="ContentElement"/> that listens to this event.</param>
		/// <param name="handler">The handler.</param>
		public static void AddLostContactCaptureHandler(DependencyObject element, ContactEventHandler handler)
		{
			addHandlerMethod.Invoke(element, LostContactCaptureEvent, handler);
		}

		/// <summary>
		/// Removes a handler for the <see cref="LostContactCaptureEvent"/> attached event.
		/// </summary>
		/// <param name="element"><see cref="UIElement"/> or <see cref="ContentElement"/> that listens to this event.</param>
		/// <param name="handler">The handler.</param>
		public static void RemoveLostContactCaptureHandler(DependencyObject element, ContactEventHandler handler)
		{
			removeHandlerMethod.Invoke(element, LostContactCaptureEvent, handler);
		}

		/// <summary>
		/// Adds a handler for the <see cref="PreviewNewContactEvent"/> attached event.
		/// </summary>
		/// <param name="element"><see cref="UIElement"/> or <see cref="ContentElement"/> that listens to this event.</param>
		/// <param name="handler">The handler.</param>
		public static void AddPreviewNewContactHandler(DependencyObject element, NewContactEventHandler handler)
		{
			addHandlerMethod.Invoke(element, PreviewNewContactEvent, handler);
		}

		/// <summary>
		/// Removes a handler for the <see cref="PreviewNewContactEvent"/> attached event.
		/// </summary>
		/// <param name="element"><see cref="UIElement"/> or <see cref="ContentElement"/> that listens to this event.</param>
		/// <param name="handler">The handler.</param>
		public static void RemovePreviewNewContactHandler(DependencyObject element, NewContactEventHandler handler)
		{
			removeHandlerMethod.Invoke(element, PreviewNewContactEvent, handler);
		}

		/// <summary>
		/// Adds a handler for the <see cref="PreviewContactRemovedEvent"/> attached event.
		/// </summary>
		/// <param name="element"><see cref="UIElement"/> or <see cref="ContentElement"/> that listens to this event.</param>
		/// <param name="handler">The handler.</param>
		public static void AddPreviewContactRemovedHandler(DependencyObject element, ContactEventHandler handler)
		{
			addHandlerMethod.Invoke(element, PreviewContactRemovedEvent, handler);
		}

		/// <summary>
		/// Removes a handler for the <see cref="PreviewContactRemovedEvent"/> attached event.
		/// </summary>
		/// <param name="element"><see cref="UIElement"/> or <see cref="ContentElement"/> that listens to this event.</param>
		/// <param name="handler">The handler.</param>
		public static void RemovePreviewContactRemovedHandler(DependencyObject element, ContactEventHandler handler)
		{
			removeHandlerMethod.Invoke(element, PreviewContactRemovedEvent, handler);
		}

		/// <summary>
		/// Adds a handler for the <see cref="PreviewContactMovedEvent"/> attached event.
		/// </summary>
		/// <param name="element"><see cref="UIElement"/> or <see cref="ContentElement"/> that listens to this event.</param>
		/// <param name="handler">The handler.</param>
		public static void AddPreviewContactMovedHandler(DependencyObject element, ContactEventHandler handler)
		{
			addHandlerMethod.Invoke(element, PreviewContactMovedEvent, handler);
		}

		/// <summary>
		/// Removes a handler for the <see cref="PreviewContactMovedEvent"/> attached event.
		/// </summary>
		/// <param name="element"><see cref="UIElement"/> or <see cref="ContentElement"/> that listens to this event.</param>
		/// <param name="handler">The handler.</param>
		public static void RemovePreviewContactMovedHandler(DependencyObject element, ContactEventHandler handler)
		{
			removeHandlerMethod.Invoke(element, PreviewContactMovedEvent, handler);
		}

		/// <summary>
		/// Gets or sets a value indicating whether mouse events are allowed.
		/// </summary>
		/// <value><c>false</c> - all mouse events will be canceled in MultitouchLogic at PreProcessInput stage.</value>
		public static bool AllowNonContactEvents { get; set; }

		/// <summary>
		/// Gets the contacts captured in specified element
		/// </summary>
		/// <param name="element">The element.</param>
		public static IEnumerable<Contact> GetContactsCaptured(IInputElement element)
		{
			return MultitouchLogic.Current.ContactsManager.GetContactsCaptured(element);
		}
	}

	/// <summary>
	/// Represents the method that will handle contact event.
	/// </summary>
	public delegate void ContactEventHandler(object sender, ContactEventArgs e);
	/// <summary>
	/// Represents the method that will handle a new contact event.
	/// </summary>
	public delegate void NewContactEventHandler(object sender, NewContactEventArgs e);
	///// <summary>
	///// Represents the method that will handle gesture event.
	///// </summary>
	//public delegate void GestureEventHandler(object sender, GestureEventArgs e);

	delegate void RawMultitouchReportHandler(object sender, RawMultitouchReport e);
}