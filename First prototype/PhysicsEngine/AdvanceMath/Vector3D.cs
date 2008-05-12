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
    /// A Vector with 3 dimensions.
    /// </summary>
    /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29"/></remarks>
    [StructLayout(LayoutKind.Sequential, Size = Vector3D.Size)]
    [AdvBrowsableOrder("X,Y,Z"), Serializable]
#if !CompactFramework && !WindowsCE && !PocketPC && !XBOX360 
    [System.ComponentModel.TypeConverter(typeof(AdvTypeConverter<Vector3D>))]
#endif
    public struct Vector3D : IVector<Vector3D>
    {
        #region const fields
        /// <summary>
        /// The number of Scalar values in the class.
        /// </summary>
        public const int Count = 3;
        /// <summary>
        /// The Size of the class in bytes;
        /// </summary>
        public const int Size = sizeof(Scalar) * Count;
        #endregion
        #region readonly fields
        /// <summary>
        /// Vector3D(0,0,0)
        /// </summary>
        public static readonly Vector3D Origin = new Vector3D();
        /// <summary>
        /// Vector3D(0,0,0)
        /// </summary>
        public static readonly Vector3D Zero = new Vector3D();
        /// <summary>
        /// Vector3D(1,0,0)
        /// </summary>
        public static readonly Vector3D XAxis = new Vector3D(1, 0, 0);
        /// <summary>
        /// Vector3D(0,1,0)
        /// </summary>
        public static readonly Vector3D YAxis = new Vector3D(0, 1, 0);
        /// <summary>
        /// Vector3D(0,0,1)
        /// </summary>
        public static readonly Vector3D ZAxis = new Vector3D(0, 0, 1);

        private static readonly string FormatString = MatrixHelper.CreateVectorFormatString(Count);
        private readonly static string FormatableString = MatrixHelper.CreateVectorFormatableString(Count);
        #endregion
        #region static methods
        public static void Copy(ref Vector3D vector, Scalar[] destArray)
        {
            Copy(ref vector, destArray, 0);
        }
        public static void Copy(ref Vector3D vector, Scalar[] destArray, int index)
        {
            ThrowHelper.CheckCopy(destArray, index, Count);

            destArray[index] = vector.X;
            destArray[++index] = vector.Y;
            destArray[++index] = vector.Z;
        }
        public static void Copy(Scalar[] sourceArray, out Vector3D result)
        {
            Copy(sourceArray, 0, out result);
        }
        public static void Copy(Scalar[] sourceArray, int index, out Vector3D result)
        {
            ThrowHelper.CheckCopy(sourceArray, index, Count);

            result.X = sourceArray[index];
            result.Y = sourceArray[++index];
            result.Z = sourceArray[++index];
        }
        public static void Copy(ref Vector4D source, out Vector3D dest)
        {
            dest.X = source.X;
            dest.Y = source.Y;
            dest.Z = source.Z;
        }
        public static void Copy(ref Vector2D source, ref Vector3D dest)
        {
            dest.X = source.X;
            dest.Y = source.Y;
        }

        public static Vector3D Clamp(Vector3D value, Vector3D min, Vector3D max)
        {
            Vector3D result;
            MathHelper.Clamp(ref value.X, ref  min.X, ref  max.X, out result.X);
            MathHelper.Clamp(ref value.Y, ref  min.Y, ref  max.Y, out result.Y);
            MathHelper.Clamp(ref value.Z, ref  min.Z, ref  max.Z, out result.Z);
            return result;
        }
        public static void Clamp(ref Vector3D value, ref Vector3D min, ref Vector3D max, out Vector3D result)
        {
            MathHelper.Clamp(ref value.X, ref  min.X, ref  max.X, out result.X);
            MathHelper.Clamp(ref value.Y, ref  min.Y, ref  max.Y, out result.Y);
            MathHelper.Clamp(ref value.Z, ref  min.Z, ref  max.Z, out result.Z);
        }

        public static Vector3D Lerp(Vector3D left, Vector3D right, Scalar amount)
        {
            Vector3D result;
            Lerp(ref left, ref right, ref amount, out result);
            return result;
        }
        public static void Lerp(ref Vector3D left, ref Vector3D right, ref Scalar amount, out Vector3D result)
        {
            result.X = (right.X - left.X) * amount + left.X;
            result.Y = (right.Y - left.Y) * amount + left.Y;
            result.Z = (right.Z - left.Z) * amount + left.Z;
        }
        public static Vector3D Lerp(Vector3D left, Vector3D right, Vector3D amount)
        {
            Vector3D result;
            Lerp(ref left, ref right, ref amount, out result);
            return result;
        }
        public static void Lerp(ref Vector3D left, ref Vector3D right, ref Vector3D amount, out Vector3D result)
        {
            result.X = (right.X - left.X) * amount.X + left.X;
            result.Y = (right.Y - left.Y) * amount.Y + left.Y;
            result.Z = (right.Z - left.Z) * amount.Z + left.Z;
        }

        public static Scalar Distance(Vector3D left, Vector3D right)
        {
            Scalar result;
            Distance(ref left, ref right, out result);
            return result;
        }
        public static void Distance(ref Vector3D left, ref Vector3D right, out Scalar result)
        {
            Vector3D diff;
            Subtract(ref left, ref right, out diff);
            GetMagnitude(ref diff, out result);
        }
        public static Scalar DistanceSq(Vector3D left, Vector3D right)
        {
            Scalar result;
            DistanceSq(ref left, ref right, out result);
            return result;
        }
        public static void DistanceSq(ref Vector3D left, ref Vector3D right, out Scalar result)
        {
            Vector3D diff;
            Subtract(ref left, ref right, out diff);
            GetMagnitudeSq(ref diff, out result);
        }

        /// <summary>
        /// Adds 2 Vectors2Ds.
        /// </summary>
        /// <param name="left">The left Vector3D operand.</param>
        /// <param name="right">The right Vector3D operand.</param>
        /// <returns>The Sum of the 2 Vector3Ds.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Vector_addition_and_subtraction"/></remarks>
        public static Vector3D Add(Vector3D left, Vector3D right)
        {
            Vector3D result;
            result.X = left.X + right.X;
            result.Y = left.Y + right.Y;
            result.Z = left.Z + right.Z;
            return result;
        }
        public static void Add(ref Vector3D left, ref Vector3D right, out Vector3D result)
        {
            result.X = left.X + right.X;
            result.Y = left.Y + right.Y;
            result.Z = left.Z + right.Z;
        }

        public static Vector3D Add(Vector2D left, Vector3D right)
        {
            Vector3D result;
            Add(ref left, ref right, out result);
            return result;
        }
        public static void Add(ref Vector2D left, ref Vector3D right, out Vector3D result)
        {
            result.X = left.X + right.X;
            result.Y = left.Y + right.Y;
            result.Z = right.Z;
        }
        public static Vector3D Add(Vector3D left, Vector2D right)
        {
            Vector3D result;
            Add(ref left, ref right, out result);
            return result;
        }
        public static void Add(ref Vector3D left, ref Vector2D right, out Vector3D result)
        {
            result.X = left.X + right.X;
            result.Y = left.Y + right.Y;
            result.Z = left.Z;
        }


        /// <summary>
        /// Subtracts 2 Vector3Ds.
        /// </summary>
        /// <param name="left">The left Vector3D operand.</param>
        /// <param name="right">The right Vector3D operand.</param>
        /// <returns>The Difference of the 2 Vector3Ds.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Vector_addition_and_subtraction"/></remarks>
        public static Vector3D Subtract(Vector3D left, Vector3D right)
        {
            Vector3D result;
            result.X = left.X - right.X;
            result.Y = left.Y - right.Y;
            result.Z = left.Z - right.Z;
            return result;
        }
        public static void Subtract(ref Vector3D left, ref Vector3D right, out Vector3D result)
        {
            result.X = left.X - right.X;
            result.Y = left.Y - right.Y;
            result.Z = left.Z - right.Z;
        }

        public static Vector3D Subtract(Vector2D left, Vector3D right)
        {
            Vector3D result;
            Subtract(ref left, ref right, out result);
            return result;
        }
        public static void Subtract(ref Vector2D left, ref Vector3D right, out Vector3D result)
        {
            result.X = left.X - right.X;
            result.Y = left.Y - right.Y;
            result.Z = -right.Z;
        }
        public static Vector3D Subtract(Vector3D left, Vector2D right)
        {
            Vector3D result;
            Subtract(ref left, ref right, out result);
            return result;
        }
        public static void Subtract(ref Vector3D left, ref Vector2D right, out Vector3D result)
        {
            result.X = left.X - right.X;
            result.Y = left.Y - right.Y;
            result.Z = left.Z;
        }

        /// <summary>
        /// Does Scaler Multiplication on a Vector3D.
        /// </summary>
        /// <param name="source">The Vector3D to be multiplied.</param>
        /// <param name="scalar">The scalar value that will multiply the Vector3D.</param>
        /// <returns>The Product of the Scaler Multiplication.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Scalar_multiplication"/></remarks>
        public static Vector3D Multiply(Vector3D source, Scalar scalar)
        {
            Vector3D result;
            result.X = source.X * scalar;
            result.Y = source.Y * scalar;
            result.Z = source.Z * scalar;
            return result;
        }
        public static void Multiply(ref Vector3D source, ref Scalar scalar, out Vector3D result)
        {
            result.X = source.X * scalar;
            result.Y = source.Y * scalar;
            result.Z = source.Z * scalar;
        }
        /// <summary>
        /// Does Scaler Multiplication on a Vector3D.
        /// </summary>
        /// <param name="scalar">The scalar value that will multiply the Vector3D.</param>
        /// <param name="source">The Vector3D to be multiplied.</param>
        /// <returns>The Product of the Scaler Multiplication.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Scalar_multiplication"/></remarks>
        public static Vector3D Multiply(Scalar scalar, Vector3D source)
        {
            Vector3D result;
            result.X = scalar * source.X;
            result.Y = scalar * source.Y;
            result.Z = scalar * source.Z;
            return result;
        }
        public static void Multiply(ref Scalar scalar, ref Vector3D source, out Vector3D result)
        {
            result.X = scalar * source.X;
            result.Y = scalar * source.Y;
            result.Z = scalar * source.Z;
        }
        /// <summary>
        ///		matrix * vector [3x3 * 3x1 = 3x1]
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Vector3D Transform(Matrix3x3 matrix, Vector3D vector)
        {
            Vector3D result;

            result.X = matrix.m00 * vector.X + matrix.m01 * vector.Y + matrix.m02 * vector.Z;
            result.Y = matrix.m10 * vector.X + matrix.m11 * vector.Y + matrix.m12 * vector.Z;
            result.Z = matrix.m20 * vector.X + matrix.m21 * vector.Y + matrix.m22 * vector.Z;

            return result;
        }
        public static void Transform(ref Matrix3x3 matrix, ref Vector3D vector, out Vector3D result)
        {
            Scalar X = vector.X;
            Scalar Y = vector.Y;
            result.X = matrix.m00 * X + matrix.m01 * Y + matrix.m02 * vector.Z;
            result.Y = matrix.m10 * X + matrix.m11 * Y + matrix.m12 * vector.Z;
            result.Z = matrix.m20 * X + matrix.m21 * Y + matrix.m22 * vector.Z;
        }
        /// <summary>
        ///		vector * matrix [1x3 * 3x3 = 1x3]
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Vector3D Transform(Vector3D vector, Matrix3x3 matrix)
        {
            Vector3D result;

            result.X = matrix.m00 * vector.X + matrix.m01 * vector.Y + matrix.m02 * vector.Z;
            result.Y = matrix.m10 * vector.X + matrix.m11 * vector.Y + matrix.m12 * vector.Z;
            result.Z = matrix.m20 * vector.X + matrix.m21 * vector.Y + matrix.m22 * vector.Z;

            return result;
        }
        public static void Transform(ref Vector3D vector, ref Matrix3x3 matrix, out Vector3D result)
        {
            Scalar X = vector.X;
            Scalar Y = vector.Y;
            result.X = matrix.m00 * X + matrix.m01 * Y + matrix.m02 * vector.Z;
            result.Y = matrix.m10 * X + matrix.m11 * Y + matrix.m12 * vector.Z;
            result.Z = matrix.m20 * X + matrix.m21 * Y + matrix.m22 * vector.Z;
        }
        /// <summary>
        ///		Transforms the given 3-D vector by the matrix, projecting the 
        ///		result back into <i>w</i> = 1.
        ///		<p/>
        ///		This means that the initial <i>w</i> is considered to be 1.0,
        ///		and then all the tree elements of the resulting 3-D vector are
        ///		divided by the resulting <i>w</i>.
        /// </summary>
        /// <param name="matrix">A Matrix4.</param>
        /// <param name="vector">A Vector3D.</param>
        /// <returns>A new vector.</returns>
        public static Vector3D Transform(Matrix4x4 matrix, Vector3D vector)
        {
            Vector3D result;

            Scalar inverseW = 1 / (matrix.m30 * vector.X + matrix.m31 * vector.Y + matrix.m32 * vector.Z + matrix.m33);
            result.X = ((matrix.m00 * vector.X) + (matrix.m01 * vector.Y) + (matrix.m02 * vector.Z) + matrix.m03) * inverseW;
            result.Y = ((matrix.m10 * vector.X) + (matrix.m11 * vector.Y) + (matrix.m12 * vector.Z) + matrix.m13) * inverseW;
            result.Z = ((matrix.m20 * vector.X) + (matrix.m21 * vector.Y) + (matrix.m22 * vector.Z) + matrix.m23) * inverseW;

            return result;
        }
        public static void Transform(ref Matrix4x4 matrix, ref Vector3D vector, out Vector3D result)
        {
            Scalar X = vector.X;
            Scalar Y = vector.Y;
            Scalar inverseW = 1 / (matrix.m30 * X + matrix.m31 * Y + matrix.m32 * vector.Z + matrix.m33);
            result.X = ((matrix.m00 * X) + (matrix.m01 * Y) + (matrix.m02 * vector.Z) + matrix.m03) * inverseW;
            result.Y = ((matrix.m10 * X) + (matrix.m11 * Y) + (matrix.m12 * vector.Z) + matrix.m13) * inverseW;
            result.Z = ((matrix.m20 * X) + (matrix.m21 * Y) + (matrix.m22 * vector.Z) + matrix.m23) * inverseW;
        }

        public static Vector3D Multiply(Quaternion quat, Vector3D vector)
        {
            // nVidia SDK implementation
            Vector3D uv, uuv;
            Vector3D qvec;// = new Vector3D(quat.X, quat.Y, quat.Z);

            qvec.X = quat.X;
            qvec.Y = quat.Y;
            qvec.Z = quat.Z;
            Vector3D.Cross(ref qvec, ref vector, out uv);
            Vector3D.Cross(ref qvec, ref uv, out uuv);
            Vector3D.Cross(ref qvec, ref uv, out uuv);
            Vector3D.Multiply(ref uv, ref quat.W, out uv);
            Vector3D.Add(ref uv, ref uuv, out uv);
            Vector3D.Multiply(ref uv, ref MathHelper.Two, out uv);
            Vector3D.Add(ref vector, ref uv, out uv);
            return uv;

            //uv = qvec ^ vector;
            //uuv = qvec ^ uv;
            //uv *= (2 * quat.W);
            //uuv *= 2;

            //return vector + uv + uuv;

            // get the rotation matriX of the Quaternion and multiplY it times the vector
            //return quat.ToRotationMatrix() * vector;
        }
        public static void Multiply(ref Quaternion quat, ref Vector3D vector, out Vector3D result)
        {
            Vector3D uv, uuv;
            Vector3D qvec;
            qvec.X = quat.X;
            qvec.Y = quat.Y;
            qvec.Z = quat.Z;
            Vector3D.Cross(ref qvec, ref vector, out uv);
            Vector3D.Cross(ref qvec, ref uv, out uuv);
            Vector3D.Cross(ref qvec, ref uv, out uuv);
            Vector3D.Multiply(ref uv, ref quat.W, out uv);
            Vector3D.Add(ref uv, ref uuv, out uv);
            Vector3D.Multiply(ref uv, ref MathHelper.Two, out uv);
            Vector3D.Add(ref vector, ref uv, out result);
        }
        /// <summary>
        /// Does a Dot Operation Also know as an Inner Product.
        /// </summary>
        /// <param name="left">The left Vector3D operand.</param>
        /// <param name="right">The right Vector3D operand.</param>
        /// <returns>The Dot Product (Inner Product).</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Dot_product"/></remarks>
        public static Scalar Dot(Vector3D left, Vector3D right)
        {
            return left.Y * right.Y + left.X * right.X + left.Z * right.Z;
        }
        public static void Dot(ref Vector3D left, ref Vector3D right, out Scalar result)
        {
            result = left.Y * right.Y + left.X * right.X + left.Z * right.Z;
        }
        /// <summary>
        /// Does a Cross Operation Also know as an Outer Product.
        /// </summary>
        /// <param name="left">The left Vector3D operand.</param>
        /// <param name="right">The right Vector3D operand.</param>
        /// <returns>The Cross Product.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Cross_product"/></remarks>
        public static Vector3D Cross(Vector3D left, Vector3D right)
        {
            Vector3D result;
            result.X = left.Y * right.Z - left.Z * right.Y;
            result.Y = left.Z * right.X - left.X * right.Z;
            result.Z = left.X * right.Y - left.Y * right.X;
            return result;
        }
        public static void Cross(ref Vector3D left, ref Vector3D right, out Vector3D result)
        {
            Scalar X = left.Y * right.Z - left.Z * right.Y;
            Scalar Y = left.Z * right.X - left.X * right.Z;
            result.Z = left.X * right.Y - left.Y * right.X;
            result.X = X;
            result.Y = Y;
        }
        /// <summary>
        /// Gets the Squared <see cref="Magnitude"/> of the Vector3D that is passed.
        /// </summary>
        /// <param name="source">The Vector3D whos Squared Magnitude is te be returned.</param>
        /// <returns>The Squared Magnitude.</returns>
        public static Scalar GetMagnitudeSq(Vector3D source)
        {
            return source.X * source.X + source.Y * source.Y + source.Z * source.Z;
        }
        public static void GetMagnitudeSq(ref Vector3D source, out Scalar result)
        {
            result = source.X * source.X + source.Y * source.Y + source.Z * source.Z;
        }
        /// <summary>
        /// Gets the <see cref="Magnitude"/> of the Vector3D that is passed.
        /// </summary>
        /// <param name="source">The Vector3D whos Magnitude is te be returned.</param>
        /// <returns>The Magnitude.</returns>
        public static Scalar GetMagnitude(Vector3D source)
        {
            return MathHelper.Sqrt(source.X * source.X + source.Y * source.Y + source.Z * source.Z);
        }
        public static void GetMagnitude(ref Vector3D source, out Scalar result)
        {
            result = MathHelper.Sqrt(source.X * source.X + source.Y * source.Y + source.Z * source.Z);
        }
        /// <summary>
        /// Sets the <see cref="Magnitude"/> of a Vector3D.
        /// </summary>
        /// <param name="source">The Vector3D whose Magnitude is to be changed.</param>
        /// <param name="magnitude">The Magnitude.</param>
        /// <returns>A Vector3D with the new Magnitude</returns>
        public static Vector3D SetMagnitude(Vector3D source, Scalar magnitude)
        {
            Vector3D result;
            SetMagnitude(ref source, ref magnitude, out result);
            return result;
        }
        public static void SetMagnitude(ref Vector3D source, ref Scalar magnitude, out Vector3D result)
        {
            Scalar oldmagnitude;
            GetMagnitude(ref source, out oldmagnitude);
            if (oldmagnitude > 0 && magnitude != 0)
            {
                oldmagnitude = (magnitude / oldmagnitude);
                Multiply(ref source, ref oldmagnitude, out result);
            }
            else
            {
                result = Zero;
            }
        }

        /// <summary>
        /// This returns the Normalized Vector3D that is passed. This is also known as a Unit Vector.
        /// </summary>
        /// <param name="source">The Vector3D to be Normalized.</param>
        /// <returns>The Normalized Vector3D. (Unit Vector)</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Unit_vector"/></remarks>
        public static Vector3D Normalize(Vector3D source)
        {
            Scalar oldmagnitude;
            GetMagnitude(ref source, out oldmagnitude);
            if (oldmagnitude == 0) { return Zero; }
            oldmagnitude = (1 / oldmagnitude);
            Vector3D result;
            result.X = source.X * oldmagnitude;
            result.Y = source.Y * oldmagnitude;
            result.Z = source.Z * oldmagnitude;
            return result;
        }
        public static void Normalize(ref Vector3D source, out Vector3D result)
        {
            Scalar oldmagnitude;
            GetMagnitude(ref source, out oldmagnitude);
            if (oldmagnitude == 0) { result = Zero; return; }
            oldmagnitude = (1 / oldmagnitude);
            result.X = source.X * oldmagnitude;
            result.Y = source.Y * oldmagnitude;
            result.Z = source.Z * oldmagnitude;
        }
        [CLSCompliant(false)]
        public static void Normalize(ref Vector3D source)
        {
            Normalize(ref source, out source);
        }
        /// <summary>
        /// Negates a Vector3D.
        /// </summary>
        /// <param name="source">The Vector3D to be Negated.</param>
        /// <returns>The Negated Vector3D.</returns>
        public static Vector3D Negate(Vector3D source)
        {
            Vector3D result;
            result.X = -source.X;
            result.Y = -source.Y;
            result.Z = -source.Z;
            return result;
        }
        [CLSCompliant(false)]
        public static void Negate(ref Vector3D source)
        {
            Negate(ref source, out source);
        }
        public static void Negate(ref Vector3D source, out Vector3D result)
        {
            result.X = -source.X;
            result.Y = -source.Y;
            result.Z = -source.Z;
        }

        /// <summary>
        /// Thie Projects the left Vector3D onto the Right Vector3D.
        /// </summary>
        /// <param name="left">The left Vector3D operand.</param>
        /// <param name="right">The right Vector3D operand.</param>
        /// <returns>The Projected Vector3D.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Projection_%28linear_algebra%29"/></remarks>
        public static Vector3D Project(Vector3D left, Vector3D right)
        {
            Vector3D result;
            Project(ref left, ref right, out result);
            return result;
        }
        public static void Project(ref Vector3D left, ref Vector3D right, out Vector3D result)
        {
            Scalar tmp, magsq;
            Dot(ref left, ref  right, out tmp);
            GetMagnitudeSq(ref right, out magsq);
            tmp /= magsq;
            Multiply(ref right, ref tmp, out result);
        }

        public static Vector3D Hermite(Vector3D value1, Vector3D tangent1, Vector3D value2, Vector3D tangent2, Scalar amount)
        {
            Vector3D result;
            Hermite(ref value1, ref tangent1, ref value2, ref tangent2, amount, out result);
            return result;
        }
        public static void Hermite(ref  Vector3D value1, ref Vector3D tangent1, ref Vector3D value2, ref Vector3D tangent2, Scalar amount, out Vector3D result)
        {
            Scalar h1, h2, h3, h4;
            MathHelper.HermiteHelper(amount, out h1, out h2, out h3, out h4);
            result.X = h1 * value1.X + h2 * value2.X + h3 * tangent1.X + h4 * tangent2.X;
            result.Y = h1 * value1.Y + h2 * value2.Y + h3 * tangent1.Y + h4 * tangent2.Y;
            result.Z = h1 * value1.Z + h2 * value2.Z + h3 * tangent1.Z + h4 * tangent2.Z;
        }

        public static Vector3D CatmullRom(Vector3D value1, Vector3D value2, Vector3D value3, Vector3D value4, Scalar amount)
        {
            Vector3D result;
            CatmullRom(ref value1, ref value2, ref value3, ref value4, amount, out result);
            return result;
        }
        public static void CatmullRom(ref Vector3D value1, ref Vector3D value2, ref Vector3D value3, ref Vector3D value4, Scalar amount, out Vector3D result)
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
            result.Z =
                0.5f * ((2 * value2.Z) +
                (-value1.Z + value3.Z) * amount +
                (2 * value1.Z - 5 * value2.Z + 4 * value3.Z - value4.Z) * amountSq +
                (-value1.Z + 3 * value2.Z - 3 * value3.Z + value4.Z) * amountCu);
        }

        public static Vector3D Max(Vector3D value1, Vector3D value2)
        {
            Vector3D result;
            Max(ref value1, ref value2, out result);
            return result;
        }
        public static void Max(ref Vector3D value1, ref Vector3D value2, out Vector3D result)
        {
            result.X = (value1.X < value2.X) ? (value2.X) : (value1.X);
            result.Y = (value1.Y < value2.Y) ? (value2.Y) : (value1.Y);
            result.Z = (value1.Z < value2.Z) ? (value2.Z) : (value1.Z);
        }

        public static Vector3D Min(Vector3D value1, Vector3D value2)
        {
            Vector3D result;
            Min(ref value1, ref value2, out result);
            return result;
        }
        public static void Min(ref Vector3D value1, ref Vector3D value2, out Vector3D result)
        {
            result.X = (value1.X > value2.X) ? (value2.X) : (value1.X);
            result.Y = (value1.Y > value2.Y) ? (value2.Y) : (value1.Y);
            result.Z = (value1.Z > value2.Z) ? (value2.Z) : (value1.Z);
        }

        #endregion
        #region fields
        /// <summary>
        /// This is the X value.
        /// </summary>
        [AdvBrowsable]
        [XmlAttribute]
        [System.ComponentModel.Description("The Magnitude on the X-Axis")]
        public Scalar X;
        /// <summary>
        /// This is the Y value.
        /// </summary>
        [AdvBrowsable]
        [XmlAttribute]
        [System.ComponentModel.Description("The Magnitude on the Y-Axis")]
        public Scalar Y;
        /// <summary>
        /// This is the Z value. 
        /// </summary>
        [AdvBrowsable]
        [XmlAttribute]
        [System.ComponentModel.Description("The Magnitude on the Z-Axis")]
        public Scalar Z;
        #endregion
        #region constructors
        /// <summary>
        /// Creates a New Vector3D Instance on the Stack.
        /// </summary>
        /// <param name="X">The X value.</param>
        /// <param name="Y">The Y value.</param>
        /// <param name="Z">The Z value.</param>
        [InstanceConstructor("X,Y,Z")]
        public Vector3D(Scalar X, Scalar Y, Scalar Z)
        {
            //this.Vector2D = Vector2D.Zero;
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }
        public Vector3D(Scalar[] vals) : this(vals, 0) { }
        public Vector3D(Scalar[] vals, int index)
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
        /// Gets or Sets the Magnitude (Length) of the Vector3D. 
        /// </summary>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Length_of_a_vector"/></remarks>
        [XmlIgnore]
        public Scalar Magnitude
        {
            get
            {
                return MathHelper.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z);
            }
            set
            {
                this = SetMagnitude(this, value);
            }
        }
        /// <summary>
        /// Gets the Squared Magnitude of the Vector3D.
        /// </summary>
        public Scalar MagnitudeSq
        {
            get
            {
                return this.X * this.X + this.Y * this.Y + this.Z * this.Z;
            }
        }
        /// <summary>
        /// Gets the Normalized Vector3D. (Unit Vector)
        /// </summary>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Unit_vector"/></remarks>
        public Vector3D Normalized
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
        #endregion
        #region operators
        /// <summary>
        /// Adds 2 Vectors2Ds.
        /// </summary>
        /// <param name="left">The left Vector3D operand.</param>
        /// <param name="right">The right Vector3D operand.</param>
        /// <returns>The Sum of the 2 Vector3Ds.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Vector_addition_and_subtraction"/></remarks>
        public static Vector3D operator +(Vector3D left, Vector3D right)
        {
            Vector3D result;
            result.X = left.X + right.X;
            result.Y = left.Y + right.Y;
            result.Z = left.Z + right.Z;
            return result;
        }
        public static Vector3D operator +(Vector2D left, Vector3D right)
        {
            Vector3D result;
            Add(ref left, ref right, out result);
            return result;
        }
        public static Vector3D operator +(Vector3D left, Vector2D right)
        {
            Vector3D result;
            Add(ref left, ref right, out result);
            return result;
        }
        /// <summary>
        /// Subtracts 2 Vector3Ds.
        /// </summary>
        /// <param name="left">The left Vector3D operand.</param>
        /// <param name="right">The right Vector3D operand.</param>
        /// <returns>The Difference of the 2 Vector3Ds.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Vector_addition_and_subtraction"/></remarks>
        public static Vector3D operator -(Vector3D left, Vector3D right)
        {
            Vector3D result;
            result.X = left.X - right.X;
            result.Y = left.Y - right.Y;
            result.Z = left.Z - right.Z;
            return result;
        }
        public static Vector3D operator -(Vector2D left, Vector3D right)
        {
            Vector3D result;
            Subtract(ref left, ref right, out result);
            return result;
        }
        public static Vector3D operator -(Vector3D left, Vector2D right)
        {
            Vector3D result;
            Subtract(ref left, ref right, out result);
            return result;
        }
        /// <summary>
        /// Does Scaler Multiplication on a Vector3D.
        /// </summary>
        /// <param name="source">The Vector3D to be multiplied.</param>
        /// <param name="scalar">The scalar value that will multiply the Vector3D.</param>
        /// <returns>The Product of the Scaler Multiplication.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Scalar_multiplication"/></remarks>
        public static Vector3D operator *(Vector3D source, Scalar scalar)
        {
            Vector3D result;
            result.X = source.X * scalar;
            result.Y = source.Y * scalar;
            result.Z = source.Z * scalar;
            return result;
        }
        /// <summary>
        /// Does Scaler Multiplication on a Vector3D.
        /// </summary>
        /// <param name="scalar">The scalar value that will multiply the Vector3D.</param>
        /// <param name="source">The Vector3D to be multiplied.</param>
        /// <returns>The Product of the Scaler Multiplication.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Scalar_multiplication"/></remarks>
        public static Vector3D operator *(Scalar scalar, Vector3D source)
        {
            Vector3D result;
            result.X = scalar * source.X;
            result.Y = scalar * source.Y;
            result.Z = scalar * source.Z;
            return result;
        }
        /// <summary>
        /// Does a Dot Operation Also know as an Inner Product.
        /// </summary>
        /// <param name="left">The left Vector3D operand.</param>
        /// <param name="right">The right Vector3D operand.</param>
        /// <returns>The Dot Product (Inner Product).</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Dot_product"/></remarks>
        public static Scalar operator *(Vector3D left, Vector3D right)
        {
            return left.X * right.X + left.Y * right.Y + left.Z * right.Z;
        }
        /// <summary>
        ///		Transforms the given 3-D vector by the matrix, projecting the 
        ///		result back into <i>w</i> = 1.
        ///		<p/>
        ///		This means that the initial <i>w</i> is considered to be 1.0,
        ///		and then all the tree elements of the resulting 3-D vector are
        ///		divided by the resulting <i>w</i>.
        /// </summary>
        /// <param name="matrix">A Matrix4.</param>
        /// <param name="vector">A Vector3D.</param>
        /// <returns>A new vector.</returns>
        public static Vector3D operator *(Matrix4x4 matrix, Vector3D vector)
        {
            Vector3D result;

            Scalar inverseW = 1 / (matrix.m30 * vector.X + matrix.m31 * vector.Y + matrix.m32 * vector.Z + matrix.m33);
            result.X = ((matrix.m00 * vector.X) + (matrix.m01 * vector.Y) + (matrix.m02 * vector.Z) + matrix.m03) * inverseW;
            result.Y = ((matrix.m10 * vector.X) + (matrix.m11 * vector.Y) + (matrix.m12 * vector.Z) + matrix.m13) * inverseW;
            result.Z = ((matrix.m20 * vector.X) + (matrix.m21 * vector.Y) + (matrix.m22 * vector.Z) + matrix.m23) * inverseW;

            return result;
        }
        /// <summary>
        ///		matrix * vector [3x3 * 3x1 = 3x1]
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Vector3D operator *(Matrix3x3 matrix, Vector3D vector)
        {
            Vector3D result;

            result.X = matrix.m00 * vector.X + matrix.m01 * vector.Y + matrix.m02 * vector.Z;
            result.Y = matrix.m10 * vector.X + matrix.m11 * vector.Y + matrix.m12 * vector.Z;
            result.Z = matrix.m20 * vector.X + matrix.m21 * vector.Y + matrix.m22 * vector.Z;

            return result;
        }
        public static Vector3D operator *(Quaternion quat, Vector3D vector)
        {
            // nVidia SDK implementation
            Vector3D uv, uuv;
            Vector3D qvec;// = new Vector3D(quat.X, quat.Y, quat.Z);

            qvec.X = quat.X;
            qvec.Y = quat.Y;
            qvec.Z = quat.Z;
            Cross(ref qvec, ref vector, out uv);
            Cross(ref qvec, ref uv, out uuv);
            Cross(ref qvec, ref uv, out uuv);
            Multiply(ref uv, ref quat.W, out uv);
            Add(ref uv, ref uuv, out uv);
            Multiply(ref uv, ref MathHelper.Two, out uv);
            Add(ref vector, ref uv, out uv);
            return uv;

            //uv = qvec ^ vector;
            //uuv = qvec ^ uv;
            //uv *= (2 * quat.W);
            //uuv *= 2;

            //return vector + uv + uuv;

            // get the rotation matriX of the Quaternion and multiplY it times the vector
            //return quat.ToRotationMatrix() * vector;
        }
        /// <summary>
        ///		vector * matrix [1x3 * 3x3 = 1x3]
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Vector3D operator *(Vector3D vector, Matrix3x3 matrix)
        {
            Vector3D result;

            result.X = matrix.m00 * vector.X + matrix.m01 * vector.Y + matrix.m02 * vector.Z;
            result.Y = matrix.m10 * vector.X + matrix.m11 * vector.Y + matrix.m12 * vector.Z;
            result.Z = matrix.m20 * vector.X + matrix.m21 * vector.Y + matrix.m22 * vector.Z;

            return result;
        }
        /// <summary>
        /// Negates a Vector3D.
        /// </summary>
        /// <param name="source">The Vector3D to be Negated.</param>
        /// <returns>The Negated Vector3D.</returns>
        public static Vector3D operator -(Vector3D source)
        {
            Vector3D result;
            result.X = -source.X;
            result.Y = -source.Y;
            result.Z = -source.Z;
            return result;
        }
        /// <summary>
        /// Does a "2D" Cross Product also know as an Outer Product.
        /// </summary>
        /// <param name="left">The left Vector3D operand.</param>
        /// <param name="right">The right Vector3D operand.</param>
        /// <returns>The Z value of the resulting vector.</returns>
        /// <remarks>
        /// <seealso href="http://en.wikipedia.org/wiki/Cross_product"/>
        /// </remarks>
        public static Vector3D operator ^(Vector3D left, Vector3D right)
        {
            Vector3D result;
            result.X = left.Y * right.Z - left.Z * right.Y;
            result.Y = left.Z * right.X - left.X * right.Z;
            result.Z = left.X * right.Y - left.Y * right.X;
            return result;
        }
        /// <summary>
        /// Specifies whether the Vector3Ds contain the same coordinates.
        /// </summary>
        /// <param name="left">The left Vector3D to test.</param>
        /// <param name="right">The right Vector3D to test.</param>
        /// <returns>true if the Vector3Ds have the same coordinates; otherwise false</returns>
        public static bool operator ==(Vector3D left, Vector3D right)
        {
            return left.X == right.X && left.Y == right.Y && left.Z == right.Z;
        }
        /// <summary>
        /// Specifies whether the Vector3Ds do not contain the same coordinates.
        /// </summary>
        /// <param name="left">The left Vector3D to test.</param>
        /// <param name="right">The right Vector3D to test.</param>
        /// <returns>true if the Vector3Ds do not have the same coordinates; otherwise false</returns>
        public static bool operator !=(Vector3D left, Vector3D right)
        {
            return !(left.X == right.X && left.Y == right.Y && left.Z == right.Z);
        }

        public static explicit operator Vector3D(Vector2D source)
        {
            Vector3D result;
            result.X = source.X;
            result.Y = source.Y;
            result.Z = 0;
            return result;
        }

        public static explicit operator Vector3D(Vector4D source)
        {
            Vector3D result;
            result.X = source.X;
            result.Y = source.Y;
            result.Z = source.Z;
            return result;
        }
        #endregion
        #region overrides
        private string ToStringInternal(string FormatString)
        {
            return string.Format(FormatString, X, Y, Z);
        }
        public string ToString(string format)
        {
            return ToStringInternal(string.Format(FormatableString, format));
        }
        public override string ToString()
        {
            return ToStringInternal(FormatString);
        }

#if !CompactFramework && !WindowsCE && !PocketPC && !XBOX360 
        public static bool TryParse(string s, out Vector3D result)
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
                Scalar.TryParse(vals[1], out result.Y) &&
                Scalar.TryParse(vals[2], out result.Z))
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
        public static Vector3D Parse(string s)
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
            Vector3D value;
            value.X = Scalar.Parse(vals[0]);
            value.Y = Scalar.Parse(vals[1]);
            value.Z = Scalar.Parse(vals[2]);
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
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }

        /// <summary>
        ///		Compares this Vector to another object.  This should be done because the 
        ///		equality operators (==, !=) have been overriden by this class.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return (obj is Vector3D) && Equals((Vector3D)obj);
        }
        public bool Equals(Vector3D other)
        {
            return Equals(ref this, ref other);
        }
        public static bool Equals(Vector3D left, Vector3D right)
        {
            return
                left.X == right.X &&
                left.Y == right.Y &&
                left.Z == right.Z;
        }
        [CLSCompliant(false)]
        public static bool Equals(ref Vector3D left, ref Vector3D right)
        {
            return
                left.X == right.X &&
                left.Y == right.Y &&
                left.Z == right.Z;
        }
        #endregion
    }
}