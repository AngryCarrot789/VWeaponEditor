using System;
using JetPacketSystem.Exceptions;
using JetPacketSystem.Packeting.Ack.Attribs;
using JetPacketSystem.Streams;

namespace JetPacketSystem.Packeting.Ack;

/// <summary>
/// A packet that supports send, acknowledgement and receive between a server and client
/// <para>
/// 
/// </para>
/// </summary>
public abstract class PacketACK : Packet {
    // Packet data structure
    // [    4b    ] [ 2b ] [  Len-b  ]
    // [ Protocol ] [ ID ] [ Payload ]
    //                     |         |
    // ACK Packet data     |         |
    // _ _ _ _ _ _ _ _ _ _ |         | _ _ _ _ _ 
    // [ Ack ID & Destination ] [  ACK Payload  ]
    // [          4b          ] [   (Len-4)b    ]
    private const int HEAD_SIZE = 4;           // ACK Header size in bytes
    private const int KEY_SHIFT = 2;           // How many times to bitshift the key (must reflect with DEST_MASK)
    private const uint DEST_MASK = 0b0011;     // The destination bit mask (must reflect with KEY_SHIFT)

    /// <summary>
    /// The ACK packet's idempotency key
    /// </summary>
    public uint key;

    /// <summary>
    /// The destination of this packet
    /// </summary>
    public Destination destination;

    protected PacketACK() {

    }

    public override void ReadPayLoad(IDataInput input) {
        uint kd = input.ReadUInt();
        this.key = kd >> KEY_SHIFT;
        Destination dest = (Destination) (kd & DEST_MASK);
        switch (dest) {
            case Destination.ToServer:
                this.destination = Destination.Ack;
                try {
                    this.ReadPayloadFromClient(input);
                }
                catch (Exception e) {
                    throw new PacketPayloadException($"Failed to read payload from client, for ACK packet type '{this.GetType().Name}'", e);
                }

            break;
            case Destination.ToClient:
                this.destination = Destination.ToClient;
                try {
                    this.ReadPayloadFromServer(input);
                }
                catch (Exception e) {
                    throw new PacketPayloadException($"Failed to read payload from server, for ACK packet type '{this.GetType().Name}'", e);
                }

            break;
            case Destination.Ack:
                throw new PacketPayloadException($"Received ACK destination, for ACK packet type '{this.GetType().Name}'");
            default:
                throw new PacketPayloadException($"Received invalid ACK code '{dest}', for ACK packet type '{this.GetType().Name}'");
        }
    }

    public override void WritePayload(IDataOutput output) {
        Destination dest = this.destination;
        switch (dest) {
            case Destination.Ack: throw new Exception($"Attempted to write {Destination.Ack}. Packet should've been recreated with {Destination.ToClient}");
            case Destination.ToServer:
            case Destination.ToClient: {
                output.WriteUInt((this.key << KEY_SHIFT) | ((uint) dest & DEST_MASK));
                if (dest == Destination.ToServer) {
                    try {
                        this.WritePayloadToServer(output);
                    }
                    catch (Exception e) {
                        throw new PacketPayloadException($"Failed to write payload to server for ACK packet type '{this.GetType().Name}'", e);
                    }
                }
                else {
                    try {
                        this.WritePayloadToClient(output);
                    }
                    catch (Exception e) {
                        throw new PacketPayloadException($"Failed to write payload to client for ACK packet type '{this.GetType().Name}'", e);
                    }
                }

                break;
            }

            default: throw new PacketPayloadException("Attempted to write unknown Destination code: " + dest);
        }
    }
    
    // These 4 methods ordered based on the data transaction between client and server:
    // -  1) Client Side: WritePayloadToServer (write request info)
    // -  2) Server Side: ReadPayloadFromClient (read request info)
    // -  3) Server Side: WritePayloadToClient (write response info)
    // -  4) Client Side: ReadPayloadFromServer (read response info)
    
    /// <summary>
    /// Writes the data to the server (this will be executed client side)
    /// </summary>
    [ClientSide]
    public abstract void WritePayloadToServer(IDataOutput output);
    
    /// <summary>
    /// Reads the data that the client has sent to the server (this will be executed server side)
    /// </summary>
    [ServerSide]
    public abstract void ReadPayloadFromClient(IDataInput input);
    
    /// <summary>
    /// Writes the data to the client (this will be executed server side)
    /// </summary>
    [ServerSide]
    public abstract void WritePayloadToClient(IDataOutput output);
    
    /// <summary>
    /// Reads the data that the server has sent to the client (this will be executed client side)
    /// </summary>
    [ClientSide]
    public abstract void ReadPayloadFromServer(IDataInput input);

    public override string ToString() {
        return $"{this.GetType().Name}({this.key} -> {this.destination})";
    }
}