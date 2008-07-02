using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
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
			communicationLogic.RegisterContactHandler(this);
			communicationLogic.Connect(((HwndSource)source).Handle);
			inputManager = InputManager.Current;
			multitouchLogic = MultitouchLogic.Current;
		}

		public void Dispose()
		{
			communicationLogic.Disconnect();
			communicationLogic.UnregisterContactHandler(this);
		}

		public void ProcessContactChange(int id, double x, double y, double width, double height, ContactState state)
		{
			MultitouchDevice device = multitouchLogic.DeviceManager.GetDevice(id, state);
			if (device != null)
			{
				RawMultitouchReport e = new RawMultitouchReport(device, source, id, x, y, width, height, state, Environment.TickCount);
				lock (lockContactsQueue)
				{
					contactsQueue.Enqueue(e);
				}
				Dispatcher.BeginInvoke(DispatcherPriority.Input, inputManagerProcessInput, null);
			}
			else
				Trace.TraceError(string.Format("Can't find MultitouchDevice for contact id: {0}, state: {1}", id, state));
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