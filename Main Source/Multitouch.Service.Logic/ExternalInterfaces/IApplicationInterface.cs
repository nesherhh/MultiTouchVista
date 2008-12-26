using System;
using System.ServiceModel;
using Multitouch.Contracts;

namespace Multitouch.Service.Logic.ExternalInterfaces
{
	[ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IApplicationInterfaceCallback))]
	public interface IApplicationInterface
	{
		[OperationContract(IsOneWay = false, IsInitiating = true)]
		void Subscribe(IntPtr windowHandle);
		[OperationContract(IsOneWay = false, IsTerminating = true)]
		void Unsubscribe();

		[OperationContract(IsOneWay = true)]
		void ReceiveFrames(bool value);
		[OperationContract]
		bool SendImageType(ImageType imageType, bool value);
	}
}