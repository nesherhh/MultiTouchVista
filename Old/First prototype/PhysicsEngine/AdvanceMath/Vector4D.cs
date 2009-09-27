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
    /// A Vector with 4 dimensions.
    /// </summary>
    /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29"/></remarks>
    //[StructLayout(LayoutKind.Sequential), Serializable]
    [StructLayout(LayoutKind.Sequential, Size = Vector4D.Size)]
    [AdvBrowsableOrder("X,Y,Z,W"), Serializable]
#if !CompactFramework && !WindowsCE && !PocketPC && !XBOX360 
    [System.ComponentModel.TypeConverter(typeof(AdvTypeConverter<Vector4D>))]
#endif
    public struct Vector4D : IVector<Vector4D>
    {
        #region const fields
        /// <summary>
        /// The number of Scalar values in the class.
        /// </summary>
        public const int Count = 4;
        /// <summary>
        /// The Size of the class in bytes;
        /// </summary>
        public const int Size = sizeof(Scalar) * Count;
        #endregion
        #region readonly fields
        /// <summary>
        /// Vector4D(0,0,0,0)
        /// </summary>
        public static readonly Vector4D Origin = new Vector4D();
        /// <summary>
        /// Vector4D(0,0,0,0)
        /// </summary>
        public static readonly Vector4D Zero = new Vector4D();
        /// <summary>
        /// Vector4D(1,0,0,0)
        /// </summary>
        public static readonly Vector4D XAxis = new Vector4D(1, 0, 0, 0);
        /// <summary>
        /// Vector4D(0,1,0,0)
        /// </summary>
        public static readonly Vector4D YAxis = new Vector4D(0, 1, 0, 0);
        /// <summary>
        /// Vector4D(0,0,1,0)
        /// </summary>
        public static readonly Vector4D ZAxis = new Vector4D(0, 0, 1, 0);
        /// <summary>
        /// Vector4D(0,0,0,1)
        /// </summary>
        public static readonly Vector4D WAxis = new Vector4D(0, 0, 0, 1);

        private static readonly string FormatString = MatrixHelper.CreateVectorFormatString(Count);
        private readonly static string FormatableString = MatrixHelper.CreateVectorFormatableString(Count);

        #endregion
        #region static methods
        public static void Copy(ref Vector4D vector, Scalar[] destArray)
        {
            Copy(ref vector, destArray, 0);
        }
        public static void Copy(ref Vector4D vector, Scalar[] destArray, int index)
        {
            ThrowHelper.CheckCopy(destArray, index, Count);

            destArray[index] = vector.X;
            destArray[++index] = vector.Y;
            destArray[++index] = vector.Z;
            destArray[++index] = vector.W;
        }
        public static void Copy(Scalar[] sourceArray, out Vector4D result)
        {
            Copy(sourceArray, 0, out result);
        }
        public static void Copy(Scalar[] sourceArray, int index, out Vector4D result)
        {
            ThrowHelper.CheckCopy(sourceArray, index, Count);

            result.X = sourceArray[index];
            result.Y = sourceArray[++index];
            result.Z = sourceArray[++index];
            result.W = sourceArray[++index];
        }
        public static void Copy(ref Vector3D source, ref Vector4D dest)
        {
            dest.X = source.X;
            dest.Y = source.Y;
            dest.Z = source.Z;
        }
        public static void Copy(ref Vector2D source, ref Vector4D dest)
        {
            dest.X = source.X;
            dest.Y = source.Y;
        }

        public static Vector4D Clamp(Vector4D value, Vector4D min, Vector4D max)
        {
            Vector4D result;
            MathHelper.Clamp(ref value.X, ref  min.X, ref  max.X, out result.X);
            MathHelper.Clamp(ref value.Y, ref  min.Y, ref  max.Y, out result.Y);
            MathHelper.Clamp(ref value.Z, ref  min.Z, ref  max.Z, out result.Z);
            MathHelper.Clamp(ref value.W, ref  min.W, ref  max.W, out result.W);
            return result;
        }
        public static void Clamp(ref Vector4D value, ref Vector4D min, ref Vector4D max, out Vector4D result)
        {
            MathHelper.Clamp(ref value.X, ref  min.X, ref  max.X, out result.X);
            MathHelper.Clamp(ref value.Y, ref  min.Y, ref  max.Y, out result.Y);
            MathHelper.Clamp(ref value.Z, ref  min.Z, ref  max.Z, out result.Z);
            MathHelper.Clamp(ref value.W, ref  min.W, ref  max.W, out result.W);
        }


        public static Vector4D Lerp(Vector4D left, Vector4D right, Scalar amount)
        {
            Vector4D result;
            Lerp(ref left, ref right, ref amount, out result);
            return result;
        }
        public static void Lerp(ref Vector4D left, ref Vector4D right, ref Scalar amount, out Vector4D result)
        {
            result.X = (right.X - left.X) * amount + left.X;
            result.Y = (right.Y - left.Y) * amount + left.Y;
            result.Z = (right.Z - left.Z) * amount + left.Z;
            result.W = (right.W - left.W) * amount + left.W;
        }
        public static Vector4D Lerp(Vector4D left, Vector4D right, Vector4D amount)
        {
            Vector4D result;
            Lerp(ref left, ref right, ref amount, out result);
            return result;
        }
        public static void Lerp(ref Vector4D left, ref Vector4D right, ref Vector4D amount, out Vector4D result)
        {
            result.X = (right.X - left.X) * amount.X + left.X;
            result.Y = (right.Y - left.Y) * amount.Y + left.Y;
            result.Z = (right.Z - left.Z) * amount.Z + left.Z;
            result.W = (right.W - left.W) * amount.W + left.W;
        }

        public static Scalar Distance(Vector4D left, Vector4D right)
        {
            Scalar result;
            Distance(ref left, ref right, out result);
            return result;
        }
        public static void Distance(ref Vector4D left, ref Vector4D right, out Scalar result)
        {
            Vector4D diff;
            Subtract(ref left, ref right, out diff);
            GetMagnitude(ref diff, out result);
        }
        public static Scalar DistanceSq(Vector4D left, Vector4D right)
        {
            Scalar result;
            DistanceSq(ref left, ref right, out result);
            return result;
        }
        public static void DistanceSq(ref Vector4D left, ref Vector4D right, out Scalar result)
        {
            Vector4D diff;
            Subtract(ref left, ref right, out diff);
            GetMagnitudeSq(ref diff, out result);
        }

        /// <summary>
        /// Adds 2 Vectors2Ds.
        /// </summary>
        /// <param name="left">The left Vector4D operand.</param>
        /// <param name="right">The right Vector4D operand.</param>
        /// <returns>The Sum of the 2 Vector4Ds.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Vector_addition_and_subtraction"/></remarks>
        public static Vector4D Add(Vector4D left, Vector4D right)
        {
            Vector4D result;
            result.X = left.X + right.X;
            result.Y = left.Y + right.Y;
            result.Z = left.Z + right.Z;
            result.W = left.W + right.W;
            return result;
        }
        public static void Add(ref Vector4D left, ref Vector4D right, out Vector4D result)
        {
            result.X = left.X + right.X;
            result.Y = left.Y + right.Y;
            result.Z = left.Z + right.Z;
            result.W = left.W + right.W;
        }

        public static Vector4D Add(Vector3D left, Vector4D right)
        {
            Vector4D result;
            Add(ref left, ref right, out result);
            return result;
        }
        public static void Add(ref Vector3D left, ref Vector4D right, out Vector4D result)
        {
            result.X = left.X + right.X;
            result.Y = left.Y + right.Y;
            result.Z = left.Z + right.Z;
            result.W = right.W;
        }
        public static Vector4D Add(Vector2D left, Vector4D right)
        {
            Vector4D result;
            Add(ref left, ref right, out result);
            return result;
        }
        public static void Add(ref Vector2D left, ref Vector4D right, out Vector4D result)
        {
            result.X = left.X + right.X;
            result.Y = left.Y + right.Y;
            result.Z = right.Z;
            result.W = right.W;
        }
        public static Vector4D Add(Vector4D left, Vector3D right)
        {
            Vector4D result;
            Add(ref left, ref right, out result);
            return result;
        }
        public static void Add(ref Vector4D left, ref Vector3D right, out Vector4D result)
        {
            result.X = left.X + right.X;
            result.Y = left.Y + right.Y;
            result.Z = left.Z + right.Z;
            result.W = left.W;
        }
        public static Vector4D Add(Vector4D left, Vector2D right)
        {
            Vector4D result;
            Add(ref left, ref right, out result);
            return result;
        }
        public static void Add(ref Vector4D left, ref Vector2D right, out Vector4D result)
        {
            result.X = left.X + right.X;
            result.Y = left.Y + right.Y;
            result.Z = left.Z;
            result.W = left.W;
        }



        /// <summary>
        /// Subtracts 2 Vector4Ds.
        /// </summary>
        /// <param name="left">The left Vector4D operand.</param>
        /// <param name="right">The right Vector4D operand.</param>
        /// <returns>The Difference of the 2 Vector4Ds.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Vector_addition_and_subtraction"/></remarks>
        public static Vector4D Subtract(Vector4D left, Vector4D right)
        {
            Vector4D result;
            result.X = left.X - right.X;
            result.Y = left.Y - right.Y;
            result.Z = left.Z - right.Z;
            result.W = left.W - right.W;
            return result;
        }
        public static void Subtract(ref Vector4D left, ref  Vector4D right, out Vector4D result)
        {
            result.X = left.X - right.X;
            result.Y = left.Y - right.Y;
            result.Z = left.Z - right.Z;
            result.W = left.W - right.W;
        }


        public static Vector4D Subtract(Vector3D left, Vector4D right)
        {
            Vector4D result;
            Subtract(ref left, ref right, out result);
            return result;
        }
        public static void Subtract(ref Vector3D left, ref Vector4D right, out Vector4D result)
        {
            result.X = left.X - right.X;
            result.Y = left.Y - right.Y;
            result.Z = left.Z - right.Z;
            result.W = -right.W;
        }
        public static Vector4D Subtract(Vector2D left, Vector4D right)
        {
            Vector4D result;
            Subtract(ref left, ref right, out result);
            return result;
        }
        public static void Subtract(ref Vector2D left, ref Vector4D right, out Vector4D result)
        {
            result.X = left.X - right.X;
            result.Y = left.Y - right.Y;
            result.Z = -right.Z;
            result.W = -right.W;
        }
        public static Vector4D Subtract(Vector4D left, Vector3D right)
        {
            Vector4D result;
            Subtract(ref left, ref right, out result);
            return result;
        }
        public static void Subtract(ref Vector4D left, ref Vector3D right, out Vector4D result)
        {
            result.X = left.X - right.X;
            result.Y = left.Y - right.Y;
            result.Z = left.Z - right.Z;
            result.W = left.W;
        }
        public static Vector4D Subtract(Vector4D left, Vector2D right)
        {
            Vector4D result;
            Subtract(ref left, ref right, out result);
            return result;
        }
        public static void Subtract(ref Vector4D left, ref Vector2D right, out Vector4D result)
        {
            result.X = left.X - right.X;
            result.Y = left.Y - right.Y;
            result.Z = left.Z;
            result.W = left.W;
        }


        /// <summary>
        /// Does Scaler Multiplication on a Vector4D.
        /// </summary>
        /// <param name="source">The Vector4D to be multiplied.</param>
        /// <param name="scalar">The scalar value that will multiply the Vector4D.</param>
        /// <returns>The Product of the Scaler Multiplication.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Scalar_multiplication"/></remarks>
        public static Vector4D Multiply(Vector4D source, Scalar scalar)
        {
            Vector4D result;
            result.X = source.X * scalar;
            result.Y = source.Y * scalar;
            result.Z = source.Z * scalar;
            result.W = source.W * scalar;
            return result;
        }
        public static void Multiply(ref Vector4D source, ref Scalar scalar, out Vector4D result)
        {
            result.X = source.X * scalar;
            result.Y = source.Y * scalar;
            result.Z = source.Z * scalar;
            result.W = source.W * scalar;
        }
        /// <summary>
        /// Does Scaler Multiplication on a Vector4D.
        /// </summary>
        /// <param name="scalar">The scalar value that will multiply the Vector4D.</param>
        /// <param name="source">The Vector4D to be multiplied.</param>
        /// <returns>The Product of the Scaler Multiplication.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Scalar_multiplication"/></remarks>
        public static Vector4D Multiply(Scalar scalar, Vector4D source)
        {
            Vector4D result;
            result.X = scalar * source.X;
            result.Y = scalar * source.Y;
            result.Z = scalar * source.Z;
            result.W = scalar * source.W;
            return result;
        }
        public static void Multiply(ref Scalar scalar, ref Vector4D source, out Vector4D result)
        {
            result.X = scalar * source.X;
            result.Y = scalar * source.Y;
            result.Z = scalar * source.Z;
            result.W = scalar * source.W;
        }

        public static Vector4D Transform(Matrix4x4 matrix, Vector4D vector)
        {
            Vector4D result;

            result.X = vector.X * matrix.m00 + vector.Y * matrix.m01 + vector.Z * matrix.m02 + vector.W * matrix.m03;
            result.Y = vector.X * matrix.m10 + vector.Y * matrix.m11 + vector.Z * matrix.m12 + vector.W * matrix.m13;
            result.Z = vector.X * matrix.m20 + vector.Y * matrix.m21 + vector.Z * matrix.m22 + vector.W * matrix.m23;
            result.W = vector.X * matrix.m30 + vector.Y * matrix.m31 + vector.Z * matrix.m32 + vector.W * matrix.m33;

            return result;
        }
        public static void Transform(ref Matrix4x4 matrix, ref Vector4D vector, out Vector4D result)
        {
            Scalar X = vector.X;
            Scalar Y = vector.Y;
            Scalar Z = vector.Z;
            result.X = X * matrix.m00 + Y * matrix.m01 + Z * matrix.m02 + vector.W * matrix.m03;
            result.Y = X * matrix.m10 + Y * matrix.m11 + Z * matrix.m12 + vector.W * matrix.m13;
            result.Z = X * matrix.m20 + Y * matrix.m21 + Z * matrix.m22 + vector.W * matrix.m23;
            result.W = X * matrix.m30 + Y * matrix.m31 + Z * matrix.m32 + vector.W * matrix.m33;
        }

        public static Vector4D Transform(Vector4D vector, Matrix4x4 matrix)
        {
            Vector4D result;

            result.X = vector.X * matrix.m00 + vector.Y * matrix.m10 + vector.Z * matrix.m20 + vector.W * matrix.m30;
            result.Y = vector.X * matrix.m01 + vector.Y * matrix.m11 + vector.Z * matrix.m21 + vector.W * matrix.m31;
            result.Z = vector.X * matrix.m02 + vector.Y * matrix.m12 + vector.Z * matrix.m22 + vector.W * matrix.m32;
            result.W = vector.X * matrix.m03 + vector.Y * matrix.m13 + vector.Z * matrix.m23 + vector.W * matrix.m33;

            return result;
        }
        public static void Transform(ref Vector4D vector, ref Matrix4x4 matrix, out Vector4D result)
        {
            Scalar X = vector.X;
            Scalar Y = vector.Y;
            Scalar Z = vector.Z;
            result.X = X * matrix.m00 + Y * matrix.m10 + Z * matrix.m20 + vector.W * matrix.m30;
            result.Y = X * matrix.m01 + Y * matrix.m11 + Z * matrix.m21 + vector.W * matrix.m31;
            result.Z = X * matrix.m02 + Y * matrix.m12 + Z * matrix.m22 + vector.W * matrix.m32;
            result.W = X * matrix.m03 + Y * matrix.m13 + Z * matrix.m23 + vector.W * matrix.m33;
        }

        /// <summary>
        /// Does a Dot Operation Also know as an Inner Product.
        /// </summary>
        /// <param name="left">The left Vector4D operand.</param>
        /// <param name="right">The right Vector4D operand.</param>
        /// <returns>The Dot Product (Inner Product).</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Dot_product"/></remarks>
        public static Scalar Dot(Vector4D left, Vector4D right)
        {
            return left.Y * right.Y + left.X * right.X + left.Z * right.Z + left.W * right.W;
        }
        public static void Dot(ref Vector4D left, ref Vector4D right, out Scalar result)
        {
            result = left.Y * right.Y + left.X * right.X + left.Z * right.Z + left.W * right.W;
        }
        /// <summary>
        /// Gets the Squared <see cref="Magnitude"/> of the Vector4D that is passed.
        /// </summary>
        /// <param name="source">The Vector4D whos Squared Magnitude is te be returned.</param>
        /// <returns>The Squared Magnitude.</returns>
        public static Scalar GetMagnitudeSq(Vector4D source)
        {
            return source.X * source.X + source.Y * source.Y + source.Z * source.Z + source.W * source.W;
        }
        public static void GetMagnitudeSq(ref Vector4D source, out Scalar result)
        {
            result = source.X * source.X + source.Y * source.Y + source.Z * source.Z + source.W * source.W;
        }
        /// <summary>
        /// Gets the <see cref="Magnitude"/> of the Vector4D that is passed.
        /// </summary>
        /// <param name="source">The Vector4D whos Magnitude is te be returned.</param>
        /// <returns>The Magnitude.</returns>
        public static Scalar GetMagnitude(Vector4D source)
        {
            return MathHelper.Sqrt(source.X * source.X + source.Y * source.Y + source.Z * source.Z + source.W * source.W);
        }
        public static void GetMagnitude(ref Vector4D source, out Scalar result)
        {
            result = MathHelper.Sqrt(source.X * source.X + source.Y * source.Y + source.Z * source.Z + source.W * source.W);
        }
        /// <summary>
        /// Sets the <see cref="Magnitude"/> of a Vector4D.
        /// </summary>
        /// <param name="source">The Vector4D whose Magnitude is to be changed.</param>
        /// <param name="magnitude">The Magnitude.</param>
        /// <returns>A Vector4D with the new Magnitude</returns>
        public static Vector4D SetMagnitude(Vector4D source, Scalar magnitude)
        {
            Vector4D result;
            SetMagnitude(ref source, ref magnitude, out result);
            return result;
        }
        public static void SetMagnitude(ref Vector4D source, ref Scalar magnitude, out Vector4D result)
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
        /// Negates a Vector4D.
        /// </summary>
        /// <param name="source">The Vector4D to be Negated.</param>
        /// <returns>The Negated Vector4D.</returns>
        public static Vector4D Negate(Vector4D source)
        {
            Vector4D result;
            result.X = -source.X;
            result.Y = -source.Y;
            result.Z = -source.Z;
            result.W = -source.W;
            return result;
        }
        [CLSCompliant(false)]
        public static void Negate(ref Vector4D source)
        {
            Negate(ref source, out source);
        }
        public static void Negate(ref Vector4D source, out Vector4D result)
        {
            result.X = -source.X;
            result.Y = -source.Y;
            result.Z = -source.Z;
            result.W = -source.W;
        }
        /// <summary>
        /// This returns the Normalized Vector4D that is passed. This is also known as a Unit Vector.
        /// </summary>
        /// <param name="source">The Vector4D to be Normalized.</param>
        /// <returns>The Normalized Vector4D. (Unit Vector)</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Unit_vector"/></remarks>
        public static Vector4D Normalize(Vector4D source)
        {
            Scalar oldmagnitude;
            GetMagnitude(ref source, out oldmagnitude);
            if (oldmagnitude == 0) { return Zero; }
            oldmagnitude = (1 / oldmagnitude);
            Vector4D result;
            result.X = source.X * oldmagnitude;
            result.Y = source.Y * oldmagnitude;
            result.Z = source.Z * oldmagnitude;
            result.W = source.W * oldmagnitude;
            return result;
        }
        public static void Normalize(ref Vector4D source, out Vector4D result)
        {
            Scalar oldmagnitude;
            GetMagnitude(ref source, out oldmagnitude);
            if (oldmagnitude == 0) { result = Zero; return; }
            oldmagnitude = (1 / oldmagnitude);
            result.X = source.X * oldmagnitude;
            result.Y = source.Y * oldmagnitude;
            result.Z = source.Z * oldmagnitude;
            result.W = source.W * oldmagnitude;
        }
        [CLSCompliant(false)]
        public static void Normalize(ref Vector4D source)
        {
            Normalize(ref source, out source);
        }
        /// <summary>
        /// Thie Projects the left Vector4D onto the Right Vector4D.
        /// </summary>
        /// <param name="left">The left Vector4D operand.</param>
        /// <param name="right">The right Vector4D operand.</param>
        /// <returns>The Projected Vector4D.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Projection_%28linear_algebra%29"/></remarks>
        public static Vector4D Project(Vector4D left, Vector4D right)
        {
            Vector4D result;
            Project(ref left, ref right, out result);
            return result;
        }
        public static void Project(ref Vector4D left, ref Vector4D right, out Vector4D result)
        {
            Scalar tmp, magsq;
            Dot(ref left, ref  right, out tmp);
            GetMagnitudeSq(ref right, out magsq);
            tmp /= magsq;
            Multiply(ref right, ref tmp, out result);
        }



        public static Vector4D TripleCross(Vector4D top, Vector4D middle, Vector4D bottom)
        {
            Vector4D result;

            result.X = Matrix3x3.GetDeterminant(
                top.Y, top.Z, top.W,
                 middle.Y, middle.Z, middle.W,
                 bottom.Y, bottom.Z, bottom.W);

            result.Y = -Matrix3x3.GetDeterminant(
                top.X, top.Z, top.W,
                middle.X, middle.Z, middle.W,
                bottom.X, bottom.Z, bottom.W);

            result.Z = Matrix3x3.GetDeterminant(
                top.X, top.Y, top.W,
                middle.X, middle.Y, middle.W,
                bottom.X, bottom.Y, bottom.W);

            result.W = -Matrix3x3.GetDeterminant(
                top.X, top.Y, top.Z,
                middle.X, middle.Y, middle.Z,
                bottom.X, bottom.Y, bottom.Z);

            return result;
        }

        public static Vector4D CatmullRom( Vector4D value1,  Vector4D value2,  Vector4D value3,  Vector4D value4, Scalar amount)
        {
            Vector4D result;
            CatmullRom(ref value1, ref value2, ref value3, ref value4, amount, out result);
            return result;
        }
        public static void CatmullRom(ref Vector4D value1, ref Vector4D value2, ref Vector4D value3, ref Vector4D value4, Scalar amount, out Vector4D result)
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
            result.W =
                0.5f * ((2 * value2.W) +
                (-value1.W + value3.W) * amount +
                (2 * value1.W - 5 * value2.W + 4 * value3.W - value4.W) * amountSq +
                (-value1.W + 3 * value2.W - 3 * value3.W + value4.W) * amountCu);
        }

        public static Vector4D Max(Vector4D value1, Vector4D value2)
        {
            Vector4D result;
            Max(ref value1, ref value2, out result);
            return result;
        }
        public static void Max(ref Vector4D value1,ref Vector4D value2,out Vector4D result)
        {
            result.X = (value1.X < value2.X) ? (value2.X) : (value1.X);
            result.Y = (value1.Y < value2.Y) ? (value2.Y) : (value1.Y);
            result.Z = (value1.Z < value2.Z) ? (value2.Z) : (value1.Z);
            result.W = (value1.W < value2.W) ? (value2.W) : (value1.W);
        }

        public static Vector4D Min(Vector4D value1, Vector4D value2)
        {
            Vector4D result;
            Min(ref value1, ref value2, out result);
            return result;
        }
        public static void Min(ref Vector4D value1, ref Vector4D value2, out Vector4D result)
        {
            result.X = (value1.X > value2.X) ? (value2.X) : (value1.X);
            result.Y = (value1.Y > value2.Y) ? (value2.Y) : (value1.Y);
            result.Z = (value1.Z > value2.Z) ? (value2.Z) : (value1.Z);
            result.W = (value1.W > value2.W) ? (value2.W) : (value1.W);
        }

        public static Vector4D Hermite(Vector4D value1, Vector4D tangent1, Vector4D value2, Vector4D tangent2, Scalar amount)
        {
            Vector4D result;
            Hermite(ref value1, ref tangent1, ref value2, ref tangent2, amount, out result);
            return result;
        }
        public static void Hermite(ref  Vector4D value1, ref Vector4D tangent1, ref Vector4D value2, ref Vector4D tangent2, Scalar amount, out Vector4D result)
        {
            Scalar h1, h2, h3, h4;
            MathHelper.HermiteHelper(amount, out h1, out h2, out h3, out h4);
            result.X = h1 * value1.X + h2 * value2.X + h3 * tangent1.X + h4 * tangent2.X;
            result.Y = h1 * value1.Y + h2 * value2.Y + h3 * tangent1.Y + h4 * tangent2.Y;
            result.Z = h1 * value1.Z + h2 * value2.Z + h3 * tangent1.Z + h4 * tangent2.Z;
            result.W = h1 * value1.W + h2 * value2.W + h3 * tangent1.W + h4 * tangent2.W;
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
        /// <summary>
        /// This is the W value. 
        /// </summary>
        [AdvBrowsable]
        [XmlAttribute]
        [System.ComponentModel.Description("The Magnitude on the W-Axis")]
        public Scalar W;

        #endregion
        #region constructors
        /// <summary>
        /// Creates a New Vector4D Instance on the Stack.
        /// </summary>
        /// <param name="X">The X value.</param>
        /// <param name="Y">The Y value.</param>
        /// <param name="Z">The Z value.</param>
        /// <param name="W">The W value.</param>
        [InstanceConstructor("X,Y,Z,W")]
        public Vector4D(Scalar X, Scalar Y, Scalar Z, Scalar W)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.W = W;
        }
        public Vector4D(Scalar[] vals) : this(vals, 0) { }
        public Vector4D(Scalar[] vals, int index)
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
        #region properties


        /// <summary>
        /// Gets or Sets the Magnitude (Length) of the Vector4D. 
        /// </summary>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Length_of_a_vector"/></remarks>
        [XmlIgnore]
        public Scalar Magnitude
        {
            get
            {
                return MathHelper.Sqrt(this.X * this.X + this.Y * this.Y + this.Z * this.Z + this.W * this.W);
            }
            set
            {
                this = SetMagnitude(this, value);
            }
        }
        /// <summary>
        /// Gets the Squared Magnitude of the Vector4D.
        /// </summary>
        public Scalar MagnitudeSq
        {
            get
            {
                return this.X * this.X + this.Y * this.Y + this.Z * this.Z + this.W * this.W;
            }
        }
        /// <summary>
        /// Gets the Normalized Vector4D. (Unit Vector)
        /// </summary>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Unit_vector"/></remarks>
        public Vector4D Normalized
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
        /// <param name="left">The left Vector4D operand.</param>
        /// <param name="right">The right Vector4D operand.</param>
        /// <returns>The Sum of the 2 Vector4Ds.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Vector_addition_and_subtraction"/></remarks>
        public static Vector4D operator +(Vector4D left, Vector4D right)
        {
            Vector4D result;
            result.X = left.X + right.X;
            result.Y = left.Y + right.Y;
            result.Z = left.Z + right.Z;
            result.W = left.W + right.W;
            return result;
        }
        public static Vector4D operator +(Vector3D left, Vector4D right)
        {
            Vector4D result;
            Add(ref left, ref right, out result);
            return result;
        }
        public static Vector4D operator +(Vector2D left, Vector4D right)
        {
            Vector4D result;
            Add(ref left, ref right, out result);
            return result;
        }
        public static Vector4D operator +(Vector4D left, Vector3D right)
        {
            Vector4D result;
            Add(ref left, ref right, out result);
            return result;
        }
        public static Vector4D operator +(Vector4D left, Vector2D right)
        {
            Vector4D result;
            Add(ref left, ref right, out result);
            return result;
        }
        /// <summary>
        /// Subtracts 2 Vector4Ds.
        /// </summary>
        /// <param name="left">The left Vector4D operand.</param>
        /// <param name="right">The right Vector4D operand.</param>
        /// <returns>The Difference of the 2 Vector4Ds.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Vector_addition_and_subtraction"/></remarks>
        public static Vector4D operator -(Vector4D left, Vector4D right)
        {
            Vector4D result;
            result.X = left.X - right.X;
            result.Y = left.Y - right.Y;
            result.Z = left.Z - right.Z;
            result.W = left.W - right.W;
            return result;
        }
        public static Vector4D operator -(Vector3D left, Vector4D right)
        {
            Vector4D result;
            Subtract(ref left, ref right, out result);
            return result;
        }
        public static Vector4D operator -(Vector2D left, Vector4D right)
        {
            Vector4D result;
            Subtract(ref left, ref right, out result);
            return result;
        }
        public static Vector4D operator -(Vector4D left, Vector3D right)
        {
            Vector4D result;
            Subtract(ref left, ref right, out result);
            return result;
        }
        public static Vector4D operator -(Vector4D left, Vector2D right)
        {
            Vector4D result;
            Subtract(ref left, ref right, out result);
            return result;
        }
        /// <summary>
        /// Does Scaler Multiplication on a Vector4D.
        /// </summary>
        /// <param name="source">The Vector4D to be multiplied.</param>
        /// <param name="scalar">The scalar value that will multiply the Vector4D.</param>
        /// <returns>The Product of the Scaler Multiplication.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Scalar_multiplication"/></remarks>
        public static Vector4D operator *(Vector4D source, Scalar scalar)
        {
            Vector4D result;
            result.X = source.X * scalar;
            result.Y = source.Y * scalar;
            result.Z = source.Z * scalar;
            result.W = source.W * scalar;
            return result;
        }
        /// <summary>
        /// Does Scaler Multiplication on a Vector4D.
        /// </summary>
        /// <param name="scalar">The scalar value that will multiply the Vector4D.</param>
        /// <param name="source">The Vector4D to be multiplied.</param>
        /// <returns>The Product of the Scaler Multiplication.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Scalar_multiplication"/></remarks>
        public static Vector4D operator *(Scalar scalar, Vector4D source)
        {
            Vector4D result;
            result.X = scalar * source.X;
            result.Y = scalar * source.Y;
            result.Z = scalar * source.Z;
            result.W = scalar * source.W;
            return result;
        }
        /// <summary>
        /// Does a Dot Operation Also know as an Inner Product.
        /// </summary>
        /// <param name="left">The left Vector4D operand.</param>
        /// <param name="right">The right Vector4D operand.</param>
        /// <returns>The Dot Product (Inner Product).</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Dot_product"/></remarks>
        public static Scalar operator *(Vector4D left, Vector4D right)
        {
            return left.X * right.X + left.Y * right.Y + left.Z * right.Z + left.W * right.W;
        }


        public static Vector4D operator *(Matrix4x4 matrix, Vector4D vector)
        {
            Vector4D result;

            result.X = vector.X * matrix.m00 + vector.Y * matrix.m01 + vector.Z * matrix.m02 + vector.W * matrix.m03;
            result.Y = vector.X * matrix.m10 + vector.Y * matrix.m11 + vector.Z * matrix.m12 + vector.W * matrix.m13;
            result.Z = vector.X * matrix.m20 + vector.Y * matrix.m21 + vector.Z * matrix.m22 + vector.W * matrix.m23;
            result.W = vector.X * matrix.m30 + vector.Y * matrix.m31 + vector.Z * matrix.m32 + vector.W * matrix.m33;

            return result;
        }

        public static Vector4D operator *(Vector4D vector, Matrix4x4 matrix)
        {
            Vector4D result;

            result.X = vector.X * matrix.m00 + vector.Y * matrix.m10 + vector.Z * matrix.m20 + vector.W * matrix.m30;
            result.Y = vector.X * matrix.m01 + vector.Y * matrix.m11 + vector.Z * matrix.m21 + vector.W * matrix.m31;
            result.Z = vector.X * matrix.m02 + vector.Y * matrix.m12 + vector.Z * matrix.m22 + vector.W * matrix.m32;
            result.W = vector.X * matrix.m03 + vector.Y * matrix.m13 + vector.Z * matrix.m23 + vector.W * matrix.m33;

            return result;
        }

        /// <summary>
        /// Negates a Vector4D.
        /// </summary>
        /// <param name="source">The Vector4D to be Negated.</param>
        /// <returns>The Negated Vector4D.</returns>
        public static Vector4D operator -(Vector4D source)
        {
            Vector4D result;
            result.X = -source.X;
            result.Y = -source.Y;
            result.Z = -source.Z;
            result.W = -source.W;
            return result;
        }
        /// <summary>
        /// Specifies whether the Vector4Ds contain the same coordinates.
        /// </summary>
        /// <param name="left">The left Vector4D to test.</param>
        /// <param name="right">The right Vector4D to test.</param>
        /// <returns>true if the Vector4Ds have the same coordinates; otherwise false</returns>
        public static bool operator ==(Vector4D left, Vector4D right)
        {
            return left.X == right.X && left.Y == right.Y && left.Z == right.Z && left.W == right.W;
        }
        /// <summary>
        /// Specifies whether the Vector4Ds do not contain the same coordinates.
        /// </summary>
        /// <param name="left">The left Vector4D to test.</param>
        /// <param name="right">The right Vector4D to test.</param>
        /// <returns>true if the Vector4Ds do not have the same coordinates; otherwise false</returns>
        public static bool operator !=(Vector4D left, Vector4D right)
        {
            return !(left.X == right.X && left.Y == right.Y && left.Z == right.Z && left.W == right.W);
        }


        public static explicit operator Vector4D(Vector3D source)
        {
            Vector4D result;
            result.X = source.X;
            result.Y = source.Y;
            result.Z = source.Z;
            result.W = 1;
            return result;
        }

        #endregion
        #region overrides
        private string ToStringInternal(string FormatString)
        {
            return string.Format(FormatString, X, Y, Z, W);
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
        public static bool TryParse(string s, out Vector4D result)
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
                Scalar.TryParse(vals[2], out result.Z) &&
                Scalar.TryParse(vals[3], out result.W))
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
        public static Vector4D Parse(string s)
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
            Vector4D value;
            value.X = Scalar.Parse(vals[0]);
            value.Y = Scalar.Parse(vals[1]);
            value.Z = Scalar.Parse(vals[2]);
            value.W = Scalar.Parse(vals[3]);
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
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ W.GetHashCode();
        }

        /// <summary>
        ///		Compares this Vector to another object.  This should be done because the 
        ///		equality operators (==, !=) have been overriden by this class.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return (obj is Vector4D) && Equals((Vector4D)obj);
        }
        public bool Equals(Vector4D other)
        {
            return Equals(ref this, ref other);
        }
        public static bool Equals(Vector4D left, Vector4D right)
        {
            return
                left.X == right.X &&
                left.Y == right.Y &&
                left.Z == right.Z &&
                left.W == right.W;
        }
        [CLSCompliant(false)]
        public static bool Equals(ref Vector4D left, ref Vector4D right)
        {
            return
                left.X == right.X &&
                left.Y == right.Y &&
                left.Z == right.Z &&
                left.W == right.W;
        }
        #endregion
    }
}