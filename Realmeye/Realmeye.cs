using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Xml.Linq;
using System.Linq;


using Lib_K_Relay;
using Lib_K_Relay.Interface;
using Lib_K_Relay.Networking;
using Lib_K_Relay.Networking.Packets;
using Lib_K_Relay.Networking.Packets.Client;
using Lib_K_Relay.GameData;
using Lib_K_Relay.GameData.DataStructures;
using Lib_K_Relay.Utilities;
using Realmeye.Properties;

namespace Realmeye
{
	public class Realmeye : IPlugin
    {
		public static string ItemAbbrXML { get; private set; }

		public Dictionary<string, string> ItemAbbrs = new Dictionary<string, string>();

		public string GetAuthor()
		{ return "CrazyJani (updated by Todddddd)"; }

		public string GetName()
		{ return "Realmeye"; }

		public string GetDescription()
		{ return "Quick commands to send tells to MrEyeball and other actions. This plugin contains a list of the most common item abbreviations that you can use with the /sell command."; }

		public string[] GetCommands()
		{ return new string[] { "/player <playername>", "/friends", "/hide", "/stats", "/mates", "/market" }; }

		public void Initialize(Proxy proxy)
		{
			proxy.HookCommand("friends", OnCommand);
			proxy.HookCommand("hide", OnCommand);
			proxy.HookCommand("stats", OnCommand);
			proxy.HookCommand("mates", OnCommand);
			proxy.HookCommand("stats", OnCommand);
			proxy.HookCommand("player", OnWebCommand);
			proxy.HookCommand("market", OnWebCommand);
			proxy.HookCommand("sell", OnWebCommand);

			LoadXMLData();
		}

		public void LoadXMLData()
		{
			// Load the XML of item abbreviations
			ItemAbbrXML = Resources.ItemAbbreviations;
			XDocument doc = XDocument.Parse(ItemAbbrXML);
			doc.Element("items")
				.Elements("item")
				.ForEach(item =>
				{
					string name = item.AttrDefault("name", "");
					foreach (var abbr in item.Descendants("abbr"))
					{
						ItemAbbrs.Add(abbr.Value, name);
					}
				});
			PluginUtils.Log("RealmEye", "Found {0} item abbreviations.", ItemAbbrs.Count);
		}

		private void OnCommand(Client client, string command, string[] args)
		{
			PlayerTextPacket ptp = (PlayerTextPacket)Packet.Create(PacketType.PLAYERTEXT);
			ptp.Text = "/tell mreyeball " + (command == "hide" ? "hide me" : command);
			client.SendToServer(ptp);
		}

		private void OnWebCommand(Client client, string command, string[] args)
		{
			if (command == "market")
			{
				Process.Start("https://www.realmeye.com/current-offers");
			}
			else if (command == "player")
			{
				if (args.Length > 0)
				{
					Process.Start("https://www.realmeye.com/player/" + args[0]);
				}
			}
			else if (command == "sell")
			{
				if (args.Length > 1)
				{
					int item1 = 0;
					int item2 = 0;

					if (ItemAbbrs.ContainsKey(args[0].ToLower()))
					{
						// Try to find the item id from our list of abbreviations
						try { item1 = GameData.Items.ByName(ItemAbbrs[args[0].ToLower()]).ID; }
						catch { }
					}
					else
					{
						// Try to find the item id from the string entered as is
						try { item1 = GameData.Items.ByName(args[0]).ID; }
						catch { }
					}

					if (ItemAbbrs.ContainsKey(args[1].ToLower()))
					{
						// Try to find the item id from our list of abbreviations
						try { item2 = GameData.Items.ByName(ItemAbbrs[args[1].ToLower()]).ID; }
						catch { }
					}
					else
					{
						// Try to find the item id from the string entered as is
						try { item2 = GameData.Items.ByName(args[1]).ID; }
						catch { }
					}

					if (item2 == 0)
					{
						client.SendToClient(PluginUtils.CreateOryxNotification("Error", "Couldn't find any item with the name or abbreviation \"" + args[1] + "\""));
						return;
					}

					Process.Start("https://www.realmeye.com/offers-to/sell/" + item2.ToString() + "/" + item1.ToString());
				}
			}
		}
    }
}
