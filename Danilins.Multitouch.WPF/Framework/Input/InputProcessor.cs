using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Danilins.Multitouch.Common;

namespace Danilins.Multitouch.Framework.Input
{
	class InputProcessor
	{
		readonly Dispatcher dispatcher;
		Dictionary<int, MultitouchDevice> devices;
		ContactClickStates clickStates;

		public InputProcessor(Dispatcher currentDispatcher)
		{
			dispatcher = currentDispatcher;
			devices = new Dictionary<int, MultitouchDevice>();
			clickStates = new ContactClickStates();
		}

		internal void Input(IEnumerable<ContactInfo> contactInfos)
		{
			foreach (ContactInfo contactInfo in contactInfos)
			{
				ContactAction clickAction = clickStates.GetAction(contactInfo);

				RoutedEvent routedEvent = null;
				switch (contactInfo.State)
				{
					case ContactState.Down:
						routedEvent = MultitouchScreen.ContactDownEvent;
						break;
					case ContactState.Up:
						routedEvent = MultitouchScreen.ContactUpEvent;
						break;
					case ContactState.Move:
						routedEvent = MultitouchScreen.ContactMoveEvent;
						break;
					default:
						break;
				}
				if (routedEvent != null)
				{
					dispatcher.Invoke(DispatcherPriority.Input,
						(Action)(delegate
						         {
						         	try
						         	{
						         		Point localPoint = Application.Current.MainWindow.PointFromScreen(contactInfo.Center);
						         		IInputElement inputElement = Application.Current.MainWindow.InputHitTest(localPoint);

						         		MultitouchDevice device;
						         		if (!devices.TryGetValue(contactInfo.Id, out device))
						         		{
						         			//novoe kasanie v predelah elementa. posilaem snachalo ContactEnterEvent.
						         			device = new MultitouchDevice(contactInfo.Id, inputElement);
						         			devices.Add(contactInfo.Id, device);
						         			ContactEventArgs mouseEnter = new ContactEventArgs(device, contactInfo);
						         			mouseEnter.RoutedEvent = MultitouchScreen.ContactEnterEvent;
						         			InputManager.Current.ProcessInput(mouseEnter);
						         		}
						         		else
						         		{
						         			if (device.Target != inputElement)
						         			{
						         				//staroe kasanie popalo na novij element. posilaem ContactLeaveEvent v starij element.
						         				ContactEventArgs mouseLeave = new ContactEventArgs(device, contactInfo);
						         				mouseLeave.RoutedEvent = MultitouchScreen.ContactLeaveEvent;
						         				try
						         				{
						         					InputManager.Current.ProcessInput(mouseLeave);
						         				}
						         				finally
						         				{
						         					devices.Remove(contactInfo.Id);
						         				}
						         			}
						         		}
						         		//posilaem event v element.
						         		RaiseInputEvent(device, inputElement, contactInfo, routedEvent);

										//esli bil click ili double click, to posilaem ih tozhe.
						         		if (clickAction == ContactAction.Click)
						         			RaiseInputEvent(device, inputElement, contactInfo, MultitouchScreen.ContactClickEvent);
						         		else if (clickAction == ContactAction.DoubleClick)
						         			RaiseInputEvent(device, inputElement, contactInfo, MultitouchScreen.ContactDoubleClickEvent);

						         		if (routedEvent == MultitouchScreen.ContactUpEvent)
						         			devices.Remove(contactInfo.Id);
						         	}
						         	catch (Exception ex)
						         	{
						         		Trace.TraceError(ex.ToString());
						         	}
						         }));
				}
			}
		}

		void RaiseInputEvent(MultitouchDevice device, IInputElement inputElement, ContactInfo contactInfo, RoutedEvent routedEvent)
		{
			ContactEventArgs e = new ContactEventArgs(device, contactInfo);
			device.SetTarget(inputElement);
			e.RoutedEvent = routedEvent;

			InputManager.Current.ProcessInput(e);
		}
	}
}