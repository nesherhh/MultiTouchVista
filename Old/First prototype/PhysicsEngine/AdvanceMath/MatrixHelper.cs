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

using System.Text;
namespace AdvanceMath
{
    internal static class MatrixHelper
    {
        public static string CreateMatrixFormatString(int RowCount, int ColumnCount)
        {
            return CreateFormatString(RowCount, ColumnCount, "|", "|\n", "", 1);
        }
        public static string CreateMatrixFormatableString(int RowCount, int ColumnCount)
        {
            return CreateFormatableString(RowCount, ColumnCount, "|", "|\n", "", 1);
        }
        public static string CreateVectorFormatString(int Count)
        {
            return CreateFormatString(1, Count, "(", ")", ",", 1);
        }
        public static string CreateVectorFormatableString(int Count)
        {
            return CreateFormatableString(1, Count, "(", ")", ",", 1);
        }

        public static string CreateFormatableString(
            int rowCount, int columnCount,
            string leftParenth, string rightParenth,
            string divider, int whitespacecount)
        {
            return GenerateFormatStringInternal(rowCount, columnCount, leftParenth, rightParenth, divider,whitespacecount, "{{", ":{0}}}");
        }
        public static string CreateFormatString(
            int rowCount, int columnCount,
            string leftParenth, string rightParenth,
            string divider, int whitespacecount)
        {
            return GenerateFormatStringInternal(rowCount, columnCount, leftParenth, rightParenth,divider, whitespacecount, "{", "}");
        }
        private static string GenerateFormatStringInternal(
            int rowCount, int columnCount,
            string leftParenth, string rightParenth,
            string divider, int whitespacecount,
            string openBracket, string closeBracket)
        {
            StringBuilder builder = new StringBuilder(
                rowCount * (leftParenth.Length + rightParenth.Length) +
                rowCount * columnCount * (1 + divider.Length + openBracket.Length + closeBracket.Length + whitespacecount));

            int index = 0;
            for (int row = 0; row < rowCount; ++row)
            {
                builder.Append(leftParenth);
                builder.Append(' ', whitespacecount);
                for (int col = 0; col < columnCount; ++col)
                {
                    builder.Append(openBracket);
                    builder.Append(index);
                    builder.Append(closeBracket);
                    if (col != columnCount - 1)
                    {
                        builder.Append(divider);
                    }
                    builder.Append(' ', whitespacecount);
                    index++;
                }
                builder.Append(rightParenth);
            }
            return builder.ToString();
        }





    }
}