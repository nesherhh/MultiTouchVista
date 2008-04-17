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
    internal static class ParseHelper
    {
        public static string[] SplitStringVector(string s)
        {
            return SpritString(s, ',', new char[] { ' ', '(', '[', '<', ')', ']', '>' });
        }
        private static string[] SpritString(string s, char divider, char[] toremove)
        {
            StringBuilder builder = new StringBuilder(s);
            foreach (char r in toremove)
            {
                builder.Replace(r, ' ');
            }
#if !CompactFramework && !WindowsCE && !PocketPC && !XBOX360
            return builder.ToString().Split(new char[] { divider, ' ' }, StringSplitOptions.RemoveEmptyEntries);
#else
            string[] temp = s.Split(new char[] { divider, ' ' });
            int index2 = 0;
            for (int index1 = 0; index1 < temp.Length; ++index1)
            {
                if (temp[index1].Length > 0)
                {
                    temp[index2++] = temp[index1];
                }
            }
            string[] vals = new string[index2];
            Array.Copy(temp, vals, vals.Length);
            return vals;
#endif
        }
        private static string[] SpritString(string s, char divider, string[] toremove)
        {
            StringBuilder builder = new StringBuilder(s);
            foreach (string r in toremove)
            {
                builder.Replace(r, " ");
            }
#if !CompactFramework && !WindowsCE && !PocketPC && !XBOX360
            return builder.ToString().Split(new char[] { divider, ' ' }, StringSplitOptions.RemoveEmptyEntries);
#else
            string[] temp = s.Split(new char[] { divider, ' ' });
            int index2 = 0;
            for (int index1 = 0; index1 < temp.Length; ++index1)
            {
                if (temp[index1].Length > 0)
                {
                    temp[index2++] = temp[index1];
                }
            }
            string[] vals = new string[index2];
            Array.Copy(temp, vals, vals.Length);
            return vals;
#endif
        }


        public static void ParseMatrix<T>(string s, ref T valueType)
            where T : IAdvanceValueType
        {
            Parse(s, ref valueType, "|\n", "|", ' ');
        }
        public static void Parse<T>(string s, ref  T valueType, string leftParenth, string rightParenth, char divider)
            where T : IAdvanceValueType
        {
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }
            string[] parts = SpritString(s, divider, new string[] { leftParenth, rightParenth });
            if (parts.Length != valueType.Count)
            {
                throw new FormatException(
                string.Format("Cannot parse the text '{0}' because it does not have {1} parts.",
                s, valueType.Count));
            }
            Scalar[] result = new Scalar[valueType.Count];
            for (int index = 0; index < valueType.Count; ++index)
            {
                result[index] = Scalar.Parse(parts[index]);
            }
            valueType.CopyFrom(result, 0);
        }
#if !CompactFramework && !WindowsCE && !PocketPC && !XBOX360
        public static bool TryParseMatrix<T>(string s, ref T valueType)
            where T : IAdvanceValueType
        {
            return TryParse(s, ref valueType, "|", "|\n", ' ');
        }
        public static bool TryParse<T>(string s, ref T valueType, string leftParenth, string rightParenth, char divider)
            where T : IAdvanceValueType
        {
            if (s == null)
            {
                return false;
            }
            string[] parts = SpritString(s, divider, new string[] { leftParenth, rightParenth });
            if (parts.Length != valueType.Count)
            {
                return false;
            }
            Scalar[] result = new Scalar[valueType.Count];
            for (int index = 0; index < valueType.Count; ++index)
            {
                if (!Scalar.TryParse(parts[index], out result[index]))
                {
                    return false;
                }
            }
            valueType.CopyFrom(result, 0);
            return true;
        }
#endif

    }
}