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
namespace Physics2DDotNet
{
    /// <summary>
    /// Describes the Coefficients of a surface.
    /// </summary>
    [Serializable]
    public sealed class Coefficients : IDuplicateable<Coefficients>
    {
        private Scalar restitution;
        private Scalar staticFriction;
        private Scalar dynamicFriction;
        public Coefficients(Scalar restitution, Scalar friction)
        {
            this.restitution = restitution;
            this.staticFriction = friction;
            this.dynamicFriction = friction;
        }
        public Coefficients(Scalar restitution, Scalar staticFriction, Scalar dynamicFriction)
        {
            this.restitution = restitution;
            this.staticFriction = staticFriction;
            this.dynamicFriction = dynamicFriction;
        }
        /// <summary>
        /// AKA Bounciness. This is how much energy is kept as kinetic energy after a collision.
        /// </summary>
        public Scalar Restitution
        {
            get { return restitution; }
            set { restitution = value; }
        }
        /// <summary>
        /// (NOT USED)
        /// http://en.wikipedia.org/wiki/Friction
        /// </summary>
        public Scalar StaticFriction
        {
            get { return staticFriction; }
            set { staticFriction = value; }
        }
        /// <summary>
        /// http://en.wikipedia.org/wiki/Friction
        /// </summary>
        public Scalar DynamicFriction
        {
            get { return dynamicFriction; }
            set { dynamicFriction = value; }
        }



        public Coefficients Duplicate()
        {
            return new Coefficients(restitution, staticFriction);//, dynamicFriction);
        }
        public object Clone()
        {
            return Duplicate();
        }

    }
}
