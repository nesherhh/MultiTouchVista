using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace MultiTouchVista
{
	public class Window
	{

		public Point OriginalPosition;

		public float Angle;

		#region Properties

		private string _Text;
		public string Text
		{
			get { return _Text; }
		}

		private IntPtr _Handle;
		public IntPtr Handle
		{
			get { return _Handle; }
		}

		private Point _Position = new Point();
		public Point Position
		{
			get
			{
				API.RECT R = new API.RECT();
				API.GetWindowRect(_Handle, R);
				_Position = new Point(R.left, R.top);
				return _Position;
			}
			set { API.SetWindowPos(_Handle, IntPtr.Zero, value.X, value.Y, 0, 0, 17693); }
		}

		private Point _Translation = new Point();
		public Point Translation
		{
			get
			{
				return _Translation;
			}
			set
			{
				_Translation = value;
			}
		}

		private Double _Rotation = new Double();
		public Double Rotation
		{
			get
			{
				return _Rotation;
			}
			set
			{
				_Rotation = value;
			}
		}

		private Double _ScaleX = new Double();
		public Double ScaleX
		{
			get
			{
				return _ScaleX;
			}
			set
			{
				_ScaleX = value;
			}
		}

		private Double _ScaleY = new Double();
		public Double ScaleY
		{
			get
			{
				return _ScaleY;
			}
			set
			{
				_ScaleY = value;
			}
		}

		private Point _CenterPoint = new Point();
		public Point CenterPoint
		{
			get
			{
				API.RECT R = new API.RECT();
				API.GetWindowRect(_Handle, R);
				if (R.left < -5000)
					return _CenterPoint;
				_CenterPoint = new Point(R.left + ((R.right - R.left) / 2), R.top + ((R.bottom - R.top) / 2));
				return _CenterPoint;
			}
		}

		/*private FormWindowState _State;
		public FormWindowState State
		{
			get { return WindowState(); }
			set
			{
				_State = value;
				if (value == FormWindowState.Minimized)
				{
					PostMessage(_Handle, WM_SYSCOMMAND, SC_MINIMIZE, 0);
				}
				else if (value == FormWindowState.Maximized)
				{
					PostMessage(_Handle, WM_SYSCOMMAND, SC_MAXIMIZE, 0);
					SetForegroundWindow(_Handle);
				}
				else
				{
					PostMessage(_Handle, WM_SYSCOMMAND, SC_RESTORE, 0);
					SetForegroundWindow(_Handle);
				}
			}
		}*/

		private API.RECT PreviousRect = new API.RECT();
		private bool Visible = true;

		#endregion

		#region Functions and Methods

		public Window(IntPtr hWnd)
		{
			_Handle = hWnd;
			_Text = WindowText();
			//_State = WindowState();
		}

		/*public void Show()
		{
			//ShowWindow(_Handle, SW_SHOW)
			if (!Visible)
			{
				API.SetWindowPos(_Handle, null, PreviousRect.left, PreviousRect.top, PreviousRect.right - PreviousRect.left, PreviousRect.bottom - PreviousRect.top, SWP_NOACTIVATE | SWP_NOSENDCHANGING | SWP_NOZORDER);
				Visible = true;
			}
		}*/

		/*public void Hide()
		{
			//ShowWindow(_Handle, SW_HIDE) 'Using hidden caused loads of problems, so i used positioning instead
			if (Visible)
			{
				API.GetWindowRect(_Handle, PreviousRect);
				API.SetWindowPos(_Handle, null, PreviousRect.left, My.Computer.Screen.WorkingArea.Bottom, PreviousRect.right - PreviousRect.left, PreviousRect.bottom - PreviousRect.top, SWP_NOACTIVATE | SWP_NOSENDCHANGING | SWP_NOZORDER);
				Visible = false;
			}
		}*/

		public string WindowText()
		{

			const int nChars = 256;
			StringBuilder Buff = new StringBuilder(nChars);

			if (API.GetWindowText(_Handle.ToInt32(), Buff, nChars) > 0)
			{
				return Buff.ToString();
			} else {
				return "";
			}

		}

		/*private FormWindowState WindowState()
		{
			WINDOWPLACEMENT wp;
			wp.length = System.Runtime.InteropServices.Marshal.SizeOf(wp);
			GetWindowPlacement(_Handle, wp);

			switch (wp.showCmd)
			{
				case SW_SHOWMINIMIZED:
					return FormWindowState.Minimized;
				case SW_SHOWMAXIMIZED:
					return FormWindowState.Maximized;
				default:
					return FormWindowState.Normal;
			}

		}*/

		#endregion

	}

}
