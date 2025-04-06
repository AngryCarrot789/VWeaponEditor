using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace JetPacketSystem.Streams;

/// <summary>
/// A class for reading primitive objects from a stream
/// <para>
/// The bytes will be read in the big-endianness format, apart from reading pointer values, which will be
/// read in your processor architecture's format, which for modern hardware is little-endianness
/// </para>
/// <para>
/// Most method have repeated code for speed reasons...
/// </para>
/// </summary>
public class DataInputStream : IDataInput {
    private Stream stream;

    /// <summary>
    /// A small buffer for reading into
    /// </summary>
    private readonly byte[] buffer8 = new byte[8];

    /// <summary>
    /// A temp buffer used for writing chars with specific encodings
    /// </summary>
    private readonly char[] cbuffer1 = new char[1];

    // nullable, for speed reasons. care must be taken to ensure the steam isn't null while in use
    public Stream Stream {
        get => this.stream;
        set => this.stream = value;
    }

    /// <summary>
    /// Creates a data input stream, with no underlying stream (it can be set later on)
    /// </summary>
    public DataInputStream() {

    }

    /// <summary>
    /// Creates a data input stream, using the given underlying stream
    /// </summary>
    public DataInputStream(Stream stream) {
        this.stream = stream;
    }

    /// <summary>
    /// Creates a new data input stream, using the given underlying stream, seeking it at the given origin, optionally at the given offset (0 by default)
    /// </summary>
    public DataInputStream(Stream stream, SeekOrigin origin, long offset = 0) {
        this.stream = stream;
        stream.Seek(offset, origin);
    }

    public void Close() {
        this.stream.Close();
    }

    public int Read(byte[] dest, int offset, int count) {
        return this.stream.Read(dest, offset, count);
    }

    public void ReadFully(byte[] dest) {
        this.ReadFully(dest, 0, dest.Length);
    }

    public void ReadFully(byte[] dest, int offset, int length) {
        int n = 0;
        Stream s = this.stream;
        while (n < length) {
            n += s.Read(dest, offset + n, length - n);
        }
    }

    public byte[] ReadBytes(int count) {
        byte[] bytes = new byte[count];
        this.ReadFully(bytes, 0, count);
        return bytes;
    }

    public byte[] ReadBytesLabelled() {
        ushort length = this.ReadUShort();
        byte[] bytes = new byte[length];
        this.ReadFully(bytes, 0, length);
        return bytes;
    }

    public bool ReadBool() {
        if (this.stream.Read(this.buffer8, 0, 1) != 1) {
            throw new EndOfStreamException("Failed to read 1 byte for a boolean");
        }

        return this.buffer8[0] != 0;
    }

    public sbyte ReadSByte() {
        if (this.stream.Read(this.buffer8, 0, 1) != 1) {
            throw new EndOfStreamException("Failed to read 1 byte for an sbyte");
        }

        return (sbyte) this.buffer8[0];
    }

    public byte ReadByte() {
        if (this.stream.Read(this.buffer8, 0, 1) != 1) {
            throw new EndOfStreamException("Failed to read 1 byte for a byte");
        }

        return this.buffer8[0];
    }

    public short ReadShort() {
        byte[] b = this.buffer8;
        if (this.stream.Read(b, 0, 2) != 2) {
            throw new EndOfStreamException("Failed to read 2 bytes for a short");
        }

        return (short) ((b[0] << 8) + (b[1] << 0));
    }

    public ushort ReadUShort() {
        byte[] b = this.buffer8;
        if (this.stream.Read(b, 0, 2) != 2) {
            throw new EndOfStreamException("Failed to read 2 bytes for a ushort");
        }

        return (ushort) ((b[0] << 8) + (b[1] << 0));
    }

    public int ReadInt() {
        byte[] b = this.buffer8;
        if (this.stream.Read(b, 0, 4) != 4) {
            throw new EndOfStreamException("Failed to read 4 bytes for a int");
        }

        return (int) (((uint) b[0] << 24) +
                      ((uint) b[1] << 16) +
                      ((uint) b[2] << 8) +
                      ((uint) b[3] << 0));
    }

