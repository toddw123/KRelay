using Lib_K_Relay;
using Lib_K_Relay.GameData;
using Lib_K_Relay.Interface;
using Lib_K_Relay.Networking;
using Lib_K_Relay.Networking.Packets;
using Lib_K_Relay.Networking.Packets.Client;
using Lib_K_Relay.Networking.Packets.DataObjects;
using Lib_K_Relay.Networking.Packets.Server;
using Lib_K_Relay.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LootHelper
{
    class LootState
    {
        public Dictionary<int, int[]> LootBagItems = new Dictionary<int, int[]>();
        public Dictionary<int, Location> LootBagLocations = new Dictionary<int, Location>();
        public Dictionary<int, string> LootBagTypes = new Dictionary<int, string>();
        public Dictionary<int, int> BagLastNotif = new Dictionary<int, int>();
        public int LastLoot = 0;
        public int CustomQuest = -1;
        public int OriginalQuest = -1;
    }
    public class LootHelper : IPlugin
    {
        private Dictionary<Client, LootState> _states = new Dictionary<Client, LootState>();

        public string GetAuthor()
        { return "KrazyShank / Kronks (Modified by Todddddd)"; }

        public string GetName()
        { return "Loot Helper"; }

        public string GetDescription()
        { return "Allows you to enable loot notifications, loot quests, and autoloot."; }

        public string[] GetCommands()
        { return new string[] { "/loothelper settings" }; }

        public void Initialize(Proxy proxy)
        {
            proxy.ClientConnected += (c) => _states.Add(c, new LootState());
            proxy.ClientDisconnected += (c) => _states.Remove(c);

            proxy.HookPacket(PacketType.MOVE, OnMove);
            proxy.HookPacket(PacketType.UPDATE, OnUpdate);
            proxy.HookPacket(PacketType.NEWTICK, OnNewTick);
            proxy.HookPacket(PacketType.QUESTOBJID, OnQuestObjId);
            proxy.HookCommand("loothelper", OnLootHelperCommand);
        }

        private void OnLootHelperCommand(Client client, string command, string[] args)
        {
            PluginUtils.ShowGenericSettingsGUI(LootHelperConfig.Default, "Loot Helper Settings");
        }

        private string BagTypeToString(short ObjectType)
        {
            switch (ObjectType)
            {
                case (short)Bags.Normal:
                    return "Normal";
                case (short)Bags.Pink:
                    return "Pink";
                case (short)Bags.Purple:
                case (short)Bags.Purple2:
                    return "Purple";
                case (short)Bags.Egg:
                    return "Egg";
                case (short)Bags.Blue:
                    return "Blue";
                case (short)Bags.Cyan:
                    return "Cyan";
                case (short)Bags.White:
                case (short)Bags.White2:
                case (short)Bags.White3:
                    return "White";
                case (short)Bags.Red:
                    return "Red";
                default:
                    return null;
            }
        }

        private bool CheckLootTier(int itemId)
        {
            byte tier = (byte)GameData.Items.ByID((ushort)itemId).Tier;
            byte st = GameData.Items.ByID((ushort)itemId).SlotType;

            switch (st)
            {
                case 1: //Swords
                case 2: //Daggers
                case 3: //Bows
                case 8: //Wands
                case 17: //Staffs
                case 24: //Katanas
                    if (tier >= LootHelperConfig.Default.AutoLootWeaponTier)
                        return true;
                    else
                        return false;
                case 6: //Leather Armor
                case 7: //Heavy Armor
                case 14: //Robes
                    if (tier >= LootHelperConfig.Default.AutoLootArmorTier)
                        return true;
                    else
                        return false;
                case 9: //Ring
                    if (tier >= LootHelperConfig.Default.AutoLootRingTier)
                        return true;
                    else
                        return false;
                case 4: //Tomes
                case 5: //Shield
                case 11: //Spells
                case 12: //Seals
                case 13: //Cloak
                case 15: //Quiver
                case 16: //Helms
                case 18: //Poisons
                case 19: //Skulls
                case 20: //Traps
                case 21: //Orbs
                case 22: //Prisms
                case 23: //Scepters
                case 25: //Shurikens
                    if (tier >= LootHelperConfig.Default.AutoLootAbilityTier)
                        return true;
                    else
                        return false;
                default:
                    return false;
            }
        }

        private void OnMove(Client client, Packet packet)
        {
            if (!_states.ContainsKey(client)) return;
            LootState state = _states[client];

            foreach (int bagId in state.LootBagItems.Keys)
            {
                if (state.LootBagTypes[bagId] == null) continue;
                float distance = state.LootBagLocations[bagId].DistanceTo(client.PlayerData.Pos);
                if (LootHelperConfig.Default.LootBags.Contains(state.LootBagTypes[bagId], StringComparison.OrdinalIgnoreCase))
                {
                    if (LootHelperConfig.Default.AutoLoot && Environment.TickCount - state.LastLoot > LootHelperConfig.Default.LootSpeed && distance <= 1)
                    {
                        for (int bi = 0; bi < state.LootBagItems[bagId].Length; bi++)
                        {
                            if (state.LootBagItems[bagId][bi] == -1)
                                continue;

                            bool next = true;
                            if (LootHelperConfig.Default.AutoLootList.Contains(ReverseLookup(state.LootBagItems[bagId][bi])))
                                next = false;
                            else if (CheckLootTier(state.LootBagItems[bagId][bi]))
                                next = false;

                            if (next)
                                continue;

                            state.LastLoot = Environment.TickCount;

                            if (state.LootBagItems[bagId][bi] == 2594 && client.PlayerData.HealthPotionCount < 6)
                            {
                                InvSwapPacket invSwap = (InvSwapPacket)Packet.Create(PacketType.INVSWAP);
                                invSwap.Time = client.Time + 10;
                                invSwap.Position = client.PlayerData.Pos;

                                invSwap.SlotObject1 = new SlotObject();
                                invSwap.SlotObject1.ObjectId = bagId;
                                invSwap.SlotObject1.SlotId = (byte)bi;
                                invSwap.SlotObject1.ObjectType = (short)state.LootBagItems[bagId][bi];

                                invSwap.SlotObject2 = new SlotObject();
                                invSwap.SlotObject2.ObjectId = client.ObjectId;
                                invSwap.SlotObject2.SlotId = (byte)(state.LootBagItems[bagId][bi] - 2340);
                                invSwap.SlotObject2.ObjectType = -1;

                                //state.LastLoot = Environment.TickCount;
                                client.SendToServer(invSwap);
                                continue;
                            }
                            else if(state.LootBagItems[bagId][bi] == 2595 && client.PlayerData.MagicPotionCount < 6)
                            {
                                InvSwapPacket invSwap = (InvSwapPacket)Packet.Create(PacketType.INVSWAP);
                                invSwap.Time = client.Time + 10;
                                invSwap.Position = client.PlayerData.Pos;

                                invSwap.SlotObject1 = new SlotObject();
                                invSwap.SlotObject1.ObjectId = bagId;
                                invSwap.SlotObject1.SlotId = (byte)bi;
                                invSwap.SlotObject1.ObjectType = (short)state.LootBagItems[bagId][bi];

                                invSwap.SlotObject2 = new SlotObject();
                                invSwap.SlotObject2.ObjectId = client.ObjectId;
                                invSwap.SlotObject2.SlotId = (byte)(state.LootBagItems[bagId][bi] - 2340);
                                invSwap.SlotObject2.ObjectType = -1;

                                //state.LastLoot = Environment.TickCount;
                                client.SendToServer(invSwap);
                                continue;
                            }
                            else
                            {
                                for (int i = 4; i < 20; i++)
                                {
                                    if (i < 12)
                                    {
                                        if (client.PlayerData.Slot[i] == -1)
                                        {
                                            InvSwapPacket invSwap = (InvSwapPacket)Packet.Create(PacketType.INVSWAP);
                                            invSwap.Time = client.Time + 10;
                                            invSwap.Position = client.PlayerData.Pos;

                                            invSwap.SlotObject1 = new SlotObject();
                                            invSwap.SlotObject1.ObjectId = bagId;
                                            invSwap.SlotObject1.SlotId = (byte)bi;
                                            invSwap.SlotObject1.ObjectType = state.LootBagItems[bagId][bi];

                                            invSwap.SlotObject2 = new SlotObject();
                                            invSwap.SlotObject2.ObjectId = client.ObjectId;
                                            invSwap.SlotObject2.SlotId = (byte)(i);
                                            invSwap.SlotObject2.ObjectType = -1;

                                            //state.LastLoot = Environment.TickCount;
                                            client.SendToServer(invSwap);
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        if (client.PlayerData.HasBackpack)
                                        {
                                            if (client.PlayerData.BackPack[i - 12] == -1)
                                            {
                                                InvSwapPacket invSwap = (InvSwapPacket)Packet.Create(PacketType.INVSWAP);
                                                invSwap.Time = client.Time + 10;
                                                invSwap.Position = client.PlayerData.Pos;

                                                invSwap.SlotObject1 = new SlotObject();
                                                invSwap.SlotObject1.ObjectId = bagId;
                                                invSwap.SlotObject1.SlotId = (byte)bi;
                                                invSwap.SlotObject1.ObjectType = state.LootBagItems[bagId][bi];

                                                invSwap.SlotObject2 = new SlotObject();
                                                invSwap.SlotObject2.ObjectId = client.ObjectId;
                                                invSwap.SlotObject2.SlotId = (byte)(i);
                                                invSwap.SlotObject2.ObjectType = -1;

                                                //state.LastLoot = Environment.TickCount;
                                                client.SendToServer(invSwap);
                                                continue;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (LootHelperConfig.Default.NotifBags.Contains(state.LootBagTypes[bagId], StringComparison.OrdinalIgnoreCase))
                {
                    if (!state.BagLastNotif.ContainsKey(bagId))
                        state.BagLastNotif.Add(bagId, 0);

                    if (LootHelperConfig.Default.LootNotifications && Environment.TickCount - state.BagLastNotif[bagId] > 2000 && distance < 15)
                    {
                        state.BagLastNotif[bagId] = Environment.TickCount;
                        string message = "";

                        foreach (int item in state.LootBagItems[bagId])
                            if (item != -1) message += ReverseLookup(item) + "\\n";

                        if (message.Length > 3)
                            client.SendToClient(PluginUtils.CreateNotification(bagId, LootHelperConfig.Default.NotificationColor.ToArgb(), message));
                    }
                }
            }
        }

        private void OnUpdate(Client client, Packet packet)
        {
            LootState state = _states[client];
            UpdatePacket update = (UpdatePacket)packet;
            // New Objects
            foreach (Entity entity in update.NewObjs)
            {
                string type = BagTypeToString(entity.ObjectType);
                if (type == null) continue;
                if (LootHelperConfig.Default.LootBags.Contains(type, StringComparison.OrdinalIgnoreCase) || LootHelperConfig.Default.NotifBags.Contains(type, StringComparison.OrdinalIgnoreCase))
                {
                    if (LootHelperConfig.Default.QuestBags.Contains(type, StringComparison.OrdinalIgnoreCase))
                    {
                        if (LootHelperConfig.Default.LootQuests)
                        {
                            state.CustomQuest = entity.Status.ObjectId;
                            QuestObjIdPacket questObjId = (QuestObjIdPacket)Packet.Create(PacketType.QUESTOBJID);
                            questObjId.ObjectId = entity.Status.ObjectId;
                            client.SendToClient(questObjId);
                        }
                    }

                    int bagId = entity.Status.ObjectId;
                    // Set the bag type
                    if (!state.LootBagTypes.ContainsKey(bagId))
                        state.LootBagTypes.Add(bagId, type);
                    else
                        state.LootBagTypes[bagId] = type;
                    // Set the bag contents to empty
                    if (!state.LootBagItems.ContainsKey(bagId))
                        state.LootBagItems.Add(bagId, new int[] { -1, -1, -1, -1, -1, -1, -1, -1 });
                    else
                        state.LootBagItems[bagId] = new int[] { -1, -1, -1, -1, -1, -1, -1, -1 };
                    // Set the bag location
                    if (!state.LootBagLocations.ContainsKey(bagId))
                        state.LootBagLocations.Add(bagId, entity.Status.Position);
                    else
                        state.LootBagLocations[bagId] = entity.Status.Position;
                    // Fill in the bag contents
                    foreach (StatData statData in entity.Status.Data)
                    {
                        if (statData.Id >= 8 && statData.Id <= 15)
                            state.LootBagItems[bagId][statData.Id - 8] = statData.IntValue;
                    }
                }
            }

            // Removed Objects
            foreach (int drop in update.Drops)
            {
                if (drop == state.CustomQuest && state.OriginalQuest != -1)
                {
                    QuestObjIdPacket questObjId = (QuestObjIdPacket)Packet.Create(PacketType.QUESTOBJID);
                    questObjId.ObjectId = state.OriginalQuest;
                    client.SendToClient(questObjId);

                    state.OriginalQuest = -1;
                    state.CustomQuest = -1;
                }

                if (state.LootBagItems.ContainsKey(drop))
                {
                    state.LootBagItems.Remove(drop);
                    state.LootBagLocations.Remove(drop);
                }
            }
        }

        private void OnNewTick(Client client, Packet packet)
        {
            LootState state = _states[client];
            NewTickPacket newTick = (NewTickPacket)packet;

            // Updated Objects
            foreach (Status status in newTick.Statuses)
            {
                if (state.LootBagItems.ContainsKey(status.ObjectId))
                {
                    foreach (StatData statData in status.Data)
                    {
                        if (statData.Id >= 8 && statData.Id <= 15)
                            state.LootBagItems[status.ObjectId][statData.Id - 8] = statData.IntValue;
                    }
                }
            }
        }

        private void OnQuestObjId(Client client, Packet packet)
        {
            _states[client].OriginalQuest = (packet as QuestObjIdPacket).ObjectId;
        }

        private string ReverseLookup(int itemId)
        {
            return GameData.Items.ByID((ushort)itemId).Name;
        }
    }

    public static class StringExtensions
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }
    }
}
