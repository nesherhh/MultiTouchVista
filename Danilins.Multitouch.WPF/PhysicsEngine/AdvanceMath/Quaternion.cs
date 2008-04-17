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



#region Axiom LGPL License
/*

Since enough of this code is Axioms here is thier License

Axiom Game Engine LibrarY
Copyright (C) 2003  Axiom Project Team

The overall design, and a majority of the core engine and rendering code 
contained Within this library is a derivative of the open source Object Oriented 
Graphics Engine OGRE, Which can be found at http://ogre.sourceforge.net.  
ManY thanks to the OGRE team for maintaining such a high qualitY project.

The math library included in this project, in addition to being a derivative of
the Works of Ogre, also include derivative Work of the free portion of the 
Wild Magic mathematics source code that is distributed With the excellent
book Game Engine Design.
http://www.wild-magic.com/

This library is free softWare; You can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free SoftWare Foundation; either
version 2.1 of the License, or (at Your option) anY later version.

This library is distributed in the hope that it Will be useful,
but WITHOUT ANY WARRANTY; Without even the implied Warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along With this library; if not, Write to the Free SoftWare
Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
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
using System.Xml.Serialization;

namespace AdvanceMath
{
    /// <summary>
    ///		Summary description for Quaternion.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [AdvBrowsableOrder("W,X,Y,Z"), Serializable]
#if !CompactFramework && !WindowsCE && !PocketPC && !XBOX360 
    [System.ComponentModel.TypeConverter(typeof(AdvTypeConverter<Quaternion>))]
#endif
    public struct Quaternion
    {
        #region static fields
        /// <summary>
        ///    An Identity Quaternion.
        /// </summary>
        public static readonly Quaternion Identity = new Quaternion(1, 0, 0, 0);
        /// <summary>
        ///    A Quaternion With all elements set to 0;
        /// </summary>
        public static readonly Quaternion Zero = new Quaternion(0, 0, 0, 0);
        private static readonly int[] next = new int[3] { 1, 2, 0 }; 
        #endregion
        #region Static methods
        public static Quaternion Slerp(Scalar time, Quaternion quatA, Quaternion quatB)
        {
            return Slerp(time, quatA, quatB, false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        /// <param name="quatA"></param>
        /// <param name="quatB"></param>
        /// <param name="useShortestPath"></param>
        /// <returns></returns>
        public static Quaternion Slerp(Scalar time, Quaternion quatA, Quaternion quatB, bool useShortestPath)
        {
            Scalar cos = quatA.Dot(quatB);

            Scalar angle = MathHelper.Acos(cos);

            if (Math.Abs(angle) < MathHelper.Epsilon)
            {
                return quatA;
            }

            Scalar sin = MathHelper.Sin(angle);
            Scalar inverseSin = 1 / sin;
            Scalar coeff0 = MathHelper.Sin((1 - time) * angle) * inverseSin;
            Scalar coeff1 = MathHelper.Sin(time * angle) * inverseSin;

            Quaternion returnvalue;

            if (cos < 0 && useShortestPath)
            {
                coeff0 = -coeff0;
                // taking the complement requires renormalisation
                Quaternion t = coeff0 * quatA + coeff1 * quatB;
                t.Normalize();
                returnvalue = t;
            }
            else
            {
                returnvalue = (coeff0 * quatA + coeff1 * quatB);
            }

            return returnvalue;
        }
        /// <summary>
        /// Creates a Quaternion from a supplied angle and aXis.
        /// </summary>
        /// <param name="angle">Value of an angle in radians.</param>
        /// <param name="aXis">ArbitrarY aXis vector.</param>
        /// <returns></returns>
        public static Quaternion FromAngleAxis(Scalar angle, Vector3D aXis)
        {
            Quaternion quat = new Quaternion();

            Scalar halfAngle = 0.5f * angle;
            Scalar sin = MathHelper.Sin(halfAngle);

            quat.W = MathHelper.Cos(halfAngle);
            quat.X = sin * aXis.X;
            quat.Y = sin * aXis.Y;
            quat.Z = sin * aXis.Z;

            return quat;
        }
        public static Quaternion Squad(Scalar t, Quaternion p, Quaternion a, Quaternion b, Quaternion q)
        {
            return Squad(t, p, a, b, q, false);
        }
        /// <summary>
        ///		Performs spherical quadratic interpolation.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="p"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="q"></param>
        /// <returns></returns>
        public static Quaternion Squad(Scalar t, Quaternion p, Quaternion a, Quaternion b, Quaternion q, bool useShortestPath)
        {
            Scalar slerpT = 2 * t * (1 - t);

            // use spherical linear interpolation
            Quaternion slerpP = Slerp(t, p, q, useShortestPath);
            Quaternion slerpQ = Slerp(t, a, b);

            // run another Slerp on the returnvalues of the first 2, and return the returnvalues
            return Slerp(slerpT, slerpP, slerpQ);
        }
        #endregion
        #region fields
        [AdvBrowsable]
        [XmlAttribute]
        public Scalar X;
        [AdvBrowsable]
        [XmlAttribute]
        public Scalar Y;
        [AdvBrowsable]
        [XmlAttribute]
        public Scalar Z;
        [AdvBrowsable]
        [XmlAttribute]
        public Scalar W;


        #endregion
        #region Constructors

        //		public Quaternion()
        //		{
        //			this.W = 1;
        //		}

        /// <summary>
        ///		Creates a new Quaternion.
        /// </summary>
        [InstanceConstructor("W,X,Y,Z")]
        public Quaternion(Scalar W, Scalar X, Scalar Y, Scalar Z)
        {
            this.W = W;
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        #endregion
        #region Properties


        /// <summary>
        ///		Squared 'length' of this quaternion.
        /// </summary>
        public Scalar Norm
        {
            get
            {
                return X * X + Y * Y + Z * Z + W * W;
            }
        }

        /// <summary>
        ///    Local X-aXis portion of this rotation.
        /// </summary>
        public Vector3D XAxis
        {
            get
            {
                Scalar fTX = 2 * X;
                Scalar fTY = 2 * Y;
                Scalar fTZ = 2 * Z;
                Scalar fTWY = fTY * W;
                Scalar fTWZ = fTZ * W;
                Scalar fTXY = fTY * X;
                Scalar fTXZ = fTZ * X;
                Scalar fTYY = fTY * Y;
                Scalar fTZZ = fTZ * Z;

                Vector3D result;
                result.X = 1 - (fTYY + fTZZ);
                result.Y = fTXY + fTWZ;
                result.Z = fTXZ - fTWY;
                return result;

                //return new Vector3D(1 - (fTYY + fTZZ), fTXY + fTWZ, fTXZ - fTWY);
            }
        }

        /// <summary>
        ///    Local Y-aXis portion of this rotation.
        /// </summary>
        public Vector3D YAxis
        {
            get
            {
                Scalar fTX = 2 * X;
                Scalar fTY = 2 * Y;
                Scalar fTZ = 2 * Z;
                Scalar fTWX = fTX * W;
                Scalar fTWZ = fTZ * W;
                Scalar fTXX = fTX * X;
                Scalar fTXY = fTY * X;
                Scalar fTYZ = fTZ * Y;
                Scalar fTZZ = fTZ * Z;

                Vector3D result;
                result.X = fTXY - fTWZ;
                result.Y = 1 - (fTXX + fTZZ);
                result.Z = fTYZ + fTWX;
                return result;

                //return new Vector3D(fTXY - fTWZ, 1 - (fTXX + fTZZ), fTYZ + fTWX);
            }
        }

        /// <summary>
        ///    Local Z-aXis portion of this rotation.
        /// </summary>
        public Vector3D ZAxis
        {
            get
            {
                Scalar fTX = 2 * X;
                Scalar fTY = 2 * Y;
                Scalar fTZ = 2 * Z;
                Scalar fTWX = fTX * W;
                Scalar fTWY = fTY * W;
                Scalar fTXX = fTX * X;
                Scalar fTXZ = fTZ * X;
                Scalar fTYY = fTY * Y;
                Scalar fTYZ = fTZ * Y;

                Vector3D result;
                result.X = fTXZ + fTWY;
                result.Y = fTYZ - fTWX;
                result.Z = 1 - (fTXX + fTYY);
                return result;

                //return new Vector3D(fTXZ + fTWY, fTYZ - fTWX, 1 - (fTXX + fTYY));
            }
        }
        [XmlIgnore]
        public Scalar PitchInDegrees { get { return MathHelper.ToDegrees(Pitch); } set { Pitch = MathHelper.ToRadians(value); } }
        [XmlIgnore]
        public Scalar YawInDegrees { get { return MathHelper.ToDegrees(Yaw); } set { Yaw = MathHelper.ToRadians(value); } }
        [XmlIgnore]
        public Scalar RollInDegrees { get { return MathHelper.ToDegrees(Roll); } set { Roll = MathHelper.ToRadians(value); } }

        [XmlIgnore]
        public Scalar Pitch
        {
            set
            {
                Scalar pitch, Yaw, roll;
                ToEulerAngles(out pitch, out Yaw, out roll);
                FromEulerAngles(value, Yaw, roll);
            }
            get
            {

                Scalar test = X * Y + Z * W;
                if (Math.Abs(test) > 0.499f) // singularitY at north and south pole
                    return 0f;
                return MathHelper.Atan2(2 * X * W - 2 * Y * Z, 1 - 2 * X * X - 2 * Z * Z);
            }
        }
        [XmlIgnore]
        public Scalar Yaw
        {
            set
            {
                Scalar pitch, Yaw, roll;
                ToEulerAngles(out pitch, out Yaw, out roll);
                FromEulerAngles(pitch, value, roll);
            }
            get
            {
                Scalar test = X * Y + Z * W;
                if (Math.Abs(test) > 0.499f) // singularitY at north and south pole
                    return Math.Sign(test) * 2 * MathHelper.Atan2(X, W);
                return MathHelper.Atan2(2 * Y * W - 2 * X * Z, 1 - 2 * Y * Y - 2 * Z * Z);
            }
        }
        [XmlIgnore]
        public Scalar Roll
        {
            set
            {

                Scalar pitch, Yaw, roll;
                ToEulerAngles(out pitch, out Yaw, out roll);
                FromEulerAngles(pitch, Yaw, value);
            }
            get
            {
                Scalar test = X * Y + Z * W;
                if (Math.Abs(test) > 0.499f) // singularitY at north and south pole
                    return Math.Sign(test) * MathHelper.PiOver2;
                return MathHelper.Asin(2 * test);
            }
        }


        #endregion

        #region Public methods

        #region Euler Angles
        public Vector3D ToEulerAnglesInDegrees()
        {
            Scalar pitch, Yaw, roll;
            ToEulerAngles(out pitch, out Yaw, out roll);
            return new Vector3D(MathHelper.ToDegrees(pitch), MathHelper.ToDegrees(Yaw), MathHelper.ToDegrees(roll));
        }
        public Vector3D ToEulerAngles()
        {
            Scalar pitch, Yaw, roll;
            ToEulerAngles(out pitch, out Yaw, out roll);
            return new Vector3D(pitch, Yaw, roll);
        }
        public void ToEulerAnglesInDegrees(out Scalar pitch, out Scalar Yaw, out Scalar roll)
        {
            ToEulerAngles(out pitch, out Yaw, out roll);
            pitch = MathHelper.ToDegrees(pitch);
            Yaw = MathHelper.ToDegrees(Yaw);
            roll = MathHelper.ToDegrees(roll);
        }
        public void ToEulerAngles(out Scalar pitch, out Scalar Yaw, out Scalar roll)
        {

            Scalar test = X * Y + Z * W;
            if (test > 0.499f)
            { // singularitY at north pole
                Yaw = 2 * MathHelper.Atan2(X, W);
                roll = MathHelper.PiOver2;
                pitch = 0;
            }
            else if (test < -0.499f)
            { // singularitY at south pole
                Yaw = -2 * MathHelper.Atan2(X, W);
                roll = -MathHelper.PiOver2;
                pitch = 0;
            }
            else
            {
                Scalar sqX = X * X;
                Scalar sqY = Y * Y;
                Scalar sqZ = Z * Z;
                Yaw = MathHelper.Atan2(2 * Y * W - 2 * X * Z, 1 - 2 * sqY - 2 * sqZ);
                roll = MathHelper.Asin(2 * test);
                pitch = MathHelper.Atan2(2 * X * W - 2 * Y * Z, 1 - 2 * sqX - 2 * sqZ);
            }

            if (pitch <= Scalar.Epsilon)
                pitch = 0f;
            if (Yaw <= Scalar.Epsilon)
                Yaw = 0f;
            if (roll <= Scalar.Epsilon)
                roll = 0f;
        }
        public static Quaternion FromEulerAnglesInDegrees(Scalar pitch, Scalar Yaw, Scalar roll)
        {
            return FromEulerAngles(MathHelper.ToRadians(pitch), MathHelper.ToRadians(Yaw), MathHelper.ToRadians(roll));
        }

        /// <summary>
        /// Combines the euler angles in the order Yaw, pitch, roll to create a rotation quaternion
        /// </summary>
        /// <param name="pitch"></param>
        /// <param name="Yaw"></param>
        /// <param name="roll"></param>
        /// <returns></returns>
        public static Quaternion FromEulerAngles(Scalar pitch, Scalar Yaw, Scalar roll)
        {
            return Quaternion.FromAngleAxis(Yaw, Vector3D.YAxis)
                * Quaternion.FromAngleAxis(pitch, Vector3D.XAxis)
                * Quaternion.FromAngleAxis(roll, Vector3D.ZAxis);

            /*TODO: Debug
            //Equation from http://WWW.euclideanspace.com/maths/geometrY/rotations/conversions/eulerToQuaternion/indeX.htm
            //heading
			
            Scalar c1 = (Scalar)Math.Cos(Yaw/2);
            Scalar s1 = (Scalar)Math.Sin(Yaw/2);
            //attitude
            Scalar c2 = (Scalar)Math.Cos(roll/2);
            Scalar s2 = (Scalar)Math.Sin(roll/2);
            //bank
            Scalar c3 = (Scalar)Math.Cos(pitch/2);
            Scalar s3 = (Scalar)Math.Sin(pitch/2);
            Scalar c1c2 = c1*c2;
            Scalar s1s2 = s1*s2;

            Scalar W =c1c2*c3 - s1s2*s3;
            Scalar X =c1c2*s3 + s1s2*c3;
            Scalar Y =s1*c2*c3 + c1*s2*s3;
            Scalar Z =c1*s2*c3 - s1*c2*s3;
            return new Quaternion(W,X,Y,Z);*/
        }

        #endregion

        /// <summary>
        /// Performs a Dot Product operation on 2 Quaternions.
        /// </summary>
        /// <param name="quat"></param>
        /// <returns></returns>
        public Scalar Dot(Quaternion quat)
        {
            return this.W * quat.W + this.X * quat.X + this.Y * quat.Y + this.Z * quat.Z;
        }

        /// <summary>
        ///		Normalizes elements of this quaterion to the range [0,1].
        /// </summary>
        public void Normalize()
        {
            Scalar factor = 1 / MathHelper.Sqrt(this.Norm);

            W = W * factor;
            X = X * factor;
            Y = Y * factor;
            Z = Z * factor;
        }

        /// <summary>
        ///    
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="aXis"></param>
        /// <returns></returns>
        public void ToAngleAxis(ref Scalar angle, ref Vector3D aXis)
        {
            // The quaternion representing the rotation is
            //   q = cos(A/2)+sin(A/2)*(X*i+Y*j+Z*k)

            Scalar sqrLength = X * X + Y * Y + Z * Z;

            if (sqrLength > 0)
            {
                angle = 2 * MathHelper.Acos(W);
                Scalar invLength = MathHelper.InvSqrt(sqrLength);
                aXis.X = X * invLength;
                aXis.Y = Y * invLength;
                aXis.Z = Z * invLength;
            }
            else
            {
                angle = 0;
                aXis.X = 1;
                aXis.Y = 0;
                aXis.Z = 0;
            }
        }

        /// <summary>
        /// Gets a 3X3 rotation matriX from this Quaternion.
        /// </summary>
        /// <returns></returns>
        public Matrix3x3 ToRotationMatrix()
        {
            Matrix3x3 rotation = new Matrix3x3();

            Scalar tX = 2 * this.X;
            Scalar tY = 2 * this.Y;
            Scalar tZ = 2 * this.Z;
            Scalar tWX = tX * this.W;
            Scalar tWY = tY * this.W;
            Scalar tWZ = tZ * this.W;
            Scalar tXX = tX * this.X;
            Scalar tXY = tY * this.X;
            Scalar tXZ = tZ * this.X;
            Scalar tYY = tY * this.Y;
            Scalar tYZ = tZ * this.Y;
            Scalar tZZ = tZ * this.Z;

            rotation.m00 = 1 - (tYY + tZZ);
            rotation.m01 = tXY - tWZ;
            rotation.m02 = tXZ + tWY;
            rotation.m10 = tXY + tWZ;
            rotation.m11 = 1 - (tXX + tZZ);
            rotation.m12 = tYZ - tWX;
            rotation.m20 = tXZ - tWY;
            rotation.m21 = tYZ + tWX;
            rotation.m22 = 1 - (tXX + tYY);

            return rotation;
        }

        /// <summary>
        /// Computes the inverse of a Quaternion.
        /// </summary>
        /// <returns></returns>
        public Quaternion Inverse()
        {
            Scalar norm = this.W * this.W + this.X * this.X + this.Y * this.Y + this.Z * this.Z;
            if (norm > 0)
            {
                Scalar inverseNorm = 1 / norm;
                return new Quaternion(this.W * inverseNorm, -this.X * inverseNorm, -this.Y * inverseNorm, -this.Z * inverseNorm);
            }
            else
            {
                // return an invalid returnvalue to flag the error
                return Quaternion.Zero;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="XAxis"></param>
        /// <param name="YAxis"></param>
        /// <param name="ZAxis"></param>
        public void ToAxis(out Vector3D XAxis, out Vector3D YAxis, out Vector3D ZAxis)
        {
            XAxis = new Vector3D();
            YAxis = new Vector3D();
            ZAxis = new Vector3D();

            Matrix3x3 rotation = this.ToRotationMatrix();

            XAxis.X = rotation.m00;
            XAxis.Y = rotation.m10;
            XAxis.Z = rotation.m20;

            YAxis.X = rotation.m01;
            YAxis.Y = rotation.m11;
            YAxis.Z = rotation.m21;

            ZAxis.X = rotation.m02;
            ZAxis.Y = rotation.m12;
            ZAxis.Z = rotation.m22;
        }
#if UNSAFE
        /// <summary>
        /// 
        /// </summary>
        /// <param name="XAxis"></param>
        /// <param name="YAxis"></param>
        /// <param name="ZAxis"></param>
        public void FromAxis(Vector3D XAxis, Vector3D YAxis, Vector3D ZAxis)
        {
            Matrix3x3 rotation = new Matrix3x3();

            rotation.m00 = XAxis.X;
            rotation.m10 = XAxis.Y;
            rotation.m20 = XAxis.Z;

            rotation.m01 = YAxis.X;
            rotation.m11 = YAxis.Y;
            rotation.m21 = YAxis.Z;

            rotation.m02 = ZAxis.X;
            rotation.m12 = ZAxis.Y;
            rotation.m22 = ZAxis.Z;

            // set this quaternions values from the rotation matriX built
            FromRotationMatrix(rotation);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matriX"></param>
        public void FromRotationMatrix(Matrix3x3 matriX)
        {
            // Algorithm in Ken Shoemake's article in 1987 SIGGRAPH course notes
            // article "Quaternion Calculus and Fast Animation".

            Scalar trace = matriX.m00 + matriX.m11 + matriX.m22;

            Scalar root = 0;

            if (trace > 0)
            {
                // |this.W| > 1/2, maY as Well choose this.W > 1/2
                root = MathHelper.Sqrt(trace + 1);  // 2W
                this.W = 0.5f * root;

                root = 0.5f / root;  // 1/(4W)

                this.X = (matriX.m21 - matriX.m12) * root;
                this.Y = (matriX.m02 - matriX.m20) * root;
                this.Z = (matriX.m10 - matriX.m01) * root;
            }
            else
            {
                // |this.W| <= 1/2

                int i = 0;
                if (matriX.m11 > matriX.m00)
                {
                    i = 1;
                }
                if (matriX.m22 > matriX[i, i])
                {
                    i = 2;
                }
                int j = next[i];
                int k = next[j];

                root = MathHelper.Sqrt(matriX[i, i] - matriX[j, j] - matriX[k, k] + 1);

                unsafe
                {
                    fixed (Scalar* apkQuat = &this.X)
                    {
                        apkQuat[i] = 0.5f * root;
                        root = 0.5f / root;

                        this.W = (matriX[k, j] - matriX[j, k]) * root;

                        apkQuat[j] = (matriX[j, i] + matriX[i, j]) * root;
                        apkQuat[k] = (matriX[k, i] + matriX[i, k]) * root;
                    }
                }
            }
        }
#endif
        /// <summary>
        ///		Calculates the logarithm of a Quaternion.
        /// </summary>
        /// <returns></returns>
        public Quaternion Log()
        {
            // BLACKBOX: Learn this
            // If q = cos(A)+sin(A)*(X*i+Y*j+Z*k) Where (X,Y,Z) is unit length, then
            // log(q) = A*(X*i+Y*j+Z*k).  If sin(A) is near Zero, use log(q) =
            // sin(A)*(X*i+Y*j+Z*k) since sin(A)/A has limit 1.

            // start off With a Zero quat
            Quaternion returnvalue = Quaternion.Zero;

            if (Math.Abs(W) < 1)
            {
                Scalar angle = MathHelper.Acos(W);
                Scalar sin = MathHelper.Sin(angle);

                if (Math.Abs(sin) >= MathHelper.Epsilon)
                {
                    Scalar coeff = angle / sin;
                    returnvalue.X = coeff * X;
                    returnvalue.Y = coeff * Y;
                    returnvalue.Z = coeff * Z;
                }
                else
                {
                    returnvalue.X = X;
                    returnvalue.Y = Y;
                    returnvalue.Z = Z;
                }
            }

            return returnvalue;
        }

        /// <summary>
        ///		Calculates the Exponent of a Quaternion.
        /// </summary>
        /// <returns></returns>
        public Quaternion Exp()
        {
            // If q = A*(X*i+Y*j+Z*k) Where (X,Y,Z) is unit length, then
            // eXp(q) = cos(A)+sin(A)*(X*i+Y*j+Z*k).  If sin(A) is near Zero,
            // use eXp(q) = cos(A)+A*(X*i+Y*j+Z*k) since A/sin(A) has limit 1.

            Scalar angle = MathHelper.Sqrt(X * X + Y * Y + Z * Z);
            Scalar sin = MathHelper.Sin(angle);

            // start off With a Zero quat
            Quaternion returnvalue = Quaternion.Zero;

            returnvalue.W = MathHelper.Cos(angle);

            if (Math.Abs(sin) >= MathHelper.Epsilon)
            {
                Scalar coeff = sin / angle;

                returnvalue.X = coeff * X;
                returnvalue.Y = coeff * Y;
                returnvalue.Z = coeff * Z;
            }
            else
            {
                returnvalue.X = X;
                returnvalue.Y = Y;
                returnvalue.Z = Z;
            }

            return returnvalue;
        }

        #endregion
        #region Object overloads

        /// <summary>
        ///		Overrides the Object.ToString() method to provide a teXt representation of 
        ///		a Quaternion.
        /// </summary>
        /// <returns>A string representation of a Quaternion.</returns>
        public override string ToString()
        {
            return string.Format("Quaternion({0}, {1}, {2}, {3})", this.X, this.Y, this.Z, this.W);
        }
        [ParseMethod]
        public static Quaternion Parse(string text)
        {
            string[] vals = text.Replace("Quaternion", "").Trim(' ', '(', '[', '<', ')', ']', '>').Split(',');

            if (vals.Length != 4)
            {
                throw new FormatException(string.Format("Cannot parse the text '{0}' because it does not have 4 parts separated by commas in the form (x,y,z,w) with optional parenthesis.", text));
            }
            else
            {
                try
                {
                    Quaternion returnvalue;
                    returnvalue.X = Scalar.Parse(vals[0].Trim());
                    returnvalue.Y = Scalar.Parse(vals[1].Trim());
                    returnvalue.Z = Scalar.Parse(vals[2].Trim());
                    returnvalue.W = Scalar.Parse(vals[3].Trim());
                    return returnvalue;
                }
                catch (Exception ex)
                {
                    throw new FormatException("The parts of the vectors must be decimal numbers", ex);
                }
            }
        }
        public override int GetHashCode()
        {
            return (int)X ^ (int)Y ^ (int)Z ^ (int)W;
        }
        public override bool Equals(object obj)
        {
            return obj is Quaternion && (Quaternion)obj == this;
        }
        #endregion


        #region operator overloads

        public static Quaternion Multiply(Quaternion left, Quaternion right)
        {
            Quaternion result;
            result.W = left.W * right.W - left.X * right.X - left.Y * right.Y - left.Z * right.Z;
            result.X = left.W * right.X + left.X * right.W + left.Y * right.Z - left.Z * right.Y;
            result.Y = left.W * right.Y + left.Y * right.W + left.Z * right.X - left.X * right.Z;
            result.Z = left.W * right.Z + left.Z * right.W + left.X * right.Y - left.Y * right.X;
            return result;
        }
        public static void Multiply(ref Quaternion left,ref Quaternion right,out Quaternion result)
        {
            Scalar W = left.W * right.W - left.X * right.X - left.Y * right.Y - left.Z * right.Z;
            Scalar X = left.W * right.X + left.X * right.W + left.Y * right.Z - left.Z * right.Y;
            Scalar Y = left.W * right.Y + left.Y * right.W + left.Z * right.X - left.X * right.Z;
            result.Z = left.W * right.Z + left.Z * right.W + left.X * right.Y - left.Y * right.X;

            result.W = W;
            result.X = X;
            result.Y = Y;

        }

        public static Quaternion Multiply(Quaternion left, Scalar scalar)
        {

            Quaternion result;
            result.W = left.W * scalar;
            result.X = left.X * scalar;
            result.Y = left.Y * scalar;
            result.Z = left.Z * scalar;
            return result;
        }
        public static void Multiply(ref Quaternion left,ref Scalar scalar, out Quaternion result)
        {
            result.W = left.W * scalar;
            result.X = left.X * scalar;
            result.Y = left.Y * scalar;
            result.Z = left.Z * scalar;
        }

        public static Quaternion Add(Quaternion left, Quaternion right)
        {
            Quaternion result;
            result.W = left.W + right.W;
            result.X = left.X + right.X;
            result.Y = left.Y + right.Y;
            result.Z = left.Z + right.Z;
            return result;
        }
        public static void Add(ref Quaternion left,ref Quaternion right, out Quaternion result)
        {
            result.W = left.W + right.W;
            result.X = left.X + right.X;
            result.Y = left.Y + right.Y;
            result.Z = left.Z + right.Z;
        }

        public static Quaternion Subtract(Quaternion left, Quaternion right)
        {
            Quaternion result;
            result.W = left.W - right.W;
            result.X = left.X - right.X;
            result.Y = left.Y - right.Y;
            result.Z = left.Z - right.Z;
            return result;
        }
        public static void Subtract(ref Quaternion left,ref Quaternion right, out Quaternion result)
        {
            result.W = left.W - right.W;
            result.X = left.X - right.X;
            result.Y = left.Y - right.Y;
            result.Z = left.Z - right.Z;
        }

        public static Quaternion Negate(Quaternion value)
        {
            Quaternion result;
            result.W = -value.W;
            result.X = -value.X;
            result.Y = -value.Y;
            result.Z = -value.Z;
            return result;
        }
        [CLSCompliant(false)]
        public static void Negate(ref Quaternion value)
        {
            Negate(ref value, out value);
        }
        public static void Negate(ref Quaternion value, out Quaternion result)
        {
            result.W = -value.W;
            result.X = -value.X;
            result.Y = -value.Y;
            result.Z = -value.Z;
        }


        public static Quaternion operator *(Quaternion left, Quaternion right)
        {
            Quaternion result;
            result.W = left.W * right.W - left.X * right.X - left.Y * right.Y - left.Z * right.Z;
            result.X = left.W * right.X + left.X * right.W + left.Y * right.Z - left.Z * right.Y;
            result.Y = left.W * right.Y + left.Y * right.W + left.Z * right.X - left.X * right.Z;
            result.Z = left.W * right.Z + left.Z * right.W + left.X * right.Y - left.Y * right.X;
            return result;
        }

        public static Quaternion operator *(Scalar scalar, Quaternion right)
        {
            Quaternion result;
            result.W = scalar * right.W;
            result.X = scalar * right.X;
            result.Y = scalar * right.Y;
            result.Z = scalar * right.Z;
            return result;
        }
        public static Quaternion operator *(Quaternion left, Scalar scalar)
        {

            Quaternion result;
            result.W = left.W * scalar;
            result.X = left.X * scalar;
            result.Y = left.Y * scalar;
            result.Z = left.Z * scalar;
            return result;
        }

        public static Quaternion operator +(Quaternion left, Quaternion right)
        {
            Quaternion result;
            result.W = left.W + right.W;
            result.X = left.X + right.X;
            result.Y = left.Y + right.Y;
            result.Z = left.Z + right.Z;
            return result;
        }

        public static Quaternion operator -(Quaternion left, Quaternion right)
        {
            Quaternion result;
            result.W = left.W - right.W;
            result.X = left.X - right.X;
            result.Y = left.Y - right.Y;
            result.Z = left.Z - right.Z;
            return result;
        }

        public static Quaternion operator -(Quaternion value)
        {
            Quaternion result;
            result.W = -value.W;
            result.X = -value.X;
            result.Y = -value.Y;
            result.Z = -value.Z;
            return result;
        }

        public static bool operator ==(Quaternion left, Quaternion right)
        {
            return
                left.W == right.W &&
                left.X == right.X &&
                left.Y == right.Y &&
                left.Z == right.Z;
        }

        public static bool operator !=(Quaternion left, Quaternion right)
        {
            return !(left == right);
        }
        #endregion
    }
}
