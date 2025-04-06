using JetPacketSystem.Packeting;

namespace VWeaponEditor.Comms;

public static class VWEPacketRegistry {
    public static void RegisterScriptPackets() {
        RegisterCommonPackets();
    }
    
    public static void RegisterApplicationPackets() {
        RegisterCommonPackets();
    }

    private static void RegisterCommonPackets() {
        Packet.Register(1, () => new Packet1KeepAlive());
        Packet.Register(2, () => new Packet2GetUserName());
        Packet.Register(3, () => new Packet3TranslateHashString());
    }
}