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
using System.Threading;
namespace Physics2DDotNet.Collections
{
    public sealed class AdvReaderWriterLock
    {
        public sealed class WriterLock : IDisposable
        {
            AdvReaderWriterLock rwLock;
            public WriterLock(AdvReaderWriterLock rwLock)
            {
                this.rwLock = rwLock;
                rwLock.EnterWrite();
            }
            public void Dispose()
            {
                rwLock.ExitWrite();
            }
        }
        public sealed class ReaderLock : IDisposable
        {
            AdvReaderWriterLock rwLock;
            public ReaderLock(AdvReaderWriterLock rwLock)
            {
                this.rwLock = rwLock;
                rwLock.EnterRead();
            }
            public void Dispose()
            {
                rwLock.ExitRead();
            }
        }

#if !CompactFramework && !WindowsCE && !PocketPC && !XBOX360
        ReaderWriterLock rwLock = new ReaderWriterLock();
#else
        object syncRoot = new object();
#endif
        public WriterLock Write
        {
            get
            {
                return new WriterLock(this);
            }
        }
        public ReaderLock Read
        {
            get
            {
                return new ReaderLock(this);
            }
        }

        public void EnterRead()
        {
#if !CompactFramework && !WindowsCE && !PocketPC && !XBOX360
            rwLock.AcquireReaderLock(Timeout.Infinite);
#else
            Monitor.Enter(syncRoot);
#endif
        }
        public void EnterWrite()
        {
#if !CompactFramework && !WindowsCE && !PocketPC && !XBOX360
            rwLock.AcquireWriterLock(Timeout.Infinite);
#else
            Monitor.Enter(syncRoot);
#endif
        }
        public void ExitRead()
        {
#if !CompactFramework && !WindowsCE && !PocketPC && !XBOX360
            rwLock.ReleaseReaderLock();
#else
            Monitor.Exit(syncRoot);
#endif
        }
        public void ExitWrite()
        {
#if !CompactFramework && !WindowsCE && !PocketPC && !XBOX360
            rwLock.ReleaseWriterLock();
#else
            Monitor.Exit(syncRoot);
#endif
        }
    }

}