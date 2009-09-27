using System;
using Danilins.Multitouch.Common;
using Danilins.Multitouch.Logic.DWM;
using Danilins.Multitouch.Logic.LegacySupport;

namespace Danilins.Multitouch.Logic
{
	class LegacySupportLogic : Common.Logics.Logic
	{
		ContactManager contactManager;
		WindowManager windowManager;

		public LegacySupportLogic(IServiceProvider parentProvider)
			: base(parentProvider)
		{
			contactManager = new ContactManager(GetLogic<MainLogic>().UIDispatcher);
			windowManager = new WindowManager();
		}

		public void Process(ContactInfo[] contacts)
		{
			contacts = windowManager.Process(contacts);
			contactManager.Update(contacts);
		}

		public void Enable()
		{
			contactManager.IsEnabled = true;
		}

		public void Disable()
		{
			contactManager.IsEnabled = false;
		}
	}
}