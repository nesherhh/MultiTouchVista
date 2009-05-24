using System;
using System.Collections.Generic;
using Multitouch.Contracts;

namespace Multitouch.Service.Logic.ExternalInterfaces
{
	class SessionContext : HashSet<IntPtr>, IEquatable<SessionContext>
	{
		public string SessionId { get; private set; }
		public IApplicationInterfaceCallback Callback { get; private set; }

		public SessionContext(string sessionId, IApplicationInterfaceCallback callback)
		{
			SessionId = sessionId;
			Callback = callback;
			ImagesToSend = new Dictionary<ImageType, bool>();
			foreach (ImageType value in Enum.GetValues(typeof(ImageType)))
				ImagesToSend.Add(value, false);
			SendEmptyFrames = false;
		}

		public bool SendEmptyFrames { get; set; }

		public IDictionary<ImageType, bool> ImagesToSend { get; private set; }
		
		public override int GetHashCode()
		{
			return SessionId.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as SessionContext);
		}

		public bool Equals(SessionContext other)
		{
			if(other == null)
				return false;
			return SessionId.Equals(other.SessionId);
		}
	}
}