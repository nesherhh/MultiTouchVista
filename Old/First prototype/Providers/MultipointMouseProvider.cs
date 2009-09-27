using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Danilins.Multitouch.Common;
using Danilins.Multitouch.Common.Providers;
using Danilins.Multitouch.Providers.Native;
using ManagedWinapi.Hooks;
using ManagedWinapi.Windows;
using InputMode=Danilins.Multitouch.Providers.Native.InputMode;
using MouseButtonState=Danilins.Multitouch.Providers.Native.MouseButtonState;
using Timer=System.Timers.Timer;
using WFCursor = System.Windows.Forms.Cursor;

namespace Danilins.Multitouch.Providers
{
	[InputProvider("{00C5E82B-2251-4985-A34D-DD7466B627D6}", "Multimouse", false)]
	public class MultipointMouseProvider : InputProvider
	{
		private readonly AutoResetEvent nextFrame;
		private readonly Dictionary<IntPtr, ContactInfo> currentList;
		private readonly Dictionary<IntPtr, ContactInfo> historyList;
		private readonly Dictionary<IntPtr, CursorData> deviceList;

		readonly ReaderWriterLockSlim currentListLock = new ReaderWriterLockSlim();
		readonly ReaderWriterLockSlim historyListLock = new ReaderWriterLockSlim();
			
		private Timer timer;

		private int width;
		private int height;
		private int bitPerPixel;
		private int currentId;
		private Dispatcher dispatcher;
		private LowLevelMouseHook lowLevelMouseHook;
		double verticalDPI;
		double horizontalDPI;

		public MultipointMouseProvider()
		{
			width = (int)SystemParameters.PrimaryScreenWidth;
			height = (int)SystemParameters.PrimaryScreenHeight;
			bitPerPixel = PixelFormats.Pbgra32.BitsPerPixel;

			currentId = 1;

			nextFrame = new AutoResetEvent(false);
			currentList = new Dictionary<IntPtr, ContactInfo>();
			historyList = new Dictionary<IntPtr, ContactInfo>();
			deviceList = new Dictionary<IntPtr, CursorData>();

			dispatcher = Dispatcher.CurrentDispatcher;
		}

		public override void Start()
		{
			foreach (RawDevice device in RawDevice.GetRawDevices())
			{
				if (device.RawType == RawType.Mouse)
					deviceList.Add(device.Handle, new CursorData());
			}

			Thread t = new Thread((ThreadStart)delegate
			                                   	{
			                                   		RawDevice.RegisterRawDevices(0x01, 0x02,
			                                   		                             InputMode.BackgroundMode | InputMode.SuppressMessages);
			                                   		RawDevice.RawInput += RawDevice_RawInput;
			                                   		lowLevelMouseHook = new LowLevelMouseHook(MouseHookCallback);
			                                   		System.Windows.Forms.Application.Run();
			                                   	});
			t.IsBackground = true;
			t.SetApartmentState(ApartmentState.STA);
			t.Start();

			timer = new Timer(10);
			timer.Elapsed += timer_Elapsed;
			timer.Start();

			Mouse.OverrideCursor = Cursors.None;

			isRunning = true;
		}

		public override void Stop()
		{
			if(timer != null)
				timer.Stop();
			foreach (KeyValuePair<IntPtr, CursorData> pair in deviceList)
				pair.Value.CloseCursor();
			if (lowLevelMouseHook != null)
				lowLevelMouseHook.Unhook();
			WFCursor.Show();
			RawDevice.UnregisterRawDevices(0x01, 0x02);
			RawDevice.RawInput -= RawDevice_RawInput;
			isRunning = false;

			System.Windows.Forms.Application.Exit();

			deviceList.Clear();
			historyList.Clear();
			currentList.Clear();
		}

		public override Guid Id
		{
			get { return new Guid("{00C5E82B-2251-4985-A34D-DD7466B627D6}"); }
		}

		public override string Name
		{
			get { return "Multimouse"; }
		}

		private void MouseHookCallback(int msg, POINT pt, int mouseData, int flags, int time, IntPtr dwExtraInfo, ref bool handled)
		{
			if (flags != -1)
				handled = true;
		}

		void RawDevice_RawInput(object sender, RawInputEventArgs e)
		{
			if (e.Handle != IntPtr.Zero)
			{
				MouseData data = (MouseData) e.GetRawData();
				CursorData cursorData = deviceList[e.Handle];

				MouseButtonState oldState = cursorData.ButtonState;
				MouseButtonState newState = data.ButtonState;

				double x = cursorData.X + data.X;
				double y = cursorData.Y + data.Y;

				if (newState == MouseButtonState.LeftDown)
					OnMouseDown(e.Handle, x, y, cursorData.X, cursorData.Y);
				else if (newState == MouseButtonState.LeftUp)
					OnMouseUp(e.Handle, x, y, cursorData.X, cursorData.Y);
				else if (oldState == MouseButtonState.LeftDown)
					OnMouseMove(e.Handle, x, y, cursorData.X, cursorData.Y);

				if (newState != MouseButtonState.None)
					cursorData.ButtonState = newState;
				dispatcher.Invoke(DispatcherPriority.Normal, (Action) delegate { cursorData.SetPosition(x, y); });
			}
		}

		private void timer_Elapsed(object state, ElapsedEventArgs e)
		{
			nextFrame.Set();
		}

