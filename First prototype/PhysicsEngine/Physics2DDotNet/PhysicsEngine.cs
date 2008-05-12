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
using System.Collections.ObjectModel;
using System.Collections.Generic;

using AdvanceMath;

using Physics2DDotNet.Solvers;
using Physics2DDotNet.Detectors;
using Physics2DDotNet.Collections;
using Physics2DDotNet.Shapes;
using Physics2DDotNet.PhysicsLogics;
using Physics2DDotNet.Joints;

namespace Physics2DDotNet
{
    /// <summary>
    /// The Engine that will Apply Physics to object added to it.
    /// </summary>
    [Serializable]
    public sealed class PhysicsEngine
    {
        public class LogicComparer : IComparer<PhysicsLogic>
        {
            public int Compare(PhysicsLogic x, PhysicsLogic y)
            {
                return x.Order.CompareTo(y.Order);
            }
        }

        #region static/const fields
        /// <summary>
        /// This is the ID the first body added to the engine will get.
        /// </summary>
        const int firstBodyID = 1;
        static LogicComparer logicComparer = new LogicComparer();
        #endregion
        #region static methods
        private static void PreCheckItem(Joint item)
        {
            if (item == null) { throw new ArgumentNullException("item"); }
            item.isChecked = false;
        }
        private static void PreCheckItem(Body item)
        {
            if (item == null) { throw new ArgumentNullException("item"); }
            item.isChecked = false;
        }
        private static void PreCheckItem(PhysicsLogic item)
        {
            if (item == null) { throw new ArgumentNullException("item"); }
            item.isChecked = false;
        }
        private static void CheckItem(Joint item)
        {
            if (item.Engine != null || item.isChecked) { throw new InvalidOperationException("A Joint cannot be added to more then one engine or added twice."); }
            item.isChecked = true;
        }
        private static void CheckItem(Body item)
        {
            if (item.Engine != null || item.isChecked) { throw new InvalidOperationException("A Body cannot be added to more then one engine or added twice."); }
            item.isChecked = true;
        }
        private static void CheckItem(PhysicsLogic item)
        {
            if (item.Engine != null || item.isChecked) { throw new InvalidOperationException("A PhysicsLogic cannot be added to more then one engine or added twice."); }
            item.isChecked = true;
        }
        #endregion
        #region events
        /// <summary>
        /// Generated when Bodies are truly added to the Engine.
        /// </summary>
        public event EventHandler<CollectionEventArgs<Body>> BodiesAdded;
        /// <summary>
        /// Generated when Joints are truly added to the Engine.
        /// </summary>
        public event EventHandler<CollectionEventArgs<Joint>> JointsAdded;
        /// <summary>
        /// Generated when PhysicsLogics are truly added to the Engine.
        /// </summary>
        public event EventHandler<CollectionEventArgs<PhysicsLogic>> LogicsAdded;
        /// <summary>
        /// Generated when a Bodies are removed to the Engine.
        /// </summary>
        public event EventHandler<CollectionEventArgs<Body>> BodiesRemoved;
        /// <summary>
        /// Generated when a Joints are removed to the Engine.
        /// </summary>
        public event EventHandler<CollectionEventArgs<Joint>> JointsRemoved;
        /// <summary>
        /// Generated when a PhysicsLogics are removed to the Engine.
        /// </summary>
        public event EventHandler<CollectionEventArgs<PhysicsLogic>> LogicsRemoved;
        #endregion
        #region fields
        private int updateCount;
        private int nextBodyID;
        [NonSerialized]
        object syncRoot;
        [NonSerialized]
        AdvReaderWriterLock rwLock;
        [NonSerialized]
        internal bool inUpdate;
        internal bool logicsNeedSorting;

        private List<PhysicsLogic> logics;
        internal List<Body> bodies;
        internal List<Joint> joints;

        private List<PhysicsLogic> pendingLogics;
        private List<Joint> pendingJoints;
        private List<Body> pendingBodies;
        private List<BodyProxy> pendingProxies;

        private List<PhysicsLogic> removedLogics;
        private List<Joint> removedJoints;
        private List<Body> removedBodies;

