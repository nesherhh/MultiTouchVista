using System;
using System.Windows;
using Multitouch.Framework.Input;

namespace Multitouch.Driver.Logic
{
	class HidContactInfo : IEquatable<HidContactInfo>
	{
		public bool TipSwitch { get; set; }
		public bool InRange { get; set; }
		public byte Id { get; set; }
		public ushort X { get; set; }
		public ushort Y { get; set; }
		public DateTime Timestamp { get; set; }

		const ushort MAX_SIZE = 32767;
		static readonly double XRatio = SystemParameters.VirtualScreenWidth / MAX_SIZE;
		static readonly double YRatio = SystemParameters.VirtualScreenHeight / MAX_SIZE;

		public HidContactInfo(bool tipSwitch, bool inRange, Contact contact)
		{
			TipSwitch = tipSwitch;
			InRange = inRange;
			Id = (byte)contact.Id;
			Point point = GetPoint(contact);
			X = (ushort)point.X;
			Y = (ushort)point.Y;
			Timestamp = DateTime.Now;
		}

		private Point GetPoint(Contact contact)
		{
			Point screen = Utility.ClientToScreen(contact.Position, contact.Hwnd);
			return new Point(Math.Max(0, screen.X / XRatio), Math.Max(0, screen.Y / YRatio));
		}

		public bool Equals(HidContactInfo obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return obj.Id == Id;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(HidContactInfo)) return false;
			return Equals((HidContactInfo)obj);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}
}