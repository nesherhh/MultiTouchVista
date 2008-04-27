using System;
using System.Collections;
using System.Collections.Generic;

namespace Multitouch.Framework.Collections
{
	public class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		readonly IDictionary<TKey, TValue> dictionary;

		public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
		{
			if (dictionary == null)
				throw new ArgumentNullException("dictionary");
			this.dictionary = dictionary;
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			return dictionary.Contains(item);
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			dictionary.CopyTo(array, arrayIndex);
		}

		public int Count
		{
			get { return dictionary.Count; }
		}

		public bool IsReadOnly
		{
			get { return true; }
		}

		public bool ContainsKey(TKey key)
		{
			return dictionary.ContainsKey(key);
		}

		public TValue this[TKey key]
		{
			get { return dictionary[key]; }
			set { throw GetException(); }
		}

		public ICollection<TKey> Keys
		{
			get { return dictionary.Keys; }
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return dictionary.TryGetValue(key, out value);
		}

		public ICollection<TValue> Values
		{
			get { return dictionary.Values; }
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return dictionary.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return dictionary.GetEnumerator();
		}

		void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
		{
			throw GetException();
		}

		bool IDictionary<TKey, TValue>.Remove(TKey key)
		{
			throw GetException();
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
		{
			throw GetException();
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Clear()
		{
			throw GetException();
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			throw GetException();
		}

		NotSupportedException GetException()
		{
			return new NotSupportedException("dictionary is read only");
		}
	}
}