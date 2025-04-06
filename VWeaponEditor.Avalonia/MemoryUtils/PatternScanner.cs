using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using VWeaponEditor.Avalonia.MemoryUtils.Native;

namespace VWeaponEditor.Avalonia.MemoryUtils {
    public class PatternScanner {
        public static unsafe IntPtr FindPattern(Process process, string pattern) {
            IntPtr hProcess = process.Handle;
            
            byte?[] pat = ParsePattern(pattern);
            foreach (ProcessModule module in process.Modules) {
                IntPtr baseAddress = module.BaseAddress;
                int moduleSize = module.ModuleMemorySize;

                byte* buffer = (byte*) Marshal.AllocHGlobal(moduleSize);
                if (!NativeMethods.ReadProcessMemory(hProcess, baseAddress, buffer, moduleSize, (int*) 0)) {
                    continue;
                }

                for (int i = 0; i < moduleSize - pat.Length; i++) {
                    bool found = true;
                    for (int j = 0; j < pat.Length; j++) {
                        if (pat[j].HasValue && pat[j] != *(buffer + i + j)) {
                            found = false;
                            break;
                        }
                    }

                    if (found) {
                        return baseAddress + i;
                    }
                }
            }

            return IntPtr.Zero; // not found
        }

        private static byte?[] ParsePattern(string pattern) {
            string[] tokens = pattern.Split(' ', int.MaxValue, StringSplitOptions.RemoveEmptyEntries);
            byte?[] array = new byte?[tokens.Length];
            for (int i = 0; i < tokens.Length; i++) {
                array[i] = tokens[i] == "?" ? null : Convert.ToByte(tokens[i], 16);
            }

            return array;
        }
    }
}