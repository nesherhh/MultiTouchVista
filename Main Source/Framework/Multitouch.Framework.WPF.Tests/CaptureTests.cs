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
	public class CaptureTests : ContactTestsBase
	{
		[TestMethod]
		public void CaptureElement()
		{
			Run(() =>
				{
					window = new TestWindow();
					window.NewContact += HandleEvent;
					window.ContactMoved += HandleEvent;
					window.ContactRemoved += HandleEvent;
					window.Show();

					HwndSource source = (HwndSource)PresentationSource.FromVisual(window);

					window.ExecuteOnNextContact(c => c.Capture(window.testElement));

					RawMultitouchReport report = new RawMultitouchReport(CreateContact(GetContactData(0, ContactState.New, new Point(100, 100)), 0, source));
					InputManager.Current.ProcessInput(report);

					report = new RawMultitouchReport(CreateContact(GetContactData(0, ContactState.Moved, new Point(130, 100)), 1, source));
					InputManager.Current.ProcessInput(report);

					report = new RawMultitouchReport(CreateContact(GetContactData(0, ContactState.Moved, new Point(200, 100)), 2, source));
					InputManager.Current.ProcessInput(report);

					report = new RawMultitouchReport(CreateContact(GetContactData(0, ContactState.Removed, new Point(250, 100)), 3, source));
					InputManager.Current.ProcessInput(report);

					Dispatcher.ExitAllFrames();

					resetEvent.Set();

				});
			resetEvent.WaitOne();

			Assert.AreEqual(4, events.Count);

			RoutedEventArgsInfo dequeue = events.Dequeue();
			CheckContactEventArgs(dequeue, MultitouchScreen.NewContactEvent, window.testElement); // 100,100
			dequeue = events.Dequeue();
			CheckContactEventArgs(dequeue, MultitouchScreen.ContactMovedEvent, window.testElement); //130,100
			dequeue = events.Dequeue();
			CheckContactEventArgs(dequeue, MultitouchScreen.ContactMovedEvent, window.testElement); //200,100
			dequeue = events.Dequeue();
			CheckContactEventArgs(dequeue, MultitouchScreen.ContactRemovedEvent, window.testElement); //250,100
		}

		[TestMethod]
		public void GotCaptureLostCapture()
		{
			Run(() =>
				{
					window = new TestWindow();

					window.testElement.NewContact += HandleEvent;
					window.testElement.ContactMoved += HandleEvent;
					window.testElement.ContactRemoved += HandleEvent;
					window.testElement.GotContactCapture += HandleEvent;
					window.testElement.LostContactCapture += HandleEvent;

					window.testElement2.NewContact += HandleEvent;
					window.testElement2.ContactMoved += HandleEvent;
					window.testElement2.ContactRemoved += HandleEvent;
					window.testElement2.GotContactCapture += HandleEvent;
					window.testElement2.LostContactCapture += HandleEvent;

					window.Show();

					HwndSource source = (HwndSource)PresentationSource.FromVisual(window);

					window.testElement.ExecuteOnNextContact(c => c.Capture(window.testElement));

					RawMultitouchReport report = new RawMultitouchReport(CreateContact(GetContactData(0, ContactState.New, new Point(100, 100)), 0, source));
					InputManager.Current.ProcessInput(report);

					report = new RawMultitouchReport(CreateContact(GetContactData(0, ContactState.Moved, new Point(130, 100)), 1, source));
					InputManager.Current.ProcessInput(report);

					report = new RawMultitouchReport(CreateContact(GetContactData(0, ContactState.Moved, new Point(130, 225)), 2, source));
					InputManager.Current.ProcessInput(report);

					window.testElement.ExecuteOnNextContact(c => c.Capture(window.testElement2));

					report = new RawMultitouchReport(CreateContact(GetContactData(0, ContactState.Moved, new Point(75, 225)), 3, source));
					InputManager.Current.ProcessInput(report);

					report = new RawMultitouchReport(CreateContact(GetContactData(0, ContactState.Removed, new Point(30, 225)), 3, source));
					InputManager.Current.ProcessInput(report);

					Dispatcher.ExitAllFrames();

					resetEvent.Set();
				});
			resetEvent.WaitOne();

			Assert.AreEqual(9, events.Count);

			RoutedEventArgsInfo dequeue = events.Dequeue();
			CheckContactEventArgs(dequeue, MultitouchScreen.NewContactEvent, window.testElement); // 100,100
			dequeue = events.Dequeue();
			CheckContactEventArgs(dequeue, MultitouchScreen.GotContactCaptureEvent, window.testElement);
			dequeue = events.Dequeue();
			CheckContactEventArgs(dequeue, MultitouchScreen.ContactMovedEvent, window.testElement); //130,100
			dequeue = events.Dequeue();
			CheckContactEventArgs(dequeue, MultitouchScreen.ContactMovedEvent, window.testElement); //130,225
			dequeue = events.Dequeue();
			CheckContactEventArgs(dequeue, MultitouchScreen.ContactMovedEvent, window.testElement); //75,225
			dequeue = events.Dequeue();
			CheckContactEventArgs(dequeue, MultitouchScreen.LostContactCaptureEvent, window.testElement);
			dequeue = events.Dequeue();
			CheckContactEventArgs(dequeue, MultitouchScreen.GotContactCaptureEvent, window.testElement2);
			dequeue = events.Dequeue();
			CheckContactEventArgs(dequeue, MultitouchScreen.ContactRemovedEvent, window.testElement2); //30,225
			dequeue = events.Dequeue();
			CheckContactEventArgs(dequeue, MultitouchScreen.LostContactCaptureEvent, window.testElement2);
		}

		[TestMethod, Ignore, Description("don't know how subtree should work")]
		public void CaptureSubTree()
		{
			Run(() =>
			{
				window = new TestWindow();
				window.NewContact += HandleEvent;
				window.ContactMoved += HandleEvent;
				window.ContactRemoved += HandleEvent;
				window.Show();

				HwndSource source = (HwndSource)PresentationSource.FromVisual(window);

				window.ExecuteOnNextContact(c => c.Capture(window.list, CaptureMode.Element));

				RawMultitouchReport report = new RawMultitouchReport(CreateContact(GetContactData(0, ContactState.New, new Point(310, 100)), 0, source));
				InputManager.Current.ProcessInput(report);

				report = new RawMultitouchReport(CreateContact(GetContactData(0, ContactState.Moved, new Point(310, 180)), 1, source));
				InputManager.Current.ProcessInput(report);

				report = new RawMultitouchReport(CreateContact(GetContactData(0, ContactState.Moved, new Point(310, 200)), 2, source));
				InputManager.Current.ProcessInput(report);

				report = new RawMultitouchReport(CreateContact(GetContactData(0, ContactState.Removed, new Point(310, 230)), 3, source));
				InputManager.Current.ProcessInput(report);

				Dispatcher.ExitAllFrames();

				resetEvent.Set();
			});
			resetEvent.WaitOne();

			Assert.AreEqual(4, events.Count);

			RoutedEventArgsInfo dequeue = events.Dequeue();
			CheckContactEventArgs(dequeue, MultitouchScreen.NewContactEvent, window.b1); // 100,100
			dequeue = events.Dequeue();
			CheckContactEventArgs(dequeue, MultitouchScreen.ContactMovedEvent, window.b1); //130,100
			dequeue = events.Dequeue();
			CheckContactEventArgs(dequeue, MultitouchScreen.ContactMovedEvent, window.b1); //200,100
			dequeue = events.Dequeue();
			CheckContactEventArgs(dequeue, MultitouchScreen.ContactRemovedEvent, window.b1); //250,10			
		}

		[TestMethod]
		public void GetContactsCaptured()
		{
			Run(() =>
			    {
			    	window = new TestWindow();
					window.NewContact += HandleEvent;
			    	window.Show();

			    	HwndSource source = (HwndSource)PresentationSource.FromVisual(window);

					Assert.AreEqual(0, MultitouchScreen.GetContactsCaptured(window.testElement).Count());

                    window.ExecuteOnNextContact(c => c.Capture(window.testElement));

			    	RawMultitouchReport report = new RawMultitouchReport(CreateContact(GetContactData(0, ContactState.New, new Point(100, 100)), 0, source));
			    	InputManager.Current.ProcessInput(report);

					Assert.AreEqual(1, MultitouchScreen.GetContactsCaptured(window.testElement).Count());

					MultitouchLogic.Current.ContactsManager.ExistingContacts.First().Value.ReleaseCapture();

					Assert.AreEqual(0, MultitouchScreen.GetContactsCaptured(window.testElement).Count());

			    	Dispatcher.ExitAllFrames();

			    	resetEvent.Set();

			    });
			resetEvent.WaitOne();
		}

		[TestMethod]
		public void MultipleContactsCaptureSameObject()
		{
			Run(()=>
			    {
			    	window = new TestWindow();
					window.NewContact += HandleEvent;
					window.Show();

					HwndSource source = (HwndSource)PresentationSource.FromVisual(window);

					Assert.AreEqual(0, MultitouchScreen.GetContactsCaptured(window.testElement2).Count());
					Assert.AreEqual(0, MultitouchScreen.GetContactsCaptured(window.testElement).Count());
					window.ExecuteOnNextContact(c=>c.Capture(window.testElement));

					RawMultitouchReport report = new RawMultitouchReport(CreateContact(GetContactData(0, ContactState.New, new Point(100, 100)), 0, source));
					InputManager.Current.ProcessInput(report);

					Assert.AreEqual(1, MultitouchScreen.GetContactsCaptured(window.testElement).Count());
                    window.ExecuteOnNextContact(c => c.Capture(window.testElement));

					report = new RawMultitouchReport(CreateContact(GetContactData(1, ContactState.New, new Point(100, 100)), 1, source));
					InputManager.Current.ProcessInput(report);

					Assert.AreEqual(2, MultitouchScreen.GetContactsCaptured(window.testElement).Count());
					Assert.AreEqual(0, MultitouchScreen.GetContactsCaptured(window.testElement2).Count());

					Dispatcher.ExitAllFrames();
					resetEvent.Set();
				});
			resetEvent.WaitOne();
		}

		[TestMethod, Ignore, Description("Physic engine in TouchablePanel does not have enough time to move elements")]
		public void TouchablePanelCapture()
		{
			Run(() =>
				{
					window = new TestWindow();
					window.Show();

					HwndSource source = (HwndSource)PresentationSource.FromVisual(window);

					Point rectPosition = GetPosition(window.panel, window.rect, true);
					Point contactPosition = rectPosition;
					contactPosition.Offset(-20, 0);
					MultitouchScreen.AddContactEnterHandler(window.rect, (sender, e) => e.Contact.Capture((IInputElement)e.Source));

					RawMultitouchReport report = new RawMultitouchReport(CreateContact(GetContactData(0, ContactState.New, contactPosition), 1, source));
					InputManager.Current.ProcessInput(report);
					Assert.AreEqual(rectPosition, GetPosition(window.panel, window.rect, true));

					contactPosition.Offset(20, 0);
					report = new RawMultitouchReport(CreateContact(GetContactData(0, ContactState.Moved, contactPosition), 2, source));
					InputManager.Current.ProcessInput(report);
					Assert.AreEqual(rectPosition, GetPosition(window.panel, window.rect, true));

					contactPosition.Offset(20, 0);
					report = new RawMultitouchReport(CreateContact(GetContactData(0, ContactState.Moved, contactPosition), 3, source));
					InputManager.Current.ProcessInput(report);
					
					Point newPosition = rectPosition;
					newPosition.Offset(20, 0);

					Point position = GetPosition(window.panel, window.rect, true);
					Assert.AreNotEqual(rectPosition, position);
					Assert.AreEqual(newPosition, position);

					Dispatcher.ExitAllFrames();
					resetEvent.Set();
				});
			resetEvent.WaitOne();
		}
	}
}