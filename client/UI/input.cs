using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using State;
namespace UI
{
    public class Input : GameState
    {
        public Test.GameTest game;
        public List<string> Characters = new();
        public int Index = 0;
        public string Title;
        public Action<string> Callback;
        public Input(Test.GameTest g, string title, Action<string> callback = null)
        {
            game = g;
            Title = title;
            Callback = callback;
        }
        public void Speak(int index)
        {
            string spoken = "";
            if (!(index < Characters.Count))
            {
                spoken = "blank";
            }
            else if (Characters[index] == " ")
            {
                spoken = "space";
            }
            else
            {
                spoken = Characters[index];
            }
            game.speak(spoken, true);
        }
        public override void OnEnter()
        {
            game.speak(Title, true);
        }
        public override void OnUpdate(GameTime gameTime)
        {
            UpdateLast(gameTime);
            if (game.IsKeyPressed(Keys.Back))
            {
                if (Index > 0 && Index <= Characters.Count)
                {
                    Speak(Index - 1);
                    Characters.RemoveAt(Index - 1);
                    Index -= 1;
                }
            }
            else if (game.IsKeyPressed(Keys.Left))
            {
                if (Index == 0)
                {
                    Speak(0);
                }
                else if (Index > 0)
                {
                    Index -= 1;
                    Speak(Index);
                }
            }
            else if (game.IsKeyPressed(Keys.Right))
            {
                if (Index == Characters.Count)
                {
                    Speak(Index);
                }
                else if (Index < Characters.Count)
                {
                    Index += 1;
                    Speak(Index);
                }
            }
            else if (game.IsKeyPressed(Keys.Up) || game.IsKeyPressed(Keys.Down))
            {
                game.speak(string.Join("", Characters));
            }
            else if (game.IsKeyPressed(Keys.Escape))
            {
                if (Callback != null)
                {
                    Callback("");
                }
            }
            else if (game.IsKeyPressed(Keys.Enter))
            {
                if (Callback != null)
                {
                    Callback(string.Join("", Characters));
                }
                else
                {
                    game.state.Stack.Remove(this);
                }
            }
            else if (game.InputText != "")
            {
                game.speak(game.InputText, true);
                Characters.Insert(Index, game.InputText);
                Index += 1;
            }
        }
    }
}
