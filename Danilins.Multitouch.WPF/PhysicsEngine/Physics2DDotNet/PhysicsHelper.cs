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


namespace Physics2DDotNet
{


    /// <summary>
    /// contains some methods to do physics calculations.
    /// </summary>
    public static class PhysicsHelper
    {
        public const Scalar GravitationalConstant = 6.67e-11f;


        public static Vector2D GetTangent(Vector2D normal)
        {
            Vector2D result;
            GetTangent(ref normal, out result);
            return result;
        }
        public static void GetTangent(ref Vector2D normal, out Vector2D result)
        {
            Scalar normalX = normal.X;
            result.X = normal.Y;
            result.Y = -normalX;
        }


        public static Scalar GetMassNormal(
            Vector2D point1, Vector2D point2, Vector2D normal,
            Scalar mass1Inv, Scalar inertia1Inv,
            Scalar mass2Inv, Scalar inertia2Inv)
        {
            Scalar result;
            GetMassNormal(
                ref point1, ref point2, ref normal, 
                ref mass1Inv, ref inertia1Inv, 
                ref mass2Inv, ref inertia2Inv, 
                out result);
            return result;
        }
        public static void GetMassNormal(
            ref Vector2D point1, ref Vector2D point2, ref Vector2D normal,
            ref Scalar mass1Inv, ref Scalar inertia1Inv,
            ref Scalar mass2Inv, ref Scalar inertia2Inv,
            out Scalar result)
        {
            Scalar rn1, rn2;
            Vector2D.Dot(ref point1, ref normal, out rn1);
            Vector2D.Dot(ref point2, ref normal, out rn2);
            result =
                1.0f /
                (mass1Inv + mass2Inv +
                inertia1Inv * ((point1.X * point1.X + point1.Y * point1.Y) - rn1 * rn1) +
                inertia2Inv * ((point2.X * point2.X + point2.Y * point2.Y) - rn2 * rn2));
        }

        public static Vector2D GetRelativeVelocity(
            ALVector2D velocity1, ALVector2D velocity2, 
            Vector2D point1, Vector2D point2)
        {
            Vector2D result;
            GetRelativeVelocity(ref velocity1, ref velocity2, ref point1, ref point2, out result);
            return result;
        }

        public static void GetRelativeVelocity(
            ref ALVector2D velocity1, ref ALVector2D velocity2,
            ref Vector2D point1, ref Vector2D point2,
            out Vector2D result)
        {
            result.X =
                (velocity2.Linear.X - velocity2.Angular * point2.Y) -
                (velocity1.Linear.X - velocity1.Angular * point1.Y);
            result.Y =
                (velocity2.Linear.Y + velocity2.Angular * point2.X) -
                (velocity1.Linear.Y + velocity1.Angular * point1.X);
        }
        public static void GetRelativeVelocity(
            ref ALVector2D velocity1, ref Vector2D point1, out Vector2D result)
        {
            result.X = -(velocity1.Linear.X - velocity1.Angular * point1.Y);
            result.Y = -(velocity1.Linear.Y + velocity1.Angular * point1.X);
        }

        public static void AddImpulse(
            ref ALVector2D velocity, ref Vector2D impulse, ref Vector2D point,
            ref Scalar massInv, ref Scalar inertiaInv)
        {
            velocity.Linear.X += impulse.X * massInv;
            velocity.Linear.Y += impulse.Y * massInv;
            velocity.Angular += (inertiaInv * (point.X * impulse.Y - point.Y * impulse.X));
        }

        public static void SubtractImpulse(
            ref ALVector2D velocity, ref Vector2D impulse, ref Vector2D point,
            ref Scalar massInv, ref Scalar inertiaInv)
        {
            velocity.Linear.X -= impulse.X * massInv;
            velocity.Linear.Y -= impulse.Y * massInv;
            velocity.Angular -= (inertiaInv * (point.X * impulse.Y - point.Y * impulse.X));
        }
    }
}
