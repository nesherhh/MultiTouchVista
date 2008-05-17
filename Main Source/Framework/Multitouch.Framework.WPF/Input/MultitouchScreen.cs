using System;
using System.Linq;
using System.Windows;
using System.Windows.Ink;
using Phydeaux.Utilities;

namespace Multitouch.Framework.WPF.Input
{
	public delegate void ContactEventHandler(object sender, ContactEventArgs e);
	public delegate void NewContactEventHandler(object sender, NewContactEventArgs e);
	public delegate void GestureEventHandler(object sender, GestureEventArgs e);

	delegate void RawMultitouchReportHandler(object sender, RawMultitouchReport e);

	public static class MultitouchScreen
	{
		public static readonly RoutedEvent PreviewNewContactEvent = EventManager.RegisterRoutedEvent("PreviewNewContact",
			RoutingStrategy.Tunnel, typeof(NewContactEventHandler), typeof(MultitouchScreen));
		public static readonly RoutedEvent PreviewContactRemovedEvent = EventManager.RegisterRoutedEvent("PreviewContactRemoved",
			RoutingStrategy.Tunnel, typeof(ContactEventHandler), typeof(MultitouchScreen));
		public static readonly RoutedEvent PreviewContactMovedEvent = EventManager.RegisterRoutedEvent("PreviewContactMoved",
			RoutingStrategy.Tunnel, typeof(ContactEventHandler), typeof(MultitouchScreen));

		public static readonly RoutedEvent NewContactEvent = EventManager.RegisterRoutedEvent("NewContact", RoutingStrategy.Bubble,
			typeof(NewContactEventHandler), typeof(MultitouchScreen));
		public static readonly RoutedEvent ContactRemovedEvent = EventManager.RegisterRoutedEvent("ContactRemoved", RoutingStrategy.Bubble,
			typeof(ContactEventHandler), typeof(MultitouchScreen));
		public static readonly RoutedEvent ContactMovedEvent = EventManager.RegisterRoutedEvent("ContactMoved", RoutingStrategy.Bubble,
			typeof(ContactEventHandler), typeof(MultitouchScreen));

		public static readonly RoutedEvent ContactEnterEvent = EventManager.RegisterRoutedEvent("ContactEnter", RoutingStrategy.Bubble,
			typeof(ContactEventHandler), typeof(MultitouchScreen));
		public static readonly RoutedEvent ContactLeaveEvent = EventManager.RegisterRoutedEvent("ContactLeave", RoutingStrategy.Bubble,
			typeof(ContactEventHandler), typeof(MultitouchScreen));

		public static readonly RoutedEvent PreviewGestureEvent = EventManager.RegisterRoutedEvent("PreviewGesture", RoutingStrategy.Tunnel, typeof(GestureEventHandler),
			typeof(MultitouchScreen));
		public static readonly RoutedEvent GestureEvent = EventManager.RegisterRoutedEvent("Gesture", RoutingStrategy.Bubble, typeof(GestureEventHandler),
			typeof(MultitouchScreen));

		static StaticProc<UIElement, DependencyObject, RoutedEvent, Delegate> addHandlerMethod;
		static StaticProc<UIElement, DependencyObject, RoutedEvent, Delegate> removeHandlerMethod;

		static MultitouchScreen()
		{
			addHandlerMethod = Dynamic<UIElement>.Static.Procedure.Explicit<DependencyObject, RoutedEvent, Delegate>.CreateDelegate("AddHandler");
			removeHandlerMethod = Dynamic<UIElement>.Static.Procedure.Explicit<DependencyObject, RoutedEvent, Delegate>.CreateDelegate("RemoveHandler");
		}

		public static void AddNewContactHandler(DependencyObject element, NewContactEventHandler handler)
		{
			addHandlerMethod.Invoke(element, NewContactEvent, handler);
		}

		public static void RemoveNewContactHandler(DependencyObject element, NewContactEventHandler handler)
		{
			removeHandlerMethod.Invoke(element, NewContactEvent, handler);
		}

		public static void AddContactMovedHandler(DependencyObject element, ContactEventHandler handler)
		{
			addHandlerMethod.Invoke(element, ContactMovedEvent, handler);
		}

		public static void RemoveContactMovedHandler(DependencyObject element, ContactEventHandler handler)
		{
			removeHandlerMethod.Invoke(element, ContactMovedEvent, handler);
		}

		public static void AddContactRemovedHandler(DependencyObject element, ContactEventHandler handler)
		{
			addHandlerMethod.Invoke(element, ContactRemovedEvent, handler);
		}

		public static void RemoveContactRemovedHandler(DependencyObject element, ContactEventHandler handler)
		{
			removeHandlerMethod.Invoke(element, ContactRemovedEvent, handler);
		}

		public static void AddContactEnterHandler(DependencyObject element, ContactEventHandler handler)
		{
			addHandlerMethod.Invoke(element, ContactEnterEvent, handler);
		}

		public static void RemoveContactEnterHandler(DependencyObject element, ContactEventHandler handler)
		{
			removeHandlerMethod.Invoke(element, ContactEnterEvent, handler);
		}

		public static void AddContactLeaveHandler(DependencyObject element, ContactEventHandler handler)
		{
			addHandlerMethod.Invoke(element, ContactLeaveEvent, handler);
		}

		public static void RemoveContactLeaveHandler(DependencyObject element, ContactEventHandler handler)
		{
			removeHandlerMethod.Invoke(element, ContactLeaveEvent, handler);
		}

		public static void AddPreviewGestureHandler(DependencyObject element, GestureEventHandler handler)
		{
			addHandlerMethod.Invoke(element, PreviewGestureEvent, handler);
		}

		public static void RemovePreviewGestureHandler(DependencyObject element, GestureEventHandler handler)
		{
			removeHandlerMethod.Invoke(element, PreviewGestureEvent, handler);
		}

		public static void AddGestureHandler(DependencyObject element, GestureEventHandler handler)
		{
			addHandlerMethod.Invoke(element, GestureEvent, handler);
		}

		public static void RemoveGestureHandler(DependencyObject element, GestureEventHandler handler)
		{
			removeHandlerMethod.Invoke(element, GestureEvent, handler);
		}

		public static bool IsGesturesEnabled { get; set; }

		public static void EnableGestures(params ApplicationGesture[] gesture)
		{
			IsGesturesEnabled = !gesture.Contains(ApplicationGesture.NoGesture);
			MultitouchLogic.Current.GestureManager.EnableGestures(gesture);
		}
	}
}