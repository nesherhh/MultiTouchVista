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
using System.Xml.Serialization;

namespace AdvanceMath
{

    /// <summary>
    /// This is the Vector Class.
    /// </summary>
    /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29"/></remarks>
    [StructLayout(LayoutKind.Sequential, Size = Vector2D.Size)]
    [AdvBrowsableOrder("X,Y"), Serializable]
#if !CompactFramework && !WindowsCE && !PocketPC && !XBOX360 
    [System.ComponentModel.TypeConverter(typeof(AdvTypeConverter<Vector2D>))]
#endif
    public struct Vector2D : IVector<Vector2D>
    {
        #region const fields
        /// <summary>
        /// The number of Scalar values in the class.
        /// </summary>
        public const int Count = 2;
        /// <summary>
        /// The Size of the class in bytes;
        /// </summary>
        public const int Size = sizeof(Scalar) * Count;
        #endregion
        #region readonly fields
        /// <summary>
        /// Vector2D(0,0)
        /// </summary>
        public static readonly Vector2D Zero = new Vector2D();
        /// <summary>
        /// Vector2D(1,0)
        /// </summary>
        public static readonly Vector2D XAxis = new Vector2D(1, 0);
        /// <summary>
        /// Vector2D(0,1)
        /// </summary>
        public static readonly Vector2D YAxis = new Vector2D(0, 1);
        /// <summary>
        /// Vector2D(0.707...,0.707...)
        /// </summary>
        public static readonly Vector2D XYAxis = new Vector2D((Scalar)0.7071067811865475244008443621052, (Scalar)0.7071067811865475244008443621052);


        private static readonly string FormatString = MatrixHelper.CreateVectorFormatString(Count);
        private readonly static string FormatableString = MatrixHelper.CreateVectorFormatableString(Count);
        #endregion
        #region static methods
        public static void Copy(ref Vector2D vector, Scalar[] destArray)
        {
            Copy(ref vector, destArray, 0);
        }
        public static void Copy(ref Vector2D vector, Scalar[] destArray, int index)
        {
            ThrowHelper.CheckCopy(destArray, index, Count);

            destArray[index] = vector.X;
            destArray[++index] = vector.Y;
        }
        public static void Copy(Scalar[] sourceArray, out Vector2D result)
        {
            Copy(sourceArray, 0, out result);
        }
        public static void Copy(Scalar[] sourceArray, int index, out Vector2D result)
        {
            ThrowHelper.CheckCopy(sourceArray, index, Count);

            result.X = sourceArray[index];
            result.Y = sourceArray[++index];
        }
        public static void Copy(ref Vector4D source, out Vector2D dest)
        {
            dest.X = source.X;
            dest.Y = source.Y;
        }
        public static void Copy(ref Vector3D source, out Vector2D dest)
        {
            dest.X = source.X;
            dest.Y = source.Y;
        }

        /// <summary>
        /// Binds a value to 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        /// <returns></returns>
        public static Vector2D Clamp(Vector2D value, Vector2D min, Vector2D max)
        {
            Vector2D result;
            MathHelper.Clamp(ref value.X, ref  min.X, ref  max.X, out result.X);
            MathHelper.Clamp(ref value.Y, ref  min.Y, ref  max.Y, out result.Y);
            return result;
        }
        public static void Clamp(ref Vector2D value, ref Vector2D min, ref Vector2D max, out Vector2D result)
        {
            MathHelper.Clamp(ref value.X, ref  min.X, ref  max.X, out result.X);
            MathHelper.Clamp(ref value.Y, ref  min.Y, ref  max.Y, out result.Y);
        }

        public static Vector2D Lerp(Vector2D left, Vector2D right, Scalar amount)
        {
            Vector2D result;
            Lerp(ref left, ref right, ref amount, out result);
            return result;
        }
        public static void Lerp(ref Vector2D left, ref Vector2D right, ref Scalar amount, out Vector2D result)
        {
            result.X = (right.X - left.X) * amount + left.X;
            result.Y = (right.Y - left.Y) * amount + left.Y;
        }
        public static Vector2D Lerp(Vector2D left, Vector2D right, Vector2D amount)
        {
            Vector2D result;
            Lerp(ref left, ref right, ref amount, out result);
            return result;
        }
        public static void Lerp(ref Vector2D left, ref Vector2D right, ref Vector2D amount, out Vector2D result)
        {
            result.X = (right.X - left.X) * amount.X + left.X;
            result.Y = (right.Y - left.Y) * amount.Y + left.Y;
        }

