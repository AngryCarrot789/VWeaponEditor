using System;
using System.IO;

namespace JetPacketSystem.Streams;

/// <summary>
/// A base data stream for sending and receiving data
/// </summary>
public abstract class DataStream : IDisposable {
    protected BlockingStream stream;
    protected IDataInput input;
    protected IDataOutput output;

    /// <summary>
    /// The actual stream that this connection uses
    /// </summary>
    public BlockingStream Stream {
        get => this.stream;
        set => this.stream = value;
    }

    /// <summary>
    /// The data input stream (for reading)
    /// </summary>
    public IDataInput Input {
        get => this.input;
        set => this.input = value;
    }

    /// <summary>
    /// The data output stream (for writing)
    /// </summary>
    public IDataOutput Output {
        get => this.output;
        set => this.output = value;
    }

    /// <summary>
    /// Gets the number of bytes that can be read without blocking
    /// </summary>
    public abstract long BytesAvailable { get; }

    /// <summary>
    /// Creates a new data stream, leaving the stream and data inputs/outputs as null
    /// </summary>
    /// <param name="stream">The stream to use</param>
    /// <exception cref="NullReferenceException">The stream is null</exception>
    protected DataStream() {

    }

    /// <summary>
    /// Creates a new data stream. A data input stream and data output stream will be created automatically
    /// </summary>
    /// <param name="stream">The stream to use</param>
    /// <exception cref="NullReferenceException">The stream is null</exception>
    protected DataStream(Stream stream) {
        if (stream == null) {
            throw new ArgumentNullException(nameof(stream), "Stream cannot be null");
        }

        this.stream = new BlockingStream(stream);
        this.input = this.ProvideInput(this.stream);
        this.output = this.ProvideOutput(this.stream);
    }

    /// <summary>
    /// Creates a new data stream, using the given stream, data input and data output
    /// </summary>
    /// <param name="stream">The stream to use</param>
    /// <param name="input">The data input to use for reading</param>
    /// <param name="output">The data output to use for writing</param>
    /// <exception cref="NullReferenceException">The stream, input or output is null</exception>
    protected DataStream(Stream stream, IDataInput input, IDataOutput output) {
        if (stream == null) {
            throw new ArgumentNullException(nameof(stream), "Stream cannot be null");
        }

        if (input == null) {
            throw new ArgumentNullException(nameof(input), "Data input stream cannot be null");
        }

        if (output == null) {
            throw new ArgumentNullException(nameof(output), "Data output stream cannot be null");
        }

        this.stream = new BlockingStream(stream);
        this.input = input;
        this.output = output;
        this.input.Stream = this.stream;
        this.output.Stream = this.stream;
    }

    /// <summary>
    /// Whether there are any bytes in the input stream
    /// </summary>
    /// <returns></returns>
    public virtual bool CanRead() {
        return this.BytesAvailable > 0;
    }

    /// <summary>
    /// Flushes the write buffer
    /// </summary>
    public virtual void Flush() {
        this.stream.Flush();
    }

    /// <summary>
    /// Disposes the internal stream
    /// </summary>
    public virtual void Dispose() {
        this.input.Stream = null;
        this.output.Stream = null;
        this.stream.Dispose();
    }

    /// <summary>
    /// Provides a data output
    /// </summary>
    protected virtual IDataOutput ProvideOutput(Stream stream) {
        return new DataOutputStream(stream);
    }

    /// <summary>
    /// Provides a data input
    /// </summary>
    protected virtual IDataInput ProvideInput(Stream stream) {
        return new DataInputStream(stream);
    }
}