		void OnMouseUp(IntPtr handle, double x, double y, double oldX, double oldY)
		{
			currentListLock.EnterReadLock();
			historyListLock.EnterWriteLock();

			var contactInfo = currentList[handle];
			//contactInfo.PreviousCenter = new Point(oldX, oldY);
			historyList[handle] = contactInfo;

			historyListLock.ExitWriteLock();
			currentListLock.ExitReadLock();
			currentListLock.EnterWriteLock();

			currentList.Remove(handle);
			
			currentListLock.ExitWriteLock();

			//MouseEmulation.Action(x, y, MouseEmulation.MouseAction.LeftUp);
		}

		void OnMouseMove(IntPtr handle, double x, double y, double oldX, double oldY)
		{
			ContactInfo contact = new ContactInfo(new Rect(x, y, 10, 10));
			
			currentListLock.EnterReadLock();
			ContactInfo oldContact = currentList[handle];
			currentListLock.ExitReadLock();

			contact.Id = oldContact.Id;
			if (contact.Id == -1)
			{
				contact.Delta = new Vector();
				contact.DeltaArea = 0;
				contact.PredictedPos = contact.Center;
				contact.Displacement = new Vector();
			}
			else
			{
				contact.Delta = contact.Center - oldContact.Center;
				contact.DeltaArea = contact.Area - oldContact.Area;
				contact.PredictedPos = contact.Center + contact.Delta;
				contact.Displacement = oldContact.Displacement + contact.Delta;
				contact.PreviousCenter = oldContact.PreviousCenter;
			}
			historyListLock.EnterWriteLock();

			historyList[handle] = oldContact;

			historyListLock.ExitWriteLock();
			currentListLock.EnterWriteLock();

			currentList[handle] = contact;

			currentListLock.ExitWriteLock();

			//MouseEmulation.Action(x, y, MouseEmulation.MouseAction.Move);
		}

		void OnMouseDown(IntPtr handle, double x, double y, double oldX, double oldY)
		{
			ContactInfo contact = new ContactInfo(new Rect(x, y, 10, 10));
			contact.PreviousCenter = contact.Center;

			historyListLock.EnterWriteLock();

			historyList.Remove(handle);

			historyListLock.ExitWriteLock();
			currentListLock.EnterWriteLock();

			currentList[handle] = contact;
			Trace.WriteLine(handle);

			currentListLock.ExitWriteLock();

			//MouseEmulation.Action(x, y, MouseEmulation.MouseAction.LeftDown);
		}

		public override void Dispose()
		{
			Stop();
		}

		protected override ContactInfo[] GetContactsCore()
		{
			nextFrame.WaitOne();

			List<ContactInfo> result = new List<ContactInfo>();

			currentListLock.EnterReadLock();
			Dictionary<IntPtr, ContactInfo> currentListCopy = new Dictionary<IntPtr,ContactInfo>(currentList);
			currentListLock.ExitReadLock();

			foreach (KeyValuePair<IntPtr, ContactInfo> pair in currentListCopy)
			{
				ContactInfo currentContact = pair.Value;
				if (currentContact.Id == -1)
				{
					currentContact.Id = currentId;
					currentId++;
					if (currentId >= 65535)
						currentId = 1;

					currentContact.State = ContactState.Down;
					result.Add(currentContact);
				}
				else
				{
					currentContact.State = ContactState.Move;
					result.Add(currentContact);
				}
			}

			List<IntPtr> toRemove = new List<IntPtr>();

			historyListLock.EnterReadLock();
			Dictionary<IntPtr, ContactInfo> historyListCopy = new Dictionary<IntPtr,ContactInfo>(historyList);
			historyListLock.ExitReadLock();

			foreach (KeyValuePair<IntPtr, ContactInfo> pair in historyListCopy)
			{
				ContactInfo previousContact = pair.Value;
				bool found = false;
				foreach (ContactInfo currentContact in currentListCopy.Values)
				{
					if (currentContact.Id == previousContact.Id)
					{
						found = true;
						break;
					}
				}

				if (!found)
				{
					previousContact.State = ContactState.Up;
					result.Add(previousContact);
					toRemove.Add(pair.Key);
				}
			}

			historyListLock.EnterWriteLock();
			foreach (IntPtr key in toRemove)
				historyList.Remove(key);
			historyListLock.ExitWriteLock();

			//PreviewInputOutput();

			return result.ToArray();
		}

		private void PreviewInputOutput()
		{
			if (ShowPreview && (inputPreviewHandler != null || outputPreviewHandler != null))
			{
				RenderTargetBitmap renderToBitmap = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);

				DrawingVisual visual = new DrawingVisual();
				DrawingContext drawingContext = visual.RenderOpen();

				currentListLock.EnterReadLock();
				Dictionary<IntPtr, ContactInfo> currentListCopy = new Dictionary<IntPtr, ContactInfo>(currentList);
				currentListLock.ExitReadLock();

				foreach (KeyValuePair<IntPtr, ContactInfo> pair in currentListCopy)
				{
					ContactInfo contactInfo = pair.Value;
					drawingContext.DrawEllipse(Brushes.White, new Pen(), contactInfo.Center, contactInfo.Rectangle.Width,
											   contactInfo.Rectangle.Height);
				}
				drawingContext.Close();

				renderToBitmap.Render(visual);

				int stride = renderToBitmap.Format.GetStride(renderToBitmap.PixelWidth);
				byte[] pixels = new byte[renderToBitmap.PixelHeight * stride];
				renderToBitmap.CopyPixels(pixels, stride, 0);

				if (inputPreviewHandler != null)
					inputPreviewHandler(width, height, bitPerPixel, pixels);
				if (outputPreviewHandler != null)
					outputPreviewHandler(width, height, bitPerPixel, pixels);
			}
		}
	}
}