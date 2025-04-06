using System;
using System.Runtime.Serialization;
using JetPacketSystem.Packeting;

namespace JetPacketSystem.Exceptions;

public class PacketWriteException : PacketException {
    public int WritesAttempted { get; set; }
    public int Written { get; set; }
    public Packet Packet { get; set; }

    public PacketWriteException() {

    }

    public PacketWriteException(string message) : base(message) {
    }

    public PacketWriteException(string message, Exception innerException) : base(message, innerException) {
    }

    protected PacketWriteException(SerializationInfo info, StreamingContext context) : base(info, context) {
    }
}