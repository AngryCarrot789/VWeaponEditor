using JetPacketSystem.Packeting;

namespace JetPacketSystem.Systems.Handling;

/// <summary>
/// An interface for a packet handler
/// </summary>
public interface IPacketHandler {
    /// <summary>
    /// Whether this handler can process the given packet
    /// </summary>
    /// <param name="packet"></param>
    /// <returns>
    /// <see langword="true"/> if it can (meaning <see cref="OnHandlePacket"/> could be executed without problem),
    /// otherwise <see langword="false"/>
    /// </returns>
    bool CanProcess(Packet packet);

    /// <summary>
    /// Try to handle the packet
    /// </summary>
    /// <param name="packet">The packet (not null)</param>
    /// <returns>
    /// <see langword="true"/> if the packet is handled, and shouldn't be processed anymore, otherwise <see langword="false"/>
    /// </returns>
    bool OnHandlePacket(Packet packet);
}