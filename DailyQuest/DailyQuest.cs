using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Lib_K_Relay;
using Lib_K_Relay.Interface;
using Lib_K_Relay.Networking;
using Lib_K_Relay.Networking.Packets;
using Lib_K_Relay.Networking.Packets.Client;
using Lib_K_Relay.Networking.Packets.DataObjects;
using Lib_K_Relay.Networking.Packets.Server;
using Lib_K_Relay.Utilities;
using Lib_K_Relay.GameData;
using Lib_K_Relay.GameData.DataStructures;

namespace DailyQuest
{
	class QuestHelper
	{
		public int goal = 0;
		public string map = "";
		public Dictionary<int, Location> bagLocations = new Dictionary<int, Location>();
		public int lastNotif = 0;
		public bool reqSent = false;
	}

    public class DailyQuest : IPlugin
    {
		private Dictionary<Client, QuestHelper> _dQuest = new Dictionary<Client, QuestHelper>();

		public short[] _bags = { (short)Bags.Red, (short)Bags.Purple, (short)Bags.Blue, (short)Bags.Cyan, (short)Bags.White, (short)Bags.Pink, (short)Bags.Normal, (short)Bags.Egg };

		public string GetAuthor()
		{ return "Todddddd"; }

		public string GetName()
		{ return "Daily Quest"; }

		public string GetDescription()
		{
			return "Show what item you are looking for, and quickly turn in the quest item.";
		}

		public string[] GetCommands()
		{ return new string[] { "/dq", "/dq settings" }; }

		public void Initialize(Proxy proxy)
		{
			proxy.ClientConnected += (c) => _dQuest.Add(c, new QuestHelper());
			proxy.ClientDisconnected += (c) => _dQuest.Remove(c);

			proxy.HookPacket(PacketType.QUESTFETCHRESPONSE, OnQuestFetch);
			proxy.HookPacket(PacketType.QUESTREDEEMRESPONSE, OnQuestRedeem);
			proxy.HookPacket(PacketType.MAPINFO, OnMapInfo);
			proxy.HookPacket(PacketType.UPDATE, OnUpdate);
			proxy.HookPacket(PacketType.MOVE, OnMove);

			proxy.HookCommand("dq", OnCommand);
		}

		public void OnMove(Client client, Packet packet)
		{
			if (!_dQuest.ContainsKey(client)) return;

			foreach (int bagId in _dQuest[client].bagLocations.Keys)
			{
				float distance = _dQuest[client].bagLocations[bagId].DistanceTo(client.PlayerData.Pos);
				
				if (DailyQuestConfig.Default.BagNotifications && Environment.TickCount - _dQuest[client].lastNotif > 2000 && distance < 15)
				{
					_dQuest[client].lastNotif = Environment.TickCount;
					client.SendToClient(PluginUtils.CreateNotification(bagId, "Current Daily Quest: " + GameData.Objects.ByID((ushort)_dQuest[client].goal).Name));
				}
			}
		}

		public void OnUpdate(Client client, Packet packet)
		{
			if (!_dQuest.ContainsKey(client)) return;

			if (DailyQuestConfig.Default.AutoRequest && !_dQuest[client].reqSent)
			{
				_dQuest[client].reqSent = true;
				client.SendToServer(Packet.Create(PacketType.QUESTFETCHASK));
			}

			UpdatePacket update = (UpdatePacket)packet;
			if (_dQuest[client].goal != 0)
			{
				foreach (Entity entity in update.NewObjs)
				{
					short type = entity.ObjectType;
					if (_bags.Contains(type))
					{
						int bagId = entity.Status.ObjectId;
						bool hasItem = false;
						foreach (StatData statData in entity.Status.Data)
						{
							if (statData.Id >= 8 && statData.Id <= 15)
							{
								if (statData.IntValue == _dQuest[client].goal)
								{
									hasItem = true;
									break;
								}
							}
						}
						if (hasItem)
						{
							if (!_dQuest[client].bagLocations.ContainsKey(bagId))
								_dQuest[client].bagLocations.Add(bagId, entity.Status.Position);
							else
								_dQuest[client].bagLocations[bagId] = entity.Status.Position;
						}
					}
				}
			}
		}

