using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Data;
using static Program;
namespace Test.Config
{
    public class KeyConfig
    {
        private readonly Dictionary<string, List<Keys>> keyCombinations = new();
        private readonly Database db;
        public KeyConfig()
        {
            DefaultKeyConfig();
            db = new Database("config/keys.json", "");
            Load();
        }
        public void DefaultKeyConfig()
        {
            AddKeyCombination("MenuUp", Keys.W);
            AddKeyCombination("MenuDown", Keys.S);
            AddKeyCombination("MenuLeft", Keys.A);
            AddKeyCombination("MenuRight", Keys.D);
            AddKeyCombination("MenuClose", Keys.Escape);
            AddKeyCombination("MenuSelect", Keys.Enter);
            AddKeyCombination("MenuMusicUp", Keys.Home);
            AddKeyCombination("MenuMusicDown", Keys.End);
            AddKeyCombination("MessageForward", Keys.PageDown);
            AddKeyCombination("MessageBackward", Keys.PageUp);
            AddKeyCombination("MessageFirst", Keys.LeftShift, Keys.PageUp);
            AddKeyCombination("MessageLast", Keys.LeftShift, Keys.PageDown);
            AddKeyCombination("MessageBufferNext", Keys.LeftControl, Keys.PageDown);
            AddKeyCombination("MessageBufferPrevious", Keys.LeftControl, Keys.PageUp);
            AddKeyCombination("MessageBufferFirst", Keys.LeftControl, Keys.LeftShift, Keys.PageUp);
            AddKeyCombination("MessageBufferLast", Keys.LeftControl, Keys.LeftShift, Keys.PageDown);
            AddKeyCombination("WhoOnline", Keys.F1);
            AddKeyCombination("Ping", Keys.F2);
            AddKeyCombination("CloseConnection", Keys.Escape);
            AddKeyCombination("Chat", Keys.OemPipe);
        }
        public void AddKeyCombination(string keyName, params Keys[] keys)
        {
            if (keyCombinations.ContainsKey(keyName))
            {
                keyCombinations[keyName].Clear();
            }
            else
            {
                keyCombinations[keyName] = new List<Keys>();
            }
            keyCombinations[keyName].AddRange(keys);
        }
        public bool IsKeyPressed(string keyName)
        {
            if (!keyCombinations.ContainsKey(keyName))
            {
                return false;
            }
            return (keyCombinations[keyName].Take(keyCombinations[keyName].Count - 1).All(k => Keyboard.GetState().IsKeyDown(k)) && game.IsKeyPressed(keyCombinations[keyName].Last()));
        }
        public void Save()
        {
            var keys = GetKeys();
            foreach (var (key, value) in keys)
            {
                db[key] = string.Join("+", value.Select(k => k.ToString()));
            }
            db.Save(false);
        }
        public void Load()
        {
            db.Load(false);
            foreach (string name in db.Dict.Keys)
            {
                string[] keyStrings = db[name].Split('+');
                Keys[] keys = keyStrings.Select(ks => (Keys)Enum.Parse(typeof(Keys), ks)).ToArray();
                AddKeyCombination(name, keys);
            }
        }
        private Dictionary<string, List<Keys>> GetKeys()
        {
            return keyCombinations;
        }
    }
}
