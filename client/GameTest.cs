using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using DavyKager;
using static Program;
using Data;
using Synthizer;
using UI;

namespace Test
{
    public class GameTest : Game
    {
        private GraphicsDeviceManager _graphics;
        public Audio.SoundPool s;
        public Network.Packets Packet;
        public Network.ClientListener Listener;
        public MainGameState state;
        public NetworkState Client;
        public KeyboardState oldKeyboardState, keyboardState;
        public string InputText = "";
        public Database UserConfig;
        public Config.KeyConfig InputConfig;
        public GameTest()
        {
            if (!Directory.Exists("config"))
            {
                Directory.CreateDirectory("config");
            }
            _graphics = new(this);
            Window.TextInput += (_, text) => InputText = text.Character.ToString();
            Tolk.Load();
            Tolk.TrySAPI(true);
            Packet = new(this);
            Listener = new(this);
            synthizer.initialize(LogLevel.debug, LoggingBackend.STDERR);
            s = new();
            s.Hrtf = true;
            state = new(this);
            Client = new(this);
            InputConfig = new();
            MainMenu();
        }
        public void MainMenu()
        {
            UI.Menu m = new(this, () => Exit());
            m.Title = "Main menu";
            m.Music = "music/main_menu.mp3";
            UI.MenuItem i1 = new("Login", (s) => state.Append(Client), true, m);
            UI.MenuItem i2 = new("Create account", s => state.Append(Client), true, m);
            UI.MenuItem i3 = new("Set account", s => state.Append(Client), true, m);
            UI.MenuItem i4 = new("Exit", s => Exit(), true, m);
            m.Items.Add(i1);
            m.Items.Add(i2);
            m.Items.Add(i3);
            m.Items.Add(i4);
            state.Append(m);
        }
        public void Load()
        {
            UserConfig = new("config/user_settings.dat", "hellofjekamfnrikdjfhnali$#@jfurk")
            {
                ["name"] = "",
                ["password"] = "",
            };
        }
        public bool IsKeyDown(Keys key)
        {
            return keyboardState.IsKeyDown(key);
        }
        public bool IsKeyPressed(Keys key)
        {
            return keyboardState.IsKeyDown(key) && !oldKeyboardState.IsKeyDown(key);
        }
        protected override void OnExiting(Object sender, EventArgs args)
        {
            Tolk.Unload();
            synthizer.shutdown();
            InputConfig.Save();
            UserConfig.Save();
        }
        protected override void Initialize()
        {
            base.Initialize();
        }
        protected override void LoadContent()
        {
            Load();
        }
        public bool speak(string text, bool interrupt = false)
        {
            return Tolk.Speak(text, interrupt);
        }
        protected override void Update(GameTime gameTime)
        {
            oldKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();
            state.OnUpdate(gameTime);
            InputText = "";
        }
    }
}
