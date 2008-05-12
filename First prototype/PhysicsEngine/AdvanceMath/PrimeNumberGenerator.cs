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
using System.Collections.Generic;

namespace AdvanceMath
{

    /// <summary>
    /// Generates prime numbers. Just felt like writting one.
    /// </summary>
    public sealed class PrimeNumberGenerator 
    {
        int[] primes;
        int maxNumber;
        public PrimeNumberGenerator(int maxNumber)
        {
            if (maxNumber < 2)
            {
                throw new ArgumentOutOfRangeException("maxNumber");
            }
            List<int> primes = new List<int>(Math.Max((int)Math.Sqrt(maxNumber), 10));
            this.maxNumber = maxNumber;
            for (int pos = 2; pos <= maxNumber; ++pos)
            {
                if (IsPrimeInternal(pos))
                {
                    primes.Add(pos);
                }
            }
            this.primes = primes.ToArray();
        }
        public int[] Primes
        {
            get { return primes; }
        }
        public int MaxNumber
        {
            get { return maxNumber; }
        }
        private bool IsPrimeInternal(int number)
        {
            int length = primes.Length;
            for (int pos = 0; pos < length; ++pos)
            {
                if (number % primes[pos] == 0)
                {
                    return false;
                }
            }
            return true;
        }
        public bool IsPrime(int number)
        {
            if (number > maxNumber || number < 1)
            {
                throw new ArgumentOutOfRangeException("number");
            }
            return IsPrimeInternal(number);
        }
    }
}
