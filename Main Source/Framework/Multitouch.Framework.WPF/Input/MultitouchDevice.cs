using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Phydeaux.Utilities;

namespace Multitouch.Framework.WPF.Input
{
	public enum MatchCriteria
	{
		LogicalParent,
		Exact
	}

	class MultitouchDevice : InputDevice
	{
		Point rawPosition;
		IInputElement target;
		PresentationSource source;
		CaptureMode _captureMode;
		IInputElement _capture;
		StaticFunc<MouseDevice, IInputElement, Point, PresentationSource> globalHitTest;
		List<Point> positionHistory;

		internal Point LastTapPoint { get; set; }
		internal int TapCount { get; set; }
		internal int LastTapTime { get; set; }

		public override PresentationSource ActiveSource
		{
			get { return source; }
		}

		public override IInputElement Target
		{
			get { return target; }
		}

		public IInputElement Captured
		{
			get { return _capture; }
		}

		public IDictionary<int, Contact> AllContacts
		{
			get { return ContactsManager.Instance.AllContacts; }
		}

		internal MultitouchDevice()
		{
			globalHitTest = Dynamic<MouseDevice>.Static.Function<IInputElement>.Explicit<Point, PresentationSource>.CreateDelegate(
				"GlobalHitTest", new[] {typeof(Point), typeof(PresentationSource)});
			positionHistory = new List<Point>();
		}

		public Point GetPosition(IInputElement relativeTo)
		{
			if (relativeTo != null && !InputElement.IsValid(relativeTo))
				throw new InvalidOperationException();

			PresentationSource presentationSource = null;
			if(relativeTo != null)
			{
				DependencyObject o = relativeTo as DependencyObject;
				Visual containingVisual = InputElement.GetContainingVisual(o);
				if (containingVisual != null)
					presentationSource = PresentationSource.FromVisual(containingVisual);
			}
			else if(ActiveSource != null)
				presentationSource = ActiveSource;
			if (presentationSource == null || presentationSource.RootVisual == null)
				return new Point(0.0, 0.0);

			Point clientPoint = PointUtil.ScreenToClient(rawPosition, presentationSource);
			bool success;
			Point rootPoint = PointUtil.TryClientToRoot(clientPoint, presentationSource, false, out success);
			if (!success)
				return new Point(0.0, 0.0);

			return InputElement.TranslatePoint(rootPoint, presentationSource.RootVisual, (DependencyObject)relativeTo);
		}

		internal void UpdateState(RawMultitouchReport rawReport)
		{
			source = rawReport.InputSource;
			rawPosition = rawReport.Contact.Position;
			positionHistory.Add(rawPosition);
		}

		internal IInputElement FindTarget(PresentationSource inputSource, Point position)
		{
			IInputElement touchOver = null;
			switch (_captureMode)
			{
				case CaptureMode.None:
					{
						touchOver = GlobalHitTest(inputSource, position);
						if (!InputElement.IsValid(touchOver))
							touchOver = InputElement.GetContainingInputElement(touchOver as DependencyObject);
					}
					break;
				case CaptureMode.Element:
					touchOver = _capture;
					break;
				case CaptureMode.SubTree:
					{
						IInputElement capture = InputElement.GetContainingInputElement(_capture as DependencyObject);
						if (capture != null && inputSource != null)
							touchOver = GlobalHitTest(inputSource, position);

						if (touchOver != null && !InputElement.IsValid(touchOver))
							touchOver = InputElement.GetContainingInputElement(touchOver as DependencyObject);

						if (touchOver != null)
						{
							IInputElement inputElementTest = touchOver;
							UIElement uiElementTest;
							ContentElement contentElementTest;

							while (inputElementTest != null && inputElementTest != capture)
							{
								uiElementTest = inputElementTest as UIElement;
								if (uiElementTest != null)
									inputElementTest = InputElement.GetContainingInputElement(uiElementTest.GetUIParent(true));
								else
								{
									contentElementTest = inputElementTest as ContentElement;
									inputElementTest = InputElement.GetContainingInputElement(contentElementTest.GetUIParent(true));
								}
							}

							if (inputElementTest != capture)
								touchOver = _capture;
						}
						else
							touchOver = _capture;
					}
					break;
			}
			return touchOver;
		}

		internal IInputElement GlobalHitTest(PresentationSource inputSource, Point position)
		{
			return globalHitTest.Invoke(position, inputSource);
		}

		internal void ChangeOver(IInputElement newTarget, RawMultitouchReport report)
		{
			IInputElement oldTarget = target;
			if (Target != newTarget)
			{
				target = newTarget;

				if (oldTarget != null)
				{
					ContactEventArgs args = new ContactEventArgs(this, report, Environment.TickCount);
					args.RoutedEvent = MultitouchScreen.ContactLeaveEvent;
					args.Source = oldTarget;
					oldTarget.RaiseEvent(args);
				}
				if (target != null)
				{
					ContactEventArgs args = new ContactEventArgs(this, report, Environment.TickCount);
					args.RoutedEvent = MultitouchScreen.ContactEnterEvent;
					args.Source = target;
					target.RaiseEvent(args);
				}
			}
		}

		public void Capture(IInputElement element)
		{
			Capture(element, CaptureMode.Element);
		}

		public void Capture(IInputElement element, CaptureMode mode)
		{
			if(element == null)
				_captureMode = CaptureMode.None;
			if (mode == CaptureMode.None)
				_capture = null;
			DependencyObject o = element as DependencyObject;
			if (o != null && !InputElement.IsValid(element))
				throw new InvalidOperationException("invalid input element");

			bool success = false;

			if(element is UIElement)
			{
				UIElement e = element as UIElement;
				if (e.IsVisible && e.IsEnabled)
					success = true;
			}
			else if(element is ContentElement)
			{
				ContentElement ce = element as ContentElement;
				if (ce.IsEnabled)
					success = true;
			}
			else if(element is UIElement3D)
			{
				UIElement3D e3 = element as UIElement3D;
				if (e3.IsVisible && e3.IsEnabled)
					success = true;
			}

			if(success)
			{
				_capture = element;
				_captureMode = mode;
			}
		}

		public IDictionary<int, Contact> GetContacts(UIElement element, MatchCriteria criteria)
		{
			Dictionary<int, Contact> result;
			if(criteria == MatchCriteria.Exact)
			{
				result = (from contact in ContactsManager.Instance.AllContacts.Values
				          where element == contact.Element
				          select contact).ToDictionary(contact => contact.Id);
			}
			else if(criteria == MatchCriteria.LogicalParent)
			{
				result = (from contact in ContactsManager.Instance.AllContacts.Values
						  where IsParent(contact.Element, element)
						  select contact).ToDictionary(contact => contact.Id);
			}
			else
				result = new Dictionary<int, Contact>();
			return result;
		}

		bool IsParent(UIElement element, UIElement parent)
		{
			UIElement current = element;
			while (current != null && current != parent)
				current = VisualTreeHelper.GetParent(current) as UIElement;
			return current != null;
		}

		/// <summary>
		/// Returns all coordinates history of this contact
		/// </summary>
		public IList<Point> ContactHistory
		{
			get { return positionHistory.AsReadOnly(); }
		}
	}
}