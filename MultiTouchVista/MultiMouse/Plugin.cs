using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using IMultiInput;
using RawInputSharp;

namespace MultiMouse
{
	public class Plugin : System.Windows.Forms.Form, IMultiInput.IMultiInput
	{
	    
		#region Info
	    
		string IMultiInput.IMultiInput.AboutInfo {
			get {return "Multiple Mouse Input Plugin By Dean North";}
		}
	    
		string IMultiInput.IMultiInput.Name {
			get {return "MultiMouse";}
		}
	    
		string IMultiInput.IMultiInput.ReleaseDate {
			get {return "April 2008";}
		}
	    
		string IMultiInput.IMultiInput.Version {
			get {return "1.0.0";}
		}
	    
		#endregion

		private IMultiInput.InputEventHandler _MyInputEvent;
		event IMultiInput.InputEventHandler IMultiInput.IMultiInput.Input
		{
		    add { _MyInputEvent += value; }
		    remove { _MyInputEvent -= value; }
		}
	    
		RawInputSharp.RawMouseInput RawInput;
		ArrayList Cursors = new ArrayList();
	    
		void IMultiInput.IMultiInput.Initialize(Int32 HostHandle)
		{
			RawInput = new RawInputSharp.RawMouseInput();
			RawInput.RegisterForWM_INPUT(this.Handle);
			for (int i = 0; i <= RawInput.Mice.Count - 1; i++) {
				DeviceVisual dv = new DeviceVisual(i.ToString(), 0, 0);
				Cursors.Add(dv);
			}
			((DeviceVisual)Cursors[2]).ChangeCursorColor(System.Drawing.Color.Blue);
			((DeviceVisual)Cursors[0]).ChangeCursorColor(System.Drawing.Color.Green);
	        
			((DeviceVisual)Cursors[1]).Visible = false;
	        
			System.Windows.Forms.Cursor.Hide();
		}

		[System.Runtime.InteropServices.DllImport("user32.dll")]
		static extern UInt32 SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int nWidth, int nHeight, UInt32 uFlags);

	    
		const int WM_INPUT = 255;
		protected override void WndProc(ref System.Windows.Forms.Message m)
		{
			switch (m.Msg) {
				case WM_INPUT:
					RawInput.UpdateRawMouse(m.LParam);
					for (int i = 0; i <= Cursors.Count - 1; i++) {
						System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper((DeviceVisual)Cursors[i]);
						RawMouse rm = (RawMouse)RawInput.Mice[i];
						SetWindowPos(helper.Handle, IntPtr.Zero, rm.X, rm.Y, 0, 0, (uint)17693);
						if (_MyInputEvent != null)
						{
							_MyInputEvent(helper.Handle.ToInt32(), rm.X, rm.Y, rm.Buttons);
						}
					}

					System.Windows.Forms.Cursor.Position = new System.Drawing.Point(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height / 2);
					return;
			}
			base.WndProc(ref m);
		}
	    
	}
}


