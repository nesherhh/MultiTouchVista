using System;
using System.Runtime.Serialization;

namespace Multitouch.Service
{
	[Serializable]
	public class MultitouchException : Exception
	{
		//
		// For guidelines regarding the creation of new exception types, see
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
		// and
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
		//

		public MultitouchException()
		{
		}

		public MultitouchException(string message) : base(message)
		{
		}

		public MultitouchException(string message, Exception inner) : base(message, inner)
		{
		}

		protected MultitouchException(
			SerializationInfo info,
			StreamingContext context) : base(info, context)
		{
		}
	}
}