        private CollisionSolver solver;
        private BroadPhaseCollisionDetector broadPhase;

        #endregion
        #region constructors
        public PhysicsEngine()
        {
            this.nextBodyID = firstBodyID;
            this.syncRoot = new object();
            this.rwLock = new AdvReaderWriterLock();

            this.joints = new List<Joint>();
            this.bodies = new List<Body>();
            this.logics = new List<PhysicsLogic>();

            this.pendingBodies = new List<Body>();
            this.pendingJoints = new List<Joint>();
            this.pendingLogics = new List<PhysicsLogic>();
            this.pendingProxies = new List<BodyProxy>();

            this.removedBodies = new List<Body>();
            this.removedJoints = new List<Joint>();
            this.removedLogics = new List<PhysicsLogic>();
        }
        #endregion
        #region properties
        /// <summary>
        /// Gets A threadSafe List of Joints (You wont get the "The collection has changed" Exception with this)
        /// </summary>
        public ReadOnlyThreadSafeCollection<Joint> Joints
        {
            get
            {
                return new ReadOnlyThreadSafeCollection<Joint>(rwLock, joints);
            }
        }
        /// <summary>
        /// Gets A threadSafe List of Bodies (You wont get the "The collection has changed" Exception with this)
        /// </summary>
        public ReadOnlyThreadSafeCollection<Body> Bodies
        {
            get
            {
                return new ReadOnlyThreadSafeCollection<Body>(rwLock, bodies);
            }
        }
        /// <summary>
        /// Gets A threadSafe List of PhysicsLogics (You wont get the "The collection has changed" Exception with this)
        /// </summary>
        public ReadOnlyThreadSafeCollection<PhysicsLogic> Logics
        {
            get
            {
                return new ReadOnlyThreadSafeCollection<PhysicsLogic>(rwLock, logics);
            }
        }
        /// <summary>
        /// Gets and Sets The BroadPhase collision Detector. (This must be Set to a non-Null value before any calls to Update)
        /// </summary>
        public BroadPhaseCollisionDetector BroadPhase
        {
            get { return broadPhase; }
            set
            {
                using (rwLock.Write)
                {
                    if (broadPhase != value)
                    {
                        if (broadPhase != null) { broadPhase.OnRemovedInternal(); }
                        if (value != null) { value.OnAddedInternal(this); }
                        broadPhase = value;
                    }
                }
            }
        }
        /// <summary>
        /// Gets and Sets the Collision Solver (This must be Set to a non-Null value before any calls to Update)
        /// </summary>
        public CollisionSolver Solver
        {
            get
            {
                return solver;
            }
            set
            {
                using (rwLock.Write)
                {
                    if (solver != value)
                    {
                        if (solver != null) { solver.OnRemovedInternal(); }
                        if (value != null) { value.OnAddedInternal(this); }
                        solver = value;
                    }
                }
            }
        }
        #endregion
        #region methods
        /// <summary>
        /// Adds a Body to the pending queue and will be truly added on a call to Update.
        /// </summary>
        /// <param name="item">The Body to be added.</param>
        public void AddBody(Body item)
        {
            PreCheckItem(item);
            lock (syncRoot)
            {
                CheckItem(item);
                item.OnPending(this);
                pendingBodies.Add(item);
            }
        }
        /// <summary>
        /// Adds a collection of Bodies to the pending queue and will be truly added on a call to Update.
        /// </summary>
        /// <param name="collection">The collection to be Added</param>
        public void AddBodyRange(ICollection<Body> collection)
        {
            if (collection == null) { throw new ArgumentNullException("collection"); }
            if (collection.Count == 0) { return; }
            lock (syncRoot)
            {
                PreCheckBodies(collection);
                CheckBodies(collection);
                BodiesOnPending(collection);
                pendingBodies.AddRange(collection);
            }
        }
        private void PreCheckBodies(ICollection<Body> collection)
        {
            foreach (Body item in collection)
            {
                PreCheckItem(item);
            }
        }
        private void CheckBodies(ICollection<Body> collection)
        {
            foreach (Body item in collection)
            {
                CheckItem(item);
            }
        }
        private void BodiesOnPending(ICollection<Body> collection)
        {
            foreach (Body item in collection)
            {
                item.OnPending(this);
            }
        }

