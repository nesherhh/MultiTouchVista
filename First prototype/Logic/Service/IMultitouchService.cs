using System;
using System.ServiceModel;
using Danilins.Multitouch.Common;

namespace Danilins.Multitouch.Logic.Service
{
	[ServiceContract(Namespace = "http://Danilins.Multitouch", SessionMode = SessionMode.Required, CallbackContract = typeof(IMultitouchServiceCallback))]
	[ServiceKnownType(typeof(ContactState))]
	public interface IMultitouchService
	{
		[OperationContract(IsOneWay = true)]
		void RegisterApplication();
		[OperationContract(IsOneWay = true)]
		void UnregisterApplication();
		[OperationContract(IsOneWay = true)]
		void EnableLegacySupport();
		[OperationContract(IsOneWay = true)]
		void DisableLegacySupport();
	}
}