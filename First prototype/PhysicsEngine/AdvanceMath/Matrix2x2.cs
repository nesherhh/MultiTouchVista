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


// NOTE.  The (x,y) coordinate system is assumed to be right-handed.
// where t > 0 indicates a counterclockwise rotation in the zx-plane
//   RZ =  cos(t) -sin(t)
//         sin(t)  cos(t) 
// where t > 0 indicates a counterclockwise rotation in the xy-plane.

namespace AdvanceMath
{

    /// <summary>
    /// A 2x2 matrix which can represent rotations for 2D vectors.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = Matrix2x2.Size)]
    [AdvBrowsableOrder("Rx,Ry"), Serializable]
#if !CompactFramework && !WindowsCE && !PocketPC && !XBOX360 
    [System.ComponentModel.TypeConverter(typeof(AdvTypeConverter<Matrix2x2>))]
#endif
    public struct Matrix2x2 : IMatrix<Matrix2x2, Vector2D, Vector2D>
    {
        #region const fields
        /// <summary>
        /// The number of rows.
        /// </summary>
        public const int RowCount = 2;
        /// <summary>
        /// The number of columns.
        /// </summary>
        public const int ColumnCount = 2;
        /// <summary>
        /// The number of Scalar values in the class.
        /// </summary>
        public const int Count = RowCount * ColumnCount;
        /// <summary>
        /// The Size of the class in bytes;
        /// </summary>
        public const int Size = sizeof(Scalar) * Count;
        #endregion
        #region static fields
        private readonly static string FormatString = MatrixHelper.CreateMatrixFormatString(RowCount, ColumnCount);
        private readonly static string FormatableString = MatrixHelper.CreateMatrixFormatableString(RowCount, ColumnCount);

        public static readonly Matrix2x2 Identity = new Matrix2x2(
            1, 0,
            0, 1);
        public static readonly Matrix2x2 Zero = new Matrix2x2(
            0, 0,
            0, 0);

        #endregion
        #region static methods
        public static void Copy(ref Matrix2x2 matrix, Scalar[] destArray)
        {
            Copy(ref matrix, destArray, 0);
        }
        public static void Copy(ref Matrix2x2 matrix, Scalar[] destArray, int index)
        {
            ThrowHelper.CheckCopy(destArray, index, Count);

            destArray[index] = matrix.m00;
            destArray[++index] = matrix.m01;

            destArray[++index] = matrix.m10;
            destArray[++index] = matrix.m11;
        }
        public static void Copy(Scalar[] sourceArray, out Matrix2x2 result)
        {
            Copy(sourceArray, 0, out result);
        }
        public static void Copy(Scalar[] sourceArray, int index, out Matrix2x2 result)
        {
            ThrowHelper.CheckCopy(sourceArray, index, Count);

            result.m00 = sourceArray[index];
            result.m01 = sourceArray[++index];

            result.m10 = sourceArray[++index];
            result.m11 = sourceArray[++index];
        }
        public static void CopyTranspose(ref Matrix2x2 matrix, Scalar[] destArray)
        {
            CopyTranspose(ref matrix, destArray, 0);
        }
        public static void CopyTranspose(ref Matrix2x2 matrix, Scalar[] destArray, int index)
        {
            ThrowHelper.CheckCopy(destArray, index, Count);

            destArray[index] = matrix.m00;
            destArray[++index] = matrix.m10;

            destArray[++index] = matrix.m01;
            destArray[++index] = matrix.m11;
        }
        public static void CopyTranspose(Scalar[] sourceArray, out Matrix2x2 result)
        {
            CopyTranspose(sourceArray, 0, out result);
        }
        public static void CopyTranspose(Scalar[] sourceArray, int index, out Matrix2x2 result)
        {
            ThrowHelper.CheckCopy(sourceArray, index, Count);

            result.m00 = sourceArray[index];
            result.m10 = sourceArray[++index];

            result.m01 = sourceArray[++index];
            result.m11 = sourceArray[++index];
        }

        public static void Copy(ref Matrix4x4 source, out Matrix2x2 dest)
        {
            dest.m00 = source.m00;
            dest.m01 = source.m01;

            dest.m10 = source.m10;
            dest.m11 = source.m11;
        }
        public static void Copy(ref Matrix3x3 source, out Matrix2x2 dest)
        {
            dest.m00 = source.m00;
            dest.m01 = source.m01;

            dest.m10 = source.m10;
            dest.m11 = source.m11;
        }
        public static void Copy(ref Matrix2x3 source, out Matrix2x2 dest)
        {
            dest.m00 = source.m00;
            dest.m01 = source.m01;

            dest.m10 = source.m10;
            dest.m11 = source.m11;
        }

