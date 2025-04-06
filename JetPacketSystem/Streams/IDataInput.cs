using System;
using System.IO;
using System.Text;

namespace JetPacketSystem.Streams;

/// <summary>
/// An interface for reading data from a stream
/// </summary>
public interface IDataInput {
    /// <summary>
    /// The base stream that this data input will read from
    /// </summary>
    Stream Stream { get; set; }

    /// <summary>
    /// Closes the stream
    /// </summary>
    void Close();

    /// <summary>
    /// Reads the given number of bytes from the stream
    /// </summary>
    /// <param name="dest">The buffer to read into</param>
    /// <param name="offset">The offset in the buffer</param>
    /// <param name="count">The number of bytes to read</param>
    int Read(byte[] dest, int offset, int count);

    /// <summary>
    /// Reads the exact number of bytes (specified by the given buffer's size) into the buffer (starting at 0)
    /// <para>
    /// Invoking this is the exact same as invoking <see cref="ReadFully(byte[], int, int)"/>,
    /// where the offset is 0 and the length is the given buffer's length
    /// </para>
    /// </summary>
    /// <param name="dest">The buffer to put the bytes into</param>
    void ReadFully(byte[] dest);

    /// <summary>
    /// Reads the exact given number of bytes into the given buffer (starting at the given offset)
    /// <para>
    /// The size of the buffer WILL NOT be checked, so it will throw an out of bounds exception if you mess up
    /// </para>
    /// </summary>
    /// <param name="dest">The buffer to put bytes into</param>
    /// <param name="offset">The index specifying where to start writing into the buffer (inclusive)</param>
    /// <param name="length">The number of bytes to read (e.g 4 for an integer, 2 for a short, etc)</param>
    void ReadFully(byte[] dest, int offset, int length);

    /// <summary>
    /// Reads the given number of bytes. This is similar to invoking <see cref="ReadFully(byte[])"/>
    /// </summary>
    /// <param name="length">The number of characters to read</param>
    byte[] ReadBytes(int count);

    /// <summary>
    /// Reads a short (2 byte) values representing the number of bytes to read, and then reads that many characters
    /// <para>
    /// The total number of bytes is equal to 2 + length
    /// </para>
    /// </summary>
    /// <param name="length">The number of characters to read</param>
    byte[] ReadBytesLabelled();

    /// <summary>
    /// Reads 1 byte and return true if its value is 1, otherwise false
    /// </summary>
    bool ReadBool();

    /// <summary>
    /// Reads 1 signed <see cref="sbyte"/> (1 sign bit + 7 value bits)
    /// </summary>
    sbyte ReadSByte();

    /// <summary>
    /// Reads 1 unsigned <see cref="byte"/> (8 value bits)
    /// </summary>
    byte ReadByte();

    /// <summary>
    /// Reads 2 bytes and joins them into a <see cref="short"/> value (1 sign bit + 15 value bits)
    /// </summary>
    short ReadShort();

    /// <summary>
    /// Reads 2 bytes and joins them into a <see cref="ushort"/> value (16 value bits)
    /// </summary>
    ushort ReadUShort();

    /// <summary>
    /// Reads 4 bytes and joins them into an <see cref="int"/> value (1 sign bit + 31 value bits)
    /// </summary>
    int ReadInt();

    /// <summary>
    /// Reads 4 bytes and joins them into a <see cref="uint"/> value (16 value bits)
    /// </summary>
    uint ReadUInt();

    /// <summary>
    /// Reads 8 bytes and joins them into a <see cref="long"/> value (1 sign bit + 63 value bits)
    /// </summary>
    long ReadLong();

    /// <summary>
    /// Reads 8 bytes and joins them into a <see cref="ulong"/> value (64 value bits)
    /// </summary>
    ulong ReadULong();

    /// <summary>
    /// Reads 4 bytes, joins them into a <see cref="uint"/> value, and uses unsafe code to cast it to a <see cref="float"/>
    /// </summary>
    float ReadFloat();

    /// <summary>
    /// Reads 8 bytes, joins them into a <see cref="ulong"/> value, and uses unsafe code to cast it to a <see cref="double"/>
    /// </summary>
    double ReadDouble();

