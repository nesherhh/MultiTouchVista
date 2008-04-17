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



// because this code was basically copied from Box2D
// Copyright (c) 2006 Erin Catto http://www.gphysics.com
#if UseDouble
using Scalar = System.Double;
#else
using Scalar = System.Single;
#endif
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;

using AdvanceMath;
using AdvanceMath.Geometry2D;
using Physics2DDotNet.Shapes;
using Physics2DDotNet.Joints;



namespace Physics2DDotNet.Solvers
{

    [Serializable]
    sealed class SequentialImpulsesTag
    {
        public ALVector2D biasVelocity;
        public Body body;
        public SequentialImpulsesTag(Body body)
        {
            this.body = body;
        }
    }
    [Serializable]
    public sealed class SequentialImpulsesSolver : CollisionSolver
    {
        [Serializable]
        sealed class Contact : IContactInfo
        {
            public int id;
            public Vector2D position;
            public Vector2D normal;
            public Scalar distance;
            public Scalar Pn;
            public Scalar Pt;
            public Scalar Pnb;	// accumulated normal impulse for position bias
            public Scalar massNormal;
            public Scalar massTangent;
            public Scalar bias;
            public Vector2D r1;
            public Vector2D r2;
            Arbiter arbiter;
            
            public Contact(Arbiter arbiter)
            {
                this.arbiter = arbiter;
            }

            Vector2D IContactInfo.Position
            {
                get { return position; }
            }
            Vector2D IContactInfo.Normal
            {
                get { return normal; }
            }
            Scalar IContactInfo.Distance
            {
                get { return distance; }
            }
            Body IContactInfo.Body1
            {
                get { return (id < 0) ? (arbiter.body1) : (arbiter.body2); }
            }
            Body IContactInfo.Body2
            {
                get { return (id < 0) ? (arbiter.body2) : (arbiter.body1); }
            }
        }
        [Serializable]
        sealed class Arbiter 
        {
            static Contact[]  Empty = new Contact[0];
            CircleShape circle1;
            CircleShape circle2;
           
            static Scalar ZeroClamp(Scalar value)
            {
                return ((value < 0) ? (0) : (value));
            }

            LinkedList<Contact> contacts;
            Contact[] contactsArray;

            public Body body1;
            public Body body2;
            SequentialImpulsesTag tag1;
            SequentialImpulsesTag tag2;

            SequentialImpulsesSolver parent;
            Scalar restitution;

            Scalar friction;
            bool updated = false;
            public Arbiter(SequentialImpulsesSolver parent, Body body1, Body body2)
            {
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
                this.tag1 = (SequentialImpulsesTag)this.body1.SolverTag;
                this.tag2 = (SequentialImpulsesTag)this.body2.SolverTag;
                this.circle1 = this.body1.Shape as CircleShape;
                this.circle2 = this.body2.Shape as CircleShape;
                this.friction = MathHelper.Sqrt(
                        this.body1.Coefficients.DynamicFriction *
                        this.body2.Coefficients.DynamicFriction);
                this.restitution = Math.Min(body1.Coefficients.Restitution, body2.Coefficients.Restitution);
                this.parent = parent;
                this.contacts = new LinkedList<Contact>();
            }
            public bool Updated
            {
                get { return updated; }
                set { updated = value; }
            }

            public void Update()
            {
                updated = true;

                if (circle1 != null && circle2 != null&&
                    !body1.IsTransformed && !body2.IsTransformed)
                {
                    CollideCircles();
                }
                else
                {
                    Collide();
                }
                UpdateContacts();
            }
            void UpdateContacts()
            {
                if (contacts.Count == 0)
                {
                    contactsArray = Empty;
                    return;
                }
                if (contactsArray == null || contactsArray.Length != contacts.Count)
                {
                    contactsArray = new Contact[contacts.Count];
                }
                contacts.CopyTo(contactsArray, 0);
            }


