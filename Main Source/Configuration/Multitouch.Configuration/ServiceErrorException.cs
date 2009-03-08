using System;
using System.Runtime.Serialization;

namespace Multitouch.Configuration
{
	[Serializable]
	public class ServiceErrorException : Exception
	{
		//
		// For guidelines regarding the creation of new exception types, see
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
		// and
		//    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
		//

		public ServiceErrorException()
		{ }

		public ServiceErrorException(string message)
			: base(message)
		{ }

		public ServiceErrorException(string message, Exception inner)
			: base(message, inner)
		{ }

		protected ServiceErrorException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{ }
	}
}