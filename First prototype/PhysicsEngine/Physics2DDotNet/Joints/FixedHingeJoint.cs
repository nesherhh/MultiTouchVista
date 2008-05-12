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
    /// A joint that makes a single Body Pivot around an Anchor.
    /// </summary>
    [Serializable]
    public class FixedHingeJoint : Joint, Solvers.ISequentialImpulsesJoint
    {
        Solvers.SequentialImpulsesSolver solver;
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


        public FixedHingeJoint(Body body, Vector2D anchor, Lifespan lifetime)
            : base(lifetime)
        {
            if (body == null) { throw new ArgumentNullException("body"); }
            this.body = body;
            this.anchor = anchor;
            body.ApplyPosition();
            Vector2D.Transform(ref body.Matrices.ToBody, ref anchor, out this.localAnchor1);
            this.softness = 0.001f;
            this.biasFactor = 0.2f;
            this.distanceTolerance = Scalar.PositiveInfinity;
        }
        public virtual Vector2D Anchor
        {
            get { return anchor; }
            set { anchor = value; }
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
        public override ReadOnlyCollection<Body> Bodies
        {
            get { return new ReadOnlyCollection<Body>(new Body[1] { body }); }
        }
        protected override void OnAdded()
        {
            this.solver = (Solvers.SequentialImpulsesSolver)Engine.Solver;
        }
        void Solvers.ISequentialImpulsesJoint.PreStep(TimeStep step)
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
                    ref body.State.Velocity, ref accumulatedImpulse,
                    ref r1, ref mass1Inv, ref inertia1Inv);
            }
            else
            {
                accumulatedImpulse = Vector2D.Zero;
            }
            body.ApplyProxy();

        }
        void Solvers.ISequentialImpulsesJoint.ApplyImpulse()
        {
            Scalar mass1Inv = body.Mass.MassInv;
            Scalar inertia1Inv = body.Mass.MomentOfInertiaInv;

            Vector2D dv;
            PhysicsHelper.GetRelativeVelocity(ref body.State.Velocity, ref r1, out dv);


            Vector2D impulse;
            impulse.X = bias.X - dv.X - softness * accumulatedImpulse.X;
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