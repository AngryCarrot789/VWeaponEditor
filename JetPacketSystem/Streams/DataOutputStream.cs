using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace JetPacketSystem.Streams;

/// <summary>
/// A class for writing primitive objects to a stream
/// <para>
/// The bytes will be written in the big-endianness format, apart from writing pointer values, which will be
/// written in your processor architecture's format, which for modern hardware is little-endianness
/// </para>
/// <para>
/// Most method have repeated code for speed reasons...
/// </para>
/// </summary>
public class DataOutputStream : IDataOutput {
    private Stream stream;

    /// <summary>
    /// A small buffer for writing to
    /// </summary>
    private readonly byte[] buffer8 = new byte[8];

    /// <summary>
    /// A smaller buffer for writing to
    /// </summary>
    private readonly byte[] buffer1 = new byte[1];

    public Stream Stream {
        get => this.stream;
        set => this.stream = value;
    }

    public DataOutputStream() {

    }

    public DataOutputStream(Stream stream) {
        this.stream = stream;
    }

    public DataOutputStream(Stream stream, SeekOrigin origin, long positon = 0) {
        stream.Seek(positon, origin);
        this.stream = stream;
    }

    public void Flush() {
        this.stream.Flush();
    }

    public void Close() {
        this.stream.Close();
    }

    public void Write(byte[] src, int offset, int count) {
        this.stream.Write(src, offset, count);
    }

    public void Write(byte[] src, int offset = 0) {
        this.stream.Write(src, offset, src.Length - offset);
    }

    public void WriteLabelled(byte[] src, int offset, int count) {
        this.WriteUShort((ushort) count);
        this.stream.Write(src, offset, count);
    }

    public void WriteLabelled(byte[] src, int offset = 0) {
        int count = src.Length - offset;
        this.WriteUShort((ushort) count);
        this.stream.Write(src, offset, count);
    }

    public void WriteBoolean(bool value) {
        this.buffer8[0] = (byte) (value ? 1 : 0);
        this.stream.Write(this.buffer8, 0, 1);
    }

    public void WriteSByte(sbyte value) {
        this.buffer8[0] = (byte) value;
        this.stream.Write(this.buffer8, 0, 1);
    }

    public void WriteByte(byte value) {
        this.buffer8[0] = value;
        this.stream.Write(this.buffer8, 0, 1);
    }

    public void WriteShort(short value) {
        uint i = (uint) value;
        byte[] b = this.buffer8;
        b[0] = (byte) (i >> 8);
        b[1] = (byte) (i >> 0);
        this.stream.Write(b, 0, 2);
    }

    public void WriteUShort(ushort value) {
        byte[] b = this.buffer8;
        b[0] = (byte) ((uint) value >> 8);
        b[1] = (byte) ((uint) value >> 0);
        this.stream.Write(b, 0, 2);
    }

    public void WriteInt(int value) {
        byte[] b = this.buffer8;
        b[0] = (byte) (value >> 24);
        b[1] = (byte) (value >> 16);
        b[2] = (byte) (value >> 8);
        b[3] = (byte) (value >> 0);
        this.stream.Write(b, 0, 4);
    }

    public void WriteUInt(uint value) {
        byte[] b = this.buffer8;
        b[0] = (byte) (value >> 24);
        b[1] = (byte) (value >> 16);
        b[2] = (byte) (value >> 8);
        b[3] = (byte) (value >> 0);
        this.stream.Write(b, 0, 4);
    }

    public void WriteLong(long value) {
        byte[] b = this.buffer8;
        b[0] = (byte) (value >> 56);
        b[1] = (byte) (value >> 48);
        b[2] = (byte) (value >> 40);
        b[3] = (byte) (value >> 32);
        b[4] = (byte) (value >> 24);
        b[5] = (byte) (value >> 16);
        b[6] = (byte) (value >> 8);
        b[7] = (byte) (value >> 0);
        this.stream.Write(b, 0, 8);
    }

    public void WriteULong(ulong value) {
        byte[] b = this.buffer8;
        b[0] = (byte) (value >> 56);
        b[1] = (byte) (value >> 48);
        b[2] = (byte) (value >> 40);
        b[3] = (byte) (value >> 32);
        b[4] = (byte) (value >> 24);
        b[5] = (byte) (value >> 16);
        b[6] = (byte) (value >> 8);
        b[7] = (byte) (value >> 0);
        this.stream.Write(b, 0, 8);
    }

