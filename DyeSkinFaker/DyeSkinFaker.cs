using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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

namespace DyeSkinFaker
{
    public class DyeSkinFaker : IPlugin
    {
		public string GetAuthor()
		{ return "Todddddd"; }

		public string GetName()
		{ return "Dye/Skin Faker"; }

		public string GetDescription()
		{
			return "Fake your dye or skin.\nUse \"/dyefaker\" to open the settings.";
		}

		public static Dictionary<int, string> LargeDyes = new Dictionary<int, string>();
		public static Dictionary<int, string> SmallDyes = new Dictionary<int, string>();
		public static Dictionary<int, string> Skins = new Dictionary<int, string>();

		public string[] GetCommands()
		{ return new string[] { "/dyefaker", "/dyefaker <enable/disable>" }; }

		public void Initialize(Proxy proxy)
		{
			// Add empty values to all the dictionary's so that way there is a blank option on the config drop downs
			if (!LargeDyes.ContainsKey(0))
				LargeDyes.Add(0, "");
			if (!SmallDyes.ContainsKey(0))
				SmallDyes.Add(0, "");
			if (!Skins.ContainsKey(0))
				Skins.Add(0, "");
			// Go through the RAW xml from the objects file and get all the skin and dyes
			XDocument doc = XDocument.Parse(GameData.RawObjectsXML);
			doc.Element("Objects")
				.Elements("Object")
				.ForEach(obj =>
				{
					string ClassName = obj.ElemDefault("Class", "");
					string name = obj.AttrDefault("id", "");
					// Check if the class is a Dye
					if (ClassName == "Dye")
					{
						if (obj.HasElement("Tex1"))
						{
							// Large Dye
							int id = obj.Element("Tex1").Value.ParseHex();
							if (!LargeDyes.ContainsKey(id))
								LargeDyes.Add(id, name);
						}
						else if (obj.HasElement("Tex2"))
						{
							// Small Dye
							int id = obj.Element("Tex2").Value.ParseHex();
							if (!SmallDyes.ContainsKey(id))
								SmallDyes.Add(id, name);
						}
					}
					// Check if the class is Skin
					if (ClassName == "Skin" /*&& obj.HasElement("Skin")*/)
					{
						int id = obj.AttrDefault("type", "0x0").ParseHex();
						if (!Skins.ContainsKey(id))
							Skins.Add(id, name);
					}
					// Double check if we have all the skins by checking the equipment with "skinType" attributes
					if (obj.HasElement("Activate"))
					{
						foreach (var attr in obj.Element("Activate").Attributes())
						{
							if (attr.Name == "skinType")
							{
								if(!Skins.ContainsKey(attr.Value.ParseInt()))
									Skins.Add(attr.Value.ParseInt(), name);
								break;
							}
						}
					}
				});


			proxy.HookPacket(PacketType.UPDATE, OnUpdate);
			//proxy.HookPacket(PacketType.UPDATEPET, OnUpdatePet);
			//proxy.HookPacket(PacketType.RESKIN, OnReskin);
			//proxy.HookPacket(PacketType.CLIENTSTAT, OnClientStat);
			//proxy.HookPacket(PacketType.RESKINUNLOCK, OnReskinUnlock);

			//proxy.ClientConnected += OnConnect;

			proxy.HookCommand("dyefaker", OnCommand);
		}

		/*public void OnReskinUnlock(Client client, Packet packet)
		{
			ReskinUnlock ru = (ReskinUnlock)packet;
			Console.WriteLine("ReskinUnlock: {0}", ru.SkinId);
		}*/


		public void OnCommand(Client client, string command, string[] args)
		{
			if (args.Length == 0)
				PluginUtils.ShowGUI(new FrmConfig(client));
			else
			{
				if (args[0] == "enable" || args[0] == "on")
					Config.Default.Enabled = true;
				else if (args[0] == "disable" || args[0] == "off")
					Config.Default.Enabled = false;

				Config.Default.Save();
			}
		}

		/*public void OnReskin(Client client, Packet packet)
		{
			ReskinPacket rp = (ReskinPacket)packet;
			Console.WriteLine("Reskin: {0}", rp.SkinId);
		}
		
		public void OnUpdatePet(Client client, Packet packet)
		{
			UpdatePetPacket upp = (UpdatePetPacket)packet;
			//Console.WriteLine("PetId: {0}", upp.PetId);
			//petid = upp.PetId;
		}*/

		/*public void OnConnect(Client client)
		{
		}

		public static void OnChange(Client client)
		{
			ReskinUnlock reskin = (ReskinUnlock)Packet.Create(PacketType.RESKINUNLOCK);
			reskin.SkinId = Config.Default.Skin;
			client.SendToClient(reskin);
		}*/

		public void OnUpdate(Client client, Packet packet)
		{
			UpdatePacket up = (UpdatePacket)packet;
			if (Config.Default.Enabled)
			{
				for (int i = 0; i < up.NewObjs.Length; i++)
				{
					if (up.NewObjs[i].Status.ObjectId == client.ObjectId)
					{
						for (int j = 0; j < up.NewObjs[i].Status.Data.Length; j++)
						{
							if (up.NewObjs[i].Status.Data[j].Id == 32)
							{
								if (Config.Default.LargeDye > 0)
									up.NewObjs[i].Status.Data[j].IntValue = Config.Default.LargeDye;
							}
							if (up.NewObjs[i].Status.Data[j].Id == 33)
							{
								if (Config.Default.SmallDye > 0)
									up.NewObjs[i].Status.Data[j].IntValue = Config.Default.SmallDye;
							}
							if (up.NewObjs[i].Status.Data[j].Id == 80)
							{
								if (Config.Default.Skin > 0)
									up.NewObjs[i].Status.Data[j].IntValue = Config.Default.Skin;
							}
						}
					}
				}
			}
		}
    }
}
