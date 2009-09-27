using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using HIDLibrary;

namespace Multitouch.Driver.Logic
{
	class DriverCommunicator : IDisposable
	{
		private readonly Queue<HidContactInfo> currentContacts;
		private readonly Dictionary<int, HidContactInfo> lastContactInfo;

		private readonly Timer timer;
		private readonly HidDevice device;
		private readonly HidContactInfoEqualityComparer comparer;
		private readonly object lockCurrentContacts;

		public DriverCommunicator()
		{
			comparer = new HidContactInfoEqualityComparer();
			lockCurrentContacts = new object();

			device = HidDevices.Enumerate(0xdddd, 0x0001).FirstOrDefault();
			if (device == null)
				throw new InvalidOperationException("Universal Software HID driver was not found. Please ensure that it is installed.");

			device.Open(HidDevice.DeviceMode.Overlapped, HidDevice.DeviceMode.NonOverlapped);

			currentContacts = new Queue<HidContactInfo>();
			lastContactInfo = new Dictionary<int, HidContactInfo>();

			timer = new Timer();
			timer.AutoReset = false;
			timer.Interval = 1000 / 133d;
			timer.Elapsed += timer_Elapsed;
			timer.Start();
		}

		void timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			SendContacts();
			timer.Start();
		}

		private void SendContacts()
		{
			List<HidContactInfo> contactsToSend = new List<HidContactInfo>();
			lock (lockCurrentContacts)
			{
				while (currentContacts.Count > 0)
				{
					HidContactInfo contactInfo = currentContacts.Dequeue();

					HidContactInfo lastContact;
					if (lastContactInfo.TryGetValue(contactInfo.Id, out lastContact))
					{
						if (contactInfo.State == HidContactState.Updated && lastContact.State == HidContactState.Removing)
							continue;
					}

					lastContactInfo[contactInfo.Id] = contactInfo;
					contactsToSend.Add(contactInfo);
				}
			}
			contactsToSend.AddRange(lastContactInfo.Values.Except(contactsToSend, comparer).Where(c => c.State == HidContactState.Updated).ToList());

			if (contactsToSend.Count > 0)
				SendContacts(contactsToSend);

			foreach (ushort id in lastContactInfo.Values.Where(c => c.State == HidContactState.Removed).Select(c => c.Id).ToList())
				lastContactInfo.Remove(id);
			foreach (HidContactInfo contactInfo in lastContactInfo.Values.Where(c => c.State == HidContactState.Removing).ToList())
				Enqueue(new HidContactInfo(HidContactState.Removed, contactInfo.Contact));
			foreach (HidContactInfo contactInfo in lastContactInfo.Values.Where(c => c.State == HidContactState.Adding).ToList())
				Enqueue(new HidContactInfo(HidContactState.Updated, contactInfo.Contact));
		}

		private void SendContacts(ICollection<HidContactInfo> contactsToSend)
		{
			MultiTouchReport report = new MultiTouchReport((byte) contactsToSend.Count, true);
			int index = 0;
			foreach (HidContactInfo contactInfo in contactsToSend)
			{
				contactInfo.Timestamp = DateTime.Now;
				report.Contacts.Add(contactInfo);
				index++;

				if (contactsToSend.Count - index == 0)
					SendReport(report);
				else if (index % MultiTouchReport.MaxContactsPerReport == 0)
				{
					SendReport(report);
					report = new MultiTouchReport((byte) contactsToSend.Count, false);
				}
			}
		}

		private void SendReport(MultiTouchReport report)
		{
			report.ToBytes();
			device.WriteReport(report);
		}

		public void Enqueue(HidContactInfo contactInfo)
		{
			lock (lockCurrentContacts)
			{
				currentContacts.Enqueue(contactInfo);
			}
		}

		public void Dispose()
		{
			device.Dispose();
			if (timer != null)
				timer.Dispose();
		}
	}
}