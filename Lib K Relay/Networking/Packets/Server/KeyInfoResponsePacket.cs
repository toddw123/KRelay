using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib_K_Relay.Networking.Packets.Server
{
	public class KeyInfoResponsePacket : Packet
	{
        public string Name;
        public string Description;
        public string Creator;

        public override PacketType Type
		{ get { return PacketType.KEYINFORESPONSE; } }

		public override void Read(PacketReader r)
		{
            Name = r.ReadString();
            Description = r.ReadString();
            Creator = r.ReadString();
		}

		public override void Write(PacketWriter w)
		{
			w.Write(Name);
            w.Write(Description);
            w.Write(Creator);
		}
	}
}
