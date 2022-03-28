namespace STCM2LEditor.classes.Action.Parameters
{

    public interface IParameter
    {
        uint Value1 { get; }
        uint Value2 { get; }
        uint Value3 { get; }
        int Length { get; }
        void Write(byte[] buffer, ref int seek);
    }
}