    public uint ReadUInt() {
        byte[] b = this.buffer8;
        if (this.stream.Read(b, 0, 4) != 4) {
            throw new EndOfStreamException("Failed to read 4 bytes for a uint");
        }

        return ((uint) b[0] << 24) +
               ((uint) b[1] << 16) +
               ((uint) b[2] << 8) +
               ((uint) b[3] << 0);
    }

    public long ReadLong() {
        byte[] b = this.buffer8;
        if (this.stream.Read(b, 0, 8) != 8) {
            throw new EndOfStreamException("Failed to read 8 bytes for a long");
        }

        return (long) (((ulong) b[0] << 56) +
                       ((ulong) b[1] << 48) +
                       ((ulong) b[2] << 40) +
                       ((ulong) b[3] << 32) +
                       ((ulong) b[4] << 24) +
                       ((ulong) b[5] << 16) +
                       ((ulong) b[6] << 8) +
                       ((ulong) b[7] << 0));
    }

    public ulong ReadULong() {
        byte[] b = this.buffer8;
        if (this.stream.Read(b, 0, 8) != 8) {
            throw new EndOfStreamException("Failed to read 8 bytes for a ulong");
        }

        return ((ulong) b[0] << 56) +
               ((ulong) b[1] << 48) +
               ((ulong) b[2] << 40) +
               ((ulong) b[3] << 32) +
               ((ulong) b[4] << 24) +
               ((ulong) b[5] << 16) +
               ((ulong) b[6] << 8) +
               ((ulong) b[7] << 0);
    }

    public float ReadFloat() {
        byte[] b = this.buffer8;
        if (this.stream.Read(b, 0, 4) != 4) {
            throw new EndOfStreamException("Failed to read 4 bytes for a float");
        }

        unsafe {
            uint p0 = ((uint) b[0] << 24) +
                      ((uint) b[1] << 16) +
                      ((uint) b[2] << 8) +
                      ((uint) b[3] << 0);
            return *(float*) &p0;
        }
    }

    public double ReadDouble() {
        byte[] b = this.buffer8;
        if (this.stream.Read(b, 0, 8) != 8) {
            throw new EndOfStreamException("Failed to read 8 bytes for a double");
        }

        unsafe {
            ulong p0 = ((ulong) b[0] << 56) +
                       ((ulong) b[1] << 48) +
                       ((ulong) b[2] << 40) +
                       ((ulong) b[3] << 32) +
                       ((ulong) b[4] << 24) +
                       ((ulong) b[5] << 16) +
                       ((ulong) b[6] << 8) +
                       ((ulong) b[7] << 0);
            return *(double*) &p0;
        }
    }

    public T ReadEnum08<T>() where T : unmanaged, Enum {
        byte value = this.ReadByte();
        unsafe {
            return *(T*) &value;
        }
    }

    public T ReadEnum16<T>() where T : unmanaged, Enum {
        ushort value = this.ReadUShort();
        unsafe {
            return *(T*) &value;
        }
    }

    public T ReadEnum32<T>() where T : unmanaged, Enum {
        uint value = this.ReadUInt();
        unsafe {
            return *(T*) &value;
        }
    }

    public T ReadEnum64<T>() where T : unmanaged, Enum {
        ulong value = this.ReadULong();
        unsafe {
            return *(T*) &value;
        }
    }

    public char ReadCharUTF16() {
        byte[] b = this.buffer8;
        if (this.stream.Read(b, 0, 2) != 2) {
            throw new EndOfStreamException("Failed to read 2 bytes for a UTF16 char");
        }

        return (char) (ushort) ((b[0] << 8) + (b[1] << 0));
    }

    public char ReadCharUTF8() {
        if (this.stream.Read(this.buffer8, 0, 1) != 1) {
            throw new EndOfStreamException("Failed to read 1 byte for a UTF8 char");
        }

        return (char) this.buffer8[0];
    }

    public char ReadChar(Encoding encoding) {
        byte bytes = this.ReadByte();
        if (this.stream.Read(this.buffer8, 0, bytes) != bytes) {
            throw new EndOfStreamException($"Failed to read {bytes} bytes for a char with encoding {encoding}");
        }

        if (encoding.GetChars(this.buffer8, 0, bytes, this.cbuffer1, 0) < 1) {
            throw new Exception("Failed to decode at least 1 character");
        }

        return this.cbuffer1[0];
    }

