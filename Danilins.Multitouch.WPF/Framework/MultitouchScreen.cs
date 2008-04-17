using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Windows;
using System.Windows.Threading;
using Danilins.Multitouch.Common;
using Danilins.Multitouch.Framework.Input;
using Danilins.Multitouch.Framework.Service;

namespace Danilins.Multitouch.Framework
{
	public class MultitouchScreen : IDisposable
	{
		#region Events

		public static readonly RoutedEvent ContactClickEvent = EventManager.RegisterRoutedEvent("ContactClick", RoutingStrategy.Bubble,
			typeof(ContactEventHandler), typeof(MultitouchScreen));

		public static readonly RoutedEvent ContactDoubleClickEvent = EventManager.RegisterRoutedEvent("ContactDoubleClick", RoutingStrategy.Bubble,
			typeof(ContactEventHandler), typeof(MultitouchScreen));

		public static readonly RoutedEvent PreviewContactClickEvent = EventManager.RegisterRoutedEvent("PreviewContactClick", RoutingStrategy.Tunnel,
			typeof(ContactEventHandler), typeof(MultitouchScreen));

		public static readonly RoutedEvent PreviewContactDoubleClickEvent = EventManager.RegisterRoutedEvent("PreviewContactDoubleClick", RoutingStrategy.Tunnel,
			typeof(ContactEventHandler), typeof(MultitouchScreen));

		public static readonly RoutedEvent PreviewContactDownEvent = EventManager.RegisterRoutedEvent("PreviewContactDown", RoutingStrategy.Tunnel,
		                                                                                              typeof(ContactEventHandler), typeof(MultitouchScreen));

		public static readonly RoutedEvent ContactDownEvent = EventManager.RegisterRoutedEvent("ContactDown", RoutingStrategy.Bubble,
		                                                                                       typeof(ContactEventHandler), typeof(MultitouchScreen));

		public static readonly RoutedEvent PreviewContactUpEvent = EventManager.RegisterRoutedEvent("PreviewContactUp", RoutingStrategy.Tunnel,
		                                                                                            typeof(ContactEventHandler), typeof(MultitouchScreen));

		public static readonly RoutedEvent ContactUpEvent = EventManager.RegisterRoutedEvent("ContactUp", RoutingStrategy.Bubble,
		                                                                                     typeof(ContactEventHandler), typeof(MultitouchScreen));

		public static readonly RoutedEvent PreviewContactMoveEvent = EventManager.RegisterRoutedEvent("PreviewContactMove", RoutingStrategy.Tunnel,
		                                                                                              typeof(ContactEventHandler), typeof(MultitouchScreen));

		public static readonly RoutedEvent ContactMoveEvent = EventManager.RegisterRoutedEvent("ContactMove", RoutingStrategy.Bubble,
		                                                                                       typeof(ContactEventHandler), typeof(MultitouchScreen));

		public static readonly RoutedEvent ContactLeaveEvent = EventManager.RegisterRoutedEvent("ContactLeave", RoutingStrategy.Bubble,
			typeof(ContactEventHandler), typeof(MultitouchScreen));

		public static readonly RoutedEvent ContactEnterEvent = EventManager.RegisterRoutedEvent("ContactEnter", RoutingStrategy.Bubble,
			typeof(ContactEventHandler), typeof(MultitouchScreen));

		public static void AddContactClickHandler(DependencyObject element, ContactEventHandler handler)
		{
			EventUtility.AddEventHandler(element, ContactClickEvent, handler);
		}

		public static void RemoveContactClickHandler(DependencyObject element, ContactEventHandler handler)
		{
			EventUtility.RemoveEventHandler(element, ContactClickEvent, handler);
		}

		public static void AddContactDoubleClickHandler(DependencyObject element, ContactEventHandler handler)
		{
			EventUtility.AddEventHandler(element, ContactDoubleClickEvent, handler);
		}

		public static void RemoveContactDoubleClickHandler(DependencyObject element, ContactEventHandler handler)
		{
			EventUtility.RemoveEventHandler(element, ContactDoubleClickEvent, handler);
		}

		public static void AddPreviewContactClickHandler(DependencyObject element, ContactEventHandler handler)
		{
			EventUtility.AddEventHandler(element, PreviewContactClickEvent, handler);
		}

		public static void RemovePreviewContactClickHandler(DependencyObject element, ContactEventHandler handler)
		{
			EventUtility.RemoveEventHandler(element, PreviewContactClickEvent, handler);
		}

		public static void AddContactPreviewDoubleClickHandler(DependencyObject element, ContactEventHandler handler)
		{
			EventUtility.AddEventHandler(element, PreviewContactDoubleClickEvent, handler);
		}

		public static void RemovePreviewContactDoubleClickHandler(DependencyObject element, ContactEventHandler handler)
		{
			EventUtility.RemoveEventHandler(element, PreviewContactDoubleClickEvent, handler);
		}

		public static void AddPreviewContactDownHandler(DependencyObject element, ContactEventHandler handler)
		{
			EventUtility.AddEventHandler(element, PreviewContactDownEvent, handler);
		}

		public static void RemovePreviewContactDownHandler(DependencyObject element, ContactEventHandler handler)
		{
			EventUtility.RemoveEventHandler(element, PreviewContactDownEvent, handler);
		}

		public static void AddContactDownHandler(DependencyObject element, ContactEventHandler handler)
		{
			EventUtility.AddEventHandler(element, ContactDownEvent, handler);
		}

