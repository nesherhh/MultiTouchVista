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
    [StructLayout(LayoutKind.Sequential, Size = Line.Size)]
    [AdvBrowsableOrder("Normal,D"), Serializable]
#if !CompactFramework && !WindowsCE && !PocketPC && !XBOX360 
    [System.ComponentModel.TypeConverter(typeof(AdvTypeConverter<Line>))]
#endif
    public struct Line : IEquatable<Line>
    {
        public const int Size = sizeof(Scalar) + Vector2D.Size;
        [AdvBrowsable]
        public Vector2D Normal;
        [AdvBrowsable]
        public Scalar D;
        [InstanceConstructor("Normal,D")]
        public Line(Vector2D normal, Scalar d)
        {
            this.Normal = normal;
            this.D = d;
        }
        public Line(Scalar nX, Scalar nY, Scalar d)
        {
            this.Normal.X = nX;
            this.Normal.Y = nY;
            this.D = d;
        }

        public Scalar GetDistance(Vector2D point)
        {
            Scalar result;
            GetDistance(ref point, out result);
            return result;
        }
        public void GetDistance(ref Vector2D point, out Scalar result)
        {
            Vector2D.Dot(ref point, ref Normal, out result);
            result -= D;
        }

        public Scalar Intersects(Ray ray)
        {
            Scalar result;
            Intersects(ref ray, out result);
            return result;
        }
        public bool Intersects(BoundingRectangle rect)
        {
            bool result;
            Intersects(ref rect, out result);
            return result;
        }
        public bool Intersects(BoundingCircle circle)
        {
            bool result;
            circle.Intersects(ref this, out result);
            return result;
        }
        public bool Intersects(BoundingPolygon polygon)
        {
            bool result;
            Intersects(ref polygon, out result);
            return result;
        }

        public void Intersects(ref Ray ray, out Scalar result)
        {
            Scalar dir;
            Vector2D.Dot(ref Normal, ref ray.Direction, out dir);
            if (-dir > 0)
            {
                Scalar DistanceFromOrigin = Normal * ray.Origin + D;
                Vector2D.Dot(ref Normal, ref ray.Origin, out DistanceFromOrigin);
                DistanceFromOrigin = -((DistanceFromOrigin + D) / dir);
                if (DistanceFromOrigin >= 0)
                {
                    result = DistanceFromOrigin;
                    return;
                }
            }
            result = -1;
        }
        public void Intersects(ref BoundingRectangle box, out bool result)
        {
            Vector2D[] vertexes = box.Corners();
            Scalar distance;
            GetDistance(ref  vertexes[0], out distance);

            int sign = Math.Sign(distance);
            result = false;
            for (int index = 1; index < vertexes.Length; ++index)
            {
                GetDistance(ref  vertexes[index], out distance);

                if (Math.Sign(distance) != sign)
                {
                    result = true;
                    break;
                }
            }
        }
        public void Intersects(ref BoundingCircle circle, out bool result)
        {
            circle.Intersects(ref this, out result);
        }
        public void Intersects(ref BoundingPolygon polygon, out bool result)
        {
            if (polygon == null) { throw new ArgumentNullException("polygon"); }
            Vector2D[] vertexes = polygon.Vertexes;
            Scalar distance;
            GetDistance(ref  vertexes[0], out distance);

            int sign = Math.Sign(distance);
            result = false;
            for (int index = 1; index < vertexes.Length; ++index)
            {
                GetDistance(ref  vertexes[index], out distance);

                if (Math.Sign(distance) != sign)
                {
                    result = true;
                    break;
                }
            }
        }


        public override string ToString()
        {
            return string.Format("N: {0} D: {1}", Normal, D);
        }
        public override int GetHashCode()
        {
            return Normal.GetHashCode() ^ D.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return obj is Line && Equals((Line)obj);
        }
        public bool Equals(Line other)
        {
            return Equals(ref this, ref other);
        }
        public static bool Equals(Line line1, Line line2)
        {
            return Equals(ref line1, ref line2);
        }
        [CLSCompliant(false)]
        public static bool Equals(ref Line line1, ref Line line2)
        {
            return Vector2D.Equals(ref line1.Normal, ref line2.Normal) && line1.D == line2.D;
        }

        public static bool operator ==(Line line1, Line line2)
        {
            return Equals(ref line1, ref line2);
        }
        public static bool operator !=(Line line1, Line line2)
        {
            return !Equals(ref line1, ref line2);
        }
    }
}