    public void WriteFloat(float value) {
        unsafe {
            uint bits = *(uint*) &value;
            byte[] b = this.buffer8;
            b[0] = (byte) (bits >> 24);
            b[1] = (byte) (bits >> 16);
            b[2] = (byte) (bits >> 8);
            b[3] = (byte) (bits >> 0);
            this.stream.Write(b, 0, 4);
        }
    }

    public void WriteDouble(double value) {
        unsafe {
            ulong bits = *(ulong*) &value;
            byte[] b = this.buffer8;
            b[0] = (byte) (bits >> 56);
            b[1] = (byte) (bits >> 48);
            b[2] = (byte) (bits >> 40);
            b[3] = (byte) (bits >> 32);
            b[4] = (byte) (bits >> 24);
            b[5] = (byte) (bits >> 16);
            b[6] = (byte) (bits >> 8);
            b[7] = (byte) (bits >> 0);
            this.stream.Write(b, 0, 8);
        }
    }

    public void WriteEnum08<TEnum>(TEnum value) where TEnum : unmanaged, Enum {
        unsafe {
            this.WriteByte(*(byte*) &value);
        }
    }

    public void WriteEnum16<TEnum>(TEnum value) where TEnum : unmanaged, Enum {
        unsafe {
            this.WriteUShort(*(ushort*) &value);
        }
    }

    public void WriteEnum32<TEnum>(TEnum value) where TEnum : unmanaged, Enum {
        unsafe {
            this.WriteUInt(*(uint*) &value);
        }
    }

    public void WriteEnum64<TEnum>(TEnum value) where TEnum : unmanaged, Enum {
        unsafe {
            this.WriteULong(*(ulong*) &value);
        }
    }

    public void WriteCharUTF16(char value) {
        byte[] b = this.buffer8;
        b[0] = (byte) (value >> 8);
        b[1] = (byte) (value >> 0);
        this.stream.Write(b, 0, 2);
    }

    public void WriteCharUTF8(char value) {
        this.buffer8[0] = (byte) value;
        this.stream.Write(this.buffer8, 0, 1);
    }

    public void WriteChar(char value, Encoding encoding) {
        unsafe {
            // cannot have more than 8 bytes per char
            fixed (byte* ptr = this.buffer8) {
                int count = encoding.GetBytes(&value, 1, ptr, 8);
                this.buffer1[0] = (byte) count;
                this.stream.Write(this.buffer1, 0, 1);
                this.stream.Write(this.buffer8, 0, count);
            }
        }
    }

    public void WriteStringUTF16(string value) {
        int length = value.Length;
        if (length > 4) {
            unsafe {
                fixed (char* ptr = value) {
                    this.WriteCharPtrUTF16(ptr, length);
                }
            }
        }
        else if (length > 0) {
            byte[] b = this.buffer8;
            switch (length) {
                case 4: {
                    char c1 = value[0];
                    char c2 = value[1];
                    char c3 = value[2];
                    char c4 = value[3];
                    b[0] = (byte) (c1 >> 8); b[1] = (byte) (c1 >> 0);
                    b[2] = (byte) (c2 >> 8); b[3] = (byte) (c2 >> 0);
                    b[4] = (byte) (c3 >> 8); b[5] = (byte) (c3 >> 0);
                    b[6] = (byte) (c4 >> 8); b[7] = (byte) (c4 >> 0);
                    break;
                }
                case 3: {
                    char c1 = value[0];
                    char c2 = value[1];
                    char c3 = value[2];
                    b[0] = (byte) (c1 >> 8); b[1] = (byte) (c1 >> 0);
                    b[2] = (byte) (c2 >> 8); b[3] = (byte) (c2 >> 0);
                    b[4] = (byte) (c3 >> 8); b[5] = (byte) (c3 >> 0);
                    break;
                }
                case 2: {
                    char c1 = value[0];
                    char c2 = value[1];
                    b[0] = (byte) (c1 >> 8); b[1] = (byte) (c1 >> 0);
                    b[2] = (byte) (c2 >> 8); b[3] = (byte) (c2 >> 0);
                    break;
                }
                case 1: {
                    char c = value[0];
                    b[0] = (byte) (c >> 8); b[1] = (byte) (c >> 0);
                    break;
                }
            }

            this.stream.Write(b, 0, length * 2);
        }
    }

