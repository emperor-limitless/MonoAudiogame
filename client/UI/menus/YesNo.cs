using System;
namespace UI.Menus
{
    public class YesNo : Menu
    {
        public YesNo(Test.GameTest g, Action c, Action<MenuItem> y, Action<MenuItem> n) : base(g, c)
        {
            Open = "notifications/alert.mp3";
            Items.Add(new("Yes", y, true, this));
            Items.Add(new("No", n, true, this));
        }
    }
}
