using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace JetPacketSystem.Streams;

/// <summary>
/// A stream wrapper that supports blocking reading (waits until data is available)
/// <para>
/// This stream can be written to and read from,
/// </para>
/// </summary>
public class BlockingStream : Stream {
    private readonly Stream stream;

    /// <summary>
    /// The underlying stream of this blocking stream
    /// </summary>
    public Stream Stream => this.stream;

    public override bool CanRead => this.stream.CanRead;

    public override bool CanSeek => this.stream.CanSeek;

    public override bool CanWrite => this.stream.CanWrite;

    public override long Length => this.stream.Length;

    public override long Position {
        get => this.stream.Position;
        set => this.stream.Position = value;
    }

    public override bool CanTimeout => this.stream.CanTimeout;

    public override int ReadTimeout {
        get => this.stream.ReadTimeout;
        set => this.stream.ReadTimeout = value;
    }

    public override int WriteTimeout {
        get => this.stream.WriteTimeout;
        set => this.stream.WriteTimeout = value;
    }

    /// <summary>
    /// A small array for reading a single byte. Saves creating
    /// an array each time the method is invoked
    /// </summary>
    private readonly byte[] read1 = new byte[1];

    /// <summary>
    /// Creates a new instance of the blocking stream, using the given stream as the underlying stream, that will be used to read from
    /// </summary>
    /// <param name="stream"></param>
    /// <exception cref="NullReferenceException"></exception>
    public BlockingStream(Stream stream) {
        if (stream == null) {
            throw new NullReferenceException("Stream cannot be null");
        }

        this.stream = stream;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Flush() {
        this.stream.Flush();
    }

    /// <summary>
    /// Reads from the stream, blocking until it reads the given count
    /// </summary>
    /// <param name="buffer">The buffer to read into</param>
    /// <param name="offset">The offset to start reading into the buffer</param>
    /// <param name="count">The exact number of bytes to read</param>
    /// <returns>Exactly the specified number of bytes</returns>
    public override int Read(byte[] buffer, int offset, int count) {
        int a = 0;
        Stream s = this.stream;
        while (count > 0) {
            int r = s.Read(buffer, offset + a, count);
            a += Math.Max(0, r);
            count -= r;
        }

        return a;
    }

    /// <summary>
    /// Blocks until a single byte can be read
    /// </summary>
    /// <returns>A single byte</returns>
    public override int ReadByte() {
        Stream s = this.stream;
        byte[] b = this.read1; // reduces ldfld, though the CLR could optimise by itself...
        while (s.Read(b, 0, 1) != 1) { }
        return b[0];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override long Seek(long offset, SeekOrigin origin) {
        return this.stream.Seek(offset, origin);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void SetLength(long value) {
        this.stream.SetLength(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Write(byte[] buffer, int offset, int count) {
        this.stream.Write(buffer, offset, count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected override void Dispose(bool disposing) {
        this.stream.Dispose();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state) {
        return this.stream.BeginRead(buffer, offset, count, callback, state);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state) {
        return this.stream.BeginWrite(buffer, offset, count, callback, state);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Close() {
        this.stream.Close();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken) {
        return this.stream.CopyToAsync(destination, bufferSize, cancellationToken);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int EndRead(IAsyncResult asyncResult) {
        return this.stream.EndRead(asyncResult);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void EndWrite(IAsyncResult asyncResult) {
        this.stream.EndWrite(asyncResult);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Task FlushAsync(CancellationToken cancellationToken) {
        return this.stream.FlushAsync(cancellationToken);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) {
        return this.stream.ReadAsync(buffer, offset, count, cancellationToken);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) {
        return this.stream.WriteAsync(buffer, offset, count, cancellationToken);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void WriteByte(byte value) {
        this.stream.WriteByte(value);
    }

    public override string ToString() {
        return $"{nameof(BlockingStream)} -> {this.stream.GetType()}";
    }
}