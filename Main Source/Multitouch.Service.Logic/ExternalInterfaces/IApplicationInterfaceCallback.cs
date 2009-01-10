using System;
using System.ServiceModel;

namespace Multitouch.Service.Logic.ExternalInterfaces
{
	public interface IApplicationInterfaceCallback
	{
		[OperationContract(IsOneWay = true)]
		void Frame(FrameData data);
	}
}