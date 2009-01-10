using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Input.StylusPlugIns;
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

		object stylusLogic;
		Type stylusLogicType;
		MethodInfo getPenContextsFromHwndMethod;
		PropertyInfo getMousePointDescriptionProperty;

		Assembly presentationCoreAssembly;
		Type rawStylusInputReportType;
		Type rawStylusActionsType;
		ConstructorInfo constructor;

		Type penContextsType;
		MethodInfo invokeStylusPluginCollectionForMouseMethod;
		MethodInfo verifyStylusPlugInCollectionTargetMethod;

		InputManager inputManager;
		int doubleTapDeltaTime;
		StylusPlugInCollection activeMousePluginCollection;
		static StylusPointDescription mousePointerDescription;
		object stylusActionDown;
		object stylusActionMove;
		object stylusActionUp;


		internal GestureManager GestureManager { get; private set; }
		internal MultitouchDeviceManager DeviceManager { get; private set; }

		public MultitouchLogic(InputManager inputManager)
		{
			Mouse.OverrideCursor = Cursors.None;

            presentationCoreAssembly = typeof(InputManager).Assembly;
			rawStylusInputReportType = presentationCoreAssembly.GetType("System.Windows.Input.RawStylusInputReport");
			rawStylusActionsType = presentationCoreAssembly.GetType("System.Windows.Input.RawStylusActions");
			penContextsType = presentationCoreAssembly.GetType("System.Windows.Input.PenContexts");
			stylusLogicType = presentationCoreAssembly.GetType("System.Windows.Input.StylusLogic");
			constructor = rawStylusInputReportType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null,
			                                                      new[]
			                                                      	{
			                                                      		typeof(InputMode), typeof(int), typeof(PresentationSource),
			                                                      		rawStylusActionsType, typeof(StylusPointDescription), typeof(int[])
			                                                      	}, null);

			FieldInfo field = rawStylusActionsType.GetField("Down");
			stylusActionDown = field.GetValue(null);
			field = rawStylusActionsType.GetField("Move");
			stylusActionMove = field.GetValue(null);
			field = rawStylusActionsType.GetField("Up");
			stylusActionUp = field.GetValue(null);

			invokeStylusPluginCollectionForMouseMethod = penContextsType.GetMethod("InvokeStylusPluginCollectionForMouse", BindingFlags.NonPublic | BindingFlags.Instance);
			getPenContextsFromHwndMethod = stylusLogicType.GetMethod("GetPenContextsFromHwnd", BindingFlags.NonPublic | BindingFlags.Instance);
			stylusLogic = typeof(InputManager).GetProperty("StylusLogic", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(InputManager.Current, null);
			getMousePointDescriptionProperty = stylusLogicType.GetProperty("GetMousePointDescription", BindingFlags.Instance | BindingFlags.NonPublic);
			verifyStylusPlugInCollectionTargetMethod = stylusLogicType.GetMethod("VerifyStylusPlugInCollectionTarget", BindingFlags.Instance | BindingFlags.NonPublic);


			doubleTapDeltaTime = 800;

			DeviceManager = new MultitouchDeviceManager();
			GestureManager = new GestureManager();

			this.inputManager = inputManager;
			this.inputManager.PreProcessInput += inputManager_PreProcessInput;
			this.inputManager.PreNotifyInput += inputManager_PreNotifyInput;
			this.inputManager.PostProcessInput += inputManager_PostProcessInput;
		}

		public static MultitouchLogic Current
		{
			get
			{
				if (current == null)
				{
					lock (lockCurrent)
					{
						if (current == null)
							current = new MultitouchLogic(InputManager.Current);
					}
				}
				return current;
			}
		}


		void inputManager_PreProcessInput(object sender, PreProcessInputEventArgs e)
		{
			RoutedEvent routedEvent = e.StagingItem.Input.RoutedEvent;
			if (routedEvent == PreviewRawInputEvent)
			{
				RawMultitouchReport report = e.StagingItem.Input as RawMultitouchReport;
				if (report != null && !report.Handled)
				{
					MultitouchDevice multitouchDevice = report.MultitouchDevice;

					if (report.Contact.State != ContactState.Removed)
						multitouchDevice.AllContacts[report.Contact.Id] = report.Contact;
					else
						multitouchDevice.AllContacts.Remove(report.Contact.Id);

					if (GestureManager.IsGestureEnabled)
						GestureManager.PreProcessInput(e);

					Point clientPoint = report.InputSource.RootVisual.PointFromScreen(report.Contact.Position);
					HitTestResult test = VisualTreeHelper.HitTest(report.InputSource.RootVisual, clientPoint);
					if (test != null)
					{
						IInputElement hitTest = test.VisualHit as IInputElement;
						report.Source = hitTest;
					}
				}
			}
			else if (!MultitouchScreen.AllowMouseEvents)
			{
				if (!(routedEvent == RawInputEvent ||
				      routedEvent == MultitouchScreen.ContactEnterEvent || routedEvent == MultitouchScreen.ContactLeaveEvent ||
				      routedEvent == MultitouchScreen.ContactMovedEvent || routedEvent == MultitouchScreen.ContactRemovedEvent ||
				      routedEvent == MultitouchScreen.NewContactEvent || routedEvent == MultitouchScreen.PreviewContactMovedEvent ||
				      routedEvent == MultitouchScreen.PreviewContactRemovedEvent || routedEvent == MultitouchScreen.PreviewNewContactEvent ||
				      routedEvent == GestureManager.RawGestureInputEvent || routedEvent == MultitouchScreen.PreviewGestureEvent ||
				      routedEvent == MultitouchScreen.GestureEvent))
				{
					e.StagingItem.Input.Handled = true;
					e.Cancel();
				}
			}
			Debug.Assert(routedEvent != null, "routed event null");
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
					multitouchDevice.ChangeOver(target, report);

					Visual visual = (Visual)report.MultitouchDevice.Target;
					if (visual != null)
					{
						PresentationSource source = PresentationSource.FromVisual(visual);
						if (source != null)
						{
							object rawStylusInputReport = CreateRawStylusInputReport(e, report, source);
							verifyStylusPlugInCollectionTargetMethod.Invoke(stylusLogic, new[] {rawStylusInputReport});
						}
					}
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

			if (GestureManager.IsGestureEnabled)
				GestureManager.PostProcessInput(e);

			CallPlugInsForMultitouch(e);

			PromotePreviewToMain(e);
		}

		void PromoteRawToPreview(RawMultitouchReport report, ProcessInputEventArgs e)
		{
			RoutedEvent routedEvent = GetPreviewEventFromRawMultitouchState(report.Contact.State);
			if (routedEvent != null)
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
			if (!e.StagingItem.Input.Handled)
			{
				RoutedEvent mainEvent = GetMainEventFromPreviewEvent(e.StagingItem.Input.RoutedEvent);
				if (mainEvent != null)
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

		void CallPlugInsForMultitouch(ProcessInputEventArgs e)
		{
			ContactEventArgs args = e.StagingItem.Input as ContactEventArgs;
			if (args != null && args.MultitouchDevice.Target != null)
			{
				PresentationSource source = PresentationSource.FromVisual((Visual)args.MultitouchDevice.Target);
				if (source != null)
				{
					object rawStylusInputReport = CreateRawStylusInputReport(e, args, source);

					using (Dispatcher.DisableProcessing())
					{
						object penContexts = getPenContextsFromHwndMethod.Invoke(stylusLogic, new object[] {source});
						activeMousePluginCollection = (StylusPlugInCollection)invokeStylusPluginCollectionForMouseMethod.Invoke(
						                                                      	penContexts,
						                                                      	new[] {rawStylusInputReport, args.MultitouchDevice.Target, activeMousePluginCollection});
					}
				}
			}
		}

		object CreateRawStylusInputReport(NotifyInputEventArgs e, ContactEventArgs args, PresentationSource source)
		{
			return CreateRawStylusInputReport(e, element => args.GetPosition(element), args.Timestamp, source);
		}

		object CreateRawStylusInputReport(NotifyInputEventArgs e, RawMultitouchReport report, PresentationSource source)
		{
			return CreateRawStylusInputReport(e, element => report.GetPosition(element), report.Timestamp, source);
		}

		object CreateRawStylusInputReport(NotifyInputEventArgs e, Func<IInputElement, Point> getPosition, int timestamp, PresentationSource source)
		{
			object stylusActions = null;
			if (e.StagingItem.Input.RoutedEvent == MultitouchScreen.PreviewNewContactEvent)
				stylusActions = stylusActionDown;
			else if (e.StagingItem.Input.RoutedEvent == MultitouchScreen.PreviewContactMovedEvent)
				stylusActions = stylusActionMove;
			else if (e.StagingItem.Input.RoutedEvent == MultitouchScreen.PreviewContactRemovedEvent)
				stylusActions = stylusActionUp;

			Point ptClient = getPosition(source.RootVisual as IInputElement);
			ptClient = source.CompositionTarget.TransformToDevice.Transform(ptClient);
			int[] data = new[] {(int)ptClient.X, (int)ptClient.Y, 1, 1};

			return constructor.Invoke(new[]
			                          	{
			                          		InputMode.Foreground, timestamp, source,
			                          		stylusActions, GetMousePointerDescription, data
			                          	});
		}

		internal StylusPointDescription GetMousePointerDescription
		{
			get
			{
				if(mousePointerDescription == null)
					mousePointerDescription = (StylusPointDescription)getMousePointDescriptionProperty.GetValue(stylusLogic, null);
				return mousePointerDescription;
			}
		}
	}
}