        /// <summary>
        /// Adds a Joint to the pending queue and will be truly added on a call to Update.
        /// </summary>
        /// <param name="item">The Joint to be added.</param>
        public void AddJoint(Joint item)
        {
            PreCheckItem(item);
            lock (syncRoot)
            {
                CheckJoint(item);
                item.OnPendingInternal(this);
                pendingJoints.Add(item);
            }
        }
        /// <summary>
        /// Adds a collection of Joints to the pending queue and will be truly added on a call to Update.
        /// </summary>
        /// <param name="collection">The collection to be Added</param>
        public void AddJointRange(ICollection<Joint> collection)
        {
            if (collection == null) { throw new ArgumentNullException("collection"); }
            if (collection.Count == 0) { return; }
            CheckState();
            lock (syncRoot)
            {
                foreach (Joint item in collection)
                {
                    PreCheckItem(item);
                }
                foreach (Joint item in collection)
                {
                    CheckJoint(item);
                }
                foreach (Joint item in collection)
                {
                    item.OnPendingInternal(this);
                }
                pendingJoints.AddRange(collection);
            }
        }
        /// <summary>
        /// Adds a collection of Joints to the pending queue and will be truly added on a call to Update.
        /// </summary>
        /// <param name="collection">The collection to be Added</param>
        /// <typeparam name="T">A Type inherited from Joint</typeparam>
        public void AddJointRange<T>(ICollection<T> collection)
            where T : Joint
        {
            if (collection == null) { throw new ArgumentNullException("collection"); }
            if (collection.Count == 0) { return; }
            CheckState();
            lock (syncRoot)
            {
                foreach (T item in collection)
                {
                    PreCheckItem(item);
                }
                Joint[] array = new Joint[collection.Count];
                int index = 0;
                foreach (T item in collection)
                {
                    CheckJoint(item);
                    array[index++] = item;
                }
                foreach (T item in collection)
                {
                    item.OnPendingInternal(this);
                }
                pendingJoints.AddRange(array);
            }
        }

