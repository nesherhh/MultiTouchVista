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

namespace Physics2DDotNet.Detectors
{
    [Serializable]
    public abstract class BroadPhaseCollisionDetector
    {
        protected static void SetTag(Body body, object tag)
        {
            if (body == null) { throw new ArgumentNullException("body"); }
            body.DetectorTag = tag;
        }

        PhysicsEngine engine;
        public PhysicsEngine Engine
        {
            get { return engine; }
        }
        protected List<Body> Bodies
        {
            get
            {
                return engine.bodies;
            }
        }

        public abstract void Detect(TimeStep step);
        internal void OnAddedInternal(PhysicsEngine engine)
        {
            if (this.engine != null)
            {
                throw new InvalidOperationException("A broadphsed Detector cannot be added to more then one engine.");
            }
            this.engine = engine;
            OnAdded();
            this.AddBodyRange(engine.bodies);
        }
        internal void OnRemovedInternal()
        {
            engine = null;
            Clear();
            OnRemoved();
        }
        protected virtual void OnAdded() { }
        protected virtual void OnRemoved() { }

        protected internal virtual void AddBodyRange(List<Body> collection) { }
        protected internal virtual void RemoveExpiredBodies() { }

        protected internal virtual void Clear() { }
        protected void OnCollision(TimeStep step, Body first, Body second)
        {
            this.engine.HandleCollision(step, first, second);
        }
    }

}