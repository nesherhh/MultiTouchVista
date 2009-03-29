using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Multitouch.Framework.Input.Service;
using Multitouch.Framework.WPF.Input;

namespace Multitouch.Framework.WPF.Tests
{
	[TestClass]
	public class MultitouchLogicTests : ContactTestsBase
	{
		[TestMethod]
		public void NewContactEvent()
		{
			ContactData contactData = GetContactData(0, ContactState.New);

			Run(() =>
				{
					window = new TestWindow();
					window.NewContact += HandleEvent;
					window.PreviewNewContact += HandleEvent;
					window.Show();

					HwndSource source = (HwndSource)PresentationSource.FromVisual(window);
					contactData.Hwnd = source.Handle;

					RawMultitouchReport report = new RawMultitouchReport(CreateContact(contactData, 0, source));
					InputManager.Current.ProcessInput(report);

					Dispatcher.ExitAllFrames();

					resetEvent.Set();
				});

			resetEvent.WaitOne();

			Assert.AreEqual(2, events.Count);

			RoutedEventArgsInfo dequeue = events.Dequeue();
			Assert.AreEqual<RoutedEvent>(MultitouchScreen.PreviewNewContactEvent, dequeue.RoutedEvent);
			dequeue = events.Dequeue();
			Assert.AreEqual<RoutedEvent>(MultitouchScreen.NewContactEvent, dequeue.RoutedEvent);
		}

		[TestMethod]
		public void ContactRemovedEvent()
		{
			ContactData contactData = GetContactData(0, ContactState.New);

			Run(() =>
				{
					window = new TestWindow();
					window.ContactRemoved += HandleEvent;
					window.PreviewContactRemoved += HandleEvent;
					window.Show();

					HwndSource source = (HwndSource)PresentationSource.FromVisual(window);
					contactData.Hwnd = source.Handle;

					RawMultitouchReport report = new RawMultitouchReport(CreateContact(contactData, 0, source));
					InputManager.Current.ProcessInput(report);

					report = new RawMultitouchReport(CreateContact(GetContactData(0, ContactState.Removed), 0, source));
					InputManager.Current.ProcessInput(report);

					Dispatcher.ExitAllFrames();

					resetEvent.Set();
				});

			resetEvent.WaitOne();

			Assert.AreEqual(2, events.Count);

			RoutedEventArgsInfo dequeue = events.Dequeue();
			Assert.AreEqual<RoutedEvent>(MultitouchScreen.PreviewContactRemovedEvent, dequeue.RoutedEvent);
			dequeue = events.Dequeue();
			Assert.AreEqual<RoutedEvent>(MultitouchScreen.ContactRemovedEvent, dequeue.RoutedEvent);
		}

		[TestMethod]
		public void ContactMovedEvent()
		{
			ContactData contactData = GetContactData(0, ContactState.New);

			Run(() =>
				{
					window = new TestWindow();
					window.ContactMoved += HandleEvent;
					window.PreviewContactMoved += HandleEvent;
					window.Show();

					HwndSource source = (HwndSource)PresentationSource.FromVisual(window);
					contactData.Hwnd = source.Handle;

					RawMultitouchReport report = new RawMultitouchReport(CreateContact(contactData, 0, source));
					InputManager.Current.ProcessInput(report);

					report = new RawMultitouchReport(CreateContact(GetContactData(0, ContactState.Moved), 0, source));
					InputManager.Current.ProcessInput(report);

					Dispatcher.ExitAllFrames();

					resetEvent.Set();
				});

			resetEvent.WaitOne();

			Assert.AreEqual(2, events.Count);

			RoutedEventArgsInfo dequeue = events.Dequeue();
			Assert.AreEqual<RoutedEvent>(MultitouchScreen.PreviewContactMovedEvent, dequeue.RoutedEvent);
			dequeue = events.Dequeue();
			Assert.AreEqual<RoutedEvent>(MultitouchScreen.ContactMovedEvent, dequeue.RoutedEvent);
		}

