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


namespace Physics2DDotNet
{
    [StructLayout(LayoutKind.Explicit)]
    public struct PairID
    {
        public static long GetId(int id1, int id2)
        {
            PairID result;
            result.ID = 0;
            if (id1 > id2)
            {
                result.lowID = id2;
                result.highID = id1;
            }
            else
            {
                result.lowID = id1;
                result.highID = id2;
            }
            return result.ID;
        }
        public static long GetHash(int value1, int value2)
        {
            PairID result;
            result.ID = 0;
            result.lowID = value1;
            result.highID = value2;
            return result.ID;
        }
        public static void GetIds(long id,out int id1,out  int id2)
        {
            PairID result;
            result.lowID = 0;
            result.highID = 0;
            result.ID = id;
            id1 = result.lowID;
            id2 = result.highID;
        }
        [FieldOffset(0)]
        long ID;
        [FieldOffset(0)]
        int lowID;
        [FieldOffset(sizeof(int))]
        int highID;
    }
}