        public static Scalar Distance(Vector2D left, Vector2D right)
        {
            Scalar x, y;
            x = left.X - right.X;
            y = left.Y - right.Y;
            return MathHelper.Sqrt(x * x + y * y);
        }
        public static void Distance(ref Vector2D left, ref Vector2D right, out Scalar result)
        {
            Scalar x, y;
            x = left.X - right.X;
            y = left.Y - right.Y;
            result = MathHelper.Sqrt(x * x + y * y);
        }
        public static Scalar DistanceSq(Vector2D left, Vector2D right)
        {
            Scalar x, y;
            x = left.X - right.X;
            y = left.Y - right.Y;
            return x * x + y * y;
        }
        public static void DistanceSq(ref Vector2D left, ref Vector2D right, out Scalar result)
        {
            Scalar x, y;
            x = left.X - right.X;
            y = left.Y - right.Y;
            result = x * x + y * y;
        }
        /// <summary>
        /// Creates a Vector2D With the given length (<see cref="Magnitude"/>) and the given <see cref="Angle"/>.
        /// </summary>
        /// <param name="length">The length (<see cref="Magnitude"/>) of the Vector2D to be created</param>
        /// <param name="radianAngle">The angle of the from the (<see cref="XAxis"/>) in Radians</param>
        /// <returns>a Vector2D With the given length and angle.</returns>
        /// <remarks>
        /// <code>FromLengthAndAngle(1,Math.PI/2)</code>
        ///  would create a Vector2D equil to 
        ///  <code>new Vector2D(0,1)</code>. 
        ///  And <code>FromLengthAndAngle(1,0)</code>
        ///  would create a Vector2D equil to 
        ///  <code>new Vector2D(1,0)</code>.
        /// </remarks>
        public static Vector2D FromLengthAndAngle(Scalar length, Scalar radianAngle)
        {
            Vector2D result;
            result.X = length * MathHelper.Cos(radianAngle);
            result.Y = length * MathHelper.Sin(radianAngle);
            return result;
        }
        public static void FromLengthAndAngle(ref Scalar length, ref  Scalar radianAngle, out Vector2D result)
        {
            result.X = length * MathHelper.Cos(radianAngle);
            result.Y = length * MathHelper.Sin(radianAngle);
        }
        /// <summary>
        /// Rotates a Vector2D.
        /// </summary>
        /// <param name="radianAngle">The <see cref="Angle"/> in radians of the amount it is to be rotated.</param>
        /// <param name="source">The Vector2D to be Rotated.</param>
        /// <returns>The Rotated Vector2D</returns>
        public static Vector2D Rotate(Scalar radianAngle, Vector2D source)
        {
            Scalar negradianAngle = -radianAngle;
            Scalar cos = MathHelper.Cos(negradianAngle);
            Scalar sin = MathHelper.Sin(negradianAngle);
            Vector2D result;
            result.X = source.X * cos + source.Y * sin;
            result.Y = source.Y * cos - source.X * sin;
            return result;
        }
        public static void Rotate(ref Scalar radianAngle, ref Vector2D source, out Vector2D result)
        {
            Scalar negradianAngle = -radianAngle;
            Scalar cos = MathHelper.Cos(negradianAngle);
            Scalar sin = MathHelper.Sin(negradianAngle);
            result.X = source.X * cos + source.Y * sin;
            result.Y = source.Y * cos - source.X * sin;
        }
        /// <summary>
        /// Sets the <see cref="Angle"/> of a Vector2D without changing the <see cref="Magnitude"/>.
        /// </summary>
        /// <param name="source">The Vector2D to have its Angle set.</param>
        /// <param name="radianAngle">The angle of the from the (<see cref="XAxis"/>) in Radians</param>
        /// <returns>A Vector2D with a new Angle.</returns>
        public static Vector2D SetAngle(Vector2D source, Scalar radianAngle)
        {
            Scalar magnitude;
            GetMagnitude(ref source, out magnitude);
            Vector2D result;
            result.X = magnitude * MathHelper.Cos(radianAngle);
            result.Y = magnitude * MathHelper.Sin(radianAngle);
            return result;
        }
        public static void SetAngle(ref Vector2D source, ref Scalar radianAngle, out Vector2D result)
        {
            Scalar magnitude;
            GetMagnitude(ref source, out magnitude);
            result.X = magnitude * MathHelper.Cos(radianAngle);
            result.Y = magnitude * MathHelper.Sin(radianAngle);
        }
        /// <summary>
        /// Determines the current <see cref="Angle"/> in radians of the Vector2D and Returns it.
        /// </summary>
        /// <param name="source">The Vector2D of whos angle is to be Determined.</param>
        /// <returns>The <see cref="Angle"/> in radians of the Vector2D.</returns>
        public static Scalar GetAngle(Vector2D source)
        {
            Scalar result = MathHelper.Atan2(source.Y, source.X);
            if (result < 0) { result += MathHelper.TwoPi; }
            return result;
        }
        public static void GetAngle(ref Vector2D source, out Scalar result)
        {
            result = MathHelper.Atan2(source.Y, source.X);
            if (result < 0) { result += MathHelper.TwoPi; }
        }
        /// <summary>
        /// Adds 2 Vectors2Ds.
        /// </summary>
        /// <param name="left">The left Vector2D operand.</param>
        /// <param name="right">The right Vector2D operand.</param>
        /// <returns>The Sum of the 2 Vector2Ds.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Vector_addition_and_subtraction"/></remarks>
        public static Vector2D Add(Vector2D left, Vector2D right)
        {
            Vector2D result;
            result.X = left.X + right.X;
            result.Y = left.Y + right.Y;
            return result;
        }
        public static void Add(ref Vector2D left, ref  Vector2D right, out Vector2D result)
        {
            result.X = left.X + right.X;
            result.Y = left.Y + right.Y;
        }
        /// <summary>
        /// Subtracts 2 Vector2Ds.
        /// </summary>
        /// <param name="left">The left Vector2D operand.</param>
        /// <param name="right">The right Vector2D operand.</param>
        /// <returns>The Difference of the 2 Vector2Ds.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Vector_addition_and_subtraction"/></remarks>
        public static Vector2D Subtract(Vector2D left, Vector2D right)
        {
            Vector2D result;
            result.X = left.X - right.X;
            result.Y = left.Y - right.Y;
            return result;
        }
        public static void Subtract(ref Vector2D left, ref  Vector2D right, out Vector2D result)
        {
            result.X = left.X - right.X;
            result.Y = left.Y - right.Y;
        }
        /// <summary>
        /// Uses a matrix multiplication to Transform the vector.
        /// </summary>
        /// <param name="matrix">The Transformation matrix</param>
        /// <param name="source">The Vector to be transformed</param>
        /// <returns>The transformed vector.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Transformation_matrix#Affine_transformations"/></remarks>
        public static Vector2D Transform(Matrix3x3 matrix, Vector2D source)
        {
            Scalar inverseZ = 1 / (source.X * matrix.m20 + source.Y * matrix.m21 + matrix.m22);
            Vector2D result;
            result.X = (source.X * matrix.m00 + source.Y * matrix.m01 + matrix.m02) * inverseZ;
            result.Y = (source.X * matrix.m10 + source.Y * matrix.m11 + matrix.m12) * inverseZ;
            return result;
        }
        public static void Transform(ref Matrix3x3 matrix, ref Vector2D source, out Vector2D result)
        {
            Scalar X = source.X;
            Scalar inverseZ = 1 / (X * matrix.m20 + source.Y * matrix.m21 + matrix.m22);
            result.X = (X * matrix.m00 + source.Y * matrix.m01 + matrix.m02) * inverseZ;
            result.Y = (X * matrix.m10 + source.Y * matrix.m11 + matrix.m12) * inverseZ;
        }
        public static Vector2D TransformNormal(Matrix3x3 matrix, Vector2D source)
        {
            Vector2D result;
            result.X = (source.X * matrix.m00 + source.Y * matrix.m01);
            result.Y = (source.X * matrix.m10 + source.Y * matrix.m11);
            return result;
        }
        public static void TransformNormal(ref Matrix3x3 matrix, ref Vector2D source, out Vector2D result)
        {
            Scalar X = source.X;
            result.X = (X * matrix.m00 + source.Y * matrix.m01);
            result.Y = (X * matrix.m10 + source.Y * matrix.m11);
        }
        /// <summary>
        /// Uses a matrix multiplication to Transform the vector.
        /// </summary>
        /// <param name="matrix">The Transformation matrix</param>
        /// <param name="source">The Vector to be transformed</param>
        /// <returns>The transformed vector.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Transformation_matrix#Affine_transformations"/></remarks>
        public static Vector2D Transform(Matrix2x3 matrix, Vector2D source)
        {
            Vector2D result;
            result.X = (source.X * matrix.m00 + source.Y * matrix.m01 + matrix.m02);
            result.Y = (source.X * matrix.m10 + source.Y * matrix.m11 + matrix.m12);
            return result;
        }
        public static void Transform(ref Matrix2x3 matrix, ref Vector2D source, out Vector2D result)
        {
            Scalar X = source.X;
            result.X = (X * matrix.m00 + source.Y * matrix.m01 + matrix.m02);
            result.Y = (X * matrix.m10 + source.Y * matrix.m11 + matrix.m12);
        }
        public static Vector2D TransformNormal(Matrix2x3 matrix, Vector2D source)
        {
            Vector2D result;
            result.X = (source.X * matrix.m00 + source.Y * matrix.m01);
            result.Y = (source.X * matrix.m10 + source.Y * matrix.m11);
            return result;
        }
        public static void TransformNormal(ref Matrix2x3 matrix, ref Vector2D source, out Vector2D result)
        {
            Scalar X = source.X;
            result.X = (X * matrix.m00 + source.Y * matrix.m01);
            result.Y = (X * matrix.m10 + source.Y * matrix.m11);
        }
        /// <summary>
        /// Uses a matrix multiplication to Transform the vector.
        /// </summary>
        /// <param name="matrix">The rotation matrix</param>
        /// <param name="source">The Vector to be transformed</param>
        /// <returns>The transformed vector.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Transformation_matrix#Rotation"/></remarks>
        public static Vector2D Transform(Matrix2x2 matrix, Vector2D source)
        {
            Vector2D result;
            result.X = (source.X * matrix.m00 + source.Y * matrix.m01);
            result.Y = (source.X * matrix.m10 + source.Y * matrix.m11);
            return result;
        }
        public static void Transform(ref Matrix2x2 matrix, ref Vector2D source, out Vector2D result)
        {
            Scalar X = source.X;
            result.X = (X * matrix.m00 + source.Y * matrix.m01);
            result.Y = (X * matrix.m10 + source.Y * matrix.m11);
        }
        /// <summary>
        /// Does Scaler Multiplication on a Vector2D.
        /// </summary>
        /// <param name="scalar">The scalar value that will multiply the Vector2D.</param>
        /// <param name="source">The Vector2D to be multiplied.</param>
        /// <returns>The Product of the Scaler Multiplication.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Scalar_multiplication"/></remarks>
        public static Vector2D Multiply(Vector2D source, Scalar scalar)
        {
            Vector2D result;
            result.X = source.X * scalar;
            result.Y = source.Y * scalar;
            return result;
        }
        public static void Multiply(ref Vector2D source, ref  Scalar scalar, out Vector2D result)
        {
            result.X = source.X * scalar;
            result.Y = source.Y * scalar;
        }