		[TestMethod]
		public void TestAllEvents()
		{
			Run(() =>
				{
					window = new TestWindow();
					window.NewContact += HandleEvent;
					window.PreviewNewContact += HandleEvent;
					window.ContactMoved += HandleEvent;
					window.PreviewContactMoved += HandleEvent;
					window.ContactRemoved += HandleEvent;
					window.PreviewContactRemoved += HandleEvent;
					window.Show();

					HwndSource source = (HwndSource)PresentationSource.FromVisual(window);

					RawMultitouchReport report = new RawMultitouchReport(CreateContact(GetContactData(0, ContactState.New), 0, source));
					InputManager.Current.ProcessInput(report);

					report = new RawMultitouchReport(CreateContact(GetContactData(0, ContactState.Moved), 1, source));
					InputManager.Current.ProcessInput(report);

					report = new RawMultitouchReport(CreateContact(GetContactData(0, ContactState.Removed), 2, source));
					InputManager.Current.ProcessInput(report);

					Dispatcher.ExitAllFrames();

					resetEvent.Set();
				});

			resetEvent.WaitOne();

			Assert.AreEqual(6, events.Count);

			RoutedEventArgsInfo dequeue = events.Dequeue();
			Assert.AreEqual<RoutedEvent>(MultitouchScreen.PreviewNewContactEvent, dequeue.RoutedEvent);
			dequeue = events.Dequeue();
			Assert.AreEqual<RoutedEvent>(MultitouchScreen.NewContactEvent, dequeue.RoutedEvent);
			dequeue = events.Dequeue();
			Assert.AreEqual<RoutedEvent>(MultitouchScreen.PreviewContactMovedEvent, dequeue.RoutedEvent);
			dequeue = events.Dequeue();
			Assert.AreEqual<RoutedEvent>(MultitouchScreen.ContactMovedEvent, dequeue.RoutedEvent);
			dequeue = events.Dequeue();
			Assert.AreEqual<RoutedEvent>(MultitouchScreen.PreviewContactRemovedEvent, dequeue.RoutedEvent);
			dequeue = events.Dequeue();
			Assert.AreEqual<RoutedEvent>(MultitouchScreen.ContactRemovedEvent, dequeue.RoutedEvent);
		}

		[TestMethod]
		public void EnterLeaveEvents()
		{
			Run(() =>
				{
					window = new TestWindow();

					window.PreviewNewContact += HandleEvent;
					window.NewContact += HandleEvent;

					window.PreviewContactMoved += HandleEvent;
					window.ContactMoved += HandleEvent;

					window.PreviewContactRemoved += HandleEvent;
					window.ContactRemoved += HandleEvent;

					window.ContactEnter += HandleEvent;
					window.ContactLeave += HandleEvent;

					MultitouchScreen.AddContactEnterHandler(window.canvas, HandleEvent);
					MultitouchScreen.AddContactLeaveHandler(window.canvas, HandleEvent);

					window.testElement.ContactEnter += HandleEvent;
					window.testElement.ContactLeave += HandleEvent;

					window.Show();

					HwndSource source = (HwndSource)PresentationSource.FromVisual(window);

					RawMultitouchReport report = new RawMultitouchReport(CreateContact(GetContactData(0, ContactState.New, new Point(30, 100)), 0, source));
					InputManager.Current.ProcessInput(report);

					report = new RawMultitouchReport(CreateContact(GetContactData(0, ContactState.Moved, new Point(49, 100)), 1, source));
					InputManager.Current.ProcessInput(report);

					report = new RawMultitouchReport(CreateContact(GetContactData(0, ContactState.Moved, new Point(100, 100)), 2, source));
					InputManager.Current.ProcessInput(report);

					report = new RawMultitouchReport(CreateContact(GetContactData(0, ContactState.Moved, new Point(150, 100)), 3, source));
					InputManager.Current.ProcessInput(report);

					report = new RawMultitouchReport(CreateContact(GetContactData(0, ContactState.Moved, new Point(200, 100)), 4, source));
					InputManager.Current.ProcessInput(report);

					report = new RawMultitouchReport(CreateContact(GetContactData(0, ContactState.Removed, new Point(200, 100)), 5, source));
					InputManager.Current.ProcessInput(report);

					Dispatcher.ExitAllFrames();

					resetEvent.Set();
				});

			resetEvent.WaitOne();

			Assert.AreEqual(18, events.Count);

			RoutedEventArgsInfo dequeue = events.Dequeue();
			CheckContactEventArgs(dequeue, MultitouchScreen.ContactEnterEvent, window); // 30,100
			dequeue = events.Dequeue();
			CheckContactEventArgs(dequeue, MultitouchScreen.PreviewNewContactEvent, window);
			dequeue = events.Dequeue();
			CheckContactEventArgs(dequeue, MultitouchScreen.NewContactEvent, window);
			dequeue = events.Dequeue();
			CheckContactEventArgs(dequeue, MultitouchScreen.PreviewContactMovedEvent, window); // 49,100
			dequeue = events.Dequeue();
			CheckContactEventArgs(dequeue, MultitouchScreen.ContactMovedEvent, window);
			dequeue = events.Dequeue();
			CheckContactEventArgs(dequeue, MultitouchScreen.ContactEnterEvent, window.testElement); //100,100
			dequeue = events.Dequeue();
			CheckContactEventArgs(dequeue, MultitouchScreen.ContactEnterEvent, window.canvas);
			dequeue = events.Dequeue();
			CheckContactEventArgs(dequeue, MultitouchScreen.PreviewContactMovedEvent, window.testElement);
			dequeue = events.Dequeue();
			CheckContactEventArgs(dequeue, MultitouchScreen.ContactMovedEvent, window.testElement);
			dequeue = events.Dequeue();
			CheckContactEventArgs(dequeue, MultitouchScreen.PreviewContactMovedEvent, window.testElement); //150,100
			dequeue = events.Dequeue();
			CheckContactEventArgs(dequeue, MultitouchScreen.ContactMovedEvent, window.testElement);
			dequeue = events.Dequeue();
			CheckContactEventArgs(dequeue, MultitouchScreen.ContactLeaveEvent, window.testElement); //200,100
			dequeue = events.Dequeue();
			CheckContactEventArgs(dequeue, MultitouchScreen.ContactLeaveEvent, window.canvas);
			dequeue = events.Dequeue();
			CheckContactEventArgs(dequeue, MultitouchScreen.PreviewContactMovedEvent, window);
			dequeue = events.Dequeue();
			CheckContactEventArgs(dequeue, MultitouchScreen.ContactMovedEvent, window);
			dequeue = events.Dequeue();
			CheckContactEventArgs(dequeue, MultitouchScreen.PreviewContactRemovedEvent, window); //200,100
			dequeue = events.Dequeue();
			CheckContactEventArgs(dequeue, MultitouchScreen.ContactRemovedEvent, window);
			dequeue = events.Dequeue();
			CheckContactEventArgs(dequeue, MultitouchScreen.ContactLeaveEvent, window);
		}

