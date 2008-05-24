using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Reflection;

namespace Multitouch.Framework.WPF.Input
{
	static class InputElement
	{
		static MethodInfo getContainingInputElementMethod;
		static MethodInfo getContainingVisualMethod;
		static MethodInfo getContainingUIElementMethod;
		static MethodInfo isValidMethod;
		static MethodInfo translatePointMethod;

		static InputElement()
		{
			Type inputElementType = typeof(InputManager).Assembly.GetType("System.Windows.Input.InputElement");
			getContainingInputElementMethod = inputElementType.GetMethod("GetContainingInputElement", BindingFlags.NonPublic | BindingFlags.Static,
				null, new[] { typeof(DependencyObject) }, null);
			getContainingVisualMethod = inputElementType.GetMethod("GetContainingVisual", BindingFlags.NonPublic | BindingFlags.Static,
				null, new[] { typeof(DependencyObject) }, null);
			getContainingUIElementMethod = inputElementType.GetMethod("GetContainingUIElement", BindingFlags.NonPublic | BindingFlags.Static,
				null, new[] { typeof(DependencyObject) }, null);
			isValidMethod = inputElementType.GetMethod("IsValid", BindingFlags.NonPublic | BindingFlags.Static, null,
				new[] { typeof(IInputElement) }, null);
			translatePointMethod = inputElementType.GetMethod("TranslatePoint", BindingFlags.NonPublic | BindingFlags.Static, null,
				new[] { typeof(Point), typeof(DependencyObject), typeof(DependencyObject) }, null);
		}

		public static IInputElement GetContainingInputElement(DependencyObject o)
		{
			return (IInputElement)getContainingInputElementMethod.Invoke(null, new object[] { o });
		}

		public static bool IsValid(IInputElement o)
		{
			return (bool)isValidMethod.Invoke(null, new[] { o });
		}

		public static Visual GetContainingVisual(DependencyObject o)
		{
			return (Visual)getContainingVisualMethod.Invoke(null, new object[] { o });
		}

		public static Point TranslatePoint(Point point, DependencyObject from, DependencyObject to)
		{
			return (Point)translatePointMethod.Invoke(null, new object[] { point, from, to });
		}

		public static UIElement GetContainingUIElement(DependencyObject target)
		{
			return (UIElement)getContainingUIElementMethod.Invoke(null, new object[] { target });
		}
	}
}