
// Mike ( lutinore@hotmail.fr ) - May 2007 - RawInput.NET.dll - Win32.cs

using System;
using System.Security;
using System.Runtime.InteropServices;

namespace MultipleMice.Native
{
	[Flags]
	internal enum PenStatus : byte
	{
		PenTipDown = 0x01,
		SideSwitchDown =0x02,
		Inverted = 0x04,
		EraserDown = 0x08,
		Reserved = 0x18,
		InRange = 0x20,
		ControlData = 0x40,
		Sync = 0x80
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct PEN_DATA
	{
		public byte ReportID;
		public PenStatus Status;
		public ushort X;
		public ushort Y;
		public ushort Pressure;
		public byte XTilt;
		public byte YTilt;
		public ushort Firmware;
		public int Data;

		public override string ToString()
		{
			return string.Format("Report ID: {0}, Status: {1}, X-Y: {2}x{3}, Pressure: {4}, Tilt X-Y: {5}x{6}, Firmware: {7}, Data: {8}",
			                     ReportID, Status, X, Y, Pressure, XTilt, YTilt, Firmware, Data);
		}
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct RAWINPUTDEVICELIST
	{
		public IntPtr hDevice;
		public uint dwType;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct RID_DEVICE_INFO_MOUSE
	{
		public uint dwId;
		public uint dwNumberOfButtons;
		public uint dwSampleRate;
		public int fHasHorizontalWheel; // Vista.
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct RID_DEVICE_INFO_KEYBOARD
	{
		public uint dwType;
		public uint dwSubType;
		public uint dwKeyboardMode;
		public uint dwNumberOfFunctionKeys;
		public uint dwNumberOfIndicators;
		public uint dwNumberOfKeysTotal;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct RID_DEVICE_INFO_HID
	{
		public uint dwVendorId;
		public uint dwProductId;
		public uint dwVersionNumber;
		public ushort usUsagePage;
		public ushort usUsage;
	}

	[StructLayout(LayoutKind.Explicit)]
	internal struct RID_DEVICE_INFO
	{
		// Fields
		[FieldOffset(0)]
		public uint cbSize;
		[FieldOffset(4)]
		public uint dwType;
		[FieldOffset(8)]
		public RID_DEVICE_INFO_HID hid;
		[FieldOffset(8)]
		public RID_DEVICE_INFO_KEYBOARD keyboard;
		[FieldOffset(8)]
		public RID_DEVICE_INFO_MOUSE mouse;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct RAWINPUTDEVICE
	{
		public ushort usUsagePage;
		public ushort usUsage;
		public uint dwFlags;
		public IntPtr hwndTarget;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct RAWINPUTHEADER
	{
		public uint dwType;
		public uint dwSize;
		public IntPtr hDevice;
		public IntPtr wParam;
	}

	[StructLayout(LayoutKind.Explicit)]
	internal struct RAWMOUSE
	{
		[FieldOffset(0)]
		public ushort usFlags;
		[FieldOffset(4)] // Alignment.
			public uint ulButtons;
		[FieldOffset(4)] // Union.
			public ushort usButtonFlags;
		[FieldOffset(6)]
		public ushort usButtonData;
		[FieldOffset(8)]
		public uint ulRawButtons;
		[FieldOffset(12)]
		public int lLastX;
		[FieldOffset(16)]
		public int lLastY;
		[FieldOffset(20)]
		public uint ulExtraInformation;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct RAWKEYBOARD
	{
		public ushort MakeCode;
		public ushort Flags;
		public ushort Reserved;
		public ushort VKey;
		public uint Message;
		public uint ExtraInformation;
	}

	[StructLayout(LayoutKind.Sequential)]
	internal struct RAWHID
	{
		public uint dwSizeHid;
		public uint dwCount;
		public byte bRawData; // fixed byte bRawData[ 1 ];
	}

	internal interface IRAWINPUT
	{
		RAWINPUTHEADER Header { get; set; }
		RAWMOUSE Mouse { get; set; }
		RAWKEYBOARD Keyboard { get; set; }
		RAWHID Hid { get; set; }
	}

	[StructLayout(LayoutKind.Explicit)]
	internal struct RAWINPUT32 : IRAWINPUT
	{
		[FieldOffset(0)] public RAWINPUTHEADER header;
		[FieldOffset(16)] public RAWMOUSE mouse;
		[FieldOffset(16)] // Union.
			public RAWKEYBOARD keyboard;
		[FieldOffset(16)] // Union.
			public RAWHID hid;

		public RAWINPUTHEADER Header
		{
			get { return header; }
			set { header = value; }
		}

		public RAWMOUSE Mouse
		{
			get { return mouse; }
			set { mouse = value; }
		}

		public RAWKEYBOARD Keyboard
		{
			get { return keyboard; }
			set { keyboard = value; }
		}

		public RAWHID Hid
		{
			get { return hid; }
			set { hid = value; }
		}
	}

	[StructLayout(LayoutKind.Explicit)]
	internal struct RAWINPUT64 : IRAWINPUT
	{
		[FieldOffset(0)] public RAWINPUTHEADER header;
		[FieldOffset(24)] public RAWMOUSE mouse;
		[FieldOffset(24)] // Union.
			public RAWKEYBOARD keyboard;
		[FieldOffset(24)] // Union.
			public RAWHID hid;

		public RAWINPUTHEADER Header
		{
			get { return header; }
			set { header = value; }
		}

		public RAWMOUSE Mouse
		{
			get { return mouse; }
			set { mouse = value; }
		}

		public RAWKEYBOARD Keyboard
		{
			get { return keyboard; }
			set { keyboard = value; }
		}

		public RAWHID Hid
		{
			get { return hid; }
			set { hid = value; }
		}
	}

	[SuppressUnmanagedCodeSecurity]
	internal static class Win32
	{
		public const int HWND_MESSAGE = -3;
		public const int WM_INPUT = 0x00FF;

		public const int RIM_TYPEMOUSE = 0;
		public const int RIM_TYPEKEYBOARD = 1;
		public const int RIM_TYPEHID = 2;

		//public const int RIDI_PREPARSEDDATA    = 0x20000005;
		public const int RIDI_DEVICENAME = 0x20000007;
		public const int RIDI_DEVICEINFO = 0x2000000b;

		public const int RIDEV_REMOVE = 0x00000001;
		//public const int RIDEV_EXCLUDE      = 0x00000010;
		//public const int RIDEV_PAGEONLY     = 0x00000020;
		//public const int RIDEV_NOLEGACY     = 0x00000030;
		//public const int RIDEV_INPUTSINK    = 0x00000100;
		//public const int RIDEV_CAPTUREMOUSE = 0x00000200;
		//public const int RIDEV_NOHOTKEYS    = 0x00000200;
		//public const int RIDEV_APPKEYS      = 0x00000400;
		//public const int RIDEV_EXINPUTSINK  = 0x00001000;
		//public const int RIDEV_DEVNOTIFY    = 0x00002000;

		//public const int RIM_INPUT      = 0;
		public const int RIM_INPUTSINK = 1;

		public const int RID_INPUT = 0x10000003;
		public const int RID_HEADER = 0x10000005;

		//public const ushort GENERIC_DESKTOP_PAGE    = 0x01;
		//public const ushort MOUSE_USAGE             = 0x02;
		//public const ushort KEYBOARD_USAGE          = 0x06;

		public const int RI_MOUSE_WHEEL = 0x0400;

		public static readonly uint SIZEOF_RAWINPUTDEVICELIST =
			(uint)Marshal.SizeOf(typeof(RAWINPUTDEVICELIST));
		public static readonly uint SIZEOF_RID_DEVICE_INFO =
			(uint)Marshal.SizeOf(typeof(RID_DEVICE_INFO));
		public static readonly uint SIZEOF_RAWINPUTDEVICE =
			(uint)Marshal.SizeOf(typeof(RAWINPUTDEVICE));
		public static readonly uint SIZEOF_RAWINPUTHEADER =
			(uint)Marshal.SizeOf(typeof(RAWINPUTHEADER));
		public static readonly uint SIZEOF_RAWINPUT64 =
			(uint)Marshal.SizeOf(typeof(RAWINPUT64));
		public static readonly uint SIZEOF_RAWINPUT32 =
			(uint)Marshal.SizeOf(typeof(RAWINPUT32));

		[SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern uint GetRawInputDeviceList([In, Out] RAWINPUTDEVICELIST[] ridl, [In, Out] ref uint numDevices, uint sizeInBytes);

		[SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern uint GetRawInputDeviceInfo(IntPtr hDevice, uint command, [In] ref RID_DEVICE_INFO ridInfo, ref uint sizeInBytes);

		[SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern uint GetRawInputDeviceInfo(IntPtr hDevice, uint command, [In] IntPtr ridInfo, ref uint sizeInBytes);
 

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool RegisterRawInputDevices
			(
			IntPtr pRawInputDevices, // [ In ] RAWINPUTDEVICE[ ]
			uint uiNumDevices,
			uint cbSize
			);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern uint GetRegisteredRawInputDevices
			(
			IntPtr pRawInputDevices, // [ In, Out ] RAWINPUTDEVICE[ ]
			ref uint puiNumDevices,
			uint cbSize
			);

		[DllImport("user32.dll")]
		public static extern uint GetRawInputData
			(
			IntPtr hRawInput,
			uint uiCommand,
			IntPtr pData, // [ In, Out ] byte[ ] // ref RAWINPUTHEADER
			ref uint pcbSize,
			uint cbSizeHeader
			);

		[DllImport("user32.dll")]
		public static extern uint GetRawInputData
			(
			IntPtr hRawInput,
			uint uiCommand,
			[Out] out RAWINPUT32 input,
			ref uint pcbSize,
			uint cbSizeHeader
			);

		[DllImport("user32.dll")]
		public static extern uint GetRawInputData
			(
			IntPtr hRawInput,
			uint uiCommand,
			[Out] out RAWINPUT64 input,
			ref uint pcbSize,
			uint cbSizeHeader
			);

		[StructLayout(LayoutKind.Sequential)]
		public class POINT
		{
			public int x;
			public int y;
			public POINT()
			{
			}

			public POINT(int x, int y)
			{
				this.x = x;
				this.y = y;
			}
		}

		[DllImport("user32.dll")]
		public static extern uint SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int nWidth, int nHeight, uint uFlags);

		public const UInt32 SWP_NOSIZE = 0x0001;
		public const UInt32 SWP_NOMOVE = 0x0002;
		public const UInt32 SWP_NOZORDER = 0x0004;
		public const UInt32 SWP_NOREDRAW = 0x0008;
		public const UInt32 SWP_NOACTIVATE = 0x0010;
		public const UInt32 SWP_FRAMECHANGED = 0x0020;  /* The frame changed: send WM_NCCALCSIZE */
		public const UInt32 SWP_SHOWWINDOW = 0x0040;
		public const UInt32 SWP_HIDEWINDOW = 0x0080;
		public const UInt32 SWP_NOCOPYBITS = 0x0100;
		public const UInt32 SWP_NOOWNERZORDER = 0x0200;  /* Don't do owner Z ordering */
		public const UInt32 SWP_NOSENDCHANGING = 0x0400;  /* Don't send WM_WINDOWPOSCHANGING */


		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("User32.dll")]
		internal static extern bool SetCursorPos(int X, int Y);

		[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		static extern bool GetCursorPos([In, Out] POINT pt);

		public static POINT GetCursorPosition()
		{
			POINT result = new POINT();
			GetCursorPos(result);
			return result;
		}
	}
}

// Wrapper.Windows