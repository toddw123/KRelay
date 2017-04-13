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
        public string GetAuthor()
        { return "KrazyShank / Kronks"; }

        public string GetName()
        { return "Chat Assist"; }

        public string GetDescription()
        { return "A collection of tools to help your reduce the spam and clutter of chat and make it easier prevent future spam."; }

        public string[] GetCommands()
        { return new string[] { "/chatassist off", "/chatassist on", "/chatassist settings" }; }

        public void Initialize(Proxy proxy)
        {
            proxy.HookCommand("chatassist", OnChatAssistCommand);
            proxy.HookPacket(PacketType.TEXT, OnText);
        }

        private void OnChatAssistCommand(Client client, string command, string[] args)
        {
            if (args.Length == 0) return;

            if (args[0] == "settings")
                PluginUtils.ShowGUI(new FrmChatAssistSettings());
            else if (args[0] == "on")
            {
                ChatAssistConfig.Default.Enabled = true;
                client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, "Chat Assist Enabled!"));
            }
            else if (args[0] == "off")
            {
                ChatAssistConfig.Default.Enabled = false;
                client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, "Chat Assist Disabled!"));
            }
        }

        private void OnText(Client client, Packet packet)
        {
            if (!ChatAssistConfig.Default.Enabled) return;
            //TextPacket text = (TextPacket)packet;
            TextPacket text = packet.To<TextPacket>();

            if (text.Text == "{\"key\":\"server.oryx_closed_realm\"}")
			{
				client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, "Realm has Closed!"));
			}

            if (text.Name.Contains("#Oryx") || text.Name.Contains("Mysterious"))
            {
                if (text.Text.Contains("Hermit_God"))
                {
                    if (text.Text.Contains("new"))
                        client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, "Hermit God Spawned!"));
                    else if (text.Text.Contains("killed") || text.Text.Contains("death"))
                        client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, "Hermit God Died!"));
                }
                else if (text.Text.Contains("Lord_of_the_Lost_Lands"))
                {
                    if (text.Text.Contains("new"))
                        client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, "Lord of the Lost Lands Spawned!"));
                    else if (text.Text.Contains("killed") || text.Text.Contains("death"))
                        client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, "Lord of the Lost Lands Died!"));
                }
                else if (text.Text.Contains("Grand_Sphinx"))
                {
                    if (text.Text.Contains("new"))
                        client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, "Grand Sphinx Spawned!"));
                    else if (text.Text.Contains("killed") || text.Text.Contains("death"))
                        client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, "Grand Sphinx Died!"));
                }
                else if (text.Text.Contains("Pentaract"))
                {
                    if (text.Text.Contains("new"))
                        client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, "Pentaract Spawned!"));
                    else if (text.Text.Contains("killed") || text.Text.Contains("death"))
                        client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, "Pentaract Died!"));
                }
                else if (text.Text.Contains("shtrs_Defense_System"))
                {
                    if (text.Text.Contains("new"))
                        client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, "Avatar Spawned!"));
                    else if (text.Text.Contains("killed") || text.Text.Contains("death"))
                        client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, "Avatar Died!"));
                }
                else if (text.Text.Contains("Ghost_Ship"))
                {
                    if (text.Text.Contains("new"))
                        client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, "Ghost Ship Spawned!"));
                    else if (text.Text.Contains("killed") || text.Text.Contains("death"))
                        client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, "Ghost Ship Died!"));
                }
                else if (text.Text.Contains("Dragon_Head_Leader"))
                {
                    if (text.Text.Contains("new"))
                        client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, "Rock Dragon Spawned!"));
                    else if (text.Text.Contains("killed") || text.Text.Contains("death"))
                        client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, "Rock Dragon Died!"));
                }
                else if (text.Text.Contains("Cube_God"))
                {
                    if (text.Text.Contains("new"))
                        client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, "Cube God Spawned!"));
                    else if (text.Text.Contains("killed") || text.Text.Contains("death"))
                        client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, "Cube God Died!"));
                }
                else if (text.Text.Contains("Skull_Shrine"))
                {
                    if (text.Text.Contains("new"))
                        client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, "Skull Shrine Spawned!"));
                    else if (text.Text.Contains("killed") || text.Text.Contains("death"))
                        client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, "Skull Shrine Died!"));
                }
                else
                {
                    client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, "Unknown New: " + text.Text));
                }
            }

            if (ChatAssistConfig.Default.DisableMessages && text.Recipient == "")
            {
                text.Send = false;
                return;
            }

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
                        return;
                    }
                    text.Send = false;
                }
            }
        }
    }
}
