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
    public sealed class SpatialHashDetector : BroadPhaseCollisionDetector
    {
        private Dictionary<long, List<Body>> hash;
        private Dictionary<long, object> filter;
        private Scalar cellSize;
        private Scalar cellSizeInv;
        bool autoAdjustCellSize = true;
        public SpatialHashDetector() : this(50, 2048) { }
        public SpatialHashDetector(Scalar cellSize, int hashCapacity)
        {
            this.hash = new Dictionary<long, List<Body>>(hashCapacity);
            this.filter = new Dictionary<long, object>();
            this.cellSize = cellSize;
            this.cellSizeInv = 1 / cellSize;
        }
        public Scalar CellSize
        {
            get { return cellSize; }
            set
            {
                cellSize = value;
                cellSizeInv = 1 / value;
            }
        }
        public bool AutoAdjustCellSize
        {
            get { return autoAdjustCellSize; }
            set { autoAdjustCellSize = value; }
        }
        public override void Detect(TimeStep step)
        {
            if (Bodies.Count == 0) { return; }
            FillHash();
            RunHash(step);
        }
        private void FillHash()
        {
            Scalar average = 0;
            for (int index = 0; index < Bodies.Count; index++)
            {
                Body body = this.Bodies[index];
                if (!body.IsCollidable) { continue; }

                BoundingRectangle rect = body.Rectangle;
                average += Math.Max(rect.Max.X - rect.Min.X, rect.Max.Y - rect.Min.Y);
                int minX = (int)(rect.Min.X * cellSizeInv);
                int maxX = (int)(rect.Max.X * cellSizeInv) + 1;
                int minY = (int)(rect.Min.Y * cellSizeInv);
                int maxY = (int)(rect.Max.Y * cellSizeInv) + 1;

                for (int x = minX; x < maxX; x++)
                {
                    for (int y = minY; y < maxY; y++)
                    {
                        long key = PairID.GetHash(x, y);
                        List<Body> list;
                        if (!hash.TryGetValue(key, out list))
                        {
                            list = new List<Body>();
                            hash.Add(key, list);
                        }
                        list.Add(body);
                    }
                }
            }
            if (autoAdjustCellSize)
            {
                CellSize = 2 * average / (Bodies.Count);
            }
        }
        private void RunHash(TimeStep step)
        {
            foreach (List<Body> list in hash.Values)
            {
                for (int index1 = 0; index1 < list.Count - 1; index1++)
                {
                    Body body1 = list[index1];
                    for (int index2 = index1 + 1; index2 < list.Count; index2++)
                    {
                        Body body2 = list[index2];
                        if ((body1.Mass.MassInv != 0 || body2.Mass.MassInv != 0) &&
                             Body.CanCollide(body1, body2) &&
                             body1.Rectangle.Intersects(body2.Rectangle))
                        {
                            long key = PairID.GetHash(body1.ID, body2.ID);
                            if (!filter.ContainsKey(key))
                            {
                                filter.Add(key, null);
                                OnCollision(step, body1, body2);
                            }
                        }
                    }
                }
                list.Clear();
            }
            filter.Clear();
        }
        protected internal override void Clear()
        {
            hash.Clear();
        }
    }
}