    public char[] ReadCharsUTF16(int length) {
        if (length < 0) {
            throw new ArgumentException("Length cannot be below 0");
        }

        char[] chars = new char[length];
        if (length == 0) {
            return chars;
        }

        unsafe {
            byte[] b = this.buffer8;
            Stream s = this.stream;
            int i = 0;
            fixed (char* cptr = chars) {
                while (length > 3) {
                    ReadExact(s, b, 8);
                    cptr[i + 0] = (char) (ushort) ((b[0] << 8) + (b[1] << 0));
                    cptr[i + 1] = (char) (ushort) ((b[2] << 8) + (b[3] << 0));
                    cptr[i + 2] = (char) (ushort) ((b[4] << 8) + (b[5] << 0));
                    cptr[i + 3] = (char) (ushort) ((b[6] << 8) + (b[7] << 0));
                    length -= 4;
                    i += 4;
                }

                switch (length) {
                    case 3:
                        ReadExact(s, b, 6);
                        cptr[i + 0] = (char) (ushort) ((b[0] << 8) + (b[1] << 0));
                        cptr[i + 1] = (char) (ushort) ((b[2] << 8) + (b[3] << 0));
                        cptr[i + 2] = (char) (ushort) ((b[4] << 8) + (b[5] << 0));
                    break;
                    case 2:
                        ReadExact(s, b, 4);
                        cptr[i + 0] = (char) (ushort) ((b[0] << 8) + (b[1] << 0));
                        cptr[i + 1] = (char) (ushort) ((b[2] << 8) + (b[3] << 0));
                    break;
                    case 1:
                        ReadExact(s, b, 2);
                        cptr[i] = (char) (ushort) ((b[0] << 8) + (b[1] << 0));
                    break;
                }

                return chars;
            }
        }
    }

    public char[] ReadCharsUTF8(int length) {
        if (length < 0) {
            throw new ArgumentException("Length cannot be below 0");
        }

        char[] chars = new char[length];
        if (length == 0) {
            return chars;
        }

        unsafe {
            Stream s = this.stream;
            byte[] b = this.buffer8;
            int i = 0;
            fixed (char* cptr = chars) {
                while (length > 7) {
                    ReadExact(s, b, 8);
                    cptr[i + 0] = (char) b[0];
                    cptr[i + 1] = (char) b[1];
                    cptr[i + 2] = (char) b[2];
                    cptr[i + 3] = (char) b[3];
                    cptr[i + 4] = (char) b[4];
                    cptr[i + 5] = (char) b[5];
                    cptr[i + 6] = (char) b[6];
                    cptr[i + 7] = (char) b[7];
                    length -= 8;
                    i += 8;
                }

                if (length > 3) {
                    ReadExact(s, b, 4);
                    cptr[i + 0] = (char) b[0];
                    cptr[i + 1] = (char) b[1];
                    cptr[i + 2] = (char) b[2];
                    cptr[i + 3] = (char) b[3];
                    length -= 4;
                    i += 4;
                }

                switch (length) {
                    case 3:
                        ReadExact(s, b, 3);
                        cptr[i + 0] = (char) b[0];
                        cptr[i + 1] = (char) b[1];
                        cptr[i + 2] = (char) b[2];
                    break;
                    case 2:
                        ReadExact(s, b, 2);
                        cptr[i + 0] = (char) b[0];
                        cptr[i + 1] = (char) b[1];
                    break;
                    case 1:
                        ReadExact(s, b, 1);
                        cptr[i] = (char) b[0];
                    break;
                }

                return chars;
            }
        }
    }

    public char[] ReadCharsUTF16Labelled() {
        return this.ReadCharsUTF16(this.ReadUShort());
    }

    public char[] ReadCharsUTF8Labelled() {
        return this.ReadCharsUTF8(this.ReadUShort());
    }

    public char[] ReadChars(Encoding encoding) {
        int count = this.ReadUShort();
        byte[] buffer = new byte[count];
        ReadExact(this.stream, buffer, count);

        char[] result = new char[encoding.GetCharCount(buffer, 0, count)];
        encoding.GetChars(buffer, 0, count, result, 0);
        return result;
    }

