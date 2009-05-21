using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

		// Device interface detail data
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public struct SP_DEVICE_INTERFACE_DETAIL_DATA
		{
			public UInt32 cbSize;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
			public string DevicePath;
		}

		public DriverCommunicator()
		{
			device = HidDevices.Enumerate(0xdddd, 0x0001).FirstOrDefault();
			if (device == null)
				throw new InvalidOperationException("Universal Software HID driver was not found. Please ensure that it is installed.");

			device.Open(HidDevice.DeviceMode.Overlapped, HidDevice.DeviceMode.NonOverlapped);

			contacts = new ContactCollection();

			timer = new Timer();
			timer.AutoReset = false;
			timer.Interval = 15d / 1000;
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
			int index = 0;

			DateTime now = DateTime.Now;

			contacts.LockObject.EnterWriteLock();
			try
			{
				List<HidContactInfo> tmpList = new List<HidContactInfo>(contacts);
				MultiTouchReport report = new MultiTouchReport((byte) tmpList.Count, true);
				foreach (HidContactInfo info in tmpList)
				{
					TimeSpan span = now - info.Timestamp;
					if (span.Seconds > ContactTimeout)
						info.State = HidContactState.Removing;

					report.Contacts.Add(info);
					index++;

					if (tmpList.Count - index == 0)
						SendReport(report);
					else if (index % MultiTouchReport.MaxContactsPerReport == 0)
					{
						SendReport(report);

						report = new MultiTouchReport((byte) tmpList.Count, false);
					}
				}

				foreach (HidContactInfo info in tmpList.Where(c => c.State == HidContactState.Removed).ToList())
				{
					contacts.Remove(info);
					tmpList.Remove(info);
				}

				UpdateStateTransition(tmpList, HidContactState.Removing, HidContactState.Removed);
				UpdateStateTransition(tmpList, HidContactState.Adding, HidContactState.Updated);
			}
			finally
			{
				contacts.LockObject.ExitWriteLock();
			}
		}

		private void UpdateStateTransition(IEnumerable<HidContactInfo> tmpList, HidContactState startState, HidContactState endState)
		{
			foreach (HidContactInfo info in tmpList.Where(c => c.State == startState))
			{
				HidContactInfo contact;
				if (contacts.TryGet(info.Id, out contact))
				{
					info.State = endState;
					info.Timestamp = DateTime.Now;
				}
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
			contacts.LockObject.EnterWriteLock();
			try
			{
				info.Timestamp = DateTime.Now;
				int index = contacts.IndexOf(info);
				if(index >= 0)
					contacts[index] = info;
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