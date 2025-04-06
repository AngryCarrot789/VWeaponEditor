using System;
using System.Collections.Generic;
using JetPacketSystem.Exceptions;
using JetPacketSystem.Packeting;

namespace JetPacketSystem.Systems.Handling;

/// <summary>
/// A map, mapping a priority to a collection of listeners and handlers
/// <para>
/// Listeners of the same priority as handlers will receive packets first, e.g a listener with priotity 1 
/// will receive packets first, then handlers or priority 1 will receive packets. But, listeners of priority 2 will
/// receive packets AFTER handlers of priority 1
/// </para>
/// <para>
/// The order of received packets being delivered is:
/// Listeners(HIGHEST), Handers(HIGHEST), Listeners(HIGH), Handers(HIGH), Listeners(NORMAL),
/// Handers(NORMAL), Listeners(LOW), Handers(LOW), Listeners(LOWEST), Handers(LOWEST), 
/// </para>
/// <para>
/// This means listeners have an overall higher priority than handlers of the same 
/// priority level, which may be useful for "sniffing" packets before they get handled. Listeners
/// cannot cancel the packet (stop them reaching other listeners or handlers) though, only handlers can
/// </para>
/// </summary>
public class ListenerMap {
    private readonly List<IPacketHandler>[] handlers;
    private readonly List<IListener>[] listeners;

    public ListenerMap() {
        this.handlers = new List<IPacketHandler>[5];
        this.listeners = new List<IListener>[5];
        this.handlers[(int) Priority.HIGHEST]  = new List<IPacketHandler>();
        this.handlers[(int) Priority.HIGH]     = new List<IPacketHandler>();
        this.handlers[(int) Priority.NORMAL]   = new List<IPacketHandler>();
        this.handlers[(int) Priority.LOW]      = new List<IPacketHandler>();
        this.handlers[(int) Priority.LOWEST]   = new List<IPacketHandler>();
        this.listeners[(int) Priority.HIGHEST] = new List<IListener>();
        this.listeners[(int) Priority.HIGH]    = new List<IListener>();
        this.listeners[(int) Priority.NORMAL]  = new List<IListener>();
        this.listeners[(int) Priority.LOW]     = new List<IListener>();
        this.listeners[(int) Priority.LOWEST]  = new List<IListener>();
    }

    public void Clear() {
        foreach (List<IPacketHandler> collection in this.handlers) {
            collection.Clear();
        }

        foreach (List<IListener> collection in this.listeners) {
            collection.Clear();
        }
    }

    public List<IPacketHandler> GetHandlers(Priority priority) {
        int index = (int) priority;
        if (index < 0 || index > 4) {
            throw new Exception("Missing priority (not 0-4): " + priority);
        }

        return this.handlers[index];
    }

    public List<IListener> GetListeners(Priority priority) {
        int index = (int) priority;
        if (index < 0 || index > 4) {
            throw new Exception("Missing priority (not 0-4): " + priority);
        }

        return this.listeners[index];
    }

    public IPacketHandler AddHandler(Priority priority, IPacketHandler handler) {
        this.GetHandlers(priority).Add(handler);
        return handler;
    }

    public IListener AddListener(Priority priority, IListener listener) {
        this.GetListeners(priority).Add(listener);
        return listener;
    }

    /// <summary>
    /// Delivers the given packet to all of the listeners and handlers, respecting their priority
    /// </summary>
    /// <param name="packet"></param>
    /// <returns></returns>
    public bool DeliverPacket(Packet packet) {
        if (this.HandlePriority(Priority.HIGHEST, packet)) {
            return true;
        }

        if (this.HandlePriority(Priority.HIGH, packet)) {
            return true;
        }

        if (this.HandlePriority(Priority.NORMAL, packet)) {
            return true;
        }

        if (this.HandlePriority(Priority.LOW, packet)) {
            return true;
        }

        if (this.HandlePriority(Priority.LOWEST, packet)) {
            return true;
        }

        return false;
    }

    private bool HandlePriority(Priority priority, Packet packet) {
        try {
            this.HandleListenerPriority(priority, packet);
        }
        catch(Exception e) {
            throw new PacketHandlerException(packet, priority, true, e);
        }

        try {
            return this.HandleHandlersPriority(priority, packet);
        }
        catch (Exception e) {
            throw new PacketHandlerException(packet, priority, false, e);
        }
    }

    private void HandleListenerPriority(Priority priority, Packet packet) {
        foreach (IListener listener in this.listeners[(int) priority]) {
            listener.OnReceived(packet);
        }
    }

    private bool HandleHandlersPriority(Priority priority, Packet packet) {
        foreach (IPacketHandler handler in this.handlers[(int) priority]) {
            if (handler.CanProcess(packet)) {
                if (handler.OnHandlePacket(packet)) {
                    return true;
                }
            }
        }

        return false;
    }
}