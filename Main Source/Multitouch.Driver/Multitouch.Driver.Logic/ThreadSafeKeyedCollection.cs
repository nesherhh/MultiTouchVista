using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

namespace Multitouch.Driver.Logic
{
	abstract class ThreadSafeKeyedCollection<T> : Collection<T>
	{
		readonly ReaderWriterLockSlim lockObject;

		protected ThreadSafeKeyedCollection()
		{
			lockObject = new ReaderWriterLockSlim();
		}

		public ReaderWriterLockSlim LockObject
		{
			get { return lockObject; }
		}

		public new IEnumerator<T> GetEnumerator()
		{
			foreach (T item in Items)
				yield return item;
		}
	}
}