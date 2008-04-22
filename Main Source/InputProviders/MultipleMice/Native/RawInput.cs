// Mike ( lutinore@hotmail.fr ) - May 2007 - RawInput.NET.dll - RawInput.cs

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Win32;

namespace MultipleMice.Native
{
	/// <summary>
	/// Type of raw device.
	/// </summary>
	public enum RawType
	{
		/// <summary>
		/// 
		/// </summary>
		Unknown = 0,
		/// <summary>
		/// 
		/// </summary>
		Mouse = 1,
		/// <summary>
		/// 
		/// </summary>
		Keyboard = 2,
		/// <summary>
		/// Input device that is not a keyboard and not a mouse.
		/// </summary>
		Device = 3
	}

	/// <summary>
	/// 
	/// </summary>
	[Flags]
	public enum InputMode
	{
		/// <summary>
		/// 
		/// </summary>
		None = 0,
		/// <summary>
		/// If set for a mouse or a keyboard, the system does not generate
		/// any legacy message (WM_*) for that device for the application.
		/// </summary>
		/// <remarks>
		/// Only for the mouse and keyboard.
		/// </remarks>
		SuppressMessages = 0x00000030,
		/// <summary>
		/// If set, this enables the application to receive the input
		/// even when the application is not in the foreground.
		/// </summary>
		BackgroundMode = 0x00000100,
		/// <summary>
		/// If set, the mouse button click does not activate the other window.
		/// </summary>
		/// <remarks>
		/// Effective when mouse SuppressMessages is specified.
		/// </remarks>
		CaptureMouse = 0x00000200,
		/// <summary>
		/// If set, the application-defined keyboard device hotkeys are not handled.
		/// </summary>
		/// <remarks>
		/// Effective for keyboard.
		/// </remarks>
		SuppressHotKeys = 0x00000200,
		/// <summary>
		/// If set, the application command keys are handled.
		/// </summary>
		/// <remarks>
		/// Effective when keyboard SuppressMessages is specified.
		/// <para>
		/// Microsoft Windows XP Service Pack 1 (SP1).
		/// </para>
		/// </remarks>
		ApplicationKeys = 0x00000400, // XPSP1.
		/// <summary>
		/// If set, this enables the application to receive input in the background
		/// only if the foreground application does not process it.
		/// </summary>
		/// <remarks>
		/// Microsoft Windows Vista.
		/// </remarks>
		ExclusiveMode = 0x00001000, // Vista.
	}

	/// <summary>
	/// 
	/// </summary>
	[Flags]
	public enum MouseState
	{
		/// <summary>
		/// Mouse movement data is relative to the last mouse position.
		/// </summary>
		RelativeMode = 0x00,
		/// <summary>
		/// Mouse movement data is based on absolute position.
		/// </summary>
		AbsoluteMode = 0x01,
		/// <summary>
		/// The coordinates are mapped to the virtual desktop (multiple monitor).
		/// </summary>
		VirtualDesktop = 0x02,
		/// <summary>
		/// Mouse attributes changed, requery for mouse attributes.
		/// </summary>
		AttributesChanged = 0x04,
		/// <summary>
		/// Do not coalesce mouse moves.
		/// </summary>
		/// <remarks>
		/// Microsoft Windows Vista.
		/// </remarks>
		NoCoalesce = 0x08
	}

	/// <summary>
	/// 
	/// </summary>
	[Flags]
	public enum MouseButtonState
	{
		/// <summary>
		/// 
		/// </summary>
		None = 0,
		/// <summary>
		/// 
		/// </summary>
		LeftDown = 0x0001,
		/// <summary>
		/// 
		/// </summary>
		LeftUp = 0x0002,
		/// <summary>
		/// 
		/// </summary>
		RightDown = 0x0004,
		/// <summary>
		/// 
		/// </summary>
		RightUp = 0x0008,
		/// <summary>
		/// 
		/// </summary>
		MiddleDown = 0x0010,
		/// <summary>
		/// 
		/// </summary>
		MiddleUp = 0x0020,
		/// <summary>
		/// The button 4 was pressed.
		/// </summary>
		XButton1Down = 0x0040,
		/// <summary>
		/// The button 4 was released.
		/// </summary>
		XButton1Up = 0x0080,
		/// <summary>
		/// The button 5 was pressed.
		/// </summary>
		XButton2Down = 0x0100,
		/// <summary>
		/// The button 5 was released.
		/// </summary>
		XButton2Up = 0x0200
	}