            void CollideCircles()
            {
                Vector2D center1 = Vector2D.Zero;
                Vector2D center2 = Vector2D.Zero;
                Vector2D.Transform(ref  body1.Matrices.ToWorld, ref center1, out center1);
                Vector2D.Transform(ref  body2.Matrices.ToWorld, ref center2, out center2);
                Vector2D normal;
                Vector2D.Subtract(ref  center2, ref center1, out normal);
                Scalar distance;
                Vector2D.Normalize(ref normal, out distance, out normal);
                Scalar depth = distance - (circle1.Radius + circle2.Radius);
                if (depth > 0)
                {
                    contacts.Clear();
                }
                else
                {
                    Contact contact;
                    if (contacts.First == null)
                    {
                        contact = new Contact(this);
                        contacts.AddLast(contact);
                    }
                    else
                    {
                        contact = contacts.First.Value;
                    }
                    contact.distance = depth;
                    contact.normal = normal;
                    contact.position.X = center2.X - normal.X * circle2.Radius;
                    contact.position.Y = center2.Y - normal.Y * circle2.Radius;
                }
            }
            void CollideCirclesOLD()
            {
                Contact contact;
                if (contacts.First == null)
                {
                    contact = new Contact(this);
                    contacts.AddLast(contact);
                }
                else
                {
                    contact = contacts.First.Value;
                }


                Vector2D normal,p1,p2;
                Scalar distance,r2;

                p1 = Vector2D.Zero;
                p2 = Vector2D.Zero;
                Vector2D.Transform(ref body1.Matrices.ToWorld, ref  p1, out p1);
                Vector2D.Transform(ref body2.Matrices.ToWorld, ref  p2, out p2);
                //p1 = circle1.Position;
                //p2 = circle2.Position;
                r2 = circle2.Radius;
                //diff = circle2.Position - circle1.Position;
                Vector2D.Subtract(ref  p2, ref p1, out normal);
                Vector2D.Normalize(ref normal, out distance, out normal);
                distance -= r2 + circle1.Radius;
                if (distance > 0)
                {
                    contacts.Clear();
                }
                else
                {
                    contact.distance = distance;
                    contact.normal = normal;
                    Vector2D.Multiply(ref r2, ref normal, out normal);
                    Vector2D.Subtract(ref p2, ref normal, out contact.position);
                  //  contact.position = circle2.Position - normal * circle2.Radius;
                }

             /*   Matrix2D inv = circle1.MatrixInv;
                Matrix2D mat = circle1.Matrix;

                Vector2D.Normalize(ref normal, out normal);

                Vector2D.Transform(ref inv.NormalMatrix, ref normal, out normal);
                Vector2D.Normalize(ref normal);
                normal = normal * circle1.Radius;
                Vector2D.Transform(ref mat.VertexMatrix, ref normal, out normal);
                IntersectionInfo info;
                circle2.TryGetIntersection(normal, out info);
                contact.distance = info.Distance;
                Vector2D.Negate(ref info.Normal, out contact.normal);
                contact.position = info.Position;*/
            }
            void Collide()
            {
                BoundingRectangle bb1 = body1.Rectangle;
                BoundingRectangle bb2 = body2.Rectangle;
                BoundingRectangle targetArea;
                BoundingRectangle.FromIntersection(ref bb1, ref bb2, out targetArea);

                LinkedListNode<Contact> node = contacts.First;
                if (!body2.Shape.IgnoreVertexes &&
                    body1.Shape.CanGetIntersection)
                {
                    Collide(ref node, this.body1, this.body2, false, ref targetArea);
                }
                if (!body1.Shape.IgnoreVertexes &&
                    body2.Shape.CanGetIntersection)
                {
                    Collide(ref node, this.body2, this.body1, true, ref targetArea);
                }
            }
            void Collide(ref LinkedListNode<Contact> node, Body b1, Body b2, bool inverse, ref BoundingRectangle targetArea)
            {
                Vector2D[] vertexes = b2.Shape.Vertexes;
                Vector2D[] normals = b2.Shape.Normals;


                Matrix2x3 b2ToWorld = b2.Matrices.ToWorld;
                Matrix2x3 b1ToBody = b1.Matrices.ToBody;
                Matrix2x2 b1ToWorldNormal = b1.Matrices.ToWorldNormal;

                Matrix2x2 normalM;
                Matrix2x2.Multiply(ref b1.Matrices.ToBodyNormal, ref b2.Matrices.ToWorldNormal, out normalM);

                IntersectionInfo info = IntersectionInfo.Zero;
                ContainmentType contains;
                Contact contact;

                for (int index = 0; index < vertexes.Length; ++index)
                {
                    Vector2D worldVertex;
                    Vector2D.Transform(ref b2ToWorld, ref vertexes[index], out worldVertex);
                    targetArea.Contains(ref worldVertex, out contains);
                    bool isBad = (contains != ContainmentType.Contains);
                    if (!isBad)
                    {
                        Vector2D bodyVertex;
                        Vector2D.Transform(ref b1ToBody, ref worldVertex, out bodyVertex);
                        isBad = !b1.Shape.TryGetIntersection(bodyVertex, out info);
                        if (!isBad && normals != null)
                        {
                            Vector2D normal;
                            Vector2D.Transform(ref normalM, ref  normals[index], out normal);
                            Scalar temp;
                            Vector2D.Dot(ref info.Normal, ref normal, out temp);
                            isBad = temp > 0;
                        }
                    }

                    int Id = (inverse) ? (index) : ((-vertexes.Length + index));
                    while (node != null && node.Value.id < Id) { node = node.Next; }

                    if (isBad)
                    {
                        if (node != null && node.Value.id == Id)
                        {
                            LinkedListNode<Contact> nextNode = node.Next;
                            contacts.Remove(node);
                            node = nextNode;
                        }
                    }
                    else
                    {
                        if (node == null)
                        {
                            contact = new Contact(this);
                            contact.id = Id;
                            contacts.AddLast(contact);
                        }
                        else if (node.Value.id == Id)
                        {
                            contact = node.Value;
                            node = node.Next;
                            if (!parent.warmStarting)
                            {
                                contact.Pn = 0;
                                contact.Pt = 0;
                                contact.Pnb = 0;
                            }
                        }
                        else
                        {
                            contact = new Contact(this);
                            contact.id = Id;
                            contacts.AddBefore(node, contact);
                        }
                        Vector2D.Transform(ref b1ToWorldNormal, ref info.Normal, out contact.normal);
                        contact.distance = info.Distance;
                        contact.position = worldVertex;
                        if (inverse)
                        {
                            Vector2D.Negate(ref contact.normal, out contact.normal);
                        }
                        Vector2D.Normalize(ref contact.normal, out contact.normal);
                    }
                }
            }
            public void PreApply(Scalar dtInv)
            {

                Scalar mass1Inv = body1.Mass.MassInv;
                Scalar I1Inv = body1.Mass.MomentOfInertiaInv;
                Scalar mass2Inv = body2.Mass.MassInv;
                Scalar I2Inv = body2.Mass.MomentOfInertiaInv;

                for (int index = 0; index < contactsArray.Length; ++index)
                {
                    Contact c = contactsArray[index];
                    Vector2D.Subtract(ref c.position, ref body1.State.Position.Linear, out c.r1);
                    Vector2D.Subtract(ref c.position, ref body2.State.Position.Linear, out c.r2);

                    // Precompute normal mass, tangent mass, and bias.
                    PhysicsHelper.GetMassNormal(
                        ref c.r1, ref c.r2,
                        ref c.normal,
                        ref mass1Inv, ref I1Inv,
                        ref mass2Inv, ref I2Inv,
                        out c.massNormal);

                    Vector2D tangent;
                    PhysicsHelper.GetTangent(ref c.normal, out tangent);

                    PhysicsHelper.GetMassNormal(
                        ref c.r1, ref c.r2,
                        ref tangent,
                        ref mass1Inv, ref I1Inv,
                        ref mass2Inv, ref I2Inv,
                        out c.massTangent);

                    if (parent.positionCorrection)
                    {
                        c.bias = -parent.biasFactor * dtInv * Math.Min(0.0f, c.distance + parent.allowedPenetration);
                    }
                    else
                    {
                        c.bias = 0;
                    }
                    if (parent.accumulateImpulses)
                    {
                        // Apply normal + friction impulse
                        Vector2D vect1, vect2, P;

                        Scalar temp = (1+this.restitution) * c.Pn;
                        Vector2D.Multiply(ref c.normal, ref temp, out vect1);
                        Vector2D.Multiply(ref tangent, ref c.Pt, out vect2);
                        Vector2D.Add(ref vect1, ref vect2, out P);

                        PhysicsHelper.SubtractImpulse(
                            ref body1.State.Velocity,
                            ref P,
                            ref c.r1,
                            ref mass1Inv,
                            ref I1Inv);

                        PhysicsHelper.AddImpulse(
                            ref body2.State.Velocity,
                            ref P,
                            ref c.r2,
                            ref mass2Inv,
                            ref I2Inv);
                    }
                    // Initialize bias impulse to zero.
                    c.Pnb = 0;
                }
                    body1.ApplyProxy();
                    body2.ApplyProxy();
            }
            public void Apply()
            {
                Body b1 = body1;
                Body b2 = body2;

                Scalar mass1Inv = b1.Mass.MassInv;
                Scalar I1Inv = b1.Mass.MomentOfInertiaInv;
                Scalar mass2Inv = b2.Mass.MassInv;
                Scalar I2Inv = b2.Mass.MomentOfInertiaInv;

                PhysicsState state1 = b1.State;
                PhysicsState state2 = b2.State;

                for (int index = 0; index < contactsArray.Length; ++index)
                {
                    Contact c = contactsArray[index];

                    // Relative velocity at contact
                    Vector2D dv;
                    PhysicsHelper.GetRelativeVelocity(
                        ref state1.Velocity,
                        ref state2.Velocity,
                        ref c.r1, ref c.r2, out dv);

                    // Compute normal impulse
                    Scalar vn;
                    Vector2D.Dot(ref dv, ref c.normal, out vn);
                    //Scalar vn = Vector2D.Dot(dv, c.normal);

                    Scalar dPn;
                    if (parent.splitImpulse)
                    {
                        dPn = c.massNormal * (-vn);
                    }
                    else
                    {
                        dPn = c.massNormal * (c.bias - vn);
                    }


                    if (parent.accumulateImpulses)
                    {
                        // Clamp the accumulated impulse
                        Scalar Pn0 = c.Pn;
                        c.Pn = ZeroClamp(Pn0 + dPn);
                        //c.Pn = Math.Max(Pn0 + dPn, 0.0f);
                        dPn = c.Pn - Pn0;
                    }
                    else
                    {
                        //dPn = Math.Max(dPn, 0.0f);
                        dPn = ZeroClamp(dPn);
                    }

                    // Apply contact impulse
                    Vector2D Pn;
                    Vector2D.Multiply(ref  c.normal, ref dPn, out Pn);
                    //Vector2D Pn = dPn * c.normal;

                    PhysicsHelper.SubtractImpulse(
                        ref state1.Velocity,
                        ref Pn,
                        ref c.r1,
                        ref mass1Inv,
                        ref I1Inv);

                    PhysicsHelper.AddImpulse(
                        ref state2.Velocity,
                        ref Pn,
                        ref c.r2,
                        ref mass2Inv,
                        ref I2Inv);


                    if (parent.splitImpulse)
                    {
                        // Compute bias impulse
                        PhysicsHelper.GetRelativeVelocity(
                            ref tag1.biasVelocity,
                            ref tag2.biasVelocity,
                            ref c.r1, ref c.r2, out dv);



                        Scalar vnb;
                        Vector2D.Dot(ref dv, ref c.normal, out vnb);
                        //Scalar vnb = Vector2D.Dot(dv, c.normal);

                        Scalar dPnb = c.massNormal * (c.bias - vnb);
                        Scalar Pnb0 = c.Pnb;
                        c.Pnb = ZeroClamp(Pnb0 + dPnb);
                        // c.Pnb = Math.Max(Pnb0 + dPnb, 0.0f);
                        dPnb = c.Pnb - Pnb0;

                        Vector2D Pb;
                        Vector2D.Multiply(ref dPnb, ref c.normal, out Pb);
                        //Vector2D Pb = dPnb * c.normal;


                        PhysicsHelper.SubtractImpulse(
                            ref tag1.biasVelocity,
                            ref Pb,
                            ref c.r1,
                            ref mass1Inv,
                            ref I1Inv);

                        PhysicsHelper.AddImpulse(
                            ref tag2.biasVelocity,
                            ref Pb,
                            ref c.r2,
                            ref mass2Inv,
                            ref I2Inv);
                    }

                    // Relative velocity at contact

                    PhysicsHelper.GetRelativeVelocity(
                        ref state1.Velocity,
                        ref state2.Velocity,
                        ref c.r1, ref c.r2, out dv);


                    Vector2D tangent;
                    PhysicsHelper.GetTangent(ref c.normal, out tangent);

                    Scalar vt;
                    Vector2D.Dot(ref dv, ref tangent, out vt);
                    //Scalar vt = Vector2D.Dot(dv, tangent);
                    Scalar dPt = c.massTangent * (-vt);




                    if (parent.accumulateImpulses)
                    {
                        // Compute friction impulse
                        Scalar maxPt = friction * c.Pn;
                        // Clamp friction
                        Scalar oldTangentImpulse = c.Pt;
                        c.Pt = MathHelper.Clamp(oldTangentImpulse + dPt, -maxPt, maxPt);
                        dPt = c.Pt - oldTangentImpulse;
                    }
                    else
                    {
                        // Compute friction impulse
                        Scalar maxPt = friction * dPn;
                        dPt = MathHelper.Clamp(dPt, -maxPt, maxPt);
                    }


                    // Apply contact impulse
                    Vector2D Pt;
                    Vector2D.Multiply(ref tangent, ref dPt, out Pt);

                    //Vector2D Pt = dPt * tangent;

                    PhysicsHelper.SubtractImpulse(
                        ref state1.Velocity,
                        ref Pt,
                        ref c.r1,
                        ref mass1Inv,
                        ref I1Inv);

                    PhysicsHelper.AddImpulse(
                        ref state2.Velocity,
                        ref Pt,
                        ref c.r2,
                        ref mass2Inv,
                        ref I2Inv);
                }
                body1.ApplyProxy();
                body2.ApplyProxy();
            }
            public bool Collided
            {
                get { return contactsArray.Length > 0; }
            }
            public ReadOnlyCollection<IContactInfo> Contacts
            {
                get
                {
                    return new ReadOnlyCollection<IContactInfo>(
                        new Physics2DDotNet.Collections.ImplicitCastCollection<IContactInfo, Contact>(contactsArray));
                }
            }
        }
        static bool IsJointRemoved(ISequentialImpulsesJoint joint)
        {
            return !joint.IsAdded;
        }
        static bool IsTagRemoved(SequentialImpulsesTag tag)
        {
            return !tag.body.IsAdded;
        }


