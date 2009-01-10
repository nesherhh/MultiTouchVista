using System;
using System.Runtime.Serialization;
using System.Windows;
using Multitouch.Contracts;

namespace Multitouch.Service.Logic.ExternalInterfaces
{
	[DataContract]
	public class ContactData
	{
		public ContactData(IContactData contactData, IntPtr hwnd)
		{
			Id = contactData.Id;
            Position = Utils.ScreenToClient(contactData.Position, hwnd);
			Bounds = Utils.ScreenToClient(contactData.Bounds, hwnd);
			Orientation = contactData.Orientation;
			State = contactData.State;
			Area = contactData.Area;
			MajorAxis = contactData.MajorAxis;
			MinorAxis = contactData.MinorAxis;
			Hwnd = hwnd;
		}

		[DataMember]
		public int Id { get; private set; }
		[DataMember]
		public Point Position { get; private set; }
		[DataMember]
		public Rect Bounds { get; private set; }
		[DataMember]
		public double Orientation { get; private set; }
		[DataMember]
		public ContactState State { get; private set; }
		[DataMember]
		public double MinorAxis { get; private set; }
		[DataMember]
		public double MajorAxis { get; private set; }
		[DataMember]
		public double Area { get; private set; }
		[DataMember]
		public IntPtr Hwnd { get; private set; }
	}
}
