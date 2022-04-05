using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace STCM2LEditor.classes
{
    public class TextEntity
    {
        public class MyString:INotifyPropertyChanged
        {
            private string text;
            private string translationOption;

            public string Text
            {
                get => text;
                set
                {
                    text = value.TrimStart();
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs(nameof(text)));
                    }
                }
            }
            public string TranslationOption
            {
                get => translationOption; set
                {
                    translationOption = value.TrimStart();
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs(nameof(TranslationOption)));
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
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
            Lines.Insert(index,new MyString { Text = text } );
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