	/// <summary>
	/// 
	/// </summary>
	public enum KeyState
	{
		/// <summary>
		/// 
		/// </summary>
		None = 0,
		/// <summary>
		/// 
		/// </summary>
		KeyDown = 0x0100,
		/// <summary>
		/// 
		/// </summary>
		KeyUp = 0x0101,
		/// <summary>
		/// A key was pressed while the ALT key was held down.
		/// </summary>
		SysKeyDown = 0x0104,
		/// <summary>
		/// A key was released while the ALT key was held down.
		/// </summary>
		SysKeyUp = 0x0105
	}

	/// <summary>
	/// Human Interface Device (HID), mouse, keyboard, joystick, remote control ..
	/// </summary>
	public sealed class RawDevice
	{
		private static MessageOnlyWindow messageOnlyWindow;

		private readonly IntPtr hDevice = IntPtr.Zero;
		private readonly RawType rawType = RawType.Unknown;

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		/// Available only when the application calls RegisterRawDevices.
		/// </remarks>
		public static event EventHandler<RawInputEventArgs> RawInput;

		/// <summary>
		/// Handle to the raw input device.
		/// </summary>
		/// <remarks>
		/// Unique identifier for the device.
		/// </remarks>
		public IntPtr Handle
		{
			get { return hDevice; }
		}

		/// <summary>
		/// Type of raw device.
		/// </summary>
		public RawType RawType
		{
			get { return rawType; }
		}

		/// <summary>
		/// Retrieves description about the device.
		/// </summary>
		public string Description
		{
			get { return GetDeviceDesc(); }
		}

		internal RawDevice(IntPtr hDevice, uint dwType)
		{
			this.hDevice = hDevice;
			rawType = (RawType)(dwType + 1);
		}