        Dictionary<long, Arbiter> arbiters;
        List<ISequentialImpulsesJoint> siJoints;
        List<SequentialImpulsesTag> tags;
        bool splitImpulse = true;
        bool accumulateImpulses = true;
        bool warmStarting = true;
        bool positionCorrection = true;

        Scalar biasFactor = 0.7f;
        Scalar allowedPenetration = 0.1f;
        int iterations = 12;

        public SequentialImpulsesSolver()
        {
            arbiters = new Dictionary<long, Arbiter>();
            siJoints = new List<ISequentialImpulsesJoint>();
            tags = new List<SequentialImpulsesTag>();
        }

        public bool PositionCorrection
        {
            get { return positionCorrection; }
            set { positionCorrection = value; }
        }
        public bool AccumulateImpulses
        {
            get { return accumulateImpulses; }
            set { accumulateImpulses = value; }
        }
        public bool SplitImpulse
        {
            get { return splitImpulse; }
            set { splitImpulse = value; }
        }
        public bool WarmStarting
        {
            get { return warmStarting; }
            set { warmStarting = value; }
        }
        public Scalar BiasFactor
        {
            get { return biasFactor; }
            set { biasFactor = value; }
        }
        public Scalar AllowedPenetration
        {
            get { return allowedPenetration; }
            set { allowedPenetration = value; }
        }
        public int Iterations
        {
            get { return iterations; }
            set { iterations = value; }
        }

