using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using JetPacketSystem.Exceptions;
using JetPacketSystem.Streams;

namespace JetPacketSystem.Sockets;

/// <summary>
/// A reusable client connection. This will wait until the server has
/// accepted a socket connection, and then allowing data to be transceived
/// </summary>
public class SocketToServerConnection : BaseConnection {
    private readonly EndPoint endPoint;
    private readonly SocketType socketType;
    private readonly ProtocolType socketProtocol;
    private NetworkDataStream? stream;

    /// <summary>
    /// The data stream which is linked to the server
    /// </summary>
    public override DataStream Stream => this.stream ?? throw new ConnectionStatusException("Not connected", false);

    /// <summary>
    /// Whether this client is logically connected (as in we have a socket).
    /// The socket may have timed out so check that too
    /// </summary>
    public override bool IsConnected => this.Socket != null;

    /// <summary>
    /// The socket which links to the server
    /// </summary>
    public Socket? Socket { get; private set; }

    /// <summary>
    /// Whether to use little endianness or big endianness (aka the order of bytes in big data types)
    /// </summary>
    public bool UseLittleEndianness { get; set; }

    public SocketToServerConnection(IPAddress ip, int port, SocketType socketType = SocketType.Stream, ProtocolType protocol = ProtocolType.Tcp) : this(new IPEndPoint(ip, port), socketType, protocol) { 
    }
    
    public SocketToServerConnection(EndPoint endPoint, SocketType socketType = SocketType.Stream, ProtocolType protocol = ProtocolType.Tcp) {
        this.socketType = socketType;
        this.socketProtocol = protocol;
        this.endPoint = endPoint;
    }

    /// <summary>
    ///
    /// </summary>
    /// <exception cref="ObjectDisposedException">The object is disposed</exception>
    /// <exception cref="ConnectionStatusException">The connection is already open</exception>
    /// <exception cref="ConnectionFailureException">Failed to open the connection</exception>
    /// <exception cref="IOException">An IO exception, most likely the network stream failed to open</exception>
    public override void Connect() {
        if (this.isDisposed) {
            throw new ObjectDisposedException("Cannot connect once the instance has been disposed!");
        }

        if (this.Socket != null) {
            throw new ConnectionStatusException("Already connected!", true);
        }

        Socket newSocket = new Socket(this.socketType, this.socketProtocol) {
            ReceiveTimeout = 20000,
            SendTimeout = 20000
        };
        
        try {
            newSocket.Connect(this.endPoint);
        }
        catch (Exception e) {
            newSocket.Close();
            throw new ConnectionFailureException($"Failed to connect to {this.endPoint}", e);
        }

        try {
            this.stream = this.CreateDataStream(newSocket);
        }
        catch (Exception e) {
            newSocket.Close();
            throw new IOException("Failed to create network data stream", e);
        }

        this.Socket = newSocket;
    }

    /// <summary>
    /// Attempts to connect asynchronously
    /// </summary>
    /// <exception cref="ObjectDisposedException">The object is disposed</exception>
    /// <exception cref="ConnectionStatusException">The connection is already open</exception>
    /// <exception cref="ConnectionFailureException">Failed to open the connection</exception>
    /// <exception cref="IOException">An IO exception, most likely the network stream failed to open</exception>
    public async Task ConnectAsync() {
        if (this.isDisposed) {
            throw new ObjectDisposedException("Cannot connect once the instance has been disposed!");
        }

        if (this.Socket != null) {
            throw new ConnectionStatusException("Already connected!", true);
        }

        Socket newSocket = new Socket(this.socketType, this.socketProtocol) {
            ReceiveTimeout = 20000,
            SendTimeout = 20000
        };
        
        try {
            await newSocket.ConnectAsync(this.endPoint);
        }
        catch(Exception e) {
            newSocket.Close();
            throw new ConnectionFailureException($"Failed to connect to {this.endPoint}", e);
        }

        try {
            this.stream = this.CreateDataStream(newSocket);
        }
        catch (Exception e) {
            newSocket.Close();
            throw new IOException("Failed to create network data stream", e);
        }

        this.Socket = newSocket;
    }

    public override void Disconnect() {
        if (this.isDisposed) {
            throw new ObjectDisposedException("Cannot disconnect once the instance has been disposed!");
        }

        if (this.Socket == null) {
            throw new ConnectionStatusException("Already disconnected!", false);
        }

        this.stream!.Dispose();
        this.stream = null;
        this.Socket.Close();
        this.Socket = null;
    }

    protected NetworkDataStream CreateDataStream(Socket theSocket) {
        return this.UseLittleEndianness ? NetworkDataStream.LittleEndianness(theSocket) : NetworkDataStream.BigEndianness(theSocket);
    }
}