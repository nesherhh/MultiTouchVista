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
using System.Xml.Serialization;

using AdvanceMath.Design ;
namespace AdvanceMath
{

    /// <summary>
    /// This is the Vector Class.
    /// </summary>
    /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29"/></remarks>
    [StructLayout(LayoutKind.Sequential, Size = Point2D.Size)]
    [AdvBrowsableOrder("X,Y"), Serializable]
#if !CompactFramework && !WindowsCE && !PocketPC && !XBOX360 
    [System.ComponentModel.TypeConverter(typeof(AdvTypeConverter<Point2D>))]
#endif
    public struct Point2D : IEquatable<Point2D>
    {
        #region const fields
        /// <summary>
        /// The number of int values in the class.
        /// </summary>
        public const int Count = 2;
        /// <summary>
        /// The Size of the class in bytes;
        /// </summary>
        public const int Size = sizeof(int) * Count;
        #endregion
        #region readonly fields
        /// <summary>
        /// Point(0,0)
        /// </summary>
        public static readonly Point2D Zero = new Point2D();
        private static readonly string FormatString = MatrixHelper.CreateVectorFormatString(Count);
        private readonly static string FormatableString = MatrixHelper.CreateVectorFormatableString(Count);
        #endregion
        #region static methods
        /// <summary>
        /// Adds 2 Vectors2Ds.
        /// </summary>
        /// <param name="left">The left Point operand.</param>
        /// <param name="right">The right Point operand.</param>
        /// <returns>The Sum of the 2 Points.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Vector_addition_and_subtraction"/></remarks>
        public static Point2D Add(Point2D left, Point2D right)
        {
            Point2D result;
            result.X = left.X + right.X;
            result.Y = left.Y + right.Y;
            return result;
        }
        public static void Add(ref Point2D left, ref  Point2D right, out Point2D result)
        {
            result.X = left.X + right.X;
            result.Y = left.Y + right.Y;
        }
        /// <summary>
        /// Subtracts 2 Points.
        /// </summary>
        /// <param name="left">The left Point operand.</param>
        /// <param name="right">The right Point operand.</param>
        /// <returns>The Difference of the 2 Points.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Vector_addition_and_subtraction"/></remarks>
        public static Point2D Subtract(Point2D left, Point2D right)
        {
            Point2D result;
            result.X = left.X - right.X;
            result.Y = left.Y - right.Y;
            return result;
        }
        public static void Subtract(ref Point2D left, ref  Point2D right, out Point2D result)
        {
            result.X = left.X - right.X;
            result.Y = left.Y - right.Y;
        }
        /// <summary>
        /// Does Scaler Multiplication on a Point.
        /// </summary>
        /// <param name="scalar">The scalar value that will multiply the Point.</param>
        /// <param name="source">The Point to be multiplied.</param>
        /// <returns>The Product of the Scaler Multiplication.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#int_multiplication"/></remarks>
        public static Point2D Multiply(Point2D source, int scalar)
        {
            Point2D result;
            result.X = source.X * scalar;
            result.Y = source.Y * scalar;
            return result;
        }
        public static void Multiply(ref Point2D source, ref  int scalar, out Point2D result)
        {
            result.X = source.X * scalar;
            result.Y = source.Y * scalar;
        }

        public static Point2D Multiply(int scalar, Point2D source)
        {
            Point2D result;
            result.X = scalar * source.X;
            result.Y = scalar * source.Y;
            return result;
        }
        public static void Multiply(ref int scalar, ref  Point2D source, out Point2D result)
        {
            result.X = scalar * source.X;
            result.Y = scalar * source.Y;
        }

        /// <summary>
        /// Negates a Point.
        /// </summary>
        /// <param name="source">The Point to be Negated.</param>
        /// <returns>The Negated Point.</returns>
        public static Point2D Negate(Point2D source)
        {
            Point2D result;
            result.X = -source.X;
            result.Y = -source.Y;
            return result;
        }
        [CLSCompliant(false)]
        public static void Negate(ref Point2D source)
        {
            Negate(ref source, out source);
        }
        public static void Negate(ref Point2D source, out Point2D result)
        {
            result.X = -source.X;
            result.Y = -source.Y;
        }

        public static Point2D Max(Point2D value1, Point2D value2)
        {
            Point2D result;
            Max(ref value1, ref value2, out result);
            return result;
        }
        public static void Max(ref Point2D value1, ref Point2D value2, out Point2D result)
        {
            result.X = (value1.X < value2.X) ? (value2.X) : (value1.X);
            result.Y = (value1.Y < value2.Y) ? (value2.Y) : (value1.Y);
        }

