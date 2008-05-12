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
using AdvanceMath.Design;
using System.Xml.Serialization;

namespace AdvanceMath
{
    /// <summary>
    /// A class that keeps a value clamped.
    /// </summary>
#if !CompactFramework && !WindowsCE && !PocketPC && !XBOX360 
    [System.ComponentModel.TypeConverter(typeof(AdvTypeConverter<Clamped>))]
#endif
    [AdvBrowsableOrder("Min,Value,Max"),Serializable]
    public sealed class Clamped : ICloneable, IComparable<Clamped>, IEquatable<Clamped>
    {
        [ParseMethod]
        public static Clamped Parse(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException("s");
            }
#if !CompactFramework && !WindowsCE && !PocketPC && !XBOX360

            string[] vals = s.Split(new char[] { '<', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
#else
            string[] temp = s.Split(new char[] { '<', '(', ')' });
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
#endif
            if (vals.Length != 3)
            {
                throw new FormatException();
            }
            return new Clamped(
                Scalar.Parse(vals[1]),
                Scalar.Parse(vals[0]),
                Scalar.Parse(vals[2]));
        }
#if !CompactFramework && !WindowsCE && !PocketPC && !XBOX360 
        public static bool TryParse(string s, out Clamped result)
        {
            if (s != null)
            {
                string[] vals = s.Split(new char[] { '<', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
                if (vals.Length == 3)
                {
                    Scalar min, value, max;
                    if (Scalar.TryParse(vals[0], out min) &&
                        Scalar.TryParse(vals[1], out value) &&
                        Scalar.TryParse(vals[2], out max))
                    {
                        result = new Clamped(value, min, max);
                        return true;
                    }
                }
            }
            result = null;
            return false;
        }
#endif

        Scalar value;
        Scalar min;
        Scalar max;
        /// <summary>
        /// Creates a new Clamped instance all values being zero.
        /// </summary>
        public Clamped() { }
        /// <summary>
        /// Creates a new Clamped instance with zero being either the max or min.
        /// </summary>
        /// <param name="value">The min or max and the current value.</param>
        public Clamped(Scalar value)
        {
            if (value < 0)
            {
                SetValues(value, value, 0);
            }
            else
            {
                SetValues(value, 0, value);
            }
        }
        /// <summary>
        /// Creates a new Clamped instance.
        /// </summary>
        /// <param name="value">The current value.</param>
        /// <param name="min">The minimum possible value.</param>
        /// <param name="max">The maximum possible value.</param>
        public Clamped(Scalar value, Scalar min, Scalar max)
        {
            SetValues(value, min, max);
        }
        public Clamped(Clamped copy)
        {
            this.value = copy.value;
            this.min = copy.min;
            this.max = copy.max;
        }
       /// <summary>
       /// Gets and Sets the current value.
       /// </summary>
        [AdvBrowsable]
        public Scalar Value
        {
            get { return this.value; }
            set
            {
                MathHelper.Clamp(ref value, ref this.min, ref this.max, out this.value);
            }
        }
        /// <summary>
        /// Gets and Sets the minimum value.
        /// </summary>
        [AdvBrowsable]
        public Scalar Min
        {
            get { return min; }
            set
            {
                if (value > max) { throw new ArgumentOutOfRangeException("value"); }
                min = value;
                MathHelper.Clamp(ref this.value, ref min, ref max, out this.value);
            }
        }
        /// <summary>
        /// Gets and Sets the maximum value.
        /// </summary>
        [AdvBrowsable]
        public Scalar Max
        {
            get { return max; }
            set
            {
                if (value < min) { throw new ArgumentOutOfRangeException("value"); }
                max = value;
                MathHelper.Clamp(ref this.value, ref min, ref max, out this.value);
            }
        }
        /// <summary>
        /// Gets and Sets the percent with Min being 0 (0%) and Max being 1 (100%)
        /// </summary>
        [XmlIgnore]
        public Scalar Percent
        {
            get
            {
                return value - min / max - min;
            }
            set
            {
                if (value >= 1)
                {
                    this.value = max;
                }
                else if (value <= 0)
                {
                    this.value = min;
                }
                else
                {
                    MathHelper.Lerp(ref min, ref max, ref value, out this.value);
                }
            }
        }
        /// <summary>
        /// Gets if the value is at its maximum value;
        /// </summary>
        public bool IsMax
        {
            get { return this.value == max; }
        }
        /// <summary>
        /// Gets if the value is at its minimum value;
        /// </summary>
        public bool IsMin
        {
            get { return this.value == min; }
        }
        /// <summary>
        /// Sets it to its maximum value;
        /// </summary>
        public void Maximize()
        {
            value = max;
        }
        /// <summary>
        /// Sets it to its minimum value;
        /// </summary>
        public void Minimize()
        {
            value = min;
        }
        /// <summary>
        /// Adds a value to the clamped vaule and returns the overflow/underflow.
        /// </summary>
        /// <param name="value">The Value to add.</param>
        /// <returns>The overflow/underflow.</returns>
        public Scalar Add(Scalar value)
        {
            Scalar newValue = value + this.value;
            if (newValue > max)
            {
                this.value = max;
                return newValue - max;
            }
            else if (newValue < min)
            {
                this.value = min;
                return newValue - min;
            }
            else
            {
                this.value = newValue;
                return 0;
            }
        }
        /// <summary>
        /// Adds a value to the clamped vaule and returns the overflow/underflow.
        /// </summary>
        /// <param name="value">The Value to add.</param>
        /// <param name="result">The overflow/underflow.</param>
        public void Add(ref Scalar value, out Scalar result)
        {
            Scalar newValue = value + this.value;
            if (newValue > max)
            {
                this.value = max;
                result = newValue - max;
            }
            else if (newValue < min)
            {
                this.value = min;
                result = newValue - min;
            }
            else
            {
                this.value = newValue;
                result = 0;
            }
        }
        /// <summary>
        /// Sets all the values at once.
        /// </summary>
        /// <param name="value">The current value.</param>
        /// <param name="min">The minimum possible value.</param>
        /// <param name="max">The maximum possible value.</param>
        public void SetValues(Scalar value, Scalar min, Scalar max)
        {
            if (min > max) { throw new ArgumentOutOfRangeException("min"); }
            this.min = min;
            this.max = max;
            MathHelper.Clamp(ref value, ref min, ref max, out this.value);
        }

        public override string ToString()
        {
            return string.Format("({0} < {1} < {2})", min, value, max);
        }
        public override int GetHashCode()
        {
            return this.value.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            Clamped other = obj as Clamped;
            return other != null && Equals(other);
        }
        public bool Equals(Clamped other)
        {
            return this.value.Equals(other.value);
        }
        public int CompareTo(Clamped other)
        {
            return this.value.CompareTo(other.value);
        }
        public object Clone()
        {
            return new Clamped(this);
        }
    }
}