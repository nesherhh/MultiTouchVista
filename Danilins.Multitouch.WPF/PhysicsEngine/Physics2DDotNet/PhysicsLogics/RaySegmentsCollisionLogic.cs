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

using AdvanceMath;

using AdvanceMath.Geometry2D;
using Physics2DDotNet.Shapes;

namespace Physics2DDotNet.PhysicsLogics
{

    public sealed class RaySegmentsCollisionInfo
    {
        internal Body body;
        internal Scalar distance = -1;

        public Body Body
        {
            get { return body; }
        }

        public Scalar Distance
        {
            get { return distance; }
        }
    }
    /// <summary>
    /// A class to manage a RaySegmentsShape collisions
    /// </summary>
    public sealed class RaySegmentsCollisionLogic : PhysicsLogic
    {
        static void Reset(RaySegmentsCollisionInfo[] info)
        {
            for (int index = 0; index < info.Length; ++index)
            {
                info[index].distance = -1;
                info[index].body = null;
            }
        }
        static void Init(RaySegmentsCollisionInfo[] info)
        {
            for (int index = 0; index < info.Length; ++index)
            {
                info[index] = new RaySegmentsCollisionInfo();
            }
        }
        static Lifespan GetLifespan(Body body)
        {
            if (body == null) { throw new ArgumentNullException("body"); }
            return body.Lifetime;
        }

        public event EventHandler NewInfo;

        Body body;
        RaySegmentsCollisionInfo[] current;
        RaySegmentsCollisionInfo[] next;


        public RaySegmentsCollisionLogic(Body body)
            : base(GetLifespan(body))
        {
            this.body = body;
            RaySegmentsShape shape = body.Shape as RaySegmentsShape;
            if (shape == null) { throw new ArgumentException("the shape must be a RaySegmentsShape"); }
            SetShape(shape);
            body.LifetimeChanged += OnLifetimeChanged;
            body.ShapeChanged += OnShapeChanged;
            body.Collided += OnCollided;
        }
        public ReadOnlyCollection<RaySegmentsCollisionInfo> Collisions
        {
            get
            {
                return new ReadOnlyCollection<RaySegmentsCollisionInfo>(current);
            }
        }


        void SetShape(RaySegmentsShape shape)
        {
            current = new RaySegmentsCollisionInfo[shape.Segments.Length];
            next = new RaySegmentsCollisionInfo[shape.Segments.Length];
            Init(current);
            Init(next);
        }

        void OnCollided(object sender, CollisionEventArgs e)
        {
            RaySegmentIntersectionInfo info = e.CustomCollisionInfo as RaySegmentIntersectionInfo;
            if (info != null)
            {
                ReadOnlyCollection<Scalar> distances = info.Distances;

                for (int index = 0; index < next.Length; ++index)
                {
                    RaySegmentsCollisionInfo cinfo = next[index];
                    if (distances[index] != -1 && (cinfo.distance == -1 || distances[index] < cinfo.distance))
                    {
                        cinfo.body = e.Other;
                        cinfo.distance = distances[index];
                    }
                }
            }
        }

        void OnShapeChanged(object sender, EventArgs e)
        {
            RaySegmentsShape newShape = body.Shape as RaySegmentsShape;
            if (newShape == null)
            {
                this.Lifetime = new Lifespan();
                this.Lifetime.IsExpired = true;
                ClearEvents();
            }
            else
            {
                SetShape(newShape);
            }
        }
        void ClearEvents()
        {
            body.LifetimeChanged -= OnLifetimeChanged;
            body.ShapeChanged -= OnShapeChanged;
            body.Collided -= OnCollided;
        }
        void OnLifetimeChanged(object sender, EventArgs e)
        {
            this.Lifetime = body.Lifetime;
        }
        protected internal override void BeforeAddCheck(PhysicsEngine engine)
        {
            if (body.Engine != engine)
            {
                throw new InvalidOperationException("Body must be in the engine");
            }
            base.BeforeAddCheck(engine);
        }
        protected internal override void RunLogic(TimeStep step)
        {
            RaySegmentsCollisionInfo[] temp = current;
            current = next;
            next = temp;
            Reset(next);
            if (NewInfo != null)
            {
                NewInfo(this, EventArgs.Empty);
            }
        }
    }

}