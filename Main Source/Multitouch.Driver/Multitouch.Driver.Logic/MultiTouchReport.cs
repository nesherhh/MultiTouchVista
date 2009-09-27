using System;
using System.Diagnostics;
using System.Text;
using HIDLibrary;
using System.IO;

namespace Multitouch.Driver.Logic
{
	class MultiTouchReport : HidReport
	{
		private readonly bool firstReport;
		private readonly byte actualContactsCount;

		const byte ReportIdMultiTouch = 1;

		/// <summary>
		/// Size of one report in bytes
		/// </summary>
		private const int ReportLength = MaxContactsPerReport * (HidContactInfo.HidContactInfoSize) + 1;

		public ReportContacts Contacts { get; private set; }
		public const int MaxContactsPerReport = 2;

		public MultiTouchReport(byte actualContactsCount, bool firstReport)
			: base(ReportLength)
		{
			this.actualContactsCount = actualContactsCount;
			this.firstReport = firstReport;
			Contacts = new ReportContacts();
			ReportId = ReportIdMultiTouch;
		}

		public void ToBytes()
		{
			using (BinaryWriter writer = new BinaryWriter(new MemoryStream(Data)))
			{
				foreach (HidContactInfo contactInfo in Contacts)
				{
					byte[] buffer = contactInfo.ToBytes();
					writer.Write(buffer);
				}
				int contactsLeft = MaxContactsPerReport - Contacts.Count;
				if (contactsLeft > 0)
				{
					byte[] buffer = new byte[(HidContactInfo.HidContactInfoSize) * contactsLeft];
					writer.Write(buffer);
				}
				if (firstReport)
					writer.Write(actualContactsCount);
				else
					writer.Write((byte)0);
			}
			//Trace.WriteLine(ToString());
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			foreach (HidContactInfo contactInfo in Contacts)
				sb.AppendLine(contactInfo.ToString());
			sb.AppendLine("-----------------------------------");
			return sb.ToString();
		}
	}
}