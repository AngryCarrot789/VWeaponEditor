using System;

namespace VWeaponEditor.Avalonia.MemoryUtils.Native {
    public static class NativeUtils {
        public static IntPtr OpenProcess(int pid, ProtectionType access) {
            return NativeMethods.OpenProcess((uint) access, false, pid);
        }

        public static unsafe bool VirtualProtect(ProtectionType access, IntPtr process, IntPtr address, uint size, out uint oldProtectType) {
            return NativeMethods.VirtualProtectEx(process, address, (UIntPtr) size, (uint) access, out oldProtectType);
        }
    }
}