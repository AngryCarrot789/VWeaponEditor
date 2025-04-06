using System;
using System.Diagnostics;
using VWeaponEditor.Avalonia.MemoryUtils.Native;

namespace VWeaponEditor.Avalonia.MemoryUtils {
    public static class PSAPI {
        public static unsafe bool GetModuleInfo(Process process, out MODULEINFO info) {
            if (process.MainModule is ProcessModule x) {
                IntPtr mainModule = NativeMethods.GetModuleHandle(x.ModuleName);
                return NativeMethods.GetModuleInformation(process.Handle, mainModule, out info, sizeof(MODULEINFO));
            }

            info = default;
            return false;
        }
    }
}