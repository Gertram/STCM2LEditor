using STCM2LEditor.classes;
using STCM2LEditor.classes.Actions;
using System.ComponentModel;
using System.Linq;

namespace STCM2LEditor
{
    public class Replic : BasePropertyChanged
    {
        public IStringAction Name { get; set; } = null;
        public BindingList<TextAction> Lines { get; set; } = new BindingList<TextAction>();
        public Replic() { }
        public Replic(IStringAction name, BindingList<TextAction> lines)
        {
            Name = name;
            Lines = lines;
        }


        internal void AddLine(STCM2L file, int index = -1)
        {
            TextAction action;
            if (Lines.Count == 0 && Name is IStringAction nameAction)
            {
                /*action = file.NewText(nameAction, false);
                Lines.Add(action);*/
                throw new System.Exception();
                return;
            }
            if (Lines.Count == 0) return;
            if (index == -1 || index >= Lines.Count)
            {
                index = Lines.Count;
                action = file.NewText(Lines.Last(), false);
            }
            else
            {
                action = file.NewText(Lines[index], true);
            }
            action.TranslatedText = "";
            action.OriginalText = "";
            Lines.Insert(index, action);
        }
        internal void InsertTranslates()
        {
            var isTranslated = Lines.Any(x => x.Translated != null && x.TranslatedText.Trim().Length > 0);
            foreach (var line in Lines)
            {
                line.IsTranslated = isTranslated;
            }
        }
        internal void DeleteLine(classes.STCM2L file, int index = -1)
        {
            if(Lines.Count == 1)
            {
                return;
            }
            if (index == -1)
            {
                index = Lines.Count - 1;
            }
            file.DeleteText(Lines[index]);
            Lines.RemoveAt(index);
        }
    }
}
