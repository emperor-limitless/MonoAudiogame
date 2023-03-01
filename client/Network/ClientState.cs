using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using State;
using UI;
namespace Test
{
    public class NetworkState : GameState
    {
        public GameTest game;
        public bool Connected = false, Pinging = false;
        public History history = new();
        public Timer PingTimer = new();
        public NetworkState(GameTest g)
        {
            game = g;
            history.Add("All");
            history.Add("chat");
            history.Add("misc");
        }
        public void UserRequest()
        {
            UI.Input user = new(game, "Enter username", (username) => Connect(username));
            Append(user);
        }
        public void Connect(string username)
        {
            game.Listener.Send(new() { ["fun"] = "login", ["user"] = username });
            game.speak("Logging in");
            Stack.Remove(Last());
        }
        public override void OnEnter()
        {
            game.Listener.Connect("localhost", 6000);
        }
        public override void OnFocus()
        {
            if (!Connected)
            {
                game.Listener.Connect("localhost", 6000);
            }
        }
        public override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
            game.Listener.PollEvents();
            if (Connected && Last() is not Input)
            {
                GameLoop();
            }
        }
        public void none()
        {
        }
        public void ExitGame(int range = 1)
        {
            game.state.Pop(range);
            game.Listener.Stop();
        }
        public void GameLoop()
        {
            if (game.InputConfig.IsKeyPressed("Chat"))
            {
                UI.Input chat = new(game, "Enter you're chat message", (message) => SendChat(message));
                Append(chat);
            }
            else if (game.InputConfig.IsKeyPressed("CloseConnection"))
            {
                UI.Menus.YesNo ex = new(game, () => none(), (a) => ExitGame(2), (b) => game.state.Pop());
                ex.Title = "Are you sure you want to exit?";
                game.state.Append(ex);
            }
            else if (game.InputConfig.IsKeyPressed("WhoOnline"))
            {
                game.Listener.Send(new() { ["fun"] = "whoonline" });
            }
            else if (game.InputConfig.IsKeyPressed("Ping") && !Pinging)
            {
                game.Listener.Send(new() { ["fun"] = "ping" }, LiteNetLib.DeliveryMethod.ReliableOrdered);
                PingTimer.Restart();
                Pinging = true;
                game.speak("Pinging", true);
            }
            else if (game.InputConfig.IsKeyPressed("MessageBufferPrevious"))
            {
                history.Cycle(0);
            }
            else if (game.InputConfig.IsKeyPressed("MessageBufferFirst"))
            {
                history.Cycle(2);
            }
            else if (game.InputConfig.IsKeyPressed("MessageBufferNext"))
            {
                history.Cycle(1);
            }
            else if (game.InputConfig.IsKeyPressed("MessageBufferLast"))
            {
                history.Cycle(3);
            }
            else if (game.InputConfig.IsKeyPressed("MessageBackward"))
            {
                history.CycleItem(0);
            }
            else if (game.InputConfig.IsKeyPressed("MessageFirst"))
            {
                history.CycleItem(2);
            }
            else if (game.InputConfig.IsKeyPressed("MessageForward"))
            {
                history.CycleItem(1);
            }
            else if (game.InputConfig.IsKeyPressed("MessageLast"))
            {
                history.CycleItem(3);
            }
        }
        public void SendChat(string message)
        {
            if (message == "")
            {
                game.speak("Canceled", true);
                Stack.Remove(Last());
                return;
            }
            game.Listener.Send(new()
            {
                ["fun"] = "chat",
                ["message"] = message
            });
            Stack.Remove(Last());
        }
    }
}
