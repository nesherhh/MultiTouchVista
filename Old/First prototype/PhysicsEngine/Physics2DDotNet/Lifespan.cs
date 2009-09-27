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
namespace Physics2DDotNet
{
    /// <summary>
    /// A object that describes the time a object will remain in the Physics engine.
    /// </summary>
    [Serializable]
    public sealed class Lifespan : IDuplicateable<Lifespan>
    {
        #region fields
        int lastUpdate;
        Scalar age;
        Scalar maxAge;
        bool isExpired;
        #endregion
        #region constructors
        /// <summary>
        /// Creates a new Lifespan Instance that is Immortal.
        /// </summary>
        public Lifespan()
            : this(0, Scalar.PositiveInfinity)
        { }
        /// <summary>
        /// Creates a new Lifespan Instance that is mortal.
        /// </summary>
        /// <param name="maxAge">How long the item will stay in the engine. (in seconds)</param>
        public Lifespan(Scalar maxAge)
            : this(0, maxAge)
        { }
        /// <summary>
        /// Creates a new Lifespan Instance that is mortal and has already aged.
        /// </summary>
        /// <param name="age">How old the item is. (in seconds)</param>
        /// <param name="maxAge">How long the item will stay in the engine. (in seconds)</param>
        public Lifespan(Scalar age, Scalar maxAge)
        {
            this.lastUpdate = -1;
            this.age = age;
            this.maxAge = maxAge;
        }
        private Lifespan(Lifespan self)
        {
            this.lastUpdate = self.lastUpdate;
            this.age = self.age;
            this.maxAge = self.maxAge;
            this.isExpired = self.isExpired;
        }
        #endregion
        #region properties
        /// <summary>
        /// Gets and Sets if it IsExpired and should be removed from the engine.
        /// </summary>
        public bool IsExpired
        {
            get
            {
                return isExpired || age >= maxAge;
            }
            set
            {
                if (value)
                {
                    isExpired = true;
                }
                else
                {
                    isExpired = false;
                    age = 0;
                }
            }
        }
        /// <summary>
        /// Gets if the only way for the object to leave the engine is for it to be set to expired.
        /// </summary>
        public bool IsImmortal
        {
            get
            {
                return Scalar.IsPositiveInfinity(maxAge);
            }
        }
        /// <summary>
        /// Gets if it is expired becuase of old age.
        /// </summary>
        public bool OverAged
        {
            get
            {
                return age >= maxAge;
            }
        }
        /// <summary>
        /// Gets and Sets how long the object will stay in the engine.
        /// </summary>
        public Scalar MaxAge
        {
            get { return maxAge; }
            set { maxAge = value; }
        }
        /// <summary>
        /// Gets how much time the object has left.
        /// </summary>
        public Scalar TimeLeft
        {
            get
            {
                return maxAge - age;
            }
        }
        /// <summary>
        /// Gets and Sets The current age of the object.
        /// </summary>
        public Scalar Age
        {
            get
            {
                return age;
            }
            set
            {
                age = value;
            }
        }
        #endregion
        #region methods
        /// <summary>
        /// Increases the Age of object by a change in time.
        /// </summary>
        /// <param name="update">the update's number (It wont age more then once on a update)</param>
        /// <param name="step">The TimeStep describing the change in time.</param>
        public void Update(TimeStep step)
        {
            if (step.UpdateCount != lastUpdate)
            {
                age += step.Dt;
                lastUpdate = step.UpdateCount;
            }
        }
        public Lifespan Duplicate()
        {
            return new Lifespan(this);
        }
        public object Clone()
        {
            return Duplicate();
        }
        #endregion
    }
}
