using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Lib_K_Relay;
using Lib_K_Relay.GameData;
using Lib_K_Relay.Interface;
using Lib_K_Relay.Networking;
using Lib_K_Relay.Networking.Packets;
using Lib_K_Relay.Networking.Packets.Client;
using Lib_K_Relay.Networking.Packets.Server;
using Lib_K_Relay.Networking.Packets.DataObjects;
using Lib_K_Relay.Utilities;

namespace Follow
{
	/// <summary>
	/// Represents a cached world map
	/// </summary>
	public class Map
	{
		/// <summary>
		/// The UUID of the map
		/// </summary>
		public uint UUID;

		/// <summary>
		/// The dimensions of the map
		/// </summary>
		public int Width, Height;

		/// <summary>
		/// The raw data of the map such that Data[x, y] is the tile type
		/// </summary>
		public ushort[,] Data;

		/// <summary>
		/// Constructs an empty map from the given packet
		/// </summary>
		/// <param name="info">The MapInfoPacket</param>
		public Map(MapInfoPacket info)
		{
			UUID = info.Fp;
			Width = info.Width;
			Height = info.Height;
			Data = new ushort[Width, Height];
		}

		/// <summary>
		/// Gets the type of the tile at the given coordinates
		/// </summary>
		/// <returns>The tile type</returns>
		public ushort At(int x, int y)
		{
			return Data[x, y];
		}

		/// <summary>
		/// Gets the type of the tile at the given coordinates
		/// </summary>
		/// <returns>The tile type</returns>
		public ushort At(float x, float y)
		{
			return Data[(int)x, (int)y];
		}
	}

	public class ClientInfo
	{
		public Client client;
		public bool Master, Slave;
		public Stopwatch sw = new Stopwatch();
		public Map map;

		public ClientInfo(Client c)
		{
			client = c;
			Master = false;
			Slave = false;
			sw.Start();
		}

		public void SetMaster()
		{
			Slave = false;
			Master = true;
		}
		public void SetSlave()
		{
			Master = false;
			Slave = true;
		}

		public void UpdateTick()
		{
			sw.Restart();
		}

		public ushort CurrentTileType()
		{
			return map.At(client.PlayerData.Pos.X, client.PlayerData.Pos.Y);
		}
	}

    public class Follow : IPlugin
    {
		public Dictionary<Client, ClientInfo> listOfClients = new Dictionary<Client, ClientInfo>();

		public Client master = null;

		public bool _enabled = false;

		public string GetAuthor()
		{ return "Todddddd"; }

		public string GetName()
		{ return "Follow"; }

		public string GetDescription()
		{ return "Set up clients to follow a master client around the map"; }

		public string[] GetCommands()
		{ return new string[] { "/follow [master|slave]", "/follow [start|stop]" }; }

		public void Initialize(Proxy proxy)
		{
			proxy.ClientConnected += OnConnect;
			proxy.ClientDisconnected += OnDisconnect;

			proxy.HookCommand("follow", OnCommand);
			proxy.HookPacket(PacketType.GOTO, OnGoto);
			proxy.HookPacket(PacketType.GOTOACK, OnGotoAck);
			proxy.HookPacket(PacketType.NEWTICK, OnNewTick);
			proxy.HookPacket(PacketType.USEPORTAL, OnUsePortal);
			proxy.HookPacket(PacketType.MAPINFO, OnMapInfo);
			proxy.HookPacket(PacketType.UPDATE, OnUpdate);
		}

