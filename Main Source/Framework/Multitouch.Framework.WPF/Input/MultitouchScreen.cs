using System;
using System.Reflection;
using System.Windows;

namespace Multitouch.Framework.WPF.Input
{
	public delegate void ContactEventHandler(object sender, ContactEventArgs e);
	public delegate void NewContactEventHandler(object sender, NewContactEventArgs e);
	
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

		public static void AddNewContactHandler(DependencyObject element, NewContactEventHandler handler)
		{
			addHandlerMethod.Invoke(null, new object[] { element, NewContactEvent, handler });
		}

		public static void RemoveNewContactHandler(DependencyObject element, NewContactEventHandler handler)
		{
			removeHandlerMethod.Invoke(null, new object[] { element, NewContactEvent, handler });
		}

		public static void AddContactMovedHandler(DependencyObject element, ContactEventHandler handler)
		{
			addHandlerMethod.Invoke(null, new object[] { element, ContactMovedEvent, handler });
		}

		public static void RemoveContactMovedHandler(DependencyObject element, ContactEventHandler handler)
		{
			removeHandlerMethod.Invoke(null, new object[] { element, ContactMovedEvent, handler });
		}

		public static void AddContactRemovedHandler(DependencyObject element, ContactEventHandler handler)
		{
			addHandlerMethod.Invoke(null, new object[] { element, ContactRemovedEvent, handler });
		}

		public static void RemoveContactRemovedHandler(DependencyObject element, ContactEventHandler handler)
		{
			removeHandlerMethod.Invoke(null, new object[] { element, ContactRemovedEvent, handler });
		}
	}
}