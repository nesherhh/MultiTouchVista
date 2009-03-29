using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Multitouch.Framework.Input.Service;
using Multitouch.Framework.WPF.Input;
using Contact=Multitouch.Framework.Input.Contact;

namespace Multitouch.Framework.WPF.Tests
{
	public class ContactTestsBase
	{
		protected readonly Queue<RoutedEventArgsInfo> events = new Queue<RoutedEventArgsInfo>();
		protected readonly ManualResetEvent resetEvent = new ManualResetEvent(false);
		protected TestWindow window;
		internal MultitouchLogic multitouchLogic;

		protected static void CheckContactEventArgs(RoutedEventArgsInfo e, RoutedEvent routedEvent, object source)
		{
			Assert.AreEqual(source, e.Source, "Source failed");
			Assert.AreEqual(routedEvent, e.RoutedEvent, "RoutedEvent failed");
		}

		internal static ContactData GetContactData(int id, ContactState state)
		{
			return GetContactData(id, state, new Point(100, 100));
		}

		internal static ContactData GetContactData(int id, ContactState state, Point point)
		{
			ContactData contactData = new ContactData();
			contactData.Position = point;
			contactData.Id = id;
			contactData.MajorAxis = 20;
			contactData.MinorAxis = 30;
			contactData.State = state;
			contactData.Orientation = 0;
			return contactData;
		}

		protected void HandleEvent(object sender, ContactEventArgs e)
		{
			events.Enqueue(new RoutedEventArgsInfo(e));
		}

		internal static ContactContext CreateContact(ContactData contactData, long timestamp, PresentationSource source)
		{
			return new ContactContext(new Contact(contactData, timestamp), source.RootVisual as UIElement);
		}

		protected static void Run(Action action)
		{
			Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, action);
			Dispatcher.Run();
		}

		[TestInitialize]
		public void Initialize()
		{
			events.Clear();
			multitouchLogic = MultitouchLogic.Current;
		}

		[TestCleanup]
		public void CleanUp()
		{
			Dispatcher.CurrentDispatcher.InvokeShutdown();
		}
	}
}