		public void OnCommand(Client client, string command, string[] args)
		{
			if (args.Length == 0) return;
			if (args[0] == "master")
			{
				master = client;
				listOfClients[client].SetMaster();
				client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, "Set as Master Client!"));
			}
			else if(args[0] == "slave")
			{
				listOfClients[client].SetSlave();
				client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, "Set as Slave Client!"));
			}
			else if (args[0] == "start")
			{
				_enabled = true;
				client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, "Follow Started!"));
			}
			else if (args[0] == "stop")
			{
				_enabled = false;
				client.SendToClient(PluginUtils.CreateNotification(client.ObjectId, "Follow Stopped!"));
			}
		}

		public void OnGoto(Client client, Packet packet)
		{
			GotoPacket gotop = (GotoPacket)packet;
			if (_enabled && listOfClients[client].Slave)
			{
				// Send our own GOTOACK when we get a GOTO from the server
				GotoAckPacket gotoack = (GotoAckPacket)Packet.Create(PacketType.GOTOACK);
				gotoack.Time = client.Time;
				client.SendToServer(gotoack);
			}
		}

		public void OnUsePortal(Client client, Packet packet)
		{
			UsePortalPacket upp = (UsePortalPacket)packet;
			// Check if the master client used a portal
			if (_enabled && listOfClients[client].Master)
			{
				// Go through each client in the list and send the same packet if they are a slave
				foreach (var ci in listOfClients)
				{
					if (ci.Value.Slave)
					{
						ci.Value.client.SendToServer(upp);
					}
				}
			}
		}

		public void OnGotoAck(Client client, Packet packet)
		{
			GotoAckPacket goap = (GotoAckPacket)packet;
			if (_enabled && listOfClients[client].Slave)
			{
				// Block the GOTOACK if the client is a slave
				goap.Send = false;
			}
		}

		public void OnNewTick(Client client, Packet packet)
		{
			NewTickPacket ntp = (NewTickPacket)packet;
			if (_enabled && listOfClients[client].Slave)
			{
				// Distance to Master
				double Distance = Math.Sqrt(Math.Pow(master.PlayerData.Pos.X - client.PlayerData.Pos.X, 2) + Math.Pow(master.PlayerData.Pos.Y - client.PlayerData.Pos.Y, 2));
				// Will hold the angle
				float Angle = 0;
				// Get the total milliseconds since the last NewTick
				long timeElapsed = listOfClients[client].sw.ElapsedMilliseconds;
				// This will be used as a multiplier, ive capped it at 250
				timeElapsed = timeElapsed >= 200 ? 200 : timeElapsed;

				// The formula from the client is (MIN_MOVE_SPEED + this.speed_ / 75 * (MAX_MOVE_SPEED - MIN_MOVE_SPEED)) * this.moveMultiplier_
				// this.moveMultiplier_ = the tile movement speed (1 by default)
				// MIN_MOVE_SPEED = 0.004 * this.moveMultiplier_
				// MAX_MOVE_SPEED = 0.0096
				// I dont take into account tile movement speed i justed used the default value of 1
				// I also decreased the max movement speed to avoid disconnect
				float moveMultiplier = GameData.Tiles.ByID(listOfClients[client].CurrentTileType()).Speed;
				float min_speed = 0.004f * moveMultiplier;
				float speed = ((min_speed + (client.PlayerData.Speed / 75.0f * (0.007f - min_speed))) * moveMultiplier) * (timeElapsed);
				Console.WriteLine("SW: {0}ms, Dist: {1}, Spd: {2}, TPT: {3}", listOfClients[client].sw.ElapsedMilliseconds, Distance, speed, client.PlayerData.TilesPerTick());

				//speed = client.PlayerData.TilesPerTick();


				Location NewLoc = new Location();
				// Check if the distance to the master is greater then the distance the slave can move
				if (Distance > speed)
				{
					// Calculate the angle
					Angle = (float)Math.Atan2(master.PlayerData.Pos.Y - client.PlayerData.Pos.Y, master.PlayerData.Pos.X - client.PlayerData.Pos.X);
					// Calculate the new location
					NewLoc.X = client.PlayerData.Pos.X + (float)Math.Cos(Angle) * speed;
					NewLoc.Y = client.PlayerData.Pos.Y + (float)Math.Sin(Angle) * speed;
				}
				else
				{
					// Set the move location as the master location
					NewLoc.X = master.PlayerData.Pos.X;
					NewLoc.Y = master.PlayerData.Pos.Y;
				}
				// Send the GOTO packet
				GotoPacket go = (GotoPacket)Packet.Create(PacketType.GOTO);
				go.ObjectId = client.ObjectId;
				go.Location = new Location();
				go.Location = NewLoc;
				client.SendToClient(go);
			}

			if (listOfClients[client].Slave)
			{
				// This is used to get the number of milliseconds between each NewTick
				listOfClients[client].UpdateTick();
			}
		}

		public void OnMapInfo(Client client, Packet packet)
		{
			MapInfoPacket mip = (MapInfoPacket)packet;
			if(listOfClients.ContainsKey(client))
				listOfClients[client].map = new Map(mip);
		}

		public void OnUpdate(Client client, Packet packet)
		{
			UpdatePacket up = (UpdatePacket)packet;
			foreach (Tile tile in up.Tiles)
				if(listOfClients.ContainsKey(client))
					listOfClients[client].map.Data[tile.X, tile.Y] = tile.Type;
		}

		// TODO: Keep settings between disconnects (like using a portal)
		public void OnConnect(Client client)
		{
			if (!listOfClients.ContainsKey(client))
			{
				listOfClients.Add(client, new ClientInfo(client));
			}
			else
			{
				listOfClients[client] = new ClientInfo(client);
			}
		}
		public void OnDisconnect(Client client)
		{
			// If the Master client disconnects turn off the plugin
			if(listOfClients.ContainsKey(client))
				if (listOfClients[client].Master)
					_enabled = false;

			if (listOfClients.ContainsKey(client))
			{
				listOfClients.Remove(client);
			}
		}

    }
}