    public void WriteStringUTF8(string value) {
        int length = value.Length;
        if (length > 8) {
            unsafe {
                fixed (char* ptr = value) {
                    this.WriteCharPtrUTF8(ptr, length);
                }
            }
        }
        else if (length > 0) {
            byte[] b = this.buffer8;
            switch (length) {
                case 8:
                    b[0] = (byte) value[0]; b[1] = (byte) value[1];
                    b[2] = (byte) value[2]; b[3] = (byte) value[3];
                    b[4] = (byte) value[4]; b[5] = (byte) value[5];
                    b[6] = (byte) value[6]; b[7] = (byte) value[7];
                break;
                case 7:
                    b[0] = (byte) value[0]; b[1] = (byte) value[1];
                    b[2] = (byte) value[2]; b[3] = (byte) value[3];
                    b[4] = (byte) value[4]; b[5] = (byte) value[5];
                    b[6] = (byte) value[6];
                break;
                case 6:
                    b[0] = (byte) value[0]; b[1] = (byte) value[1];
                    b[2] = (byte) value[2]; b[3] = (byte) value[3];
                    b[4] = (byte) value[4]; b[5] = (byte) value[5];
                break;
                case 5:
                    b[0] = (byte) value[0]; b[1] = (byte) value[1];
                    b[2] = (byte) value[2]; b[3] = (byte) value[3];
                    b[4] = (byte) value[4];
                break;
                case 4:
                    b[0] = (byte) value[0]; b[1] = (byte) value[1];
                    b[2] = (byte) value[2]; b[3] = (byte) value[3];
                break;
                case 3:
                    b[0] = (byte) value[0]; b[1] = (byte) value[1];
                    b[2] = (byte) value[2];
                break;
                case 2:
                    b[0] = (byte) value[0]; b[1] = (byte) value[1];
                break;
                case 1:
                    b[0] = (byte) value[0];
                break;
            }

            this.stream.Write(b, 0, length);
        }
    }

    public void WriteString(string value, Encoding encoding) {
        unsafe {
            fixed (char* cptr = value) {
                this.WriteCharPtr(cptr, value.Length, encoding);
            }
        }
    }

    public void WriteCharsUTF16(char[] chars) {
        int length = chars.Length;
        if (length > 4) {
            unsafe {
                fixed (char* ptr = chars) {
                    this.WriteCharPtrUTF16(ptr, length);
                }
            }
        }
        else if (length > 0) {
            byte[] b = this.buffer8;
            switch (length) {
                case 4: {
                    char c1 = chars[0];
                    char c2 = chars[1];
                    char c3 = chars[2];
                    char c4 = chars[3];
                    b[0] = (byte) (c1 >> 8); b[1] = (byte) (c1 >> 0);
                    b[2] = (byte) (c2 >> 8); b[3] = (byte) (c2 >> 0);
                    b[4] = (byte) (c3 >> 8); b[5] = (byte) (c3 >> 0);
                    b[6] = (byte) (c4 >> 8); b[7] = (byte) (c4 >> 0);
                    break;
                }
                case 3: {
                    char c1 = chars[0];
                    char c2 = chars[1];
                    char c3 = chars[2];
                    b[0] = (byte) (c1 >> 8); b[1] = (byte) (c1 >> 0);
                    b[2] = (byte) (c2 >> 8); b[3] = (byte) (c2 >> 0);
                    b[4] = (byte) (c3 >> 8); b[5] = (byte) (c3 >> 0);
                    break;
                }
                case 2: {
                    char c1 = chars[0];
                    char c2 = chars[1];
                    b[0] = (byte) (c1 >> 8); b[1] = (byte) (c1 >> 0);
                    b[2] = (byte) (c2 >> 8); b[3] = (byte) (c2 >> 0);
                    break;
                }
                case 1: {
                    char c = chars[0];
                    b[0] = (byte) (c >> 8); b[1] = (byte) (c >> 0);
                    break;
                }
            }

            this.stream.Write(b, 0, length * 2);
        }
    }

