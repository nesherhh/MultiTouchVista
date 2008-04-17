using System;
using System.Diagnostics;
using System.Windows;

namespace Danilins.Multitouch.Logic.LegacySupport
{
	static class ContactActionHelper
	{
		public static void LeftTap(Point location)
		{
			MouseEmulation.Action(location.X, location.Y, MouseEmulation.MouseAction.LeftDown);
			MouseEmulation.Action(location.X, location.Y, MouseEmulation.MouseAction.LeftUp);
			Trace.WriteLine("LeftDown");
			Trace.WriteLine("LeftUp");
			Trace.WriteLine("---------------------");
		}

		public static void RightTap(Point location)
		{
			MouseEmulation.Action(location.X, location.Y, MouseEmulation.MouseAction.RightDown);
			MouseEmulation.Action(location.X, location.Y, MouseEmulation.MouseAction.RightUp);
			Trace.WriteLine("RightDown");
			Trace.WriteLine("RightUp");
			Trace.WriteLine("---------------------");
		}

		public static void LeftDown(Point location)
		{
			MouseEmulation.Action(location.X, location.Y, MouseEmulation.MouseAction.LeftDown);
			Trace.WriteLine("LeftDown");
		}

		public static void LeftUp(Point location)
		{
			MouseEmulation.Action(location.X, location.Y, MouseEmulation.MouseAction.LeftUp);
			Trace.WriteLine("LeftUp");
			Trace.WriteLine("---------------------");
		}

		public static void Drag(Point location)
		{
			MouseEmulation.Action(location.X, location.Y, MouseEmulation.MouseAction.Move);
			Trace.WriteLine("Move");
		}

		public static void RightDown(Point location)
		{
			MouseEmulation.Action(location.X, location.Y, MouseEmulation.MouseAction.RightDown);
			Trace.WriteLine("RightDown");
		}

		public static void RightUp(Point location)
		{
			MouseEmulation.Action(location.X, location.Y, MouseEmulation.MouseAction.RightUp);
			Trace.WriteLine("RightUp");
			Trace.WriteLine("---------------------");
		}
	}
}
