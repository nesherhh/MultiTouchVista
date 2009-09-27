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

using AdvanceMath;

namespace Physics2DDotNet.PhysicsLogics
{
    [Serializable]
    public sealed class VelocityLimitLogic : PhysicsLogic
    {
        Scalar maxLinearVelocity;
        Scalar maxAngularVelocity;
        public VelocityLimitLogic(Scalar maxLinearVelocity, Scalar maxAngularVelocity, Lifespan lifetime)
            : base(lifetime)
        {
            if (maxLinearVelocity < 0) { throw new ArgumentOutOfRangeException("maxLinearVelocity"); }
            if (maxAngularVelocity < 0) { throw new ArgumentOutOfRangeException("maxAngularVelocity"); }
            this.maxLinearVelocity = maxLinearVelocity;
            this.maxAngularVelocity = maxAngularVelocity;
        }
        protected internal override void RunLogic(TimeStep step)
        {
            foreach (Body e in Bodies)
            {
                Scalar velocity;
                Vector2D.GetMagnitude(ref e.State.Velocity.Linear, out velocity);
                if (velocity > maxLinearVelocity)
                {
                    velocity = maxLinearVelocity / velocity;
                    Vector2D.Multiply(ref e.State.Velocity.Linear, ref velocity, out e.State.Velocity.Linear);
                }
                if (e.State.Velocity.Angular > maxAngularVelocity)
                {
                    e.State.Velocity.Angular = maxAngularVelocity;
                }
                else if (e.State.Velocity.Angular < -maxAngularVelocity)
                {
                    e.State.Velocity.Angular = -maxAngularVelocity;
                }
            }
        }
    }
}