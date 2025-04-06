using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using VWeaponEditor.Avalonia.MemoryUtils.Native;

namespace VWeaponEditor.Avalonia.MemoryUtils;

public class Mem64 {
    public Process Process { get; }

    public IntPtr ProcessHandle { get; set; }

    public Mem64(Process process) {
        this.Process = process;
        this.ProcessHandle = process.Handle;
    }

    public static unsafe bool ReadMemory(IntPtr hProcess, IntPtr hAddress, void* lpBuffer, int dwSize) {
        return NativeMethods.ReadProcessMemory(hProcess, hAddress, lpBuffer, dwSize, (int*) 0);
    }

    public unsafe bool Read(IntPtr address, void* buffer, int count) {
        return ReadMemory(this.ProcessHandle, address, buffer, count);
    }
        
    public unsafe bool Read(IntPtr address, void* buffer, int count, int[] offsets) {
        address = GetAddress(this.ProcessHandle, address, offsets);
        return ReadMemory(this.ProcessHandle, address, buffer, count);
    }

    public bool ReadArray<T>(IntPtr address, T[] array) where T : unmanaged {
        unsafe {
            fixed (T* ptr = array) {
                int bytes = array.Length * sizeof(T);
                return this.Read(address, ptr, bytes);
            }
        }
    }

    public bool Read<T>(IntPtr address, out T value) where T : unmanaged {
        T x = default;
        unsafe {
            if (this.Read(address, &x, sizeof(T))) {
                value = x;
                return true;
            }
        }

        value = default;
        return false;
    }

    public T Read<T>(IntPtr address) where T : unmanaged {
        unsafe {
            T value = default;
            if (this.Read(address, &value, sizeof(T)))
                return value;
            throw new Win32Exception();
        }
    }

    public T Read<T>(IntPtr address, int[] offsets) where T : unmanaged {
        address = GetAddress(this.ProcessHandle, address, offsets);
        unsafe {
            T value = default;
            if (this.Read(address, &value, sizeof(T)))
                return value;
            throw new Win32Exception();
        }
    }
        
    public string ReadString(IntPtr address, int length, int[] offsets) {
        address = GetAddress(this.ProcessHandle, address, offsets);
        unsafe {
            byte* buffer = stackalloc byte[length];
            if (!ReadMemory(this.ProcessHandle, address, buffer, length))
                throw new Win32Exception();
            return Encoding.ASCII.GetString(buffer, length);
        }
    }

    public T[] ReadArray<T>(IntPtr address, int count) where T : unmanaged {
        T[] array = new T[count];
        if (!this.ReadArray(address, array))
            throw new Win32Exception();
        return array;
    }

    // public unsafe void ScanPattern(byte* pattern) {
    //     IntPtr hProcess = this.ProcessHandle;
    //     byte* address = (byte*) 0;
    //     while (true) {
    //         int read = NativeMethods.VirtualQueryEx(hProcess, address, out MEMORY_BASIC_INFORMATION64 info, sizeof(MEMORY_BASIC_INFORMATION64));
    //         address += info.RegionSize;
    //     }
    // }
        
    /// <summary>
    /// Dereferences the address and follows a pointer change up until the last offset in the array which
    /// is treated as an offset. Same as adding the final offset to the result of <see cref="Dereference(System.IntPtr,System.IntPtr,int[])"/>
    /// </summary>
    public static unsafe IntPtr GetAddress(IntPtr hProcess, IntPtr addr, int[] offsets) {
        IntPtr nAddress = addr;
        if (addr == IntPtr.Zero)
            return nAddress;

        int endIdx = offsets.Length - 1;
        if (endIdx < 0)
            return nAddress;

        if (!ReadMemory(hProcess, nAddress, &nAddress, sizeof(IntPtr)))
            throw new Win32Exception();
        for (int i = 0; i != endIdx; i++)
            if (!ReadMemory(hProcess, nAddress + offsets[i], &nAddress, sizeof(IntPtr)))
                throw new Win32Exception();

        nAddress += offsets[endIdx];
        return nAddress;
    }
        
    /// <summary>
    /// Dereferences a pointer at the given address, returning the address of the actual object in memory
    /// </summary>
    public static unsafe IntPtr Dereference(IntPtr hProcess, IntPtr address) {
        if (address == IntPtr.Zero)
            return address;
        if (!ReadMemory(hProcess, address, &address, sizeof(IntPtr)))
            throw new Win32Exception();
        return address;
    }

    /// <summary>
    /// Dereferences a pointer with a list of offsets, returning the final address a pointer points to
    /// </summary>
    public static unsafe IntPtr Dereference(IntPtr hProcess, IntPtr address, int[] offsets) {
        if (address == IntPtr.Zero)
            return address;

        if (!ReadMemory(hProcess, address, &address, sizeof(IntPtr)))
            throw new Win32Exception();
        foreach (int offset in offsets)
            if (!ReadMemory(hProcess, address + offset, &address, sizeof(IntPtr)))
                throw new Win32Exception();
        return address;
    }

    public IntPtr ReadPointer(IntPtr address) {
        return Dereference(this.ProcessHandle, address);
    }

    public byte ReadByte(IntPtr address) {
        return this.Read<byte>(address);
    }
        
    public short ReadInt16(IntPtr address) {
        return this.Read<short>(address);
    }
        
    public ushort ReadUInt16(IntPtr address) {
        return this.Read<ushort>(address);
    }
        
    public int ReadInt32(IntPtr address) {
        return this.Read<int>(address);
    }
        
    public uint ReadUInt32(IntPtr address) {
        return this.Read<uint>(address);
    }
        
    public long ReadInt64(IntPtr address) {
        return this.Read<long>(address);
    }
        
    public ulong ReadUInt64(IntPtr address) {
        return this.Read<ulong>(address);
    }
        
    public float ReadFloat(IntPtr address) {
        return this.Read<float>(address);
    }
        
    public double ReadDouble(IntPtr address) {
        return this.Read<double>(address);
    }
        
    public unsafe bool Write<T>(IntPtr address, T value) where T : unmanaged {
        return NativeMethods.WriteProcessMemory(this.ProcessHandle, address, &value, sizeof(T), (int*) 0);
    }
}