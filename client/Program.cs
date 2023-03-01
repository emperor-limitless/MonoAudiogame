using System;
using System.IO;
using System.Threading;

public static class Program
{
    public static Test.GameTest game;
    private static readonly Random getrandom = new();
    public static int randint(int min, int max)
    {
        lock (getrandom) // synchronize
        {
            return getrandom.Next(min, max + 1);
        }
    }
    public static void Main()
    {
        try
        {
            game = new();
            game.Run();
        }
        catch (Exception e)
        {
            int err = Messagebox.error_yes_no(new IntPtr(0), $@"A runtime error occurred.
                In function: {e.TargetSite}
                Description: {e.Message}
                Would you like to copy the stack trace text to the clipboard?", "C Sharp runtimeError");
            if (err == 6)
            {
                TextCopy.ClipboardService.SetText(e.ToString());
            }
            else
            {
                File.WriteAllText("errors.log", e.ToString());
            }
        }
    }
}
