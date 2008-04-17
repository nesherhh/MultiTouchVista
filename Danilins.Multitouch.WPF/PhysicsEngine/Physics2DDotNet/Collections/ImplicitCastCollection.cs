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
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading;
namespace Physics2DDotNet.Collections
{

    public class ImplicitCastCollection<TBase, TParent> : IList<TBase>
        where TParent : TBase
    {
        private struct ImplicitCastEnumerator : IEnumerator<TBase>
        {
            IEnumerator<TParent> self;
            public ImplicitCastEnumerator(IList<TParent> parent)
            {
                this.self = parent.GetEnumerator();
            }
            public TBase Current
            {
                get { return self.Current; }
            }
            object System.Collections.IEnumerator.Current
            {
                get { return self.Current; }
            }
            public void Dispose()
            {
                self.Dispose();
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
        private IList<TParent> self;
        public ImplicitCastCollection(IList<TParent> self)
        {
            this.self = self;
        }
        public int IndexOf(TBase item)
        {
            return self.IndexOf((TParent)item);
        }
        public void Insert(int index, TBase item)
        {
            self.Insert(index, (TParent)item);
        }
        public void RemoveAt(int index)
        {
            self.RemoveAt(index);
        }
        public TBase this[int index]
        {
            get
            {
                return self[index];
            }
            set
            {
                self[index] = (TParent)value;
            }
        }
        public void Add(TBase item)
        {
            self.Add((TParent)item);
        }
        public void Clear()
        {
            self.Clear();
        }
        public bool Contains(TBase item)
        {
            return self.Contains((TParent)item);
        }
        public void CopyTo(TBase[] array, int arrayIndex)
        {
            if (array == null) { throw new ArgumentNullException("array"); }
            if ((arrayIndex < 0)) { throw new ArgumentOutOfRangeException("arrayIndex"); }
            if ((array.Length - arrayIndex) < self.Count) { throw new ArgumentOutOfRangeException("arrayIndex"); }
            for (int index = 0; index < self.Count; ++index, ++arrayIndex)
            {
                array[arrayIndex] = self[index];
            }
        }
        public int Count
        {
            get { return self.Count; }
        }
        public bool IsReadOnly
        {
            get { return self.IsReadOnly; }
        }
        public bool Remove(TBase item)
        {
            return self.Remove((TParent)item);
        }
        public IEnumerator<TBase> GetEnumerator()
        {
            return new ImplicitCastEnumerator(self);
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}