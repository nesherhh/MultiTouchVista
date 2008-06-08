using System;
using System.Runtime.InteropServices;
using DWMaxxAddIn.Native;
using Microsoft.Win32;

namespace DWMaxxAddIn
{
	class Accelerometer
	{
		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		delegate int GetImmediate(out short x, out short y);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		delegate int GetZeroBase(out short a1, out short a2, out short a3, out short a4);

		GetImmediate getImmediate;
		GetZeroBase getZeroBase;
		short xMin;
		short yMin;
		short xMax;
		short yMax;
		IntPtr tAcelMgrLibrary;

		public Accelerometer()
		{
			RegistryKey tAcelMgrKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\TOSHIBA\\AcelUtil\\TAcelMgr");
			string path = (string)tAcelMgrKey.GetValue("PATH");
			tAcelMgrLibrary = NativeMethods.LoadLibrary(path);

			IntPtr getImmediateFunction = NativeMethods.GetProcAddress(tAcelMgrLibrary, "GetImmediate");
			getImmediate = (GetImmediate)Marshal.GetDelegateForFunctionPointer(getImmediateFunction, typeof(GetImmediate));

			IntPtr getZeroBaseFunction = NativeMethods.GetProcAddress(tAcelMgrLibrary, "GetZeroBase");
			getZeroBase = (GetZeroBase)Marshal.GetDelegateForFunctionPointer(getZeroBaseFunction, typeof(GetZeroBase));

			getZeroBase(out xMin, out yMin, out xMax, out yMax);

			XLength = (short)(XMax - XMin);
			YLength = (short)(YMax - YMin);
		}

		~Accelerometer()
		{
			NativeMethods.FreeLibrary(tAcelMgrLibrary);
		}

		public short XMin
		{
			get { return xMin; }
		}

		public short YMin
		{
			get { return yMin; }
		}

		public short XMax
		{
			get { return xMax; }
		}

		public short YMax
		{
			get { return yMax; }
		}

		public short XLength { get; private set; }
		public short YLength { get; private set; }

		public void ReadData(out short x, out short y)
		{
			getImmediate(out x, out y);

			x = (short)(x - xMin);
			y = (short)(y - yMin);
		}
	}
}
