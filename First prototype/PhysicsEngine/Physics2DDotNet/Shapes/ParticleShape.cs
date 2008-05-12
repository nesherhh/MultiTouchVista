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

using AdvanceMath.Geometry2D;

namespace Physics2DDotNet.Shapes
{
    /// <summary>
    /// Represents a Single point.
    /// </summary>
    [Serializable]
    public sealed class ParticleShape : Shape
    {
        #region static
        /// <summary>
        /// All particles are the same! so use this one!
        /// </summary>
        public static readonly ParticleShape Default = new ParticleShape();
        #endregion
        #region constructors
        /// <summary>
        /// Creates a new Particle Instance.
        /// </summary>
        public ParticleShape()
            : this(1)
        { }
        /// <summary>
        /// Creates a new Particle Instance.
        /// </summary>
        /// <param name="momentOfInertiaMultiplier">
        /// How hard it is to turn the shape. Depending on the construtor in the 
        /// Body this will be multiplied with the mass to determine the moment of inertia.
        /// </param>
        public ParticleShape(Scalar momentOfInertiaMultiplier)
            : base(new Vector2D[] { Vector2D.Zero }, momentOfInertiaMultiplier)
        { }
        #endregion
        #region Properties

        public override bool CanGetIntersection
        {
            get { return false; }
        }
        public override bool CanGetDistance
        {
            get { return true; }
        }
        public override bool BroadPhaseDetectionOnly
        {
            get { return false; }
        }
        public override bool CanGetCustomIntersection
        {
            get { return false; }
        }
        #endregion
        #region Methods
        public override void CalcBoundingRectangle(Matrices matrices, out BoundingRectangle rectangle)
        {
            Vector2D.Transform(ref matrices.ToWorld, ref Zero, out rectangle.Max);
            rectangle.Min = rectangle.Max;
        }
        public override bool TryGetIntersection(Vector2D point, out IntersectionInfo info)
        {
            throw new NotSupportedException();
        }
        public override bool TryGetCustomIntersection(Body self, Body other, out object customIntersectionInfo)
        {
            throw new NotSupportedException();
        }
        public override void GetDistance(ref Vector2D point, out Scalar result)
        {
            Vector2D.Distance(ref point, ref Zero, out result);
        }
        #endregion
    }
}