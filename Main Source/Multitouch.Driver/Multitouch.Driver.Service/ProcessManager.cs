using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Multitouch.Driver.Service.Native;

namespace Multitouch.Driver.Service
{
	class ProcessManager : IDisposable
	{
		IntPtr environment;
		IntPtr tokenCopy;
		NativeMethods.PROCESS_INFORMATION processInfo;

		public const string EMBEDDING = "-Embedding";

		ProcessManager()
		{ }

		public void Dispose()
		{
			if (processInfo.dwProcessId != 0)
			{
				Process process = Process.GetProcessById(processInfo.dwProcessId);
				process.Kill();
				NativeMethods.CloseHandle(processInfo.hThread);
				NativeMethods.CloseHandle(processInfo.hProcess);

				if (environment != IntPtr.Zero)
					NativeMethods.DestroyEnvironmentBlock(environment);

				if (tokenCopy != IntPtr.Zero)
					NativeMethods.CloseHandle(tokenCopy);
			}
		}

		private void StartProcess()
		{
			uint id = (uint)NativeMethods.WTSGetActiveConsoleSessionId();

			IntPtr token;
			bool ok = NativeMethods.WTSQueryUserToken(id, out token);

			NativeMethods.SECURITY_ATTRIBUTES security = new NativeMethods.SECURITY_ATTRIBUTES();
			security.nLength = Marshal.SizeOf(typeof(NativeMethods.SECURITY_ATTRIBUTES));
			if (!NativeMethods.DuplicateTokenEx(token, NativeMethods.ACCESS_MASK.MAXIMUM_ALLOWED, ref security,
			                                    NativeMethods.SECURITY_IMPERSONATION_LEVEL.SecurityIdentification, NativeMethods.TOKEN_TYPE.TokenPrimary, out tokenCopy))
			{
				throw new Exception("Can't create duplicate token");
			}

			if (ok)
				NativeMethods.CloseHandle(token);

			if (!NativeMethods.CreateEnvironmentBlock(out environment, tokenCopy, false))
				throw new Exception("Can't create environment block");

			security = new NativeMethods.SECURITY_ATTRIBUTES();
			NativeMethods.STARTUPINFO startupInfo = new NativeMethods.STARTUPINFO();
			startupInfo.dwFlags = 1;
			startupInfo.wShowWindow = 1;
			startupInfo.lpDesktop = "winsta0\\default";

			string location = Assembly.GetEntryAssembly().Location;
			StringBuilder cmd = new StringBuilder();
			cmd.AppendFormat("\"{0}\" {1}", location, EMBEDDING);

			string directory = Path.GetDirectoryName(location);
			if (!NativeMethods.CreateProcessAsUser(tokenCopy, null, cmd, ref security, ref security, false,
				NativeMethods.CREATE_UNICODE_ENVIRONMENT, environment, directory, ref startupInfo, out processInfo))
			{
				throw new Exception("Can't create process as a user. Error: " + Marshal.GetLastWin32Error());
			}
		}

		public static IDisposable Start()
		{
			ProcessManager result = new ProcessManager();
			result.StartProcess();
			return result;
		}
	}
}