using JetPacketSystem.Packeting.Ack;
using JetPacketSystem.Streams;

namespace VWeaponEditor.Comms;

public class Packet2GetUserName : PacketACK {
    public string? name;

    public override void WritePayloadToServer(IDataOutput output) {
    }

    public override void ReadPayloadFromClient(IDataInput input) {
    }

    public override void WritePayloadToClient(IDataOutput output) {
        output.WriteStringLabelledUTF16(this.name ?? "");
    }
    
    public override void ReadPayloadFromServer(IDataInput input) {
        this.name = input.ReadStringUTF16Labelled();
    }
}