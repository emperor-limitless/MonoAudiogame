namespace Map
{
    public class Tile
    {
        public int minx, maxx, miny, maxy, minz, maxz;
        public string type;
        public Tile(int minx, int maxx, int miny, int maxy, int minz, int maxz, string type)
        {
            this.minx = minx;
            this.maxx = maxx;
            this.miny = miny;
            this.maxy = maxy;
            this.minz = minz;
            this.maxz = maxz;
            this.type = type;
        }
    }
}
