namespace STCM2LEditor.classes.Action
{
    public interface IStringAction : IAction, IString
    {
        void SetTranslateAddress(ref int address);
        byte[] WriteTranslate();
    }
}