        public static Point2D Min(Point2D value1, Point2D value2)
        {
            Point2D result;
            Min(ref value1, ref value2, out result);
            return result;
        }
        public static void Min(ref Point2D value1, ref Point2D value2, out Point2D result)
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
        public int X;
        /// <summary>
        /// This is the Y value. (Usually represents a vertical position or direction.)
        /// </summary>
        [AdvBrowsable]
        [XmlAttribute]
        [System.ComponentModel.Description("The Magnitude on the Y-Axis")]
        public int Y;
        #endregion
        #region constructors
        /// <summary>
        /// Creates a New Point Instance on the Stack.
        /// </summary>
        /// <param name="X">The X value.</param>
        /// <param name="Y">The Y value.</param>
        [InstanceConstructor("X,Y")]
        public Point2D(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }
        #endregion
        #region operators
        /// <summary>
        /// Adds 2 Vectors2Ds.
        /// </summary>
        /// <param name="left">The left Point operand.</param>
        /// <param name="right">The right Point operand.</param>
        /// <returns>The Sum of the 2 Points.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Vector_addition_and_subtraction"/></remarks>
        public static Point2D operator +(Point2D left, Point2D right)
        {
            Point2D result;
            result.X = left.X + right.X;
            result.Y = left.Y + right.Y;
            return result;
        }
        /// <summary>
        /// Subtracts 2 Points.
        /// </summary>
        /// <param name="left">The left Point operand.</param>
        /// <param name="right">The right Point operand.</param>
        /// <returns>The Difference of the 2 Points.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Vector_addition_and_subtraction"/></remarks>
        public static Point2D operator -(Point2D left, Point2D right)
        {
            Point2D result;
            result.X = left.X - right.X;
            result.Y = left.Y - right.Y;
            return result;
        }
        /// <summary>
        /// Does Scaler Multiplication on a Point.
        /// </summary>
        /// <param name="source">The Point to be multiplied.</param>
        /// <param name="scalar">The scalar value that will multiply the Point.</param>
        /// <returns>The Product of the Scaler Multiplication.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#int_multiplication"/></remarks>
        public static Point2D operator *(Point2D source, int scalar)
        {
            Point2D result;
            result.X = source.X * scalar;
            result.Y = source.Y * scalar;
            return result;
        }
        /// <summary>
        /// Does Scaler Multiplication on a Point.
        /// </summary>
        /// <param name="scalar">The scalar value that will multiply the Point.</param>
        /// <param name="source">The Point to be multiplied.</param>
        /// <returns>The Product of the Scaler Multiplication.</returns>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#int_multiplication"/></remarks>
        public static Point2D operator *(int scalar, Point2D source)
        {
            Point2D result;
            result.X = scalar * source.X;
            result.Y = scalar * source.Y;
            return result;
        }
        /// <summary>
        /// Negates a Point.
        /// </summary>
        /// <param name="source">The Point to be Negated.</param>
        /// <returns>The Negated Point.</returns>
        public static Point2D operator -(Point2D source)
        {
            Point2D result;
            result.X = -source.X;
            result.Y = -source.Y;
            return result;
        }

        /// <summary>
        /// Specifies whether the Points contain the same coordinates.
        /// </summary>
        /// <param name="left">The left Point to test.</param>
        /// <param name="right">The right Point to test.</param>
        /// <returns>true if the Points have the same coordinates; otherwise false</returns>
        public static bool operator ==(Point2D left, Point2D right)
        {
            return left.X == right.X && left.Y == right.Y;
        }
        /// <summary>
        /// Specifies whether the Points do not contain the same coordinates.
        /// </summary>
        /// <param name="left">The left Point to test.</param>
        /// <param name="right">The right Point to test.</param>
        /// <returns>true if the Points do not have the same coordinates; otherwise false</returns>
        public static bool operator !=(Point2D left, Point2D right)
        {
            return !(left.X == right.X && left.Y == right.Y);
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
        public static bool TryParse(string s, out Point2D result)
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
            if (int.TryParse(vals[0], out result.X) &&
                int.TryParse(vals[1], out result.Y))
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
        public static Point2D Parse(string s)
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
            Point2D value;
            value.X = int.Parse(vals[0]);
            value.Y = int.Parse(vals[1]);
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
            return (obj is Point2D) && Equals((Point2D)obj);
        }
        public bool Equals(Point2D other)
        {
            return Equals(ref this, ref other);
        }
        public static bool Equals(Point2D left, Point2D right)
        {
            return
                left.X == right.X &&
                left.Y == right.Y;
        }
        [CLSCompliant(false)]
        public static bool Equals(ref Point2D left, ref Point2D right)
        {
            return
                left.X == right.X &&
                left.Y == right.Y;
        }
        #endregion
    }
}