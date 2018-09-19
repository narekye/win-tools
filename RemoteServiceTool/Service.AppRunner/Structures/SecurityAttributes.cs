using System;
using System.Runtime.InteropServices;

namespace Service.AppRunner.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SecurityAttributes
    {
        public int Length;
        public IntPtr lpSecurityDescriptor;
        public bool bInheritHandle;
    }
}
