
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
    [StructLayout(LayoutKind.Sequential, Size = Ray.Size)]
#if !CompactFramework && !WindowsCE && !PocketPC && !XBOX360 
    [System.ComponentModel.TypeConverter(typeof(AdvTypeConverter<Ray>))]
#endif
    [AdvBrowsableOrder("Origin,Direction"), Serializable]
    public struct Ray : IEquatable<Ray>
    {
        public const int Size = Vector2D.Size * 2;

        [AdvBrowsable]
        public Vector2D Origin;
        [AdvBrowsable]
        public Vector2D Direction;

        [InstanceConstructor("Origin,Direction")]
        public Ray(Vector2D origin, Vector2D direction)
        {
            this.Origin = origin;
            this.Direction = direction;
        }

        public Scalar Intersects(BoundingRectangle rect)
        {
            Scalar result;
            rect.Intersects(ref this, out result);
            return result;
        }
        public Scalar Intersects(Line line)
        {
            Scalar result;
            line.Intersects(ref this, out result);
            return result;
        }
        public Scalar Intersects(LineSegment line)
        {
            Scalar result;
            line.Intersects(ref this, out result);
            return result;
        }
        public Scalar Intersects(BoundingCircle circle)
        {
            Scalar result;
            circle.Intersects(ref this, out result);
            return result;
        }
        public Scalar Intersects(BoundingPolygon polygon)
        {
            if (polygon == null) { throw new ArgumentNullException("polygon"); }
            Scalar result;
            polygon.Intersects(ref this, out result);
            return result;
        }

        public void Intersects(ref BoundingRectangle rect, out Scalar result)
        {
            rect.Intersects(ref this, out result);
        }
        public void Intersects(ref Line line, out Scalar result)
        {
            line.Intersects(ref this, out result);
        }
        public void Intersects(ref LineSegment line, out Scalar result)
        {
            line.Intersects(ref this, out result);
        }
        public void Intersects(ref BoundingCircle circle, out Scalar result)
        {
            circle.Intersects(ref this, out result);
        }
        public void Intersects(ref BoundingPolygon polygon, out Scalar result)
        {
            if (polygon == null) { throw new ArgumentNullException("polygon"); }
            polygon.Intersects(ref this, out result);
        }

        public override string ToString()
        {
            return string.Format("O: {0} D: {1}", Origin, Direction);
        }
        public override int GetHashCode()
        {
            return Origin.GetHashCode() ^ Direction.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return obj is Ray && Equals((Ray)obj);
        }
        public bool Equals(Ray other)
        {
            return Equals(ref this, ref other);
        }
        public static bool Equals(Ray ray1, Ray ray2)
        {
            return Equals(ref ray1, ref ray2);
        }
        [CLSCompliant(false)]
        public static bool Equals(ref Ray ray1, ref Ray ray2)
        {
            return Vector2D.Equals(ref ray1.Origin, ref ray2.Origin) && Vector2D.Equals(ref ray1.Direction, ref ray2.Direction);
        }

        public static bool operator ==(Ray ray1, Ray ray2)
        {
            return Equals(ref ray1, ref ray2);
        }
        public static bool operator !=(Ray ray1, Ray ray2)
        {
            return !Equals(ref ray1, ref ray2);
        }
    }
}