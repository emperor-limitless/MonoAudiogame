using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Reflection;
using LiteNetLib;
using LiteNetLib.Utils;
using Test;
using Newtonsoft.Json;

namespace Network
{
    public class ClientListener : INetEventListener
    {
        public NetManager Manager;
        private bool connected;
        public bool Connected
        {
            get
            {
                return connected;
            }
        }
        public GameTest game;
        public ClientListener(GameTest game)
        {
            Manager = new(this);
            this.game = game;
        }
        public bool Connect(string host, int port)
        {
            if (!Manager.Start())
            {
                return false;
            }
            Manager.Connect(host, port, "gamekey");
            return true;
        }
        public void Send(Dictionary<dynamic, dynamic> packet, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
        {
            Manager.SendToAll(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(packet)), deliveryMethod);
        }
        public void Stop()
        {
            game.Client.Connected = false;
            connected = false;
            Manager.Stop();
        }
        public void PollEvents()
        {
            Manager.PollEvents();
        }
        public void OnPeerConnected(NetPeer peer)
        {
        }
        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            connected = false;
            game.Client.Connected = false;
            game.speak("disconnected: " + disconnectInfo.Reason);
        }
        public void OnNetworkError(IPEndPoint endPoint, SocketError socketErrorCode)
        {
            Console.WriteLine("[Client] error! " + socketErrorCode);
        }
        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            string str = Encoding.UTF8.GetString(reader.GetRemainingBytes());
            Dictionary<dynamic, dynamic> pk = JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(str);
            if (!pk.ContainsKey("fun"))
            {
                return;
            }
            Type tp = game.Packet.GetType();
            MethodInfo method = tp.GetMethod(pk["fun"]);
            if (method != null)
            {
                method.Invoke(game.Packet, new[] { pk });
            }
        }
        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
        }
        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
        }
        public void OnConnectionRequest(ConnectionRequest request)
        {
        }
    }
}