        /// <summary>
        /// Adds a PhysicsLogic to the pending queue and will be truly added on a call to Update.
        /// </summary>
        /// <param name="item">The PhysicsLogic to be added.</param>
        public void AddLogic(PhysicsLogic item)
        {
            PreCheckItem(item);
            lock (syncRoot)
            {
                ReadOnlyCollection<Body> logicBodies = item.LogicBodies;
                PreCheckBodies(logicBodies);
                CheckLogic(item);
                CheckBodies(logicBodies);
                item.OnPendingInternal(this);
                pendingLogics.Add(item);
                BodiesOnPending(logicBodies);
                pendingBodies.AddRange(logicBodies);
            }
        }
        /// <summary>
        /// Adds a collection of PhysicsLogics to the pending queue and will be truly added on a call to Update.
        /// </summary>
        /// <param name="collection">The collection to be Added</param>
        public void AddLogicRange(ICollection<PhysicsLogic> collection)
        {
            if (collection == null) { throw new ArgumentNullException("collection"); }
            if (collection.Count == 0) { return; }
            lock (syncRoot)
            {
                List<Body> logicBodies = new List<Body>();
                foreach (PhysicsLogic item in collection)
                {
                    PreCheckItem(item);
                    logicBodies.AddRange(item.LogicBodies);
                }
                PreCheckBodies(logicBodies);
                foreach (PhysicsLogic item in collection)
                {
                    CheckLogic(item);
                }
                CheckBodies(logicBodies);
                foreach (PhysicsLogic item in collection)
                {
                    item.OnPendingInternal(this);
                }
                pendingLogics.AddRange(collection);
                BodiesOnPending(logicBodies);
                pendingBodies.AddRange(logicBodies);
            }
        }
        /// <summary>
        /// Adds a collection of PhysicsLogics to the pending queue and will be truly added on a call to Update.
        /// </summary>
        /// <param name="collection">The collection to be Added</param>
        /// <typeparam name="T">A Type inherited from PhysicsLogic</typeparam>
        public void AddLogicRange<T>(ICollection<T> collection)
            where T : PhysicsLogic
        {
            if (collection == null) { throw new ArgumentNullException("collection"); }
            if (collection.Count == 0) { return; }
            lock (syncRoot)
            {
                List<Body> logicBodies = new List<Body>();
                foreach (T item in collection)
                {
                    PreCheckItem(item);
                    logicBodies.AddRange(item.LogicBodies);
                }
                PreCheckBodies(logicBodies);
                PhysicsLogic[] array = new PhysicsLogic[collection.Count];
                int index = 0;
                foreach (T item in collection)
                {
                    CheckLogic(item);
                    array[index++] = item;
                }
                CheckBodies(logicBodies);
                foreach (T item in collection)
                {
                    item.OnPendingInternal(this);
                }
                pendingLogics.AddRange(array);
                BodiesOnPending(logicBodies);
                pendingBodies.AddRange(logicBodies);
            }
        }
        /// <summary>
        /// Adds 2 bodies to the same proxy list. 
        /// If they are both already part of their own proxy list then the lists will merge.
        /// The transformations will be calcualted automatically. 
        /// </summary>
        /// <param name="body1">The first Body.</param>
        /// <param name="body2">The second Body.</param>
        /// <param name="transformation">How velocities will be transformed from body1 to body2.</param>
        /// <remarks>
        /// This will most likely be removed if i ever figure out how to make a joint like this.
        /// </remarks>
        public void AddProxy(Body body1, Body body2, Matrix2x2 transformation)
        {
            if (body1 == null) { throw new ArgumentNullException("body1"); }
            if (body2 == null) { throw new ArgumentNullException("body2"); }
            if (body1 == body2) { throw new ArgumentException("They cannot be the same body"); }
            lock (syncRoot)
            {
                pendingProxies.Add(new BodyProxy(body1, body2, transformation));
            }
        }
        /// <summary>
        /// Updates the Engine with a change in time. 
        /// This call will block all access to the engine while it is running.
        /// A complete call to this method is also known as a timestep.
        /// </summary>
        /// <param name="dt">The change in time since the last call to this method. (In Seconds)</param>
        public void Update(Scalar dt)
        {
            if (dt < 0) { throw new ArgumentOutOfRangeException("dt"); }
            CheckState();
            rwLock.EnterWrite();
            TimeStep step = new TimeStep(dt, updateCount++);
            inUpdate = true;
            try
            {
                RemoveExpired();
                AddPending();
                if (logicsNeedSorting)
                {
                    logicsNeedSorting = false;
                    logics.Sort(logicComparer);
                }
                UpdateTime(step);
                solver.Solve(step);
                OnPositionChanged();
            }
            finally
            {
                inUpdate = false;
                rwLock.ExitWrite();
            }
        }

        /// <summary>
        /// Clears the Engine of all objects. Also clears the Detector and Solver.
        /// </summary>
        public void Clear()
        {
            rwLock.EnterWrite();
            try
            {
                ClearPending();
                ClearAdded();
                updateCount = 0;
                nextBodyID = firstBodyID;
            }
            finally
            {
                rwLock.ExitWrite();
            }
        }

        private void ClearPending()
        {
            List<Body> pendingBodies;
            List<Joint> pendingJoints;
            List<PhysicsLogic> pendingLogics;
            lock (syncRoot)
            {
                pendingBodies = this.pendingBodies;
                this.pendingBodies = new List<Body>();
                pendingJoints = this.pendingJoints;
                this.pendingJoints = new List<Joint>();
                pendingLogics = this.pendingLogics;
                this.pendingLogics = new List<PhysicsLogic>();
                pendingProxies.Clear();
            }
            foreach (Body body in pendingBodies)
            {
                body.OnRemoved();
            }
            pendingBodies.Clear();
            foreach (Joint joint in pendingJoints)
            {
                joint.OnRemovedInternal();
            }
            pendingJoints.Clear();
            foreach (PhysicsLogic logic in pendingLogics)
            {
                logic.OnRemovedInternal();
            }
            pendingLogics.Clear();
        }
        private void ClearAdded()
        {
            solver.Clear();
            broadPhase.Clear();
            foreach (Body body in bodies)
            {
                body.OnRemoved();
            }
            foreach (Joint joint in joints)
            {
                joint.OnRemovedInternal();
            }
            foreach (PhysicsLogic logic in logics)
            {
                logic.OnRemovedInternal();
            }
            if (BodiesRemoved != null && bodies.Count > 0)
            {
                BodiesRemoved(this, new CollectionEventArgs<Body>(bodies.AsReadOnly()));
            }
            if (JointsRemoved != null && joints.Count > 0)
            {
                JointsRemoved(this, new CollectionEventArgs<Joint>(joints.AsReadOnly()));
            }
            if (LogicsRemoved != null && logics.Count > 0)
            {
                LogicsRemoved(this, new CollectionEventArgs<PhysicsLogic>(logics.AsReadOnly()));
            }
            bodies.Clear();
            joints.Clear();
            logics.Clear();
        }

