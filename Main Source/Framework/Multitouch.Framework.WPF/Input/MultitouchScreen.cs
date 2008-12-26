using System;
using System.Linq;
using System.Windows;
using System.Windows.Ink;
using Phydeaux.Utilities;

namespace Multitouch.Framework.WPF.Input
{
	/// <summary>
	/// Represents the method that will handle contact event.
	/// </summary>
	public delegate void ContactEventHandler(object sender, ContactEventArgs e);
	/// <summary>
	/// Represents the method that will handle a new contact event.
	/// </summary>
	public delegate void NewContactEventHandler(object sender, NewContactEventArgs e);
	/// <summary>
	/// Represents the method that will handle gesture event.
	/// </summary>
	public delegate void GestureEventHandler(object sender, GestureEventArgs e);

	delegate void RawMultitouchReportHandler(object sender, RawMultitouchReport e);

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
		public static readonly RoutedEvent ContactEnterEvent = EventManager.RegisterRoutedEvent("ContactEnter", RoutingStrategy.Bubble,
			typeof(ContactEventHandler), typeof(MultitouchScreen));
		/// <summary>
		/// Identifies <see cref="ContactLeaveEvent"/> attached event.
		/// </summary>
		public static readonly RoutedEvent ContactLeaveEvent = EventManager.RegisterRoutedEvent("ContactLeave", RoutingStrategy.Bubble,
			typeof(ContactEventHandler), typeof(MultitouchScreen));

		/// <summary>
		/// Identifies <see cref="PreviewGestureEvent"/> attached event.
		/// </summary>
		public static readonly RoutedEvent PreviewGestureEvent = EventManager.RegisterRoutedEvent("PreviewGesture", RoutingStrategy.Tunnel, typeof(GestureEventHandler),
			typeof(MultitouchScreen));
		/// <summary>
		/// Identifies <see cref="GestureEvent"/> attached event.
		/// </summary>
		public static readonly RoutedEvent GestureEvent = EventManager.RegisterRoutedEvent("Gesture", RoutingStrategy.Bubble, typeof(GestureEventHandler),
			typeof(MultitouchScreen));

		static StaticProc<UIElement, DependencyObject, RoutedEvent, Delegate> addHandlerMethod;
		static StaticProc<UIElement, DependencyObject, RoutedEvent, Delegate> removeHandlerMethod;

		static MultitouchScreen()
		{
			addHandlerMethod = Dynamic<UIElement>.Static.Procedure.Explicit<DependencyObject, RoutedEvent, Delegate>.CreateDelegate("AddHandler");
			removeHandlerMethod = Dynamic<UIElement>.Static.Procedure.Explicit<DependencyObject, RoutedEvent, Delegate>.CreateDelegate("RemoveHandler");
			//AllowMouseEvents = true;
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
		/// Adds a handler for the <see cref="PreviewGestureEvent"/> attached event.
		/// </summary>
		/// <param name="element"><see cref="UIElement"/> or <see cref="ContentElement"/> that listens to this event.</param>
		/// <param name="handler">The handler.</param>
		public static void AddPreviewGestureHandler(DependencyObject element, GestureEventHandler handler)
		{
			addHandlerMethod.Invoke(element, PreviewGestureEvent, handler);
		}

		/// <summary>
		/// Removes a handler for the <see cref="PreviewGestureEvent"/> attached event.
		/// </summary>
		/// <param name="element"><see cref="UIElement"/> or <see cref="ContentElement"/> that listens to this event.</param>
		/// <param name="handler">The handler.</param>
		public static void RemovePreviewGestureHandler(DependencyObject element, GestureEventHandler handler)
		{
			removeHandlerMethod.Invoke(element, PreviewGestureEvent, handler);
		}

		/// <summary>
		/// Adds a handler <see cref="GestureEvent"/> attached event.
		/// </summary>
		/// <param name="element"><see cref="UIElement"/> or <see cref="ContentElement"/> that listens to this event.</param>
		/// <param name="handler">The handler.</param>
		public static void AddGestureHandler(DependencyObject element, GestureEventHandler handler)
		{
			addHandlerMethod.Invoke(element, GestureEvent, handler);
		}

		/// <summary>
		/// Removes a handler for the <see cref="GestureEvent"/> attached event.
		/// </summary>
		/// <param name="element"><see cref="UIElement"/> or <see cref="ContentElement"/> that listens to this event.</param>
		/// <param name="handler">The handler.</param>
		public static void RemoveGestureHandler(DependencyObject element, GestureEventHandler handler)
		{
			removeHandlerMethod.Invoke(element, GestureEvent, handler);
		}

		/// <summary>
		/// Gets or sets a value indicating whether gestures are enabled.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if gestures are enabled; otherwise, <c>false</c>.
		/// </value>
		public static bool IsGesturesEnabled { get; set; }

		/// <summary>
		/// Enables the specified gestures.
		/// </summary>
		/// <param name="gesture">The gesture.</param>
		public static void EnableGestures(params ApplicationGesture[] gesture)
		{
			IsGesturesEnabled = !gesture.Contains(ApplicationGesture.NoGesture);
			MultitouchLogic.Current.GestureManager.EnableGestures(gesture);
		}

		/// <summary>
		/// Gets or sets a value indicating whether mouse events are allowed.
		/// </summary>
		/// <value><c>false</c> - all mouse events will be canceled in MultitouchLogic at PreProcessInput stage.</value>
		public static bool AllowMouseEvents { get; set; }
	}
}