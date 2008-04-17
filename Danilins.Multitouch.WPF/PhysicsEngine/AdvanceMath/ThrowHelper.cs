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
    internal static class ThrowHelper
    {
        public static void CheckCopy(Scalar[] array, int index, int count)
        {
            if (array == null) { throw new ArgumentNullException("array", "The array cannot be null"); }
            if (index < 0) { throw new ArgumentOutOfRangeException("index", "the index must be greater or equal to zero"); }
            if (array.Length - index < count) { throw new ArgumentOutOfRangeException("array", String.Format("the array must have the length of {0} from the index", count)); }
        }
#if UNSAFE
        public static void CheckIndex(string name, int index, int count)
        {
            if (index < 0 || index >= count)
            {
                throw GetThrowIndex(name, count);
            }
        }
#endif
        public static Exception GetThrowIndex(string name, int count)
        {
            return new ArgumentOutOfRangeException(name, string.Format("the {0} must be greater or equal to zero and less then {1}", name, count));
        }
        public static void ThrowVectorFormatException(string value, int count, string format)
        {
            throw new FormatException(
                string.Format("Cannot parse the text '{0}' because it does not have {1} parts separated by commas in the form {2} with optional parenthesis.",
                value, count, string.Format(format, 'X', 'Y', 'Z', 'W')));
        }
    }
}