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

using AdvanceMath.Geometry2D;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;

namespace Physics2DDotNet.Detectors
{
    /// <summary>
    /// Full name is Frame Coherent Sweep and Prune. 
    /// This class is used to isolate the AABB pairs that are currently in a collision
    /// state without having to check all pair combinations. It relies heavily on frame
    /// coherence or the idea that objects will typically be near their last position
    /// from frame to frame. The class caches the various state information and doesn't
    /// update it unless an extent on an axis "swaps" positions with its neighbor.
    /// Note: If your application has "teleporting" objects or objects that are 
    /// extremely high-speed in relation to other objects, then this Sweep and Prune
    /// method may breakdown. 
    /// </summary>
    public sealed class FrameCoherentSAPDetector : BroadPhaseCollisionDetector
    {
        /// <summary>
        /// This class keeps a list of information that relates extents to geometries.
        /// </summary>
        sealed class ExtentInfoList : List<ExtentInfo>
        {
            public FrameCoherentSAPDetector owner;
            public ExtentInfoList(FrameCoherentSAPDetector sap) { owner = sap; }

            public void MoveUnderConsiderationToOverlaps()
            {
                for (int i = 0; i < Count; i++)
                {
                    if (this[i].underConsideration.Count == 0)
                        continue;

                    Body g1 = this[i].body;
                    BoundingRectangle aabb1 = g1.Rectangle;

                    // First transfer those under consideration to overlaps,
                    // for, they have been considered...
                    int startIndex = this[i].overlaps.Count;
                    this[i].overlaps.AddRange(this[i].underConsideration);
                    this[i].underConsideration.Clear();

                    for (int j = startIndex; j < this[i].overlaps.Count; j++)
                    {
                        Body g2 = this[i].overlaps[j];

                        // It is possible that we may test the same pair of geometries
                        // for both extents (x and y), however, I'm banking on that
                        // one of the extents has probably already been cached and
                        // therefore, won't be checked.
                        if (FrameCoherentSAPDetector.TestForCollisions(g1, g2) == false)
                            continue;

                        owner.collisionPairs.AddPair(g1, g2);
                    }
                }
            }
        }

        /// <summary>
        /// This class is used to keep track of the pairs of geometry that need to be
        /// passed on to the narrow phase. The keys stored in the dictionary are
        /// the actual geometry pairs (the boolean value is currently unused).
        /// NOTE: May eventually want to add OnEnterCollisionState / 
        /// OnExitCollisionState callbacks which might be useful for debugging
        /// or possibly in user applications.
        /// </summary>
        sealed class CollisionPairDictionary : Dictionary<CollisionPair, bool>
        {
            public void RemovePair(Body g1, Body g2)
            {
                CollisionPair cp = new CollisionPair(g1, g2);
                Remove(cp);

                // May want a OnDeactivatedCollision here.
                // For example, if you were highlighting colliding
                // ABV's for debugging, this callback would let you
                // know to stop.
            }

            public void AddPair(Body g1, Body g2)
            {
                CollisionPair cp = new CollisionPair(g1, g2);

                // This check is a trade-off. In many cases, we don't need to perform
                // this check and we could just do a "try" block instead, however,
                // when exceptions are thrown, they are mega-slow... so checking for
                // the key is really the best option all round.
                if (ContainsKey(cp))
                    return;

                Add(cp, true);
            }
        }

        /// <summary>
        /// Houses collision pairs as geom1 and geom2. The pairs are always ordered such
        /// that the lower id geometry is first. This allows the CollisionPairDictionary
        /// to have a consistent key / hash code for a pair of geometry.
        /// </summary>
        struct CollisionPair
        {
            public Body body1;
            public Body body2;

            public CollisionPair(Body body1, Body body2)
            {
                System.Diagnostics.Debug.Assert(body1 != body2);

                if (body1.ID < body2.ID)
                {
                    this.body1 = body1;
                    this.body2 = body2;
                }
                else
                {
                    this.body1 = body2;
                    this.body2 = body1;
                }
            }

            public override int GetHashCode()
            {
                // Arbitrarly choose 20000 as a number of colliders that we won't 
                // approach any time soon.
                return (body1.ID * 20000 + body2.ID);
            }
        }

