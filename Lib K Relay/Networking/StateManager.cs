using Lib_K_Relay.Networking.Packets.Client;
using Lib_K_Relay.Networking.Packets.DataObjects;
using Lib_K_Relay.Networking.Packets.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib_K_Relay.Networking
{
    public class StateManager
    {
        private Proxy _proxy;

        public void Attach(Proxy proxy)
        {
            _proxy = proxy;
            proxy.HookPacket<CreateSuccessPacket>(OnCreateSuccess);
            proxy.HookPacket<MapInfoPacket>(OnMapInfo);
            proxy.HookPacket<UpdatePacket>(OnUpdate);
            proxy.HookPacket<NewTickPacket>(OnNewTick);
            proxy.HookPacket<PlayerShootPacket>(OnPlayerShoot);
            proxy.HookPacket<MovePacket>(OnMove);
        }

        private void OnMove(Client client, MovePacket packet)
        {
            client.PreviousTime = packet.Time;
            client.LastUpdate = Environment.TickCount;
            client.PlayerData.Pos = packet.NewPosition;
        }

        private void OnPlayerShoot(Client client, PlayerShootPacket packet)
        {
            client.PlayerData.Pos = new Location()
            {
                X = packet.Position.X - 0.3f * (float)Math.Cos(packet.Angle),
                Y = packet.Position.Y - 0.3f * (float)Math.Sin(packet.Angle)
            };
        }

        private void OnNewTick(Client client, NewTickPacket packet)
        {
            client.PlayerData.Parse(packet);
        }

        private void OnMapInfo(Client client, MapInfoPacket packet)
        {
            client.State["MapInfo"] = packet;
        }

        private void OnCreateSuccess(Client client, CreateSuccessPacket packet)
        {
            client.PlayerData = new PlayerData(packet.ObjectId, client.State.Value<MapInfoPacket>("MapInfo"));
        }

        private void OnUpdate(Client client, UpdatePacket packet)
        {
            client.PlayerData.Parse(packet);
            if (client.State.ACCID != null) return;
            //client.State.ACCID = client.PlayerData.AccountId;

            State resolvedState = null;

            foreach (State cstate in _proxy.States.Values)
                if (cstate.ACCID == client.PlayerData.AccountId)
                    resolvedState = cstate;

            if (resolvedState == null)
                client.State.ACCID = client.PlayerData.AccountId;
            else
            {
                /*foreach (var pair in client.State.States)
                    resolvedState[pair.Key] = pair.Value;
                foreach (var pair in client.State.States)
                    resolvedState[pair.Key] = pair.Value;
                client.State = resolvedState;*/

                if (client.State.ACCID == null && resolvedState.ACCID != null)
                    client.State.ACCID = resolvedState.ACCID;
                if (client.State.ConRealKey == null && resolvedState.ConRealKey != null)
                    client.State.ConRealKey = resolvedState.ConRealKey;
                if (client.State.ConTargetAddress == null && resolvedState.ConTargetAddress != null)
                {
                    client.State.ConTargetAddress = resolvedState.ConTargetAddress;
                    client.State.ConTargetPort = resolvedState.ConTargetPort;
                }
                if (client.State.GUID == null && resolvedState.GUID != null)
                    client.State.GUID = resolvedState.GUID;
                if (client.State.LastDungeon == null && resolvedState.LastDungeon != null)
                    client.State.LastDungeon = resolvedState.LastDungeon;
                if (client.State.LastRealm == null && resolvedState.LastRealm != null)
                    client.State.LastRealm = resolvedState.LastRealm;

                foreach (var s in resolvedState.States)
                {
                    if (!client.State.States.ContainsKey(s.Key))
                        client.State.States.Add(s.Key, s.Value);
                    else if (client.State.States[s.Key] == null && s.Value != null)
                    {
                        client.State.States[s.Key] = s.Value;
                    }
                }
            }
        }
    }
}
