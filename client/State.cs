using Microsoft.Xna.Framework;
using System.Collections.Generic;
namespace State
{
    public class GameState
    {
        public List<GameState> Stack = new();
        public void Append(GameState state)
        {
            if (Stack.Count != 0)
            {
                Last().OnUnfocus();
            }
            state.OnEnter();
            Stack.Add(state);
        }
        public void Pop(int range = 1)
        {
            if (Stack.Count != 0)
            {
                Last().OnUnfocus();
                if (range == 1)
                {
                    Stack.Remove(Last());
                }
                else
                {
                    var val = ^1;
                    Stack.RemoveRange(val.Value, range);
                }
                if (Stack.Count != 0)
                {
                    Last().OnFocus();
                }
            }
        }
        public void Replace(GameState state)
        {
            if (Stack.Count != 0)
            {
                state.OnExit();
                Stack.Remove(state);
            }
            Append(state);
        }
        public void UpdateLast(GameTime gameTime)
        {
            if (Last() != null)
            {
                Last().OnUpdate(gameTime);
            }
        }
        public GameState Last()
        {
            if (Stack.Count == 0)
            {
                return null;
            }
            return Stack[^1];
        }
        public virtual void OnEnter()
        {
        }
        public virtual void OnExit()
        {
        }
        public virtual void OnFocus()
        {
        }
        public virtual void OnUnfocus()
        {
        }
        public virtual void OnUpdate(GameTime gameTime)
        {
            UpdateLast(gameTime);
        }
    }
}
