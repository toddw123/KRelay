namespace Lib_K_Relay.Networking.Packets.Server
{
    public class ClaimDailyRewardResponsePacket : Packet
    {
        public int ItemId;
        public int Qty;
        public int Gold;

        public override PacketType Type
        { get { return PacketType.LOGINREWARDMSG; } }

        public override void Read(PacketReader r)
        {
            ItemId = r.ReadInt32();
            Qty = r.ReadInt32();
            Gold = r.ReadInt32();
        }

        public override void Write(PacketWriter w)
        {
            w.Write(ItemId);
            w.Write(Qty);
            w.Write(Gold);
        }
    }
}
