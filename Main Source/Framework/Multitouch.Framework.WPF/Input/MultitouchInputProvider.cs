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
	class MultitouchInputProvider : DispatcherObject, IDisposable
	{
		readonly PresentationSource source;
		InputManager inputManager;
		MultitouchLogic multitouchLogic;
		object lockContactsQueue = new object();
		Queue<RawMultitouchReport> contactsQueue;
		DispatcherOperationCallback inputManagerProcessInput;
		ContactHandler contactHandler;

		public MultitouchInputProvider(PresentationSource source)
		{
			this.source = source;
			contactsQueue = new Queue<RawMultitouchReport>();
			inputManagerProcessInput = InputManagerProcessInput;

			contactHandler = new ContactHandler(((HwndSource)source).Handle);
			contactHandler.ContactMoved += HandleContact;
			contactHandler.ContactRemoved += HandleContact;
			contactHandler.NewContact += HandleContact;

			inputManager = InputManager.Current;
			multitouchLogic = MultitouchLogic.Current;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if(disposing)
				contactHandler.Dispose();
		}

		void HandleContact(object sender, Framework.Input.ContactEventArgs e)
		{
			MultitouchDevice device = multitouchLogic.DeviceManager.GetDevice(e.Contact.Id, e.Contact.State);
			if (device != null)
			{
				RawMultitouchReport rawReport = new RawMultitouchReport(device, source, e.Contact);
				lock (lockContactsQueue)
				{
					contactsQueue.Enqueue(rawReport);
				}
				Dispatcher.BeginInvoke(DispatcherPriority.Input, inputManagerProcessInput, null);
			}
			else
				Trace.TraceError(string.Format("Can't find MultitouchDevice for contact id: {0}, state: {1}", e.Contact.Id, e.Contact.State));
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