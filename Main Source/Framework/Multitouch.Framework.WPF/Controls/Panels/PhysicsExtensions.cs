using System;
using System.Windows;
using AdvanceMath;

namespace Multitouch.Framework.WPF.Controls
{
	static class PhysicsExtensions
	{
		public static Vector2D ToVector2D(this Point item)
		{
			return new Vector2D(item.X, item.Y);
		}
	}
}