        public static Vector2D Multiply(Scalar scalar, Vector2D source)
        {
            Vector2D result;
            result.X = scalar * source.X;
            result.Y = scalar * source.Y;
            return result;
        }
        public static void Multiply(ref Scalar scalar, ref  Vector2D source, out Vector2D result)
        {
            result.X = scalar * source.X;
            result.Y = scalar * source.Y;
        }
        /// <summary>
        /// Does a Dot Operation Also know as an Inner Product.
        /// </summary>
        /// <param name="left">The left Vector2D operand.</param>
        /// <param name="right">The right Vector2D operand.</param>
        /// <returns>The Dot Product (Inner Product).</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Dot_product"/></remarks>
        public static Scalar Dot(Vector2D left, Vector2D right)
        {
            return left.Y * right.Y + left.X * right.X;
        }
        public static void Dot(ref Vector2D left, ref Vector2D right, out Scalar result)
        {
            result = left.X * right.X + left.Y * right.Y;
        }
        /// <summary>
        /// Does a "2D" Cross Product also know as an Outer Product.
        /// </summary>
        /// <param name="left">The left Vector2D operand.</param>
        /// <param name="right">The right Vector2D operand.</param>
        /// <returns>The Z value of the resulting vector.</returns>
        /// <remarks>
        /// This 2D Cross Product is using a cheat. Since the Cross product (in 3D space) 
        /// always generates a vector perpendicular (orthogonal) to the 2 vectors used as 
        /// arguments. The cheat is that the only vector that can be perpendicular to two 
        /// vectors in the XY Plane will parallel to the Z Axis. Since any vector that is 
        /// parallel to the Z Axis will have zeros in both the X and Y Fields I can represent
        /// the cross product of 2 vectors in the XY plane as single scalar: Z. Also the 
        /// Cross Product of and Vector on the XY plan and that of one ont on the Z Axis 
        /// will result in a vector on the XY Plane. So the ZCross Methods were well thought
        /// out and can be trusted.
        /// <seealso href="http://en.wikipedia.org/wiki/Cross_product"/>
        /// </remarks>
        public static Scalar ZCross(Vector2D left, Vector2D right)
        {
            return left.X * right.Y - left.Y * right.X;
        }
        public static void ZCross(ref Vector2D left, ref Vector2D right, out Scalar result)
        {
            result = left.X * right.Y - left.Y * right.X;
        }
        /// <summary>
        /// Does a "2D" Cross Product also know as an Outer Product.
        /// </summary>
        /// <param name="leftZ">The Z value of the left vector operand.</param>
        /// <param name="right">The right Vector2D operand.</param>
        /// <returns>The Vector2D that fully represents the resulting vector.</returns>
        /// <remarks>
        /// This 2D Cross Product is using a cheat. Since the Cross product (in 3D space) 
        /// always generates a vector perpendicular (orthogonal) to the 2 vectors used as 
        /// arguments. The cheat is that the only vector that can be perpendicular to two 
        /// vectors in the XY Plane will parallel to the Z Axis. Since any vector that is 
        /// parallel to the Z Axis will have zeros in both the X and Y Fields I can represent
        /// the cross product of 2 vectors in the XY plane as single scalar: Z. Also the 
        /// Cross Product of and Vector on the XY plan and that of one ont on the Z Axis 
        /// will result in a vector on the XY Plane. So the ZCross Methods were well thought
        /// out and can be trusted.
        /// <seealso href="http://en.wikipedia.org/wiki/Cross_product"/>
        /// </remarks>
        public static Vector2D ZCross(Scalar leftZ, Vector2D right)
        {
            Vector2D result;
            result.X = -leftZ * right.Y;
            result.Y = leftZ * right.X;
            return result;
        }
        public static void ZCross(ref Scalar leftZ, ref Vector2D right, out Vector2D result)
        {
            Scalar rightX = right.X;
            result.X = -leftZ * right.Y;
            result.Y = leftZ * rightX;
        }
        /// <summary>
        /// Does a "2D" Cross Product also know as an Outer Product.
        /// </summary>
        /// <param name="left">The left Vector2D operand.</param>
        /// <param name="rightZ">The Z value of the right vector operand.</param>
        /// <returns>The Vector2D that fully represents the resulting vector.</returns>
        /// <remarks>
        /// This 2D Cross Product is using a cheat. Since the Cross product (in 3D space) 
        /// always generates a vector perpendicular (orthogonal) to the 2 vectors used as 
        /// arguments. The cheat is that the only vector that can be perpendicular to two 
        /// vectors in the XY Plane will parallel to the Z Axis. Since any vector that is 
        /// parallel to the Z Axis will have zeros in both the X and Y Fields I can represent
        /// the cross product of 2 vectors in the XY plane as single scalar: Z. Also the 
        /// Cross Product of and Vector on the XY plan and that of one ont on the Z Axis 
        /// will result in a vector on the XY Plane. So the ZCross Methods were well thought
        /// out and can be trusted.
        /// <seealso href="http://en.wikipedia.org/wiki/Cross_product"/>
        /// </remarks>
        public static Vector2D ZCross(Vector2D left, Scalar rightZ)
        {
            Vector2D result;
            result.X = left.Y * rightZ;
            result.Y = -left.X * rightZ;
            return result;
        }
        public static void ZCross(ref Vector2D left, ref Scalar rightZ, out Vector2D result)
        {
            Scalar leftX = left.X;
            result.X = left.Y * rightZ;
            result.Y = -leftX * rightZ;
        }
        /// <summary>
        /// Gets the Squared <see cref="Magnitude"/> of the Vector2D that is passed.
        /// </summary>
        /// <param name="source">The Vector2D whos Squared Magnitude is te be returned.</param>
        /// <returns>The Squared Magnitude.</returns>
        public static Scalar GetMagnitudeSq(Vector2D source)
        {
            return source.X * source.X + source.Y * source.Y;
        }
        public static void GetMagnitudeSq(ref Vector2D source, out Scalar result)
        {
            result = source.X * source.X + source.Y * source.Y;
        }
        /// <summary>
        /// Gets the <see cref="Magnitude"/> of the Vector2D that is passed.
        /// </summary>
        /// <param name="source">The Vector2D whos Magnitude is te be returned.</param>
        /// <returns>The Magnitude.</returns>
        public static Scalar GetMagnitude(Vector2D source)
        {
            return MathHelper.Sqrt(source.X * source.X + source.Y * source.Y);
        }
        public static void GetMagnitude(ref Vector2D source, out Scalar result)
        {
            result = MathHelper.Sqrt(source.X * source.X + source.Y * source.Y);
        }
        /// <summary>
        /// Sets the <see cref="Magnitude"/> of a Vector2D without changing the  <see cref="Angle"/>.
        /// </summary>
        /// <param name="source">The Vector2D whose Magnitude is to be changed.</param>
        /// <param name="magnitude">The Magnitude.</param>
        /// <returns>A Vector2D with the new Magnitude</returns>
        public static Vector2D SetMagnitude(Vector2D source, Scalar magnitude)
        {
            Vector2D result;
            SetMagnitude(ref source, ref magnitude, out result);
            return result;
        }
        public static void SetMagnitude(ref Vector2D source, ref Scalar magnitude, out Vector2D result)
        {
            if (magnitude == 0) { result = Zero; return; }
            Scalar oldmagnitude = MathHelper.Sqrt(source.X * source.X + source.Y * source.Y);
            if (oldmagnitude == 0) { result = Zero; return; }
            oldmagnitude = (magnitude / oldmagnitude);
            result.X = source.X * oldmagnitude;
            result.Y = source.Y * oldmagnitude;
        }
        /// <summary>
        /// Negates a Vector2D.
        /// </summary>
        /// <param name="source">The Vector2D to be Negated.</param>
        /// <returns>The Negated Vector2D.</returns>
        public static Vector2D Negate(Vector2D source)
        {
            Vector2D result;
            result.X = -source.X;
            result.Y = -source.Y;
            return result;
        }
        [CLSCompliant(false)]
        public static void Negate(ref Vector2D source)
        {
            Negate(ref source, out source);
        }
        public static void Negate(ref Vector2D source, out Vector2D result)
        {
            result.X = -source.X;
            result.Y = -source.Y;
        }
        /// <summary>
        /// This returns the Normalized Vector2D that is passed. This is also known as a Unit Vector.
        /// </summary>
        /// <param name="source">The Vector2D to be Normalized.</param>
        /// <returns>The Normalized Vector2D. (Unit Vector)</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Unit_vector"/></remarks>
        public static Vector2D Normalize(Vector2D source)
        {
            Scalar oldmagnitude;
            GetMagnitude(ref source, out oldmagnitude);
            if (oldmagnitude == 0) { return Vector2D.Zero; }
            oldmagnitude = (1 / oldmagnitude);
            Vector2D result;
            result.X = source.X * oldmagnitude;
            result.Y = source.Y * oldmagnitude;
            return result;
        }
        public static void Normalize(ref Vector2D source, out Vector2D result)
        {
            Scalar oldmagnitude;
            GetMagnitude(ref source, out oldmagnitude);
            if (oldmagnitude == 0) { result = Zero; return; }
            oldmagnitude = (1 / oldmagnitude);
            result.X = source.X * oldmagnitude;
            result.Y = source.Y * oldmagnitude;
        }
        [CLSCompliant(false)]
        public static void Normalize(ref Vector2D source)
        {
            Normalize(ref source, out source);
        }
        /// <summary>
        /// This returns the Normalized Vector2D that is passed. This is also known as a Unit Vector.
        /// </summary>
        /// <param name="source">The Vector2D to be Normalized.</param>
        /// <param name="magnitude">the magitude of the Vector2D passed</param>
        /// <returns>The Normalized Vector2D. (Unit Vector)</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Unit_vector"/></remarks>
        public static Vector2D Normalize(Vector2D source, out Scalar magnitude)
        {
            Vector2D result;
            Normalize(ref source, out magnitude, out result);
            return result;
        }
        public static void Normalize(ref Vector2D source, out Scalar magnitude, out Vector2D result)
        {
            GetMagnitude(ref source, out magnitude);
            if (magnitude > 0)
            {
                Scalar magnitudeInv = (1 / magnitude);
                result.X = source.X * magnitudeInv;
                result.Y = source.Y * magnitudeInv;
            }
            else
            {
                result = Zero;
            }
        }
        /// <summary>
        /// Thie Projects the left Vector2D onto the Right Vector2D.
        /// </summary>
        /// <param name="left">The left Vector2D operand.</param>
        /// <param name="right">The right Vector2D operand.</param>
        /// <returns>The Projected Vector2D.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Projection_%28linear_algebra%29"/></remarks>
        public static Vector2D Project(Vector2D left, Vector2D right)
        {
            Vector2D result;
            Project(ref left, ref right, out result);
            return result;
        }
        public static void Project(ref Vector2D left, ref Vector2D right, out Vector2D result)
        {
            Scalar tmp, magsq;
            Dot(ref left, ref  right, out tmp);
            GetMagnitudeSq(ref right, out magsq);
            tmp /= magsq;
            Multiply(ref right, ref tmp, out result);
        }
        /// <summary>
        /// Gets a Vector2D that is perpendicular(orthogonal) to the passed Vector2D while staying on the XY Plane.
        /// </summary>
        /// <param name="source">The Vector2D whose perpendicular(orthogonal) is to be determined.</param>
        /// <returns>An perpendicular(orthogonal) Vector2D using the Right Hand Rule</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Right-hand_rule"/></remarks>
        public static Vector2D GetRightHandNormal(Vector2D source)
        {
            Vector2D result;
            result.X = -source.Y;
            result.Y = source.X;
            return result;
        }
        public static void GetRightHandNormal(ref Vector2D source, out Vector2D result)
        {
            Scalar sourceX = source.X;
            result.X = -source.Y;
            result.Y = sourceX;
        }
        /// <summary>
        /// Gets a Vector2D that is perpendicular(orthogonal) to the passed Vector2D while staying on the XY Plane.
        /// </summary>
        /// <param name="source">The Vector2D whose perpendicular(orthogonal) is to be determined.</param>
        /// <returns>An perpendicular(orthogonal) Vector2D using the Left Hand Rule (opposite of the Right hand Rule)</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Right-hand_rule#Left-hand_rule"/></remarks>
        public static Vector2D GetLeftHandNormal(Vector2D source)
        {
            Vector2D result;
            result.X = source.Y;
            result.Y = -source.X;
            return result;
        }
        public static void GetLeftHandNormal(ref Vector2D source, out Vector2D result)
        {
            Scalar sourceX = source.X;
            result.X = source.Y;
            result.Y = -sourceX;
        }

