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
    /// A Gravity Field that apply gravity pulling in one direction regardless of the Body's position with zero dropoff.
    /// </summary>
    [Serializable]
    public sealed class GravityField : PhysicsLogic
    {
        Vector2D gravity;
        /// <summary>
        /// Creates a new GravityField Instance.
        /// </summary>
        /// <param name="gravity">The direction and magnitude of the gravity.</param>
        /// <param name="lifeTime">A object Describing how long the object will be in the engine.</param>
        public GravityField(Vector2D gravity, Lifespan lifetime)
            : base(lifetime)
        {
            this.gravity = gravity;
        }
        protected internal override void RunLogic(TimeStep step)
        {
            foreach (Body e in this.Bodies)
            {
                if (!e.IgnoresGravity)
                {
                    Vector2D.Add(ref e.State.Acceleration.Linear, ref gravity, out e.State.Acceleration.Linear);
                }
            }
        }
    }
}