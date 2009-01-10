using System;
using Multitouch.Framework.Input.Service;

namespace Multitouch.Framework.Input
{
	class MultitouchServiceContactDispatcher : IApplicationInterfaceCallback
	{
		readonly CommunicationLogic logic;

		public MultitouchServiceContactDispatcher(CommunicationLogic logic)
		{
			this.logic = logic;
		}

		public void Frame(FrameData data)
		{
			logic.DispatchFrame(data);
		}
	}
}