        protected internal override bool TryGetIntersection(TimeStep step, Body first, Body second, out ReadOnlyCollection<IContactInfo> contacts)
        {
            long id = PairID.GetId(first.ID, second.ID);
            Arbiter arbiter;
            if (arbiters.TryGetValue(id, out arbiter))
            {
                arbiter.Update();
                if (!arbiter.Collided)
                {
                    arbiters.Remove(id);
                }
            }
            else
            {
                arbiter = new Arbiter(this, first, second);
                arbiter.Update();
                if (!first.IgnoresCollisionResponse &&
                    !second.IgnoresCollisionResponse &&
                    arbiter.Collided)
                {
                    arbiters.Add(id, arbiter);
                }
            }
            contacts = arbiter.Contacts;
            return arbiter.Collided;
        }
        void RemoveEmpty()
        {
            List<long> empty = new List<long>();
            foreach (KeyValuePair<long, Arbiter> pair in arbiters)
            {
                Arbiter value = pair.Value;
                if (!value.Collided || !value.Updated)
                {
                    empty.Add(pair.Key);
                }
            }
            for (int index = 0; index < empty.Count; ++index)
            {
                arbiters.Remove(empty[index]);
            }
        }

        protected internal override void Solve(TimeStep step)
        {
            foreach (Arbiter arb in arbiters.Values)
            {
                arb.Updated = false;
            }
            Detect(step);
            RemoveEmpty();
            this.Engine.RunLogic(step);
            for (int index = 0; index < tags.Count; ++index)
            {
                SequentialImpulsesTag tag = tags[index];
                tag.biasVelocity = ALVector2D.Zero;
                tag.body.UpdateVelocity(step);
                tag.body.ClearForces();
            }

            Arbiter[] arbs = new Arbiter[arbiters.Count];
            arbiters.Values.CopyTo(arbs, 0);
            for (int index = 0; index < arbs.Length; ++index)
            {
                arbs[index].PreApply(step.DtInv);
            }
            for (int index = 0; index < siJoints.Count; ++index)
            {
                siJoints[index].PreStep(step);
            }
            for (int i = 0; i < iterations; ++i)
            {
                for (int index = 0; index < arbs.Length; ++index)
                {
                    arbs[index].Apply();
                }
                for (int index = 0; index < siJoints.Count; ++index)
                {
                    siJoints[index].ApplyImpulse();
                }
            }
            for (int index = 0; index < tags.Count; ++index)
            {
                SequentialImpulsesTag tag = tags[index];
                if (splitImpulse)
                {
                    tag.body.UpdatePosition(step, ref tag.biasVelocity);
                }
                else
                {
                    tag.body.UpdatePosition(step);
                }
            }
        }
        protected internal override void AddBodyRange(List<Body> collection)
        {
            foreach (Body item in collection)
            {
                if (item.SolverTag == null)
                {
                    SequentialImpulsesTag tag = new SequentialImpulsesTag(item);
                    SetTag(item, tag);
                    tags.Add(tag);
                }
                else
                {
                    tags.Add((SequentialImpulsesTag)item.SolverTag);
                }
            }
        }
        protected internal override void AddJointRange(List<Joint> collection)
        {
            ISequentialImpulsesJoint[] newJoints = new ISequentialImpulsesJoint[collection.Count];
            for (int index = 0; index < newJoints.Length; ++index)
            {
                newJoints[index] = (ISequentialImpulsesJoint)collection[index];
            }
            siJoints.AddRange(newJoints);
        }
        protected internal override void Clear()
        {
            arbiters.Clear();
            siJoints.Clear();
            tags.Clear();
        }
        protected internal override void RemoveExpiredJoints()
        {
            siJoints.RemoveAll(IsJointRemoved);
        }
        protected internal override void RemoveExpiredBodies()
        {
            tags.RemoveAll(IsTagRemoved);
        }
        protected internal override void CheckJoint(Joint joint)
        {
            if (!(joint is ISequentialImpulsesJoint))
            {
                throw new ArgumentException("The joint must implement ISequentialImpulsesJoint to be added to this solver.");
            }
        }
    }
}