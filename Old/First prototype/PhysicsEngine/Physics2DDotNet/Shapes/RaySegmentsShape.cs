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


namespace Physics2DDotNet.Shapes
{
    /// <summary>
    /// A Ray Segment is a Ray that has a length. It can be used to represent 
    /// lasers or very fast projectiles.
    /// </summary>
    [Serializable]
    public struct RaySegment
    {
        public Ray RayInstance;
        public Scalar Length;
        public RaySegment(Ray ray, Scalar length)
        {
            this.RayInstance = ray;
            this.Length = length;
        }
        public RaySegment(Vector2D origin,Vector2D direction, Scalar length)
        {
            this.RayInstance.Origin = origin;
            this.RayInstance.Direction = direction;
            this.Length = length;
        }
    }
    /// <summary>
    /// The information of an intersection with another shape. 
    /// </summary>
    [Serializable]
    public class RaySegmentIntersectionInfo
    {
        Scalar[] distances;
        public RaySegmentIntersectionInfo(Scalar[] distances)
        {
            this.distances = distances;
        }
        /// <summary>
        /// An collection of distances away from the Ray Segments. 
        /// The indexes match up with the Segments in the Ray Segments class. 
        /// A negative value means there was no intersection. 
        /// </summary>
        public ReadOnlyCollection<Scalar> Distances
        {
            get { return new ReadOnlyCollection<Scalar>(distances); }
        }
    }

    /// <summary>
    /// A shape that holds multiple Ray Segments and generates custom collision events 
    /// for when they intersect something. The Sequential Impulses Solver does not 
    /// handle collisions with this shape.
    /// </summary>
    [Serializable]
    public class RaySegmentsShape : Shape
    {
        #region fields
        RaySegment[] segments;
        #endregion
        #region constructors
        public RaySegmentsShape(params RaySegment[] segments)
            : base(Empty, 1)
        {
            if (segments == null) { throw new ArgumentNullException("segments"); }
            this.segments = segments;
        } 
        #endregion
        #region properties
        public RaySegment[] Segments
        {
            get { return segments; }
        }
        public override bool CanGetIntersection
        {
            get { return false; }
        }
        public override bool CanGetDistance
        {
            get { return false; }
        }
        public override bool CanGetCustomIntersection
        {
            get { return true; }
        }
        public override bool BroadPhaseDetectionOnly
        {
            get { return false; }
        }
        #endregion
        #region methods
        public override bool TryGetCustomIntersection(Body self, Body other, out object customIntersectionInfo)
        {
            IRaySegmentsCollidable rayCollidable = other.Shape as IRaySegmentsCollidable;
            RaySegmentIntersectionInfo info;
            if (rayCollidable != null &&
                rayCollidable.TryGetRayCollision(other, self, this, out info))
            {
                customIntersectionInfo = info;
                return true;
            }
            else
            {
                customIntersectionInfo = null;
                return false;
            }
        }

        public override void GetDistance(ref Vector2D point, out Scalar result)
        {
            throw new NotSupportedException();
        }
        public override bool TryGetIntersection(Vector2D point, out IntersectionInfo info)
        {
            throw new NotSupportedException();
        }
        public override void CalcBoundingRectangle(Matrices matrices, out BoundingRectangle rectangle)
        {
            Matrix2x3 matrix = matrices.ToWorld;
            Vector2D v1, v2;
            RaySegment segment = segments[0];
            v1 = segment.RayInstance.Origin;
            v2.X = segment.RayInstance.Direction.X * segment.Length + v1.X;
            v2.Y = segment.RayInstance.Direction.Y * segment.Length + v1.Y;
            Vector2D.Transform(ref matrix, ref v1, out v1);
            Vector2D.Transform(ref matrix, ref v2, out v2);
            if (v1.X > v2.X)
            {
                rectangle.Min.X = v2.X;
                rectangle.Max.X = v1.X;
            }
            else
            {
                rectangle.Min.X = v1.X;
                rectangle.Max.X = v2.X;
            }
            if (v1.Y > v2.Y)
            {
                rectangle.Min.Y = v2.Y;
                rectangle.Max.Y = v1.Y;
            }
            else
            {
                rectangle.Min.Y = v1.Y;
                rectangle.Max.Y = v2.Y;
            }

            for (int index = 1; index < segments.Length; ++index)
            {
                segment = segments[index];
                v1 = segment.RayInstance.Origin;
                v2.X = segment.RayInstance.Direction.X * segment.Length + v1.X;
                v2.Y = segment.RayInstance.Direction.Y * segment.Length + v1.Y;
                Vector2D.Transform(ref matrix, ref v1, out v1);
                Vector2D.Transform(ref matrix, ref v2, out v2);
                if (v1.X > rectangle.Max.X)
                {
                    rectangle.Max.X = v1.X;
                }
                else if (v1.X < rectangle.Min.X)
                {
                    rectangle.Min.X = v1.X;
                }
                if (v1.Y > rectangle.Max.Y)
                {
                    rectangle.Max.Y = v1.Y;
                }
                else if (v1.Y < rectangle.Min.Y)
                {
                    rectangle.Min.Y = v1.Y;
                }

                if (v2.X > rectangle.Max.X)
                {
                    rectangle.Max.X = v2.X;
                }
                else if (v2.X < rectangle.Min.X)
                {
                    rectangle.Min.X = v2.X;
                }
                if (v2.Y > rectangle.Max.Y)
                {
                    rectangle.Max.Y = v2.Y;
                }
                else if (v2.Y < rectangle.Min.Y)
                {
                    rectangle.Min.Y = v2.Y;
                }
            }
        }

        #endregion
    }

}