using System;

namespace Danilins.Multitouch.Common
{
	[Serializable]
	public class MultitouchException : Exception
	{
		public MultitouchException() { }
		public MultitouchException(string message) : base(message) { }
		public MultitouchException(string message, Exception inner) : base(message, inner) { }
		protected MultitouchException(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context)
			: base(info, context) { }
	}
}