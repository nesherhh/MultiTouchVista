using System;
using System.Diagnostics;
using System.Windows.Threading;
using Danilins.Multitouch.Common;

namespace Danilins.Multitouch.Logic.LegacySupport
{
	class ContactManager
	{
		Dispatcher dispatcher;
		ContactDataCollection contactCollection;
		private bool isEnabled;

		public ContactManager(Dispatcher dispatcher)
		{
			this.dispatcher = dispatcher;
			contactCollection = new ContactDataCollection();
			IsEnabled = true;
		}

		public bool IsEnabled
		{
			get { return isEnabled; }
			set
			{
				isEnabled = value;
				if (!isEnabled)
					contactCollection.Clear();
			}
		}

		public void Update(ContactInfo[] contactInfos)
		{
			if (isEnabled)
			{
				foreach (ContactInfo info in contactInfos)
				{
					if (info.State == ContactState.Down)
						ContactDown(info);
					else if (info.State == ContactState.Move)
						ContactMove(info);
					else if (info.State == ContactState.Up)
						ContactUp(info);
				}
			}
		}

		private void ContactUp(ContactInfo info)
		{
			if (contactCollection.Contains(info.Id))
			{
				ContactData data = contactCollection[info.Id];
				contactCollection.Remove(data);
				data.Dispose();
			}
			else
				Trace.TraceWarning(string.Format("Up without Down (Id: {0})", info.Id));
		}

		private void ContactDown(ContactInfo info)
		{
			contactCollection.Add(new ContactData(info, dispatcher));
		}

		private void ContactMove(ContactInfo info)
		{
			if(contactCollection.Contains(info.Id))
				contactCollection[info.Id].Move(info);
		}
	}
}
