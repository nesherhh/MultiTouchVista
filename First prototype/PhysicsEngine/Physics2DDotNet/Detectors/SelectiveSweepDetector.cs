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
  // Contributor: Andrew D. Jones



#if UseDouble
using Scalar = System.Double;
#else
using Scalar = System.Single;
#endif
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using AdvanceMath.Geometry2D;

namespace Physics2DDotNet.Detectors
{
    /// <summary>
    /// Faster then sweep and prune and does not stutter like SingleSweep
    /// </summary>
    [Serializable]
    public sealed class SelectiveSweepDetector : BroadPhaseCollisionDetector
    {
        [Serializable]
        sealed class StubComparer : IComparer<Stub>
        {
            public int Compare(Stub left, Stub right)
            {
                if (left.value < right.value) { return -1; }
                if (left.value > right.value) { return 1; }
                return ((left == right) ? (0) : ((left.begin) ? (-1) : (1)));
            }
        }
        [Serializable]
        sealed class Wrapper : IDeserializationCallback
        {
            [NonSerialized]
            public LinkedListNode<Wrapper> node;
            public Body body;
            public bool shouldAddNode;
            public Scalar min;
            public Scalar max;
            Stub xBegin;
            Stub xEnd;
            Stub yBegin;
            Stub yEnd;
            public Wrapper(Body body)
            {
                this.body = body;
                this.node = new LinkedListNode<Wrapper>(this);
                xBegin = new Stub(this, true);
                xEnd = new Stub(this, false);
                yBegin = new Stub(this, true);
                yEnd = new Stub(this, false);
            }
            public void AddStubs(List<Stub> xStubs, List<Stub> yStubs)
            {
                xStubs.Add(xBegin);
                xStubs.Add(xEnd);

                yStubs.Add(yBegin);
                yStubs.Add(yEnd);
            }
            public void Update()
            {
                BoundingRectangle rect = body.Rectangle;
                //if it is a single point in space
                //then dont even add it to the link list.
                shouldAddNode = rect.Min.X != rect.Max.X || rect.Min.Y != rect.Max.Y;

                xBegin.value = rect.Min.X;
                xEnd.value = rect.Max.X;

                yBegin.value = rect.Min.Y;
                yEnd.value = rect.Max.Y;
            }

            public void SetX()
            {
                this.min = this.xBegin.value;
                this.max = this.xEnd.value;
            }
            public void SetY()
            {
                this.min = this.yBegin.value;
                this.max = this.yEnd.value;
            }
            void IDeserializationCallback.OnDeserialization(object sender)
            {
                this.node = new LinkedListNode<Wrapper>(this);
            }
        }
        [Serializable]
        sealed class Stub
        {
            public Wrapper wrapper;
            public bool begin;
            public Scalar value;
            public Stub(Wrapper wrapper, bool begin)
            {
                this.wrapper = wrapper;
                this.begin = begin;
            }
        }

        static StubComparer comparer = new StubComparer();

        static bool WrapperIsRemoved(Wrapper wrapper)
        {
            return !wrapper.body.IsAdded;
        }
        static bool StubIsRemoved(Stub stub)
        {
            return !stub.wrapper.body.IsAdded;
        }
        List<Wrapper> wrappers;
        List<Stub> xStubs;
        List<Stub> yStubs;

        public SelectiveSweepDetector()
        {
            this.wrappers = new List<Wrapper>();
            this.xStubs = new List<Stub>();
            this.yStubs = new List<Stub>();
        }

        protected internal override void AddBodyRange(List<Body> collection)
        {
            int wrappercount = collection.Count + wrappers.Count;
            if (wrappers.Capacity < wrappercount)
            {
                wrappers.Capacity = wrappercount;
            }
            int nodeCount = collection.Count * 2 + xStubs.Count;
            if (xStubs.Capacity < nodeCount)
            {
                xStubs.Capacity = nodeCount;
                yStubs.Capacity = nodeCount;
            }
            foreach (Body item in collection)
            {
                Wrapper wrapper = new Wrapper(item);
                wrappers.Add(wrapper);
                wrapper.AddStubs(xStubs, yStubs);
            }
        }
        protected internal override void Clear()
        {
            wrappers.Clear();
            xStubs.Clear();
            yStubs.Clear();
        }
        protected internal override void RemoveExpiredBodies()
        {
            wrappers.RemoveAll(WrapperIsRemoved);
            xStubs.RemoveAll(StubIsRemoved);
            yStubs.RemoveAll(StubIsRemoved);
        }

        /// <summary>
        /// updates all the nodes to their new values and sorts the lists
        /// </summary>
        private void Update()
        {
            for (int index = 0; index < wrappers.Count; ++index)
            {
                wrappers[index].Update();
            }
            xStubs.Sort(comparer);
            yStubs.Sort(comparer);
        }

        /// <summary>
        /// Finds how many collisions there are on the x and y and returns if
        /// the x axis has the least
        /// </summary>
        private bool ShouldDoX()
        {
            int xCount = 0;
            int xdepth = 0;
            int yCount = 0;
            int ydepth = 0;
            for (int index = 0; index < xStubs.Count; index++)
            {
                if (xStubs[index].begin) { xCount += xdepth++; }
                else { xdepth--; }

                if (yStubs[index].begin) { yCount += ydepth++; }
                else { ydepth--; }
            }
            return xCount < yCount;
        }

        public override void Detect(TimeStep step)
        {
            Update();
            DetectInternal(step, ShouldDoX());
        }
        private void DetectInternal(TimeStep step, bool doX)
        {
            List<Stub> stubs = (doX) ? (xStubs) : (yStubs);
            LinkedList<Wrapper> currentBodies = new LinkedList<Wrapper>();
            for (int index = 0; index < stubs.Count; index++)
            {
                Stub stub = stubs[index];
                Wrapper wrapper1 = stub.wrapper;
                if (stub.begin)
                {
                    //set the min and max values
                    if (doX) { wrapper1.SetY(); }
                    else { wrapper1.SetX(); }

                    Body body1 = wrapper1.body;
                    for (LinkedListNode<Wrapper> node = currentBodies.First;
                        node != null;
                        node = node.Next)
                    {
                        Wrapper wrapper2 = node.Value;
                        Body body2 = wrapper2.body;
                        if (wrapper1.min <= wrapper2.max && //tests the other axis
                            wrapper2.min <= wrapper1.max &&
                            Body.CanCollide(body1, body2))
                        {
                            OnCollision(step, body1, body2);
                        }
                    }
                    if (wrapper1.shouldAddNode)
                    {
                        currentBodies.AddLast(wrapper1.node);
                    }
                }
                else
                {
                    if (wrapper1.shouldAddNode)
                    {
                        currentBodies.Remove(wrapper1.node);
                    }
                }
            }
        }
    }




}