        public static Vector2D Hermite(Vector2D value1, Vector2D tangent1, Vector2D value2, Vector2D tangent2, Scalar amount)
        {
            Vector2D result;
            Hermite(ref value1, ref tangent1, ref value2, ref tangent2, amount, out result);
            return result;
        }
        public static void Hermite(ref  Vector2D value1, ref Vector2D tangent1, ref Vector2D value2, ref Vector2D tangent2, Scalar amount, out Vector2D result)
        {
            Scalar h1, h2, h3, h4;
            MathHelper.HermiteHelper(amount, out h1, out h2, out h3, out h4);
            result.X = h1 * value1.X + h2 * value2.X + h3 * tangent1.X + h4 * tangent2.X;
            result.Y = h1 * value1.Y + h2 * value2.Y + h3 * tangent1.Y + h4 * tangent2.Y;
        }

        public static Vector2D CatmullRom(Vector2D value1, Vector2D value2, Vector2D value3, Vector2D value4, Scalar amount)
        {
            Vector2D result;
            CatmullRom(ref value1, ref value2, ref value3, ref value4, amount, out result);
            return result;
        }
        public static void CatmullRom(ref Vector2D value1, ref Vector2D value2, ref Vector2D value3, ref Vector2D value4, Scalar amount, out Vector2D result)
        {
            Scalar amountSq = amount * amount;
            Scalar amountCu = amountSq * amount;
            result.X =
                0.5f * ((2 * value2.X) +
                (-value1.X + value3.X) * amount +
                (2 * value1.X - 5 * value2.X + 4 * value3.X - value4.X) * amountSq +
                (-value1.X + 3 * value2.X - 3 * value3.X + value4.X) * amountCu);
            result.Y =
                0.5f * ((2 * value2.Y) +
                (-value1.Y + value3.Y) * amount +
                (2 * value1.Y - 5 * value2.Y + 4 * value3.Y - value4.Y) * amountSq +
                (-value1.Y + 3 * value2.Y - 3 * value3.Y + value4.Y) * amountCu);
        }

