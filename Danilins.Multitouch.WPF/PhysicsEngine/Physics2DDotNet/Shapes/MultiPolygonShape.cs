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


namespace Physics2DDotNet.Shapes
{
    /// <summary>
    /// A shape that contains multiple polygons.
    /// </summary>
    [Serializable]
    public sealed class MultiPolygonShape : Shape, IRaySegmentsCollidable, ILineFluidAffectable, IExplosionAffectable
    {

        #region static


        public static Vector2D[][] CreateFromBitmap(bool[,] bitmap)
        {
            return CreateFromBitmap(new ArrayBitmap(bitmap));
        }
        public static Vector2D[][] CreateFromBitmap(IBitmap bitmap)
        {
            if (bitmap == null) { throw new ArgumentNullException("bitmap"); }
            if (bitmap.Width < 2 || bitmap.Height < 2) { throw new ArgumentOutOfRangeException("bitmap"); }
            return BitmapHelper.CreateManyFromBitmap(bitmap);
        }

        private static Vector2D[] ConcatVertexes(Vector2D[][] polygons)
        {
            if (polygons == null) { throw new ArgumentNullException("polygons"); }
            if (polygons.Length == 0) { throw new ArgumentOutOfRangeException("polygons"); }
            int totalLength = 0;
            Vector2D[] polygon;
            for (int index = 0; index < polygons.Length; ++index)
            {
                polygon = polygons[index];
                if (polygon == null) { throw new ArgumentNullException("polygons"); }
                if (polygon.Length < 3) { throw new ArgumentException("too few", "polygons"); }
                totalLength += polygon.Length;
            }
            Vector2D[] result = new Vector2D[totalLength];
            int offset = 0;
            for (int index = 0; index < polygons.Length; ++index)
            {
                polygon = polygons[index];
                polygon.CopyTo(result, offset);
                offset += polygon.Length;
            }
            return result;
        }
        /// <summary>
        /// Gets the Inertia of a MultiPartPolygon
        /// </summary>
        /// <param name="polygons"></param>
        /// <returns></returns>
        public static Scalar InertiaOfMultipartPolygon(Vector2D[][] polygons)
        {
            if (polygons == null) { throw new ArgumentNullException("polygons"); }
            if (polygons.Length == 0) { throw new ArgumentOutOfRangeException("polygons"); }
            Scalar denom = 0;
            Scalar numer = 0;
            Scalar a, b, c, d;
            Vector2D v1, v2;
            for (int polyIndex = 0; polyIndex < polygons.Length; ++polyIndex)
            {
                Vector2D[] vertexes = polygons[polyIndex];
                if (vertexes == null) { throw new ArgumentNullException("polygons"); }
                if (vertexes.Length == 0) { throw new ArgumentOutOfRangeException("polygons"); }
                if (vertexes.Length == 1) { break; }
                v1 = vertexes[vertexes.Length - 1];
                for (int index = 0; index < vertexes.Length; index++, v1 = v2)
                {
                    v2 = vertexes[index];
                    Vector2D.Dot(ref v2, ref v2, out a);
                    Vector2D.Dot(ref v2, ref v1, out b);
                    Vector2D.Dot(ref v1, ref v1, out c);
                    Vector2D.ZCross(ref v1, ref v2, out d);
                    d = Math.Abs(d);
                    numer += d;
                    denom += (a + b + c) * d;
                }
            }
            if (numer == 0) { return 1; }
            return denom / (numer * 6);
        }

        public static Scalar GetArea(Vector2D[][] polygons)
        {
            if (polygons == null) { throw new ArgumentNullException("polygons"); }
            if (polygons.Length == 0) { throw new ArgumentOutOfRangeException("polygons"); }
            Scalar result = 0;
            Scalar temp;
            Vector2D[] polygon;
            for (int index = 0; index < polygons.Length; ++index)
            {
                polygon = polygons[index];
                if (polygon == null) { throw new ArgumentNullException("polygons"); }
                BoundingPolygon.GetArea(polygon, out temp);
                result += temp;
            }
            return result;
        }

