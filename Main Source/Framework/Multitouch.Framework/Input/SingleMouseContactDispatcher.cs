using System;

namespace Multitouch.Framework.Input
{
	class SingleMouseContactDispatcher : IApplicationInterfaceCallback
	{
		readonly CommunicationLogic logic;

		public SingleMouseContactDispatcher(CommunicationLogic logic)
		{
			this.logic = logic;
			System.
		}

		public void ContactChanged(int id, double x, double y, double width, double height, ContactState state)
		{
			logic.ProcessChangedContact(id, x, y, width, height, state);
		}
	}
}
