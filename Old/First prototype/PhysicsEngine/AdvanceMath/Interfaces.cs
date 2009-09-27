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

namespace AdvanceMath
{
    public interface IAdvanceValueType
    {
        /// <summary>
        /// Gets a 32-bit integer that represents the total number of elements in all the dimensions of IAdvanceValueType. 
        /// </summary>
        int Count { get;}
#if UNSAFE
        /// <summary>
        /// Gets or sets the <see cref="Scalar"/>  at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the <see cref="Scalar"/> to get or set.</param>
        /// <returns>The <see cref="Scalar"/> at the specified index.</returns>
        Scalar this[int index] { get;set;}
#endif
        /// <summary>
        /// Copies the elements of the IAdvanceValueType to a new array of <see cref="Scalar"/> . 
        /// </summary>
        /// <returns>An array containing copies of the elements of the IAdvanceValueType.</returns>
        Scalar[] ToArray();
        /// <summary>
        /// Copies all the elements of the IAdvanceValueType to the specified one-dimensional Array of <see cref="Scalar"/>. 
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied from the IAdvanceValueType.</param>
        /// <param name="index">A 32-bit integer that represents the index in array at which copying begins.</param>
        void CopyTo(Scalar[] array, int index);
        /// <summary>
        /// Copies all the elements, up to the <see cref="Length"/> of the IAdvanceValueType, of the specified one-dimensional Array to the IAdvanceValueType. 
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the source of the elements copied to the IAdvanceValueType.</param>
        /// <param name="index">A 32-bit integer that represents the index in array at which copying begins.</param>
        void CopyFrom(Scalar[] array, int index);
        /// <summary>
        /// turns the object into a string representation of itself with a special format for each Scaler in it.
        /// </summary>
        /// <param name="format">the format to be applied to each Scalar </param>
        /// <returns>a string with every Scalar formated with the provided format.  </returns>
        string ToString(string format);
    }
    public interface IVector<V> : IAdvanceValueType, IEquatable<V>
        where V : struct, IVector<V>
    {
        /// <summary>
        /// Gets or Sets the Magnitude (Length of a Vector).
        /// </summary>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Length_of_a_vector"/></remarks>
        Scalar Magnitude { get;set;}
        /// <summary>
        /// Gets the Squared Magnitude (IE Magnitude*Magnitude).
        /// </summary>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Length_of_a_vector"/></remarks>
        Scalar MagnitudeSq { get;}
        /// <summary>
        /// Gets the Normalized Vector. (Unit Vector)
        /// </summary>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Vector_%28spatial%29#Unit_vector"/></remarks>
        V Normalized { get;}
    }
    public interface IMatrix : IAdvanceValueType
    {
        /// <summary>
        /// Gets a 32-bit integer that represents the total number of Rows in the IMatrix. 
        /// </summary>
        int RowCount { get;}
        /// <summary>
        /// Gets a 32-bit integer that represents the total number of Columns in the IMatrix. 
        /// </summary>
        int ColumnCount { get;}
#if UNSAFE
        /// <summary>
        /// Gets or sets the <see cref="Scalar"/>  at the specified row and column.
        /// </summary>
        /// <param name="row">The zero-based index of the Row to get or set.</param>
        /// <param name="column">The zero-based index of the Column to get or set.</param>
        /// <returns>The <see cref="Scalar"/> at the specified index.</returns>
        Scalar this[int row, int column] { get;set;}
#endif
        /// <summary>
        /// Copies the elements of the IMatrix to a new 2-dimensional array of <see cref="Scalar"/>s. 
        /// </summary>
        /// <returns>A 2-dimensional array containing copies of the elements of the IMatrix.</returns>
        Scalar[,] ToMatrixArray();
        /// <returns></returns>
        /// <summary>
        /// Copies the elements, in a Transposed order, of the IMatrix to a new array of <see cref="Scalar"/>. 
        /// </summary>
        /// <returns>An array containing copies of the elements, in a Transposed order, of the IAdvanceValueType.</returns>
        /// <remarks>
        /// This is the Format Accepted by OpenGL.
        /// </remarks>
        Scalar[] ToTransposedArray();
        /// <summary>
        /// Copies all the elements, in a Transposed order, of the IAdvanceValueType to the specified one-dimensional Array of <see cref="Scalar"/>. 
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied from the IAdvanceValueType.</param>
        /// <param name="index">A 32-bit integer that represents the index in array at which copying begins.</param>
        void CopyTransposedTo(Scalar[] array, int index);
        /// <summary>
        /// Copies all the elements, in a Transposed order, up to the <see cref="IAdvanceValueType.Length"/> of the IAdvanceValueType, of the specified one-dimensional Array to the IAdvanceValueType. 
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the source of the elements copied to the IAdvanceValueType.</param>
        /// <param name="index">A 32-bit integer that represents the index in array at which copying begins.</param>
        void CopyTransposedFrom(Scalar[] array, int index);
        /// <summary>
        /// Gets the Determinant of the IMatrix
        /// </summary>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Determinant"/></remarks>
        Scalar Determinant { get;}
    }

    public interface IMatrix<M, VC, VR> : IMatrix, IAdvanceValueType, IEquatable<M>
        where M : struct, IMatrix<M, VC, VR>
        where VC : struct, IVector<VC>
        where VR : struct, IVector<VR>
    {
        /// <summary>
        /// Gets the <typeparamref name="V"/> at the specified Column.
        /// </summary>
        /// <param name="column">The zero-based index of the Column of the <typeparamref name="V"/> to get.</param>
        /// <returns>The <typeparamref name="V"/> at the specified Column.</returns>
        VC GetColumn(int columnIndex);
        /// <summary>
        /// Sets the <typeparamref name="V"/>  at the specified Column.
        /// </summary>
        /// <param name="column">The zero-based index of the Column of the <typeparamref name="V"/> to set.</param>
        /// <param name="value">The <typeparamref name="V"/> to set at the specified Column.</param>
        void SetColumn(int columnIndex, VC value);
        /// <summary>
        /// Gets the <typeparamref name="V"/> at the specified Row.
        /// </summary>
        /// <param name="row">The zero-based index of the Row of the <typeparamref name="V"/> to get.</param>
        /// <returns>The <typeparamref name="V"/> at the specified Row.</returns>
        VR GetRow(int rowIndex);
        /// <summary>
        /// Sets the <typeparamref name="V"/> at the specified Row.
        /// </summary>
        /// <param name="row">The zero-based index of the Row of the <typeparamref name="V"/> to set.</param>
        /// <param name="value">The <typeparamref name="V"/> to set at the specified Row.</param>
        void SetRow(int rowIndex, VR value);
        /// <summary>
        /// Gets the Inverse of the IMatrix
        /// </summary>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Invertible_matrix"/></remarks>
        M Inverted { get;}
        /// <summary>
        /// Gets the Transpose of the IMatrix
        /// </summary>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Transpose"/></remarks>
        M Transposed { get;}
        /// <summary>
        /// Gets the Adjoint (Conjugate Transpose) of the IMatrix
        /// </summary>
        /// <remarks><seealso href="http://en.wikipedia.org/wiki/Conjugate_transpose"/></remarks>
        M Adjoint { get;}
        /// <summary>
        /// Gets the Cofactor (The Transpose of the Adjoint) of the IMatrix
        /// </summary>
        M Cofactor { get;}
    }



}