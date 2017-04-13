using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Globalization;

using Lib_K_Relay;
using Lib_K_Relay.Interface;
using Lib_K_Relay.Networking;
using Lib_K_Relay.Networking.Packets;
using Lib_K_Relay.Networking.Packets.Server;
using Lib_K_Relay.GameData;
using Lib_K_Relay.GameData.DataStructures;
using Lib_K_Relay.Utilities;

namespace Sprite
{
	internal static class Extensions
	{
		public static void OryxMessage(this Client client, string fmt, params object[] args)
		{
			client.SendToClient(PluginUtils.CreateOryxNotification("Sprite", string.Format(fmt, args)));
		}
	}

    public class Sprite : IPlugin
    {
		public Dictionary<Client, string> CurrentMap = new Dictionary<Client, string>();

		public string GetAuthor()
		{ return "CrazyJani (updated by Todddddd)"; }

		public string GetName()
		{ return "Sprite World No Clip"; }

		public string GetDescription()
		{ return "Enables no-clipping in Sprite World. Also prevents sliding in Ice Cave.\nOptions:\nTrees - enabled by default, remove the trees in sprite world\n" +
			"Space - enabled by default, relace the space in Sprite World with gold tile to allow walked on\n" +
			"Floor - disabled by default, relace the floor tiles in Sprite World with gold tiles.\n" +
			"Ice - enabled by default, relace the floor in Ice Cave with gold told to prevent sliding"; }

		public string[] GetCommands()
		{ return new string[] { "/sprite [on/off] (enables/disables all)", "/sprite [option] [on/off]" }; }

		public void Initialize(Proxy proxy)
		{
			proxy.HookPacket(PacketType.UPDATE, OnUpdate);
			proxy.HookPacket(PacketType.MAPINFO, OnMapInfo);
			proxy.HookCommand("sprite", OnCommand);
		}

		public void OnCommand(Client client, string command, string[] args)
		{
			if (args.Length == 0) return;
			else
			{
				if (args[0] == "on")
				{
					// Enabled all
					Config.Default.SpriteTrees = true;
					Config.Default.SpriteSpace = true;
					Config.Default.SpriteFloor = true;
					Config.Default.IceSlide    = true;

					client.SendToClient(PluginUtils.CreateOryxNotification("Sprite", "All Enabled"));
				}
				else if (args[0] == "off")
				{
					// Disable all
					Config.Default.SpriteTrees = false;
					Config.Default.SpriteSpace = false;
					Config.Default.SpriteFloor = false;
					Config.Default.IceSlide    = false;

					client.SendToClient(PluginUtils.CreateOryxNotification("Sprite", "All Disabled"));
				}
				else if (args[0] == "trees")
				{
					Config.Default.SpriteTrees = ( args[1] == "on" ? true : false );
					client.SendToClient(PluginUtils.CreateOryxNotification("Sprite", "Sprite Trees: " + (Config.Default.SpriteTrees ? "Enabled" : " Disabled")));
				}
				else if (args[0] == "space")
				{
					Config.Default.SpriteSpace = (args[1] == "on" ? true : false);
					client.SendToClient(PluginUtils.CreateOryxNotification("Sprite", "Sprite Space: " + (Config.Default.SpriteSpace ? "Enabled" : " Disabled")));
				}
				else if (args[0] == "floor")
				{
					Config.Default.SpriteSpace = (args[1] == "on" ? true : false);
					client.SendToClient(PluginUtils.CreateOryxNotification("Sprite", "Sprite Floor: " + (Config.Default.SpriteFloor ? "Enabled" : " Disabled")));
				}
				else if (args[0] == "ice")
				{
					Config.Default.IceSlide = (args[1] == "on" ? true : false);
					client.SendToClient(PluginUtils.CreateOryxNotification("Sprite", "Ice Slide: " + (Config.Default.IceSlide ? "Enabled" : " Disabled")));
				}
				else
				{
					client.SendToClient(PluginUtils.CreateOryxNotification("Sprite", "Invalid Argument: " + args[0]));
				}
			}
		}

		public void OnMapInfo(Client client, Packet packet)
		{
			MapInfoPacket mip = (MapInfoPacket)packet;
			CurrentMap[client] = mip.Name;
		}

		public void OnUpdate(Client client, Packet packet)
		{
			if (CurrentMap.ContainsKey(client))
			{
				if (CurrentMap[client] == "Sprite World" || CurrentMap[client] == "Ice Cave" || CurrentMap[client] == "The Inner Sanctum")
				{
					UpdatePacket up = (UpdatePacket)packet;
					if(Config.Default.SpriteTrees)
						for (int i = 0; i < up.NewObjs.Length; i++)
							if (up.NewObjs[i].ObjectType <= GameData.Objects.ByName("Yellow Sprite Tree").ID && up.NewObjs[i].ObjectType >= GameData.Objects.ByName("White Sprite Tree").ID)
								up.NewObjs[i].ObjectType = 0;

					for (int i = 0; i < up.Tiles.Length; i++)
					{
						if(Config.Default.SpriteSpace)
							if (up.Tiles[i].Type == GameData.Tiles.ByName("Space").ID )
								up.Tiles[i].Type = GameData.Tiles.ByName("Gold Tile").ID;
						if(Config.Default.SpriteFloor)
							if (up.Tiles[i].Type > GameData.Tiles.ByName("White Alpha Square").ID && up.Tiles[i].Type <= GameData.Tiles.ByName("Yellow Alpha Square").ID)
								up.Tiles[i].Type = GameData.Tiles.ByName("Gold Tile").ID;
						if(Config.Default.IceSlide)
							if(up.Tiles[i].Type == GameData.Tiles.ByName("Ice Slide").ID)
								up.Tiles[i].Type = GameData.Tiles.ByName("Gold Tile").ID;
					}
				}
			}
		}
    }
}
