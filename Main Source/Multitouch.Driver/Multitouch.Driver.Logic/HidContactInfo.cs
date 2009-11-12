using System;
using System.Collections;
using System.IO;
using System.Windows;
using Multitouch.Framework.Input;

namespace Multitouch.Driver.Logic
{
	class HidContactInfo : IEquatable<HidContactInfo>
	{
		public HidContactState State { get; private set; }
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

		const ushort MaxSize = 32767;
		static readonly double XRatio = SystemParameters.VirtualScreenWidth / MaxSize;
		static readonly double YRatio = SystemParameters.VirtualScreenHeight / MaxSize;

		public bool TipSwitch
		{
			get
			{
				switch (State)
				{
					case HidContactState.Adding:
					case HidContactState.Removing:
					case HidContactState.Removed:
						return false;
					case HidContactState.Updated:
						return true;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

		public bool InRange
		{
			get
			{
				switch (State)
				{
					case HidContactState.Adding:
					case HidContactState.Removing:
					case HidContactState.Updated:
						return true;
					case HidContactState.Removed:
						return false;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

		internal HidContactInfo(HidContactState state, Contact contact)
		{
			State = state;
			if (contact != null)
			{
				Point point = GetPoint(contact.Position);
				X = Convert.ToUInt16(point.X);
				Y = Convert.ToUInt16(point.Y);
				Width = Convert.ToUInt16(contact.MajorAxis);
				Height = Convert.ToUInt16(contact.MinorAxis);
				Pressure = Convert.ToUInt16(Math.Max(0, Math.Min(MaxSize, contact.Area)));

				UInt16 id = unchecked((UInt16)contact.Id);

				Id = id;
				Timestamp = DateTime.Now;
				Contact = contact;
			}
		}

		public Contact Contact { get; set; }

		internal static Point GetPoint(Point position)
		{
			return new Point(Math.Max(0, position.X / XRatio), Math.Max(0, position.Y / YRatio));
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
			return string.Format("Id: {0}, State: {1}, TipSwitch: {2}, InRange: {3}, X,Y: {4},{5}, W,H: {6},{7}, Pressure: {8}, TimeStamp: {9}",
				Id, State, TipSwitch, InRange, X, Y, Width, Height, Pressure, Timestamp.Ticks);
		}
	}
}