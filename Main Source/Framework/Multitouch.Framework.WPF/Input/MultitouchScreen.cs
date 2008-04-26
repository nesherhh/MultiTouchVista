using System;
using System.Reflection;
using System.Windows;

namespace Multitouch.Framework.WPF.Input
{
	public static class MultitouchScreen
	{
		public static readonly RoutedEvent PreviewNewContactEvent = EventManager.RegisterRoutedEvent("PreviewNewContact",
			RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(MultitouchScreen));
		public static readonly RoutedEvent PreviewContactRemovedEvent = EventManager.RegisterRoutedEvent("PreviewContactRemoved",
			RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(MultitouchScreen));
		public static readonly RoutedEvent PreviewContactMovedEvent = EventManager.RegisterRoutedEvent("PreviewContactMoved",
			RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(MultitouchScreen));
		
		public static readonly RoutedEvent NewContactEvent = EventManager.RegisterRoutedEvent("NewContact", RoutingStrategy.Bubble,
			typeof(RoutedEventHandler), typeof(MultitouchScreen));
		public static readonly RoutedEvent ContactRemovedEvent = EventManager.RegisterRoutedEvent("ContactRemoved", RoutingStrategy.Bubble,
			typeof(RoutedEventHandler), typeof(MultitouchScreen));
		public static readonly RoutedEvent ContactMovedEvent = EventManager.RegisterRoutedEvent("ContactMoved", RoutingStrategy.Bubble,
			typeof(RoutedEventHandler), typeof(MultitouchScreen));

		public static readonly RoutedEvent ContactEnterEvent = EventManager.RegisterRoutedEvent("ContactEnter", RoutingStrategy.Bubble,
			typeof(RoutedEventHandler), typeof(MultitouchScreen));
		public static readonly RoutedEvent ContactLeaveEvent = EventManager.RegisterRoutedEvent("ContactLeave", RoutingStrategy.Bubble,
			typeof(RoutedEventHandler), typeof(MultitouchScreen));

		static MethodInfo addHandlerMethod;
		static MethodInfo removeHandlerMethod;

		static MultitouchScreen()
		{
			Type uiElementType = typeof(UIElement);
			addHandlerMethod = uiElementType.GetMethod("AddHandler", BindingFlags.NonPublic | BindingFlags.Static, null,
														new[] { typeof(DependencyObject), typeof(RoutedEvent), typeof(Delegate) }, null);
			removeHandlerMethod = uiElementType.GetMethod("RemoveHandler", BindingFlags.NonPublic | BindingFlags.Static, null,
														new[] { typeof(DependencyObject), typeof(RoutedEvent), typeof(Delegate) }, null);
		}

		public static void AddNewContactHandler(DependencyObject element, RoutedEventHandler handler)
		{
			addHandlerMethod.Invoke(null, new object[] { element, NewContactEvent, handler });
		}

		public static void RemoveNewContactHandler(DependencyObject element, RoutedEventHandler handler)
		{
			removeHandlerMethod.Invoke(null, new object[] { element, NewContactEvent, handler });
		}

		public static void AddContactMovedHandler(DependencyObject element, RoutedEventHandler handler)
		{
			addHandlerMethod.Invoke(null, new object[] { element, ContactMovedEvent, handler });
		}

		public static void RemoveContactMovedHandler(DependencyObject element, RoutedEventHandler handler)
		{
			removeHandlerMethod.Invoke(null, new object[] { element, ContactMovedEvent, handler });
		}

		public static void AddContactRemovedHandler(DependencyObject element, RoutedEventHandler handler)
		{
			addHandlerMethod.Invoke(null, new object[] { element, ContactRemovedEvent, handler });
		}

		public static void RemoveContactRemovedHandler(DependencyObject element, RoutedEventHandler handler)
		{
			removeHandlerMethod.Invoke(null, new object[] { element, ContactRemovedEvent, handler });
		}
	}
}