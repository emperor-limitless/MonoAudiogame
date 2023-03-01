using System;
using System.Runtime.InteropServices;
// We call user32.dll because I'm planning to make this work crossplatform later, Lets hope.
public static class Messagebox {
    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int MessageBox(IntPtr hWnd, String text, String caption, long type);
    public static int error_yes_no(IntPtr hwnd, string text, string cap) {
        return MessageBox(new IntPtr(0), text, cap, 0x00000004L | 0x00000010L);
    }
}