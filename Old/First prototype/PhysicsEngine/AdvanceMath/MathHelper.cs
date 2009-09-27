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
    public static class MathHelper
    {
        #region consts
        public const Scalar E = (Scalar)System.Math.E;
        public const Scalar Pi = (Scalar)System.Math.PI;
        public const Scalar TwoPi = (Scalar)(System.Math.PI * 2);
        public const Scalar PiOver2 = (Scalar)(System.Math.PI / 2);
        public const Scalar PiOver4 = (Scalar)(System.Math.PI / 4);
        public const Scalar ThreePiOver2 = (Scalar)((3 * System.Math.PI) / 2);
        public const Scalar RadiansPerDegree = (Scalar)(System.Math.PI / 180);
        public const Scalar DegreesPerRadian = (Scalar)(180 / System.Math.PI);
        public const Scalar Tolerance = 0.000000001f;

        public const Scalar Epsilon = 1e-03f;

        internal static Scalar Two = 2;
        #endregion
        #region methods

        public static Scalar Lerp(Scalar left, Scalar right, Scalar amount)
        {
            return (right - left) * amount + left;
        }
        public static void Lerp(ref Scalar left, ref  Scalar right, ref  Scalar amount, out Scalar result)
        {
            result = (right - left) * amount + left;
        }

        public static Scalar CatmullRom(Scalar value1, Scalar value2, Scalar value3, Scalar value4, Scalar amount)
        {
            Scalar amountSq = amount * amount;
            Scalar amountCu = amountSq * amount;
            return
                0.5f * ((2 * value2) +
                (-value1 + value3) * amount +
                (2 * value1 - 5 * value2 + 4 * value3 - value4) * amountSq +
                (-value1 + 3 * value2 - 3 * value3 + value4) * amountCu);
        }
        public static void CatmullRom(ref Scalar value1, ref Scalar value2, ref Scalar value3, ref Scalar value4, Scalar amount, out Scalar result)
        {
            Scalar amountSq = amount * amount;
            Scalar amountCu = amountSq * amount;
            result =
                0.5f * ((2 * value2) +
                (-value1 + value3) * amount +
                (2 * value1 - 5 * value2 + 4 * value3 - value4) * amountSq +
                (-value1 + 3 * value2 - 3 * value3 + value4) * amountCu);
        }

        internal static void HermiteHelper(Scalar amount, out Scalar h1, out Scalar h2, out Scalar h3, out Scalar h4)
        {
            Scalar wf2 = amount * amount;
            Scalar wf3 = wf2 * amount;
            Scalar wf3t2 = 2 * wf3;
            Scalar wf2t3 = 3 * wf2;
            h1 = wf3t2 - wf2t3 + 1;
            h2 = wf2t3 - wf3t2;
            h3 = wf3 - 2 * wf2 + amount;
            h4 = wf3 - wf2;
        }
        public static Scalar Hermite(Scalar value1, Scalar tangent1, Scalar value2, Scalar tangent2, Scalar amount)
        {
            Scalar result;
            Hermite(ref value1, ref tangent1, ref value2, ref tangent2, amount, out result);
            return result;
        }
        public static void Hermite(ref  Scalar value1, ref Scalar tangent1, ref Scalar value2, ref Scalar tangent2, Scalar amount, out Scalar result)
        {
            Scalar h1, h2, h3, h4;
            MathHelper.HermiteHelper(amount, out h1, out h2, out h3, out h4);
            result = h1 * value1 + h2 * value2 + h3 * tangent1 + h4 * tangent2;
        }


        public static Scalar Clamp(Scalar value, Scalar min, Scalar max)
        {
            return (value < min) ? (min) : ((value > max) ? (max) : (value));
        }
        public static void Clamp(ref Scalar value, ref Scalar min, ref Scalar max, out Scalar result)
        {
            result = (value < min) ? (min) : ((value > max) ? (max) : (value));
        }


        public static void Sort(Scalar value1, Scalar value2, out Scalar max, out Scalar min)
        {
            if (value1 > value2)
            {
                max = value1;
                min = value2;
            }
            else
            {
                max = value2;
                min = value1;
            }
        }


        /// <summary>
        /// Clamps a value between 2 values, but wraps the value around. So that one plus max would result in one plus min.
        /// </summary>
        /// <param name="value">the value to clamp</param>
        /// <param name="min">the minimum value</param>
        /// <param name="max">the maximum value</param>
        /// <returns>the clamped result</returns>
        public static Scalar WrapClamp(Scalar value, Scalar min, Scalar max)
        {
            if (min <= value && value < max) { return value; }
            Scalar rem = (value - min) % (max - min);
            return rem + ((rem < 0) ? (max) : (min));
        }
        /// <summary>
        /// Clamps a value between 2 values, but wraps the value around. So that one plus max would result in one plus min.
        /// </summary>
        /// <param name="value">the value to clamp</param>
        /// <param name="min">the minimum value</param>
        /// <param name="max">the maximum value</param>
        /// <param name="result">the clamped result</param>
        public static void WrapClamp(ref Scalar value, ref Scalar min, ref Scalar max, out Scalar result)
        {
            if (min <= value && value < max) { result = value; return; }
            Scalar rem = (value - min) % (max - min);
            result = rem + ((rem < 0) ? (max) : (min));
        }


        public static Scalar ClampAngle(Scalar angle)
        {
            if (-Pi <= angle && angle < Pi) { return angle; }
            Scalar rem = (angle + Pi) % (TwoPi);
            return rem + ((rem < 0) ? (Pi) : (-Pi));
        }
        [CLSCompliant(false)]
        public static void ClampAngle(ref Scalar angle)
        {
            if (-Pi <= angle && angle < Pi) { return; }
            Scalar rem = (angle + Pi) % (TwoPi);
            angle = rem + ((rem < 0) ? (Pi) : (-Pi));
        }
        public static void ClampAngle(ref Scalar angle, out Scalar result)
        {
            if (-Pi <= angle && angle < Pi) { result = angle; return; }
            Scalar rem = (angle + Pi) % (TwoPi);
            result = rem + ((rem < 0) ? (Pi) : (-Pi));
        }

        public static Scalar AngleSubtract(Scalar angle1, Scalar angle2)
        {
            return ClampAngle(angle1 - angle2);
        }
        public static void AngleSubtract(ref Scalar angle1, ref  Scalar angle2, out Scalar result)
        {
            result = angle1 - angle2;
            ClampAngle(ref result);
        }

        /// <summary>
        /// Trys to Solve for x in the equation: (a * (x * x) + b * x + c == 0)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="plus">The result of (b + Math.Sqrt((b * b) - (4 * a * c))) / (2 * a)</param>
        /// <param name="minus">The result of (b - Math.Sqrt((b * b) - (4 * a * c))) / (2 * a)</param>
        /// <returns><see langword="false" /> if an error would have been thrown; otherwise <see langword="true" />.</returns>
        public static bool TrySolveQuadratic(Scalar a, Scalar b, Scalar c, out Scalar plus, out Scalar minus)
        {
            if (0 == a)
            {
                plus = -c / b;
                minus = plus;
                return true;
            }
            c = (b * b) - (4 * a * c);
            if (0 <= c)
            {
                c = Sqrt(c);
                a = .5f / a;
                plus = ((c - b) * a);
                minus = ((-c - b) * a);
                return true;
            }
            plus = 0;
            minus = 0;
            return false;
        }
        public static Scalar InvSqrt(Scalar number)
        {
            return 1 / Sqrt(number);
        }

        public static Scalar Max(params Scalar[] vals)
        {
            if (vals == null) { throw new ArgumentNullException("vals"); }
            if (vals.Length == 0) { throw new ArgumentException("There must be at least one value to compare", "vals"); }
            Scalar max = vals[0];
            if (Scalar.IsNaN(max)) { return max; }
            for (int i = 1; i < vals.Length; i++)
            {
                Scalar val = vals[i];
                if (val > max) { max = val; }
                else if (Scalar.IsNaN(val)) { return val; }
            }
            return max;
        }
        public static Scalar Min(params Scalar[] vals)
        {
            if (vals == null) { throw new ArgumentNullException("vals"); }
            if (vals.Length == 0) { throw new ArgumentException("There must be at least one value to compare", "vals"); }
            Scalar min = vals[0];
            if (Scalar.IsNaN(min)) { return min; }
            for (int i = 1; i < vals.Length; i++)
            {
                Scalar val = vals[i];
                if (val < min) { min = val; }
                else if (Scalar.IsNaN(val)) { return val; }
            }
            return min;
        }

        public static bool PointInTri2D(Vector2D point, Vector2D a, Vector2D b, Vector2D c)
        {
            Vector2D vect1, vect2;
            Scalar temp;
            Vector2D.Subtract(ref b, ref a, out vect1);
            Vector2D.Subtract(ref point, ref b, out vect2);
            Vector2D.ZCross(ref vect1, ref vect2, out temp);
            bool bClockwise = temp >= 0;
            Vector2D.Subtract(ref c, ref b, out vect1);
            Vector2D.Subtract(ref point, ref c, out vect2);
            Vector2D.ZCross(ref vect1, ref vect2, out temp);
            if (temp < 0 ^ bClockwise) { return true; }
            Vector2D.Subtract(ref a, ref c, out vect1);
            Vector2D.Subtract(ref point, ref a, out vect2);
            Vector2D.ZCross(ref vect1, ref vect2, out temp);
            return temp < 0 ^ bClockwise;

            /* bool bClockwise = (((b - a) ^ (point - b)) >= 0);
             return !(((((c - b) ^ (point - c)) >= 0) ^ bClockwise) && ((((a - c) ^ (point - a)) >= 0) ^ bClockwise));*/
        }
        /// <summary>
        ///		Converts degrees to radians.
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static Scalar ToRadians(Scalar degrees)
        {
            return degrees * RadiansPerDegree;
        }
        /// <summary>
        ///		Converts radians to degrees.
        /// </summary>
        /// <param name="radians"></param>
        /// <returns></returns>
        public static Scalar ToDegrees(Scalar radians)
        {
            return radians * DegreesPerRadian;
        }

        #region System.Math Methods
        public static Scalar Acos(Scalar d) { return (Scalar)Math.Acos(d); }
        public static Scalar Asin(Scalar d) { return (Scalar)Math.Asin(d); }
        public static Scalar Atan(Scalar d) { return (Scalar)Math.Atan(d); }
        public static Scalar Atan2(Scalar y, Scalar x) { return (Scalar)Math.Atan2(y, x); }
        public static Scalar Ceiling(Scalar a) { return (Scalar)Math.Ceiling(a); }
        public static Scalar Cos(Scalar d) { return (Scalar)Math.Cos(d); }
        public static Scalar Cosh(Scalar value) { return (Scalar)Math.Cosh(value); }
        public static Scalar Exp(Scalar d) { return (Scalar)Math.Exp(d); }
        public static Scalar Floor(Scalar d) { return (Scalar)Math.Floor(d); }
        public static Scalar IEEERemainder(Scalar x, Scalar y) { return (Scalar)Math.IEEERemainder(x, y); }
        public static Scalar Log(Scalar d) { return (Scalar)Math.Log(d); }
        public static Scalar Log10(Scalar d) { return (Scalar)Math.Log10(d); }
        public static Scalar Pow(Scalar x, Scalar y) { return (Scalar)Math.Pow(x, y); }
        public static Scalar Round(Scalar a) { return (Scalar)Math.Round(a); }
        public static Scalar Round(Scalar value, int digits) { return (Scalar)Math.Round(value, digits); }
        public static Scalar Sin(Scalar a) { return (Scalar)Math.Sin(a); }
        public static Scalar Sinh(Scalar value) { return (Scalar)Math.Sinh(value); }
        public static Scalar Sqrt(Scalar d) { return (Scalar)Math.Sqrt(d); }
        public static Scalar Tan(Scalar a) { return (Scalar)Math.Tan(a); }
        public static Scalar Tanh(Scalar value) { return (Scalar)Math.Tanh(value); }
        #endregion


        #endregion
    }
}
