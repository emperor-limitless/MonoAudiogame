using System;
using Test;
using dict = System.Collections.Generic.Dictionary<dynamic, dynamic>;
namespace Network
{
    public class Packets
    {
        public GameTest game;
        public Packets(GameTest game)
        {
            this.game = game;
        }
        public void accepted(dict data)
        {
            game.Client.UserRequest();
        }
        public void connected(dict data)
        {
            game.Client.Connected = true;
        }
        public void message(dict data)
        {
            string buffer = "misc";
            if (data.ContainsKey("buffer"))
            {
                buffer = data["buffer"];
            }
            if (data.ContainsKey("sound"))
            {
                game.s.Play(data["sound"]);
            }
            game.Client.history.AddItem(buffer, data["text"]);
        }
        public void pong(dict data)
        {
            if (!game.Client.Pinging)
            {
                return;
            }
            game.Client.Pinging = false;
            game.speak($"The ping took {game.Client.PingTimer.Elapsed} Milliseconds", true);
            game.Client.PingTimer.Restart();
        }
        public void disconnect(dict data)
        {
            game.s.Play("notifications/offline.mp3");
            game.Client.history.AddItem("Connections", $"{data["who"]} Has lost the connection to the server!");
        }
        public void offline(dict data)
        {
            game.s.Play("notifications/offline.mp3");
            game.Client.history.AddItem("connections", $"{data["who"]} Has just went offline");
        }
        public void online(dict data)
        {
            game.s.Play("notifications/online.mp3");
            game.Client.history.AddItem("Connections", $"{data["who"]} Has just came online");
        }
    }
}
