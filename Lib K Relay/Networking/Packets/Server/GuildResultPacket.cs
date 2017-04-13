using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib_K_Relay.Networking.Packets.Server
{
    public class GuildResultPacket : Packet
    {
        public bool Success;
        public string ErrorText;
        public override PacketType Type
        { get { return PacketType.GUILDRESULT; } }

        public override void Read(PacketReader r)
        {
            Success = r.ReadBoolean();
            ErrorText = r.ReadString();
        }

        public override void Write(PacketWriter w)
        {
            w.Write(Success);
            w.Write(ErrorText);
        }
    }
}
