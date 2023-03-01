using Microsoft.Xna.Framework;
using State;
namespace Test
{
    public class MainGameState : GameState
    {
        public GameTest game;
        public MainGameState(GameTest g)
        {
            game = g;
        }
        public override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);
        }
    }
}
