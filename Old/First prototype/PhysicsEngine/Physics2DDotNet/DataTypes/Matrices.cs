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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AdvanceMath;
using AdvanceMath.Geometry2D;


namespace Physics2DDotNet
{
    [Serializable]
    public sealed class Matrices : IDuplicateable<Matrices>
    {

        /// <summary>
        /// The matrix that is multiplied against to transform a vertex from relative 
        /// to the Body to the vertex relative to the World.
        /// </summary>
        public Matrix2x3 ToWorld;
        /// <summary>
        /// The matrix that is multiplied against to transform a vertex from relative 
        /// to the World to the vertex relative to the Body.
        /// </summary>
        public Matrix2x3 ToBody;
        /// <summary>
        /// The matrix that is multiplied against to transform a normal (unit vector) from relative 
        /// to the Body to the normal relative to the World.
        /// </summary>
        public Matrix2x2 ToWorldNormal;
        /// The matrix that is multiplied against to transform a normal (unit vector) from relative 
        /// to the World to the normal relative to the Body.
        /// </summary>
        public Matrix2x2 ToBodyNormal;
        public Matrices()
        {
            this.ToWorld = Matrix2x3.Identity;
            this.ToBody = Matrix2x3.Identity;
            this.ToWorldNormal = Matrix2x2.Identity;
            this.ToBodyNormal = Matrix2x2.Identity;
        }
        private Matrices(Matrices copy)
        {
            this.ToWorld = copy.ToWorld;
            this.ToBody = copy.ToBody;
            this.ToWorldNormal = copy.ToWorldNormal;
            this.ToBodyNormal = copy.ToBodyNormal;
        }
        internal void Set(ref Matrix2x3 toWorld)
        {
            this.ToWorld = toWorld;
            Matrix2x3.Invert(ref toWorld, out ToBody);
            Matrix2x2.CreateNormal(ref toWorld, out ToWorldNormal);
            Matrix2x2.CreateNormal(ref ToBody, out ToBodyNormal);
        }
        public Matrices Duplicate()
        {
            return new Matrices(this);
        }
        public object Clone()
        {
            return Duplicate();
        }
    }
}