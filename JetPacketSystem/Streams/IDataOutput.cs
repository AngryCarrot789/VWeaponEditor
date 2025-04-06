using System;
using System.IO;
using System.Text;

namespace JetPacketSystem.Streams;

/// <summary>
/// An interface for writing data to a stream
/// </summary>
public interface IDataOutput {
    /// <summary>
    /// The base stream that this data output will write to
    /// </summary>
    Stream Stream { get; set; }

    /// <summary>
    /// Flushes the data to the stream
    /// </summary>
    void Flush();

    /// <summary>
    /// Closes the stream
    /// </summary>
    void Close();

    /// <summary>
    /// Writes the given number of bytes, starting at the given offset, from the given buffer
    /// </summary>
    /// <param name="src">The buffer to write data from</param>
    /// <param name="offset">The index to start reading from the buffer</param>
    /// <param name="count">The number of bytes to write</param>
    void Write(byte[] src, int offset, int count);

    /// <summary>
    /// Writes the bytes in the given buffer, starting at the given offset
    /// <para>
    /// The number of bytes written will be: <code>src.Length - offset</code>
    /// </para>
    /// </summary>
    /// <param name="src">The buffer to write data from</param>
    /// <param name="offset">The index to start reading from the buffer</param>
    void Write(byte[] src, int offset = 0);

    /// <summary>
    /// Writes a ushort value representing the given count of bytes,
    /// and then writes the bytes in the given buffer, starting at the given offset
    /// </summary>
    /// <param name="src">The buffer to write data from</param>
    /// <param name="offset">The index to start reading from the buffer</param>
    void WriteLabelled(byte[] src, int offset, int count);

    /// <summary>
    /// Writes a ushort value representing the given array length,
    /// and then writes the bytes in the given buffer, starting at the given offset
    /// <para>
    /// The number of bytes written will be: <code>src.Length - offset</code>
    /// </para>
    /// </summary>
    /// <param name="src">The buffer to write data from</param>
    /// <param name="offset">The index to start reading from the buffer</param>
    void WriteLabelled(byte[] src, int offset = 0);

    /// <summary>
    /// Writes a boolean value (1 byte)
    /// </summary>
    /// <param name="value"></param>
    void WriteBoolean(bool value);

    /// <summary>
    /// Writes a single signed byte (-128 to 127)
    /// </summary>
    /// <param name="value"></param>
    void WriteSByte(sbyte value);

    /// <summary>
    /// Writes a single unsigned byte (0-255)
    /// </summary>
    /// <param name="value"></param>
    void WriteByte(byte value);

    /// <summary>
    /// Writes a signed short (2 bytes) (-32768 to 32767)
    /// </summary>
    /// <param name="value"></param>
    void WriteShort(short value);

    /// <summary>
    /// Writes a short (2 bytes) (0 to 65535)
    /// </summary>
    /// <param name="value"></param>
    void WriteUShort(ushort value);

    /// <summary>
    /// Writes an integer (4 bytes) (-2,147,483,648 to 2,147,483,647)
    /// </summary>
    /// <param name="value"></param>
    void WriteInt(int value);

    /// <summary>
    /// Writes an unsigned integer (4 bytes) (0 to 4,294,967,295)
    /// </summary>
    /// <param name="value"></param>
    void WriteUInt(uint value);

    /// <summary>
    /// Writes a signed long (8 bytes) (-9,223,372,036,854,775,808 to 9,223,372,036,854,775,807)
    /// </summary>
    /// <param name="value"></param>
    void WriteLong(long value);

    /// <summary>
    /// Writes an unsigned long (8 bytes) (0 to 18,446,744,073,709,551,615)
    /// </summary>
    /// <param name="value"></param>
    void WriteULong(ulong value);

    /// <summary>
    /// Writes a floating point number (4 bytes)
    /// </summary>
    /// <param name="value"></param>
    void WriteFloat(float value);

    /// <summary>
    /// Writes a double percision floating point number (8 bytes)
    /// </summary>
    /// <param name="value"></param>
    void WriteDouble(double value);

    /// <summary>
    /// Writes an enum value as a byte
    /// </summary>
    /// <typeparam name="TEnum">The enum type whose size is 1 byte big</typeparam>
    /// <param name="value">The value to write</param>
    void WriteEnum08<TEnum>(TEnum value) where TEnum : unmanaged, Enum;

    /// <summary>
    /// Writes an enum value as a ushort (2 bytes)
    /// </summary>
    /// <typeparam name="TEnum">The enum type whose size is atleast 2 bytes big</typeparam>
    /// <param name="value">The value to write</param>
    void WriteEnum16<TEnum>(TEnum value) where TEnum : unmanaged, Enum;

    /// <summary>
    /// Writes an enum value as a uint (4 bytes)
    /// </summary>
    /// <typeparam name="TEnum">The enum type whose size is atleast 4 bytes big</typeparam>
    /// <param name="value">The value to write</param>
    void WriteEnum32<TEnum>(TEnum value) where TEnum : unmanaged, Enum;

    /// <summary>
    /// Writes an enum value as a ulong value (8 bytes)
    /// </summary>
    /// <typeparam name="TEnum">The enum type whose size is at least 8 bytes big</typeparam>
    /// <param name="value">The value to write</param>
    void WriteEnum64<TEnum>(TEnum value) where TEnum : unmanaged, Enum;

    /// <summary>
    /// Writes a UTF16 char (2 bytes)
    /// </summary>
    /// <param name="value">The value to write</param>
    void WriteCharUTF16(char value);

