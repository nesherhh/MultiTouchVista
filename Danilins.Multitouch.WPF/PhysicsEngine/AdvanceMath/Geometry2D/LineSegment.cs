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
using System.Runtime.InteropServices;
using AdvanceMath.Design;
namespace AdvanceMath.Geometry2D
{
    [StructLayout(LayoutKind.Sequential, Size = LineSegment.Size), Serializable]
    [AdvBrowsableOrder("Vertex1,Vertex2")]
#if !CompactFramework && !WindowsCE && !PocketPC && !XBOX360 
    [System.ComponentModel.TypeConverter(typeof(AdvTypeConverter<LineSegment>))]
#endif
    public struct LineSegment : IEquatable<LineSegment>
    {
        public const int Size = Vector2D.Size * 2;

        public static void Intersects(ref Vector2D v1, ref Vector2D v2, ref Vector2D v3, ref Vector2D v4, out bool result)
        {
            Scalar div, ua, ub;
            div = 1 / ((v4.Y - v3.Y) * (v2.X - v1.X) - (v4.X - v3.X) * (v2.Y - v1.Y));
            ua = ((v4.X - v3.X) * (v1.Y - v3.Y) - (v4.Y - v3.Y) * (v1.X - v3.X)) * div;
            ub = ((v2.X - v1.X) * (v1.Y - v3.Y) - (v2.Y - v1.Y) * (v1.X - v3.X)) * div;
            result = ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1;
        }
        public static bool Intersects(ref Vector2D v1, ref Vector2D v2, ref Vector2D v3, ref Vector2D v4, out Vector2D result)
        {
            Scalar div, ua, ub;
            div = 1 / ((v4.Y - v3.Y) * (v2.X - v1.X) - (v4.X - v3.X) * (v2.Y - v1.Y));
            ua = ((v4.X - v3.X) * (v1.Y - v3.Y) - (v4.Y - v3.Y) * (v1.X - v3.X)) * div;
            ub = ((v2.X - v1.X) * (v1.Y - v3.Y) - (v2.Y - v1.Y) * (v1.X - v3.X)) * div;
            if (ua >= 0 && ua <= 1 && ub >= 0 && ub <= 1)
            {
                Vector2D.Lerp(ref v1, ref v2, ref ua, out result);
                return true;
            }
            else
            {
                result = Vector2D.Zero;
                return false;
            }
        }


        public static void Intersects(ref Vector2D vertex1, ref Vector2D vertex2, ref Ray ray, out Scalar result)
        {
            Vector2D tanget, normal;
            Scalar edgeMagnitude;
            Vector2D.Subtract(ref vertex1, ref vertex2, out tanget);
            Vector2D.Normalize(ref tanget, out edgeMagnitude, out tanget);
            Vector2D.GetRightHandNormal(ref tanget, out normal);

            Scalar dir;
            Vector2D.Dot(ref normal, ref ray.Direction, out dir);
            if (Math.Abs(dir) >= MathHelper.Tolerance)
            {
                Vector2D originDiff;
                Vector2D.Subtract(ref ray.Origin, ref vertex2, out originDiff);
                Scalar actualDistance;
                Vector2D.Dot(ref normal, ref originDiff, out actualDistance);
                Scalar DistanceFromOrigin = -(actualDistance / dir);
                if (DistanceFromOrigin >= 0)
                {
                    Vector2D intersectPos;
                    Vector2D.Multiply(ref ray.Direction, ref DistanceFromOrigin, out intersectPos);
                    Vector2D.Add(ref intersectPos, ref originDiff, out intersectPos);

                    Scalar distanceFromSecond;
                    Vector2D.Dot(ref intersectPos, ref tanget, out distanceFromSecond);

                    if (distanceFromSecond >= 0 && distanceFromSecond <= edgeMagnitude)
                    {
                        result = DistanceFromOrigin;
                        return;
                    }
                }
            }
            result = -1;
        }

        public static void GetDistance(ref Vector2D vertex1, ref Vector2D vertex2, ref Vector2D point, out Scalar result)
        {
            Scalar edgeLength;
            Vector2D edge, local;

            Vector2D.Subtract(ref point, ref vertex2, out local);
            Vector2D.Subtract(ref vertex1, ref vertex2, out edge);
            Vector2D.Normalize(ref edge, out edgeLength, out edge);

            Scalar nProj = local.Y * edge.X - local.X * edge.Y;
            Scalar tProj = local.X * edge.X + local.Y * edge.Y;
            if (tProj < 0)
            {
                result = MathHelper.Sqrt(tProj * tProj + nProj * nProj);
            }
            else if (tProj > edgeLength)
            {
                tProj -= edgeLength;
                result = MathHelper.Sqrt(tProj * tProj + nProj * nProj);
            }
            else
            {
                result = Math.Abs(nProj);
            }
        }
        public static void GetDistanceSq(ref Vector2D vertex1, ref Vector2D vertex2, ref Vector2D point, out Scalar result)
        {
            Scalar edgeLength;
            Vector2D edge, local;

            Vector2D.Subtract(ref point, ref vertex2, out local);
            Vector2D.Subtract(ref vertex1, ref vertex2, out edge);
            Vector2D.Normalize(ref edge, out edgeLength, out edge);

            Scalar nProj = local.Y * edge.X - local.X * edge.Y;
            Scalar tProj = local.X * edge.X + local.Y * edge.Y;
            if (tProj < 0)
            {
                result = tProj * tProj + nProj * nProj;
            }
            else if (tProj > edgeLength)
            {
                tProj -= edgeLength;
                result = tProj * tProj + nProj * nProj;
            }
            else
            {
                result = nProj * nProj;
            }
        }

        [AdvBrowsable]
        public Vector2D Vertex1;
        [AdvBrowsable]
        public Vector2D Vertex2;

        [InstanceConstructor("Vertex1,Vertex2")]
        public LineSegment(Vector2D vertex1, Vector2D vertex2)
        {
            this.Vertex1 = vertex1;
            this.Vertex2 = vertex2;
        }

        public Scalar GetDistance(Vector2D point)
        {
            Scalar result;
            GetDistance(ref point, out result);
            return result;
        }
        public void GetDistance(ref Vector2D point, out Scalar result)
        {
            GetDistance(ref Vertex1, ref Vertex2, ref point, out result);
        }

        public Scalar Intersects(Ray ray)
        {
            Scalar result;
            Intersects(ref ray, out result);
            return result;
        }
        public void Intersects(ref Ray ray, out Scalar result)
        {
            Intersects(ref Vertex1, ref Vertex2, ref ray, out result);
        }



        public override string ToString()
        {
            return string.Format("V1: {0} V2: {1}", Vertex1, Vertex2);
        }
        public override int GetHashCode()
        {
            return Vertex1.GetHashCode() ^ Vertex2.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return obj is LineSegment && Equals((LineSegment)obj);
        }
        public bool Equals(LineSegment other)
        {
            return Equals(ref this, ref other);
        }
        public static bool Equals(LineSegment line1, LineSegment line2)
        {
            return Equals(ref line1, ref line2);
        }
        [CLSCompliant(false)]
        public static bool Equals(ref LineSegment line1, ref LineSegment line2)
        {
            return Vector2D.Equals(ref line1.Vertex1, ref line2.Vertex1) && Vector2D.Equals(ref line1.Vertex2, ref line2.Vertex2);
        }

        public static bool operator ==(LineSegment line1, LineSegment line2)
        {
            return Equals(ref line1, ref line2);
        }
        public static bool operator !=(LineSegment line1, LineSegment line2)
        {
            return !Equals(ref line1, ref line2);
        }
    }
}