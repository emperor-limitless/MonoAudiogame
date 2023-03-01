using System.Collections.Generic;
using System.Threading;
using LiteNetLib;
using LiteNetLib.Utils;
using Entity;
namespace Server
{
    public class GameServer
    {
        public ServerListener Listener;
        public Packets Packet;
        public Dictionary<NetPeer, Player> Players = new();
        public Dictionary<string, Player> PlayersNamed = new();
        public GameServer(int port)
        {
            Packet = new(this);
            Listener = new(this);
            Listener.Start(port);
        }
        public Player? GetPlayer(NetPeer peer)
        {
            if (!Players.ContainsKey(peer))
            {
                return null;
            }
            return Players[peer];
        }
        public Player? GetPlayerFrom(string name)
        {
            if (!PlayersNamed.ContainsKey(name))
            {
                return null;
            }

            return PlayersNamed[name];
        }
        public void AddPlayer(NetPeer peer, string name)
        {
            Player player = new(name, new());
            Players.Add(peer, player);
            Listener.Broadcast(new() { ["fun"] = "online", ["who"] = name });
        }
        public void RemovePlayer(NetPeer peer, bool loggedOff = false)
        {
            Player? player = GetPlayer(peer);
            if (player == null)
            {
                return;
            }
            if (loggedOff)
            {
                Listener.Broadcast(new() { ["fun"] = "disconnect", ["who"] = player.Name });
            }
            else
            {
                Listener.Broadcast(new() { ["fun"] = "offline", ["who"] = player.Name });
            }
            Players.Remove(peer);
            PlayersNamed.Remove(player.Name);
        }
        public void Run()
        {
            while (true)
            {
                Listener.Handle.PollEvents();
            }
        }
    }
}