    /// <summary>
    /// Reads 1 byte and converts it to an enum. This requires that the enum type's size is equal to the
    /// size of a byte, otherwise you may lose the extra data (e.g if the enum's value is above 255),
    /// resulting in undefined behaviour
    /// </summary>
    /// <typeparam name="T">The enum type</typeparam>
    /// <returns>The enum itself</returns>
    T ReadEnum08<T>() where T : unmanaged, Enum;

    /// <summary>
    /// Reads 2 bytes and converts it to an enum. This requires that the enum type's size is smaller than or equal to the
    /// size of a short/ushort, otherwise you may lose the extra data, resulting in undefined behaviour
    /// </summary>
    /// <typeparam name="T">The enum type</typeparam>
    /// <returns>The enum itself</returns>
    T ReadEnum16<T>() where T : unmanaged, Enum;

    /// <summary>
    /// Reads 4 bytes and converts it to an enum. This requires that the enum type's size is smaller than or equal to the
    /// size of a int/uint, otherwise you may lose the extra data, resulting in undefined behaviour
    /// </summary>
    /// <typeparam name="T">The enum type</typeparam>
    /// <returns>The enum itself</returns>
    T ReadEnum32<T>() where T : unmanaged, Enum;

    /// <summary>
    /// Reads 8 bytes and converts it to an enum. This requires that the enum type's size is smaller than or equal to the
    /// size of a long/ulong, otherwise you may lose the extra data, resulting in undefined behaviour
    /// </summary>
    /// <typeparam name="T">The enum type</typeparam>
    /// <returns>The enum itself</returns>
    T ReadEnum64<T>() where T : unmanaged, Enum;

    /// <summary>
    /// Reads 2 bytes, joins them into a <see cref="ushort"/> value, and casts it to a <see cref="char"/>
    /// </summary>
    char ReadCharUTF16();

    /// <summary>
    /// Reads 1 byte (low byte), and casts it to a <see cref="char"/>
    /// </summary>
    char ReadCharUTF8();

    /// <summary>
    /// Reads a character using the given encoding
    /// </summary>
    /// <param name="encoding"></param>
    /// <returns></returns>
    char ReadChar(Encoding encoding);

    /// <summary>
    /// Reads the given number of characters into a character array. This reads UTF16 chars, so
    /// each character is 2 bytes, meaning, reading 10 chars will read 20 bytes
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    char[] ReadCharsUTF16(int length);

    /// <summary>
    /// Reads the given number of characters into a character array. This reads UTF8 chars, so
    /// each character is 1 bytes, meaning, reading 10 chars will read 10 bytes
    /// </summary>
    /// <param name="length">The number of chars (and therefore bytes) to read</param>
    /// <returns>
    /// An array with the exact same size of the given length
    /// </returns>
    char[] ReadCharsUTF8(int length);

    /// <summary>
    /// Reads a short (2 byte) values representing the number of chars to read, and then reads that many characters
    /// <para>
    /// The total number of bytes is equal to 2 + (length * 2), because UTF16 uses 2 bytes per char
    /// </para>
    /// </summary>
    char[] ReadCharsUTF16Labelled();

    /// <summary>
    /// Reads a short (2 byte) values representing the number of chars to read, and then reads that many characters
    /// <para>
    /// The total number of bytes is equal to 2 + length, because UTF8 uses 1 byte per char
    /// </para>
    /// </summary>
    char[] ReadCharsUTF8Labelled();

    /// <summary>
    /// Reads an array of characters using the given encoding
    /// <para>
    /// Supplying a length parameter is not required as it is always sent
    /// </para>
    /// </summary>
    /// <param name="encoding">The encoding used to decode the bytes</param>
    /// <returns></returns>
    char[] ReadChars(Encoding encoding);

    /// <summary>
    /// Reads the given number of characters, and joins them into a string
    /// <para>
    /// Invoking this is the exact same as invoking <see cref="ReadCharsUTF16(int)"/>, only passing the char array into the string's constructor
    /// </para>
    /// </summary>
    /// <param name="length">The number of characters to read</param>
    /// <returns></returns>
    string ReadStringUTF16(int length);

