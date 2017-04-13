using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib_K_Relay.Networking.Packets.Server
{
    public class ImminentArenaWavePacket : Packet
    {
        public int currentRuntime;

        public override PacketType Type
        { get { return PacketType.IMMINENTARENAWAVE; } }

        public override void Read(PacketReader r)
        {
            currentRuntime = r.ReadInt32();
        }

        public override void Write(PacketWriter w)
        {
            w.Write(currentRuntime);
        }
    }
}
