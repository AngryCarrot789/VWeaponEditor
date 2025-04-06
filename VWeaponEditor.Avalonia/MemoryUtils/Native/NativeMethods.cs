using System;
using System.Runtime.InteropServices;

namespace VWeaponEditor.Avalonia.MemoryUtils.Native {
    // Most of this is found on https://www.pinvoke.net/
    public static class NativeMethods {
        public const uint PROCESS_ALL_ACCESS = 0x1F0FFF;
        public const uint PROCESS_VM_READ = 0x0010;
        public const uint PROCESS_VM_WRITE = 0x0020;

        [DllImport("kernel32.dll")]
        public static extern unsafe int VirtualQueryEx(
            IntPtr hProcess,
            void* lpAddress,
            out MEMORY_BASIC_INFORMATION64 lpBuffer,
            int dwLength);

        [DllImport("kernel32.dll")]
        public static extern unsafe bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            void* lpBuffer,
            int dwSize,
            int* lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern unsafe bool WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            void* lpBuffer,
            int dwSize,
            int* lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(
            uint dwDesiredAccess,
            bool bInheritHandle,
            int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll")]
        public static extern bool VirtualProtectEx(
            IntPtr hProcess,
            IntPtr lpAdress,
            UIntPtr dwSize,
            uint newProtectionType,
            out uint oldProtectionType);

        [DllImport("psapi.dll", SetLastError=true)]
        public static extern bool GetModuleInformation(
            IntPtr hProcess,
            IntPtr hModule,
            out MODULEINFO lpmodinfo,
            int cb);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}