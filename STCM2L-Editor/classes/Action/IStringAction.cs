using System.ComponentModel;

namespace STCM2LEditor.classes.Action
{
    public interface IStringAction : IAction, IString,INotifyPropertyChanged
    {
        void SetTranslateAddress(ref int address);
        byte[] WriteTranslate();

        IStringAction Copy();
        bool IsTranslated { get; }
        void ReverseStrings();
    }
}
