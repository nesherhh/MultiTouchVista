using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace DWMaxxAddIn
{
	static class MessageEvents
	{
		static SynchronizationContext context;
		static MessageWindow window;
		static IntPtr handle;
		static object lockObject = new object();

		public static event EventHandler<MessageReceivedEventArgs> MessageReceived;

		public static void WatchMessage(int message)
		{
			EnsureInitialized();
			window.RegisterEventForMessage(message);
		}

		public static IntPtr Handle
		{
			get
			{
				EnsureInitialized();
				return handle;
			}
		}

		static void EnsureInitialized()
		{
			lock (lockObject)
			{
				if (window == null)
				{
					context = AsyncOperationManager.SynchronizationContext;
					using (ManualResetEvent mre = new ManualResetEvent(false))
					{
						Thread t = new Thread(() =>
						                      {
						                      	window = new MessageWindow();
						                      	handle = window.Handle;
						                      	mre.Set();
						                      	Application.Run();
						                      });
						t.Name = "MessageEvents";
						t.IsBackground = true;
						t.Start();
						mre.WaitOne();
					}
				}
			}
		}

		class MessageWindow : Form
		{
			ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();
			Dictionary<int, bool> messages = new Dictionary<int, bool>();

			public void RegisterEventForMessage(int messageId)
			{
				rwLock.EnterWriteLock();
				messages[messageId] = true;
				rwLock.ExitWriteLock();
			}

			protected override void WndProc(ref Message m)
			{
				rwLock.EnterReadLock();
				bool shouldHandle = messages.ContainsKey(m.Msg);
				rwLock.ExitReadLock();

				if (shouldHandle)
				{
					context.Post(state =>
								 {
									 EventHandler<MessageReceivedEventArgs> handler = MessageReceived;
									 if (handler != null)
										 handler(null, new MessageReceivedEventArgs((Message)state));
								 }, m);
				}
				base.WndProc(ref m);
			}
		}
	}
}