        public static Vector2D GetCentroid(Vector2D[][] polygons)
        {
            if (polygons == null) { throw new ArgumentNullException("polygons"); }
            if (polygons.Length == 0) { throw new ArgumentOutOfRangeException("polygons"); }


            Scalar temp, area, areaTotal;
            Vector2D v1, v2;
            Vector2D[] vertices;
            areaTotal = 0;
            Vector2D result = Vector2D.Zero;
            for (int index1 = 0; index1 < polygons.Length; ++index1)
            {
                vertices = polygons[index1];
                if (vertices == null) { throw new ArgumentNullException("polygons"); }
                if (vertices.Length < 3) { throw new ArgumentOutOfRangeException("polygons", "There must be at least 3 vertices"); }
                v1 = vertices[vertices.Length - 1];
                area = 0;
                for (int index = 0; index < vertices.Length; ++index, v1 = v2)
                {
                    v2 = vertices[index];
                    Vector2D.ZCross(ref v1, ref v2, out temp);
                    area += temp;
                    result.X += ((v1.X + v2.X) * temp);
                    result.Y += ((v1.Y + v2.Y) * temp);
                }
                areaTotal += Math.Abs(area);
            }
            temp = 1 / (areaTotal * 3);
            result.X *= temp;
            result.Y *= temp;
            return result;
        }

        public static Vector2D[][] MakeCentroidOrigin(Vector2D[][] polygons)
        {
            Vector2D centroid = GetCentroid(polygons);
            Vector2D[][] result = new Vector2D[polygons.Length][];
            for (int index = 0; index < polygons.Length; ++index)
            {
                result[index] = OperationHelper.ArrayRefOp<Vector2D, Vector2D, Vector2D>(polygons[index], ref centroid, Vector2D.Subtract);
            }
            return result;
        }


        public static Vector2D[][] Reduce(Vector2D[][] polygons)
        {
            return Reduce(polygons, 0);
        }
        public static Vector2D[][] Reduce(Vector2D[][] polygons, Scalar areaTolerance)
        {
            Vector2D[][] result = new Vector2D[polygons.Length][];
            for (int index = 0; index < polygons.Length; ++index)
            {
                result[index] = PolygonShape.Reduce(polygons[index], areaTolerance);
            }
            return result;
        }
        public static Vector2D[][] Subdivide(Vector2D[][] polygons, Scalar maxLength)
        {
            return Subdivide(polygons, maxLength, true);
        }
        public static Vector2D[][] Subdivide(Vector2D[][] polygons, Scalar maxLength, bool loop)
        {
            Vector2D[][] result = new Vector2D[polygons.Length][];
            for (int index = 0; index < polygons.Length; ++index)
            {
                result[index] = PolygonShape.Subdivide(polygons[index], maxLength, loop);
            }
            return result;
        }



        #endregion
        #region fields
        private Vector2D[][] polygons;


        private DistanceGrid grid;

        #endregion
        #region constructors
        [CLSCompliant(false)]
        public MultiPolygonShape(Vector2D[][] polygons, Scalar gridSpacing)
            : this(polygons, gridSpacing, InertiaOfMultipartPolygon(polygons)) { }
        public MultiPolygonShape(Vector2D[][] polygons, Scalar gridSpacing, Scalar momentOfInertiaMultiplier)
            : base(ConcatVertexes(polygons), momentOfInertiaMultiplier)
        {
            if (gridSpacing <= 0) { throw new ArgumentOutOfRangeException("gridSpacing"); }
            this.polygons = polygons;
            this.Normals = CalculateNormals();
            this.grid = new DistanceGrid(this, gridSpacing);
        }
        #endregion
        #region properties
        public Vector2D Centroid
        {
            get { return GetCentroid(polygons); }
        }
        public Scalar Area
        {
            get { return GetArea(polygons); }
        }

        public Vector2D[][] Polygons
        {
            get { return polygons; }
        }

        public override bool CanGetIntersection
        {
            get { return true; }
        }
        public override bool CanGetDistance
        {
            get { return true; }
        }
        public override bool BroadPhaseDetectionOnly
        {
            get { return false; }
        }
        public override bool CanGetCustomIntersection
        {
            get { return false; }
        }
        #endregion
        #region methods
        private Vector2D[] CalculateNormals()
        {
            Vector2D[] result = new Vector2D[Vertexes.Length];
            int offset = 0;
            for (int index = 0; index < polygons.Length; ++index)
            {
                Vector2D[] polygon = polygons[index];
                ShapeHelper.CalculateNormals(polygon, result, offset);
                offset += polygon.Length;
            }
            return result;
        }

        public override void CalcBoundingRectangle(Matrices matrices, out BoundingRectangle rectangle)
        {
            BoundingRectangle.FromVectors(ref matrices.ToWorld, Vertexes, out rectangle);
        }

        public override bool TryGetIntersection(Vector2D point, out IntersectionInfo info)
        {
            return grid.TryGetIntersection(point, out info);
        }