        public static Matrix2x2 Lerp(Matrix2x2 left, Matrix2x2 right, Scalar amount)
        {
            Matrix2x2 result;
            Lerp(ref left, ref right, ref amount, out result);
            return result;
        }
        public static void Lerp(ref Matrix2x2 left, ref  Matrix2x2 right, ref  Scalar amount, out Matrix2x2 result)
        {
            result.m00 = (right.m00 - left.m00) * amount + left.m00;
            result.m01 = (right.m01 - left.m01) * amount + left.m01;

            result.m10 = (right.m10 - left.m10) * amount + left.m10;
            result.m11 = (right.m11 - left.m11) * amount + left.m11;
        }

        /// <summary>
        ///		Used to multiply (concatenate) two Matrix4x4s.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix2x2 Multiply(Matrix2x2 left, Matrix2x2 right)
        {
            Matrix2x2 result;

            result.m00 = left.m00 * right.m00 + left.m01 * right.m10;
            result.m01 = left.m00 * right.m01 + left.m01 * right.m11;

            result.m10 = left.m10 * right.m00 + left.m11 * right.m10;
            result.m11 = left.m10 * right.m01 + left.m11 * right.m11;

            return result;
        }
        public static void Multiply(ref Matrix2x2 left, ref Matrix2x2 right, out  Matrix2x2 result)
        {
            Scalar m00 = left.m00 * right.m00 + left.m01 * right.m10;
            Scalar m01 = left.m00 * right.m01 + left.m01 * right.m11;

            Scalar m10 = left.m10 * right.m00 + left.m11 * right.m10;
            Scalar m11 = left.m10 * right.m01 + left.m11 * right.m11;

            result.m00 = m00;
            result.m01 = m01;

            result.m10 = m10;
            result.m11 = m11;

        }

        /// <summary>
        ///		Used to multiply a Matrix2x2 object by a scalar value..
        /// </summary>
        /// <param name="left"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Matrix2x2 Multiply(Matrix2x2 left, Scalar scalar)
        {
            Matrix2x2 result;

            result.m00 = left.m00 * scalar;
            result.m01 = left.m01 * scalar;

            result.m10 = left.m10 * scalar;
            result.m11 = left.m11 * scalar;

            return result;
        }
        public static void Multiply(ref Matrix2x2 left, ref Scalar scalar, out  Matrix2x2 result)
        {
            result.m00 = left.m00 * scalar;
            result.m01 = left.m01 * scalar;

            result.m10 = left.m10 * scalar;
            result.m11 = left.m11 * scalar;
        }

        /// <summary>
        ///		Used to add two matrices together.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix2x2 Add(Matrix2x2 left, Matrix2x2 right)
        {
            Matrix2x2 result;

            result.m00 = left.m00 + right.m00;
            result.m01 = left.m01 + right.m01;

            result.m10 = left.m10 + right.m10;
            result.m11 = left.m11 + right.m11;

            return result;
        }
        public static void Add(ref Matrix2x2 left, ref Matrix2x2 right, out  Matrix2x2 result)
        {
            result.m00 = left.m00 + right.m00;
            result.m01 = left.m01 + right.m01;

            result.m10 = left.m10 + right.m10;
            result.m11 = left.m11 + right.m11;
        }
        /// <summary>
        ///		Used to subtract two matrices.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix2x2 Subtract(Matrix2x2 left, Matrix2x2 right)
        {
            Matrix2x2 result;

            result.m00 = left.m00 - right.m00;
            result.m01 = left.m01 - right.m01;

            result.m10 = left.m10 - right.m10;
            result.m11 = left.m11 - right.m11;

            return result;
        }
        public static void Subtract(ref Matrix2x2 left, ref Matrix2x2 right, out  Matrix2x2 result)
        {
            result.m00 = left.m00 - right.m00;
            result.m01 = left.m01 - right.m01;

            result.m10 = left.m10 - right.m10;
            result.m11 = left.m11 - right.m11;
        }

