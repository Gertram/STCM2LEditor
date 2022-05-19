using System.ComponentModel;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace STCM2LEditor.classes
{
    public class TextEntity : BasePropertyChanged
    {
        private string name = "";
        private BindingList<MyString> lines = new BindingList<MyString>();

        public class MyString : BasePropertyChanged
        {
            private string text;
            private string translationOption;

            public string Text
            {
                get => text;
                set
                {
                    text = value.TrimStart();
                    Notify(nameof(Text));
                }
            }
            public string TranslationOption
            {
                get => translationOption; set
                {
                    translationOption = value.TrimStart();
                    Notify(nameof(TranslationOption));
                }
            }
        }
        public TextEntity(IEnumerable<string> texts)
        {
            if (texts != null)
            {
                lines = new BindingList<MyString>(texts.Select(x => new MyString { Text = x }).ToList());
            }
            lines.AddingNew += Lines_AddingNew;
            lines.ListChanged += Lines_ListChanged;
        }
        public TextEntity() : this(null)
        {
        }

        private void Lines_ListChanged(object sender, ListChangedEventArgs e)
        {
            Notify(nameof(Lines));
        }

        private void Lines_AddingNew(object sender, AddingNewEventArgs e)
        {
            Notify(nameof(Lines));
        }

        public void AddLine(string text = "", int index = -1)
        {
            if (index == -1)
            {
                Lines.Add(new MyString { Text = text });
                return;
            }
            Lines.Insert(index, new MyString { Text = text });
            Notify(nameof(Lines));
        }
        public void AddRange(IEnumerable<string> strings)
        {
            foreach(var str in strings)
            {
                AddLine(str);
            }
        }
        public void DeleteLine(int index = -1)
        {
            if(Lines.Count == 1)
            {
                return;
            }
            if (index == -1)
            {
                Lines.RemoveAt(Lines.Count - 1);
                return;
            }
            Lines.RemoveAt(index);
            Notify(nameof(Lines));
        }

        public string Name
        {
            get => name;
            set
            {
                name = value;
                Notify(nameof(Name));
            }
        }
        public BindingList<MyString> Lines
        {
            get => lines; 
            private set
            {
                lines = value;
            }
        }
    }
}
