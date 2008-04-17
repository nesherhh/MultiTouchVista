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
using System.IO;
using System.Text;

namespace AdvanceMath.IO
{
    public class MathWriter : BinaryWriter
    {
        protected static Stream GetBaseStream(BinaryWriter writer)
        {
            if (writer == null) { throw new ArgumentNullException("writer"); }
            return writer.BaseStream;
        }
        public MathWriter(Stream output)
            : base(output) { }
        public MathWriter(Stream output, Encoding encoding)
            : base(output, encoding) { }
        public MathWriter(BinaryWriter writer)
            : base(GetBaseStream(writer)) { }
        public void Write(Vector2D value)
        {
            Write(value.X);
            Write(value.Y);
        }
        public void Write(Vector3D value)
        {
            Write(value.X);
            Write(value.Y);
            Write(value.Z);
        }
        public void Write(Vector4D value)
        {
            Write(value.X);
            Write(value.Y);
            Write(value.Z);
            Write(value.W);
        }
        public void Write(Quaternion value)
        {
            Write(value.X);
            Write(value.Y);
            Write(value.Z);
            Write(value.W);
        }
        public void Write(Matrix2x2 value)
        {
            Write(value.m00);
            Write(value.m01);

            Write(value.m10);
            Write(value.m11);
        }
        public void Write(Matrix3x3 value)
        {
            Write(value.m00);
            Write(value.m01);
            Write(value.m02);

            Write(value.m10);
            Write(value.m11);
            Write(value.m12);

            Write(value.m20);
            Write(value.m21);
            Write(value.m22);
        }
        public void Write(Matrix4x4 value)
        {
            Write(value.m00);
            Write(value.m01);
            Write(value.m02);
            Write(value.m03);

            Write(value.m10);
            Write(value.m11);
            Write(value.m12);
            Write(value.m13);

            Write(value.m20);
            Write(value.m21);
            Write(value.m22);
            Write(value.m23);

            Write(value.m30);
            Write(value.m31);
            Write(value.m32);
            Write(value.m33);
        }
        public void Write(Scalar[] array)
        {
            if (array == null) { throw new ArgumentNullException("array"); }
            Write(array.Length);
            for (int index = 0; index < array.Length; index++)
            {
                Write(array[index]);
            }
        }
    }
}
