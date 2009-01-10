using System;
using System.ServiceModel;
using Multitouch.Contracts;

namespace Multitouch.Service.Logic.ExternalInterfaces
{
	[ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IApplicationInterfaceCallback))]
	public interface IApplicationInterface
	{
		[OperationContract(IsOneWay = true, IsInitiating = true)]
		void CreateSession();
		[OperationContract(IsOneWay = true, IsTerminating = true)]
		void RemoveSession();
        
		[OperationContract(IsOneWay = true)]
		void AddWindowHandleToSession(IntPtr windowHandle);
		[OperationContract(IsOneWay = true)]
		void RemoveWindowHandleFromSession(IntPtr windowHandle);

		[OperationContract(IsOneWay = true)]
		void SendEmptyFrames(bool value);
		[OperationContract]
		bool SendImageType(ImageType imageType, bool value);
	}
}