using System;
using System.Text;
using System.Reflection;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using LiteNetLib;
using LiteNetLib.Utils;
using Newtonsoft.Json;
using dict = System.Collections.Generic.Dictionary<dynamic, dynamic>;

namespace Server
{
    public class ServerListener : INetEventListener
    {
        public NetManager Handle;
        public GameServer Server;
        public ServerListener(GameServer s)
        {
            Handle = new(this);
            Server = s;
        }
        public bool Start(int port)
        {
            if (!Handle.Start(port))
            {
                return false;
            }
            return true;
        }
        public void Send(NetPeer peer, dict packet, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
        {
            Console.WriteLine(packet);
            peer.Send(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(packet)), deliveryMethod);
        }
        public void Broadcast(dict packet, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered)
        {
            Handle.SendToAll(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(packet)), deliveryMethod);
        }

        public void Stop()
        {
            Handle.Stop();
        }
        public void OnPeerConnected(NetPeer peer)
        {
            Send(peer, new() { ["fun"] = "accepted" });
        }
        public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Server.RemovePlayer(peer);
        }
        public void OnNetworkError(IPEndPoint endPoint, SocketError socketErrorCode)
        {
            Console.WriteLine("[Server] error: " + socketErrorCode);
        }
        public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
        {
            dict pk = JsonConvert.DeserializeObject<dict>(Encoding.UTF8.GetString(reader.GetRemainingBytes()));
            Console.WriteLine(pk);
            if (!pk.ContainsKey("fun"))
            {
                return;
            }
            Type tp = Server.Packet.GetType();
            MethodInfo? method = tp.GetMethod(pk["fun"]);
            if (method != null)
            {
                method.Invoke(Server.Packet, new Object[] { peer, pk });
            }
        }
        public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
        {
            Console.WriteLine("[Server] ReceiveUnconnected: {0}", reader.GetString(100));
        }

        public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
        {
        }

        public void OnConnectionRequest(ConnectionRequest request)
        {
            var acceptedPeer = request.AcceptIfKey("gamekey");
            Console.WriteLine("[Server] ConnectionRequest. Ep: {0}, Accepted: {1}",
                request.RemoteEndPoint,
                acceptedPeer != null);
        }
    }
}
