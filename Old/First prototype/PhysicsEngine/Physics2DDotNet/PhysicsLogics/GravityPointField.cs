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
    /// A PhysicsLogic to causes a Gravity a a certain point with zero dropoff.
    /// </summary>
    [Serializable]
    public sealed class GravityPointField : PhysicsLogic
    {
        Vector2D location;
        Scalar gravity;
        /// <summary>
        /// Creates a new GravityPointField Instance.
        /// </summary>
        /// <param name="location">The location of the Gravity point.</param>
        /// <param name="gravity"></param>
        /// <param name="lifetime"></param>
        public GravityPointField(Vector2D location, Scalar gravity, Lifespan lifetime)
            : base(lifetime)
        {
            this.location = location;
            this.gravity = gravity;
        }
        public Vector2D Location {
            get { return location; }
            set { location = value; }
        }
        public Scalar Gravity
        {
            get { return gravity; }
            set { gravity = value; }
        }
        protected internal override void RunLogic(TimeStep step)
        {
            foreach (Body e in this.Bodies)
            {
                if (!e.IgnoresGravity)
                {
                    Vector2D vect;
                    Vector2D.Subtract(ref location, ref e.State.Position.Linear, out vect);
                    Vector2D.Normalize(ref vect, out vect);
                    Vector2D.Multiply(ref vect, ref gravity, out vect);
                    Vector2D.Add(ref e.State.Acceleration.Linear, ref vect, out e.State.Acceleration.Linear);
                }
            }
        }
    }

}