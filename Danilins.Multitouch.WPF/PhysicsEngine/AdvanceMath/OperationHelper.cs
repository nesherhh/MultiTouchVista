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
    public delegate void RefOperation<TLeft, TRight, TResult>(ref TLeft left, ref TRight right, out TResult result);
    public delegate TResult ValOperation<TLeft, TRight, TResult>(TLeft left, TRight right);


    public static class OperationHelper
    {
        public static TResult[] ArrayRefOp<TLeft, TRight, TResult>(TLeft[] leftArray, ref TRight right, RefOperation<TLeft, TRight, TResult> operation)
        {
            TResult[] result = new TResult[leftArray.Length];
            ArrayRefOp<TLeft, TRight, TResult>(leftArray, ref right, result, operation);
            return result;
        }
        public static TResult[] ArrayRefOp<TLeft, TRight, TResult>(ref TLeft left, TRight[] rightArray, RefOperation<TLeft, TRight, TResult> operation)
        {
            TResult[] result = new TResult[rightArray.Length];
            ArrayRefOp<TLeft, TRight, TResult>(ref left, rightArray, result, operation);
            return result;
        }
        public static TResult[] ArrayRefOp<TLeft, TRight, TResult>(TLeft[] leftArray, TRight[] rightArray, RefOperation<TLeft, TRight, TResult> operation)
        {
            TResult[] result = new TResult[leftArray.Length];
            ArrayRefOp<TLeft, TRight, TResult>(leftArray, rightArray, result, operation);
            return result;
        }

        public static void ArrayRefOp<TLeft, TRight, TResult>(TLeft[] leftArray, ref TRight right, TResult[] result, RefOperation<TLeft, TRight, TResult> operation)
        {
            if (leftArray == null) { throw new ArgumentNullException("leftArray"); }
            if (result == null) { throw new ArgumentNullException("result"); }
            if (leftArray.Length != result.Length) { throw new ArgumentException("The Arrays Passed must equal in size."); }
            for (int pos = 0; pos < result.Length; ++pos)
            {
                operation(ref leftArray[pos], ref right, out result[pos]);
            }
        }
        public static void ArrayRefOp<TLeft, TRight, TResult>(ref TLeft left, TRight[] rightArray, TResult[] result, RefOperation<TLeft, TRight, TResult> operation)
        {
            if (rightArray == null) { throw new ArgumentNullException("rightArray"); }
            if (result == null) { throw new ArgumentNullException("result"); }
            if (rightArray.Length != result.Length) { throw new ArgumentException("The Arrays Passed must equal in size."); }
            for (int pos = 0; pos < result.Length; ++pos)
            {
                operation(ref left, ref rightArray[pos],out result[pos]);
            }
        }
        public static void ArrayRefOp<TLeft, TRight, TResult>(TLeft[] leftArray, TRight[] rightArray, TResult[] result, RefOperation<TLeft, TRight, TResult> operation)
        {
            if (leftArray == null) { throw new ArgumentNullException("leftArray"); }
            if (rightArray == null) { throw new ArgumentNullException("rightArray"); }
            if (result == null) { throw new ArgumentNullException("result"); }
            if (leftArray.Length != rightArray.Length || rightArray.Length != result.Length) { throw new ArgumentException("The Arrays Passed must equal in size."); }
            for (int pos = 0; pos < result.Length; ++pos)
            {
                operation(ref leftArray[pos], ref rightArray[pos], out result[pos]);
            }
        }

        public static TResult[] ArrayValOp<TLeft, TRight, TResult>(TLeft[] leftArray, TRight right, ValOperation<TLeft, TRight, TResult> operation)
        {
            TResult[] result = new TResult[leftArray.Length];
            ArrayValOp<TLeft, TRight, TResult>(leftArray, right, result, operation);
            return result;
        }
        public static TResult[] ArrayValOp<TLeft, TRight, TResult>(TLeft left, TRight[] rightArray, ValOperation<TLeft, TRight, TResult> operation)
        {
            TResult[] result = new TResult[rightArray.Length];
            ArrayValOp<TLeft, TRight, TResult>(left, rightArray, result, operation);
            return result;
        }
        public static TResult[] ArrayValOp<TLeft, TRight, TResult>(TLeft[] leftArray, TRight[] rightArray, ValOperation<TLeft, TRight, TResult> operation)
        {
            TResult[] result = new TResult[leftArray.Length];
            ArrayValOp<TLeft, TRight, TResult>(leftArray, rightArray, result, operation);
            return result;
        }

        public static void ArrayValOp<TLeft, TRight, TResult>(TLeft[] leftArray, TRight right, TResult[] result, ValOperation<TLeft, TRight, TResult> operation)
        {
            if (leftArray == null) { throw new ArgumentNullException("leftArray"); }
            if (result == null) { throw new ArgumentNullException("result"); }
            if (leftArray.Length != result.Length) { throw new ArgumentException("The Arrays Passed must equal in size."); }
            for (int pos = 0; pos < result.Length; ++pos)
            {
                result[pos] = operation(leftArray[pos], right);
            }
        }
        public static void ArrayValOp<TLeft, TRight, TResult>(TLeft left, TRight[] rightArray, TResult[] result, ValOperation<TLeft, TRight, TResult> operation)
        {
            if (rightArray == null) { throw new ArgumentNullException("rightArray"); }
            if (result == null) { throw new ArgumentNullException("result"); }
            if (rightArray.Length != result.Length) { throw new ArgumentException("The Arrays Passed must equal in size."); }
            for (int pos = 0; pos < result.Length; ++pos)
            {
                result[pos] = operation(left, rightArray[pos]);
            }
        }
        public static void ArrayValOp<TLeft, TRight, TResult>(TLeft[] leftArray, TRight[] rightArray, TResult[] result, ValOperation<TLeft, TRight, TResult> operation)
        {
            if (leftArray == null) { throw new ArgumentNullException("leftArray"); }
            if (rightArray == null) { throw new ArgumentNullException("rightArray"); }
            if (result == null) { throw new ArgumentNullException("result"); }
            if (leftArray.Length != rightArray.Length || rightArray.Length != result.Length) { throw new ArgumentException("The Arrays Passed must equal in size."); }
            for (int pos = 0; pos < result.Length; ++pos)
            {
                result[pos] = operation(leftArray[pos], rightArray[pos]);
            }
        }
    }
}