        public static Vector2D Max(Vector2D value1, Vector2D value2)
        {
            Vector2D result;
            Max(ref value1, ref value2, out result);
            return result;
        }
        public static void Max(ref Vector2D value1, ref Vector2D value2, out Vector2D result)
        {
            result.X = (value1.X < value2.X) ? (value2.X) : (value1.X);
            result.Y = (value1.Y < value2.Y) ? (value2.Y) : (value1.Y);
        }

        public static Vector2D Min(Vector2D value1, Vector2D value2)
        {
            Vector2D result;
            Min(ref value1, ref value2, out result);
            return result;
        }
        public static void Min(ref Vector2D value1, ref Vector2D value2, out Vector2D result)
        {
            result.X = (value1.X > value2.X) ? (value2.X) : (value1.X);
            result.Y = (value1.Y > value2.Y) ? (value2.Y) : (value1.Y);
        }

        #endregion
        #region fields
        /// <summary>
        /// This is the X value. (Usually represents a horizontal position or direction.)
        /// </summary>
        [AdvBrowsable]
        [XmlAttribute]
        [System.ComponentModel.Description("The Magnitude on the X-Axis")]
        public Scalar X;
        /// <summary>
        /// This is the Y value. (Usually represents a vertical position or direction.)
        /// </summary>
        [AdvBrowsable]
        [XmlAttribute]
        [System.ComponentModel.Description("The Magnitude on the Y-Axis")]
        public Scalar Y;
        #endregion
        #region constructors
        /// <summary>
        /// Creates a New Vector2D Instance on the Stack.
        /// </summary>
        /// <param name="X">The X value.</param>
        /// <param name="Y">The Y value.</param>
        [InstanceConstructor("X,Y")]
        public Vector2D(Scalar X, Scalar Y)
        {
            this.X = X;
            this.Y = Y;
        }
        public Vector2D(Scalar[] vals) : this(vals, 0) { }
        public Vector2D(Scalar[] vals, int index)
        {
            Copy(vals, index, out this);
        }
        #endregion
        #region indexers
#if UNSAFE
        /// <summary>
        ///		Allows the Vector to be accessed linearly (v[0] -> v[Count-1]).  
        /// </summary>
        /// <remarks>
        ///    This indexer is only provided as a convenience, and is <b>not</b> recommended for use in
        ///    intensive applications.  
        /// </remarks>
        public Scalar this[int index]
        {
            get
            {
                ThrowHelper.CheckIndex("index", index, Count);
                unsafe
                {
                    fixed (Scalar* ptr = &this.X)
                    {
                        return ptr[index];
                    }
                }
            }
            set
            {
                ThrowHelper.CheckIndex("index", index, Count);
                unsafe
                {
                    fixed (Scalar* ptr = &this.X)
                    {
                        ptr[index] = value;
                    }
                }
            }
        }
#endif
        #endregion
        #region public properties
        /// <summary>
        /// Gets A perpendicular(orthogonal) Vector2D using the Right Hand Rule.
        /// </summary>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Right-hand_rule"/></remarks>
        public Vector2D RightHandNormal
        {
            get
            {
                return GetRightHandNormal(this);
            }
        }
        /// <summary>
        /// Gets A perpendicular(orthogonal) Vector2D using the Left Hand Rule.
        /// </summary>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Right-hand_rule#Left-hand_rule"/></remarks>
        public Vector2D LeftHandNormal
        {
            get
            {
                return GetLeftHandNormal(this);
            }
        }
        /// <summary>
        /// Gets or Sets the Magnitude (Length) of the Vector2D. 
        /// </summary>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Length_of_a_vector"/></remarks>
        [XmlIgnore]
        public Scalar Magnitude
        {
            get
            {
                return MathHelper.Sqrt(this.X * this.X + this.Y * this.Y);
            }
            set
            {
                SetMagnitude(ref this, ref value, out this);
            }
        }
        /// <summary>
        /// Gets the Squared Magnitude of the Vector2D.
        /// </summary>
        public Scalar MagnitudeSq
        {
            get
            {
                return this.X * this.X + this.Y * this.Y;
            }
        }
        /// <summary>
        /// Gets or Sets the Angle in radians of the Vector2D.
        /// </summary>
        /// <remarks>
        /// If the Magnitude of the Vector is 1 then The 
        /// Angles {0,Math.PI/2,Math.PI/2,3*Math.PI/2} would have the vectors {(1,0),(0,1),(-1,0),(0,-1)} respectively.
        /// </remarks>
        [XmlIgnore]
        public Scalar Angle
        {
            get
            {
                Scalar result;
                GetAngle(ref this, out result);
                return result;
            }
            set
            {
                SetAngle(ref this, ref value, out this);
            }
        }
        /// <summary>
        /// Gets the Normalized Vector2D. (Unit Vector)
        /// </summary>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Unit_vector"/></remarks>
        public Vector2D Normalized
        {
            get
            {
                return Normalize(this);
            }
        }
        /// <summary>
        /// The Number of Variables accesable though the indexer.
        /// </summary>
        int IAdvanceValueType.Count { get { return Count; } }
        #endregion
        #region public methods
        public Scalar[] ToArray()
        {
            Scalar[] array = new Scalar[Count];
            Copy(ref this, array, 0);
            return array;
        }
        public void CopyFrom(Scalar[] array, int index)
        {
            Copy(array, index, out this);
        }
        public void CopyTo(Scalar[] array, int index)
        {
            Copy(ref this, array, index);
        }
        public Vector3D ToVector3D(Scalar Z)
        {
            Vector3D rv;
            rv.X = this.X;
            rv.Y = this.Y;
            rv.Z = Z;
            return rv;
        }
        #endregion
        #region operators
        /// <summary>
        /// Adds 2 Vectors2Ds.
        /// </summary>
        /// <param name="left">The left Vector2D operand.</param>
        /// <param name="right">The right Vector2D operand.</param>
        /// <returns>The Sum of the 2 Vector2Ds.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Vector_addition_and_subtraction"/></remarks>
        public static Vector2D operator +(Vector2D left, Vector2D right)
        {
            Vector2D result;
            result.X = left.X + right.X;
            result.Y = left.Y + right.Y;
            return result;
        }
        /// <summary>
        /// Subtracts 2 Vector2Ds.
        /// </summary>
        /// <param name="left">The left Vector2D operand.</param>
        /// <param name="right">The right Vector2D operand.</param>
        /// <returns>The Difference of the 2 Vector2Ds.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Vector_addition_and_subtraction"/></remarks>
        public static Vector2D operator -(Vector2D left, Vector2D right)
        {
            Vector2D result;
            result.X = left.X - right.X;
            result.Y = left.Y - right.Y;
            return result;
        }
        /// <summary>
        /// Does Scaler Multiplication on a Vector2D.
        /// </summary>
        /// <param name="source">The Vector2D to be multiplied.</param>
        /// <param name="scalar">The scalar value that will multiply the Vector2D.</param>
        /// <returns>The Product of the Scaler Multiplication.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Scalar_multiplication"/></remarks>
        public static Vector2D operator *(Vector2D source, Scalar scalar)
        {
            Vector2D result;
            result.X = source.X * scalar;
            result.Y = source.Y * scalar;
            return result;
        }
        /// <summary>
        /// Does Scaler Multiplication on a Vector2D.
        /// </summary>
        /// <param name="scalar">The scalar value that will multiply the Vector2D.</param>
        /// <param name="source">The Vector2D to be multiplied.</param>
        /// <returns>The Product of the Scaler Multiplication.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Scalar_multiplication"/></remarks>
        public static Vector2D operator *(Scalar scalar, Vector2D source)
        {
            Vector2D result;
            result.X = scalar * source.X;
            result.Y = scalar * source.Y;
            return result;
        }
        /// <summary>
        /// Does a Dot Operation Also know as an Inner Product.
        /// </summary>
        /// <param name="left">The left Vector2D operand.</param>
        /// <param name="right">The right Vector2D operand.</param>
        /// <returns>The Dot Product (Inner Product).</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Dot_product"/></remarks>
        public static Scalar operator *(Vector2D left, Vector2D right)
        {
            return left.Y * right.Y + left.X * right.X;
        }
        public static Vector2D operator *(Matrix2x3 matrix, Vector2D source)
        {
            Vector2D result;
            result.X = (source.X * matrix.m00 + source.Y * matrix.m01 + matrix.m02);
            result.Y = (source.X * matrix.m10 + source.Y * matrix.m11 + matrix.m12);
            return result;
        }
        public static Vector2D operator *(Matrix3x3 matrix, Vector2D source)
        {
            Scalar inverseZ = 1 / (source.X * matrix.m20 + source.Y * matrix.m21 + matrix.m22);
            Vector2D result;
            result.X = (source.X * matrix.m00 + source.Y * matrix.m01 + matrix.m02) * inverseZ;
            result.Y = (source.X * matrix.m10 + source.Y * matrix.m11 + matrix.m12) * inverseZ;
            return result;
        }
        public static Vector2D operator *(Matrix2x2 matrix, Vector2D source)
        {
            Vector2D result;
            result.X = (source.X * matrix.m00 + source.Y * matrix.m01);
            result.Y = (source.X * matrix.m10 + source.Y * matrix.m11);
            return result;
        }
        /// <summary>
        /// Negates a Vector2D.
        /// </summary>
        /// <param name="source">The Vector2D to be Negated.</param>
        /// <returns>The Negated Vector2D.</returns>
        public static Vector2D operator -(Vector2D source)
        {
            Vector2D result;
            result.X = -source.X;
            result.Y = -source.Y;
            return result;
        }
        /// <summary>
        /// Does a "2D" Cross Product also know as an Outer Product.
        /// </summary>
        /// <param name="left">The left Vector2D operand.</param>
        /// <param name="right">The right Vector2D operand.</param>
        /// <returns>The Z value of the resulting vector.</returns>
        /// <remarks>
        /// This 2D Cross Product is using a cheat. Since the Cross product (in 3D space) 
        /// always generates a vector perpendicular (orthogonal) to the 2 vectors used as 
        /// arguments. The cheat is that the only vector that can be perpendicular to two 
        /// vectors in the XY Plane will parallel to the Z Axis. Since any vector that is 
        /// parallel to the Z Axis will have zeros in both the X and Y Fields I can represent
        /// the cross product of 2 vectors in the XY plane as single scalar: Z. Also the 
        /// Cross Product of and Vector on the XY plan and that of one ont on the Z Axis 
        /// will result in a vector on the XY Plane. So the ZCross Methods were well thought
        /// out and can be trusted.
        /// <seealso href="http://en.wikipedia.org/wiki/Cross_product"/>
        /// </remarks>
        public static Scalar operator ^(Vector2D left, Vector2D right)
        {
            return left.X * right.Y - left.Y * right.X;
        }
        /// <summary>
        /// Does a "2D" Cross Product also know as an Outer Product.
        /// </summary>
        /// <param name="leftZ">The Z value of the left vector operand.</param>
        /// <param name="right">The right Vector2D operand.</param>
        /// <returns>The Vector2D that fully represents the resulting vector.</returns>
        /// <remarks>
        /// This 2D Cross Product is using a cheat. Since the Cross product (in 3D space) 
        /// always generates a vector perpendicular (orthogonal) to the 2 vectors used as 
        /// arguments. The cheat is that the only vector that can be perpendicular to two 
        /// vectors in the XY Plane will parallel to the Z Axis. Since any vector that is 
        /// parallel to the Z Axis will have zeros in both the X and Y Fields I can represent
        /// the cross product of 2 vectors in the XY plane as single scalar: Z. Also the 
        /// Cross Product of and Vector on the XY plan and that of one ont on the Z Axis 
        /// will result in a vector on the XY Plane. So the ZCross Methods were well thought
        /// out and can be trusted.
        /// <seealso href="http://en.wikipedia.org/wiki/Cross_product"/>
        /// </remarks>
        public static Vector2D operator ^(Scalar leftZ, Vector2D right)
        {
            Vector2D result;
            result.X = -leftZ * right.Y;
            result.Y = leftZ * right.X;
            return result;
        }
        /// <summary>
        /// Does a "2D" Cross Product also know as an Outer Product.
        /// </summary>
        /// <param name="left">The left Vector2D operand.</param>
        /// <param name="rightZ">The Z value of the right vector operand.</param>
        /// <returns>The Vector2D that fully represents the resulting vector.</returns>
        /// <remarks>
        /// This 2D Cross Product is using a cheat. Since the Cross product (in 3D space) 
        /// always generates a vector perpendicular (orthogonal) to the 2 vectors used as 
        /// arguments. The cheat is that the only vector that can be perpendicular to two 
        /// vectors in the XY Plane will parallel to the Z Axis. Since any vector that is 
        /// parallel to the Z Axis will have zeros in both the X and Y Fields I can represent
        /// the cross product of 2 vectors in the XY plane as single scalar: Z. Also the 
        /// Cross Product of and Vector on the XY plan and that of one ont on the Z Axis 
        /// will result in a vector on the XY Plane. So the ZCross Methods were well thought
        /// out and can be trusted.
        /// <seealso href="http://en.wikipedia.org/wiki/Cross_product"/>
        /// </remarks>
        public static Vector2D operator ^(Vector2D left, Scalar rightZ)
        {
            Vector2D result;
            result.X = left.Y * rightZ;
            result.Y = -left.X * rightZ;
            return result;
        }
        /// <summary>
        /// Specifies whether the Vector2Ds contain the same coordinates.
        /// </summary>
        /// <param name="left">The left Vector2D to test.</param>
        /// <param name="right">The right Vector2D to test.</param>
        /// <returns>true if the Vector2Ds have the same coordinates; otherwise false</returns>
        public static bool operator ==(Vector2D left, Vector2D right)
        {
            return left.X == right.X && left.Y == right.Y;
        }
        /// <summary>
        /// Specifies whether the Vector2Ds do not contain the same coordinates.
        /// </summary>
        /// <param name="left">The left Vector2D to test.</param>
        /// <param name="right">The right Vector2D to test.</param>
        /// <returns>true if the Vector2Ds do not have the same coordinates; otherwise false</returns>
        public static bool operator !=(Vector2D left, Vector2D right)
        {
            return !(left.X == right.X && left.Y == right.Y);
        }



