using System;
using System.Runtime.InteropServices;

namespace VWeaponEditor.Avalonia.MemoryUtils.Native;

[StructLayout(LayoutKind.Sequential)]
public struct MODULEINFO {
    public IntPtr lpBaseOfDll; // void*, LPVOID
    public uint SizeOfImage;
    public IntPtr EntryPoint; // void*, LPVOID
}