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
using AdvanceMath;
using AdvanceMath.Design;
namespace Physics2DDotNet
{
    /// <summary>
    /// Class Used to store a Linear Value along with an Angular Value. Like Position and Orientation. 
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = ALVector2D.Size), Serializable]
#if !CompactFramework && !WindowsCE && !PocketPC && !XBOX360 
    [System.ComponentModel.TypeConverter(typeof(AdvTypeConverter<ALVector2D>))]
#endif
    [AdvBrowsableOrder("Angular,Linear")]
    public struct ALVector2D : IEquatable<ALVector2D>
    {
        public static ALVector2D Parse(string text)
        {
            string[] vals = text.Trim(' ', '(', '[', '<', ')', ']', '>').Split(new char[] { ',' });
            if (vals.Length != 2)
            {
                throw new FormatException(string.Format("Cannot parse the text '{0}' because it does not have 2 parts separated by commas in the form (x,y) with optional parenthesis.", text));
            }
            else
            {
                try
                {
                    ALVector2D returnvalue;
                    returnvalue.Angular = Scalar.Parse(vals[0]);
                    returnvalue.Linear = Vector2D.Parse(vals[1]);
                    return returnvalue;
                }
                catch (Exception ex)
                {
                    throw new FormatException("The parts of the vectors must be decimal numbers", ex);
                }
            }
        }

        public const int Size = sizeof(Scalar) + Vector2D.Size;
        /// <summary>
        /// ALVector2D(0,Vector2D.Zero)
        /// </summary>
        public static readonly ALVector2D Zero = new ALVector2D(0, Vector2D.Zero);
        /// <summary>
        /// This is the Angular value of this ALVector2D. 
        /// </summary>
        /// <remarks>Example: Angular would be Orientation and Linear would be Location Completly describing a Position.</remarks>
        [AdvBrowsable]
        public Scalar Angular;
        /// <summary>
        /// This is the Linear value of this ALVector2D. 
        /// </summary>
        /// <remarks>Example: Angular would be Orientation and Linear would be Location Completly describing a Position.</remarks>
        [AdvBrowsable]
        public Vector2D Linear;

        /// <summary>
        /// Creates a new ALVector2D instance on the stack.
        /// </summary>
        /// <param name="Angular">The Angular value.</param>
        /// <param name="Linear">The Linear value.</param>
        [InstanceConstructor("Angular,Linear")]
        public ALVector2D(Scalar angular, Vector2D linear)
        {
            this.Angular = angular;
            this.Linear = linear;
        }
        public ALVector2D(Scalar angular, Scalar x, Scalar y)
        {
            this.Angular = angular;
            this.Linear = new Vector2D(x, y);
        }


        public static ALVector2D Add(ALVector2D left, ALVector2D right)
        {
            ALVector2D result;
            result.Angular = left.Angular + right.Angular;
            result.Linear.X = left.Linear.X + right.Linear.X;
            result.Linear.Y = left.Linear.Y + right.Linear.Y;
            return result;
        }
        public static void Add(ref ALVector2D left, ref  ALVector2D right, out ALVector2D result)
        {
            result.Angular = left.Angular + right.Angular;
            result.Linear.X = left.Linear.X + right.Linear.X;
            result.Linear.Y = left.Linear.Y + right.Linear.Y;
        }

        /// <summary>
        /// Does Addition of 2 ALVector2Ds.
        /// </summary>
        /// <param name="left">The left ALVector2D operand.</param>
        /// <param name="right">The right ALVector2D operand.</param>
        /// <returns>The Sum of the ALVector2Ds.</returns>
        public static ALVector2D operator +(ALVector2D left, ALVector2D right)
        {
            ALVector2D result;
            result.Angular = left.Angular + right.Angular;
            result.Linear.X = left.Linear.X + right.Linear.X;
            result.Linear.Y = left.Linear.Y + right.Linear.Y;
            return result;
        }
        /// <summary>
        /// Does Subtraction of 2 ALVector2Ds.
        /// </summary>
        /// <param name="left">The left ALVector2D operand.</param>
        /// <param name="right">The right ALVector2D operand.</param>
        /// <returns>The Difference of the ALVector2Ds.</returns>
        public static ALVector2D operator -(ALVector2D left, ALVector2D right)
        {
            ALVector2D result;
            result.Angular = left.Angular - right.Angular;
            result.Linear.X = left.Linear.X - right.Linear.X;
            result.Linear.Y = left.Linear.Y - right.Linear.Y;
            return result;
        }
        public static ALVector2D Subtract(ALVector2D left, ALVector2D right)
        {
            ALVector2D result;
            result.Angular = left.Angular - right.Angular;
            result.Linear.X = left.Linear.X - right.Linear.X;
            result.Linear.Y = left.Linear.Y - right.Linear.Y;
            return result;
        }
        public static void Subtract(ref ALVector2D left,ref  ALVector2D right, out ALVector2D result)
        {
            result.Angular = left.Angular - right.Angular;
            result.Linear.X = left.Linear.X - right.Linear.X;
            result.Linear.Y = left.Linear.Y - right.Linear.Y;
        }
        /// <summary>
        /// Does Multiplication of 2 ALVector2Ds 
        /// </summary>
        /// <param name="source">The ALVector2D to be Multiplied.</param>
        /// <param name="scalar">The Scalar multiplier.</param>
        /// <returns>The Product of the ALVector2Ds.</returns>
        /// <remarks>It does normal Multiplication of the Angular value but does Scalar Multiplication of the Linear value.</remarks>
        public static ALVector2D operator *(ALVector2D source, Scalar scalar)
        {
            ALVector2D result;
            result.Angular = source.Angular * scalar;
            result.Linear.X = source.Linear.X * scalar;
            result.Linear.Y = source.Linear.Y * scalar;
            return result;
        }
        public static ALVector2D Multiply(ALVector2D source, Scalar scalar)
        {
            ALVector2D result;
            result.Angular = source.Angular * scalar;
            result.Linear.X = source.Linear.X * scalar;
            result.Linear.Y = source.Linear.Y * scalar;
            return result;
        }
        public static void Multiply(ref ALVector2D source,ref Scalar scalar,out ALVector2D result)
        {
            result.Angular = source.Angular * scalar;
            result.Linear.X = source.Linear.X * scalar;
            result.Linear.Y = source.Linear.Y * scalar;
        }
        /// <summary>
        /// Does Multiplication of 2 ALVector2Ds 
        /// </summary>
        /// <param name="scalar">The Scalar multiplier.</param>
        /// <param name="source">The ALVector2D to be Multiplied.</param>
        /// <returns>The Product of the ALVector2Ds.</returns>
        /// <remarks>It does normal Multiplication of the Angular value but does Scalar Multiplication of the Linear value.</remarks>
        public static ALVector2D operator *(Scalar scalar, ALVector2D source)
        {
            ALVector2D result;
            result.Angular = source.Angular * scalar;
            result.Linear.X = source.Linear.X * scalar;
            result.Linear.Y = source.Linear.Y * scalar;
            return result;
        }
        public static ALVector2D Multiply(Scalar scalar, ALVector2D source)
        {
            ALVector2D result;
            result.Angular = source.Angular * scalar;
            result.Linear.X = source.Linear.X * scalar;
            result.Linear.Y = source.Linear.Y * scalar;
            return result;
        }
        public static void Multiply(ref Scalar scalar,ref ALVector2D source, out ALVector2D result)
        {
            result.Angular = source.Angular * scalar;
            result.Linear.X = source.Linear.X * scalar;
            result.Linear.Y = source.Linear.Y * scalar;
        }
        public static void ToMatrix2x3(ref ALVector2D source, out Matrix2x3 result)
        {
            result.m00 = MathHelper.Cos(source.Angular);
            result.m10 = MathHelper.Sin(source.Angular);
            result.m01 = -result.m10;
            result.m11 = result.m00;
            result.m02 = source.Linear.X;
            result.m12 = source.Linear.Y;
        }

       /* public static ALVector2D Transform(Matrix2D matrix, ALVector2D source)
        {
            ALVector2D result;
            Transform(ref matrix,ref source,out result);
            return result;
        }
        public static void Transform(ref Matrix2D matrix, ref ALVector2D source, out ALVector2D result)
        {
            Scalar dAngle;
            Vector2D angular = Vector2D.XAxis;
            Vector2D.Transform(ref matrix.Vertex, ref source.Linear, out result.Linear);
            Vector2D.Transform(ref matrix.Normal, ref angular, out angular);
            Vector2D.GetAngle(ref angular, out dAngle);
            result.Angular = source.Angular + dAngle;
        }*/
        public static void Transform(ref Matrix2x3 matrix, ref ALVector2D source, out ALVector2D result)
        {
            Scalar dAngle;
            Vector2D angular = Vector2D.XAxis;
            Vector2D.Transform(ref matrix, ref source.Linear, out result.Linear);
            Matrix2x2 normal;
            Matrix2x2.CreateNormal(ref matrix, out normal);

            Vector2D.Transform(ref normal, ref angular, out angular);
            Vector2D.GetAngle(ref angular, out dAngle);
            result.Angular = source.Angular + dAngle;
        }
        
        
        public static bool operator ==(ALVector2D left, ALVector2D right)
        {
            return left.Angular == right.Angular &&
                Vector2D.Equals(ref left.Linear, ref right.Linear);
        }
        public static bool operator !=(ALVector2D left, ALVector2D right)
        {
            return !(left.Angular == right.Angular &&
                Vector2D.Equals(ref left.Linear, ref right.Linear));
        }
        public static bool Equals(ALVector2D left, ALVector2D right)
        {
            return left.Angular == right.Angular &&
                Vector2D.Equals(ref left.Linear, ref right.Linear);
        }
        [CLSCompliant(false)]
        public static bool Equals(ref ALVector2D left,ref ALVector2D right)
        {
            return left.Angular == right.Angular &&
                Vector2D.Equals(ref left.Linear, ref right.Linear);
        }
        public override bool Equals(object obj)
        {
            return
                obj is ALVector2D &&
                Equals((ALVector2D)obj);
        }
        public bool Equals(ALVector2D other)
        {
            return Equals(ref this, ref other);
        }
        public override int GetHashCode()
        {
            return Angular.GetHashCode() ^ Linear.GetHashCode();
        }
        public override string ToString()
        {
            return string.Format("({0}, {1})", Angular, Linear);
        }
    }
}
