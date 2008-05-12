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



using System;
using System.Collections.Generic;
namespace Physics2DDotNet.Collections
{


    public sealed class ReadOnlyThreadSafeCollection<T> : IList<T>
    {
        sealed class ThreadSafeEnumerator : IEnumerator<T>
        {
            AdvReaderWriterLock.ReaderLock readLock;
            IEnumerator<T> self;

            public ThreadSafeEnumerator(AdvReaderWriterLock.ReaderLock readLock, IEnumerator<T> self)
            {
                if (self == null) { throw new ArgumentNullException("self"); }
                if (readLock == null) { throw new ArgumentNullException("readLock"); }
                this.readLock = readLock;
                this.self = self;
            }
            object System.Collections.IEnumerator.Current
            {
                get { return self.Current; }
            }
            public T Current
            {
                get { return self.Current; }
            }

            public void Dispose()
            {
                self.Dispose();
                readLock.Dispose();
            }
            public bool MoveNext()
            {
                return self.MoveNext();
            }
            public void Reset()
            {
                self.Reset();
            }
        }

        AdvReaderWriterLock rwLock;
        List<T> self;
        public ReadOnlyThreadSafeCollection(AdvReaderWriterLock swLock, List<T> self)
        {
            this.rwLock = swLock;
            this.self = self;
        }

        public int Count
        {
            get
            {
                return self.Count;
            }
        }
        public bool IsReadOnly
        {
            get { return true; }
        }
        public T this[int index]
        {
            get
            {
                using (rwLock.Read)
                {
                    return self[index];
                }
            }
            set
            {
                throw new NotSupportedException();
            }
        }


        public int BinarySearch(T item)
        {
            using (rwLock.Read)
            {
                return self.BinarySearch(item);
            }
        }
        public int BinarySearch(T item, IComparer<T> comparer)
        {
            using (rwLock.Read)
            {
                return self.BinarySearch(item, comparer);
            }
        }
        public int BinarySearch(int index, int count, T item, IComparer<T> comparer)
        {
            using (rwLock.Read)
            {
                return self.BinarySearch(index, count, item, comparer);
            }
        }

        public bool Contains(T item)
        {
            using (rwLock.Read)
            {
                return self.Contains(item);
            }
        }
        public List<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
        {
            using (rwLock.Read)
            {
                return self.ConvertAll<TOutput>(converter);
            }
        }


        public void CopyTo(T[] array)
        {
            using (rwLock.Read)
            {
                self.CopyTo(array);
            }
        }
        public void CopyTo(T[] array, int arrayIndex)
        {
            using (rwLock.Read)
            {
                self.CopyTo(array, arrayIndex);
            }
        }
        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            using (rwLock.Read)
            {
                self.CopyTo(index, array, arrayIndex, count);
            }
        }
        public bool Exists(Predicate<T> match)
        {
            using (rwLock.Read)
            {
                return self.Exists(match);
            }
        }
        public T Find(Predicate<T> match)
        {
            using (rwLock.Read)
            {
                return self.Find(match);
            }
        }
        public List<T> FindAll(Predicate<T> match)
        {
            using (rwLock.Read)
            {
                return self.FindAll(match);
            }
        }
        public int FindIndex(Predicate<T> match)
        {
            using (rwLock.Read)
            {
                return self.FindIndex(match);
            }
        }
        public int FindIndex(int startIndex, Predicate<T> match)
        {
            using (rwLock.Read)
            {
                return self.FindIndex(startIndex, match);
            }
        }
        public int FindIndex(int startIndex, int count, Predicate<T> match)
        {
            using (rwLock.Read)
            {
                return self.FindIndex(startIndex, count, match);
            }
        }
        public T FindLast(Predicate<T> match)
        {
            using (rwLock.Read)
            {
                return self.FindLast(match);
            }
        }
        public int FindLastIndex(Predicate<T> match)
        {
            using (rwLock.Read)
            {
                return self.FindLastIndex(match);
            }
        }
        public int FindLastIndex(int startIndex, Predicate<T> match)
        {
            using (rwLock.Read)
            {
                return self.FindLastIndex(startIndex, match);
            }
        }
        public int FindLastIndex(int startIndex, int count, Predicate<T> match)
        {
            using (rwLock.Read)
            {
                return self.FindLastIndex(startIndex, count, match);
            }
        }
        public List<T> GetRange(int index, int count)
        {
            using (rwLock.Read)
            {
                return self.GetRange(index, count);
            }
        }
        public int IndexOf(T item)
        {
            using (rwLock.Read)
            {
                return self.IndexOf(item);
            }
        }
        public int IndexOf(T item, int index)
        {
            using (rwLock.Read)
            {
                return self.IndexOf(item, index);
            }
        }
        public int IndexOf(T item, int index, int count)
        {
            using (rwLock.Read)
            {
                return self.IndexOf(item, index, count);
            }
        }
        public int LastIndexOf(T item)
        {
            using (rwLock.Read)
            {
                return self.LastIndexOf(item);
            }
        }
        public int LastIndexOf(T item, int index)
        {
            using (rwLock.Read)
            {
                return self.LastIndexOf(item, index);
            }
        }
        public int LastIndexOf(T item, int index, int count)
        {
            using (rwLock.Read)
            {
                return self.LastIndexOf(item, index, count);
            }
        }
        public bool TrueForAll(Predicate<T> match)
        {
            using (rwLock.Read)
            {
                return self.TrueForAll(match);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new ThreadSafeEnumerator(rwLock.Read, self.GetEnumerator());
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }
        void IList<T>.Insert(int index, T item)
        {
            throw new NotSupportedException();
        }
        void IList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }
        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }
        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new ThreadSafeEnumerator(rwLock.Read, self.GetEnumerator());
        }
    }

}