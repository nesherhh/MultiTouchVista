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

using AdvanceMath.Geometry2D;

namespace Physics2DDotNet.Detectors
{
    [Serializable]
    public sealed class BruteForceDetector : BroadPhaseCollisionDetector
    {
        public override void Detect(TimeStep step)
        {
            for (int index1 = 0; index1 < this.Bodies.Count; index1++)
            {
                Body body1 = this.Bodies[index1];
                for (int index2 = index1 + 1; index2 < this.Bodies.Count; index2++)
                {
                    Body body2 = this.Bodies[index2];
                    if ((body1.Mass.MassInv != 0 || body2.Mass.MassInv != 0) &&
                            Body.CanCollide(body1, body2) &&
                            body1.Rectangle.Intersects(body2.Rectangle))
                    {
                        OnCollision(step, body1, body2);
                    }
                }
            }
        }
    }

}