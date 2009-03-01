using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using HIDLibrary;

namespace Multitouch.Driver.Logic
{
	class DriverCommunicator : IDisposable
	{
		private const int ContactTimeout = 3;

		readonly ContactCollection contacts;

		readonly Timer timer;
		private readonly HidDevice device;

		public DriverCommunicator()
		{
			device = HidDevices.Enumerate(0xdddd, 0x0001).FirstOrDefault();
			if (device == null)
				throw new InvalidOperationException("Universal Software HID driver was not found. Please ensure that it is installed.");

			device.OpenDevice(HidDevice.DeviceMode.Overlapped, HidDevice.DeviceMode.NonOverlapped);

			contacts = new ContactCollection();

			timer = new Timer();
			timer.AutoReset = false;
			timer.Interval = 50d / 1000;
			timer.Elapsed += timer_Elapsed;
			timer.Start();
		}

		void timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			SendContacts();
			timer.Start();
		}

		public void SendContacts()
		{
			bool firstReport = true;
			int index = 0;

			DateTime now = DateTime.Now;

			contacts.LockObject.EnterWriteLock();
			try
			{
				List<HidContactInfo> tmpList = new List<HidContactInfo>(contacts);
				MultiTouchReport report = new MultiTouchReport((byte)tmpList.Count, firstReport);
				foreach (HidContactInfo info in tmpList)
				{
					TimeSpan span = now - info.Timestamp;
					if (span.Seconds > ContactTimeout)
						info.InRange = info.TipSwitch = false;

					report.Contacts.Add(info);
					index++;

					if (tmpList.Count - index == 0)
						SendReport(report);
					else if (index % MultiTouchReport.MaxContactsProReport == 0)
					{
						SendReport(report);

						firstReport = false;
						report = new MultiTouchReport((byte)tmpList.Count, firstReport);
					}
				}

				foreach (HidContactInfo info in tmpList.Where(c => !c.InRange && !c.TipSwitch))
					contacts.Remove(info);
			}
			finally
			{
				contacts.LockObject.ExitWriteLock();
			}
		}

		private void SendReport(MultiTouchReport report)
		{
			report.ToBytes();
			device.WriteReport(report);
		}

		public void AddContact(HidContactInfo info)
		{
			contacts.LockObject.EnterWriteLock();
			try
			{
				contacts.Add(info);
			}
			finally
			{
				contacts.LockObject.ExitWriteLock();
			}
		}

		public void UpdateContact(HidContactInfo info)
		{
			int index = 0;
			contacts.LockObject.EnterWriteLock();
			try
			{
				foreach (HidContactInfo contact in contacts)
				{
					if (contact.Equals(info))
					{
						info.Timestamp = DateTime.Now;
						contacts[index] = info;
						break;
					}
					index++;
				}
			}
			finally
			{
				contacts.LockObject.ExitWriteLock();
			}
		}

		public void RemoveContact(HidContactInfo info)
		{
			UpdateContact(info);
		}

		public void Dispose()
		{
			device.Dispose();
			if (timer != null)
				timer.Dispose();
		}
	}
}