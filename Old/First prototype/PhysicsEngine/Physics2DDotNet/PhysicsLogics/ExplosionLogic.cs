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

    

    /// <summary>
    /// Simulates a simple explosion.  
    /// </summary>
    public sealed class ExplosionLogic : PhysicsLogic
    {
        sealed class Wrapper
        {
            public Body body;
            public IExplosionAffectable  affectable;
        }

        Scalar dragCoefficient;
        Scalar radius;
        Scalar time;
        Scalar pressurePulseSpeed;
        Body explosionBody;
        List<Wrapper> items;

        /// <summary>
        /// Creates a new instance of the ExplosionLogic
        /// </summary>
        /// <param name="location">ground zero</param>
        /// <param name="velocity">the velocity of the explosion (this would be from the missile or bomb that spawns it).</param>
        /// <param name="pressurePulseSpeed">the speed at which the explosion expands</param>
        /// <param name="dragCoefficient">the drag Coefficient</param>
        /// <param name="mass">the mass of the expanding cloud</param>
        /// <param name="lifetime"></param>
        public ExplosionLogic(
            Vector2D location,
            Vector2D velocity,
            Scalar pressurePulseSpeed,
            Scalar dragCoefficient,
            Scalar mass,
            Lifespan lifetime)
            : base(lifetime)
        {
            this.dragCoefficient = dragCoefficient;
            this.pressurePulseSpeed = pressurePulseSpeed;
            this.explosionBody = new Body(
                new PhysicsState(new ALVector2D(0, location), new ALVector2D(0, velocity)),
                new CircleShape(1, 3),
                mass,
                new Coefficients(1, 1), lifetime);
            this.explosionBody.IgnoresCollisionResponse = true;
            this.explosionBody.Collided += OnCollided;
            this.items = new List<Wrapper>();
        }
        public Body ExplosionBody { get { return explosionBody; } }
        void OnCollided(object sender, CollisionEventArgs e)
        {
            IExplosionAffectable affectable = e.Other.Shape as IExplosionAffectable;
            if (affectable != null)
            {
                Wrapper wrapper = new Wrapper();
                wrapper.body = e.Other;
                wrapper.affectable = affectable;
                items.Add(wrapper);
            }
        }
        public override ReadOnlyCollection<Body> LogicBodies
        {
            get
            {
                return new ReadOnlyCollection<Body>(new Body[] { explosionBody });
            }
        }
        protected internal override void UpdateTime(TimeStep step)
        {
            time += step.Dt;
            radius = 1 + time * pressurePulseSpeed;
            explosionBody.Transformation = Matrix2x3.FromScale(new Vector2D(radius, radius));
            items.Clear();
        }
        protected internal override void RunLogic(TimeStep step)
        {
            Scalar area = MathHelper.Pi * radius * radius;
            Scalar density = explosionBody.Mass.Mass / area;
            BoundingCircle circle = new BoundingCircle(explosionBody.State.Position.Linear, radius);
            Matrix2x3 temp;
            ALVector2D.ToMatrix2x3(ref explosionBody.State.Position, out temp);

            Matrices matrices = new Matrices();
            matrices.Set(ref temp);


            Vector2D relativeVelocity = Vector2D.Zero;
            Vector2D velocityDirection = Vector2D.Zero;
            Vector2D dragDirection = Vector2D.Zero;


            for (int index = 0; index < items.Count; ++index)
            {
                Wrapper wrapper = items[index];
                Body body = wrapper.body;
                Matrix2x3 matrix;
                Matrix2x3.Multiply(ref matrices.ToBody, ref body.Matrices.ToWorld, out matrix);
                ContainmentType containmentType;
                BoundingRectangle rect = body.Rectangle;
                circle.Contains(ref rect, out containmentType);

                if (containmentType == ContainmentType.Intersects)
                {

                    GetTangentCallback callback = delegate(Vector2D centroid)
                    {
                        centroid = body.Matrices.ToWorld * centroid;
                        Vector2D p1 = centroid - explosionBody.State.Position.Linear;
                        Vector2D p2 = centroid - body.State.Position.Linear;

                        PhysicsHelper.GetRelativeVelocity(
                            ref explosionBody.State.Velocity,
                            ref body.State.Velocity,
                            ref p1,
                            ref p2,
                            out relativeVelocity);
                        relativeVelocity = p1.Normalized * this.pressurePulseSpeed;
                        relativeVelocity = -relativeVelocity;
                        velocityDirection = relativeVelocity.Normalized;
                        dragDirection = matrices.ToBodyNormal * velocityDirection.LeftHandNormal;
                        return dragDirection;
                    };

                    DragInfo dragInfo = wrapper.affectable.GetExplosionInfo(matrix, radius, callback);
                    if (dragInfo == null) { continue; }
                    if (velocityDirection == Vector2D.Zero) { continue; }

                    if (dragInfo.DragArea < .01f) { continue; }
                    Scalar speedSq = relativeVelocity.MagnitudeSq;
                    Scalar dragForceMag = -.5f * density * speedSq * dragInfo.DragArea * dragCoefficient;
                    Scalar maxDrag = -MathHelper.Sqrt(speedSq) * body.Mass.Mass * step.DtInv;
                    if (dragForceMag < maxDrag)
                    {
                        dragForceMag = maxDrag;
                    }

                    Vector2D dragForce = dragForceMag * velocityDirection;
                    wrapper.body.ApplyForce(dragForce, (body.Matrices.ToBody * matrices.ToWorld) * dragInfo.DragCenter);
                }
                else if (containmentType == ContainmentType.Contains)
                {
                    Vector2D centroid = body.Matrices.ToWorld * wrapper.affectable.Centroid;

                    Vector2D p1 = centroid - explosionBody.State.Position.Linear;
                    Vector2D p2 = centroid - body.State.Position.Linear;

                    PhysicsHelper.GetRelativeVelocity(
                        ref explosionBody.State.Velocity,
                        ref body.State.Velocity,
                        ref p1,
                        ref p2,
                        out relativeVelocity);
                    relativeVelocity = p1.Normalized * this.pressurePulseSpeed;
                    relativeVelocity = -relativeVelocity;
                    velocityDirection = relativeVelocity.Normalized;
                    dragDirection = matrices.ToBodyNormal * velocityDirection.LeftHandNormal;


                    DragInfo dragInfo = wrapper.affectable.GetFluidInfo(dragDirection);
                    if (dragInfo.DragArea < .01f) { continue; }
                    Scalar speedSq = relativeVelocity.MagnitudeSq;
                    Scalar dragForceMag = -.5f * density * speedSq * dragInfo.DragArea * dragCoefficient;
                    Scalar maxDrag = -MathHelper.Sqrt(speedSq) * body.Mass.Mass * step.DtInv;
                    if (dragForceMag < maxDrag)
                    {
                        dragForceMag = maxDrag;
                    }

                    Vector2D dragForce = dragForceMag * velocityDirection;
                    wrapper.body.ApplyForce(dragForce, body.Matrices.ToWorldNormal * dragInfo.DragCenter);

                    wrapper.body.ApplyTorque(
                       -body.Mass.MomentOfInertia *
                       (body.Coefficients.DynamicFriction + density + dragCoefficient) *
                       body.State.Velocity.Angular);
                }

            }
        }
    }
}