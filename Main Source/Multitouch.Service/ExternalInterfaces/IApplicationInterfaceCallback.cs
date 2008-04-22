using System;
using System.ServiceModel;
using Multitouch.Contracts;

namespace Multitouch.Service.ExternalInterfaces
{
	public interface IApplicationInterfaceCallback
	{
		[OperationContract(IsOneWay = true)]
		void ContactChanged(int id, double x, double y, double width, double height, ContactState state);
	}
}