    public string ReadStringUTF16(int len) {
        return new string(this.ReadCharsUTF16(len));
    }

    public string ReadStringUTF8(int len) {
        return new string(this.ReadCharsUTF8(len));
    }

    public string ReadStringUTF16Labelled() {
        return new string(this.ReadCharsUTF16(this.ReadUShort()));
    }

    public string ReadStringUTF8Labelled() {
        return new string(this.ReadCharsUTF8(this.ReadUShort()));
    }

    public string ReadString(Encoding encoding) {
        byte[] buffer = new byte[this.ReadUShort()];
        ReadExact(this.stream, buffer, buffer.Length);
        return encoding.GetString(buffer, 0, buffer.Length);
    }

    public unsafe void ReadPtr(byte* dest, int offset, int length) {
        byte[] b = this.buffer8;
        Stream s = this.stream;
        fixed (byte* buf = b) {
            while (length > 7) {
                ReadExact(s, b, 8);
                *(ulong*) (dest + offset) = *(ulong*) buf;
                length -= 8;
                offset += 8;
            }

            if (length == 0) {
                return;
            }

            if (length > 3) {
                ReadExact(s, b, 4);
                *(uint*) (dest + offset) = *(uint*) buf;
                length -= 4;
                offset += 4;
            }

            switch (length) {
                case 3:
                    ReadExact(s, b, 3);
                    dest[offset + 0] = buf[0];
                    dest[offset + 1] = buf[1];
                    dest[offset + 2] = buf[2];
                break;
                case 2:
                    ReadExact(s, b, 2);
                    dest[offset + 0] = buf[0];
                    dest[offset + 1] = buf[1];
                break;
                case 1:
                    ReadExact(s, b, 1);
                    dest[offset] = buf[0];
                break;
            }
        }
    }

    public void ReadPtr(IntPtr dest, int offset, int length) {
        byte[] b = this.buffer8;
        Stream s = this.stream;
        while (length > 7) {
            ReadExact(s, b, 8);
            Marshal.WriteInt64(dest, offset, (long) (((ulong) b[0] << 56) +
                                                     ((ulong) b[1] << 48) +
                                                     ((ulong) b[2] << 40) +
                                                     ((ulong) b[3] << 32) +
                                                     ((ulong) b[4] << 24) +
                                                     ((ulong) b[5] << 16) +
                                                     ((ulong) b[6] << 8) +
                                                     ((ulong) b[7] << 0)));
            length -= 8;
            offset += 8;
        }

        if (length > 3) {
            ReadExact(s, b, 4);
            Marshal.WriteInt32(dest, offset, (int) (((uint) b[0] << 24) +
                                                    ((uint) b[1] << 16) +
                                                    ((uint) b[2] << 8) +
                                                    ((uint) b[3] << 0)));
            length -= 4;
            offset += 4;
        }

        switch (length) {
            case 3:
                ReadExact(s, b, 3);
                Marshal.WriteByte(dest, offset + 0, b[0]);
                Marshal.WriteByte(dest, offset + 1, b[1]);
                Marshal.WriteByte(dest, offset + 2, b[2]);
                return;
            case 2:
                ReadExact(s, b, 2);
                Marshal.WriteByte(dest, offset + 0, b[0]);
                Marshal.WriteByte(dest, offset + 1, b[1]);
                return;
            case 1:
                ReadExact(s, b, 1);
                Marshal.WriteByte(dest, offset, b[0]);
                return;
        }
    }

    public T ReadStruct<T>() where T : unmanaged {
        unsafe {
            T t = new T();
            this.ReadPtr((byte*) &t, 0, sizeof(T));
            return t;
        }
    }

    public T ReadStruct<T>(T value) where T : unmanaged {
        unsafe {
            this.ReadPtr((byte*) &value, 0, sizeof(T));
            return value;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReadExact(int count) {
        if (this.stream.Read(this.buffer8, 0, count) != count) {
            throw new EndOfStreamException($"Failed to read {count} bytes");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ReadExact(Stream stream, byte[] buffer, int count) {
        if (stream.Read(buffer, 0, count) != count) {
            throw new EndOfStreamException($"Failed to read {count} bytes");
        }
    }
}