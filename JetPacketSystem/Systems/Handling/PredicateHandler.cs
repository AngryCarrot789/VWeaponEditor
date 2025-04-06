﻿using System;
using JetPacketSystem.Packeting;

namespace JetPacketSystem.Systems.Handling;

public class PredicateHandler : IPacketHandler {
    private readonly Predicate<Packet> handler;
    private readonly Predicate<Packet> canProcess;

    public PredicateHandler(Predicate<Packet> handler, Predicate<Packet> canProcess = null) {
        if (handler == null) {
            throw new NullReferenceException("Handler cannot be null");
        }

        this.handler = handler;
        this.canProcess = canProcess;
    }

    public bool CanProcess(Packet packet) {
        return this.canProcess == null ? true : this.canProcess(packet);
    }

    public bool OnHandlePacket(Packet packet) {
        return this.handler(packet);
    }
}