        /// <summary>
        ///	Negates a Matrix2x2.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix2x2 Negate(Matrix2x2 source)
        {
            Matrix2x2 result;

            result.m00 = -source.m00;
            result.m01 = -source.m01;

            result.m10 = -source.m10;
            result.m11 = -source.m11;

            return result;
        }
        [CLSCompliant(false)]
        public static void Negate(ref Matrix2x2 source)
        {
            Negate(ref source, out source);
        }
        public static void Negate(ref Matrix2x2 source, out Matrix2x2 result)
        {
            result.m00 = -source.m00;
            result.m01 = -source.m01;

            result.m10 = -source.m10;
            result.m11 = -source.m11;
        }

        public static Matrix2x2 Invert(Matrix2x2 source)
        {
            Matrix2x2 result;
            Invert(ref source, out  result);
            return result;
        }
        public static void Invert(ref Matrix2x2 source, out Matrix2x2 result)
        {
            Scalar m11 = source.m11;
            Scalar detInv = 1 / (source.m00 * m11 - source.m01 * source.m10);
            result.m01 = detInv * -source.m01;
            result.m11 = detInv * source.m00;
            result.m00 = detInv * m11;
            result.m10 = detInv * -source.m10;
        }

        public static Scalar GetDeterminant(Matrix2x2 source)
        {
            Scalar result;
            GetDeterminant(ref source, out result);
            return result;
        }
        public static void GetDeterminant(ref Matrix2x2 source, out Scalar result)
        {
            result = (source.m00 * source.m11 - source.m01 * source.m10);
        }

        public static Matrix2x2 Transpose(Matrix2x2 source)
        {
            Matrix2x2 result;
            Transpose(ref source, out result);
            return result;
        }
        public static void Transpose(ref Matrix2x2 source, out Matrix2x2 result)
        {
            Scalar m01 = source.m01;
            result.m00 = source.m00;
            result.m01 = source.m10;
            result.m10 = m01;
            result.m11 = source.m11;
        }

        public static Matrix2x2 GetAdjoint(Matrix2x2 source)
        {
            Matrix2x2 result;
            GetAdjoint(ref source, out  result);
            return result;
        }
        public static void GetAdjoint(ref Matrix2x2 source, out Matrix2x2 result)
        {
            Scalar m11 = source.m11;
            result.m01 = -source.m01;
            result.m11 = source.m00;
            result.m00 = m11;
            result.m10 = -source.m10;
        }

        public static Matrix2x2 GetCofactor(Matrix2x2 source)
        {
            Matrix2x2 result;
            GetCofactor(ref source, out  result);
            return result;
        }
        public static void GetCofactor(ref Matrix2x2 source, out Matrix2x2 result)
        {
            Scalar m11 = source.m11;
            result.m01 = source.m01;
            result.m11 = -source.m00;
            result.m00 = -m11;
            result.m10 = source.m10;
        }



        public static Matrix2x2 FromArray(Scalar[] array)
        {
            Matrix2x2 result;
            Copy(array, 0, out result);
            return result;
        }
        public static Matrix2x2 FromTransposedArray(Scalar[] array)
        {
            Matrix2x2 result;
            CopyTranspose(array, 0, out result);
            return result;
        }

        public static Matrix2x2 FromRotation(Scalar radianAngle)
        {
            Matrix2x2 result;
            result.m00 = MathHelper.Cos(radianAngle);
            result.m10 = MathHelper.Sin(radianAngle);
            result.m01 = -result.m10;
            result.m11 = result.m00;
            return result;
        }
        public static void FromRotation(ref Scalar radianAngle, out Matrix2x2 result)
        {
            result.m00 = MathHelper.Cos(radianAngle);
            result.m10 = MathHelper.Sin(radianAngle);
            result.m01 = -result.m10;
            result.m11 = result.m00;
        }

        public static Matrix2x2 FromScale(Vector2D scale)
        {
            Matrix2x2 result;
            result.m00 = scale.X;
            result.m11 = scale.Y;
            result.m01 = 0;
            result.m10 = 0;
            return result;
        }
        public static void FromScale(ref Vector2D scale, out Matrix2x2 result)
        {
            result.m00 = scale.X;
            result.m11 = scale.Y;
            result.m01 = 0;
            result.m10 = 0;
        }

