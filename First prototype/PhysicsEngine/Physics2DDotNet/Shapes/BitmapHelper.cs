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
    public interface IBitmap
    {
        int Width { get;}
        int Height { get;}
        bool this[int x, int y] { get;}
    }
    public sealed class ArrayBitmap : IBitmap
    {
        bool[,] bitmap;
        public ArrayBitmap(bool[,] bitmap)
        {
            if (bitmap == null) { throw new ArgumentNullException("bitmap"); }
            this.bitmap = bitmap;
        }
        public int Width
        {
            get { return bitmap.GetLength(0); }
        }
        public int Height
        {
            get { return bitmap.GetLength(1); }
        }
        public bool this[int x, int y]
        {
            get
            {
                return
                    x >= 0 && x < Width &&
                    y >= 0 && y < Height &&
                    bitmap[x, y];
            }
        }
    }
    class BitMapSkipper
    {
        int xMin;
        int xMax;
        List<int>[] scans;


        public BitMapSkipper(IBitmap bitmap, List<Point2D> points)
        {
            FromVectors(points);
            CreateScans();
            FillScans(points);
            FormatScans(bitmap);
        }

        void FromVectors(List<Point2D> points)
        {
            if (points == null) { throw new ArgumentNullException("vectors"); }
            if (points.Count == 0) { throw new ArgumentOutOfRangeException("points"); }
            xMin = points[0].X;
            xMax = xMin;
            for (int index = 1; index < points.Count; ++index)
            {
                Point2D current = points[index];
                if (current.X > xMax)
                {
                    xMax = current.X;
                }
                else if (current.X < xMin)
                {
                    xMin = current.X;
                }
            }
        }
        void CreateScans()
        {
            scans = new List<int>[(xMax - xMin) + 1];
            for (int index = 0; index < scans.Length; ++index)
            {
                scans[index] = new List<int>();
            }
        }
        void FillScans(List<Point2D> points)
        {
            for (int index = 0; index < points.Count; ++index)
            {
                Point2D point = points[index];
                int scanIndex = point.X - xMin;
                scans[scanIndex].Add(point.Y);
            }
        }

        void FormatScans(IBitmap bitmap)
        {
            for (int index = 0; index < scans.Length; ++index)
            {
                scans[index].Sort();
                FormatScan(bitmap, index);
            }
        }
        void FormatScan(IBitmap bitmap, int x)
        {
            List<int> scan = scans[x];
            List<int> newScan = new List<int>();
            bool inPoly = false;
            for (int index = 0; index < scan.Count; ++index)
            {
                int y = scan[index];
                if (!inPoly)
                {
                    newScan.Add(y);
                }
                bool value = bitmap[x + xMin, y + 1];
                if (value)
                {
                    inPoly = true;
                }
                else
                {
                    newScan.Add(y);
                    inPoly = false;
                }
            }
            scans[x] = newScan;
        }
        public bool TryGetSkip(Point2D point, out int nextY)
        {
            int scanIndex = point.X - xMin;
            if (scanIndex < 0 || scanIndex >= scans.Length)
            {
                nextY = 0;
                return false;
            }
            List<int> scan = scans[scanIndex];
            for (int index = 0; index < scan.Count; index += 2)
            {
                if (point.Y >= scan[index] &&
                    point.Y <= scan[index + 1])
                {
                    nextY = scan[index + 1];
                    return true;
                }
            }
            nextY = 0;
            return false;
        }
    }
    static class BitmapHelper
    {
        static readonly Point2D[] bitmapPoints = new Point2D[]{
            new Point2D (1,1),
            new Point2D (0,1),
            new Point2D (-1,1),
            new Point2D (-1,0),
            new Point2D (-1,-1),
            new Point2D (0,-1),
            new Point2D (1,-1),
            new Point2D (1,0),
        };

        public static Vector2D[] CreateFromBitmap(IBitmap bitmap)
        {
            return Reduce(CreateFromBitmap(bitmap, GetFirst(bitmap)));
        }
        public static Vector2D[][] CreateManyFromBitmap(IBitmap bitmap)
        {
            List<BitMapSkipper> skippers = new List<BitMapSkipper>();
            List<Vector2D[]> result = new List<Vector2D[]>();
            foreach(Point2D first in GetFirsts(bitmap,skippers))
            {
                List<Point2D> points = CreateFromBitmap(bitmap, first);
                BitMapSkipper skipper = new BitMapSkipper(bitmap, points);
                skippers.Add(skipper);
                result.Add(Reduce(points));
            }
            return result.ToArray();
        }
        private static List<Point2D> CreateFromBitmap(IBitmap bitmap, Point2D first)
        {
            Point2D current = first;
            Point2D last = first - new Point2D(0, 1);
            List<Point2D> result = new List<Point2D>();
            do
            {
                result.Add(current);
                current = GetNextVertex(bitmap, current, last);
                last = result[result.Count - 1];
            } while (current != first);
            if (result.Count < 3) { throw new ArgumentException("TODO", "bitmap"); }
            return result;
        }
        private static Point2D GetFirst(IBitmap bitmap)
        {
            for (int x = bitmap.Width - 1; x > -1; --x)
            {
                for (int y = 0; y < bitmap.Height; ++y)
                {
                    if (bitmap[x, y])
                    {
                        return new Point2D(x, y);
                    }
                }
            }
            throw new ArgumentException("TODO", "bitmap");
        }
        private static IEnumerable<Point2D> GetFirsts(IBitmap bitmap, List<BitMapSkipper> skippers)
        {
            for (int x = bitmap.Width - 1; x > -1; --x)
            {
                for (int y = 0; y < bitmap.Height; ++y)
                {
                    if (bitmap[x, y])
                    {
                        bool contains = false;
                        Point2D result = new Point2D(x, y);
                        for (int index = 0; index < skippers.Count; ++index)
                        {
                            int nextY;
                            if (skippers[index].TryGetSkip(result, out nextY))
                            {
                                contains = true;
                                y = nextY;
                                break;
                            }
                        }
                        if (!contains)
                        {
                            yield return result;
                        }
                    }
                }
            }
        }
        private static Point2D GetNextVertex(IBitmap bitmap, Point2D current, Point2D last)
        {
            int offset = 0;
            Point2D point;
            for (int index = 0; index < bitmapPoints.Length; ++index)
            {
                Point2D.Add(ref current, ref bitmapPoints[index], out point);
                if (Point2D.Equals(ref point, ref last))
                {
                    offset = index + 1;
                    break;
                }
            }
            for (int index = 0; index < bitmapPoints.Length; ++index)
            {
                Point2D.Add(
                    ref current,
                    ref bitmapPoints[(index + offset) % bitmapPoints.Length],
                    out point);
                if (point.X >= 0 && point.X < bitmap.Width &&
                    point.Y >= 0 && point.Y < bitmap.Height &&
                    bitmap[point.X, point.Y])
                {
                    return point;
                }
            }
            throw new ArgumentException("TODO", "bitmap");
        }
        private static Vector2D[] Reduce(List<Point2D> list)
        {
            List<Vector2D> result = new List<Vector2D>(list.Count);
            Point2D p1 = list[list.Count - 2];
            Point2D p2 = list[list.Count - 1];
            Point2D p3;
            for (int index = 0; index < list.Count; ++index, p2 = p3)
            {
                if (index == list.Count - 1)
                {
                    if (result.Count == 0) { throw new ArgumentException("Bad Polygon"); }
                    p3.X = (int)result[0].X;
                    p3.Y = (int)result[0].Y;
                }
                else { p3 = list[index]; }
                if (!IsInLine(ref p1, ref p2, ref p3))
                {
                    result.Add(new Vector2D(p2.X, p2.Y));
                    p1 = p2;
                }
            }
            return result.ToArray();
        }
        private static bool IsInLine(ref Point2D p1, ref Point2D p2, ref Point2D p3)
        {
            int slope1 = (p1.Y - p2.Y);
            int slope2 = (p2.Y - p3.Y);
            return 0 == slope1 && 0 == slope2 ||
               ((p1.X - p2.X) / (Scalar)slope1) == ((p2.X - p3.X) / (Scalar)slope2);
        }
    }
}