        private void UpdateTime(TimeStep step)
        {
            for (int index = 0; index < bodies.Count; ++index)
            {
                bodies[index].UpdateTime(step);
            }
            for (int index = 0; index < joints.Count; ++index)
            {
                joints[index].UpdateTime(step);
            }
            for (int index = 0; index < logics.Count; ++index)
            {
                logics[index].UpdateTime(step);
            }
        }
        private void OnPositionChanged()
        {
            int count = bodies.Count;
            for (int index = 0; index < count; ++index)
            {
                bodies[index].OnPositionChanged();
            }
        }

        private void RemoveExpired()
        {
            RemoveExpiredBodies();
            RemoveExpiredJoints();
            RemoveExpiredLogics();
        }
        private void RemoveExpiredBodies()
        {
            if (bodies.RemoveAll(IsBodyExpired) == 0) { return; }
            solver.RemoveExpiredBodies();
            broadPhase.RemoveExpiredBodies();
            for (int index = 0; index < logics.Count; ++index)
            {
                logics[0].RemoveExpiredBodies();
            }
            if (BodiesRemoved != null)
            {
                BodiesRemoved(this, new CollectionEventArgs<Body>(removedBodies.AsReadOnly()));
                removedBodies.Clear();
            }
        }
        private void RemoveExpiredJoints()
        {
            if (joints.RemoveAll(IsJointExpired) == 0) { return; }
            solver.RemoveExpiredJoints();
            if (JointsRemoved != null)
            {
                JointsRemoved(this, new CollectionEventArgs<Joint>(removedJoints.AsReadOnly()));
                removedJoints.Clear();
            }
        }
        private void RemoveExpiredLogics()
        {
            if (logics.RemoveAll(IsLogicExpired) == 0) { return; }
            if (LogicsRemoved != null)
            {
                LogicsRemoved(this, new CollectionEventArgs<PhysicsLogic>(removedLogics.AsReadOnly()));
                removedLogics.Clear();
            }
        }

        private bool IsBodyExpired(Body body)
        {
            if (!body.Lifetime.IsExpired) { return false; }
            if (BodiesRemoved != null) { removedBodies.Add(body); }
            body.OnRemoved();
            return true;
        }
        private bool IsJointExpired(Joint joint)
        {
            if (!joint.Lifetime.IsExpired) { return false; }
            if (JointsRemoved != null) { removedJoints.Add(joint); }
            joint.OnRemovedInternal();
            return true;
        }
        private bool IsLogicExpired(PhysicsLogic logic)
        {
            if (!logic.Lifetime.IsExpired) { return false; }
            if (LogicsRemoved != null) { removedLogics.Add(logic); }
            logic.OnRemovedInternal();
            return true;
        }

