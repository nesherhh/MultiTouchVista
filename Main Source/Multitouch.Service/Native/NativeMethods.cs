using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Multitouch.Service.Native
{
	class NativeMethods
	{
		public const uint CREATE_UNICODE_ENVIRONMENT = 0x00000400;

		/// <summary>
		/// The WTSGetActiveConsoleSessionId function retrieves the 
		/// Terminal Services session currently attached to the physical console. 
		/// The physical console is the monitor, keyboard, and mouse.
		/// </summary>
		/// <returns>An <see cref="int"/> equal to 0 indicates that the current session is attached to the physical console.</returns>
		/// <remarks>It is not necessary that Terminal Services be running for this function to succeed.
		/// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/termserv/termserv/wtsgetactiveconsolesessionid.asp</remarks>
		[DllImport("kernel32.dll")]
		public static extern int WTSGetActiveConsoleSessionId();

		[DllImport("wtsapi32.dll", SetLastError = true)]
		public static extern bool WTSQueryUserToken(UInt32 sessionId, out IntPtr Token);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool CloseHandle(IntPtr hHandle);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public extern static bool DuplicateTokenEx(
			IntPtr hExistingToken,
			ACCESS_MASK dwDesiredAccess,
			ref SECURITY_ATTRIBUTES lpTokenAttributes,
			SECURITY_IMPERSONATION_LEVEL ImpersonationLevel,
			TOKEN_TYPE TokenType,
			out IntPtr phNewToken);

		[StructLayout(LayoutKind.Sequential)]
		public struct SECURITY_ATTRIBUTES
		{
			public int nLength;
			public IntPtr lpSecurityDescriptor;
			public int bInheritHandle;
		}

		public enum SECURITY_IMPERSONATION_LEVEL
		{
			SecurityAnonymous,
			SecurityIdentification,
			SecurityImpersonation,
			SecurityDelegation
		}

		public enum TOKEN_TYPE
		{
			TokenPrimary = 1,
			TokenImpersonation
		}

		[Flags]
		public enum ACCESS_MASK : uint
		{
			DELETE = 0x00010000,
			READ_CONTROL = 0x00020000,
			WRITE_DAC = 0x00040000,
			WRITE_OWNER = 0x00080000,
			SYNCHRONIZE = 0x00100000,

			STANDARD_RIGHTS_REQUIRED = 0x000f0000,

			STANDARD_RIGHTS_READ = 0x00020000,
			STANDARD_RIGHTS_WRITE = 0x00020000,
			STANDARD_RIGHTS_EXECUTE = 0x00020000,

			STANDARD_RIGHTS_ALL = 0x001f0000,

			SPECIFIC_RIGHTS_ALL = 0x0000ffff,

			ACCESS_SYSTEM_SECURITY = 0x01000000,

			MAXIMUM_ALLOWED = 0x02000000,

			GENERIC_READ = 0x80000000,
			GENERIC_WRITE = 0x40000000,
			GENERIC_EXECUTE = 0x20000000,
			GENERIC_ALL = 0x10000000,

			DESKTOP_READOBJECTS = 0x00000001,
			DESKTOP_CREATEWINDOW = 0x00000002,
			DESKTOP_CREATEMENU = 0x00000004,
			DESKTOP_HOOKCONTROL = 0x00000008,
			DESKTOP_JOURNALRECORD = 0x00000010,
			DESKTOP_JOURNALPLAYBACK = 0x00000020,
			DESKTOP_ENUMERATE = 0x00000040,
			DESKTOP_WRITEOBJECTS = 0x00000080,
			DESKTOP_SWITCHDESKTOP = 0x00000100,

			WINSTA_ENUMDESKTOPS = 0x00000001,
			WINSTA_READATTRIBUTES = 0x00000002,
			WINSTA_ACCESSCLIPBOARD = 0x00000004,
			WINSTA_CREATEDESKTOP = 0x00000008,
			WINSTA_WRITEATTRIBUTES = 0x00000010,
			WINSTA_ACCESSGLOBALATOMS = 0x00000020,
			WINSTA_EXITWINDOWS = 0x00000040,
			WINSTA_ENUMERATE = 0x00000100,
			WINSTA_READSCREEN = 0x00000200,

			WINSTA_ALL_ACCESS = 0x0000037f
		}

		[DllImport("userenv.dll", SetLastError = true)]
		public static extern bool CreateEnvironmentBlock(out IntPtr lpEnvironment, IntPtr hToken, bool bInherit);

		[DllImport("userenv.dll", SetLastError = true)]
		public static extern bool DestroyEnvironmentBlock(IntPtr lpEnvironment);

		[DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern bool CreateProcessAsUser(
			IntPtr hToken,
			string lpApplicationName,
			[In] StringBuilder lpCommandLine,
			ref SECURITY_ATTRIBUTES lpProcessAttributes,
			ref SECURITY_ATTRIBUTES lpThreadAttributes,
			bool bInheritHandles,
			uint dwCreationFlags,
			IntPtr lpEnvironment,
			string lpCurrentDirectory,
			ref STARTUPINFO lpStartupInfo,
			out PROCESS_INFORMATION lpProcessInformation);

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct STARTUPINFO
		{
			public Int32 cb;
			public string lpReserved;
			public string lpDesktop;
			public string lpTitle;
			public Int32 dwX;
			public Int32 dwY;
			public Int32 dwXSize;
			public Int32 dwYSize;
			public Int32 dwXCountChars;
			public Int32 dwYCountChars;
			public Int32 dwFillAttribute;
			public Int32 dwFlags;
			public Int16 wShowWindow;
			public Int16 cbReserved2;
			public IntPtr lpReserved2;
			public IntPtr hStdInput;
			public IntPtr hStdOutput;
			public IntPtr hStdError;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct PROCESS_INFORMATION
		{
			public IntPtr hProcess;
			public IntPtr hThread;
			public int dwProcessId;
			public int dwThreadId;
		}
	}
}
