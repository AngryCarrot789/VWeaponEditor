using System.Runtime.InteropServices;

namespace VWeaponEditor.Avalonia.MemoryUtils.Native {
    // https://www.pinvoke.net/default.aspx/kernel32.virtualqueryex
    [StructLayout(LayoutKind.Sequential)]
    public struct MEMORY_BASIC_INFORMATION64 {
        public long BaseAddress;
        public long AllocationBase;
        public int AllocationProtect;
        public int __alignment1;
        public long RegionSize;
        public int State;
        public int Protect;
        public int Type;
        public int __alignment2;
    }
}