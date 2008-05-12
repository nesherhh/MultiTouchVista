using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Danilins.Multitouch.Common
{
	[Serializable, StructLayout(LayoutKind.Sequential)]
	public class KeyValuesTripple<TKey, TValue1, TValue2>
	{
		private TKey key;
		private TValue1 value1;
		private TValue2 value2;
		public KeyValuesTripple(TKey key, TValue1 value1, TValue2 value2)
		{
			this.key = key;
			this.value1 = value1;
			this.value2 = value2;
		}

		public TKey Key
		{
			get { return key; }
		}
		
		public TValue1 Value1
		{
			get { return value1; }
		}
	
		public TValue2 Value2
		{
			get { return value2; }
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append('[');
			if (Key != null)
				builder.Append(Key.ToString());

			builder.Append(", ");
			if (Value1 != null)
				builder.Append(Value1.ToString());

			builder.Append(", ");
			if (Value2 != null)
				builder.Append(Value2.ToString());

			builder.Append(']');
			return builder.ToString();
		}
	}
}