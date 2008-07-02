using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using AdvanceMath;
using Physics2DDotNet;
using Physics2DDotNet.Joints;
using Physics2DDotNet.Solvers;
using Scalar = System.Double;

namespace Multitouch.Framework.WPF.Physics
{
	/// <summary>
	/// A joint that makes a single Body Pivot around an Anchor.
	/// </summary>
	[Serializable]
	public sealed class FixedSlidingHingeJoint : Joint, ISequentialImpulsesJoint
	{
		SequentialImpulsesSolver solver;
		Body body;


		Matrix2x2 M;
		Vector2D localAnchor1;
		Vector2D anchor;
		Vector2D r1;
		Vector2D bias;
		Vector2D accumulatedImpulse;
		Scalar biasFactor;
		Scalar softness;
		Scalar distanceTolerance;


		/// <summary>
		/// Initializes a new instance of the <see cref="FixedSlidingHingeJoint"/> class.
		/// </summary>
		/// <param name="body">The body.</param>
		/// <param name="anchor">The anchor.</param>
		/// <param name="lifetime">The lifetime.</param>
		/// <param name="orientation">The orientation.</param>
		public FixedSlidingHingeJoint(Body body, Vector2D anchor, Lifespan lifetime, Orientation orientation)
			: base(lifetime)
		{
			if (body == null) { throw new ArgumentNullException("body"); }
			this.body = body;
			this.anchor = anchor;
			Orientation = orientation;
			body.ApplyPosition();
			Vector2D.Transform(ref body.Matrices.ToBody, ref anchor, out localAnchor1);
			softness = 0.001f;
			biasFactor = 0.2f;
			distanceTolerance = Scalar.PositiveInfinity;
		}

		Orientation Orientation { get; set; }

		/// <summary>
		/// Gets or sets the anchor.
		/// </summary>
		/// <value>The anchor.</value>
		public Vector2D Anchor
		{
			get { return anchor; }
			set { anchor = value; }
		}

		/// <summary>
		/// Gets or sets the bias factor.
		/// </summary>
		/// <value>The bias factor.</value>
		public Scalar BiasFactor
		{
			get { return biasFactor; }
			set { biasFactor = value; }
		}
		/// <summary>
		/// Gets or sets the softness.
		/// </summary>
		/// <value>The softness.</value>
		public Scalar Softness
		{
			get { return softness; }
			set { softness = value; }
		}
		/// <summary>
		/// The distance the joint can stretch before breaking. 
		/// </summary>
		public Scalar DistanceTolerance
		{
			get { return distanceTolerance; }
			set
			{
				if (value <= 0) { throw new ArgumentOutOfRangeException("value"); }
				distanceTolerance = value;
			}
		}
		/// <summary>
		/// Gets the bodies the Joint effects.
		/// </summary>
		/// <value></value>
		public override ReadOnlyCollection<Body> Bodies
		{
			get { return new ReadOnlyCollection<Body>(new[] { body }); }
		}
		/// <summary>
		/// Raises the <see cref="E:Added"/> event.
		/// </summary>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		protected override void OnAdded(EventArgs e)
		{
			solver = (SequentialImpulsesSolver)Engine.Solver;
			base.OnAdded(e);
		}
		void ISequentialImpulsesJoint.PreStep(TimeStep step)
		{

			Scalar mass1Inv = body.Mass.MassInv;
			Scalar inertia1Inv = body.Mass.MomentOfInertiaInv;

			// Pre-compute anchors, mass matrix, and bias.

			Vector2D.TransformNormal(ref body.Matrices.ToWorld, ref localAnchor1, out r1);

			// deltaV = deltaV0 + K * impulse
			// invM = [(1/m1 + 1/m2) * eye(2) - skew(r1) * invI1 * skew(r1) - skew(r2) * invI2 * skew(r2)]
			//      = [1/m1+1/m2     0    ] + invI1 * [r1.y*r1.y -r1.x*r1.y] + invI2 * [r1.y*r1.y -r1.x*r1.y]
			//        [    0     1/m1+1/m2]           [-r1.x*r1.y r1.x*r1.x]           [-r1.x*r1.y r1.x*r1.x]

			Matrix2x2 K;
			K.m00 = mass1Inv;
			K.m11 = mass1Inv;

			K.m00 += inertia1Inv * r1.Y * r1.Y;
			K.m01 = -inertia1Inv * r1.X * r1.Y;
			K.m10 = -inertia1Inv * r1.X * r1.Y;
			K.m11 += inertia1Inv * r1.X * r1.X;

			K.m00 += softness;
			K.m11 += softness;

			Matrix2x2.Invert(ref K, out M);


			Vector2D dp;
			Vector2D.Add(ref body.State.Position.Linear, ref r1, out dp);
			Vector2D.Subtract(ref anchor, ref dp, out dp);

			if (!Scalar.IsPositiveInfinity(distanceTolerance) &&
			    dp.MagnitudeSq > distanceTolerance * distanceTolerance)
			{
				Lifetime.IsExpired = true;
			}

			if (solver.PositionCorrection)
			{
				//bias = -0.1f * dtInv * dp;
				Scalar flt = -biasFactor * step.DtInv;
				Vector2D.Multiply(ref dp, ref flt, out bias);
			}
			else
			{
				bias = Vector2D.Zero;
			}
			if (solver.WarmStarting)
			{
				PhysicsHelper.SubtractImpulse(
					ref body.State.Velocity, ref accumulatedImpulse,
					ref r1, ref mass1Inv, ref inertia1Inv);
			}
			else
			{
				accumulatedImpulse = Vector2D.Zero;
			}
			body.ApplyProxy();

		}

		void ISequentialImpulsesJoint.ApplyImpulse()
		{
			Scalar mass1Inv = body.Mass.MassInv;
			Scalar inertia1Inv = body.Mass.MomentOfInertiaInv;

			Vector2D dv;
			PhysicsHelper.GetRelativeVelocity(ref body.State.Velocity, ref r1, out dv);


			Vector2D impulse = new Vector2D();
			if(Orientation == Orientation.Horizontal)
				impulse.X = bias.X - dv.X - softness * accumulatedImpulse.X;
			if(Orientation == Orientation.Vertical)
				impulse.Y = bias.Y - dv.Y - softness * accumulatedImpulse.Y;
			Vector2D.Transform(ref  M, ref impulse, out impulse);
			//impulse = M * (bias - dv - softness * P);


			PhysicsHelper.SubtractImpulse(
				ref body.State.Velocity, ref impulse,
				ref r1, ref mass1Inv, ref inertia1Inv);


			Vector2D.Add(ref accumulatedImpulse, ref impulse, out accumulatedImpulse);
			body.ApplyProxy();
		}
	}
}