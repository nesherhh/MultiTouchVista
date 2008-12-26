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

		public void ContactChanged(int id, double x, double y, double width, double height, double angle, Rect bounds, Service.ContactState state)
		{
			logic.ProcessChangedContact(id, x, y, width, height, state);
		}

		public void Frame(FrameData data)
		{
			logic.ProcessFrame(data);
		}
	}
}
