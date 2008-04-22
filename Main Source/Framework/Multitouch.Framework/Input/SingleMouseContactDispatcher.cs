using System;

namespace Multitouch.Framework.Input
{
	class SingleMouseContactDispatcher : IApplicationInterfaceCallback
	{
		public SingleMouseContactDispatcher(MultitouchLogic logic)
		{
		}

		public void ContactChanged(int id, double x, double y, double width, double height, ContactState state)
		{
			throw new NotImplementedException();
		}
	}
}
