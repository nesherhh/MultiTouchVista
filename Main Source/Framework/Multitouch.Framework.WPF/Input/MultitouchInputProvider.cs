using System;
using System.Collections.Generic;
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
		readonly InputManager inputManager;
		MultitouchLogic multitouchLogic;
		readonly object lockContactsQueue = new object();
		readonly Queue<RawMultitouchReport> contactsQueue;
		readonly DispatcherOperationCallback inputManagerProcessInput;
		readonly ContactHandler contactHandler;

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
			ContactContext context = new ContactContext(e.Contact, source.RootVisual as UIElement);
			RawMultitouchReport rawReport = new RawMultitouchReport(context);
			lock (lockContactsQueue)
			{
				contactsQueue.Enqueue(rawReport);
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