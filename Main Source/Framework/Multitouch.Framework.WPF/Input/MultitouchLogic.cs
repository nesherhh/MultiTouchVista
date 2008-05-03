using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Multitouch.Framework.Input;
using System.Diagnostics;

namespace Multitouch.Framework.WPF.Input
{
	class MultitouchLogic : DispatcherObject
	{
		static MultitouchLogic current;
		static object lockCurrent = new object();

		internal static readonly RoutedEvent PreviewRawInputEvent = EventManager.RegisterRoutedEvent("PreviewRawInput", RoutingStrategy.Tunnel,
			typeof(RawMultitouchReportHandler), typeof(MultitouchLogic));
		internal static readonly RoutedEvent RawInputEvent = EventManager.RegisterRoutedEvent("RawInput", RoutingStrategy.Bubble,
			typeof(RawMultitouchReportHandler), typeof(MultitouchLogic));

		InputManager inputManager;
		int doubleTapDeltaTime;
		internal MultitouchDeviceManager DeviceManager { get; private set; }

		public static MultitouchLogic Current
		{
			get
			{
				if (current == null)
				{
					lock (lockCurrent)
					{
						if(current == null)
							current = new MultitouchLogic(InputManager.Current);
					}
				}
				return current;
			}
		}

		public MultitouchLogic(InputManager inputManager)
		{
			doubleTapDeltaTime = 800;
            
			DeviceManager = new MultitouchDeviceManager();

			this.inputManager = inputManager;
			this.inputManager.PreProcessInput += inputManager_PreProcessInput;
			this.inputManager.PreNotifyInput += inputManager_PreNotifyInput;
			this.inputManager.PostProcessInput += inputManager_PostProcessInput;
		}

		void inputManager_PreProcessInput(object sender, PreProcessInputEventArgs e)
		{
			if (e.StagingItem.Input.RoutedEvent == PreviewRawInputEvent)
			{
				RawMultitouchReport report = e.StagingItem.Input as RawMultitouchReport;
				if (report != null && !report.Handled)
				{
					MultitouchDevice multitouchDevice = report.MultitouchDevice;

					if (report.Contact.State != ContactState.Removed)
						multitouchDevice.AllContacts[report.Contact.Id] = report.Contact;
					else
						multitouchDevice.AllContacts.Remove(report.Contact.Id);

					Point clientPoint = report.InputSource.RootVisual.PointFromScreen(new Point(report.Contact.X, report.Contact.Y));
                    HitTestResult test = VisualTreeHelper.HitTest(report.InputSource.RootVisual, clientPoint);
					if (test != null)
					{
						IInputElement hitTest = test.VisualHit as IInputElement;
						if (report.MultitouchDevice.Captured != hitTest)
							UpdateCapture((DependencyObject)hitTest, e);
						report.Contact.SetElement((UIElement)test.VisualHit);
					}
				}
			}
			Debug.Assert(e.StagingItem.Input.RoutedEvent != null, "routed event null");
		}

		void inputManager_PreNotifyInput(object sender, NotifyInputEventArgs e)
		{
			if (e.StagingItem.Input.RoutedEvent == PreviewRawInputEvent)
			{
				RawMultitouchReport report = e.StagingItem.Input as RawMultitouchReport;
				if (report != null && !report.Handled)
				{
					MultitouchDevice multitouchDevice = report.MultitouchDevice;
					multitouchDevice.UpdateState(report);

					Point position = multitouchDevice.GetPosition(null);
					IInputElement target = multitouchDevice.FindTarget(multitouchDevice.ActiveSource, position);
					multitouchDevice.ChangeOver(target);
				}
			}

			if (e.StagingItem.Input.RoutedEvent == MultitouchScreen.PreviewNewContactEvent)
			{
				NewContactEventArgs newContact = e.StagingItem.Input as NewContactEventArgs;
				if (newContact != null)
				{
					MultitouchDevice multitouchDevice = newContact.MultitouchDevice;
					Point position = multitouchDevice.GetPosition(null);
					Point lastTapPoint = multitouchDevice.LastTapPoint;

					int timeSpan = Math.Abs(newContact.Timestamp - multitouchDevice.LastTapTime);

					Size doubleTapSize = new Size(25, 25);
					bool isSameSpot = (Math.Abs(position.X - lastTapPoint.X) < doubleTapSize.Width) &&
					                  (Math.Abs(position.Y - lastTapPoint.Y) < doubleTapSize.Height);

					if (timeSpan < doubleTapDeltaTime && isSameSpot)
						multitouchDevice.TapCount++;
					else
					{
						multitouchDevice.TapCount = 1;
						multitouchDevice.LastTapPoint = new Point(position.X, position.Y);
						multitouchDevice.LastTapTime = newContact.Timestamp;
					}
				}
			}
			Debug.Assert(e.StagingItem.Input.RoutedEvent != null, "routed event null");
		}

