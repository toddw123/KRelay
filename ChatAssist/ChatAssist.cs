using Lib_K_Relay;
using Lib_K_Relay.Interface;
using Lib_K_Relay.Networking;
using Lib_K_Relay.Networking.Packets;
using Lib_K_Relay.Networking.Packets.Client;
using Lib_K_Relay.Networking.Packets.Server;
using Lib_K_Relay.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatAssist
{
    public class ChatAssist : IPlugin
    {
        private string lastMessage = "";
        private string[] NPCIgnoreList = { "#Mystery Box Shop", "#The Alchemist", "#Login Seer", "#The Tinkerer", "#Bandit Leader", "#Drake Baby", "#Dwarf King", "#Killer Pillar", "#Haunted Armor", "#Red Demon", "#Cyclops God", "#Belladonna", "#Sumo Master", "#Avatar of the Forgotten King", "#Small Ghost", "#Medium Ghost", "#Large Ghost", "#Ghost Master", "#Ghost King", "#Lich", "#Haunted Spirit", "#Rock Construct", "#Phylactery Bearer", "#Mini Yeti", "#Big Yeti", "#Esben the Unwilling", "#Creepy Weird Dark Spirit Mirror Image Monster", "#Ent Ancient", "#Kage Kami", "#Twilight Archmage", "#The Forgotten Sentinel", "#The Cursed Crown", "#The Forgotten King", "#Titanum of Cruelty", "#Titanum of Despair", "#Titanum of Lies", "#Titanum of Hate", "#Grand Sphinx", "#Troll Matriarch", "#Dreadstump the Pirate King", "#Stone Mage", "#Deathmage", "#Horrid Reaper", "#Bes", "#Nut", "#Geb", "#Nikao the Defiler", "#Limoz the Plague Bearer", "#Feargus the Demented", "#Pyyr the Wicked", "#Ivory Wyvern", "#Red Soul of Pyrr", "#Blue Soul of Nikao", "#Green Soul of Limoz", "#Black Soul of Feargus", "#Shaitan the Advisor", "#Left Hand of Shaitan", "#Right Hand of Shaitan", "#The Puppet Master", "#Trick in a Box" };

        private string[,] NPCResponseList = { { "What time is it?", "Its pizza time!" }, { "Where is the safest place in the world?", "Inside my shell." }, { "What is fast, quiet and hidden by the night?", "A ninja of course!" }, { "How do you like your pizza?", "Extra cheese, hold the anchovies." }, { "Who did this to me?", "Dr. Terrible, the mad scientist." }, { "Is King Alexander alive?", "He lives and reigns and conquers the world" }, { "Say, 'READY' when you are ready to face your opponents.", "ready" }, { "Well, before I explain how this all works, let me tell you that you can always say SKIP and we'll just get on with it. Otherwise, just wait a sec while I get everything in order.", "skip" } };

        public string GetAuthor()
        { return "KrazyShank / Kronks / RotMGHacker"; }

        public string GetName()
        { return "Chat Assist"; }

        public string GetDescription()
        { return "A collection of tools to help your reduce the spam and clutter of chat and make it easier prevent future spam."; }

        public string[] GetCommands()
        {
            return new string[]
            {
                "/chatassist [On/Off]",
                "/chatassist settings",
                "/chatassist add [message] - add a string to the spam filter",
                "/chatassist remove [message] - removes a string from the spam filter",
                "/chatassist list - list all strings included in the spam filter",
                "/chatassist log (On/Off) - Toggle Chat logging",
                "/re (new recipient) - Resends the last message you've typed on chat. Optionally to a new recipient."
            };
        }

        public void Initialize(Proxy proxy)
        {
            proxy.HookCommand("chatassist", OnChatAssistCommand);
            proxy.HookCommand("ca", OnChatAssistCommand);
            proxy.HookCommand("re", OnResendCommand);

            proxy.HookPacket(PacketType.TEXT, OnText);
            proxy.HookPacket(PacketType.PLAYERTEXT, OnPlayerText);
        }

        private void OnChatAssistCommand(Client client, string command, string[] args)
        {
            if (args.Length == 0)
            {
                client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, "Chat Assist is " + (ChatAssistConfig.Default.Enabled ? "Enabled" : "Disabled")));
            }
            else
            {
                switch (args[0])
                {
                    case "on":
                        ChatAssistConfig.Default.Enabled = true;
                        ChatAssistConfig.Default.Save();
                        client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, "Chat Assist Enabled!"));
                        break;
                    case "off":
                        ChatAssistConfig.Default.Enabled = false;
                        ChatAssistConfig.Default.Save();
                        client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, "Chat Assist Disabled!"));
                        break;
                    case "settings":
                        PluginUtils.ShowGUI(new FrmChatAssistSettings());
                        break;
                    case "add":
                        if (args.Length > 1)
                        {
                            // This is needed because args are seperated by whitespaces and the string to filter coud contain whitespaces so we concatenate them together
                            string toFilter = "";
                            for (int i = 1; i < args.Length; ++i)
                            {
                                toFilter += args[i] + " ";
                            }

                            toFilter = toFilter.Trim();

                            // Only add valid entries
                            if (toFilter.Length > 0)
                            {
                                ChatAssistConfig.Default.Blacklist.Add(toFilter);
                                client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, toFilter + " added to spam filter!"));

                                ChatAssistConfig.Default.Save(); // Save our changes
                            }
                            else
                            {
                                client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, "Invalid message!"));
                            }
                        }
                        else
                        {
                            client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, "Missing message to filter!"));
                        }
                        break;
                    case "remove":
                    case "rem":
                        if (args.Length > 1)
                        {
                            string toRemove = "";
                            for (int i = 1; i < args.Length; ++i)
                            {
                                toRemove += args[i] + " ";
                            }

                            toRemove = toRemove.Trim();

                            if (ChatAssistConfig.Default.Blacklist.Contains(toRemove))
                            {
                                ChatAssistConfig.Default.Blacklist.Remove(toRemove);
                                client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, toRemove + " removed from spam filter!"));

                                ChatAssistConfig.Default.Save(); // Save our changes
                            }
                            else
                            {
                                client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, "Couldn't find " + toRemove + " in spam filter!"));
                            }
                        }
                        else
                        {
                            client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, "Missing message to remove!"));
                        }
                        break;
                    case "list":
                        if (ChatAssistConfig.Default.Blacklist.Count == 0)
                        {
                            client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, "Spam filter is empty!"));
                            return;
                        }

                        string message = "Spam filter contains: ";

                        // Construct our list
                        foreach (string filter in ChatAssistConfig.Default.Blacklist)
                        {
                            message += filter + ", ";
                        }

                        message = message.Remove(message.Length - 2);

                        client.SendToClient(PluginUtils.CreateOryxNotification("ChatAssist", message));
                        break;
                    case "log":
                        if (args.Length == 1) { ChatAssistConfig.Default.LogChat = !ChatAssistConfig.Default.LogChat; }
                        else
                        {
                            if (args[1] == "on") { ChatAssistConfig.Default.LogChat = true; }
                            else { ChatAssistConfig.Default.LogChat = false; }
                        }

                        client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, "Turned Chat logging " + (ChatAssistConfig.Default.LogChat ? "On" : "Off")));
                        break;
                    default:
                        client.SendToClient(PluginUtils.CreateOryxNotification("ChatAssist", "Unrecognized command: " + args[0]));
                        client.SendToClient(PluginUtils.CreateOryxNotification("ChatAssist", "Usage:"));
                        client.SendToClient(PluginUtils.CreateOryxNotification("ChatAssist", "'/chatassist on' - enable chatassist"));
                        client.SendToClient(PluginUtils.CreateOryxNotification("ChatAssist", "'/chatassist off' - disable chatassist"));
                        client.SendToClient(PluginUtils.CreateOryxNotification("ChatAssist", "'/chatassist settings' - open settings"));
                        client.SendToClient(PluginUtils.CreateOryxNotification("ChatAssist", "'/chatassist add [message]' - add the give string to the spam filter"));
                        client.SendToClient(PluginUtils.CreateOryxNotification("ChatAssist", "'/chatassist remove [message]' - remove the give string from the spam filter"));
                        client.SendToClient(PluginUtils.CreateOryxNotification("ChatAssist", "'/chatassist list' - display all string in the spam filter"));
                        client.SendToClient(PluginUtils.CreateOryxNotification("ChatAssist", "'/chatassist log (On/Off)' - turn chat logging On/Off"));
                        client.SendToClient(PluginUtils.CreateOryxNotification("ChatAssist", "'/re (new recipient)' - resend the last message you've typed. Optionally to a new recipient"));
                        break;
                }
            }
        }

        private void OnResendCommand(Client client, string command, string[] args)
        {
            if (lastMessage != "")
            {
                PlayerTextPacket playerTextPacket = (PlayerTextPacket)Packet.Create(PacketType.PLAYERTEXT);

                // Only try to change the recipient when whispering
                if (args.Length == 0 && (lastMessage.StartsWith("/t ") || lastMessage.StartsWith("/tell ") || lastMessage.StartsWith("/w ") || lastMessage.StartsWith("/whisper ")))
                {
                    int count = 3;
                    String[] sub = lastMessage.Split(" ".ToCharArray(), count);
                    // sub[0] = original command (/t, /tell, /w, /whisper)
                    // args[0] = new recipient
                    // sub[2] = the actual message
                    playerTextPacket.Text = sub[0] + " " + args[0] + " " + sub[2];
                }
                else // same or no recipient
                {
                    playerTextPacket.Text = lastMessage;
                }

                client.SendToServer(playerTextPacket);
            }
        }

        private void OnText(Client client, Packet packet)
        {
            if (!ChatAssistConfig.Default.Enabled) return;

            TextPacket text = packet.To<TextPacket>();

            if (text.NumStars == -1) // Not a message from a user
            {
                if (ChatAssistConfig.Default.EnableNPCFilter)
                {
                    foreach (string name in NPCIgnoreList)
                    {
                        if (text.Name.Contains(name))
                        {
                            text.Send = false;
                            return;
                        }
                    }
                }

                if (ChatAssistConfig.Default.AutoResponse)
                {
                    for (int i = 0; i < NPCResponseList.GetLength(0); ++i)
                    {
                        if (text.Text.Contains(NPCResponseList[i, 0]))
                        {
                            PlayerTextPacket playerText = (PlayerTextPacket)Packet.Create(PacketType.PLAYERTEXT);
                            playerText.Text = NPCResponseList[i, 1];
                            client.SendToServer(playerText);

                            return;
                        }
                    }
                }

                // Event notifications

                string message = "";

                if (text.Text == "{\"key\":\"server.oryx_closed_realm\"}") { message = "Realm has Closed!"; }
                else if (text.Text.Contains("stringlist.Lich.one")) { message = "Final Lich!"; }
                else if (text.Text == "Squeek!") { message = "Golden Rat Encountered!"; }
                else if (text.Text == "You thrash His Lordship's castle, kill his brothers, and challenge us. Come, come if you dare.") { message = "Janus Spawned!"; }
                else if (text.Text == "Sweet treasure awaits for powerful adventurers!") { message = "Crystal Spawned!"; }
                else if (text.Text == "Me door is open. Come let me crush you!") { message = "Door Opened!"; }
                else if (text.Name.Contains("#Oryx"))
                {
                    if (text.Text.Contains("Hermit_God")) { message = "Hermit God"; }
                    else if (text.Text.Contains("Lord_of_the_Lost_Lands")) { message = "Lord of the Lost Lands"; }
                    else if (text.Text.Contains("Grand_Sphinx")) { message = "Grand Sphinx"; }
                    else if (text.Text.Contains("Pentaract")) { message = "Pentaract"; }
                    else if (text.Text.Contains("shtrs_Defense_System")) { message = "Avatar"; }
                    else if (text.Text.Contains("Ghost_Ship")) { message = "Ghost Ship"; }
                    else if (text.Text.Contains("Dragon_Head_Leader")) { message = "Rock Dragon"; }
                    else if (text.Text.Contains("Cube_God")) { message = "Cube God"; }
                    else if (text.Text.Contains("Skull_Shrine")) { message = "Skull Shrine"; }
                    else if (text.Text.Contains("Temple_Encounter")) { message = "Temple Statues"; }
                    else { message = "Unknown New: " + text.Text; }

                    if (text.Text.Contains("new")) { message += " Spawned!"; }
                    else if (text.Text.Contains("killed") || text.Text.Contains("death")) { message += " Died!"; }
                    else { return; }
                }
                else 
                {
                    return;
                }

                client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, message));
                return;
            }

            if ((ChatAssistConfig.Default.DisableMessages && text.Recipient == "") ||
                (text.Recipient == "" && text.NumStars < ChatAssistConfig.Default.StarFilter && text.NumStars != -1) ||
                (text.Recipient != "" && text.NumStars < ChatAssistConfig.Default.StarFilterPM) && text.NumStars != -1)
            {
                text.Send = false;
                return;
            }

            if (ChatAssistConfig.Default.EnableSpamFilter)
            {
                foreach (string filter in ChatAssistConfig.Default.Blacklist)
                {
                    if (filter.ToLower().Trim() == "") continue;

                    if (text.Text.ToLower().Contains(filter.ToLower().Trim()))
                    {
                        // Is spam
                        if (ChatAssistConfig.Default.CensorSpamMessages)
                        {
                            text.Text = "...";
                            text.CleanText = "...";
                        }
                        else { text.Send = false; }

                        if (ChatAssistConfig.Default.AutoIgnoreSpamMessage ||
                           (ChatAssistConfig.Default.AutoIgnoreSpamPM && text.Recipient != ""))
                        {
                            // Ignore
                            PlayerTextPacket playerText = (PlayerTextPacket)Packet.Create(PacketType.PLAYERTEXT);
                            playerText.Text = "/ignore " + text.Name;
                            client.SendToServer(playerText);
                        }

                        return;
                    }
                }
            }

            // ChatLog
            if (ChatAssistConfig.Default.LogChat && text.NumStars != -1)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter("ChatAssist.log", true))
                {
                    file.WriteLine("<" + DateTime.Now.ToString() + ">: " + text.Name + "[" + text.NumStars + "]: '" + text.Text + "'");
                }
            }
        }

        private void OnPlayerText(Client client, Packet packet)
        {
            PlayerTextPacket playerTextPacket = (PlayerTextPacket)packet;

            if (!playerTextPacket.Text.StartsWith("/") || playerTextPacket.Text.StartsWith("/t ") || playerTextPacket.Text.StartsWith("/tell ") || playerTextPacket.Text.StartsWith("/w ") || playerTextPacket.Text.StartsWith("/whisper ") || playerTextPacket.Text.StartsWith("/yell "))
            {
                lastMessage = playerTextPacket.Text;
            }

            if (ChatAssistConfig.Default.LogChat)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter("ChatAssist.log", true))
                {
                    file.WriteLine("<" + DateTime.Now.ToString() + ">: You: '" + playerTextPacket.Text + "'");
                }
            }
        }
    }
}
