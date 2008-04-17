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
    /// A Joint between 2 Bodies that will keep the Angles between the 2 bodies at a certain amount.
    /// </summary>
    [Serializable]
    public sealed class FixedAngleJoint : Joint, Solvers.ISequentialImpulsesJoint
    {
        Body body;
        Scalar angle;

        Scalar bias;
        Scalar biasFactor;
        Scalar softness;
        Scalar M;

        public FixedAngleJoint(Body body, Lifespan lifetime)
            : base(lifetime)
        {
            if (body == null) { throw new ArgumentNullException("body"); }
            this.body = body;
            this.angle = body.State.Position.Angular;
            this.softness = 0.001f;
            this.biasFactor = 0.2f;
        }

        public Scalar Angle
        {
            get { return angle; }
            set { MathHelper.ClampAngle(ref value, out angle); }
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
        public override ReadOnlyCollection<Body> Bodies
        {
            get { return new ReadOnlyCollection<Body>(new Body[1] { body }); }
        }
        void Solvers.ISequentialImpulsesJoint.PreStep(TimeStep step)
        {
            Scalar difference = body.State.Position.Angular - angle;
            bias = -biasFactor * step.DtInv * difference;
            M = (1 - softness) / (body.Mass.MomentOfInertiaInv);
        }
        void Solvers.ISequentialImpulsesJoint.ApplyImpulse()
        {
            Scalar angularImpulse = M * (bias - body.State.Velocity.Angular);
            body.State.Velocity.Angular += body.Mass.MomentOfInertiaInv * angularImpulse;
        }
    }
}