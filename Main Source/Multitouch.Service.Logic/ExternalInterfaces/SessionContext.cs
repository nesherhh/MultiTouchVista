using System;
using System.Collections.Generic;
using Multitouch.Contracts;

namespace Multitouch.Service.Logic.ExternalInterfaces
{
	class SessionContext : HashSet<IntPtr>
	{
		public IApplicationInterfaceCallback Callback { get; private set; }

		public SessionContext(IApplicationInterfaceCallback callback)
		{
			Callback = callback;
			ImagesToSend = new Dictionary<ImageType, bool>();
			foreach (ImageType value in Enum.GetValues(typeof(ImageType)))
				ImagesToSend.Add(value, false);
			SendEmptyFrames = false;
		}

		public bool SendEmptyFrames { get; set; }

		public IDictionary<ImageType, bool> ImagesToSend { get; private set; }
	}
}