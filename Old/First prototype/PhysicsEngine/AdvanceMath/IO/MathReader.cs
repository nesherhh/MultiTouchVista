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
    public class MathReader : BinaryReader
    {
        protected static Stream GetBaseStream(BinaryReader reader)
        {
            if (reader == null) { throw new ArgumentNullException("reader"); }
            return reader.BaseStream;
        }
        public MathReader(Stream input)
            : base(input) { }
        public MathReader(Stream input, Encoding encoding)
            : base(input, encoding) { }
        public MathReader(BinaryReader reader)
            : base(GetBaseStream(reader)) { }
        private Scalar ReadScalar()
        {
#if UseDouble
            return base.ReadDouble();
#else
            return base.ReadSingle();
#endif
        }

        public Vector2D ReadVector2D()
        {
            Vector2D returnvalue;
            returnvalue.X = ReadScalar();
            returnvalue.Y = ReadScalar();
            return returnvalue;
        }
        public Vector3D ReadVector3D()
        {
            Vector3D returnvalue;
            returnvalue.X = ReadScalar();
            returnvalue.Y = ReadScalar();
            returnvalue.Z = ReadScalar();
            return returnvalue;
        }
        public Vector4D ReadVector4D()
        {
            Vector4D returnvalue;
            returnvalue.X = ReadScalar();
            returnvalue.Y = ReadScalar();
            returnvalue.Z = ReadScalar();
            returnvalue.W = ReadScalar();
            return returnvalue;
        }
        public Quaternion ReadQuaternion()
        {
            Quaternion returnvalue;
            returnvalue.X = ReadScalar();
            returnvalue.Y = ReadScalar();
            returnvalue.Z = ReadScalar();
            returnvalue.W = ReadScalar();
            return returnvalue;
        }
        public Matrix2x2 ReadMatrix2x2()
        {
            Matrix2x2 returnvalue;

            returnvalue.m00 = ReadScalar();
            returnvalue.m01 = ReadScalar();

            returnvalue.m10 = ReadScalar();
            returnvalue.m11 = ReadScalar();

            return returnvalue;
        }
        public Matrix3x3 ReadMatrix3x3()
        {
            Matrix3x3 returnvalue;

            returnvalue.m00 = ReadScalar();
            returnvalue.m01 = ReadScalar();
            returnvalue.m02 = ReadScalar();

            returnvalue.m10 = ReadScalar();
            returnvalue.m11 = ReadScalar();
            returnvalue.m12 = ReadScalar();

            returnvalue.m20 = ReadScalar();
            returnvalue.m21 = ReadScalar();
            returnvalue.m22 = ReadScalar();

            return returnvalue;
        }
        public Matrix4x4 ReadMatrix4x4()
        {
            Matrix4x4 returnvalue;

            returnvalue.m00 = ReadScalar();
            returnvalue.m01 = ReadScalar();
            returnvalue.m02 = ReadScalar();
            returnvalue.m03 = ReadScalar();

            returnvalue.m10 = ReadScalar();
            returnvalue.m11 = ReadScalar();
            returnvalue.m12 = ReadScalar();
            returnvalue.m13 = ReadScalar();

            returnvalue.m20 = ReadScalar();
            returnvalue.m21 = ReadScalar();
            returnvalue.m22 = ReadScalar();
            returnvalue.m23 = ReadScalar();

            returnvalue.m30 = ReadScalar();
            returnvalue.m31 = ReadScalar();
            returnvalue.m32 = ReadScalar();
            returnvalue.m33 = ReadScalar();

            return returnvalue;
        }

        public Scalar[] ReadScalarArray()
        {
            Scalar[] returnvalue = new Scalar[ReadInt32()];
            for (int index = 0; index < returnvalue.Length; index++)
            {
                returnvalue[index] = ReadScalar();
            }
            return returnvalue;
        }
    }
}
