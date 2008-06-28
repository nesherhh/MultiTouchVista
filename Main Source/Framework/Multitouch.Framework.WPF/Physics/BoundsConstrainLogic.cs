using System;
using AdvanceMath;
using Multitouch.Framework.WPF.Controls;
using Physics2DDotNet;
using Physics2DDotNet.PhysicsLogics;

namespace Multitouch.Framework.WPF.Physics
{
	class BoundsConstrainLogic : PhysicsLogic
	{
		readonly ScrollViewer viewer;

		public BoundsConstrainLogic(ScrollViewer viewer)
			: base(new Lifespan())
		{
			this.viewer = viewer;
		}

		protected override void RunLogic(TimeStep step)
		{
			Vector2D linear = -viewer.Body.State.Position.Linear;
			Vector2D velocity = -viewer.Body.State.Velocity.Linear;
			if (linear.X < 0)
			{
				linear.X = 0;
				velocity.X = 0;
			}
			else if (viewer.ScrollableWidth > 0 && linear.X > viewer.ScrollableWidth)
			{
				linear.X = viewer.ScrollableWidth;
				velocity.X = 0;
			}
			if (linear.Y < 0)
			{
				linear.Y = 0;
				velocity.Y = 0;
			}
			else if (viewer.ScrollableHeight > 0 && linear.Y > viewer.ScrollableHeight)
			{
				linear.Y = viewer.ScrollableHeight;
				velocity.Y = 0;
			}
			viewer.Body.State.Position.Linear = -linear;
			viewer.Body.State.Velocity.Linear = -velocity;
		}
	}
}