    public void WriteCharsUTF8(char[] chars) {
        int length = chars.Length;
        if (length > 8) {
            unsafe {
                fixed (char* ptr = chars) {
                    this.WriteCharPtrUTF8(ptr, length);
                }
            }
        }
        else if (length > 0) {
            byte[] b = this.buffer8;
            switch (length) {
                case 8:
                    b[0] = (byte) chars[0]; b[1] = (byte) chars[1];
                    b[2] = (byte) chars[2]; b[3] = (byte) chars[3];
                    b[4] = (byte) chars[4]; b[5] = (byte) chars[5];
                    b[6] = (byte) chars[6]; b[7] = (byte) chars[7];
                break;
                case 7:
                    b[0] = (byte) chars[0]; b[1] = (byte) chars[1];
                    b[2] = (byte) chars[2]; b[3] = (byte) chars[3];
                    b[4] = (byte) chars[4]; b[5] = (byte) chars[5];
                    b[6] = (byte) chars[6];
                break;
                case 6:
                    b[0] = (byte) chars[0]; b[1] = (byte) chars[1];
                    b[2] = (byte) chars[2]; b[3] = (byte) chars[3];
                    b[4] = (byte) chars[4]; b[5] = (byte) chars[5];
                break;
                case 5:
                    b[0] = (byte) chars[0]; b[1] = (byte) chars[1];
                    b[2] = (byte) chars[2]; b[3] = (byte) chars[3];
                    b[4] = (byte) chars[4];
                break;
                case 4:
                    b[0] = (byte) chars[0]; b[1] = (byte) chars[1];
                    b[2] = (byte) chars[2]; b[3] = (byte) chars[3];
                break;
                case 3:
                    b[0] = (byte) chars[0]; b[1] = (byte) chars[1];
                    b[2] = (byte) chars[2];
                break;
                case 2:
                    b[0] = (byte) chars[0]; b[1] = (byte) chars[1];
                break;
                case 1:
                    b[0] = (byte) chars[0];
                break;
            }

            this.stream.Write(b, 0, length);
        }
    }

    public void WriteChars(char[] chars, Encoding encoding) {
        unsafe {
            fixed (char* cptr = chars) {
                this.WriteCharPtr(cptr, chars.Length, encoding);
            }
        }
    }

    public void WriteCharsLabelledUTF16(char[] chars) {
        this.WriteUShort((ushort) chars.Length);
        this.WriteCharsUTF16(chars);
    }

    public void WriteCharsLabelledUTF8(char[] chars) {
        this.WriteUShort((ushort) chars.Length);
        this.WriteCharsUTF8(chars);
    }

    public void WriteStringLabelledUTF16(string value) {
        this.WriteUShort((ushort) value.Length);
        this.WriteStringUTF16(value);
    }

    public void WriteStringLabelledUTF8(string value) {
        this.WriteUShort((ushort) value.Length);
        this.WriteStringUTF8(value);
    }

    public unsafe void WriteCharPtrUTF16(char* src, int count) {
        byte[] b = this.buffer8;
        Stream s = this.stream;
        fixed (byte* bptr = this.buffer8) {
            while (count > 3) {
                bptr[0] = (byte) (src[0] >> 8);
                bptr[1] = (byte) (src[0] >> 0);
                bptr[2] = (byte) (src[1] >> 8);
                bptr[3] = (byte) (src[1] >> 0);
                bptr[4] = (byte) (src[2] >> 8);
                bptr[5] = (byte) (src[2] >> 0);
                bptr[6] = (byte) (src[3] >> 8);
                bptr[7] = (byte) (src[3] >> 0);
                src += 4;
                count -= 4;
                s.Write(b, 0, 8);
            }

            switch (count) {
                case 0: break;
                case 3:
                    bptr[0] = (byte) (src[0] >> 8);
                    bptr[1] = (byte) (src[0] >> 0);
                    bptr[2] = (byte) (src[1] >> 8);
                    bptr[3] = (byte) (src[1] >> 0);
                    bptr[4] = (byte) (src[2] >> 8);
                    bptr[5] = (byte) (src[2] >> 0);
                    s.Write(b, 0, 6);
                break;
                case 2:
                    bptr[0] = (byte) (src[0] >> 8);
                    bptr[1] = (byte) (src[0] >> 0);
                    bptr[2] = (byte) (src[1] >> 8);
                    bptr[3] = (byte) (src[1] >> 0);
                    s.Write(b, 0, 4);
                break;
                case 1:
                    bptr[0] = (byte) (src[0] >> 8);
                    bptr[1] = (byte) (src[0] >> 0);
                    s.Write(b, 0, 2);
                break;
            }
        }
    }

