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
    /// <summary>
    /// A PhysicsLogic to causes a Body to have a realistic pull of Gravity with a exponential drop-off. 
    /// </summary>
    [Serializable]
    public sealed class GravityPointMass : PhysicsLogic
    {
        Scalar metersPerDistanceUnit;
        Body body;
        /// <summary>
        /// Creates a new GravityPointMass Instance.
        /// </summary>
        /// <param name="body">The body that will be the source of gravity.</param>
        /// <param name="lifetime">A object Describing how long the object will be in the engine.</param>
        public GravityPointMass(Body body, Lifespan lifetime):
            this(body,1,lifetime)
        {}
        /// <summary>
        /// Creates a new GravityPointMass Instance.
        /// </summary>
        /// <param name="body">The body that will be the source of gravity.</param>
        /// <param name="metersPerDistanceUnit">The scale of of the universe.</param>
        /// <param name="lifetime">A object Describing how long the object will be in the engine.</param>
        public GravityPointMass(Body body, Scalar metersPerDistanceUnit, Lifespan lifetime)
            : base(lifetime)
        {
            if (body == null) { throw new ArgumentNullException("body"); }
            if (metersPerDistanceUnit <= 0) { throw new ArgumentOutOfRangeException("metersPerDistanceUnit"); }
            this.body = body;
            this.metersPerDistanceUnit = metersPerDistanceUnit;
        }
        protected internal override void RunLogic(TimeStep step)
        {
            foreach (Body e in Bodies)
            {
                if (e != body && !e.IgnoresGravity)
                {
                    Scalar magnitude;
                    Vector2D gravity;
                    Vector2D.Subtract(ref body.State.Position.Linear, ref e.State.Position.Linear, out gravity);
                    Vector2D.Normalize(ref gravity, out magnitude, out gravity);
                    magnitude = (body.Mass.AccelerationDueToGravity /
                            (magnitude * magnitude * metersPerDistanceUnit * metersPerDistanceUnit));
                    Vector2D.Multiply(ref gravity, ref magnitude, out gravity);
                    Vector2D.Add(ref e.State.Acceleration.Linear, ref gravity, out e.State.Acceleration.Linear);
                }
            }
        }
        protected internal override void BeforeAddCheck(PhysicsEngine engine)
        {
            if (body.Engine != engine) { throw new InvalidOperationException("The Body must be added to the Engine before the GravityPointMass."); }
        }
        protected override void OnAdded()
        {
            this.body.Removed += OnBodyRemoved;
        }
        void OnBodyRemoved(object sender, RemovedEventArgs e)
        {
            this.Lifetime.IsExpired = true;
        }
        protected override void OnRemoved(PhysicsEngine engine, bool wasPending)
        {
            if (!wasPending)
            {
                this.body.Removed -= OnBodyRemoved;
            }
        }
    }

}