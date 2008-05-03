using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Multitouch.Framework.Input;

namespace Multitouch.Framework.WPF.Input
{
	class MultitouchInputProvider : DispatcherObject, IDisposable, IContactHandler
	{
		readonly PresentationSource source;
		CommunicationLogic communicationLogic;
		InputManager inputManager;
		MultitouchLogic multitouchLogic;
		object lockContactsQueue = new object();
		Queue<RawMultitouchReport> contactsQueue;
		DispatcherOperationCallback inputManagerProcessInput;

		public MultitouchInputProvider(PresentationSource source)
		{
			this.source = source;
			contactsQueue = new Queue<RawMultitouchReport>();
			inputManagerProcessInput = InputManagerProcessInput;

			communicationLogic = CommunicationLogic.Instance;
			communicationLogic.RegisterInputProvider(this);
			communicationLogic.Connect();
			inputManager = InputManager.Current;
			multitouchLogic = MultitouchLogic.Current;
		}

		public void Dispose()
		{
			communicationLogic.Disconnect();
			communicationLogic.UnRegisterInputProvider(this);
		}

		public void ProcessContactChange(int id, double x, double y, double width, double height, ContactState state)
		{
			MultitouchDevice device = multitouchLogic.DeviceManager.GetDevice(id, state);
			RawMultitouchReport e = new RawMultitouchReport(device, source, id, x, y, width, height, state, Environment.TickCount);
			lock (lockContactsQueue)
			{
				contactsQueue.Enqueue(e);
			}
			Dispatcher.BeginInvoke(DispatcherPriority.Input, inputManagerProcessInput, null);
		}

		object InputManagerProcessInput(object arg)
		{
			RawMultitouchReport report = null;
			lock (lockContactsQueue)
			{
				if (contactsQueue.Count > 0)
					report = contactsQueue.Dequeue();
			}
			if (report != null)
				inputManager.ProcessInput(report);
			return null;
		}
	}
}
