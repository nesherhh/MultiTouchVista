using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using DWMaxxAddIn.Native;

namespace DWMaxxAddIn
{
	public class WindowManager : IDisposable
	{
		public event WindowsChangedEventHandler WindowsChanged;
		public delegate void WindowsChangedEventHandler();

		public delegate void WindowEventHandler(Window window);
		public event WindowEventHandler WindowCreated;
		public event WindowEventHandler WindowDestroyed;
		public event WindowEventHandler WindowActivated;

		public event TaskManActivatedEventHandler TaskManActivated;
		public delegate void TaskManActivatedEventHandler(IntPtr hwnd);
		public event ShellWindowActivatedEventHandler ShellWindowActivated;
		public delegate void ShellWindowActivatedEventHandler();

		public IntPtr ActiveWindowHandle { get; set; }

		bool bHookSet;

		const string shellHandles = ",";
		NativeMethods.WndProcDelegate WndProcCallback;
		Dictionary<IntPtr, Window> windows;

		public bool MonitorWindowEvents { get; set; }

		public WindowManager()
		{
			windows = new Dictionary<IntPtr, Window>();
			mShellHook.SHptr = Marshal.GetIUnknownForObject(this);
			MonitorWindowEvents = true;
		}

		~WindowManager()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			RemoveShellHook();
		}

		public void Register(IntPtr handle)
		{
			SetShellHook(handle, NativeMethods.ShellHookTypes.RSH_REGISTER_TASKMAN);
			GetWindows();
		}

		internal bool SetShellHook(IntPtr hWnd, NativeMethods.ShellHookTypes hookType)
		{
			mShellHook.uRegMsg = NativeMethods.RegisterWindowMessage("SHELLHOOK");
			NativeMethods.RegisterShellHook(hWnd, (int)hookType);
			mShellHook.OldProc = NativeMethods.GetWindowLong(hWnd, NativeMethods.GetWindowLongIndex.GWL_WNDPROC);
			WndProcCallback = mShellHook.WndProc;
			NativeMethods.SetWindowLong2(hWnd, NativeMethods.GWL_WNDPROC, WndProcCallback);
			bHookSet = true;
			return bHookSet;
		}

		private void GetWindows()
		{
			MonitorWindowEvents = false;
			Thread t = new Thread(GetWindowsThread);
			t.Start();
		}

		private void GetWindowsThread()
		{
			NativeMethods.EnumWindows(EnumWindowsCallback, IntPtr.Zero);
			MonitorWindowEvents = true;
			RaiseWindowsChanged();
		}

		private void RaiseWindowsChanged()
		{
			if (WindowCreated != null)
			{
				foreach (Window window in windows.Values.ToArray())
					WindowCreated(window);
			}
		}

		public bool EnumWindowsCallback(IntPtr hWnd, IntPtr lParam)
		{
			// Check if the window has any Owner, we do not need 
			// multiple Windows of an Application.
			IntPtr RetValue = NativeMethods.GetWindow(hWnd, NativeMethods.GetWindowCmd.GW_OWNER);
			int RetValueWL = NativeMethods.GetWindowLong(hWnd, NativeMethods.GetWindowLongIndex.GWL_STYLE);

			// If the handle belongs to our application lets not display it
			//If hWnd = fInstance.Handle.ToInt32 Then RetValue = -1

			if (NativeMethods.IsWindowVisible(hWnd))
			{
				if (RetValue == IntPtr.Zero && (RetValueWL & NativeMethods.WS_SYSMENU) != 0)
				{
					// Initialize a new Window Object
					Window window = new Window(hWnd);

					// Add the Window Item to the collection
					windows.Add(window.HWnd, window);
				}
			}

			// Return 1, so that the loop continues until the last window
			return true;
		}

		public bool RemoveShellHook()
		{
			IntPtr hWnd = IntPtr.Zero;
			if (!bHookSet)
				return false;

			NativeMethods.RegisterShellHook(hWnd, NativeMethods.RSH_DEREGISTER);
			NativeMethods.SetWindowLong(hWnd, NativeMethods.GWL_WNDPROC, new IntPtr(mShellHook.OldProc));
			bHookSet = false;

			return true;
		}

		internal void FireEvent(NativeMethods.ShellEvents nEvent, IntPtr lExtraInfo)
		{
			if (MonitorWindowEvents)
			{
				//int pId;
				//int tId;
				switch (nEvent)
				{
					case NativeMethods.ShellEvents.HSHELL_WINDOWCREATED:
						if (!windows.ContainsKey(lExtraInfo))
						{
							Window window = new Window(lExtraInfo);
							windows.Add(window.HWnd, window);
							if (WindowCreated != null)
								WindowCreated(window);
						}

						break;
					case NativeMethods.ShellEvents.HSHELL_WINDOWDESTROYED:
						if (windows.ContainsKey(lExtraInfo))
						{
							if (WindowDestroyed != null)
								WindowDestroyed(windows[lExtraInfo]);
							windows.Remove(lExtraInfo);
						}

						break;
					case NativeMethods.ShellEvents.HSHELL_ACTIVATESHELLWINDOW:
						if (ShellWindowActivated != null)
							ShellWindowActivated();

						break;
					case NativeMethods.ShellEvents.HSHELL_WINDOWACTIVATED:
						if (!shellHandles.Contains("," + lExtraInfo + ","))
						{
							ActiveWindowHandle = lExtraInfo;
							if (WindowActivated != null)
								WindowActivated(windows[lExtraInfo]);
						}

						break;
					case NativeMethods.ShellEvents.HSHELL_TASKMAN:
						if (TaskManActivated != null)
							TaskManActivated(lExtraInfo);

						break;
					default:
						break;
				}
			}
		}

		class mShellHook
		{

			public static int OldProc;
			public static uint uRegMsg;
			public static IntPtr SHptr;

			public static IntPtr WndProc(IntPtr hWnd, uint wMsg, IntPtr wParam, IntPtr lParam)
			{
				IntPtr functionReturnValue = IntPtr.Zero;
				if (wMsg == uRegMsg)
					ResolvePointer(SHptr).FireEvent((NativeMethods.ShellEvents)wParam, lParam);
				else
					functionReturnValue = NativeMethods.CallWindowProc(OldProc, hWnd, wMsg, wParam, lParam);
				return functionReturnValue;
			}

			private static WindowManager ResolvePointer(IntPtr lpObj)
			{
				return (WindowManager)Marshal.GetObjectForIUnknown(lpObj);
			}

		}
	}
}