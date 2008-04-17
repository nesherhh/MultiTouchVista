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

using AdvanceMath;
using AdvanceMath.Geometry2D;
using Physics2DDotNet.Shapes;


namespace Physics2DDotNet
{
    /// <summary>
    /// A class used by some Shape Objects for Narrow Phased collision.
    /// </summary>
    [Serializable]
    public sealed class DistanceGrid
    {
        Scalar gridSpacing;
        Scalar gridSpacingInv;
        BoundingRectangle rect;
        Scalar[][] nodes;
        public DistanceGrid(Shape shape, Scalar spacing)
        {
            if (shape == null) { throw new ArgumentNullException("shape"); }
            if (!shape.CanGetDistance) { throw new ArgumentException("The Shape must support Get Distance", "shape"); }
            if (spacing <= 0) { throw new ArgumentOutOfRangeException("spacing"); }
            Matrices matrc = new Matrices();
            shape.CalcBoundingRectangle(matrc, out this.rect);

           // this.rect = shape.Rectangle;
            this.gridSpacing = spacing;
            this.gridSpacingInv = 1 / spacing;
            int xSize = (int)Math.Ceiling((rect.Max.X - rect.Min.X) * gridSpacingInv) + 2;
            int ySize = (int)Math.Ceiling((rect.Max.Y - rect.Min.Y) * gridSpacingInv) + 2;

            this.nodes = new Scalar[xSize][];
            for (int index = 0; index < xSize; ++index)
            {
                this.nodes[index] = new Scalar[ySize];
            }
            Vector2D point;
            point.X = rect.Min.X;
            for (int x = 0; x < xSize; ++x, point.X += gridSpacing)
            {
                point.Y = rect.Min.Y;
                for (int y = 0; y < ySize; ++y, point.Y += gridSpacing)
                {
                    shape.GetDistance(ref point, out nodes[x][y]);
                }
            }
            //restore the shape
           // shape.ApplyMatrix(ref old);
        }
        public bool TryGetIntersection(Vector2D point, out IntersectionInfo result)
        {
            ContainmentType contains;
            rect.Contains(ref point,out contains);
            if (contains == ContainmentType.Contains)
            {
                int x = (int)Math.Floor((point.X - rect.Min.X) * gridSpacingInv);
                int y = (int)Math.Floor((point.Y - rect.Min.Y) * gridSpacingInv);

                Scalar bottomLeft = nodes[x][y];
                Scalar bottomRight = nodes[x + 1][y];
                Scalar topLeft = nodes[x][y + 1];
                Scalar topRight = nodes[x + 1][y + 1];

                if (bottomLeft <= 0 ||
                    bottomRight <= 0 ||
                    topLeft <= 0 ||
                    topRight <= 0)
                {
                    Scalar xPercent = (point.X - (gridSpacing * x + rect.Min.X)) * gridSpacingInv;
                    Scalar yPercent = (point.Y - (gridSpacing * y + rect.Min.Y)) * gridSpacingInv;

                    Scalar top, bottom, distance;

                    MathHelper.Lerp(ref topLeft, ref topRight, ref xPercent, out top);
                    MathHelper.Lerp(ref bottomLeft, ref bottomRight, ref xPercent, out bottom);
                    MathHelper.Lerp(ref bottom, ref top, ref yPercent, out distance);

                    if (distance <= 0)
                    {
                        Scalar right, left;

                        MathHelper.Lerp(ref bottomRight, ref topRight, ref yPercent, out right);
                        MathHelper.Lerp(ref bottomLeft, ref topLeft, ref yPercent, out left);

                        Vector2D normal;
                        normal.X = right - left;
                        normal.Y = top - bottom;
                        Vector2D.Normalize(ref normal, out result.Normal);
                        result.Position = point;
                        result.Distance = distance;
                        return true;
                    }
                }
            }
            result = IntersectionInfo.Zero;
            return false;
        }
    }
}