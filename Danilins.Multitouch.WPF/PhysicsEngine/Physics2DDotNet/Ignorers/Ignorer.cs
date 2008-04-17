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


namespace Physics2DDotNet.Ignorers
{
    /// <summary>
    /// Base class for Collision Ignorers to impliment.
    /// </summary>
    [Serializable]
    public abstract class Ignorer
    {
        bool isInverted;
        internal static bool CanCollide(Ignorer left, Ignorer right)
        {
            return left.CanCollideInternal(right);
        }
        protected Ignorer() { }
        protected Ignorer(Ignorer copy)
        {
            this.isInverted = copy.isInverted;
        }
        /// <summary>
        /// Get and sets if the result of this ignorer is inverted.
        /// </summary>
        public bool IsInverted
        {
            get { return isInverted; }
            set { isInverted = value; }
        }
        public abstract bool BothNeeded { get;}
        private bool CanCollideInternal(Ignorer other)
        {
            return isInverted ^ CanCollide(other);
        }
        protected abstract bool CanCollide(Ignorer other);
        public virtual void UpdateTime(TimeStep step) { }
    }
}