    /// <summary>
    /// Writes a UTF8 char (1 byte)
    /// </summary>
    /// <param name="value">The value to write</param>
    void WriteCharUTF8(char value);

    /// <summary>
    /// Writes a character using the given encoding
    /// </summary>
    /// <param name="value">The value to write</param>
    /// <param name="encoding">The encoding that will be used to get the bytes of the char</param>
    void WriteChar(char value, Encoding encoding);

    /// <summary>
    /// Writes all of the chars in the given string
    /// </summary>
    /// <param name="value">The string to write</param>
    void WriteStringUTF16(string value);

    /// <summary>
    /// Writes all of the chars in the given string
    /// </summary>
    /// <param name="value">The string to write</param>
    void WriteStringUTF8(string value);

    /// <summary>
    /// Writes the given string using the given encoding
    /// </summary>
    /// <param name="value">The value to write</param>
    /// <param name="encoding">The encoding that will be used to get the bytes of the string</param>
    void WriteString(string value, Encoding encoding);

    /// <summary>
    /// Writes all of the chars in the given string. This writes 2 bytes per char
    /// </summary>
    /// <param name="chars">The chars to write</param>
    void WriteCharsUTF16(char[] chars);

    /// <summary>
    /// Writes all of the chars in the given string. This only writes the low byte of
    /// the char (bit 1-8), and does not send the high byte. Meaning, only 1 byte per char; value range from 0 to 255
    /// </summary>
    /// <param name="chars">The chars to write</param>
    void WriteCharsUTF8(char[] chars);

    /// <summary>
    /// Writes all of the chars in the given char array, using the given encoding
    /// </summary>
    /// <param name="chars">The chars to write</param>
    /// <param name="encoding">The encoding that will be used to get the bytes of the string</param>
    void WriteChars(char[] chars, Encoding encoding);

    /// <summary>
    /// Writes the size of the given array as a ushort value (2 bytes), and then writes
    /// all of the chars in the given array in UTF16 (2 byte per char)
    /// </summary>
    /// <param name="chars">The chars to write</param>
    void WriteCharsLabelledUTF16(char[] chars);

    /// <summary>
    /// Writes the size of the given array as a ushort value (2 bytes), and then writes
    /// all of the chars in the given array in UTF8 (1 byte per char)
    /// </summary>
    /// <param name="chars">The chars to write</param>
    void WriteCharsLabelledUTF8(char[] chars);

    /// <summary>
    /// Writes the length of the string as a ushort value (2 bytes),
    /// and then writes the string in UTF16 (2 bytes per char)
    /// </summary>
    void WriteStringLabelledUTF16(string value);

    /// <summary>
    /// Writes the length of the string as a ushort value (2 bytes),
    /// and then writes the string in UTF8 (1 bytes per char)
    /// </summary>
    void WriteStringLabelledUTF8(string value);

    /// <summary>
    /// Writes '2 * length' bytes from the given pointer (starting, in the pointer, at the given offset)
    /// </summary>
    /// <param name="src">The pointer to get the chars from</param>
    /// <param name="count">The number of characters to write (not bytes, characters)</param>
    unsafe void WriteCharPtrUTF16(char* src, int count);

    /// <summary>
    /// Writes 'length' bytes from the given pointer (starting, in the pointer, at the given offset)
    /// </summary>
    /// <param name="src">The pointer to get the chars from</param>
    /// <param name="count">The number of characters/bytes to write</param>
    unsafe void WriteCharPtrUTF8(char* src, int count);

    /// <summary>
    /// Writes 'count' of chars from the given pointer, starting at the given offset, using the given encoding
    /// </summary>
    /// <param name="cptr">The pointer to get the chars from</param>
    /// <param name="count">The number of characters/bytes to write</param>
    /// <param name="encoding">The encoding used to get the bytes</param>
    unsafe void WriteCharPtr(char* cptr, int count, Encoding encoding);

    /// <summary>
    /// Writes 'length' bytes from the given pointer (starting, in the pointer, at the given offset)
    /// <para>
    /// The data written will be in your processor architecture's endianness, which for modern hardware is little-endianness.
    /// However, most of the functions in this library write in big-endianness (e.g <see cref="WriteUInt"/>)
    /// </para>
    /// </summary>
    /// <param name="src">The pointer to a buffer</param>
    /// <param name="offset">The offset within the pointer (usually this starts at 0)</param>
    /// <param name="count">The number of characters/bytes to write</param>
    unsafe void WritePtr(byte* src, int count);

    /// <summary>
    /// Writes 'length' bytes from the given pointer (starting, in the pointer, at the given offset)
    /// <para>
    /// The data written will be in your processor architecture's endianness, which for modern hardware is little-endianness.
    /// However, most of the functions in this library write in big-endianness (e.g <see cref="WriteUInt"/>)
    /// </para>
    /// </summary>
    /// <param name="src">The pointer to a buffer</param>
    /// <param name="offset">The offset within the pointer (usually this starts at 0)</param>
    /// <param name="count">The number of characters/bytes to write</param>
    void WritePtr(IntPtr src, int count);

    /// <summary>
    /// Writes a blittable value/object, where all of the value's bytes will be written
    /// <para>
    /// The data written will be in your processor architecture's endianness, which for modern hardware is little-endianness.
    /// However, most of the functions in this library write in big-endianness (e.g <see cref="WriteUInt"/>)
    /// </para>
    /// </summary>
    /// <param name="value">The value to write</param>
    /// <typeparam name="T">The blittable type</typeparam>
    void WriteStruct<T>(T value) where T : unmanaged;
}