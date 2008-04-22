using System;
using System.ServiceModel;

namespace Multitouch.Service.ExternalInterfaces
{
	[ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IApplicationInterfaceCallback))]
	public interface IApplicationInterface
	{
		[OperationContract(IsOneWay = false, IsInitiating = true)]
		void Subscribe();
		[OperationContract(IsOneWay = false, IsTerminating = true)]
		void Unsubscribe();
	}
}
