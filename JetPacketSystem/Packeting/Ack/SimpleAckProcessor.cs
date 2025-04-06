using System;
using JetPacketSystem.Systems;

namespace JetPacketSystem.Packeting.Ack;

/// <summary>
/// An <see cref="AckProcessor{TPacket}"/> that uses a lambda method to create a response packet during <see cref="OnProcessPacketFromClient"/>
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class SimpleAckProcessor<T> : AckProcessor<T> where T : PacketACK {
    private readonly Func<T,T> processor;

    public SimpleAckProcessor(PacketSystem system, Func<T, T> processor) : base(system) {
        this.processor = processor;
    }

    protected override bool OnProcessPacketFromClient(T packet) {
        this.SendToClient(packet, this.processor(packet));
        return true;
    }
}