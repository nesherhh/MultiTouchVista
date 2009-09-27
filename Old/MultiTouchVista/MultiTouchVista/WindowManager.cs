using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;

namespace MultiTouchVista
{
	public class WindowManager
	{

		public string ShellHandles = ",";
		public Form ParentForm;

		private SortedList<string, Window> _Windows = new SortedList<string, Window>();
		public SortedList<string, Window> Windows
		{
			get
			{
				int RemoveCount = 0;
				for (int i = 0; i <= _Windows.Count - 1; i++)
				{
					if (API.IsWindow(_Windows.Values[i - RemoveCount].Handle.ToInt32()) == 0)
					{
						_Windows.Remove(_Windows.Values[i - RemoveCount].Handle.ToString());
						RemoveCount += 1;
					}
				}
				return _Windows;
			}
		}

		public IntPtr ActiveWindowHandle;

		private bool _MonitorWindowEvents = true;
		public bool MonitorWindowEvents
		{
			get { return MonitorWindowEvents; }
			set { _MonitorWindowEvents = value; }
		}

		public WindowManager()
		{
			mShellHook.SHptr = Marshal.GetIUnknownForObject(this);
		}

		~WindowManager()
		{
			RemoveShellHook();
		}

		public void Register(IntPtr Handle)
		{
			SetShellHook(Handle.ToInt32(), API.ShellHookTypes.RSH_REGISTER_TASKMAN);
			GetWindows();
		}

		public event WindowsChangedEventHandler WindowsChanged;
		public delegate void WindowsChangedEventHandler();

		private void GetWindows()
		{
			_Windows = new SortedList<string, Window>();
			_MonitorWindowEvents = false;
			Thread t = new Thread(GetWindowsThread);
			t.Start();
		}

		private void GetWindowsThread()
		{
			API.EnumWindows(EnumWindowsCallback, 0);
			_MonitorWindowEvents = true;
			this.ParentForm.Invoke(new System.CrossAppDomainDelegate(RaiseWindowsChanged));
		}

		private void RaiseWindowsChanged()
		{
			if (WindowCreated != null)
			{
				foreach (Window window in Windows.Values)
				{
					WindowCreated(window);
				}
			}
		}

		public int EnumWindowsCallback(int hWnd, int lParam)
		{
			Window Window;

			int RetValue;
			int RetValueWL;

			// Check if the window has any Owner, we do not need 
			// multiple Windows of an Application.
			RetValue = API.GetWindow(hWnd, API.GW_OWNER);
			RetValueWL = API.GetWindowLong(hWnd, API.GWL_STYLE);

			// If the handle belongs to our application lets not display it
			//If hWnd = fInstance.Handle.ToInt32 Then RetValue = -1

			if (API.IsWindowVisible(hWnd) == 1)
			{
				if (RetValue == 0 && (RetValueWL & API.WS_SYSMENU)!=0)
				{
					// Initialize a new Window Object
					Window = new Window(new IntPtr(hWnd));

					// Add the Window Item to the collection
					Windows.Add(Window.Handle.ToString(), Window);
				}
			}

			// Return 1, so that the loop continues until the last window
			return 1;
		}

		#region Shell Window Events Hook

		public event WindowCreatedEventHandler WindowCreated;
		public delegate void WindowCreatedEventHandler(Window window);
		public event WindowDestroyedEventHandler WindowDestroyed;
		public delegate void WindowDestroyedEventHandler(Window window);
		public event WindowActivatedEventHandler WindowActivated;
		public delegate void WindowActivatedEventHandler(Window window);

		public event TaskManActivatedEventHandler TaskManActivated;
		public delegate void TaskManActivatedEventHandler(int hwnd);
		public event ShellWindowActivatedEventHandler ShellWindowActivated;
		public delegate void ShellWindowActivatedEventHandler();

		int m_var_hwnd;
		bool bHookSet;

		public bool RemoveShellHook()
		{
			int hwnd = 0;
			if (!bHookSet)
				return false;

			API.RegisterShellHook(hwnd, API.RSH_DEREGISTER);
			API.SetWindowLong(hwnd, API.GWL_WNDPROC, mShellHook.OldProc);
			bHookSet = false;

			return true;
		}

		private API.WndProcDelegate WndProcCallback;
		public bool SetShellHook(int hwnd, API.ShellHookTypes HookType)
		{
			mShellHook.uRegMsg = API.RegisterWindowMessage("SHELLHOOK");
			API.RegisterShellHook(hwnd, (int)HookType);
			mShellHook.OldProc = API.GetWindowLong(hwnd, API.GWL_WNDPROC);
			WndProcCallback = new API.WndProcDelegate(mShellHook.WndProc);
			API.SetWindowLong2(hwnd, API.GWL_WNDPROC, WndProcCallback);
			bHookSet = true;
			return bHookSet;
		}

		internal void FireEvent(API.ShellEvents nEvent, int lExtraInfo)
		{
			if (_MonitorWindowEvents)
			{
				//int pId;
				//int tId;
				switch (nEvent)
				{
					case API.ShellEvents.HSHELL_WINDOWCREATED:
						if (!Windows.ContainsKey(lExtraInfo.ToString()))
						{
							Window window = new Window(new IntPtr(lExtraInfo));
							Windows.Add(window.Handle.ToString(), window);
							if (WindowCreated != null)
							{
								WindowCreated(window);
							}
						}

						break;
					case API.ShellEvents.HSHELL_WINDOWDESTROYED:
						if (Windows.ContainsKey(lExtraInfo.ToString()))
						{
							if (WindowDestroyed != null)
							{
								WindowDestroyed(Windows[lExtraInfo.ToString()]);
							}
							Windows.Remove(lExtraInfo.ToString());
						}

						break;
					case API.ShellEvents.HSHELL_ACTIVATESHELLWINDOW:
						if (ShellWindowActivated != null)
						{
							ShellWindowActivated();
						}

						break;
					case API.ShellEvents.HSHELL_WINDOWACTIVATED:
						if (!ShellHandles.Contains("," + lExtraInfo + ","))
						{
							ActiveWindowHandle = new IntPtr(lExtraInfo);
							if (WindowActivated != null)
							{
								WindowActivated(Windows[lExtraInfo.ToString()]);
							}
						}

						break;
					case API.ShellEvents.HSHELL_TASKMAN:
						if (TaskManActivated != null)
						{
							TaskManActivated(lExtraInfo);
						}

						break;
					default:
						break;
				}
			}
		}

		private short LoWord(ref int DWORD)
		{
			short functionReturnValue = 0;
			if ((DWORD & 32768)!=0)
			{
				functionReturnValue = (short)(-32768 | (DWORD & 32767));
			}
			else
			{
				functionReturnValue = (short)(DWORD & 65535);
			}
			return functionReturnValue;
		}

		#endregion

	}

	class mShellHook
	{

		public static int OldProc;
		public static int uRegMsg;
		public static IntPtr SHptr;

		public static int WndProc(int hwnd, int wMsg,int wParam,int lParam)
		{
			int functionReturnValue = 0;
			if (wMsg == uRegMsg)
			{
				ResolvePointer(SHptr).FireEvent((API.ShellEvents)wParam,lParam);
			}
			else
			{
				functionReturnValue = API.CallWindowProc(OldProc, hwnd, wMsg, wParam, lParam);
			}
			return functionReturnValue;
		}

		private static WindowManager ResolvePointer(IntPtr lpObj)
		{
			return (WindowManager)Marshal.GetObjectForIUnknown(lpObj);
		}

	}


}
