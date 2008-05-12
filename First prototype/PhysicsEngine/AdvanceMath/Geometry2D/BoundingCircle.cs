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

    [StructLayout(LayoutKind.Sequential, Size = BoundingCircle.Size), Serializable]
    [AdvBrowsableOrder("Position,Radius")]
#if !CompactFramework && !WindowsCE && !PocketPC && !XBOX360 
    [System.ComponentModel.TypeConverter(typeof(AdvTypeConverter<BoundingCircle>))]
#endif
    public struct BoundingCircle : IEquatable<BoundingCircle>
    {
        public const int Size = Vector2D.Size + sizeof(Scalar);

        public static BoundingCircle FromRectangle(BoundingRectangle rect)
        {
            BoundingCircle result;
            FromRectangle(ref rect, out result);
            return result;
        }
        public static void FromRectangle(ref BoundingRectangle rect, out BoundingCircle result)
        {
            result.Position.X = (rect.Min.X + rect.Max.X) * .5f;
            result.Position.Y = (rect.Min.Y + rect.Max.Y) * .5f;
            Scalar xRadius = (rect.Max.X - rect.Min.X) * .5f;
            Scalar yRadius = (rect.Max.Y - rect.Min.Y) * .5f;
            result.Radius = MathHelper.Sqrt(xRadius * xRadius + yRadius * yRadius);
        }
        public static BoundingCircle FromVectors(Vector2D[] vertexes)
        {
            BoundingCircle result;
            FromVectors(vertexes, out result);
            return result;
        }
        public static void FromVectors(Vector2D[] vertexes, out BoundingCircle result)
        {
            BoundingPolygon.GetCentroid(vertexes, out result.Position);
            result.Radius = -1;
            for (int index = 0; index < vertexes.Length; ++index)
            {
                Scalar distSq;
                Vector2D.DistanceSq(ref result.Position, ref vertexes[index], out distSq);
                if (result.Radius == -1 || (distSq < result.Radius))
                {
                    result.Radius = distSq;
                }
            }
            result.Radius = MathHelper.Sqrt(result.Radius);
        }

        [AdvBrowsable]
        public Vector2D Position;
        [AdvBrowsable]
        public Scalar Radius;
        [InstanceConstructor("Position,Radius")]
        public BoundingCircle(Vector2D position, Scalar radius)
        {
            this.Position = position;
            this.Radius = radius;
        }
        public BoundingCircle(Scalar x, Scalar y, Scalar radius)
        {
            this.Position.X = x;
            this.Position.Y = y;
            this.Radius = radius;
        }

        public Scalar Area
        {
            get
            {
                return MathHelper.Pi * Radius * Radius;
            }
        }
        public Scalar Perimeter
        {
            get
            {
                return MathHelper.TwoPi * Radius;
            }
        }

        public Scalar GetDistance(Vector2D point)
        {
            Scalar result;
            GetDistance(ref point, out result);
            return result;
        }
        public void GetDistance(ref Vector2D point, out Scalar result)
        {
            Vector2D diff;
            Vector2D.Subtract(ref point, ref Position, out diff);
            Vector2D.GetMagnitude(ref  diff, out result);
            result -= Radius;
        }

        public ContainmentType Contains(Vector2D point)
        {
            Scalar distance;
            GetDistance(ref point, out distance);
            return ((distance <= 0) ? (ContainmentType.Contains) : (ContainmentType.Disjoint));
        }
        public void Contains(ref Vector2D point, out ContainmentType result)
        {
            Scalar distance;
            GetDistance(ref point, out distance);
            result = ((distance <= 0) ? (ContainmentType.Contains) : (ContainmentType.Disjoint));
        }

        public ContainmentType Contains(BoundingCircle circle)
        {
            Scalar distance;
            GetDistance(ref circle.Position, out distance);
            if (-distance >= circle.Radius)
            {
                return ContainmentType.Contains;
            }
            else if (distance > circle.Radius)
            {
                return ContainmentType.Disjoint;
            }
            else
            {
                return ContainmentType.Intersects;
            }
        }
        public void Contains(ref BoundingCircle circle, out ContainmentType result)
        {
            Scalar distance;
            GetDistance(ref circle.Position, out distance);
            if (-distance >= circle.Radius)
            {
                result = ContainmentType.Contains;
            }
            else if (distance > circle.Radius)
            {
                result = ContainmentType.Disjoint;
            }
            else
            {
                result = ContainmentType.Intersects;
            }
        }

        public ContainmentType Contains(BoundingRectangle rect)
        {
            ContainmentType result;
            Contains(ref rect, out result);
            return result;
        }
        public void Contains(ref BoundingRectangle rect, out ContainmentType result)
        {
            Scalar mag;
            Vector2D maxDistance,minDistance;
            MathHelper.Sort(rect.Max.X - Position.X, Position.X - rect.Min.X, out maxDistance.X,out minDistance.X);
            MathHelper.Sort(rect.Max.Y - Position.Y, Position.Y - rect.Min.Y, out maxDistance.Y,out minDistance.Y);
            Vector2D.GetMagnitude(ref maxDistance, out mag);
            if (mag <= Radius)
            {
                result = ContainmentType.Contains;
            }
            else 
            {
                Vector2D.GetMagnitude(ref minDistance, out mag);
                if (mag <= Radius)
                {
                    result = ContainmentType.Intersects;
                }
                else
                {
                    result = ContainmentType.Disjoint;
                }
            }
        }

        public ContainmentType Contains(BoundingPolygon polygon)
        {
            ContainmentType result;
            Contains(ref polygon, out result);
            return result;
        }
        public void Contains(ref BoundingPolygon polygon, out ContainmentType result)
        {
            if (polygon == null) { throw new ArgumentNullException("polygon"); }
            Vector2D[] vertexes = polygon.Vertexes;
            result = ContainmentType.Unknown;
            for (int index = 0; index < vertexes.Length && result != ContainmentType.Intersects; ++index)
            {
                ContainmentType con;
                Contains(ref vertexes[index], out con);
                result |= con;
            }
            if (result == ContainmentType.Disjoint)
            {
                bool test;
                polygon.Intersects(ref this, out test);
                if (test)
                {
                    result = ContainmentType.Intersects;
                }
            }
        }

        public Scalar Intersects(Ray ray)
        {
            Scalar result;
            Intersects(ref ray, true, out result);
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
            Intersects(ref circle, out result);
            return result;
        }
        public bool Intersects(BoundingPolygon polygon)
        {
            bool result;
            polygon.Intersects(ref this, out result);
            return result;
        }
        public bool Intersects(LineSegment line)
        {
            bool result;
            Intersects(ref line, out result);
            return result;
        }
        public bool Intersects(Line line)
        {
            bool result;
            Intersects(ref line, out result);
            return result;
        }

        public void Intersects(ref Ray ray, out Scalar result)
        {
            Intersects(ref ray, true, out result);
        }
        public void Intersects(ref Ray ray, bool discardInside, out Scalar result)
        {
            Vector2D rayOriginRelativeToCircle2D;
            Vector2D.Subtract(ref ray.Origin, ref Position, out rayOriginRelativeToCircle2D);
            Scalar radiusSq = this.Radius * this.Radius;
            Scalar MagSq = rayOriginRelativeToCircle2D.MagnitudeSq;

            if ((MagSq <= radiusSq) && !discardInside)
            {
                result = 0;
                return;
            }
            Scalar a = ray.Direction.MagnitudeSq;
            Scalar b = 2 * rayOriginRelativeToCircle2D * ray.Direction;
            Scalar c = MagSq - radiusSq;
            Scalar minus;
            Scalar plus;
            if (MathHelper.TrySolveQuadratic(a, b, c, out plus, out minus))
            {
                if (minus < 0)
                {
                    if (plus > 0)
                    {
                        result = plus;
                        return;
                    }
                }
                else
                {
                    result = minus;
                    return;
                }
            }
            result = -1;
        }
        public void Intersects(ref LineSegment line, out bool result)
        {
            Scalar distance;
            line.GetDistance(ref Position, out distance);
            result = Math.Abs(distance) <= Radius;
        }
        public void Intersects(ref Line line, out bool result)
        {
            Scalar distance;
            Vector2D.Dot(ref line.Normal, ref Position, out distance);
            result = (distance + line.D) <= Radius;
        }
        public void Intersects(ref BoundingRectangle rect, out bool result)
        {
            Vector2D proj;
            Vector2D.Clamp(ref Position,ref rect.Min,ref rect.Max, out proj);
            Scalar distSq;
            Vector2D.DistanceSq(ref Position, ref proj, out distSq);
            result = distSq <= Radius * Radius;
        }
        public void Intersects(ref BoundingCircle circle, out bool result)
        {
            Scalar distSq;
            Vector2D.DistanceSq(ref Position, ref circle.Position, out distSq);
            result = distSq <= (Radius * Radius + circle.Radius * circle.Radius);
        }
        public void Intersects(ref BoundingPolygon polygon, out bool result)
        {
            polygon.Intersects(ref this, out result);
        }



        public override string ToString()
        {
            return string.Format("P: {0} R: {1}", Position, Radius);
        }
        public override int GetHashCode()
        {
            return Position.GetHashCode() ^ Radius.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return obj is BoundingCircle && Equals((BoundingCircle)obj);
        }
        public bool Equals(BoundingCircle other)
        {
            return Equals(ref this, ref other);
        }
        public static bool Equals(BoundingCircle circle1, BoundingCircle circle2)
        {
            return Equals(ref circle1, ref circle2);
        }
        [CLSCompliant(false)]
        public static bool Equals(ref BoundingCircle circle1, ref BoundingCircle circle2)
        {
            return Vector2D.Equals(ref circle1.Position, ref circle2.Position) && circle1.Radius == circle2.Radius;
        }
        public static bool operator ==(BoundingCircle circle1, BoundingCircle circle2)
        {
            return Equals(ref circle1, ref circle2);
        }
        public static bool operator !=(BoundingCircle circle1, BoundingCircle circle2)
        {
            return !Equals(ref circle1, ref circle2);
        }
    }
}