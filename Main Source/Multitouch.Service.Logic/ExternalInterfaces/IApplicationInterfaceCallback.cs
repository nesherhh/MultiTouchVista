using System;
using System.ServiceModel;
using System.Windows;
using Multitouch.Contracts;

namespace Multitouch.Service.Logic.ExternalInterfaces
{
	public interface IApplicationInterfaceCallback
	{
		[OperationContract(IsOneWay = true)]
		void ContactChanged(int id, double x, double y, double width, double height, double angle, Rect bounds, ContactState state);

		[OperationContract(IsOneWay = true)]
		void Frame(FrameData data);
	}
}