        private void AddPending()
        {
            lock (syncRoot)
            {
                if (pendingBodies.Count > 0) { AddPendingBodies(); }
                if (pendingJoints.Count > 0) { AddPendingJoints(); }
                if (pendingLogics.Count > 0) { AddPendingLogics(); }
                if (pendingProxies.Count > 0) { AddPendingProxies(); }
            }
        }
        private void AddPendingBodies()
        {
            for (int index = 0; index < pendingBodies.Count; ++index)
            {
                Body item = pendingBodies[index];
                item.ID = nextBodyID++;
                item.ApplyPosition();
            }
            bodies.AddRange(pendingBodies);
            solver.AddBodyRange(pendingBodies);
            broadPhase.AddBodyRange(pendingBodies);
            for (int index = 0; index < logics.Count; ++index)
            {
                logics[index].AddBodyRange(pendingBodies);
            }
            for (int index = 0; index < pendingBodies.Count; ++index)
            {
                pendingBodies[index].OnAdded();
            }
            if (BodiesAdded != null) { BodiesAdded(this, new CollectionEventArgs<Body>(pendingBodies.AsReadOnly())); }
            pendingBodies.Clear();
        }
        private void AddPendingJoints()
        {
            joints.AddRange(pendingJoints);
            solver.AddJointRange(pendingJoints);
            for (int index = 0; index < pendingJoints.Count; ++index)
            {
                Joint item = pendingJoints[index];
                item.OnAddedInternal(this);
            }
            if (JointsAdded != null) { JointsAdded(this, new CollectionEventArgs<Joint>(pendingJoints.AsReadOnly())); }
            pendingJoints.Clear();
        }
        private void AddPendingLogics()
        {
            logics.AddRange(pendingLogics);
            for (int index = 0; index < pendingLogics.Count; ++index)
            {
                PhysicsLogic logic = pendingLogics[index];
                logic.OnAddedInternal();
            }
            if (LogicsAdded != null) { LogicsAdded(this, new CollectionEventArgs<PhysicsLogic>(pendingLogics.AsReadOnly())); }
            pendingLogics.Clear();
            this.logicsNeedSorting = true;
        }
        private void AddPendingProxies()
        {
            for (int index = 0; index < pendingProxies.Count; ++index)
            {
                BodyProxy proxy = pendingProxies[index];
                Body.CreateProxy(proxy.Body1, proxy.Body2, proxy.transformation);
            }
            pendingProxies.Clear();
        }
        private void CheckState()
        {
            if (this.broadPhase == null) { throw new InvalidOperationException("The BroadPhase property must be set."); }
            if (this.solver == null) { throw new InvalidOperationException("The Solver property must be set."); }
        }
        private void CheckJoint(Joint joint)
        {
            CheckItem(joint);
            joint.BeforeAddCheckInternal(this);
            solver.CheckJoint(joint);
        }
        private void CheckLogic(PhysicsLogic logic)
        {
            CheckItem(logic);
            logic.BeforeAddCheck(this);
        }
        internal void RunLogic(TimeStep step)
        {
            for (int index = 0; index < logics.Count; ++index)
            {
                logics[index].RunLogic(step);
            }
        }
        internal void HandleCollision(TimeStep step, Body body1, Body body2)
        {
            if (body1.Mass.MassInv == 0 && body2.Mass.MassInv == 0) { return; }
            Shape shape1 = body1.Shape;
            Shape shape2 = body2.Shape;


            if (shape1.CanGetCustomIntersection ||
                shape2.CanGetCustomIntersection ||
                shape1.BroadPhaseDetectionOnly ||
                shape2.BroadPhaseDetectionOnly)
            {
                object customIntersectionInfo;
                if (shape1.BroadPhaseDetectionOnly)
                {
                    body1.OnCollision(body2, null);
                }
                else if (shape1.CanGetCustomIntersection &&
                         shape1.TryGetCustomIntersection(body1, body2, out  customIntersectionInfo))
                {
                    body1.OnCollision(body2, customIntersectionInfo);
                }
                if (shape2.BroadPhaseDetectionOnly)
                {
                    body2.OnCollision(body1, null);
                }
                else if (shape2.CanGetCustomIntersection &&
                         shape2.TryGetCustomIntersection(body2, body1, out  customIntersectionInfo))
                {
                    body2.OnCollision(body1, customIntersectionInfo);
                }
            }
            else
            {
                ReadOnlyCollection<IContactInfo> contacts;
                if (solver.TryGetIntersection(step, body1, body2, out contacts))
                {
                    body1.OnCollision(body2, contacts);
                    body2.OnCollision(body1, contacts);
                }
            }

        }
        #endregion
    }
}