        [ParseMethod]
        public static Matrix2x2 Parse(string s)
        {
            Matrix2x2 rv = Zero;
            ParseHelper.ParseMatrix<Matrix2x2>(s, ref rv);
            return rv;
        }
#if !CompactFramework && !WindowsCE && !PocketPC && !XBOX360 
        public static bool TryParse(string s, out Matrix2x2 result)
        {
            result = Zero;
            return ParseHelper.TryParseMatrix<Matrix2x2>(s, ref result);
        }
#endif

        public static bool Equals(Matrix2x2 left, Matrix2x2 right)
        {
            return
                left.m00 == right.m00 && left.m01 == right.m01 &&
                left.m10 == right.m10 && left.m11 == right.m11;
        }
        [CLSCompliant(false)]
        public static bool Equals(ref Matrix2x2 left, ref Matrix2x2 right)
        {
            return
                left.m00 == right.m00 && left.m01 == right.m01 &&
                left.m10 == right.m10 && left.m11 == right.m11;
        }


        public static Matrix2x2 CreateNormal(Matrix2x3 source)
        {
            Matrix2x2 result;
            CreateNormal(ref source, out result);
            return result;
        }
        public static void CreateNormal(ref Matrix2x3 source, out Matrix2x2 result)
        {
            Scalar detInv = 1 / (source.m00 * source.m11 - source.m01 * source.m10);
            result.m10 = detInv * -source.m01;
            result.m11 = detInv * source.m00;
            result.m00 = detInv * source.m11;
            result.m01 = detInv * -source.m10;
        }
        public static Matrix2x2 CreateNormal(Matrix3x3 source)
        {
            Matrix2x2 result;
            CreateNormal(ref source, out result);
            return result;
        }
        public static void CreateNormal(ref Matrix3x3 source, out Matrix2x2 result)
        {
            Scalar detInv = 1 / (source.m00 * source.m11 - source.m01 * source.m10);
            result.m10 = detInv * -source.m01;
            result.m11 = detInv * source.m00;
            result.m00 = detInv * source.m11;
            result.m01 = detInv * -source.m10;
        }

        #endregion
        #region fields
        // | m00 m01 |
        // | m10 m11 |
        [XmlIgnore]
        public Scalar m00, m01;
        [XmlIgnore]
        public Scalar m10, m11;
        #endregion
        #region Constructors

        /// <summary>
        ///		Creates a new Matrix3 with all the specified parameters.
        /// </summary>
        public Matrix2x2(
            Scalar m00, Scalar m01,
            Scalar m10, Scalar m11)
        {
            this.m00 = m00; this.m01 = m01;
            this.m10 = m10; this.m11 = m11;
        }

        /// <summary>
        /// Create a new Matrix from 3 Vertex3 objects.
        /// </summary>
        /// <param name="xAxis"></param>
        /// <param name="yAxis"></param>
        [InstanceConstructor("Rx,Ry")]
        public Matrix2x2(Vector2D xAxis, Vector2D yAxis)
        {
            m00 = xAxis.X;
            m01 = xAxis.Y;
            m10 = yAxis.X;
            m11 = yAxis.Y;
        }
        public Matrix2x2(Scalar[] values) : this(values, 0) { }
        public Matrix2x2(Scalar[] values, int index)
        {
            Copy(values, index, out this);
        }
        #endregion
        #region Properties
        [XmlIgnore]
        public Vector2D Cx
        {
            get
            {
                return new Vector2D(m00, m10);
            }
            set
            {
                this.m00 = value.X;
                this.m10 = value.Y;
            }
        }
        [XmlIgnore]
        public Vector2D Cy
        {
            get
            {
                return new Vector2D(m01, m11);
            }
            set
            {
                this.m01 = value.X;
                this.m11 = value.Y;
            }
        }
        /// <summary>
        /// The X Row or row zero.
        /// </summary>
        [AdvBrowsable]
        [System.ComponentModel.Description("The First row of the Matrix2x2")]
        public Vector2D Rx
        {
            get
            {
                Vector2D value;
                value.X = m00;
                value.Y = m01;
                return value;
            }
            set
            {
                m00 = value.X;
                m01 = value.Y;
            }
        }
        /// <summary>
        /// The Y Row or row one.
        /// </summary>
        [AdvBrowsable]
        [System.ComponentModel.Description("The Second row of the Matrix2x2")]
        public Vector2D Ry
        {
            get
            {
                Vector2D value;
                value.X = m10;
                value.Y = m11;
                return value;
            }
            set
            {
                m10 = value.X;
                m11 = value.Y;
            }
        }
        public Scalar Determinant
        {
            get
            {
                Scalar result;
                GetDeterminant(ref this, out result);
                return result;
            }
        }
        /// <summary>
        /// Swap the rows of the matrix with the columns.
        /// </summary>
        /// <returns>A transposed Matrix.</returns>
        public Matrix2x2 Transposed
        {
            get
            {
                Matrix2x2 result;
                Transpose(ref this, out result);
                return result;
            }
        }
        public Matrix2x2 Adjoint
        {
            get
            {
                Matrix2x2 result;
                GetAdjoint(ref this, out result);
                return result;
            }
        }
        public Matrix2x2 Cofactor
        {
            get
            {
                Matrix2x2 result;
                GetCofactor(ref this, out result);
                return result;
            }
        }
        public Matrix2x2 Inverted
        {
            get
            {
                Matrix2x2 result;
                Invert(ref this, out result);
                return result;
            }
        }