    public unsafe void WriteCharPtrUTF8(char* src, int count) {
        byte[] b = this.buffer8;
        Stream s = this.stream;
        fixed (byte* bptr = b) {
            while (count > 7) {
                bptr[0] = (byte) src[0];
                bptr[1] = (byte) src[1];
                bptr[2] = (byte) src[2];
                bptr[3] = (byte) src[3];
                bptr[4] = (byte) src[4];
                bptr[5] = (byte) src[5];
                bptr[6] = (byte) src[6];
                bptr[7] = (byte) src[7];
                src += 8;
                count -= 8;
                s.Write(b, 0, 8);
            }

            if (count > 3) {
                bptr[0] = (byte) src[0];
                bptr[1] = (byte) src[1];
                bptr[2] = (byte) src[2];
                bptr[3] = (byte) src[3];
                src += 4;
                count -= 4;
                s.Write(b, 0, 4);
            }

            switch (count) {
                case 3:
                    bptr[0] = (byte) src[0];
                    bptr[1] = (byte) src[1];
                    bptr[2] = (byte) src[2];
                    s.Write(b, 0, 3);
                break;
                case 2:
                    bptr[0] = (byte) src[0];
                    bptr[1] = (byte) src[1];
                    s.Write(b, 0, 2);
                break;
                case 1:
                    bptr[0] = (byte) src[0];
                    s.Write(b, 0, 1);
                break;
            }
        }
    }

    public unsafe void WriteCharPtr(char* cptr, int count, Encoding encoding) {
        byte[] buffer = new byte[encoding.GetByteCount(cptr, count)];
        fixed (byte* bptr = buffer) {
            int bytes = encoding.GetBytes(cptr, count, bptr, buffer.Length);
            this.WriteUShort((ushort) bytes);
            this.stream.Write(buffer, 0, bytes);
        }
    }

    public unsafe void WritePtr(byte* src, int count) {
        byte[] b = this.buffer8;
        Stream s = this.stream;
        fixed (byte* ptr = b) {
            while (count > 7) {
                *(ulong*) ptr = *(ulong*) (src);
                src += 8;
                count -= 8;
                s.Write(b, 0, 8);
            }

            if (count > 3) {
                *(uint*) ptr = *(uint*) (src);
                src += 4;
                count -= 4;
                s.Write(b, 0, 4);
            }

            switch (count) {
                case 3:
                    ptr[0] = src[0];
                    ptr[1] = src[1];
                    ptr[2] = src[2];
                    s.Write(b, 0, 3);
                break;
                case 2:
                    ptr[0] = src[0];
                    ptr[1] = src[1];
                    s.Write(b, 0, 2);
                break;
                case 1:
                    ptr[0] = src[0];
                    s.Write(b, 0, 1);
                break;
            }

        }
    }

    public void WritePtr(IntPtr src, int count) {
        byte[] b = this.buffer8;
        Stream s = this.stream;
        unsafe {
            fixed (byte* ptr = b) {
                while (count > 7) {
                    *(long*) ptr = Marshal.ReadInt64(src);
                    src += 8;
                    count -= 8;
                    s.Write(b, 0, 8);
                }

                if (count > 3) {
                    *(int*) ptr = Marshal.ReadInt32(src);
                    src += 4;
                    count -= 4;
                    s.Write(b, 0, 4);
                }

                switch (count) {
                    case 3:
                        b[0] = Marshal.ReadByte(src, 0);
                        b[1] = Marshal.ReadByte(src, 1);
                        b[2] = Marshal.ReadByte(src, 2);
                        s.Write(b, 0, 3);
                    break;
                    case 2:
                        b[0] = Marshal.ReadByte(src, 0);
                        b[1] = Marshal.ReadByte(src, 1);
                        s.Write(b, 0, 2);
                    break;
                    case 1:
                        b[0] = Marshal.ReadByte(src, 0);
                        s.Write(b, 0, 1);
                    break;
                }
            }
        }
    }

    public void WriteStruct<T>(T value) where T : unmanaged {
        unsafe {
            this.WritePtr((byte*) &value, sizeof(T));
        }
    }
}