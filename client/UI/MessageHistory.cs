using System;
using System.Collections.Generic;
using DavyKager;
namespace UI
{
    public class History
    {
        public List<HistoryBuffer> Items = new();
        int Index = 0;
        public void Add(string title)
        {
            HistoryBuffer item = new(title);
            Items.Add(item);
        }
        public void AddItem(string bufferName, string text, uint id = 0, bool speak = true)
        {
            if (Items.Count >= 1)
            {
                Items[0].Add(text, id, false);
            }
            bool found = false;
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].Title == bufferName)
                {
                    found = true;
                    Items[i].Add(text, id, speak);
                }
            }
            if (!found)
            {
                Add(bufferName);
                Items[^1].Add(text, id, speak);
            }
        }
        public void Cycle(int direction)
        {
            if (Items.Count == 0)
            {
                return;
            }
            if (direction == 1)
            {
                if (Index < Items.Count - 1)
                {
                    Index++;
                    Speak();
                }
            }
            else if (direction == 0)
            {
                if (Index > 0)
                {
                    Index--;
                    Speak();
                }
            }
            else if (direction == 2)
            {
                Index = 0;
                Speak();
            }
            else if (direction == 3)
            {
                Index = Items.Count - 1;
                Speak();
            }
        }
        public void CycleItem(int direction)
        {
            Items[Index].Cycle(direction);
        }
        public void Speak()
        {
            string text = $"{Items[Index].Title}: {Items[Index].Items.Count} items, {Index + 1} of {Items.Count}";
            Tolk.Speak(text, true);
        }
    }
    public class HistoryBuffer
    {
        public List<HistoryItem> Items = new();
        public string Title;
        public int Index = 0;
        public HistoryBuffer(string t)
        {
            Title = t;
        }
        public void Cycle(int direction)
        {
            if (Items.Count == 0)
            {
                return;
            }
            if (direction == 1)
            {
                if (Index < Items.Count - 1)
                {
                    Index++;
                    Tolk.Speak(Items[Index].Text, true);
                }
            }
            else if (direction == 0)
            {
                if (Index > 0)
                {
                    Index--;
                    Tolk.Speak(Items[Index].Text, true);
                }
            }
            else if (direction == 2)
            {
                Index = 0;
                Tolk.Speak(Items[Index].Text, true);
            }
            else if (direction == 3)
            {
                Index = Items.Count - 1;
                Tolk.Speak(Items[Index].Text, true);
            }
        }
        public void Add(string title, uint id = 0, bool speak = true)
        {
            if (speak)
            {
                Tolk.Speak(title, true);
            }
            HistoryItem item = new(title, id);
            Items.Add(item);
        }
    }
    public class HistoryItem
    {
        public string Text;
        public readonly uint ID = 0;
        public DateTime Time = DateTime.Now;
        public HistoryItem(string t, uint i)
        {
            Text = t;
            ID = i;
        }
    }
}
