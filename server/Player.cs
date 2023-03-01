using System;
using System.Numerics;
namespace Entity
{
    public class Player
    {
        public Vector3 Position;
        public string Name;
        public Player(string Name, Vector3 position)
        {
            this.Name = Name;
            this.Position = position;
        }
    }
}
