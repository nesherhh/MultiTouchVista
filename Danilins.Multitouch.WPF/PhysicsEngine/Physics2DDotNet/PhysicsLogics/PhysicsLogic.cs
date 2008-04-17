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

namespace Physics2DDotNet.PhysicsLogics
{
    /// <summary>
    /// A physics logic is a way for the engine to effect object within the Update call.
    /// Gravity is a Example of a PhysicsLogic.
    /// </summary>
    [Serializable]
    public abstract class PhysicsLogic : IPhysicsEntity
    {
        #region static
        private static ReadOnlyCollection<Body> none = new ReadOnlyCollection<Body>(new Body[0]);
        #endregion
        #region events
        /// <summary>
        /// Raised when the Lifetime property has been Changed.
        /// </summary>
        public event EventHandler LifetimeChanged;
        /// <summary>
        /// Raised when the object is added to a Physics Engine.
        /// </summary>
        public event EventHandler Added;
        /// <summary>
        /// Raised when the object is Added to the engine but is not yet part of the update process.
        /// </summary>
        public event EventHandler Pending;
        /// <summary>
        /// Raised when the object is Removed from a Physics Engine. 
        /// </summary>
        public event EventHandler<RemovedEventArgs> Removed; 
        #endregion
        #region fields
        private int order;
        private Lifespan lifetime;
        private PhysicsEngine engine;
        private object tag;
        private bool isAdded;
        internal bool isChecked; 
        #endregion
        #region constructors
        protected PhysicsLogic(Lifespan lifetime)
        {
            if (lifetime == null) { throw new ArgumentNullException("lifetime"); }
            this.lifetime = lifetime;
        }
        #endregion
        #region properties
        public virtual ReadOnlyCollection<Body> LogicBodies
        {
            get { return none; }
        }
        /// <summary>
        /// Gets if it has been added the the Engine's PendingQueue, but not yet added to the engine.
        /// </summary>
        public bool IsPending
        {
            get { return engine != null && !isAdded; }
        }
        /// <summary>
        /// Gets if the object has been added to the engine.
        /// </summary>
        public bool IsAdded
        {
            get { return isAdded; }
        }
        /// <summary>
        /// Gets The PhysicsEngine the object is currently in. Null if it is in none.
        /// </summary>
        public PhysicsEngine Engine
        {
            get { return engine; }
        }
        /// <summary>
        /// Gets and Sets a User defined object.
        /// </summary>
        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }
        /// <summary>
        /// Gets and Sets the LifeTime of the object. The object will be removed from the engine when it is Expired.
        /// </summary>
        public Lifespan Lifetime
        {
            get
            {
                return lifetime;
            }
            set
            {
                if (value == null) { throw new ArgumentNullException("value"); }
                if (this.lifetime != value)
                {
                    lifetime = value;
                    if (LifetimeChanged != null) { LifetimeChanged(this, EventArgs.Empty); }
                }
            }
        }

        protected List<Body> Bodies
        {
            get
            {
                return engine.bodies;
            }
        }
        /// <summary>
        /// Get and Sets The order number of when it will be ran.
        /// All Logics with a higher order will run after this one and all logics
        /// with a lower order number will be ran before this one.
        /// </summary>
        public int Order
        {
            get { return order; }
            set
            {
                if (order != value)
                {
                    order = value;
                    if (engine != null)
                    {
                        engine.logicsNeedSorting = true;
                    }
                }
            }
        } 
        #endregion
        #region methods

        protected internal abstract void RunLogic(TimeStep step);

        /// <summary>
        /// Before the item is allowed to be added to pending this method is called to 
        /// throw any exceptions without corrupting the state of the Physics engine.
        /// </summary>
        /// <param name="engine">The engine the item is about to be added too.</param>
        protected internal virtual void BeforeAddCheck(PhysicsEngine engine) { }
        internal void OnPendingInternal(PhysicsEngine engine)
        {
            this.isChecked = true;
            this.engine = engine;
            OnPending();
            if (Pending != null) { Pending(this, EventArgs.Empty); }
        }
        protected virtual void OnPending() { }

        internal void OnAddedInternal()
        {
            this.isAdded = true;
            AddBodyRange(engine.bodies);
            OnAdded();
            if (Added != null) { Added(this, EventArgs.Empty); }
        }
        internal void OnRemovedInternal()
        {
            Clear();
            bool wasPending = this.IsPending;
            PhysicsEngine engine = this.engine;
            this.isAdded = false;
            this.engine = null;
            OnRemoved(engine, wasPending);
            if (Removed != null) { Removed(this, new RemovedEventArgs(engine, wasPending)); }
        }

        protected virtual void OnAdded() { }
        protected virtual void OnRemoved(PhysicsEngine engine, bool wasPending) { }
        protected internal virtual void UpdateTime(TimeStep step)
        {
            this.lifetime.Update(step);
        }

        protected internal virtual void AddBodyRange(List<Body> collection) { }
        protected internal virtual void RemoveExpiredBodies() { }
        protected internal virtual void Clear() { } 
        #endregion
    }
}