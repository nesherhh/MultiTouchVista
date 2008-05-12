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

using AdvanceMath;
using AdvanceMath.Geometry2D;
using Physics2DDotNet.Shapes;

namespace Physics2DDotNet.PhysicsLogics
{
    /// <summary>
    /// Applys drag and buoyancy to items on one side of a line;
    /// </summary>
    public sealed class LineFluidLogic : PhysicsLogic
    {
        sealed class Wrapper : IDisposable
        {
            public Vector2D centroid;
            public Scalar area;
            public Body body;
            public ILineFluidAffectable affectable;
            public Wrapper(Body body)
            {
                this.body = body;
                this.body.ShapeChanged += OnShapeChanged;
                this.Calculate();
            }
            void Calculate()
            {
                CalculatePart();
                if (Math.Abs(this.centroid.X) < .00001f)
                {
                    this.centroid.X = 0;
                }
                if (Math.Abs(this.centroid.Y) < .00001f)
                {
                    this.centroid.Y = 0;
                }
            }
            void CalculatePart()
            {
                Shape shape = body.Shape;
                affectable = shape as ILineFluidAffectable;
                if (affectable != null)
                {
                    area = affectable.Area;
                    centroid = affectable.Centroid;
                }
            }
            void OnShapeChanged(object sender, EventArgs e)
            {
                Calculate();
            }
            public void Dispose()
            {
                this.body.ShapeChanged -= OnShapeChanged;
            }
        }
        static bool IsRemoved(Wrapper wrapper)
        {
            if (!wrapper.body.IsAdded)
            {
                wrapper.Dispose();
                return true;
            }
            return false;
        }
        Scalar dragCoefficient;
        Scalar density;
        Vector2D fluidVelocity;
        Line line;

        List<Wrapper> items;


        public LineFluidLogic(
            Line line,
            Scalar dragCoefficient,
            Scalar density,
            Vector2D fluidVelocity,
            Lifespan lifetime)
            : base(lifetime)
        {
            this.line = line;
            this.dragCoefficient = dragCoefficient;
            this.density = density;
            this.fluidVelocity = fluidVelocity;
            this.Order = 1;
            this.items = new List<Wrapper>();
        }

        public Scalar DragCoefficient
        {
            get { return dragCoefficient; }
            set { dragCoefficient = value; }
        }
        public Scalar Density
        {
            get { return density; }
            set { density = value; }
        }
        public Vector2D FluidVelocity
        {
            get { return fluidVelocity; }
            set { fluidVelocity = value; }
        }

