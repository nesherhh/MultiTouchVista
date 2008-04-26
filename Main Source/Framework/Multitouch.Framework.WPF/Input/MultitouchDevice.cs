using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace Multitouch.Framework.WPF.Input
{
	public class MultitouchDevice : InputDevice
	{
		Point rawPosition;
		IInputElement target;
		PresentationSource source;
		CaptureMode captureMode;
		IInputElement touchCapture;
		MethodInfo globalHitTestMethod;

		public MultitouchDevice()
		{
			Type mouseDeviceType = typeof(MouseDevice);
			globalHitTestMethod = mouseDeviceType.GetMethod("GlobalHitTest", BindingFlags.Static | BindingFlags.NonPublic,
				null, new[] { typeof(Point), typeof(PresentationSource) }, null);
		}

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
			get { return touchCapture; }
		}

		internal Point LastTapPoint { get; set; }
		internal int TapCount { get; set; }
		internal int LastTapTime { get; set; }

		public Point GetPosition(IInputElement relativeTo)
		{
			if (relativeTo != null)
				throw new NotImplementedException();

			return source.RootVisual.PointFromScreen(rawPosition);
		}

		internal void UpdateState(RawMultitouchReport rawReport)
		{
			source = rawReport.InputSource;
			rawPosition = new Point(rawReport.X, rawReport.Y);
		}

		internal IInputElement FindTarget(PresentationSource inputSource, Point position)
		{
			IInputElement touchOver = null;
			switch (captureMode)
			{
				case CaptureMode.None:
					{
						touchOver = GlobalHitTest(inputSource, position);
						if (!InputElement.IsValid(touchOver))
							touchOver = InputElement.GetContainingInputElement(touchOver as DependencyObject);
					}
					break;
				case CaptureMode.Element:
					touchOver = touchCapture;
					break;
				case CaptureMode.SubTree:
					{
						IInputElement capture = InputElement.GetContainingInputElement(touchCapture as DependencyObject);
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
								touchOver = touchCapture;
						}
						else
							touchOver = touchCapture;
					}
					break;
			}
			return touchOver;
		}

		internal IInputElement GlobalHitTest(PresentationSource inputSource, Point position)
		{
			return (IInputElement)globalHitTestMethod.Invoke(null, new object[] { position, inputSource });
		}

		public void ChangeOver(IInputElement newTarget)
		{
			if (Target != newTarget)
				target = newTarget;
		}

		public void Capture(IInputElement element)
		{
			Capture(element, CaptureMode.Element);
		}

		public void Capture(IInputElement element, CaptureMode mode)
		{
			if(element == null)
				captureMode = CaptureMode.None;
			if (mode == CaptureMode.None)
				element = null;
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
				touchCapture = element;
				captureMode = mode;
			}
		}
	}
}