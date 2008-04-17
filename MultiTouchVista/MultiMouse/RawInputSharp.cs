using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace RawInputSharp
{

	#region RawInput
		
		/// <summary>
		/// Parent class, contains constants from winuser.h and DllImport function stubs. Will enumerate all raw input devices.
		/// </summary>
		public abstract class RawInput
		{

			//constants from winuser.h
			public const Int32 RIM_TYPEMOUSE = 0;
			public const Int32 RIDI_DEVICENAME = 0x20000007;
			public const Int32 RID_INPUT = 0x10000003;
			public const Int32 RIDI_DEVICEINFO = 0x2000000b;
			public const Int32 RIDEV_NOLEGACY = 0x00000030;
			public const Int32 RID_HEADER = 0x10000005;
			public const Int32 RI_MOUSE_LEFT_BUTTON_DOWN = 0x0001;  // Left Button changed to down.
			public const Int32 RI_MOUSE_LEFT_BUTTON_UP = 0x0002;  // Left Button changed to up.
			public const Int32 RI_MOUSE_RIGHT_BUTTON_DOWN = 0x0004;  // Right Button changed to down.
			public const Int32 RI_MOUSE_RIGHT_BUTTON_UP = 0x0008;  // Right Button changed to up.
			public const Int32 RI_MOUSE_MIDDLE_BUTTON_DOWN = 0x0010;  // Middle Button changed to down.
			public const Int32 RI_MOUSE_MIDDLE_BUTTON_UP = 0x0020;  // Middle Button changed to up.
			public const Int32 RI_MOUSE_BUTTON_1_DOWN = RI_MOUSE_LEFT_BUTTON_DOWN;
			public const Int32 RI_MOUSE_BUTTON_1_UP = RI_MOUSE_LEFT_BUTTON_UP;
			public const Int32 RI_MOUSE_BUTTON_2_DOWN = RI_MOUSE_RIGHT_BUTTON_DOWN;
			public const Int32 RI_MOUSE_BUTTON_2_UP = RI_MOUSE_RIGHT_BUTTON_UP;
			public const Int32 RI_MOUSE_BUTTON_3_DOWN = RI_MOUSE_MIDDLE_BUTTON_DOWN;
			public const Int32 RI_MOUSE_BUTTON_3_UP = RI_MOUSE_MIDDLE_BUTTON_UP;
			public const Int32 RI_MOUSE_WHEEL = 0x0400;
			public const Int32 WHEEL_DELTA = 120;


			private ArrayList _devices;

			public RawInput()
			{
				GetRawInputDevices();
			}


			/// <summary>
			/// Returns raw input devices. Standard win32 call in user32.dll.
			/// </summary>
			/// <param name="pRawInputDeviceList"></param>
			/// <param name="puiNumDevices"></param>
			/// <param name="cbSize"></param>
			/// <returns></returns>
			[DllImport("User32.Dll")]
			public static extern Int32 GetRawInputDeviceList(IntPtr pRawInputDeviceList, out Int32 puiNumDevices, Int32 cbSize);

			/// <summary>
			/// Gets information about a raw input device. Used to determine if a device is the windows terminal services (rdp?) mouse.
			/// </summary>
			/// <param name="hDevice"></param>
			/// <param name="uiCommand"></param>
			/// <param name="o"></param>
			/// <param name="pcbSize"></param>
			/// <returns></returns>
			[DllImport("User32.dll", EntryPoint = "GetRawInputDeviceInfo", CharSet = CharSet.Unicode, SetLastError = true)]
			public extern static Int32 GetRawInputDeviceInfo([In] IntPtr hDevice, [In] Int32 uiCommand, [In, Out, MarshalAs(UnmanagedType.AsAny)] Object o, [In, Out] ref Int32 pcbSize);

			/// <summary>
			/// Called with IntPtr.Zero to get size of pData for other calls
			/// </summary>
			/// <param name="hDevice"></param>
			/// <param name="uiCommand"></param>
			/// <param name="pMouseInfo"></param>
			/// <param name="pcbSize"></param>
			/// <returns></returns>
			[DllImport("User32.dll", EntryPoint = "GetRawInputDeviceInfo", CharSet = CharSet.Unicode, SetLastError = true)]
			public extern static Int32 GetRawInputDeviceInfo([In] IntPtr hDevice, [In] Int32 uiCommand, IntPtr pMouseInfo, [In, Out] ref Int32 pcbSize);

			/// <summary>
			/// Retrieves mouse information -- specifically the number of buttons
			/// </summary>
			/// <param name="hDevice"></param>
			/// <param name="uiCommand"></param>
			/// <param name="devInfo"></param>
			/// <param name="pcbSize"></param>
			/// <returns></returns>
			[DllImport("User32.dll", EntryPoint = "GetRawInputDeviceInfo", CharSet = CharSet.Unicode, SetLastError = true)]
			public extern static Int32 GetRawInputDeviceInfo([In] IntPtr hDevice, [In] Int32 uiCommand, [In, Out] ref RID_DEVICE_INFO devInfo, [In, Out] ref Int32 pcbSize);

			/// <summary>
			/// Register for WM_INPUT messages
			/// </summary>
			/// <param name="pcRawInputDevices"></param>
			/// <param name="uiNumDevices"></param>
			/// <param name="cbSize"></param>
			/// <returns></returns>
			[DllImport("User32.dll", EntryPoint = "RegisterRawInputDevices", CharSet = CharSet.Unicode, SetLastError = true)]
			public extern static uint RegisterRawInputDevices([In] IntPtr pcRawInputDevices, [In] uint uiNumDevices, [In] uint cbSize);

			[DllImport("User32.dll", EntryPoint = "GetRawInputData", CharSet = CharSet.Unicode, SetLastError = true)]
			public extern static Int32 GetRawInputData([In] Int32 hRawInput, [In] Int32 uiCommand, IntPtr pRawInput, [In, Out] ref Int32 pcbSize, [In] Int32 cbSizeHeader);
			[DllImport("User32.dll", EntryPoint = "GetRawInputData", CharSet = CharSet.Unicode, SetLastError = true)]
			public extern static Int32 GetRawInputData([In] Int32 hRawInput, [In] Int32 uiCommand, [In, Out] ref RAWINPUT rawInput, [In, Out] ref Int32 pcbSize, [In] Int32 cbSizeHeader);

			/// <summary>
			/// Enumerates the Raw Input Devices and places their corresponding RawInputDevice structures into an ArrayList.
			/// </summary>
			private void GetRawInputDevices()
			{
				ArrayList devices = new ArrayList();
				RAWINPUTDEVICELIST rawInputDevice;
				IntPtr pRawInputDeviceList = IntPtr.Zero;
				Int32 numDevices;
				Int32 rCode = GetRawInputDeviceList(IntPtr.Zero, out numDevices, Marshal.SizeOf(typeof(RAWINPUTDEVICELIST)));

				if (numDevices > 0)
				{
					pRawInputDeviceList = Marshal.AllocHGlobal(numDevices * Marshal.SizeOf(typeof(RAWINPUTDEVICELIST)));
					if (GetRawInputDeviceList(pRawInputDeviceList, out numDevices, Marshal.SizeOf(typeof(RAWINPUTDEVICELIST))) > 0)
					{
						Int32 listIndex;
						for (listIndex = 0; listIndex < numDevices; listIndex++)
						{
							rawInputDevice = (RAWINPUTDEVICELIST)Marshal.PtrToStructure(new IntPtr(pRawInputDeviceList.ToInt32() + (listIndex * Marshal.SizeOf(typeof(RAWINPUTDEVICELIST)))), typeof(RAWINPUTDEVICELIST));
							devices.Add(rawInputDevice);
						}
					}
					Marshal.FreeHGlobal(pRawInputDeviceList);
				}
				_devices = devices;
			}

			public ArrayList Devices
			{
				get
				{
					return _devices;
				}
			}
		}

	#endregion

	#region StructDefs

	/// <summary>
	/// Win32 struct for raw input devices in a list. If dwType == RIM_TYPEMOUSE, our
	/// device is a mouse. hDevice is a handle.
	/// </summary>
	public struct RAWINPUTDEVICELIST
	{
		public IntPtr hDevice;
		public Int32 dwType;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct RID_DEVICE_INFO
	{
		[FieldOffset(0)]
		public uint cbSize;
		[FieldOffset(4)]
		public uint dwType;
		[FieldOffset(8)]
		public RID_DEVICE_INFO_MOUSE mouse;
		[FieldOffset(8)]
		public RID_DEVICE_INFO_KEYBOARD keyboard;
		[FieldOffset(8)]
		public RID_DEVICE_INFO_HID hid;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct RID_DEVICE_INFO_MOUSE
	{
		[FieldOffset(0)]
		public uint dwId;
		[FieldOffset(4)]
		public uint dwNumberOfButtons;
		[FieldOffset(8)]
		public uint dwSampleRate;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct RID_DEVICE_INFO_KEYBOARD
	{
		[FieldOffset(0)]
		public uint dwType;
		[FieldOffset(4)]
		public uint dwSubType;
		[FieldOffset(8)]
		public uint dwKeyboardMode;
		[FieldOffset(12)]
		public uint dwNumberOfFunctionKeys;
		[FieldOffset(16)]
		public uint dwNumberOfIndicators;
		[FieldOffset(20)]
		public uint dwNumberOfKeysTotal;
	}
	[StructLayout(LayoutKind.Explicit)]
	public struct RID_DEVICE_INFO_HID
	{
		[FieldOffset(0)]
		public uint dwVendorId;
		[FieldOffset(4)]
		public uint dwProductId;
		[FieldOffset(8)]
		public uint dwVersionNumber;
		[FieldOffset(12)]
		public ushort usUsagePage;
		[FieldOffset(14)]
		public ushort usUsage;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct RAWINPUTDEVICE
	{
		[FieldOffset(0)]
		public ushort usUsagePage; //Toplevel collection UsagePage
		[FieldOffset(2)]
		public ushort usUsage; //Toplevel collection Usage
		[FieldOffset(4)]
		public uint dwFlags;
		[FieldOffset(8)]
		public uint hwndTarget; // Target hwnd. NULL = follows keyboard focus
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct RAWINPUTHEADER
	{
		[FieldOffset(0)]
		public uint dwType;
		[FieldOffset(4)]
		public uint dwSize;
		[FieldOffset(8)]
		public uint hDevice;
		[FieldOffset(12)]
		public uint wParam;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct RAWINPUT
	{
		[FieldOffset(0)]
		public RAWINPUTHEADER header;
		[FieldOffset(16)]
		public RAWMOUSE mouse;
		[FieldOffset(16)]
		public RAWKEYBOARD keyboard;
		[FieldOffset(16)]
		public RAWHID hid;
	}

	/// <summary>
	/// I had to play with the layout of this one quite a bit. The usFlags field is listed as a USHORT in winuser.h.
	/// Changing it to a uint makes all the fields line up properly for the WM_INPUT messages.
	/// </summary>
	[StructLayout(LayoutKind.Explicit)]
	public struct RAWMOUSE
	{
		[FieldOffset(0)]
		public uint usFlags; //indicator flags
		[FieldOffset(4)]
		public ushort usButtonFlags;
		[FieldOffset(6)]
		public ushort usButtonData;
		[FieldOffset(8)]
		public uint ulRawButtons; //The raw state of the mouse buttons
		[FieldOffset(12)]
		public int lLastX; //The signed relative or absolute motion in the X direction.
		[FieldOffset(16)]
		public int lLastY; //The signed relative or absolute motion in the Y direction.
		[FieldOffset(20)]
		public uint ulExtraInformation; //Device-specific additional information for the event.
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct RAWKEYBOARD
	{
		[FieldOffset(0)]
		public ushort MakeCode; //The "make" scan code (key depression).
		[FieldOffset(2)]
		public ushort Flags; //The flags field indicates a "break" (key release) and other
		//miscellaneous scan code information defined in ntddkbd.h.
		[FieldOffset(4)]
		public ushort Reserved;
		[FieldOffset(6)]
		public ushort VKey; //Windows message compatible information
		[FieldOffset(8)]
		public uint Message;
		[FieldOffset(12)]
		public uint ExtraInformation; //Device-specific additional information for the event.
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct RAWHID
	{
		[FieldOffset(0)]
		uint dwSizeHid;    // byte size of each report
		[FieldOffset(4)]
		uint dwCount;      // number of input packed
		[FieldOffset(8)]
		byte bRawData; //winuser.h has this as BYTE bRawData[1]... should it be
		//uint pbRawData then instead?
	}

	#endregion

	#region RawMouse

	/// <summary>
	/// Maintains state of a given mouse.
	/// </summary>
	public class RawMouse
	{
		private IntPtr _handle;
		private int _x;
		private int _y;
		private int _z;
		private bool[] _buttons;
		private string _name;

		public RawMouse(IntPtr handle, int numButtons, string name)
		{
			_handle = handle;
			_buttons = new bool[numButtons];
			_name = name;
		}

		public int X
		{
			get
			{
				return _x;
			}

			set
			{
				_x = value;
			}
		}

		public int Y
		{
			get
			{
				return _y;
			}

			set
			{
				_y = value;
			}
		}

		public int Z
		{
			get
			{
				return _z;
			}

			set
			{
				_z = value;
			}
		}

		public int YDelta
		{
			get
			{
				int y = _y;
				_y = 0;
				return y;
			}
		}

		public int XDelta
		{
			get
			{
				int x = _x;
				_x = 0;
				return x;
			}
		}

		public int ZDelta
		{
			get
			{
				int z = _z;
				_z = 0;
				return z;
			}
		}

		public IntPtr Handle
		{
			get
			{
				return _handle;
			}
		}

		public bool[] Buttons
		{
			get
			{
				return _buttons;
			}
		}

		public string Name
		{
			get
			{
				return _name;
			}
		}



	}


	#endregion

	#region RawMouseInput

	/// <summary>
	/// Handles raw mouse input. Ignores the system mouse (handle == 0) and the RDP mouse. This uses
	/// the same means to identify the RDP mouse as raw_mouse.c, and those caveats apply. 
	/// </summary>
	public class RawMouseInput : RawInput
	{

		private ArrayList _mice;

		public RawMouseInput()
			: base()
		{
			GetRawInputMice();
		}

		/// <summary>
		/// Gets all the raw mice and initializes the Mice property.
		/// </summary>
		private void GetRawInputMice()
		{
			_mice = new ArrayList();

			foreach (RAWINPUTDEVICELIST d in Devices)
			{
				//skip everything but mice.
				if (d.dwType != RIM_TYPEMOUSE)
				{
					continue;
				}

				//Get length of name.
				Int32 pcbSize = 0;
				GetRawInputDeviceInfo(d.hDevice, RIDI_DEVICENAME, IntPtr.Zero, ref pcbSize);

				//Get name
				StringBuilder sb = new StringBuilder();
				GetRawInputDeviceInfo(d.hDevice, RIDI_DEVICENAME, sb, ref pcbSize);

				//skip windows terminal (rdp?) mouse
				if (sb.ToString().IndexOf(@"\??\Root#RDP_MOU#0000#") < 0)
				{
					//Get size of RID_DEVICE_INFO struct
					GetRawInputDeviceInfo(d.hDevice, RIDI_DEVICEINFO, IntPtr.Zero, ref pcbSize);

					//Get the struct.
					RID_DEVICE_INFO mouseInfo = new RID_DEVICE_INFO();
					//Set cbSize as per docs on GetRawInputDeviceInfo
					mouseInfo.cbSize = (uint)Marshal.SizeOf(typeof(RID_DEVICE_INFO));
					GetRawInputDeviceInfo(d.hDevice, RIDI_DEVICEINFO, ref mouseInfo, ref pcbSize);

					//Create our state container
					RawMouse mouse = new RawMouse(d.hDevice, (int)mouseInfo.mouse.dwNumberOfButtons, sb.ToString());
					_mice.Add(mouse);
				}
			}
		}

		/// <summary>
		/// Registers the application to receive WM_INPUT messages for mice.
		/// </summary>
		/// <param name="hwndTarget">The application's hwnd.</param>
		public void RegisterForWM_INPUT(IntPtr hwndTarget)
		{
			RAWINPUTDEVICE rid = new RAWINPUTDEVICE();
			rid.usUsagePage = 0x01;
			rid.usUsage = 0x02; //mouse
			rid.dwFlags = 0;// RIDEV_NOLEGACY;   // adds HID mouse and also ignores legacy mouse messages
			rid.hwndTarget = (uint)hwndTarget.ToInt32();

			//supposed to be a pointer to an array, we're only registering one device though.
			IntPtr pRawInputDeviceArray = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(RAWINPUTDEVICE)));
			Marshal.StructureToPtr(rid, pRawInputDeviceArray, true);
			uint retval = RegisterRawInputDevices(pRawInputDeviceArray, 1, (uint)Marshal.SizeOf(typeof(RAWINPUTDEVICE)));
			Marshal.FreeHGlobal(pRawInputDeviceArray);
		}

		/// <summary>
		/// Updates the status of the mouse given a raw mouse handle.
		/// </summary>
		/// <param name="dHandle"></param>
		public void UpdateRawMouse(IntPtr dHandle)
		{
			Int32 hRawInput = dHandle.ToInt32();
			Int32 pcbSize = 0;

			//get the size of the raw input struct
			Int32 retval = GetRawInputData(hRawInput, RawInput.RID_INPUT, IntPtr.Zero, ref pcbSize, Marshal.SizeOf(typeof(RAWINPUTHEADER)));

			//get the RAWINPUT structure.
			RAWINPUT ri = new RAWINPUT();
			retval = GetRawInputData(hRawInput, RawInput.RID_INPUT, ref ri, ref pcbSize, Marshal.SizeOf(typeof(RAWINPUTHEADER)));

			foreach (RawMouse mouse in Mice)
			{
				if (mouse.Handle.ToInt32() == (Int32)ri.header.hDevice)
				{
					//Console.WriteLine("usflags: " + ri.mouse.usFlags + " button data: " + ri.mouse.usButtonData);
					//relative mouse
					mouse.X += ri.mouse.lLastX;
					mouse.Y += ri.mouse.lLastY;

					//mouse buttons
					if ((ri.mouse.usButtonFlags & RI_MOUSE_BUTTON_1_DOWN) > 0) mouse.Buttons[0] = true;
					if ((ri.mouse.usButtonFlags & RI_MOUSE_BUTTON_1_UP) > 0) mouse.Buttons[0] = false;
					if ((ri.mouse.usButtonFlags & RI_MOUSE_BUTTON_2_DOWN) > 0) mouse.Buttons[1] = true;
					if ((ri.mouse.usButtonFlags & RI_MOUSE_BUTTON_2_UP) > 0) mouse.Buttons[1] = false;
					if ((ri.mouse.usButtonFlags & RI_MOUSE_BUTTON_3_DOWN) > 0) mouse.Buttons[2] = true;
					if ((ri.mouse.usButtonFlags & RI_MOUSE_BUTTON_3_UP) > 0) mouse.Buttons[2] = false;

					//mouse wheel
					if ((ri.mouse.usButtonFlags & RI_MOUSE_WHEEL) > 0)
					{
						if ((short)ri.mouse.usButtonData > 0)
						{
							mouse.Z++;
						}
						if ((short)ri.mouse.usButtonData < 0)
						{
							mouse.Z--;
						}
					}
				}
			}
		}

		/// <summary>
		/// Returns an ArrayList of RawMouse objects. The system mouse is not included.
		/// </summary>
		public ArrayList Mice
		{
			get
			{
				return _mice;
			}
		}
	}


	#endregion

}
