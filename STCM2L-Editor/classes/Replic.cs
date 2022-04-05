using STCM2LEditor.classes;
using STCM2LEditor.classes.Action;
using System.ComponentModel;
using System.Linq;

namespace STCM2LEditor
{
    public class Replic:BasePropertyChanged
    {
        public IStringAction Name { get; set; } = null;
        public BindingList<IStringAction> Lines { get; set; } = new BindingList<IStringAction>();
        internal void AddLine(STCM2L file, int index = -1)
        {
            IStringAction action;
            if (Lines.Count == 0 && Name is IStringAction nameAction) {
                action = file.NewText(nameAction, false);
                Lines.Add(action);
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

            Lines.Insert(index, action);
        }
        internal void DeleteLine(classes.STCM2L file, int index = -1)
        {
            if (index == -1)
            {
                index = Lines.Count - 1;
            }
            file.DeleteText(Lines[index]);
            Lines.RemoveAt(index);
        }
    }
}
