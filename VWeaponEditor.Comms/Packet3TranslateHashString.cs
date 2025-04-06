using JetPacketSystem.Packeting.Ack;
using JetPacketSystem.Streams;

namespace VWeaponEditor.Comms;

public class Packet3TranslateHashString : PacketACK {
    public int req_Hash;
    public string resp_String;
    
    public override void WritePayloadToServer(IDataOutput output) {
        output.WriteInt(this.req_Hash);
    }

    public override void ReadPayloadFromClient(IDataInput input) {
        this.req_Hash = input.ReadInt();
    }

    public override void WritePayloadToClient(IDataOutput output) {
        output.WriteStringLabelledUTF16(this.resp_String);
    }

    public override void ReadPayloadFromServer(IDataInput input) {
        this.resp_String = input.ReadStringUTF16Labelled();
    }
}