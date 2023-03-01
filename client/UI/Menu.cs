using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using System.Collections.Generic;
using State;
namespace UI
{
    public class MenuItem : GameState
    {
        public string Text;
        public Action<MenuItem> Callback;
        public bool Clickable;
        public int Index = -1;
        public List<MenuItem> Items = new();
        public Menu Parent;
        public MenuItem(string t, Action<MenuItem> c, bool cl, Menu p)
        {
            Text = t;
            Callback = c;
            Clickable = cl;
            Parent = p;
        }
        public virtual void Announce(bool one = false)
        {
            string res = Text;
            if (Items.Count > 0 && !one)
            {
                IEnumerable<string> names = Items.Select(i => i.Text);
                res += $";{string.Join(";", names)}";
            }
            Parent.game.speak(res, true);
            Parent.game.s.Play(Parent.Move);
        }
        public virtual void OnWrap()
        {
            Parent.game.s.Play(Parent.Wrap);
        }
        public virtual void OnClick()
        {
            Parent.game.s.Play(Parent.Enter);
            if (Index == -1)
            {
                Callback(this);
            }
            else if (Index > 0)
            {
                Items[Index].Callback(Items[Index]);
            }
        }
        public override void OnFocus()
        {
            Announce();
        }
        public override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
        }
    }
    public class Menu : GameState
    {
        public int Index { get; private set; }
        public List<MenuItem> Items = new();
        public Audio.Sound MusicHandle;
        public Action Callback;
        public string Title, Open, Move = "menus/main/move.mp3", Enter = "menus/main/enter.mp3", Escape, Wrap = "menus/main/wrap.mp3", Music;
        public Test.GameTest game;
        public Menu(Test.GameTest g, Action c)
        {
            game = g;
            Callback = c;
        }
        public override void OnEnter()
        {
            game.s.Play(Open);
            if (Music != "" && Music != null)
            {
                MusicHandle = game.s.Play(Music, true);
            }
            game.speak(Title, true);
        }
        public override void OnUnfocus()
        {
            if (MusicHandle != null)
            {
                MusicHandle.Pause();
                MusicHandle.PlaybackPosition = 0;
            }
        }
        public override void OnFocus()
        {
            if (MusicHandle != null)
            {
                MusicHandle.Play();
            }
        }
        public override void OnUpdate(GameTime gameTime)
        {
            if (Items.Count == 0)
            {
                return;
            }
            Items[Index].OnUpdate(gameTime);
            if (game.InputConfig.IsKeyPressed("MenuUp"))
            {
                if (Index > 0)
                {
                    Items[Index].OnUnfocus();
                    Index--;
                    Items[Index].OnFocus();
                }
                else if (Index == 0)
                {
                    Items[Index].OnUnfocus();
                    Index = Items.Count - 1;
                    Items[Index].OnFocus();
                    Items[Index].OnWrap();
                }
            }
            else if (game.InputConfig.IsKeyPressed("MenuDown"))
            {
                if (Index < Items.Count - 1)
                {
                    Items[Index].OnUnfocus();
                    Index++;
                    Items[Index].OnFocus();
                }
                else if (Index == Items.Count - 1)
                {
                    Items[Index].OnUnfocus();
                    Index = 0;
                    Items[Index].OnFocus();
                    Items[Index].OnWrap();
                }
            }
            else if (game.InputConfig.IsKeyPressed("MenuSelect"))
            {
                Items[Index].OnClick();
            }
            else if (game.InputConfig.IsKeyPressed("MenuClose"))
            {
                if (Callback != null)
                {
                    Callback();
                }
            }
            if (Items[Index].Items.Count > 0)
            {
                MenuItem i = Items[Index];
                if (i.Index >= 0)
                {
                    i.Items[i.Index].OnUpdate(gameTime);
                }
                if (game.InputConfig.IsKeyPressed("MenuLeft"))
                {
                    if (i.Index > 0)
                    {
                        i.Items[i.Index].OnUnfocus();
                        i.Index--;
                        i.Items[i.Index].OnFocus();
                    }
                    else if (i.Index >= -1 && i.Index <= 0)
                    {
                        if (i.Index >= 0)
                        {
                            i.Items[i.Index].OnUnfocus();
                        }
                        i.Index = -1;
                        i.Announce(true);
                    }
                }
                else if (game.InputConfig.IsKeyPressed("MenuRight"))
                {
                    if (i.Index < i.Items.Count - 1)
                    {
                        if (i.Index >= 0)
                        {
                            i.Items[i.Index].OnUnfocus();
                        }
                        i.Index++;
                        i.Items[i.Index].OnFocus();
                    }
                    else if (i.Index == i.Items.Count - 1)
                    {
                        i.Items[i.Index].OnFocus();
                    }
                }
            }
        }
    }
}