        /// <summary>
        /// This class represents a single extent of an AABB on a single axis. It has a
        /// reference to ExtentInfo which has information about the geometry it belongs
        /// to.
        /// </summary>
        sealed class Extent
        {
            public bool isMin;
            public Scalar value;
            public ExtentInfo info;
            public Extent(ExtentInfo info, Scalar value, bool isMin)
            {
                this.info = info;
                this.value = value;
                this.isMin = isMin;
            }
        }

        /// <summary>
        /// Represents a lists of extents for a given axis. This list will be kept
        /// sorted incrementally.
        /// </summary>
        sealed class ExtentList : List<Extent>
        {
            public FrameCoherentSAPDetector owner;
            public ExtentList(FrameCoherentSAPDetector sap) { owner = sap; }

            /// <summary>
            /// Inserts a new Extent into the already sorted list. As the ExtentList
            /// class is currently derived from the generic List class, insertions
            /// of new geometry (and extents) are going to be somewhat slow right
            /// off the bat. Additionally, this function currently performs 
            /// linear insertion. Two big optimizations in the future would be to
            /// (1) make this function perform a binary search and (2) allow for
            /// a "hint" of what index to start with. The reason for this is because
            /// there is always a min and max extents that need inserting and we
            /// know the max extent is always after the min extent.
            /// </summary>
            /*private int InsertIntoSortedList(Extent newExtent)
            {
                // List<> is not the most speedy for insertion, however, since
                // we don't plan to do this except for when geometry is added
                // to the system, we go ahead an use List<> instead of LinkedList<>
                // This code, btw, assumes the list is already sorted and that
                // the new entry just needs to be inserted.
                //
                // Optimization Note: A binary search could be used here and would
                // improve speed for when adding geometry.
                int insertAt = 0;

                // Check for empty list
                if (this.Count == 0)
                {
                    this.Add(newExtent);
                    return 0;
                }

                while (insertAt < this.Count &&
                    (this[insertAt].value < newExtent.value ||
                    (this[insertAt].value == newExtent.value &&
                    this[insertAt].isMin && !newExtent.isMin)))
                {
                    insertAt++;
                }

                this.Insert(insertAt, newExtent);
                return insertAt;
            }

            /// <summary>
            /// Incrementally inserts the min/max extents into the ExtentList. As it
            /// does so, the method ensures that overlap records, the collisionpair
            /// map, and all other book-keeping is up todate.
            /// </summary>
            /// <param name="ourInfo">The extent info for a give axis</param>
            public void IncrementalInsertExtent(ExtentInfo ourInfo)
            {
                Extent min = ourInfo.min;
                Extent max = ourInfo.max;

                System.Diagnostics.Debug.Assert(min.value < max.value);

                int iMin = InsertIntoSortedList(min);
                int iMax = InsertIntoSortedList(max);

                Body ourGeom = ourInfo.body;

                // As this is a newly inserted extent, we need to update the overlap 
                // information.

                // RULE 1: Traverse from min to max. Look for other "min" Extents
                // and when found, add our wrapper/geometry to their list.
                int iCurr = iMin + 1;
                while (iCurr != iMax)
                {
                    if (this[iCurr].isMin)
                        this[iCurr].info.underConsideration.Add(ourGeom);
                    iCurr++;
                }

                // RULE 2: From min, traverse to the left until we encounter
                // another "min" extent. If we find one, we add its geometry
                // to our underConsideration list and go to RULE 3, otherwise
                // there is no more work to do and we can exit.
                iCurr = iMin - 1;
                while (iCurr >= 0 && this[iCurr].isMin == false)
                    iCurr--;

                if (iCurr < 0)
                    return;

                List<Body> ourUnderConsideration = ourInfo.underConsideration;
                Extent currExtent = this[iCurr];

                ourUnderConsideration.Add(currExtent.info.body);

                // RULE 3: Now that we have found a "min" extent, we take
                // its existing overlap list and copy it into our underConsideration
                // list. All except for ourselves.
                ourUnderConsideration.AddRange(currExtent.info.underConsideration);
                ourUnderConsideration.Remove(ourGeom); // just in case

                // RULE 4: Move from the found extent back toward our "min" extent.
                // Whenever and "max" extent is found, we remove its reference
                // from our extents list.
                while (iCurr != iMin)
                {
                    if (currExtent.isMin == false)
                    {
                        ourUnderConsideration.Remove(currExtent.info.body);

                        if (ourInfo.overlaps.Remove(currExtent.info.body))
                        {
                            owner.collisionPairs.RemovePair(ourGeom,
                                currExtent.info.body);
                        }
                    }
                    currExtent = this[++iCurr];
                }
            }*/