		/// <summary>
		/// Gets the raw input devices attached to the system.
		/// </summary>
		/// <returns></returns>
		public static unsafe RawDevice[] GetRawDevices()
		{
			uint numDevices = 0;

			uint res = Win32.GetRawInputDeviceList(IntPtr.Zero,
			                                       ref numDevices, Win32.SIZEOF_RAWINPUTDEVICELIST);

			if (res != 0)
				throw new Win32Exception(Marshal.GetLastWin32Error());

			List<RawDevice> deviceList = new List<RawDevice>((int)numDevices);

			if (numDevices > 0)
			{
				RAWINPUTDEVICELIST[] rawDevices = new RAWINPUTDEVICELIST[numDevices];

				fixed (RAWINPUTDEVICELIST* pRawDevices = rawDevices)
				{
					res = Win32.GetRawInputDeviceList((IntPtr)pRawDevices,
					                                  ref numDevices, Win32.SIZEOF_RAWINPUTDEVICELIST);

					if (res != numDevices)
						throw new Win32Exception(Marshal.GetLastWin32Error());
				}

				foreach (RAWINPUTDEVICELIST rawDevice in rawDevices)
				{
					if (rawDevice.dwType == Win32.RIM_TYPEMOUSE)
					{
						// Terminal Server virtual mouse device.
						if (GetDeviceName(rawDevice.hDevice).Contains("RDP_MOU"))
							continue;
					}
					else if (rawDevice.dwType == Win32.RIM_TYPEKEYBOARD)
					{
						// Terminal Server virtual keyboard device.
						if (GetDeviceName(rawDevice.hDevice).Contains("RDP_KBD"))
							continue;
					}

					deviceList.Add(new RawDevice(rawDevice.hDevice, rawDevice.dwType));
				}
			}

			return deviceList.ToArray(); // Will never return a null reference.
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="usagePage"></param>
		/// <param name="usage"></param>
		/// <returns></returns>
		public static bool RegisterRawDevices(int usagePage, int usage)
		{
			return RegisterRawDevices(usagePage, usage, InputMode.None);
		}

		/// <summary>
		/// Registers the devices.
		/// </summary>
		/// <remarks>
		/// An application must first register the raw input devices.
		/// By default, an application does not receive raw input.
		/// <para>
		/// For the mouse, the Generic Desktop Usage Page is 0x01 and the Usage ID is 0x02.
		/// For the keyboard, the Generic Desktop Usage Page is 0x01 and the Usage ID is 0x06.
		/// </para>
		/// </remarks>
		/// <param name="usagePage"></param>
		/// <param name="usage"></param>
		/// <param name="mode"></param>
		/// <returns></returns>
		public static bool RegisterRawDevices(int usagePage, int usage, InputMode mode)
		{
			if (usagePage < UInt16.MinValue || usagePage > UInt16.MaxValue)
				throw new ArgumentOutOfRangeException("usagePage");

			if (usage < UInt16.MinValue || usage > UInt16.MaxValue)
				throw new ArgumentOutOfRangeException("usage");

			if (((int)mode & Win32.RIDEV_REMOVE) == Win32.RIDEV_REMOVE)
				throw new ArgumentOutOfRangeException("mode");

			//if ( ( ( int )mode & Win32.RIDEV_PAGEONLY ) == Win32.RIDEV_PAGEONLY )
			//    throw new ArgumentOutOfRangeException( "mode" );

			//if ( ( ( int )mode & Win32.RIDEV_EXCLUDE ) == Win32.RIDEV_EXCLUDE )
			//    throw new ArgumentOutOfRangeException( "mode" );

			return RegisterRawDevice((ushort)usagePage, (ushort)usage, (uint)mode);
		}

		/// <summary>
		/// Unregisters the devices.
		/// </summary>
		/// <param name="usagePage"></param>
		/// <param name="usage"></param>
		/// <returns></returns>
		public static bool UnregisterRawDevices(int usagePage, int usage)
		{
			if (usagePage < UInt16.MinValue || usagePage > UInt16.MaxValue)
				throw new ArgumentOutOfRangeException("usagePage");

			if (usage < UInt16.MinValue || usage > UInt16.MaxValue)
				throw new ArgumentOutOfRangeException("usage");

			return RegisterRawDevice((ushort)usagePage, (ushort)usage, Win32.RIDEV_REMOVE);
		}

		private static unsafe bool RegisterRawDevice(ushort usagePage, ushort usage, uint flags)
		{
			RAWINPUTDEVICE rawDevice;
			rawDevice.usUsagePage = usagePage;
			rawDevice.usUsage = usage;
			rawDevice.dwFlags = flags;

			if ((flags & Win32.RIDEV_REMOVE) != Win32.RIDEV_REMOVE)
			{
				if (messageOnlyWindow == null)
					messageOnlyWindow = new MessageOnlyWindow();

				rawDevice.hwndTarget = messageOnlyWindow.Handle;
			}
			else
			{
				rawDevice.hwndTarget = IntPtr.Zero; // Must be IntPtr.Zero if RIDEV_REMOVE is set.
			}

			bool registered = Win32.RegisterRawInputDevices((IntPtr)(&rawDevice),
			                                                1U, Win32.SIZEOF_RAWINPUTDEVICE);

#if DEBUG
			if (!registered)
				throw new Win32Exception(Marshal.GetLastWin32Error());
#endif // DEBUG

			uint numDevices = unchecked((uint)-1); // UInt32.MaxValue.

			uint res = Win32.GetRegisteredRawInputDevices(IntPtr.Zero,
			                                              ref numDevices, Win32.SIZEOF_RAWINPUTDEVICE);

#if DEBUG
			if (res != 0)
				throw new Win32Exception(Marshal.GetLastWin32Error());
#endif // DEBUG

			if (numDevices == 0)
			{
				if (messageOnlyWindow != null)
				{
					messageOnlyWindow.Dispose();
					messageOnlyWindow = null;
				}
			}

			return registered;
		}

		// Gets the hardware ID.
		private static unsafe string GetDeviceName(IntPtr hDevice)
		{
			uint charCount = 0;

			uint res = Win32.GetRawInputDeviceInfo(hDevice,
			                                       Win32.RIDI_DEVICENAME, IntPtr.Zero, ref charCount);

#if DEBUG
			if (res != 0)
				throw new Win32Exception(Marshal.GetLastWin32Error());
#endif // DEBUG

			string deviceName = String.Empty;

			if (charCount > 0)
			{
				char* pName = stackalloc char[(int)charCount + 1];
				pName[0] = '\0';

				res = Win32.GetRawInputDeviceInfo(hDevice,
				                                  Win32.RIDI_DEVICENAME, (IntPtr)pName, ref charCount);

#if DEBUG
				if (res != charCount)
					throw new Win32Exception(Marshal.GetLastWin32Error());
#endif // DEBUG

				deviceName = new string(pName);
			}

			return deviceName; // Will never return a null reference.
		}

		private string GetDeviceDesc()
		{
			string deviceDesc = String.Empty;

			try
			{
				string deviceName = GetDeviceName(hDevice);

				if (deviceName != String.Empty)
				{
					deviceName = deviceName.Remove(deviceName.LastIndexOf('#'));
					deviceName = deviceName.Substring(deviceName.LastIndexOf('\\') + 1);
					deviceName = deviceName.Replace('#', '\\');
					deviceName = "HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Enum\\" + deviceName;
					deviceName = (string)Registry.GetValue(deviceName, "DeviceDesc", String.Empty);
					deviceDesc = deviceName.Substring(deviceName.LastIndexOf(';') + 1);
				}
			}
			catch
			{
#if DEBUG
				throw;
#endif // DEBUG
			}

			return deviceDesc; // Will never return a null reference.
		}

		/// <summary>
		/// Gets information about the raw input device.
		/// </summary>
		/// <returns></returns>
		public RawInfo GetRawInfo()
		{
			switch (rawType)
			{
				case RawType.Mouse:
					return new MouseInfo(hDevice);
				case RawType.Keyboard:
					return new KeyboardInfo(hDevice);
				case RawType.Device:
					return new DeviceInfo(hDevice);
				default:
					throw new InvalidOperationException("Unknown device type.");
			}
		}

		private static bool ProcessRawInput(IntPtr hRawInput)
		{
			if (RawInput != null)
			{
				RawInputEventArgs args;

				try
				{
					args = new RawInputEventArgs(hRawInput);
				}
				catch
				{
#if DEBUG
					throw;
#else
                    return false;
#endif // DEBUG
				}

				RawInput(null, args);
				return true;
			}

			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			RawDevice rawDevice = obj as RawDevice;

			if (ReferenceEquals(rawDevice, null))
				return false;
			return hDevice == rawDevice.hDevice;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static bool operator ==(RawDevice left, RawDevice right)
		{
			if (ReferenceEquals(left, null))
				return ReferenceEquals(right, null);

			return left.Equals(right);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static bool operator !=(RawDevice left, RawDevice right)
		{
			return !(left == right);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static bool Equals(RawDevice left, RawDevice right) // CLS-compliant.
		{
			return left == right;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return (int)((long)hDevice ^ ((long)hDevice >> 32)); // IntPtr is 32 or 64 bits.
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return GetDeviceDesc();
		}

		// A message-only window is not visible, has no z-order, cannot be enumerated,
		// and does not receive broadcast messages. The window simply dispatches messages.
		private sealed class MessageOnlyWindow : NativeWindow, IDisposable
		{
			public MessageOnlyWindow()
			{
				CreateParams cp = new CreateParams();
				cp.Parent = (IntPtr)Win32.HWND_MESSAGE;
				CreateHandle(cp);

				Debug.WriteLine("The message only window is created.");
			}

			~MessageOnlyWindow()
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
				//if ( disposing )
				//{

				//}

				if (Handle != IntPtr.Zero)
					DestroyHandle();

				Debug.WriteLine("The message only window is destroyed.");
			}

			protected override void WndProc(ref Message m)
			{
				if (m.Msg == Win32.WM_INPUT)
				{
					if (ProcessRawInput(m.LParam))
					{
						m.Result = IntPtr.Zero;
						return; // Handled.
					}
				}

				base.WndProc(ref m);
			}
		} // MessageOnlyWindow
	}

	/// <summary>
	/// 
	/// </summary>
	public abstract class RawInfo
	{
		internal RID_DEVICE_INFO info;

		/// <summary>
		/// HID top level collection Usage Page.
		/// </summary>
		public abstract int UsagePage
		{
			get;
		}

		/// <summary>
		/// HID top level collection Usage ID.
		/// </summary>
		public abstract int Usage
		{
			get;
		}

		internal unsafe RawInfo(IntPtr hDevice)
		{
			info.cbSize = Win32.SIZEOF_RID_DEVICE_INFO;
			uint dataSize = Win32.SIZEOF_RID_DEVICE_INFO;

			fixed (RID_DEVICE_INFO* pInfo = &info)
			{
				uint res = Win32.GetRawInputDeviceInfo(hDevice,
				                                       Win32.RIDI_DEVICEINFO, (IntPtr)pInfo, ref dataSize);

				if (res == unchecked((uint)-1)) // UInt32.MaxValue;
					throw new Win32Exception(Marshal.GetLastWin32Error());
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public sealed class MouseInfo : RawInfo
	{
		/// <summary>
		/// ID for the mouse device.
		/// </summary>
		public int Id
		{
			get { return (int)info.mouse.dwId; }
		}

		/// <summary>
		/// Number of buttons for the mouse.
		/// </summary>
		public int NumberButtons
		{
			get { return (int)info.mouse.dwNumberOfButtons; }
		}

		/// <summary>
		/// Number of data points per second.
		/// </summary>
		/// <remarks>
		/// This information may not be applicable for every mouse device.
		/// </remarks>
		public int SampleRate
		{
			get { return (int)info.mouse.dwSampleRate; }
		}

		/// <summary>
		/// True if the mouse has a wheel for horizontal scrolling.
		/// </summary>
		/// <remarks>
		/// Microsoft Windows Vista.
		/// </remarks>
		public bool HasHorizontalWheel
		{
			get
			{
				if (Environment.OSVersion.Version.Major < 6)
					throw new NotSupportedException
						("This property is only supported under " +
						 "Microsoft Windows Vista and later versions.");

				return (info.mouse.fHasHorizontalWheel != 0);
			}
		}

		/// <summary>
		/// The Generic Desktop Usage Page.
		/// </summary>
		public override int UsagePage
		{
			get { return 0x01; }
		}

		/// <summary>
		/// The mouse usage ID.
		/// </summary>
		public override int Usage
		{
			get { return 0x02; }
		}

		internal MouseInfo(IntPtr hDevice)
			: base(hDevice)
		{
			// ..
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public sealed class KeyboardInfo : RawInfo
	{
		/// <summary>
		/// Retrieves description about the keyboard.
		/// </summary>
		/// <remarks>
		/// The return value can be an empty string.
		/// </remarks>
		public string KeyboardDescription
		{
			get
			{
				switch (info.keyboard.dwType)
				{
					case 1: return "IBM PC/XT or compatible (83-key) keyboard";
					case 2: return "Olivetti ICO (102-key) keyboard";
					case 3: return "IBM PC/AT (84-key) or similar keyboard";
					case 4: return "IBM enhanced (101- or 102-key) keyboard";
					case 5: return "Nokia 1050 and similar keyboards";
					case 6: return "Nokia 9140 and similar keyboards";
					case 7: return "Japanese keyboard";
					case 81: return "USB keyboard";
					default: return String.Empty;
				}
			}
		}

		/// <summary>
		/// Type of the keyboard.
		/// </summary>
		public int KeyboardType
		{
			get { return (int)info.keyboard.dwType; }
		}

		/// <summary>
		/// The subtype is an original equipment manufacturer (OEM) dependent value.
		/// </summary>
		public int KeyboardSubtype
		{
			get { return (int)info.keyboard.dwSubType; }
		}

		/// <summary>
		/// Scan code mode.
		/// </summary>
		public int ScanCodeMode
		{
			get { return (int)info.keyboard.dwKeyboardMode; }
		}

		/// <summary>
		/// Number of function keys on the keyboard.
		/// </summary>
		public int NumberFunctionKeys
		{
			get { return (int)info.keyboard.dwNumberOfFunctionKeys; }
		}

		/// <summary>
		/// Number of LED indicators on the keyboard.
		/// </summary>
		public int NumberIndicators
		{
			get { return (int)info.keyboard.dwNumberOfIndicators; }
		}

		/// <summary>
		/// Number of keys on the keyboard.
		/// </summary>
		public int NumberKeys
		{
			get { return (int)info.keyboard.dwNumberOfKeysTotal; }
		}

		/// <summary>
		/// The Generic Desktop Usage Page.
		/// </summary>
		public override int UsagePage
		{
			get { return 0x01; }
		}

		/// <summary>
		/// The keyboard usage ID.
		/// </summary>
		public override int Usage
		{
			get { return 0x06; }
		}

		internal KeyboardInfo(IntPtr hDevice)
			: base(hDevice)
		{
			// ..
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public sealed class DeviceInfo : RawInfo
	{
		/// <summary>
		/// Vendor ID for the HID.
		/// </summary>
		public int VendorId
		{
			get { return (int)info.hid.dwVendorId; }
		}

		/// <summary>
		/// Product ID for the HID.
		/// </summary>
		public int ProductId
		{
			get { return (int)info.hid.dwProductId; }
		}

		/// <summary>
		/// Version number for the HID.
		/// </summary>
		public int Version
		{
			get { return (int)info.hid.dwVersionNumber; }
		}

		/// <summary>
		/// HID top level collection Usage Page.
		/// </summary>
		public override int UsagePage
		{
			get { return info.hid.usUsagePage; }
		}

		/// <summary>
		/// HID top level collection Usage ID.
		/// </summary>
		public override int Usage
		{
			get { return info.hid.usUsage; }
		}

		internal DeviceInfo(IntPtr hDevice)
			: base(hDevice)
		{
			// ..
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public sealed class RawInputEventArgs : EventArgs
	{
		private IntPtr hRawInput = IntPtr.Zero;
		private RAWINPUTHEADER header;

		/// <summary>
		/// Handle to the device generating the raw input data.
		/// </summary>
		/// <remarks>
		/// Returns zero if an application inserts input data, for example, by using SendInput.
		/// </remarks>
		public IntPtr Handle
		{
			get { return header.hDevice; }
		}

		/// <summary>
		/// Type of the device generating the raw input data.
		/// </summary>
		public RawType RawType
		{
			get { return (RawType)(header.dwType + 1); }
		}

		/// <summary>
		/// If true, input occurred while the application was in the background.
		/// </summary>
		public bool IsBackground
		{
			get { return ((int)header.wParam & 0xff) == Win32.RIM_INPUTSINK; }
		}

		internal unsafe RawInputEventArgs(IntPtr hRawInput)
		{
			this.hRawInput = hRawInput;

			uint dataSize = Win32.SIZEOF_RAWINPUTHEADER;

			fixed (RAWINPUTHEADER* pHeader = &header)
			{
				uint res = Win32.GetRawInputData(hRawInput, Win32.RID_HEADER,
				                                 (IntPtr)pHeader, ref dataSize, Win32.SIZEOF_RAWINPUTHEADER);

				if (res != dataSize)
					throw new Win32Exception(Marshal.GetLastWin32Error());
			}
		}

		/// <summary>
		/// Device generating the raw input data.
		/// </summary>
		/// <remarks>
		/// Returns null if an application inserts input data, for example, by using SendInput.
		/// </remarks>
		/// <returns></returns>
		public RawDevice GetRawDevice()
		{
			if (header.hDevice == IntPtr.Zero)
				return null;
			return new RawDevice(header.hDevice, header.dwType);
		}

		/// <summary>
		/// Gets the raw input data.
		/// </summary>
		/// <returns></returns>
		public RawData GetRawData()
		{
			switch (header.dwType)
			{
				case Win32.RIM_TYPEMOUSE:
					return new MouseData(hRawInput);
				case Win32.RIM_TYPEKEYBOARD:
					return new KeyboardData(hRawInput);
				case Win32.RIM_TYPEHID:
					return new DeviceData(hRawInput, header.dwSize);
				default:
					throw new InvalidOperationException("Unknown device type.");
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public abstract class RawData
	{
		internal RawData()
		{
			// ..
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public sealed class MouseData : RawData
	{
		private RAWINPUT input;

		/// <summary>
		/// Information about the state of the mouse.
		/// </summary>
		public MouseState State
		{
			get { return (MouseState)input.mouse.usFlags; }
		}

		/// <summary>
		/// Motion in the X direction.
		/// </summary>
		/// <remarks>
		/// Signed relative motion or absolute motion, depending on the value of MouseData.State.
		/// </remarks>
		public int X
		{
			get { return input.mouse.lLastX; }
		}

		/// <summary>
		/// Motion in the Y direction.
		/// </summary>
		/// <remarks>
		/// Signed relative motion or absolute motion, depending on the value of MouseData.State.
		/// </remarks>
		public int Y
		{
			get { return input.mouse.lLastY; }
		}

		/// <summary>
		/// The wheel delta (Z).
		/// </summary>
		public int Wheel
		{
			get { return (short)input.mouse.usButtonData; } // Signed value.
		}

		/// <summary>
		/// State of the mouse buttons.
		/// </summary>
		public MouseButtonState ButtonState
		{
			get { return (MouseButtonState)(input.mouse.usButtonFlags & ~Win32.RI_MOUSE_WHEEL); }
		}

		/// <summary>
		/// Raw state of the mouse buttons.
		/// </summary>
		public long RawButtonState
		{
			get { return input.mouse.ulRawButtons; }
		}

		internal unsafe MouseData(IntPtr hRawInput)
		{
			uint dataSize = Win32.SIZEOF_RAWINPUT;

			fixed (RAWINPUT* pInput = &input)
			{
				uint res = Win32.GetRawInputData(hRawInput, Win32.RID_INPUT,
				                                 (IntPtr)pInput, ref dataSize, Win32.SIZEOF_RAWINPUTHEADER);

				if (res == unchecked((uint)-1)) // UInt32.MaxValue;
					throw new Win32Exception(Marshal.GetLastWin32Error());
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public sealed class KeyboardData : RawData
	{
		private RAWINPUT input;

		/// <summary>
		/// Scan code.
		/// </summary>
		/// <remarks>
		/// The scan code for keyboard overrun :
		/// 
		/// KEYBOARD_OVERRUN_MAKE_CODE = 0xFF
		/// </remarks>
		public int MakeCode
		{
			get { return input.keyboard.MakeCode; }
		}

		/// <summary>
		/// Scan code information.
		/// </summary>
		/// <remarks>
		/// It can be one or more of the following values :
		/// 
		/// KEY_MAKE    = 0,
		/// KEY_BREAK   = 1,
		/// KEY_E0      = 2,
		/// KEY_E1      = 4
		/// </remarks>
		public int ScanCode
		{
			get { return input.keyboard.Flags; }
		}

		/// <summary>
		/// Virtual-key code.
		/// </summary>
		public Keys Key
		{
			get { return (Keys)input.keyboard.VKey; }
		}

		/// <summary>
		/// Corresponding window message (WM_*).
		/// </summary>
		public KeyState KeyState
		{
			get { return (KeyState)input.keyboard.Message; }
		}

		internal unsafe KeyboardData(IntPtr hRawInput)
		{
			uint dataSize = Win32.SIZEOF_RAWINPUT;

			fixed (RAWINPUT* pInput = &input)
			{
				uint res = Win32.GetRawInputData(hRawInput, Win32.RID_INPUT,
				                                 (IntPtr)pInput, ref dataSize, Win32.SIZEOF_RAWINPUTHEADER);

				if (res == unchecked((uint)-1)) // UInt32.MaxValue;
					throw new Win32Exception(Marshal.GetLastWin32Error());
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <remarks>
	/// You should call Dispose after you have used it.
	/// </remarks>
	public sealed class DeviceData : RawData, IDisposable
	{
		private IntPtr hGlobal = IntPtr.Zero;

		/// <summary>
		/// 
		/// </summary>
		public bool IsDisposed
		{
			get { return hGlobal == IntPtr.Zero; }
		}

		internal DeviceData(IntPtr hRawInput, uint dwSize)
		{
			hGlobal = Marshal.AllocHGlobal((IntPtr)dwSize);

			uint res = Win32.GetRawInputData(hRawInput, Win32.RID_INPUT,
			                                 hGlobal, ref dwSize, Win32.SIZEOF_RAWINPUTHEADER);

			if (res != dwSize)
			{
				if (hGlobal != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(hGlobal);
					hGlobal = IntPtr.Zero;
				}

				throw new Win32Exception(Marshal.GetLastWin32Error());
			}
		}

		/// <summary>
		/// 
		/// </summary>
		~DeviceData()
		{
			Dispose(false);
		}

		/// <summary>
		/// Frees the unmanaged (native) memory block containing the raw data.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			//if ( disposing )
			//{

			//}

			if (hGlobal != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(hGlobal);
				hGlobal = IntPtr.Zero;
			}
		}

		/// <summary>
		/// Gets the address of the unmanaged (native) memory block containing the raw data.
		/// </summary>
		/// <remarks>
		/// Each RawInput event can indicate several inputs from the same device. 
		/// <para>
		/// The size of the memory block is DataSize * DataCount.
		/// </para>
		/// </remarks>
		/// <param name="dataSize">Size, in bytes, of each raw input.</param>
		/// <param name="dataCount">Number of raw inputs.</param>
		/// <returns></returns>
		public unsafe IntPtr GetDataPtr(out int dataSize, out int dataCount)
		{
			if (hGlobal == IntPtr.Zero)
				throw new ObjectDisposedException("DeviceData");

			RAWINPUT* pInput = (RAWINPUT*)hGlobal;
			dataSize = (int)pInput->hid.dwSizeHid;
			dataCount = (int)pInput->hid.dwCount;
			return (IntPtr)(&pInput->hid.bRawData);
		}

		/// <summary>
		/// Returns a copy of the unmanaged (native) memory block containing the raw data.
		/// </summary>
		/// <remarks>
		/// Each dimension indicate several inputs from the same device.
		/// </remarks>
		/// <returns></returns>
		public unsafe byte[,] GetDataBuffer()
		{
			int dataSize;
			int dataCount;
			byte* pData = (byte*)GetDataPtr(out dataSize, out dataCount);

			byte[,] buffer = new byte[dataCount, dataSize];
			int bufferSize = dataSize * dataCount;

			fixed (byte* pBuffer = buffer)
			{
				int i = 0;

				while (i < bufferSize)
				{
					pBuffer[i] = pData[i];
					i++;
				}
			}

			return buffer;
		}
	}
}