        public static explicit operator Vector2D(Vector3D source)
        {
            Vector2D result;
            result.X = source.X;
            result.Y = source.Y;
            return result;
        }

        #endregion
        #region overrides
        private string ToStringInternal(string FormatString)
        {
            return string.Format(FormatString, X, Y);
        }
        /// <summary>
        /// Converts the numeric value of this instance to its equivalent string representation, using the specified format.
        /// </summary>
        /// <param name="format">the format for each scaler in this Vector</param>
        /// <returns></returns>
        public string ToString(string format)
        {
            return ToStringInternal(string.Format(FormatableString, format));
        }
        public override string ToString()
        {
            return ToStringInternal(FormatString);
        }

#if !CompactFramework && !WindowsCE && !PocketPC && !XBOX360 
        public static bool TryParse(string s, out Vector2D result)
        {
            if (s == null)
            {
                result = Zero;
                return false;
            }
            string[] vals = ParseHelper.SplitStringVector(s);
            if (vals.Length != Count)
            {
                result = Zero;
                return false;
            }
            if (Scalar.TryParse(vals[0], out result.X) &&
                Scalar.TryParse(vals[1], out result.Y))
            {
                return true;
            }
            else
            {
                result = Zero;
                return false;
            }
        }
#endif
        [ParseMethod]
        public static Vector2D Parse(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }
            string[] vals = ParseHelper.SplitStringVector(s);
            if (vals.Length != Count)
            {
                ThrowHelper.ThrowVectorFormatException(s, Count, FormatString);
            }
            Vector2D value;
            value.X = Scalar.Parse(vals[0]);
            value.Y = Scalar.Parse(vals[1]);
            return value;
        }

        /// <summary>
        ///		Provides a unique hash code based on the member variables of this
        ///		class.  This should be done because the equality operators (==, !=)
        ///		have been overriden by this class.
        ///		<p/>
        ///		The standard implementation is a simple XOR operation between all local
        ///		member variables.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode();
        }
        /// <summary>
        ///		Compares this Vector to another object.  This should be done because the 
        ///		equality operators (==, !=) have been overriden by this class.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return (obj is Vector2D) && Equals((Vector2D)obj);
        }
        public bool Equals(Vector2D other)
        {
            return Equals(ref this, ref other);
        }
        public static bool Equals(Vector2D left, Vector2D right)
        {
            return
                left.X == right.X &&
                left.Y == right.Y;
        }
        [CLSCompliant(false)]
        public static bool Equals(ref Vector2D left, ref Vector2D right)
        {
            return
                left.X == right.X &&
                left.Y == right.Y;
        }
        #endregion
    }
}