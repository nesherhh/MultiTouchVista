using System;
using System.Collections.Generic;
using System.Linq;
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
		static readonly object lockCurrent = new object();

		internal static readonly RoutedEvent PreviewRawInputEvent = EventManager.RegisterRoutedEvent("PreviewRawInput", RoutingStrategy.Tunnel,
			typeof(RawMultitouchReportHandler), typeof(MultitouchLogic));
		internal ContactsManager ContactsManager { get; private set; }

		readonly InputManager inputManager;
		readonly Type contactType;
		readonly IEnumerable<RoutedEvent> contactEvents;

		MultitouchLogic(InputManager inputManager)
		{
			if (!MouseHelper.SingleMouseFallback)
				Mouse.OverrideCursor = Cursors.None;

			ContactsManager = new ContactsManager(inputManager.Dispatcher);

			this.inputManager = inputManager;
			this.inputManager.PreProcessInput += inputManager_PreProcessInput;
			this.inputManager.PreNotifyInput += inputManager_PreNotifyInput;
			this.inputManager.PostProcessInput += inputManager_PostProcessInput;
			contactType = typeof(Contact);

			Dispatcher.ShutdownFinished += Dispatcher_ShutdownFinished;

			Type routedEventType = typeof(RoutedEvent);
			contactEvents = typeof(MultitouchScreen).GetFields().Where(m => m.FieldType == routedEventType).Select(f => (RoutedEvent)f.GetValue(null));
		}

		static void Dispatcher_ShutdownFinished(object sender, EventArgs e)
		{
			current = null;
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
					switch (report.Context.Contact.State)
					{
						case ContactState.New:
							return;
						case ContactState.Removed:
						case ContactState.Moved:
							{
								Contact contact;
								if (!ContactsManager.ExistingContacts.TryGetValue(report.Context.Contact.Id, out contact))
									break;
								report.UpdateFromPrevious(contact.InputArgs);
								return;
							}
						default:
							return;
					}
					report.Handled = true;
				}
			}
			else if (!MultitouchScreen.AllowNonContactEvents)
			{
				if (!IsContactEvent(routedEvent))
				{
					e.StagingItem.Input.Handled = true;
					e.Cancel();
				}
			}
			Debug.Assert(routedEvent != null, "routed event null");
		}

		bool IsContactEvent(RoutedEvent routedEvent)
		{
			return contactEvents.Contains(routedEvent);
		}

		void inputManager_PreNotifyInput(object sender, NotifyInputEventArgs e)
		{
			if (e.StagingItem.Input.RoutedEvent == PreviewRawInputEvent)
			{
				RawMultitouchReport report = e.StagingItem.Input as RawMultitouchReport;
				if (report != null && !report.Handled)
				{
					report.Context.OverElement = GetTargetElement(report.Context.Contact.Position, report.Context.Root, report);

					Contact contact;
					if (!report.CleanUp)
					{
						if (ContactsManager.ExistingContacts.TryGetValue(report.Context.Contact.Id, out contact))
							contact.InputArgs = report;
						else
						{
							contact = new Contact(report);
							ContactsManager.ExistingContacts[report.Context.Contact.Id] = contact;
						}
					}
					else
					{
						contact = ContactsManager.ExistingContacts[report.Context.Contact.Id];
						contact.InputArgs = report;
						report.Handled = true;
					}

					UIElementsList mergedList;

					report.Context.ElementsList = BuildElementsList(report, report.Context.OverElement, report.Context.ElementsList, out mergedList);
					if(mergedList != null)
						RaiseEnterLeave(contact, mergedList);

					if(report.CleanUp)
					{
						if (report.Captured != null)
							contact.Capture(null);
						ContactsManager.ExistingContacts.Remove(contact.Id);
					}
				}
			}
			Debug.Assert(e.StagingItem.Input.RoutedEvent != null, "routed event null");
		}

		static UIElementsList BuildElementsList(RawMultitouchReport report, DependencyObject newOver, UIElementsList oldList, out UIElementsList mergedList)
		{
			UIElementsList newList = null;
			if (newOver != null)
			{
				newList = new UIElementsList(newOver);
				if (!report.CleanUp)
					BuildElementsList(newList, newOver);
			}
			mergedList = UIElementsList.BuildDifference(newList, oldList);
			return newList;
		}

		static void BuildElementsList(UIElementsList list, DependencyObject over)
		{
			if(!list.Contains(over))
			{
				list.Add(over, ContactState.New);

				DependencyObject visualParent = over is Visual ? VisualTreeHelper.GetParent(over) : null;
				DependencyObject logicalParent = LogicalTreeHelper.GetParent(over);
				if(visualParent != null)
					BuildElementsList(list, visualParent);
				if(logicalParent != null && logicalParent != visualParent)
					BuildElementsList(list, logicalParent);
			}
		}

		static void RaiseEnterLeave(Contact contact, UIElementsList list)
		{
			foreach (DependencyObject treeElement in list)
			{
				ContactState state = list.GetState(treeElement);
				switch (state)
				{
					case ContactState.New:
						RaiseEnterLeaveEvents(contact, treeElement, MultitouchScreen.ContactEnterEvent);
						break;
					case ContactState.Removed:
						RaiseEnterLeaveEvents(contact, treeElement, MultitouchScreen.ContactLeaveEvent);
						break;
				}
			}
		}

		static void RaiseEnterLeaveEvents(Contact contact, DependencyObject element, RoutedEvent routedEvent)
		{
			ContactEventArgs e = new ContactEventArgs(contact, Environment.TickCount);
			e.RoutedEvent = routedEvent;
			e.Source = element;
			RaiseEvent(element, e);
		}

		internal static void RaiseEvent(DependencyObject source, RoutedEventArgs args)
		{
			UIElement element = source as UIElement;
			if (element != null)
				element.RaiseEvent(args);
			else
			{
				ContentElement contentElement = source as ContentElement;
				if (contentElement != null)
					contentElement.RaiseEvent(args);
			}
		}

		void inputManager_PostProcessInput(object sender, ProcessInputEventArgs e)
		{
			InputEventArgs input = e.StagingItem.Input;
			if (input.RoutedEvent == PreviewRawInputEvent && !input.Handled)
				PromoteRawToPreview(e, (RawMultitouchReport)input);
			else if (input.Device != null && input.Device.GetType() == contactType)
			{
				if (!input.Handled)
					PromotePreviewToMain(e, input);
				
				if (input.RoutedEvent == MultitouchScreen.ContactRemovedEvent)
				{
					ContactEventArgs args = (ContactEventArgs)input;
					RawMultitouchReport report = args.Contact.InputArgs.Clone();
					report.CleanUp = true;
					e.PushInput(report, e.StagingItem);
				}
			}
		}

		UIElement GetTargetElement(Point position, UIElement root, RawMultitouchReport report)
		{
			DependencyObject captured;
			if (report.CaptureState == CaptureMode.Element)
				captured = report.Captured;
			else
			{
				captured = HitTest(root, position);
				if (captured != null && report.Captured != null && report.CaptureState == CaptureMode.SubTree)
				{
					if (!IsPartOfSubTree(captured, report.Captured))
						captured = report.Captured;
				}
			}
			if (captured != null)
			{
				DependencyObject currentObject = captured;
				UIElement target = currentObject as UIElement;
				while (target == null)
				{
					currentObject = LogicalTreeHelper.GetParent(currentObject);
					target = currentObject as UIElement;
				}
				return target;
			}
			return null;
		}

		static DependencyObject HitTest(UIElement root, Point position)
		{
			return root.InputHitTest(position) as DependencyObject;
		}

		bool IsPartOfSubTree(DependencyObject child, DependencyObject parent)
		{
			if (!IsPartOfSubTree(child, parent, VisualTreeHelper.GetParent))
				return IsPartOfSubTree(child, parent, LogicalTreeHelper.GetParent);
			return true;
		}

		bool IsPartOfSubTree(DependencyObject child, DependencyObject parent, Func<DependencyObject, DependencyObject> getParent)
		{
			DependencyObject childsParent = getParent(child);
			if (childsParent != null && (childsParent == parent || IsPartOfSubTree(childsParent, parent)))
				return true;
			return false;
		}

		void PromoteRawToPreview(ProcessInputEventArgs e, RawMultitouchReport report)
		{
			ContactContext context = report.Context;
			RoutedEvent routedEvent = GetPreviewEventFromRawMultitouchState(context.Contact.State);
			if (report.Context.OverElement != null && routedEvent != null)
			{
				Contact contact = ContactsManager.ExistingContacts[report.Context.Contact.Id];
				ContactEventArgs args;
				if (routedEvent == MultitouchScreen.PreviewNewContactEvent)
					args = new NewContactEventArgs(contact, report.Timestamp);
				else
					args = new ContactEventArgs(contact, report.Timestamp);
				args.RoutedEvent = routedEvent;
				args.Source = report.Context.OverElement;
				e.PushInput(args, e.StagingItem);
			}
		}

		static RoutedEvent GetPreviewEventFromRawMultitouchState(ContactState state)
		{
			if (state == ContactState.New)
				return MultitouchScreen.PreviewNewContactEvent;
			if (state == ContactState.Moved)
				return MultitouchScreen.PreviewContactMovedEvent;
			if (state == ContactState.Removed)
				return MultitouchScreen.PreviewContactRemovedEvent;
			return null;
		}

		static void PromotePreviewToMain(ProcessInputEventArgs e, InputEventArgs input)
		{
			ContactEventArgs previewArgs = (ContactEventArgs)input;
			RoutedEvent mainEvent = GetMainEventFromPreviewEvent(input.RoutedEvent);
			if (mainEvent != null)
			{
				ContactEventArgs mainArgs;
				if (mainEvent == MultitouchScreen.NewContactEvent || mainEvent == MultitouchScreen.PreviewNewContactEvent)
				{
					NewContactEventArgs newContactEventArgs = (NewContactEventArgs)previewArgs;
					mainArgs = new NewContactEventArgs(newContactEventArgs.Contact, previewArgs.Timestamp);
				}
				else
					mainArgs = new ContactEventArgs(previewArgs.Contact, previewArgs.Timestamp);
				mainArgs.RoutedEvent = mainEvent;
				mainArgs.Source = previewArgs.Source;
				e.PushInput(mainArgs, e.StagingItem);
			}
		}

		static RoutedEvent GetMainEventFromPreviewEvent(RoutedEvent routedEvent)
		{
			if (routedEvent == MultitouchScreen.PreviewContactMovedEvent)
				return MultitouchScreen.ContactMovedEvent;
			if (routedEvent == MultitouchScreen.PreviewContactRemovedEvent)
				return MultitouchScreen.ContactRemovedEvent;
			if (routedEvent == MultitouchScreen.PreviewNewContactEvent)
				return MultitouchScreen.NewContactEvent;
			return null;
		}
	}
}