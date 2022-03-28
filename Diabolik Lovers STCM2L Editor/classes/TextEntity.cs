using System.ComponentModel;

namespace STCM2LEditor.classes
{
    public class TextEntity
    {
        public class MyString
        {
            public string Text { get; set; }
        }
        public TextEntity()
        {
        }

        public TextEntity(string name, BindingList<MyString> lines)
        {
            Name = name;
            Lines = lines;
        }
        public void AddLine(string text = "", int index = -1)
        {
            if (index == -1)
            {
                Lines.Add(new MyString { Text = text });
                return;
            }
            Lines.Insert(index, new MyString { Text = text });
        }
        public void DeleteLine(int index = -1)
        {
            if (index == -1)
            {
                Lines.RemoveAt(Lines.Count - 1);
                return;
            }
            Lines.RemoveAt(index);
        }

        public string Name { get; set; } = "";
        public BindingList<MyString> Lines { get; set; } = new BindingList<MyString>();
    }
}
