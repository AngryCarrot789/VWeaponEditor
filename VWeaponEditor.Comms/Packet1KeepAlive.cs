using JetPacketSystem.Packeting;
using JetPacketSystem.Streams;

namespace VWeaponEditor.Comms;

public sealed class Packet1KeepAlive : Packet {
    public override void ReadPayLoad(IDataInput input) {
    }

    public override void WritePayload(IDataOutput output) {
    }
}