        int IAdvanceValueType.Count { get { return Count; } }
        int IMatrix.RowCount { get { return RowCount; } }
        int IMatrix.ColumnCount { get { return ColumnCount; } }
        #endregion Properties
        #region Methods

        public Vector2D GetColumn(int columnIndex)
        {
            switch (columnIndex)
            {
                case 0:
                    return Cx;
                case 1:
                    return Cy;
            }
            throw ThrowHelper.GetThrowIndex("columnIndex", ColumnCount);
        }
        public void SetColumn(int columnIndex, Vector2D value)
        {

            switch (columnIndex)
            {
                case 0:
                    Cx = value;
                    return;
                case 1:
                    Cy = value;
                    return;
            }
            throw ThrowHelper.GetThrowIndex("columnIndex", ColumnCount);
        }
        public Vector2D GetRow(int rowIndex)
        {
            switch (rowIndex)
            {
                case 0:
                    return Rx;
                case 1:
                    return Ry;
            }
            throw ThrowHelper.GetThrowIndex("rowIndex", RowCount);
        }
        public void SetRow(int rowIndex, Vector2D value)
        {
            switch (rowIndex)
            {
                case 0:
                    Rx = value;
                    return;
                case 1:
                    Ry = value;
                    return;
            }
            throw ThrowHelper.GetThrowIndex("rowIndex", RowCount);
        }


        public Scalar[,] ToMatrixArray()
        {
            return new Scalar[RowCount, ColumnCount] { { m00, m01 }, { m10, m11 } };
        }
        public Scalar[] ToArray()
        {
            return new Scalar[Count] { m00, m01, m10, m11 };
        }
        public Scalar[] ToTransposedArray()
        {
            return new Scalar[Count] { m00, m10, m01, m11 };
        }


        public void CopyTo(Scalar[] array, int index)
        {
            Copy(ref this, array, index);
        }
        public void CopyTransposedTo(Scalar[] array, int index)
        {
            CopyTranspose(ref this, array, index);
        }
        public void CopyFrom(Scalar[] array, int index)
        {
            Copy(array, index, out this);
        }
        public void CopyTransposedFrom(Scalar[] array, int index)
        {
            CopyTranspose(array, index, out this);
        }

        private string ToStringInternal(string FormatString)
        {
            return string.Format(FormatString,
                m00, m01,
                m10, m11);
        }
        public string ToString(string format)
        {
            return ToStringInternal(string.Format(FormatableString, format));
        }
        public override string ToString()
        {
            return ToStringInternal(FormatString);
        }

