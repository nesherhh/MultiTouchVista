using System;
using HIDLibrary;
using System.Collections;
using System.IO;
using System.Collections.Generic;

namespace Multitouch.Driver.Logic
{
	class MultiTouchReport : HidReport
	{
		private readonly bool firstReport;
		private readonly byte actualContactsCount;
		const byte MultiTouch = 1;


		public List<HidContactInfo> Contacts { get; private set; }
		public const int MaxContactsProReport = 2;

		public MultiTouchReport(byte actualContactsCount, bool firstReport)
			: base(13)
		{
			this.actualContactsCount = actualContactsCount;
			this.firstReport = firstReport;
			Contacts = new List<HidContactInfo>(MaxContactsProReport);
			ReportId = MultiTouch;
		}

		public void ToBytes()
		{
			BitArray bits = new BitArray((Data.Length - 1) * 8);
			bits[0] = Contacts[0].TipSwitch;
			bits[1] = Contacts[0].InRange;
			if (Contacts.Count == 2)
			{
				bits[48] = Contacts[1].TipSwitch;
				bits[49] = Contacts[1].InRange;
			}
			bits.CopyTo(Data, 0);
			using (BinaryWriter writer = new BinaryWriter(new MemoryStream(Data)))
			{
				writer.Seek(1, SeekOrigin.Begin);
				writer.Write(Contacts[0].Id);
				writer.Write(Contacts[0].X);
				writer.Write(Contacts[0].Y);
				writer.Seek(1, SeekOrigin.Current);
				if (Contacts.Count == 2)
				{
					writer.Write(Contacts[1].Id);
					writer.Write(Contacts[1].X);
					writer.Write(Contacts[1].Y);
				}
				else
				{
					writer.Write((byte)0);
					writer.Write((ushort)0);
					writer.Write((ushort)0);
				}
				if (firstReport)
					writer.Write(actualContactsCount);
				else
					writer.Write((byte)0);
			}
			Console.WriteLine(this);
		}

		public override string ToString()
		{
			string format = string.Empty;
			for (int i = 0; i < Contacts.Count; i++)
			{
				format += string.Format("Tip: {0}, In: {1}, Id: {2}, X,Y: {3},{4}\r\n", Contacts[i].TipSwitch, Contacts[i].InRange,
										Contacts[i].Id, Contacts[i].X, Contacts[i].Y);
			}
			format += "-----------------------------------";
			return format;
		}
	}
}