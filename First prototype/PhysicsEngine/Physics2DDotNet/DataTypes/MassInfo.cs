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
using AdvanceMath.Design;

using System.Runtime.Serialization;
namespace Physics2DDotNet
{

    /// <summary>
    /// This class Stores mass information and Moment of Inertia Together since they are very closly related.
    /// </summary>
#if !CompactFramework && !WindowsCE && !PocketPC && !XBOX360
    [System.ComponentModel.TypeConverter(typeof(AdvTypeConverter<MassInfo>))]
#endif
    [AdvBrowsableOrder("Mass,MomentofInertia"), Serializable]
    public sealed class MassInfo : IDeserializationCallback, IDuplicateable<MassInfo>
    {
        #region static
        private static MassInfo infinite = new MassInfo(Scalar.PositiveInfinity, Scalar.PositiveInfinity);
        /// <summary>
        /// A body with this mass will not be affected by forces or impulse.
        /// </summary>
        /// <remarks>
        /// This is the mass of Chuck Norris.
        /// </remarks>
        public static MassInfo Infinite
        {
            get { return new MassInfo(infinite); }
        }

        public static MassInfo FromCylindricalShell(Scalar mass, Scalar radius)
        {
            return new MassInfo(mass, mass * radius * radius);
        }
        public static MassInfo FromHollowCylinder(Scalar mass, Scalar innerRadius, Scalar outerRadius)
        {
            return new MassInfo(mass, .5f * mass * (innerRadius * innerRadius + outerRadius * outerRadius));
        }
        public static MassInfo FromSolidCylinder(Scalar mass, Scalar radius)
        {
            return new MassInfo(mass, .5f * mass * (radius * radius));
        }
        public static MassInfo FromRectangle(Scalar mass, Scalar length, Scalar width)
        {
            return new MassInfo(mass, (1f / 12f) * mass * (length * length + width * width));
        }
        public static MassInfo FromSquare(Scalar mass, Scalar sideLength)
        {
            return new MassInfo(mass, (1f / 6f) * mass * (sideLength * sideLength));
        }
        public static MassInfo FromPolygon(Vector2D[] vertexes, Scalar mass)
        {
            if (vertexes == null) { throw new ArgumentNullException("vertexes"); }
            if (vertexes.Length == 0) { throw new ArgumentOutOfRangeException("vertexes"); }

            if (vertexes.Length == 1) { return new MassInfo(mass, 0); }

            Scalar denom = 0.0f;
            Scalar numer = 0.0f;

            for (int j = vertexes.Length - 1, i = 0; i < vertexes.Length; j = i, i++)
            {
                Scalar a, b, c;
                Vector2D P0 = vertexes[j];
                Vector2D P1 = vertexes[i];
                Vector2D.Dot(ref P1, ref P1, out a);
                Vector2D.Dot(ref P1, ref P0, out b);
                Vector2D.Dot(ref P0, ref P0, out c);
                a += b + c;
                Vector2D.ZCross(ref P0, ref P1, out b);
                b = Math.Abs(b);
                denom += (b * a);
                numer += b;
            }
            return new MassInfo(mass, (mass * denom) / (numer * 6));
        }

        #endregion
        #region fields
        private Scalar mass;
        private Scalar momentOfInertia;
        [NonSerialized]
        private Scalar massInv;
        [NonSerialized]
        private Scalar momentOfInertiaInv;
        [NonSerialized]
        private Scalar accelerationDueToGravity;
        #endregion
        #region constructors
        public MassInfo() { }
        [InstanceConstructor("Mass,MomentofInertia")]
        public MassInfo(Scalar mass, Scalar momentOfInertia)
        {
            this.MomentOfInertia = momentOfInertia;
            this.Mass = mass;
        }
        private MassInfo(MassInfo copy)
        {
            this.mass = copy.mass;
            this.momentOfInertia = copy.momentOfInertia;
            this.massInv = copy.massInv;
            this.momentOfInertiaInv = copy.momentOfInertiaInv;
            this.accelerationDueToGravity = copy.accelerationDueToGravity;
        }
        #endregion
        #region properties
        [AdvBrowsable]
        public Scalar Mass
        {
            get
            {
                return mass;
            }
            set
            {
                this.mass = value;
                this.massInv = 1 / value;
                this.accelerationDueToGravity = value * PhysicsHelper.GravitationalConstant;
            }
        }
        [AdvBrowsable]
        public Scalar MomentOfInertia
        {
            get
            {
                return momentOfInertia;
            }
            set
            {
                this.momentOfInertia = value;
                this.momentOfInertiaInv = 1 / value;
            }
        }
        public Scalar MassInv
        {
            get
            {
                return massInv;
            }
        }
        public Scalar MomentOfInertiaInv
        {
            get
            {
                return momentOfInertiaInv;
            }
        }
        public Scalar AccelerationDueToGravity
        {
            get
            {
                return accelerationDueToGravity;
            }
        }
        #endregion
        #region Methods
        public MassInfo Duplicate()
        {
            return new MassInfo(this);
        }
        public object Clone()
        {
            return Duplicate();
        }
        void IDeserializationCallback.OnDeserialization(object sender)
        {
            this.massInv = 1 / this.mass;
            this.momentOfInertiaInv = 1 / this.momentOfInertia;
            this.accelerationDueToGravity = this.mass * PhysicsHelper.GravitationalConstant;
        }
        #endregion
    }
}