            /// <summary>
            /// Incrementally sorts ExtentList. It is assumed that there is a high level
            /// of frame coherence and that much of the list is already fairly well
            /// sorted. This algorithm makes use of "insert sort" which is notoriously
            /// slow - except for when a list is already almost sorted - which is the
            /// case when there is high frame coherence.
            /// </summary>
            public void IncrementalSort()
            {
                if (Count < 2)
                    return;

                for (int i = 0; i < Count - 1; i++)
                {
                    int evalCnt = i + 1;
                    Extent evalExtent = this[evalCnt];
                    Extent currExtent = this[i];

                    if (currExtent.value <= evalExtent.value)
                        continue;

                    Extent savedExtent = evalExtent;

                    if (evalExtent.isMin)
                    {
                        while (currExtent.value > evalExtent.value)
                        {
                            if (currExtent.isMin)
                            {
                                // Begin extent is inserted before another begin extent.
                                // So, Inserted extent's object looses reference to
                                // non-inserted extent and Non-inserted extent gains 
                                // reference to inserted extent's object.
                                evalExtent.info.underConsideration.Remove(
                                    currExtent.info.body);

                                if (evalExtent.info.overlaps.Remove(
                                    currExtent.info.body))
                                {
                                    owner.collisionPairs.RemovePair(
                                        evalExtent.info.body,
                                        currExtent.info.body);
                                }

                                // Add extent
                                currExtent.info.underConsideration.Add(
                                    evalExtent.info.body);
                            }
                            else
                            {
                                // "min" extent inserted before the max extent.
                                // Inserted extent gains reference to non-inserted extent.
                                evalExtent.info.underConsideration.Add(
                                    currExtent.info.body);
                            }

                            this[evalCnt--] = this[i--];
                            if (i < 0)
                                break;
                            currExtent = this[i];
                        }
                    }
                    else
                    {
                        while (currExtent.value > evalExtent.value)
                        {
                            if (currExtent.isMin)
                            {
                                // Ending extent inserted before a beginning extent
                                // the non inserted extent looses a reference to the
                                // inserted one
                                currExtent.info.underConsideration.Remove(
                                    evalExtent.info.body);

                                if (currExtent.info.overlaps.Remove(
                                    evalExtent.info.body))
                                {
                                    owner.collisionPairs.RemovePair(
                                        evalExtent.info.body,
                                        currExtent.info.body);
                                }
                            }
                            this[evalCnt--] = this[i--];
                            if (i < 0)
                                break;
                            currExtent = this[i];
                        }
                    }
                    this[evalCnt] = savedExtent;
                }
            }
        }

        /// <summary>
        /// This class contains represents additional extent info for a particular axis
        /// It has a reference to the geometry whose extents are being tracked. It
        /// also has a min and max extent reference into the ExtentList itself.
        /// The class keeps track of overlaps with other geometries.
        /// </summary>
        sealed class ExtentInfo
        {
            public Body body; // Specific to Farseer
            public Extent min;
            public Extent max;
            public List<Body> overlaps;
            public List<Body> underConsideration;

            public ExtentInfo(Body g, Scalar min, Scalar max)
            {
                this.body = g;
                this.underConsideration = new List<Body>();
                this.overlaps = new List<Body>();
                this.min = new Extent(this, min, true);
                this.max = new Extent(this, max, false);
            }
        }


