using System;
using AdvanceMath;
using Physics2DDotNet;
using Physics2DDotNet.Joints;

namespace Danilins.Multitouch.PhysicsLibrary
{
	public class CustomFixedHingeJoint : FixedHingeJoint
	{
		public CustomFixedHingeJoint(Body body, Vector2D anchor, Lifespan lifetime)
			: base(body, anchor, lifetime)
		{ }

		public override Vector2D Anchor
		{
			get { return base.Anchor; }
			set
			{
				PreviousAnchor = base.Anchor;
				base.Anchor = value;
			}
		}

		public Vector2D PreviousAnchor { get; private set; }

		protected override void OnAdded()
		{
			foreach (Body body in Bodies)
			{
				if (body is CustomBody)
					((CustomBody)body).joints.Add(this);
			}
			base.OnAdded();
		}

		protected override void OnRemoved(PhysicsEngine engine, bool wasPending)
		{
			foreach (Body body in Bodies)
			{
				if (body is CustomBody)
					((CustomBody)body).joints.Remove(this);
			}
			base.OnRemoved(engine, wasPending);
		}
	}
}