        public override int GetHashCode()
        {
            return
                m00.GetHashCode() ^ m01.GetHashCode() ^
                m10.GetHashCode() ^ m11.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return
                (obj is Matrix2x2) &&
                Equals((Matrix2x2)obj);
        }
        public bool Equals(Matrix2x2 other)
        {
            return Equals(ref this, ref other);
        }
        #endregion
        #region Indexors
#if UNSAFE
        /// <summary>
        ///    Allows the Matrix to be accessed like a 2d array (i.e. matrix[2,3])
        /// </summary>
        /// <remarks>
        ///    This indexer is only provided as a convenience, and is <b>not</b> recommended for use in
        ///    intensive applications.  
        /// </remarks>
        public Scalar this[int rowIndex, int columnIndex]
        {
            get
            {
                ThrowHelper.CheckIndex("rowIndex", rowIndex, RowCount);
                ThrowHelper.CheckIndex("columnIndex", columnIndex, ColumnCount);
                unsafe
                {
                    fixed (Scalar* pM = &m00)
                    {
                        return pM[(ColumnCount * rowIndex) + columnIndex];
                    }
                }
            }
            set
            {
                ThrowHelper.CheckIndex("rowIndex", rowIndex, RowCount);
                ThrowHelper.CheckIndex("columnIndex", columnIndex, ColumnCount);
                unsafe
                {
                    fixed (Scalar* pM = &m00)
                    {
                        pM[(ColumnCount * rowIndex) + columnIndex] = value;
                    }
                }
            }
        }
        /// <summary>
        ///		Allows the Matrix to be accessed linearly (m[0] -> m[ColumnCount*RowCount-1]).  
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
                    fixed (Scalar* pMatrix = &this.m00)
                    {
                        return pMatrix[index];
                    }
                }
            }
            set
            {
              //  System.Runtime.InteropServices.
                ThrowHelper.CheckIndex("index", index, Count);
                unsafe
                {
                    fixed (Scalar* pMatrix = &this.m00)
                    {
                        pMatrix[index] = value;
                    }
                }
            }
        }
#endif
        #endregion
        #region Operator overloads
        /// <summary>
        /// Multiply (concatenate) two Matrix3 instances together.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix2x2 operator *(Matrix2x2 left, Matrix2x2 right)
        {

            Matrix2x2 result;

            result.m00 = left.m00 * right.m00 + left.m01 * right.m10;
            result.m01 = left.m00 * right.m01 + left.m01 * right.m11;

            result.m10 = left.m10 * right.m00 + left.m11 * right.m10;
            result.m11 = left.m10 * right.m01 + left.m11 * right.m11;

            return result;
        }
        /// <summary>
        ///		Used to add two matrices together.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix2x2 operator +(Matrix2x2 left, Matrix2x2 right)
        {
            Matrix2x2 result;

            result.m00 = left.m00 + right.m00;
            result.m01 = left.m01 + right.m01;

            result.m10 = left.m10 + right.m10;
            result.m11 = left.m11 + right.m11;

            return result;
        }
        /// <summary>
        ///		Used to subtract two matrices.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Matrix2x2 operator -(Matrix2x2 left, Matrix2x2 right)
        {
            Matrix2x2 result;

            result.m00 = left.m00 - right.m00;
            result.m01 = left.m01 - right.m01;

            result.m10 = left.m10 - right.m10;
            result.m11 = left.m11 - right.m11;

            return result;
        }
        /// <summary>
        /// Multiplies all the items in the Matrix3 by a scalar value.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Matrix2x2 operator *(Matrix2x2 matrix, Scalar scalar)
        {
            Matrix2x2 result;

            result.m00 = matrix.m00 * scalar;
            result.m01 = matrix.m01 * scalar;

            result.m10 = matrix.m10 * scalar;
            result.m11 = matrix.m11 * scalar;

            return result;
        }
        /// <summary>
        /// Multiplies all the items in the Matrix3 by a scalar value.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="scalar"></param>
        /// <returns></returns>
        public static Matrix2x2 operator *(Scalar scalar, Matrix2x2 matrix)
        {
            Matrix2x2 result;

            result.m00 = matrix.m00 * scalar;
            result.m01 = matrix.m01 * scalar;

            result.m10 = matrix.m10 * scalar;
            result.m11 = matrix.m11 * scalar;

            return result;
        }
        /// <summary>
        /// Negates all the items in the Matrix.
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Matrix2x2 operator -(Matrix2x2 matrix)
        {
            Matrix2x2 result;

            result.m00 = -matrix.m00;
            result.m01 = -matrix.m01;

            result.m10 = -matrix.m10;
            result.m11 = -matrix.m11;

            return result;
        }
        /// <summary>
        /// 	Test two matrices for (value) equality
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Matrix2x2 left, Matrix2x2 right)
        {
            return
                left.m00 == right.m00 && left.m01 == right.m01 &&
                left.m10 == right.m10 && left.m11 == right.m11;
        }

        public static bool operator !=(Matrix2x2 left, Matrix2x2 right)
        {
            return !(left == right);
        }

        public static explicit operator Matrix2x2(Matrix3x3 source)
        {
            Matrix2x2 result;

            result.m00 = source.m00;
            result.m01 = source.m01;

            result.m10 = source.m10;
            result.m11 = source.m11;

            return result;
        }

        #endregion
    }
}


