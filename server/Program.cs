namespace Server
{
    public static class Program
    {
        public static void Main()
        {
            GameServer server = new(18832);
            server.Run();
        }
    }
}