        protected internal override void RunLogic(TimeStep step)
        {
            for (int index = 0; index < items.Count; ++index)
            {
                Wrapper wrapper = items[index];
                Body body = wrapper.body;
                if (wrapper.affectable == null ||
                   Scalar.IsPositiveInfinity(body.Mass.Mass))
                {
                    continue;
                }
                Shape shape = body.Shape;
                int isInsideCount = 0;
                foreach (Vector2D corner in body.Rectangle.Corners())
                {
                    Scalar distance = line.GetDistance(corner);
                    if (distance <= 0)
                    {
                        isInsideCount++;
                    }
                }
                if (isInsideCount == 0) { continue; }

                if (isInsideCount != 4)
                {
                    Vector2D relativeVelocity = Vector2D.Zero;
                    Vector2D velocityDirection  = Vector2D.Zero;
                    Vector2D dragDirection = Vector2D.Zero;
                    Vector2D centroid = Vector2D.Zero;
                    
                    
                    Line bodyLine;
                    ShapeHelper.Transform(ref body.Matrices.ToBody, ref line, out bodyLine);


                    GetTangentCallback callback = delegate(Vector2D centTemp)
                    {
                        centroid = body.Matrices.ToWorldNormal * centTemp;
                        PhysicsHelper.GetRelativeVelocity(ref body.State.Velocity, ref centroid, out relativeVelocity);
                        relativeVelocity = FluidVelocity - relativeVelocity;
                        velocityDirection = relativeVelocity.Normalized;
                        dragDirection = body.Matrices.ToBodyNormal * velocityDirection.LeftHandNormal;
                        return dragDirection;
                    };

                    FluidInfo lineInfo = wrapper.affectable.GetFluidInfo(callback, bodyLine);
                    if (lineInfo == null) { continue; }
                  //  Vector2D centTemp = lineInfo.Centroid;
                    Scalar areaTemp = lineInfo.Area;
                  //  Vector2D centroid = body.Matrices.ToWorldNormal * centTemp;
                    Vector2D buoyancyForce = body.State.Acceleration.Linear * areaTemp * -Density;
                    body.ApplyForce(buoyancyForce, centroid);


/*
                    PhysicsHelper.GetRelativeVelocity(ref body.State.Velocity, ref centroid, out relativeVelocity);
                    relativeVelocity =  FluidVelocity-relativeVelocity;
                    velocityDirection = relativeVelocity.Normalized;
                    dragDirection = body.Matrices.ToBodyNormal * velocityDirection.LeftHandNormal;

                    lineInfo = wrapper.affectable.GetFluidInfo(dragDirection, bodyLine);
                    if (lineInfo == null) { continue; }*/

                    Scalar speedSq = relativeVelocity.MagnitudeSq;
                    Scalar dragForceMag = -.5f * Density * speedSq * lineInfo.DragArea * DragCoefficient;
                    Scalar maxDrag = -MathHelper.Sqrt(speedSq) * body.Mass.Mass * step.DtInv;
                    if (dragForceMag < maxDrag)
                    {
                        dragForceMag = maxDrag;
                    }

                    Vector2D dragForce = dragForceMag * velocityDirection;
                    body.ApplyForce(dragForce, body.Matrices.ToWorldNormal * lineInfo.DragCenter);

                    body.ApplyTorque(
                       -body.Mass.MomentOfInertia *
                       (body.Coefficients.DynamicFriction + Density + DragCoefficient) *
                       body.State.Velocity.Angular);
                }
                else
                {

                    Vector2D relativeVelocity = body.State.Velocity.Linear - FluidVelocity;
                    Vector2D velocityDirection = relativeVelocity.Normalized;
                    Vector2D dragDirection = body.Matrices.ToBodyNormal * velocityDirection.LeftHandNormal;

                    Vector2D centroid = wrapper.body.Matrices.ToWorldNormal * wrapper.centroid;
                    Vector2D buoyancyForce = body.State.Acceleration.Linear * wrapper.area * -Density;
                    wrapper.body.ApplyForce(buoyancyForce, centroid);
                    if (velocityDirection == Vector2D.Zero) { continue; }

                    DragInfo dragInfo = wrapper.affectable.GetFluidInfo(dragDirection);
                    if (dragInfo.DragArea < .01f) { continue; }
                    Scalar speedSq = relativeVelocity.MagnitudeSq;
                    Scalar dragForceMag = -.5f * Density * speedSq * dragInfo.DragArea * DragCoefficient;
                    Scalar maxDrag = -MathHelper.Sqrt(speedSq) * body.Mass.Mass * step.DtInv;
                    if (dragForceMag < maxDrag)
                    {
                        dragForceMag = maxDrag;
                    }

                    Vector2D dragForce = dragForceMag * velocityDirection;
                    wrapper.body.ApplyForce(dragForce, body.Matrices.ToWorldNormal * dragInfo.DragCenter);

                    wrapper.body.ApplyTorque(
                       -body.Mass.MomentOfInertia *
                       (body.Coefficients.DynamicFriction + Density + DragCoefficient) *
                       body.State.Velocity.Angular);
                }
            }
        }

        protected internal override void RemoveExpiredBodies()
        {
            items.RemoveAll(IsRemoved);
        }
        protected internal override void AddBodyRange(List<Body> collection)
        {
            int newCapacity = collection.Count + items.Count;
            if (items.Capacity < newCapacity)
            {
                items.Capacity = newCapacity;
            }
            for (int index = 0; index < collection.Count; ++index)
            {
                items.Add(new Wrapper(collection[index]));
            }
        }
        protected internal override void Clear()
        {
            for (int index = 0; index < items.Count; ++index)
            {
                items[index].Dispose();
            }
            items.Clear();
        }
    }

}