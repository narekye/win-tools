using Service.AppRunner.Enums;
using Service.AppRunner.Structures;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;

namespace Service.AppRunner
{
    public class ApplicationRunner
    {
        #region Constants

        public const int TokenDuplicate = 0x0002;
        public const uint MaximumAllowed = 0x2000000;
        public const int CreateNewConsole = 0x00000010;

        public const int IdlePriorityClass = 0x40;
        public const int NormalPriorityClass = 0x20;
        public const int HighPriorityCLass = 0x80;
        public const int ReadlTimePriorityClass = 0x100;

        #endregion

        #region Win32 imports

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CloseHandle(IntPtr hSnapshot);

        [DllImport("kernel32.dll")]
        static extern uint WTSGetActiveConsoleSessionId();

        [DllImport("kernel32.dll", EntryPoint = "CreateProcessAsUser", SetLastError = true, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        public static extern bool CreateProcessAsUser(IntPtr hToken, string lpApplicationName, string lpCommandLine, ref SecurityAttributes lpProcessAttributes,
            ref SecurityAttributes lpThreadAttributes, bool bInheritHanlde, int dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, ref StartupInfo lpStartupInfo, out ProcessInformation lpProcessInformation);

        [DllImport("kernel32.dll")]
        static extern bool ProcessToSessionId(uint dwProcessId, ref uint pSessionId);

        [DllImport("advapi32.dll", EntryPoint = "DuplicateTokenEx")]
        public static extern bool DuplicateTokenEx(IntPtr ExistingTokenHandle, uint dwDesiredAccess, ref SecurityAttributes lpThreadAttributes,
            int TokenType, int ImpersonationLevel, ref IntPtr DuplicateTokenHandler);

        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

        [DllImport("advapi32.dll", SetLastError = true), SuppressUnmanagedCodeSecurity]
        static extern bool OpenProcessToken(IntPtr ProcessHandle, int DesiredAccess, ref IntPtr TokenHandle);

        #endregion

        #region Methods

        public static bool StartProcessAndBypassUAC(string applicationName, out ProcessInformation information)
        {
            uint winLogonID = 0;
            IntPtr hUserTokenDup = IntPtr.Zero, hPToken = IntPtr.Zero, hProcess = IntPtr.Zero;
            information = new ProcessInformation();

            uint dwSessionId = WTSGetActiveConsoleSessionId();

            Process[] processes = Process.GetProcessesByName("winlogon");
            if (processes != null && processes.Any())
            {
                foreach (var process in processes)
                {
                    if ((uint)process.SessionId == dwSessionId)
                        winLogonID = (uint)process.Id;
                }
            }

            hProcess = OpenProcess(MaximumAllowed, false, winLogonID);

            if (!OpenProcessToken(hProcess, TokenDuplicate, ref hPToken))
            {
                CloseHandle(hProcess);
                return false;
            }

            SecurityAttributes sa = new SecurityAttributes();
            sa.Length = Marshal.SizeOf(sa);

            if (!DuplicateTokenEx(hPToken, MaximumAllowed, ref sa, (int)SecurityImpersonationLevel.SecurityIdentification, (int)TokenType.TokenPrimary, ref hUserTokenDup))
            {
                CloseHandle(hProcess);
                CloseHandle(hPToken);
                return false;
            }

            StartupInfo si = new StartupInfo();
            si.cb = Marshal.SizeOf(si);
            si.lpDesktop = @"winsta0\default";

            int dwCreationFlags = NormalPriorityClass | CreateNewConsole;

            bool result = CreateProcessAsUser(hUserTokenDup,
                                                null,
                                                applicationName,
                                                ref sa,
                                                ref sa,
                                                false,
                                                dwCreationFlags,
                                                IntPtr.Zero,
                                                null,
                                                ref si,
                                                out information);

            CloseHandle(hProcess);
            CloseHandle(hPToken);
            CloseHandle(hUserTokenDup);

            return result;
        }
        #endregion
    }
}