        /// <summary>
        /// Test AABB collisions between two geometries. Tests include checking if the
        /// geometries are enabled, static, in the right collision categories, etc.
        /// </summary>
        /// <returns>Returns true if there is a collision, false otherwise</returns>
        static bool TestForCollisions(Body g1, Body g2)
        {



            //TMP
            /*            AABB aabb1 = new AABB();
                        AABB aabb2 = new AABB();
                        aabb1.min = g1.aabb.min;
                        aabb1.max = g1.aabb.max;
                        aabb2.min = g2.aabb.min;
                        aabb2.max = g2.aabb.max;
                        aabb1.min.X -= fTol;
                        aabb1.min.Y -= fTol;
                        aabb1.max.X += fTol;
                        aabb1.max.Y += fTol;
                        aabb2.min.X -= fTol;
                        aabb2.min.Y -= fTol;
                        aabb2.max.X += fTol;
                        aabb2.max.Y += fTol;
                        if (AABB.Intersect(aabb1, aabb2) == false)
                            return false;
            */
            /* if (AABB.Intersect(g1.Rectangle, g2.Rectangle) == false)
                 return false;*/
            if (!g1.Rectangle.Intersects(g2.Rectangle))
            {
                return false;
            }
            if (!Body.CanCollide(g1, g2))
            {
                return false;
            }

            return true;
        }

        ExtentList xExtentList;
        ExtentList yExtentList;
        ExtentInfoList xInfoList;
        ExtentInfoList yInfoList;
        CollisionPairDictionary collisionPairs;
        //static public Scalar fTol = 1.5f; //.01f;
        TimeStep step;

        public FrameCoherentSAPDetector()
        {
            this.xExtentList = new ExtentList(this);
            this.yExtentList = new ExtentList(this);
            this.xInfoList = new ExtentInfoList(this);
            this.yInfoList = new ExtentInfoList(this);
            this.collisionPairs = new CollisionPairDictionary();
        }

        protected internal override void RemoveExpiredBodies()
        {
            xInfoList.RemoveAll(delegate(ExtentInfo i) { return !i.body.IsAdded; });
            xExtentList.RemoveAll(delegate(Extent n) { return !n.info.body.IsAdded; });
            yInfoList.RemoveAll(delegate(ExtentInfo i) { return !i.body.IsAdded; });
            yExtentList.RemoveAll(delegate(Extent n) { return !n.info.body.IsAdded; });
            // We force a non-incremental update because that will insure that the
            // collisionPairs get recreated and that the geometry isn't being held
            // by overlaps, etc. Its just easier this way.
            ForceNonIncrementalUpdate();
        }

        //TODO: do a merge sort insertion
        protected internal override void AddBodyRange(List<Body> collection)
        {
            foreach (Body b in collection)
            {
                AddGeom(b);
            }
            ForceNonIncrementalUpdate();
        }

        public override void Detect(TimeStep step)
        {
            this.step = step;
            this.Run();
        }
        protected internal override void Clear()
        {
            xExtentList.Clear();
            yExtentList.Clear();
            xInfoList.Clear();
            yInfoList.Clear();
            collisionPairs.Clear();
        }

        /// <summary>
        /// This method is used by the PhysicsSimulator to notify Sweep and Prune that 
        /// new geometry is to be tracked.
        /// </summary>
        /// <param name="g">The geometry to be added</param>
        void AddGeom(Body g)
        {
            ExtentInfo xExtentInfo = new ExtentInfo(g, g.Rectangle.Min.X, g.Rectangle.Max.X);
            xInfoList.Add(xExtentInfo);
            xExtentList.Add(xExtentInfo.min);
            xExtentList.Add(xExtentInfo.max);

            ExtentInfo yExtentInfo = new ExtentInfo(g, g.Rectangle.Min.Y, g.Rectangle.Max.Y);
            yInfoList.Add(yExtentInfo);
            yExtentList.Add(yExtentInfo.min);
            yExtentList.Add(yExtentInfo.max);
        }


        /// <summary>
        /// Updates the values in the x and y extent lists by the changing aabb values.
        /// </summary>
        private void UpdateExtentValues()
        {
            System.Diagnostics.Debug.Assert(xInfoList.Count == yInfoList.Count);
            for (int i = 0; i < xInfoList.Count; i++)
            {
                ExtentInfo xInfo = xInfoList[i];
                ExtentInfo yInfo = yInfoList[i];
                System.Diagnostics.Debug.Assert(xInfo.body == yInfo.body);
                BoundingRectangle aabb = xInfo.body.Rectangle;

                xInfo.min.value = aabb.Min.X;
                xInfo.max.value = aabb.Max.X;
                yInfo.min.value = aabb.Min.Y;
                yInfo.max.value = aabb.Max.Y;

                /*                xInfo.min.value = aabb.min.X - fTol;
                                xInfo.max.value = aabb.max.X + fTol;
                                yInfo.min.value = aabb.min.Y - fTol;
                                yInfo.max.value = aabb.max.Y + fTol;*/
            }
        }

