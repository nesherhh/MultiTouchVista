using System;
using Multitouch.Contracts;

namespace Multitouch.Service.Logic
{
	class Receiver
	{
		public Action<IContactData> ContactHandler { get; private set; }
		public Action<IFrameData> FrameHandler { get; set; }

		public Receiver(Action<IContactData> contactHandler)
		{
			ContactHandler = contactHandler;
		}
	}
}