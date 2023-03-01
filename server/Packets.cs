using System;
using LiteNetLib;
using LiteNetLib.Utils;
using Entity;
using dict = System.Collections.Generic.Dictionary<dynamic, dynamic>;
namespace Server
{
    public class Packets
    {
        public GameServer server;
        public Packets(GameServer s)
        {
            server = s;
        }
        public void login(NetPeer peer, dict data)
        {
            if (server.GetPlayerFrom(data["user"]) != null)
            {
                server.Listener.Send(peer, new() { ["fun"] = "error", ["text"] = "A player with the same name is already logged in!" });
                return;
            }
            server.Listener.Send(peer, new() { ["fun"] = "connected" });
            server.AddPlayer(peer, data["user"]);
        }
        public void chat(NetPeer peer, dict data)
        {
            Player? handle = server.GetPlayer(peer);
            if (handle == null)
            {
                return;
            }
            server.Listener.Broadcast(new() { ["fun"] = "message", ["text"] = data["message"], ["buffer"] = "chat", ["sound"] = "notifications/chat.mp3" });
        }
        public void whoonline(NetPeer peer, dict data)
        {
            var players = server.Players.Values.Select(p => p.Name).ToList();
            if (players.Count == 1)
            {
                server.Listener.Send(peer, new() { ["fun"] = "message", ["text"] = "You are alone.", ["buffer"] = "misc" });
                return;
            }
            server.Listener.Send(peer, new() { ["fun"] = "message", ["text"] = $"There are currently {players.Count} players online: {string.Join(", ", players)}", ["buffer"] = "misc" });
        }
        public void ping(NetPeer peer, dict data)
        {
            server.Listener.Send(peer, new() { ["fun"] = "pong" });
        }
    }
}
