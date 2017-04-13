using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib_K_Relay.Networking.Packets.Client
{
    public class ClaimDailyRewardMessagePacket : Packet
    {
        public string ClaimKey;
        public string TypeVal;

        public override PacketType Type
        { get { return PacketType.CLAIMLOGINREWARDMSG; } }

        public override void Read(PacketReader r)
        {
            ClaimKey = r.ReadString();
            TypeVal = r.ReadString();
        }

        public override void Write(PacketWriter w)
        {
            w.Write(ClaimKey);
            w.Write(TypeVal);
        }
    }
}
