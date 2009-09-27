using System;
using System.Windows;

namespace Danilins.Multitouch.Framework
{
	internal class EventUtility
	{
		public static void AddEventHandler(DependencyObject element, RoutedEvent routedEvent, Delegate handler)
		{
			UIElement uiElement = element as UIElement;
			if (uiElement != null)
				uiElement.AddHandler(routedEvent, handler);
			else
			{
				ContentElement contentElement = element as ContentElement;
				if (contentElement != null)
					contentElement.AddHandler(routedEvent, handler);
				else
				{
					UIElement3D uiElement3d = element as UIElement3D;
					if (uiElement3d != null)
						uiElement3d.AddHandler(routedEvent, handler);
				}
			}
		}

		public static void RemoveEventHandler(DependencyObject element, RoutedEvent routedEvent, Delegate handler)
		{
			UIElement uiElement = element as UIElement;
			if (uiElement != null)
				uiElement.RemoveHandler(routedEvent, handler);
			else
			{
				ContentElement contentElement = element as ContentElement;
				if (contentElement != null)
					contentElement.RemoveHandler(routedEvent, handler);
				else
				{
					UIElement3D uiElement3d = element as UIElement3D;
					if (uiElement3d != null)
						uiElement3d.RemoveHandler(routedEvent, handler);
				}
			}
		}
	}
}