		public void OnMapInfo(Client client, Packet packet)
		{
			if (!_dQuest.ContainsKey(client)) return;
			MapInfoPacket mip = (MapInfoPacket)packet;
			_dQuest[client].map = mip.Name;
		}

		public void OnQuestFetch(Client client, Packet packet)
		{
			if (!_dQuest.ContainsKey(client)) return;

			QuestFetchResponsePacket qfrp = (QuestFetchResponsePacket)packet;
			client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, "Current Daily Quest: " + GameData.Objects.ByID((ushort)qfrp.Goal.ParseInt()).Name));
			_dQuest[client].goal = qfrp.Goal.ParseInt();
		}

		public void OnQuestRedeem(Client client, Packet packet)
		{
			if (!_dQuest.ContainsKey(client)) return;
			QuestRedeemResponsePacket qrrp = (QuestRedeemResponsePacket)packet;
			if (qrrp.Success)
			{
				_dQuest[client].goal = 0;
				client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, "Quest Turned In!"));
			}
		}

		public void TurnInQuest(Client client, byte slot)
		{
            QuestRedeemPacket tqp = (QuestRedeemPacket)Packet.Create(PacketType.QUESTREDEEM);
			tqp.Slot = new SlotObject();
			tqp.Slot.SlotId = slot;
			tqp.Slot.ObjectId = client.PlayerData.OwnerObjectId;

			if (slot > 11)
				tqp.Slot.ObjectType = client.PlayerData.BackPack[(slot - 12)];
			else
				tqp.Slot.ObjectType = client.PlayerData.Slot[slot];

			client.SendToServer(tqp);
		}

		public void OnCommand(Client client, string command, string[] args)
		{
			if (!_dQuest.ContainsKey(client)) return;

			if (args.Length == 0)
			{
				// The quest can only be turned in when you are in the Daily Quest Room
				if (_dQuest[client].map == "Daily Quest Room")
				{
					byte slot = 0;
					if (_dQuest[client].goal != 0)
					{
						for (byte i = 0; i < 8; i++)
						{
							if (client.PlayerData.Slot[i + 4] == _dQuest[client].goal)
							{
								slot = (byte)(i + 4);
								break;
							}
							if (client.PlayerData.BackPack[i] == _dQuest[client].goal)
							{
								slot = (byte)(i + 12);
								break;
							}
						}
					}
					// If slot does not equal 0 that means we have the item
					if (slot != 0)
					{
						Console.WriteLine("[DailyQuest] Attempting turn in");
						TurnInQuest(client, slot);
					}
					else
					{
						Console.WriteLine("[DailyQuest] Requesting Quest Data");
						client.SendToServer(Packet.Create(PacketType.QUESTFETCHASK));
					}
				}
				else
				{
					Console.WriteLine("[DailyQuest] Requesting Quest Data");
					client.SendToServer(Packet.Create(PacketType.QUESTFETCHASK));
				}
			}
			else if (args[0] == "get")
			{
				Console.WriteLine("[DailyQuest] Requesting Quest Data");
				client.SendToServer(Packet.Create(PacketType.QUESTFETCHASK));
			}
			else if (args[0] == "turnin")
			{
				byte slot;
				if (byte.TryParse(args[1], out slot))
				{
                    QuestRedeemPacket tqp = (QuestRedeemPacket)Packet.Create(PacketType.QUESTREDEEM);
					tqp.Slot            = new SlotObject();
					tqp.Slot.SlotId     = slot;
					tqp.Slot.ObjectId   = client.PlayerData.OwnerObjectId;
					if (slot > 11)
						tqp.Slot.ObjectType = client.PlayerData.BackPack[(slot - 12)];
					else
						tqp.Slot.ObjectType = client.PlayerData.Slot[slot];
					tqp.Send = true;

					Console.WriteLine("[DailyQuest] Attempting turn in" );

					client.SendToServer(tqp);
				}
			}
			else if (args[0] == "settings")
			{
				PluginUtils.ShowGenericSettingsGUI(DailyQuestConfig.Default, "Daily Quest Settings");
			}
		}
    }
}
