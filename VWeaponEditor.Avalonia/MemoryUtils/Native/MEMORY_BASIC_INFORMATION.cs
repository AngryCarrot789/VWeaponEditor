using System;
using System.Runtime.InteropServices;

namespace VWeaponEditor.Avalonia.MemoryUtils.Native {
    // https://www.pinvoke.net/default.aspx/kernel32.virtualqueryex
    [StructLayout(LayoutKind.Sequential)]
    public struct MEMORY_BASIC_INFORMATION {
        public IntPtr BaseAddress;
        public IntPtr AllocationBase;
        public uint AllocationProtect;
        public IntPtr RegionSize;
        public uint State;
        public uint Protect;
        public uint Type;
    }
}