		[TestMethod]
		public void ContactHistory()
		{
			Run(() =>
				{
					window = new TestWindow();
					window.Show();

					ContactsManager manager = MultitouchLogic.Current.ContactsManager;

					HwndSource source = (HwndSource)PresentationSource.FromVisual(window);

					RawMultitouchReport report = new RawMultitouchReport(CreateContact(GetContactData(0, ContactState.New, new Point(100, 100)), 0, source));
					InputManager.Current.ProcessInput(report);

					Assert.AreEqual(1, manager.ExistingContacts.Count);
					Contact contact = manager.ExistingContacts.First().Value;
					Assert.AreEqual(0, contact.GetPoints(null).Count());

					report = new RawMultitouchReport(CreateContact(GetContactData(0, ContactState.Moved, new Point(100, 120)), 1, source));
					InputManager.Current.ProcessInput(report);

					Assert.AreEqual(1, manager.ExistingContacts.Count);
					contact = manager.ExistingContacts.First().Value;
					Assert.AreEqual(1, contact.GetPoints(null).Count());
                    
					report = new RawMultitouchReport(CreateContact(GetContactData(0, ContactState.Moved, new Point(130, 120)), 2, source));
					InputManager.Current.ProcessInput(report);

					Assert.AreEqual(1, manager.ExistingContacts.Count);
					contact = manager.ExistingContacts.First().Value;
					Assert.AreEqual(2, contact.GetPoints(null).Count());

					report = new RawMultitouchReport(CreateContact(GetContactData(1, ContactState.New, new Point(130, 120)), 2, source));
					InputManager.Current.ProcessInput(report);

					Assert.AreEqual(2, manager.ExistingContacts.Count);
					contact = manager.ExistingContacts.First().Value;
					Assert.AreEqual(2, contact.GetPoints(null).Count());
					contact = manager.ExistingContacts.ElementAt(1).Value;
					Assert.AreEqual(0, contact.GetPoints(null).Count());
                    
					report = new RawMultitouchReport(CreateContact(GetContactData(0, ContactState.Removed, new Point(130, 120)), 3, source));
					InputManager.Current.ProcessInput(report);

					Assert.AreEqual(1, manager.ExistingContacts.Count);
					contact = manager.ExistingContacts.First().Value;
					Assert.AreEqual(0, contact.GetPoints(null).Count());

					report = new RawMultitouchReport(CreateContact(GetContactData(1, ContactState.Moved, new Point(140, 120)), 3, source));
					InputManager.Current.ProcessInput(report);

					Assert.AreEqual(1, manager.ExistingContacts.Count);
					contact = manager.ExistingContacts.First().Value;
					Assert.AreEqual(1, contact.GetPoints(null).Count());

					report = new RawMultitouchReport(CreateContact(GetContactData(1, ContactState.Removed, new Point(140, 120)), 4, source));
					InputManager.Current.ProcessInput(report);

					Assert.AreEqual(0, manager.ExistingContacts.Count);

					Dispatcher.ExitAllFrames();
					resetEvent.Set();
				});
			resetEvent.WaitOne();
		}
	}
}