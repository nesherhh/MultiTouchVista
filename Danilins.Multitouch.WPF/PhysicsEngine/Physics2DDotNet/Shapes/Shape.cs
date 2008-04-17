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

using System.Xml.Serialization;

namespace Physics2DDotNet.Shapes
{


    /// <summary>
    /// A abstract class used to define the Shape of a Body.
    /// </summary>
    [Serializable]
    public abstract class Shape : IShape
    {
        #region static fields
        protected static Vector2D Zero = Vector2D.Zero;
        private static Vector2D[] empty = new Vector2D[0];
        protected static Vector2D[] Empty { get { return empty; } }
        #endregion
        #region static methods
        public static Scalar InertiaOfCylindricalShell(Scalar radius)
        {
            return radius * radius;
        }
        public static Scalar InertiaOfHollowCylinder(Scalar innerRadius, Scalar outerRadius)
        {
            return .5f * (innerRadius * innerRadius + outerRadius * outerRadius);
        }
        public static Scalar InertiaOfSolidCylinder(Scalar radius)
        {
            return .5f * (radius * radius);
        }
        public static Scalar InertiaOfRectangle(Scalar length, Scalar width)
        {
            return (1f / 12f) * (length * length + width * width);
        }
        public static Scalar InertiaOfSquare(Scalar sideLength)
        {
            return (1f / 6f) * (sideLength * sideLength);
        }
        public static Scalar InertiaOfPolygon(Vector2D[] vertexes)
        {
            if (vertexes == null) { throw new ArgumentNullException("vertexes"); }
            if (vertexes.Length == 0) { throw new ArgumentOutOfRangeException("vertexes"); }
            if (vertexes.Length == 1) { return 0; }

            Scalar denom = 0;
            Scalar numer = 0;
            Scalar a, b, c, d;
            Vector2D v1, v2;
            v1 = vertexes[vertexes.Length - 1];
            for (int index = 0; index < vertexes.Length; index++, v1 = v2)
            {
                v2 = vertexes[index];
                Vector2D.Dot(ref v2, ref v2, out a);
                Vector2D.Dot(ref v2, ref v1, out b);
                Vector2D.Dot(ref v1, ref v1, out c);
                Vector2D.ZCross(ref v1, ref v2, out d);
                d = Math.Abs(d);
                numer += d;
                denom += (a + b + c) * d;
            }
            return denom / (numer * 6);
        }
        public static Vector2D[] CreateCircle(Scalar radius, int vertexCount)
        {
            if (radius <= 0) { throw new ArgumentOutOfRangeException("radius", "Must be greater then zero."); }
            if (vertexCount < 3) { throw new ArgumentOutOfRangeException("vertexCount", "Must be equal or greater then 3"); }
            Vector2D[] result = new Vector2D[vertexCount];
            Scalar angleIncrement = MathHelper.TwoPi / vertexCount;
            for (int index = 0; index < vertexCount; ++index)
            {
                Scalar angle = angleIncrement * index;
                Vector2D.FromLengthAndAngle(ref radius, ref angle, out result[index]);
            }
            return result;
        }

        #endregion
        #region fields
        private Scalar inertiaMultiplier;
        private Vector2D[] vertexes;
        private Vector2D[] normals;
        private bool ignoreVertexes;
        private object tag;
        #endregion
        #region constructors
        protected Shape(Vector2D[] vertexes, Scalar momentOfInertiaMultiplier)
        {
            if (vertexes == null) { throw new ArgumentNullException("vertexes"); }
            if (momentOfInertiaMultiplier <= 0) { throw new ArgumentOutOfRangeException("momentOfInertiaMultiplier"); }
            this.vertexes = vertexes;
            this.inertiaMultiplier = momentOfInertiaMultiplier;
        }
        #endregion
        #region properties
        /// <summary>
        /// Gets the original (body/local) Vertices with the origin being the center of the Body.
        /// </summary>
        public Vector2D[] Vertexes
        {
            get { return vertexes; }
        }
        /// <summary>
        /// These are the normals for the original vertexes.
        /// </summary>
        public Vector2D[] Normals
        {
            get { return normals; }
            protected set { normals = value; }
        }

        /// <summary>
        /// Gets and Sets if this shape's Vertexes will not be used in collision detection.
        /// </summary>
        public bool IgnoreVertexes
        {
            get { return ignoreVertexes; }
            set { ignoreVertexes = value; }
        }
        public abstract bool CanGetIntersection { get;}
        public abstract bool CanGetDistance { get;}
        public abstract bool CanGetCustomIntersection { get;}
        /// <summary>
        /// Gets if this detects collisions only with bounding boxes 
        /// and if it does then only bodies colliding it will also generate collision events as well.
        /// if this is true it can allow you to write your own collision Solver just for this Shape. 
        /// Or you can use this to do clipping.
        /// </summary>
        public abstract bool BroadPhaseDetectionOnly { get;}
        /// <summary>
        /// Gets the Moment of Inertia Multiplier. Which is the ratio of inertia to mass of a Body.
        /// </summary>
        public Scalar MomentofInertiaMultiplier
        {
            get { return inertiaMultiplier; }
        }
        [XmlIgnore]
        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        #endregion
        #region methods

        public abstract void CalcBoundingRectangle(Matrices matrices, out BoundingRectangle rectangle);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point">(In Body Coordinates)</param>
        /// <param name="info"></param>
        /// <returns></returns>
        public abstract bool TryGetIntersection(Vector2D point, out IntersectionInfo info);
        public abstract bool TryGetCustomIntersection(Body self, Body other, out object customIntersectionInfo);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="point">(In Body Coordinates)</param>
        /// <param name="result"></param>
        public abstract void GetDistance(ref Vector2D point, out Scalar result);
        #endregion
    }
}