using System;
using System.Collections;
using System.IO;
using System.Windows;
using Multitouch.Framework.Input;

namespace Multitouch.Driver.Logic
{
	class HidContactInfo : IEquatable<HidContactInfo>
	{
		public bool TipSwitch { get; set; }
		public bool InRange { get; set; }
		public ushort X { get; set; }
		public ushort Y { get; set; }
		public ushort Pressure { get; set; }
		public ushort Width { get; set; }
		public ushort Height { get; set; }
		public ushort Id { get; set; }

		public DateTime Timestamp { get; set; }

		/// <summary>
		/// Size of one contact in bytes
		/// </summary>
		public const byte HidContactInfoSize = 14;

		const ushort MAX_SIZE = 32767;
		static readonly double XRatio = SystemParameters.VirtualScreenWidth / MAX_SIZE;
		static readonly double YRatio = SystemParameters.VirtualScreenHeight / MAX_SIZE;

		internal HidContactInfo(bool tipSwitch, bool inRange, Contact contact)
		{
			TipSwitch = tipSwitch;
			InRange = inRange;
			Point point = GetPoint(contact.Position, contact.Hwnd);
			X = Convert.ToUInt16(point.X);
			Y = Convert.ToUInt16(point.Y);
			Width = Convert.ToUInt16(contact.MajorAxis);
			Height = Convert.ToUInt16(contact.MinorAxis);
			Pressure = Convert.ToUInt16(Math.Max(0, Math.Min(MAX_SIZE, contact.Area)));
			Id = Convert.ToUInt16(contact.Id);
			Timestamp = DateTime.Now;
		}

		internal static Point GetPoint(Point position, IntPtr hwnd)
		{
			Point screen = Utility.ClientToScreen(position, hwnd);
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

		internal byte[] ToBytes()
		{
			byte[] buffer = new byte[HidContactInfoSize];
			BitArray bits = new BitArray(new[] { TipSwitch, InRange });
			bits.CopyTo(buffer, 0);
			using (BinaryWriter writer = new BinaryWriter(new MemoryStream(buffer)))
			{
				writer.Seek(2, SeekOrigin.Begin);
				writer.Write(X);
				writer.Write(Y);
				writer.Write(Pressure);
				writer.Write(Width);
				writer.Write(Height);
				writer.Write(Id);
			}
			return buffer;
		}

		public override string ToString()
		{
			return string.Format("Tip: {0}, In: {1}, Id: {2}, X,Y: {3},{4}, W,H: {5},{6}, Pressure: {7}\r\n",
				TipSwitch, InRange, Id, X, Y, Width, Height, Pressure);
		}
	}
}