		public static void RemoveContactDownHandler(DependencyObject element, ContactEventHandler handler)
		{
			EventUtility.RemoveEventHandler(element, ContactDownEvent, handler);
		}

		public static void AddPreviewContactUpHandler(DependencyObject element, ContactEventHandler handler)
		{
			EventUtility.AddEventHandler(element, PreviewContactUpEvent, handler);
		}

		public static void RemovePreviewContactUpHandler(DependencyObject element, ContactEventHandler handler)
		{
			EventUtility.RemoveEventHandler(element, PreviewContactUpEvent, handler);
		}

		public static void AddContactUpHandler(DependencyObject element, ContactEventHandler handler)
		{
			EventUtility.AddEventHandler(element, ContactUpEvent, handler);
		}

		public static void RemoveContactUpHandler(DependencyObject element, ContactEventHandler handler)
		{
			EventUtility.RemoveEventHandler(element, ContactUpEvent, handler);
		}

		public static void AddPreviewContactMoveHandler(DependencyObject element, ContactEventHandler handler)
		{
			EventUtility.AddEventHandler(element, PreviewContactMoveEvent, handler);
		}

		public static void RemovePreviewContactMoveHandler(DependencyObject element, ContactEventHandler handler)
		{
			EventUtility.RemoveEventHandler(element, PreviewContactMoveEvent, handler);
		}

		public static void AddContactMoveHandler(DependencyObject element, ContactEventHandler handler)
		{
			EventUtility.AddEventHandler(element, ContactMoveEvent, handler);
		}

		public static void RemoveContactMoveHandler(DependencyObject element, ContactEventHandler handler)
		{
			EventUtility.RemoveEventHandler(element, ContactMoveEvent, handler);
		}

		public static void AddContactLeaveHandler(DependencyObject element, ContactEventHandler handler)
		{
			EventUtility.AddEventHandler(element, ContactLeaveEvent, handler);
		}

		public static void RemoveContactLeaveHandler(DependencyObject element, ContactEventHandler handler)
		{
			EventUtility.RemoveEventHandler(element, ContactLeaveEvent, handler);
		}

		public static void AddContactEnterHandler(DependencyObject element, ContactEventHandler handler)
		{
			EventUtility.AddEventHandler(element, ContactEnterEvent, handler);
		}

		public static void RemoveContactEnterHandler(DependencyObject element, ContactEventHandler handler)
		{
			EventUtility.RemoveEventHandler(element, ContactEnterEvent, handler);
		}

		#endregion

		private static volatile MultitouchScreen instace;

		private bool disposed;
		private static readonly object lockObject = new object();
		private MultitouchServiceClient serviceClient;
		private MultitouchServiceCallbackHandler callbackHandler;
		private bool isConnected;
		private MouseHandler mouseHandler;
		private InputProcessor inputProcessor;

		private MultitouchScreen()
		{
			isConnected = false;
			inputProcessor = new InputProcessor(Dispatcher.CurrentDispatcher);

			if (!ViewUtility.IsDesignTime)
			{
				ConnectToMultitouchService();
				if(IsConnected)
					Win32.HideSystemCursor();
			}
		}

		public bool IsConnected
		{
			get { return isConnected; }
		}

		private void ConnectToMultitouchService()
		{
			Uri uri = new Uri("net.pipe://localhost/Danilins.Multitouch.Logic.Service/MultitouchService");
			EndpointAddress remoteAddress = new EndpointAddress(uri);
			NetNamedPipeBinding namedPipeBinding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);


			callbackHandler = new MultitouchServiceCallbackHandler(this);
			InstanceContext context = new InstanceContext(callbackHandler);
			serviceClient = new MultitouchServiceClient(context, namedPipeBinding, remoteAddress);
			try
			{
				serviceClient.RegisterApplication();
				isConnected = true;

			}
			catch (EndpointNotFoundException)
			{
				Trace.TraceWarning("Can't connect to Multitouch service. Falling back to single mouse input.");
				EnableSystemMouseInput();
			}
		}

		private void EnableSystemMouseInput()
		{
			mouseHandler = new MouseHandler(this);
		}

		public static MultitouchScreen Instace
		{
			get
			{
				if (instace == null)
				{
					lock (lockObject)
					{
						if (instace == null)
							instace = new MultitouchScreen();
					}
				}
				return instace;
			}
		}

		internal void ProcessContacts(ContactInfo[] contacts)
		{
			if (contacts.Length > 0)
				inputProcessor.Input(contacts);
		}

		public void Dispose()
		{
			if (!disposed)
				Cleanup();
		}

		private void Cleanup()
		{
			if (IsConnected)
				Win32.ShowSystemCursor();
			else if (mouseHandler != null)
			{
				mouseHandler.Dispose();
				mouseHandler = null;
			}
			disposed = true;
			if (serviceClient != null)
			{
				if(IsConnected)
					serviceClient.UnregisterApplication();
				if(serviceClient.State == CommunicationState.Opened)
					serviceClient.Close();
				serviceClient = null;
			}
		}

		public void DisableLegacySupport()
		{
			if (IsConnected)
			{
				try
				{
					serviceClient.DisableLegacySupport();
				}
				catch (TimeoutException e)
				{
					Trace.TraceWarning(e.Message);
				}
			}
		}

		public void EnableLegacySupport()
		{
			if (IsConnected)
			{
				try
				{
					serviceClient.EnableLegacySupport();
				}
				catch (TimeoutException e)
				{
					Trace.TraceWarning(e.Message);
				}
			}
		}
	}
}