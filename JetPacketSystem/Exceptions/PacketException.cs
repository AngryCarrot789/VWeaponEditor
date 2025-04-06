using System;
using System.Runtime.Serialization;

namespace JetPacketSystem.Exceptions;

public class PacketException : Exception {
    public PacketException() {
    }

    protected PacketException(SerializationInfo info, StreamingContext context) : base(info, context) {
    }

    public PacketException(string message) : base(message) {
    }

    public PacketException(string message, Exception innerException) : base(message, innerException) {
    }
}