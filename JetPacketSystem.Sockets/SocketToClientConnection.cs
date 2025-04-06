using System;
using System.Net;
using System.Net.Sockets;
using JetPacketSystem.Streams;

namespace JetPacketSystem.Sockets;

/// <summary>
/// Represents a one-time connection to the client. When this class is instantated, it is
/// assumed that the socket is already open. So calling <see cref="Connect"/> will do nothing
/// <para>
/// Calling <see cref="Disconnect"/> will fully disconenct and dispose of the socket,
/// meaning you cannot reconnect (it will throw an exception if you try to invoke <see cref="Connect"/>,
/// just for the sake of bug tracking)
/// </para>
/// </summary>
public class SocketToClientConnection : BaseConnection {
    private readonly NetworkDataStream stream;

    /// <summary>
    /// The data stream which is linked to the server
    /// </summary>
    public override DataStream Stream => this.stream;

    /// <summary>
    /// Whether this client is connected to the server
    /// </summary>
    public override bool IsConnected => !this.isDisposed;

    /// <summary>
    /// The socket that this connection is connected to
    /// </summary>
    public Socket Client { get; }

    /// <summary>
    /// The server that this connection uses
    /// </summary>
    public Socket Server { get; }

    public EndPoint LocalEndPoint => this.Client.LocalEndPoint;

    public EndPoint RemoteEndPoint => this.Client.RemoteEndPoint;

    public SocketToClientConnection(Socket client, Socket server, bool useLittleEndianness = false) {
        this.Client = client ?? throw new ArgumentNullException(nameof(client), "Client is null");
        this.Server = server ?? throw new ArgumentNullException(nameof(server), "Server is null");
        this.stream = useLittleEndianness ? NetworkDataStream.LittleEndianness(this.Client) : NetworkDataStream.BigEndianness(this.Client);
    }

    public override void Connect() {
        if (this.isDisposed) {
            throw new ObjectDisposedException("Cannot reconnect once the instance has been disposed!");
        }
    }

    public override void Disconnect() {
        if (this.isDisposed) {
            throw new ObjectDisposedException("Cannot disconnect once the instance has been disposed!");
        }

        this.Client.Disconnect(false);
        this.stream.Dispose();
    }
}