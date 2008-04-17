using System;
using System.ServiceModel;
using Danilins.Multitouch.Common;

namespace Danilins.Multitouch.Logic.Service
{
	[ServiceKnownType(typeof(ContactState))]
	public interface IMultitouchServiceCallback
	{
		[OperationContract(IsOneWay = true)]
		void ProcessContact(ContactInfo[] contacts);
	}
}