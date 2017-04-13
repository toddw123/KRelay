using Lib_K_Relay;
using Lib_K_Relay.Interface;
using Lib_K_Relay.Networking;
using Lib_K_Relay.Networking.Packets;
using Lib_K_Relay.Networking.Packets.Client;
using Lib_K_Relay.Networking.Packets.DataObjects;
using Lib_K_Relay.Utilities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Switch
{
    public class Switch : IPlugin
    {
		public string GetAuthor()
		{ return "Todddddd"; }

		public string GetName()
		{ return "Item Switcher"; }

		public string GetDescription()
		{ return "Switches items from backpack to inventory, and vice versa."; }

		public string[] GetCommands()
		{ return new string[] { "/s" }; }

		public void Initialize(Proxy proxy)
		{
			proxy.HookCommand("s", OnSwitch);
		}

		public void OnSwitch(Client client, string command, string[] args)
		{
            if (client.PlayerData.HasBackpack == false)
            {
                client.SendToClient(PluginUtils.CreateOryxNotification("Item Switcher", "YOU DONT HAVE A BACKPACK!"));
                return;
            }

			if (args.Length == 0)
			{
				Swap(client,
					new SlotObject
					{
						ObjectId = client.ObjectId,
						SlotId = 4,
						ObjectType = client.PlayerData.Slot[4]
					},
					new SlotObject
					{
						ObjectId = client.ObjectId,
						SlotId = 12,
						ObjectType = client.PlayerData.BackPack[0]
					}, true);
			}
			else if (args.Length == 1)
			{
				int slot;
				if (Int32.TryParse(args[0], out slot))
				{
					slot = slot + 3;
					Swap(client,
						new SlotObject
						{
							ObjectId = client.ObjectId,
							SlotId = (byte)slot,
							ObjectType = client.PlayerData.Slot[slot]
						},
						new SlotObject
						{
							ObjectId = client.ObjectId,
							SlotId = (byte)(slot + 8),
							ObjectType = client.PlayerData.BackPack[slot - 4]
						});
				}
			}
		}

		public void Swap(Client client, SlotObject sobj1, SlotObject sobj2, bool all = false)
		{
			//Console.WriteLine("Swap called with {0} and {1}", sobj1.SlotId, sobj2.SlotId);
			InvSwapPacket invSwapPacket = (InvSwapPacket)Packet.Create(PacketType.INVSWAP);
			invSwapPacket.Time = client.Time;
			invSwapPacket.Position = client.PlayerData.Pos;
			invSwapPacket.SlotObject1 = sobj1;
			invSwapPacket.SlotObject2 = sobj2;
			client.SendToServer(invSwapPacket);

			if (all)
			{
				if (sobj1.SlotId < 11)
				{
					PluginUtils.Delay(600, () =>
					{
						Swap(client,
							new SlotObject
							{
								ObjectId = client.ObjectId,
								SlotId = (byte)(sobj1.SlotId + 1),
								ObjectType = client.PlayerData.Slot[sobj1.SlotId + 1]
							},
							new SlotObject
							{
								ObjectId = client.ObjectId,
								SlotId = (byte)(sobj1.SlotId + 9),
								ObjectType = client.PlayerData.BackPack[(sobj1.SlotId+1) - 4]
							}, true);
					});
				}
			}
		}
    }
}
