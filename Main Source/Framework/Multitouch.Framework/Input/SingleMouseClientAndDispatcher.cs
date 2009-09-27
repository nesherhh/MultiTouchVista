using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using Multitouch.Framework.Input.Service;

namespace Multitouch.Framework.Input
{
	class SingleMouseClientAndDispatcher : IApplicationInterface, IApplicationInterfaceCallback, IDisposable
	{
		readonly CommunicationLogic logic;
		readonly NativeMethods.LowLevelMouseProc mouseProc;
		readonly IntPtr hookId;

		int id;
		bool contactPressent;

		public SingleMouseClientAndDispatcher(CommunicationLogic logic)
		{
			this.logic = logic;
			mouseProc = mouseHook_MessageIntercepted;
			hookId = SetHook(mouseProc);
			id = 0;
		}

		private static IntPtr SetHook(NativeMethods.LowLevelMouseProc proc)
		{
			using (Process curProcess = Process.GetCurrentProcess())
			{
				using (ProcessModule curModule = curProcess.MainModule)
				{
					return NativeMethods.SetWindowsHookEx(NativeMethods.WH_MOUSE_LL, proc, NativeMethods.GetModuleHandle(curModule.ModuleName), 0);
				}
			}
		}

		IntPtr mouseHook_MessageIntercepted(int nCode, IntPtr wParam, IntPtr lParam)
		{
			if (nCode >= 0)
			{
				FrameData data = new FrameData();
				data.Timestamp = Stopwatch.GetTimestamp();
				data.Images = new Service.ImageData[0];

				NativeMethods.MSLLHOOKSTRUCT mouseStructure = (NativeMethods.MSLLHOOKSTRUCT) Marshal.PtrToStructure(lParam, typeof (NativeMethods.MSLLHOOKSTRUCT));

				ContactData contact = new ContactData();
				contact.MajorAxis = 10;
				contact.MinorAxis = 10;
				contact.Orientation = 0;

				IntPtr hwnd = NativeMethods.WindowFromPoint(mouseStructure.pt);
				if (hwnd != IntPtr.Zero)
				{
					contact.Hwnd = hwnd;

					NativeMethods.POINT clientPoint = NativeMethods.ScreenToClient(mouseStructure.pt, hwnd);
					contact.Position = new Point(clientPoint.X, clientPoint.Y);

					if (wParam == (IntPtr) NativeMethods.MouseMessages.WM_LBUTTONDOWN)
					{
						id++;
						if (id == int.MaxValue)
							id = 0;
						contact.State = Service.ContactState.New;
					}
					else if (wParam == (IntPtr) NativeMethods.MouseMessages.WM_LBUTTONUP)
					{
						contact.State = Service.ContactState.Removed;
					}
					else if (wParam == (IntPtr) NativeMethods.MouseMessages.WM_MOUSEMOVE)
					{
						contact.State = Service.ContactState.Moved;
					}
					contact.Id = id;
					contact.Bounds = new Rect(contact.Position.X - (contact.MajorAxis/2), contact.Position.Y - (contact.MinorAxis/2),
					                          contact.MajorAxis, contact.MinorAxis);
					data.Contacts = new[] {contact};

					if (contact.State == Service.ContactState.New)
						contactPressent = true;

					if (contactPressent)
						Frame(data);

					else if (contact.State == Service.ContactState.Removed)
						contactPressent = false;
				}
			}
			return NativeMethods.CallNextHookEx(hookId, nCode, wParam, lParam);
		}

		public void CreateSession()
		{}

		public void RemoveSession()
		{}

		public void AddWindowHandleToSession(IntPtr windowHandle)
		{}

		public void RemoveWindowHandleFromSession(IntPtr windowHandle)
		{}

		public void SendEmptyFrames(bool value)
		{}

		public bool SendImageType(Service.ImageType imageType, bool value)
		{
			return false;
		}

		public void Frame(FrameData data)
		{
			logic.DispatchFrame(data);			
		}

		public void Dispose()
		{
			if(hookId != IntPtr.Zero)
				NativeMethods.UnhookWindowsHookEx(hookId);
		}
	}
}
