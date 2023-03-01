using System.Collections.Generic;
namespace Map
{
    public class Map
    {
        private List<Tile> tiles = new();
        int minx, maxx, miny, maxy, minz, maxz;
        public Map(int minx, int maxx, int miny, int maxy, int minz, int maxz)
        {
            this.minx = minx;
            this.maxx = maxx;
            this.miny = miny;
            this.maxy = maxy;
            this.minz = minz;
            this.maxz = maxz;
        }
        public Tile GetTile(int x, int y, int z)
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                if (x >= tiles[i].minx && x <= tiles[i].maxx && y >= tiles[i].miny && y <= tiles[i].maxy && z >= tiles[i].minz && z <= tiles[i].maxz)
                {
                    return tiles[i];
                }
            }
            return null;
        }
        public void AddTile(int minx, int maxx, int miny, int maxy, int minz, int maxz, string type)
        {
            Tile tile = new(minx, maxx, miny, maxy, minz, maxz, type);
            tiles.Add(tile);
        }
    }
}