		void inputManager_PostProcessInput(object sender, ProcessInputEventArgs e)
		{
			if (e.StagingItem.Input.RoutedEvent == RawInputEvent)
			{
				RawMultitouchReport report = e.StagingItem.Input as RawMultitouchReport;
				if (report != null && !report.Handled)
					PromoteRawToPreview(report, e);
			}
			if (e.StagingItem.Input.RoutedEvent == PreviewRawInputEvent)
			{
				RawMultitouchReport report = e.StagingItem.Input as RawMultitouchReport;
				if (report != null)
				{
					RawMultitouchReport args = new RawMultitouchReport(report);
					args.RoutedEvent = RawInputEvent;
					e.PushInput(args, e.StagingItem);
				}
			}
			PromotePreviewToMain(e);
		}

		void PromoteRawToPreview(RawMultitouchReport report, ProcessInputEventArgs e)
		{
			RoutedEvent routedEvent = GetPreviewEventFromRawMultitouchState(report.Contact.State);
			if(routedEvent != null)
			{
				ContactEventArgs args;
				if (routedEvent == MultitouchScreen.PreviewNewContactEvent)
					args = new NewContactEventArgs(report.MultitouchDevice, report, report.Timestamp);
				else
					args = new ContactEventArgs(report.MultitouchDevice, report, report.Timestamp);
				args.RoutedEvent = routedEvent;
				e.PushInput(args, e.StagingItem);
			}
		}

		RoutedEvent GetPreviewEventFromRawMultitouchState(ContactState state)
		{
			if (state == ContactState.New)
				return MultitouchScreen.PreviewNewContactEvent;
			if (state == ContactState.Moved)
				return MultitouchScreen.PreviewContactMovedEvent;
			if (state == ContactState.Removed)
				return MultitouchScreen.PreviewContactRemovedEvent;
			return null;
		}

		void PromotePreviewToMain(ProcessInputEventArgs e)
		{
			if(!e.StagingItem.Input.Handled)
			{
				RoutedEvent mainEvent = GetMainEventFromPreviewEvent(e.StagingItem.Input.RoutedEvent);
				if(mainEvent != null)
				{
					ContactEventArgs previewArgs = (ContactEventArgs)e.StagingItem.Input;
					MultitouchDevice multitouchDevice = previewArgs.MultitouchDevice;
					ContactEventArgs mainArgs;
					if (mainEvent == MultitouchScreen.NewContactEvent || mainEvent == MultitouchScreen.PreviewNewContactEvent)
					{
						NewContactEventArgs newContactEventArgs = (NewContactEventArgs)previewArgs;
						mainArgs = new NewContactEventArgs(multitouchDevice, newContactEventArgs.Contact, previewArgs.Timestamp);
					}
					else
						mainArgs = new ContactEventArgs(multitouchDevice, previewArgs.Contact, previewArgs.Timestamp);
					mainArgs.RoutedEvent = mainEvent;
					e.PushInput(mainArgs, e.StagingItem);
				}
			}
		}

		RoutedEvent GetMainEventFromPreviewEvent(RoutedEvent routedEvent)
		{
			if (routedEvent == MultitouchScreen.PreviewContactMovedEvent)
				return MultitouchScreen.ContactMovedEvent;
			if (routedEvent == MultitouchScreen.PreviewContactRemovedEvent)
				return MultitouchScreen.ContactRemovedEvent;
			if (routedEvent == MultitouchScreen.PreviewNewContactEvent)
				return MultitouchScreen.NewContactEvent;
			return null;
		}

		void UpdateCapture(DependencyObject o, PreProcessInputEventArgs e)
		{
			RawMultitouchReport report = (RawMultitouchReport)e.StagingItem.Input;

			ContactEventArgs args = new ContactEventArgs(report.MultitouchDevice, report, report.Timestamp);
			if(o == null && args.MultitouchDevice.Captured != null)
			{
				args.RoutedEvent = MultitouchScreen.ContactLeaveEvent;
				args.Source = args.MultitouchDevice.Captured;
				args.MultitouchDevice.Capture(null);
				e.StagingItem.Input.Handled = true;
				inputManager.ProcessInput(args);
			}
			if(o != null)
			{
				if(args.MultitouchDevice.Captured != null && args.MultitouchDevice.Captured != o)
				{
					args.RoutedEvent = MultitouchScreen.ContactLeaveEvent;
					args.Source = args.MultitouchDevice.Captured;
					args.MultitouchDevice.Capture(null);
					inputManager.ProcessInput(args);
				}
				if(args.MultitouchDevice.Captured == null)
				{
					args.MultitouchDevice.Capture((IInputElement)o);
					args.RoutedEvent = MultitouchScreen.ContactEnterEvent;
					args.Source = args.MultitouchDevice.Captured;
					inputManager.ProcessInput(args);
				}
			}
		}
	}
}