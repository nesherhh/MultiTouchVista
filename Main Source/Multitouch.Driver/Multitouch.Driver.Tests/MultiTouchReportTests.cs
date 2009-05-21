using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Multitouch.Driver.Logic;
using Multitouch.Framework.Input;
using Multitouch.Framework.Input.Service;

namespace Multitouch.Driver.Tests
{
	[TestClass]
	public class MultiTouchReportTests
	{
		[TestMethod]
		public void OneContactToBytes()
		{
			MultiTouchReport report = new MultiTouchReport(1, true);

			Point point = HidContactInfo.GetPoint(new Point(2,3), IntPtr.Zero);
			report.Contacts.Add(CreateContact(HidContactState.Updated, 1, 2, 3, 4, 5));
			const int expectedPressure = 4 * 5;
			
			report.ToBytes();

			byte[] data = report.Data;
			BinaryReader reader = new BinaryReader(new MemoryStream(data));
			Assert.AreEqual(3, reader.ReadByte()); //tip, inrange
			Assert.AreEqual(0, reader.ReadByte()); //
			Assert.AreEqual(Convert.ToUInt16(point.X), reader.ReadUInt16()); // x
			Assert.AreEqual(Convert.ToUInt16(point.Y), reader.ReadUInt16()); // y
			Assert.AreEqual(expectedPressure, reader.ReadUInt16()); // pressure
			Assert.AreEqual(4, reader.ReadUInt16()); // width
			Assert.AreEqual(5, reader.ReadUInt16()); // height
			Assert.AreEqual(1, reader.ReadUInt16()); // id

			for (int i = 0; i < data.Length - HidContactInfo.HidContactInfoSize - 1; i++)
				Assert.AreEqual(0, reader.ReadByte());

			Assert.AreEqual(1, reader.ReadByte());
		}
		[TestMethod]
		public void TwoContactsToBytes()
		{
			MultiTouchReport report = new MultiTouchReport(2, true);

			Point point = HidContactInfo.GetPoint(new Point(2,3), IntPtr.Zero);
			report.Contacts.Add(CreateContact(HidContactState.Updated, 1, 2, 3, 4, 5));
			report.Contacts.Add(CreateContact(HidContactState.Updated, 2, 2, 3, 4, 5));
			const int expectedPresure = 4 * 5;

			report.ToBytes();

			byte[] data = report.Data;
			BinaryReader reader = new BinaryReader(new MemoryStream(data));
			Assert.AreEqual(3, reader.ReadByte()); //tip, inrange
			Assert.AreEqual(0, reader.ReadByte()); //
			Assert.AreEqual(Convert.ToUInt16(point.X), reader.ReadUInt16()); // x
			Assert.AreEqual(Convert.ToUInt16(point.Y), reader.ReadUInt16()); // y
			Assert.AreEqual(expectedPresure, reader.ReadUInt16()); // pressure
            Assert.AreEqual(4, reader.ReadUInt16()); // width
			Assert.AreEqual(5, reader.ReadUInt16()); // height
			Assert.AreEqual(1, reader.ReadUInt16()); // id

			Assert.AreEqual(3, reader.ReadByte()); //tip, inrange
			Assert.AreEqual(0, reader.ReadByte()); //
			Assert.AreEqual(Convert.ToUInt16(point.X), reader.ReadUInt16()); // x
			Assert.AreEqual(Convert.ToUInt16(point.Y), reader.ReadUInt16()); // y
			Assert.AreEqual(expectedPresure, reader.ReadUInt16()); // pressure
			Assert.AreEqual(4, reader.ReadUInt16()); // width
			Assert.AreEqual(5, reader.ReadUInt16()); // height
			Assert.AreEqual(2, reader.ReadUInt16()); // id

	
			Assert.AreEqual(2, reader.ReadByte());
		}

		private HidContactInfo CreateContact(HidContactState state, int id, int x, int y, int width, int height)
		{
			long timestamp = Stopwatch.GetTimestamp();

			ContactData data = new ContactData();
			data.Id = id;
			data.Position = new Point(x, y);
			data.MajorAxis = width;
			data.MinorAxis = height;
			data.Area = width * height;
			Contact contact = new Contact(data, timestamp);
			HidContactInfo hidContactInfo = new HidContactInfo(state, contact);
			return hidContactInfo;
		}
	}
}