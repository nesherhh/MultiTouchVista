using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace Multitouch.Framework.WPF.Input
{
	static class InputElement
	{
		static MethodInfo getContainingInputElementMethod;
		static MethodInfo isValidMethod;

		static InputElement()
		{
			Type inputElementType = typeof(InputManager).Assembly.GetType("System.Windows.Input.InputElement");
			getContainingInputElementMethod = inputElementType.GetMethod("GetContainingInputElement", BindingFlags.NonPublic | BindingFlags.Static,
				null, new[] { typeof(DependencyObject) }, null);
			isValidMethod = inputElementType.GetMethod("IsValid", BindingFlags.NonPublic | BindingFlags.Static, null,
				new[] { typeof(IInputElement) }, null);
		}

		public static IInputElement GetContainingInputElement(DependencyObject o)
		{
			return (IInputElement)getContainingInputElementMethod.Invoke(null, new object[] { o });
		}

		public static bool IsValid(IInputElement o)
		{
			return (bool)isValidMethod.Invoke(null, new[] { o });
		}
	}
}