using System;
using System.Reflection;
using System.Windows;

namespace Multitouch.Framework.WPF.Input
{
	static class Extensions
	{
		static MethodInfo uiElement_GetUIParentMathod;
		static MethodInfo contentElement_GetUIParentMethod;

		static Extensions()
		{
			Type uiElementType = typeof(UIElement);
			uiElement_GetUIParentMathod = uiElementType.GetMethod("GetUIParent", new[] { typeof(bool) });
			Type contentElementType = typeof(ContentElement);
			contentElement_GetUIParentMethod = contentElementType.GetMethod("GetUIParent", new[] { typeof(bool) });
		}

		internal static DependencyObject GetUIParent(this UIElement item, bool continuePastVisualTree)
		{
			return (DependencyObject)uiElement_GetUIParentMathod.Invoke(item, new object[] { continuePastVisualTree });
		}

		internal static DependencyObject GetUIParent(this ContentElement item, bool continuePastVisualTree)
		{
			return (DependencyObject)contentElement_GetUIParentMethod.Invoke(item, new object[] { continuePastVisualTree });
		}

		public static bool IsEmpty(this Point point)
		{
			return point.X == 0 && point.Y == 0;
		}
	}
}