        public override void GetDistance(ref Vector2D point, out Scalar result)
        {
            result = Scalar.MaxValue;
            Scalar temp;
            for (int index = 0; index < polygons.Length; ++index)
            {
                BoundingPolygon.GetDistance(polygons[index], ref point, out temp);
                if (temp < result)
                {
                    result = temp;
                }
            }
        }
        public override bool TryGetCustomIntersection(Body self, Body other, out object customIntersectionInfo)
        {
            throw new NotSupportedException();
        }

        bool IRaySegmentsCollidable.TryGetRayCollision(Body thisBody, Body raysBody, RaySegmentsShape raySegments, out RaySegmentIntersectionInfo info)
        {
            bool intersects = false;
            RaySegment[] segments = raySegments.Segments;
            Scalar[] result = new Scalar[segments.Length];
            Scalar temp;
            Vector2D[][] polygons = this.Polygons;
            for (int index = 0; index < segments.Length; ++index)
            {
                result[index] = -1;
            }
            Matrix2x3 matrix = raysBody.Matrices.ToBody* thisBody.Matrices.ToWorld ;
            for (int polyIndex = 0; polyIndex < polygons.Length; ++polyIndex)
            {
                Vector2D[] unTrans = polygons[polyIndex];
                Vector2D[] polygon = new Vector2D[unTrans.Length];
                for (int index = 0; index < unTrans.Length; ++index)
                {
                    Vector2D.Transform(ref matrix, ref unTrans[index], out polygon[index]);
                }
                BoundingRectangle rect;
                BoundingRectangle.FromVectors(polygon, out rect);
                BoundingPolygon poly = new BoundingPolygon(polygon);
                for (int index = 0; index < segments.Length; ++index)
                {
                    RaySegment segment = segments[index];
                    rect.Intersects(ref segment.RayInstance, out temp);
                    if (temp >= 0 && temp <= segment.Length)
                    {
                        poly.Intersects(ref segment.RayInstance, out temp);
                        if (temp >= 0 && temp <= segment.Length)
                        {
                            if (result[index] == -1 || temp < result[index])
                            {
                                result[index] = temp;
                            }
                            intersects = true;
                        }
                    }
                }
            }
            if (intersects)
            {
                info = new RaySegmentIntersectionInfo(result);
            }
            else
            {
                info = null;
            }
            return intersects;
        }
        DragInfo IGlobalFluidAffectable.GetFluidInfo(Vector2D tangent)
        {
            Vector2D dragCenter;
            Scalar dragArea;
            ShapeHelper.GetFluidInfo(polygons, tangent, out dragCenter, out dragArea);
            return new DragInfo(dragCenter, dragArea);
        }
        FluidInfo ILineFluidAffectable.GetFluidInfo(GetTangentCallback callback, Line line)
        {
            if (polygons.Length == 1)
            {
                return ShapeHelper.GetFluidInfo(Vertexes,callback, line);
            }
            List<Vector2D[]> submerged = new List<Vector2D[]>(polygons.Length);
            for (int index = 0; index < polygons.Length; ++index)
            {
                Vector2D[] vertexes = ShapeHelper.GetIntersection(polygons[index], line);
                if (vertexes.Length >= 3) { submerged.Add(vertexes); }
            }
            if (submerged.Count == 0) { return null; }
            Vector2D[][] newPolygons = submerged.ToArray();
            Vector2D centroid = MultiPolygonShape.GetCentroid(newPolygons);
            Scalar area = MultiPolygonShape.GetArea(newPolygons);
            Vector2D tangent = callback(centroid);
            Vector2D dragCenter;
            Scalar dragArea;
            ShapeHelper.GetFluidInfo(newPolygons, tangent, out dragCenter, out dragArea);
            return new FluidInfo(dragCenter, dragArea, centroid, area);
        }

        DragInfo IExplosionAffectable.GetExplosionInfo(Matrix2x3 matrix, Scalar radius, GetTangentCallback callback)
        {
            Vector2D[] vertexes2 = new Vector2D[Vertexes.Length];
            for (int index = 0; index < vertexes2.Length; ++index)
            {
                vertexes2[index] = matrix * Vertexes[index];
            }
            Vector2D[] inter = ShapeHelper.GetIntersection(vertexes2, radius);
            if (inter.Length < 3) { return null; }
            Vector2D centroid = PolygonShape.GetCentroid(inter);
            Vector2D tangent = callback(centroid);
            Scalar min, max;
            ShapeHelper.GetProjectedBounds(inter, tangent, out min, out max);
            Scalar avg = (max + min) / 2;
            return new DragInfo(tangent * avg, max - min);
        }
#endregion
    }
}