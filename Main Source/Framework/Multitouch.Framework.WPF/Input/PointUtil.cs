using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace Multitouch.Framework.WPF.Input
{
	static class PointUtil
	{
		static MethodInfo screenToClientMethod;
		static MethodInfo tryClientToRootMethod;

		static PointUtil()
		{
			Type pointUtilType = typeof(InputDevice).Assembly.GetType("MS.Internal.PointUtil");
			screenToClientMethod = pointUtilType.GetMethod("ScreenToClient", BindingFlags.NonPublic | BindingFlags.Static);
			tryClientToRootMethod = pointUtilType.GetMethod("TryClientToRoot", BindingFlags.Static | BindingFlags.Public);
		}

		public static Point ScreenToClient(Point point, PresentationSource presentationSource)
		{
			return (Point)screenToClientMethod.Invoke(null, new object[] { point, presentationSource });
		}

		public static Point TryClientToRoot(Point point, PresentationSource source, bool throwOnError, out bool success)
		{
			success = default(bool);
			object[] parameters = new object[] { point, source, throwOnError, success };
			Point result = (Point)tryClientToRootMethod.Invoke(null, parameters);
			success = (bool)parameters[3];
			return result;
		}
	}
}
