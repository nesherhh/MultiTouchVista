#region MIT License
/*
 * Copyright (c) 2005-2007 Jonathan Mark Porter. http://physics2d.googlepages.com/
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to deal 
 * in the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of 
 * the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be 
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
 * PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE 
 * LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 */
#endregion



// because this code was basically copied from Box2D
// Copyright (c) 2006 Erin Catto http://www.gphysics.com
#if UseDouble
using Scalar = System.Double;
#else
using Scalar = System.Single;
#endif
using System;
using System.Collections.ObjectModel;

using AdvanceMath;






namespace Physics2DDotNet.Joints
{
    /// <summary>
    /// A Joint Between 2 Bodies that will pivot around an Anchor.
    /// </summary>
    [Serializable]
    public sealed class HingeJoint : Joint, Solvers.ISequentialImpulsesJoint
    {
        Solvers.SequentialImpulsesSolver solver;
        Body body1;
        Body body2;
        Matrix2x2 M;
        Vector2D localAnchor1, localAnchor2;
        Vector2D r1, r2;
        Vector2D bias;
        Vector2D accumulatedImpulse;
        Scalar biasFactor;
        Scalar softness;
        Scalar distanceTolerance;



        /// <summary>
        /// Creates a new HingeJoint Instance.
        /// </summary>
        /// <param name="body1">One of the bodies to be Jointed.</param>
        /// <param name="body2">One of the bodies to be Jointed.</param>
        /// <param name="anchor">The location of the Hinge.</param>
        /// <param name="lifeTime">A object Describing how long the object will be in the engine.</param>
        public HingeJoint(Body body1, Body body2, Vector2D anchor, Lifespan lifetime)
            : base(lifetime)
        {
            if (body1 == null) { throw new ArgumentNullException("body1"); }
            if (body2 == null) { throw new ArgumentNullException("body2"); }
            if (body1 == body2) { throw new ArgumentException("You cannot add a joint to a body to itself"); }
            this.body1 = body1;
            this.body2 = body2;
            body1.ApplyPosition();
            body2.ApplyPosition();

            Vector2D.Transform(ref body1.Matrices.ToBody, ref anchor, out  this.localAnchor1);
            Vector2D.Transform(ref body2.Matrices.ToBody, ref anchor, out  this.localAnchor2);

            this.softness = 0.001f;
            this.biasFactor = 0.2f;
            this.distanceTolerance = Scalar.PositiveInfinity;
        }

        public Scalar BiasFactor
        {
            get { return biasFactor; }
            set { biasFactor = value; }
        }
        public Scalar Softness
        {
            get { return softness; }
            set { softness = value; }
        }
        /// <summary>
        /// The Distance the joint can stretch before breaking. 
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
        public override ReadOnlyCollection<Body> Bodies
        {
            get { return new ReadOnlyCollection<Body>(new Body[2] { body1, body2 }); }
        }
        protected override void OnAdded()
        {
            this.solver = (Solvers.SequentialImpulsesSolver)Engine.Solver;
        }
        void Solvers.ISequentialImpulsesJoint.PreStep(TimeStep step)
        {


            Scalar mass1Inv = body1.Mass.MassInv;
            Scalar mass2Inv = body2.Mass.MassInv;
            Scalar inertia1Inv = body1.Mass.MomentOfInertiaInv;
            Scalar inertia2Inv = body2.Mass.MomentOfInertiaInv;

            // Pre-compute anchors, mass matrix, and bias.

            Vector2D.TransformNormal(ref body1.Matrices.ToWorld, ref localAnchor1, out r1);
            Vector2D.TransformNormal(ref body2.Matrices.ToWorld, ref localAnchor2, out r2);

            // deltaV = deltaV0 + K * impulse
            // invM = [(1/m1 + 1/m2) * eye(2) - skew(r1) * invI1 * skew(r1) - skew(r2) * invI2 * skew(r2)]
            //      = [1/m1+1/m2     0    ] + invI1 * [r1.y*r1.y -r1.x*r1.y] + invI2 * [r1.y*r1.y -r1.x*r1.y]
            //        [    0     1/m1+1/m2]           [-r1.x*r1.y r1.x*r1.x]           [-r1.x*r1.y r1.x*r1.x]

            Matrix2x2 K;
            K.m00 = mass1Inv + mass2Inv;
            K.m11 = mass1Inv + mass2Inv;

            K.m00 += inertia1Inv * r1.Y * r1.Y;
            K.m01 = -inertia1Inv * r1.X * r1.Y;
            K.m10 = -inertia1Inv * r1.X * r1.Y;
            K.m11 += inertia1Inv * r1.X * r1.X;

            K.m00 += inertia2Inv * r2.Y * r2.Y;
            K.m01 -= inertia2Inv * r2.X * r2.Y;
            K.m10 -= inertia2Inv * r2.X * r2.Y;
            K.m11 += inertia2Inv * r2.X * r2.X;


            K.m00 += softness;
            K.m11 += softness;

            Matrix2x2.Invert(ref K, out M);


            Vector2D dp, vect1, vect2;
            Vector2D.Add(ref body1.State.Position.Linear, ref r1, out vect1);
            Vector2D.Add(ref body2.State.Position.Linear, ref r2, out vect2);
            Vector2D.Subtract(ref vect2, ref vect1, out dp);

            if (!Scalar.IsPositiveInfinity(distanceTolerance) &&
                dp.MagnitudeSq > distanceTolerance * distanceTolerance)
            {
                this.Lifetime.IsExpired = true;
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
                    ref body1.State.Velocity, ref accumulatedImpulse,
                    ref r1, ref mass1Inv, ref inertia1Inv);

                PhysicsHelper.AddImpulse(
                    ref body2.State.Velocity, ref accumulatedImpulse,
                    ref r2, ref mass2Inv, ref inertia2Inv);
            }
            else
            {
                accumulatedImpulse = Vector2D.Zero;
            }
            body1.ApplyProxy();
            body2.ApplyProxy();
        }
        void Solvers.ISequentialImpulsesJoint.ApplyImpulse()
        {

            Scalar mass1Inv = body1.Mass.MassInv;
            Scalar mass2Inv = body2.Mass.MassInv;
            Scalar inertia1Inv = body1.Mass.MomentOfInertiaInv;
            Scalar inertia2Inv = body2.Mass.MomentOfInertiaInv;
            PhysicsState state1 = body1.State;
            PhysicsState state2 = body2.State;

            Vector2D dv;
            PhysicsHelper.GetRelativeVelocity(
                ref state1.Velocity, ref state2.Velocity,
                ref r1, ref r2, out dv);

           

            Vector2D impulse;
            impulse.X = bias.X - dv.X - softness * accumulatedImpulse.X;
            impulse.Y = bias.Y - dv.Y - softness * accumulatedImpulse.Y;
            Vector2D.Transform(ref  M, ref impulse, out impulse);
            //impulse = M * (bias - dv - softness * P);

            PhysicsHelper.SubtractImpulse(
                ref state1.Velocity, ref impulse,
                ref r1, ref mass1Inv, ref inertia1Inv);

            PhysicsHelper.AddImpulse(
                ref state2.Velocity, ref impulse,
                ref r2, ref mass2Inv, ref inertia2Inv);

            Vector2D.Add(ref accumulatedImpulse, ref impulse, out accumulatedImpulse);

            body1.ApplyProxy();
            body2.ApplyProxy();
        }
    }
}