        /// <summary>
        /// Iterates over the collision pairs and creates arbiters.
        /// </summary>
        private void HandleCollisions()
        {
            foreach (CollisionPair cp in collisionPairs.Keys)
            {
                // Note: Possible optimization. Maybe arbiter can be cached into value of
                // collisionPairs? Currently, the collisionPairs hash doesn't use its
                // value parameter - its just an unused bool value.
                /* Arbiter arbiter;
                 arbiter = physicsSimulator.arbiterPool.Fetch();
                 arbiter.ConstructArbiter(cp.geom1, cp.geom2,
                     physicsSimulator.allowedPenetration,
                     physicsSimulator.biasFactor,
                     physicsSimulator.maxContactsToDetect,
                     physicsSimulator.maxContactsToResolve);

                 if (!physicsSimulator.arbiterList.Contains(arbiter))
                     physicsSimulator.arbiterList.Add(arbiter);
                 else
                     physicsSimulator.arbiterPool.Release(arbiter);*/
                OnCollision(step, cp.body1, cp.body2);
            }
        }

        bool bForce = false;
        /// <summary>
        /// Just calls Update.
        /// </summary>
        void Run()
        {
            if (bForce)
                ForceNonIncrementalUpdate();
            else
                Update();
        }

        /// <summary>
        /// Incrementally updates the system. Assumes relatively good frame coherence.
        /// </summary>
        void Update()
        {
            UpdateExtentValues();

            xExtentList.IncrementalSort();
            yExtentList.IncrementalSort();

            xInfoList.MoveUnderConsiderationToOverlaps();
            yInfoList.MoveUnderConsiderationToOverlaps();

            HandleCollisions();
        }

        /// <summary>
        /// This function can be used for times when frame-coherence is temporarily lost
        /// or when it is simply more convenient to completely rebuild all the cached
        /// data instead of incrementally updating it. Currently it is used after
        /// removing disposed/removed geometries. If your application had an object
        /// that teleported across the universe or some other situation where
        /// frame-coherence was lost, you might consider this function.
        /// </summary>
        public void ForceNonIncrementalUpdate()
        {
            UpdateExtentValues();

            // First, wipe out the collision records
            collisionPairs.Clear();

            // And clear out all the overlap records
            System.Diagnostics.Debug.Assert(xInfoList.Count == yInfoList.Count);
            for (int i = 0; i < xInfoList.Count; i++)
            {
                xInfoList[i].overlaps.Clear();
                xInfoList[i].underConsideration.Clear();
                yInfoList[i].overlaps.Clear();
                yInfoList[i].underConsideration.Clear();
            }

            // Force sort
            xExtentList.Sort(delegate(Extent l, Extent r)
                { return l.value.CompareTo(r.value); });
            yExtentList.Sort(delegate(Extent l, Extent r)
                { return l.value.CompareTo(r.value); });

            // Rebuild overlap information
            List<Body> overlaps = new List<Body>();
            for (int i = 0; i < 2; i++)
            {
                overlaps.Clear();

                ExtentList extentList = null;
                if (i == 0)
                    extentList = xExtentList;
                else
                    extentList = yExtentList;

                foreach (Extent extent in extentList)
                {
                    if (extent.isMin)
                    {
                        // Add whatever is currently in overlaps to this
                        extent.info.overlaps.AddRange(overlaps);

                        // Now add, this geom to overlaps
                        overlaps.Add(extent.info.body);
                    }
                    else
                    {
                        // remove this geom from overlaps
                        overlaps.Remove(extent.info.body);

                        // Test this geom against its overlaps for collisionpairs
                        Body thisGeom = extent.info.body;
                        foreach (Body g in extent.info.overlaps)
                        {
                            if (FrameCoherentSAPDetector.TestForCollisions(thisGeom, g) == false)
                                continue;

                            collisionPairs.AddPair(thisGeom, g);
                        }
                    }
                }
            }
            HandleCollisions();
        }
    }
}

