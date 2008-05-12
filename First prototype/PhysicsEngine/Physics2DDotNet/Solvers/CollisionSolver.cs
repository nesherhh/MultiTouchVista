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
using System.Collections.ObjectModel;
using Physics2DDotNet.Joints;


namespace Physics2DDotNet.Solvers
{

    [Serializable]
    public abstract class CollisionSolver
    {
        protected static void SetTag(Body body, object tag)
        {
            if (body == null) { throw new ArgumentNullException("body"); }
            body.SolverTag = tag;
        }

        PhysicsEngine engine;

        protected List<Joint> Joints { get { return engine.joints; } }
        protected List<Body> Bodies { get { return engine.bodies; } }
        /// <summary>
        /// The engine this solver is in.
        /// </summary>
        public PhysicsEngine Engine
        {
            get { return engine; }
        }

        protected internal abstract bool TryGetIntersection(TimeStep step, Body first, Body second, out ReadOnlyCollection<IContactInfo> contacts);
        protected internal abstract void Solve(TimeStep step);

        internal void OnAddedInternal(PhysicsEngine engine)
        {
            if (this.engine != null) { throw new InvalidOperationException(); }
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


        protected internal virtual void Clear() { }

        protected internal virtual void AddBodyRange(List<Body> collection) { }
        protected internal virtual void AddJointRange(List<Joint> collection) { }

        protected internal virtual void RemoveExpiredBodies() { }
        protected internal virtual void RemoveExpiredJoints() { }
        protected void Detect(TimeStep step)
        {
            engine.BroadPhase.Detect(step);
        }

        protected internal abstract void CheckJoint(Joint joint);
    }

}