    /// <summary>
    /// Reads the given number of characters, and joins them into a string
    /// <para>
    /// Invoking this is the exact same as invoking <see cref="ReadCharsUTF16(int)"/>, only passing the char array into the string's constructor
    /// </para>
    /// </summary>
    /// <param name="length">The number of characters to read</param>
    /// <returns></returns>
    string ReadStringUTF8(int length);

    /// <summary>
    /// Reads a short (2 byte) values representing the number of chars to read, and then reads that many characters into a string
    /// <para>
    /// This is the exact same as <see cref="ReadCharsUTF16Labelled"/>, but it crates a string for convenience
    /// </para>
    /// <para>
    /// The total number of bytes is equal to 2 + (length * 2), because UTF16 uses 2 bytes per char
    /// </para>
    /// </summary>
    string ReadStringUTF16Labelled();

    /// <summary>
    /// Reads a short (2 byte) values representing the number of chars to read, and then reads that many characters into a string
    /// <para>
    /// This is the exact same as <see cref="ReadCharsUTF8Labelled"/>, but it crates a string for convenience
    /// </para>
    /// <para>
    /// The total number of bytes is equal to 2 + length, because UTF8 uses 1 byte per char
    /// </para>
    /// </summary>
    string ReadStringUTF8Labelled();

    /// <summary>
    /// Reads a string with the given encoding. Providing the string's length is not required, because the length is always sent
    /// </summary>
    /// <param name="length">The number of characters to read</param>
    /// <returns></returns>
    string ReadString(Encoding encoding);

    /// <summary>
    /// Reads 'length' number of bytes into the given pointer buffer (starting at the given offset index)
    /// <para>
    /// The data read will be in your processor architecture's endianness, which for modern hardware is little-endianness.
    /// However, most of the functions in this library read in big-endianness (e.g <see cref="ReadUInt"/>)
    /// </para>
    /// </summary>
    /// <param name="dest">The buffer to write bytes into</param>
    /// <param name="offset">The index of where to start writing into the pointer (inclusive)</param>
    /// <param name="length">The give number of bytes to read</param>
    unsafe void ReadPtr(byte* dest, int offset, int length);

    /// <summary>
    /// Reads 'length' number of bytes into the given pointer (starting at the given offset index)
    /// <para>
    /// The data read will be in your processor architecture's endianness, which for modern hardware is little-endianness.
    /// However, most of the functions in this library read in big-endianness (e.g <see cref="ReadUInt"/>)
    /// </para>
    /// </summary>
    /// <param name="dest">The pointer which should point to a buffer, which is where bytes will be written to</param>
    /// <param name="offset">The index of where to start writing into the pointer/buffer (inclusive)</param>
    /// <param name="length">The give number of bytes to read</param>
    void ReadPtr(IntPtr dest, int offset, int length);

    /// <summary>
    /// Reads a verified unmanaged type, where the given number of bytes will be read as the given type
    /// <para>
    /// The number of bytes will be equivalent to the size of the generic unmanaged type passed to the method.
    /// E.g, reading an integer (Int32) will read 4 bytes
    /// </para>
    /// <para>
    /// The data read will be in your processor architecture's endianness, which for modern hardware is little-endianness
    /// </para>
    /// </summary>
    /// <typeparam name="T">The unmanaged type</typeparam>
    T ReadStruct<T>() where T : unmanaged;

    /// <summary>
    /// Reads a verified unmanaged type, where the given number of bytes will be read into the given value instance
    /// <para>
    /// The number of bytes will be equivalent to the size of the generic unmanaged type passed to the method.
    /// E.g, reading an integer (Int32) will read 4 bytes
    /// </para>
    /// <para>
    /// The data read will be in your processor architecture's endianness, which for modern hardware is little-endianness
    /// </para>
    /// </summary>
    /// <typeparam name="T">The unmanaged type</typeparam>
    T ReadStruct<T>(T value) where T : unmanaged;
}