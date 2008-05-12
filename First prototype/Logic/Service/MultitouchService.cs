using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using Danilins.Multitouch.Common;
using Danilins.Multitouch.Common.Providers;

namespace Danilins.Multitouch.Logic.Service
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
	class MultitouchService : IMultitouchService, IDisposable
	{
		IInputProvider inputProvider;
		LegacySupportLogic legacySupportLogic;
		HashSet<IMultitouchServiceCallback> callbacks;
		bool disposing;

		public MultitouchService(IInputProvider inputProvider, LegacySupportLogic legacySupportLogic)
		{
			disposing = false;
			callbacks = new HashSet<IMultitouchServiceCallback>();
			this.inputProvider = inputProvider;
			this.legacySupportLogic = legacySupportLogic;

			Thread t = new Thread(ThreadProc);
			t.SetApartmentState(ApartmentState.STA);
			t.IsBackground = true;
			t.Start();
		}

		public void RegisterApplication()
		{
			callbacks.Add(OperationContext.Current.GetCallbackChannel<IMultitouchServiceCallback>());
		}

		public void UnregisterApplication()
		{
			IMultitouchServiceCallback channel = OperationContext.Current.GetCallbackChannel<IMultitouchServiceCallback>();
			callbacks.Remove(channel);
		}

		private void ThreadProc()
		{
			while (!disposing)
			{
				if (inputProvider != null)
				{
					ContactInfo[] contacts = inputProvider.GetContacts();
					legacySupportLogic.Process(contacts);

					if (contacts.Length > 0)
					{
						HashSet<IMultitouchServiceCallback>.Enumerator enumerator = callbacks.GetEnumerator();
						List<IMultitouchServiceCallback> failed = new List<IMultitouchServiceCallback>();
						while (enumerator.MoveNext())
						{
							try
							{
								enumerator.Current.ProcessContact(contacts);
							}
							catch (Exception)
							{
								failed.Add(enumerator.Current);
							}
						}
						foreach (IMultitouchServiceCallback failedCallback in failed)
							callbacks.Remove(failedCallback);
					}
				}
			}
		}

		public void Dispose()
		{
			disposing = true;
		}

		public void EnableLegacySupport()
		{
			legacySupportLogic.Enable();
		}

		public void DisableLegacySupport()
		{
			legacySupportLogic.Disable();
		}
	}
}