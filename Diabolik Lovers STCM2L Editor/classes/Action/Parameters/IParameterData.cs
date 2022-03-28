using System.Collections.Generic;

namespace STCM2LEditor.classes.Action.Parameters
{
    public interface IParameterData
    {
        int Address { get; set; }
        int Length { get; }
        int DataLength { get; }
        uint Value { get; }
        int AlignedLength { get; }
        uint Type { get; }
        IReadOnlyList<byte> ExtraData { get; }

        byte[] Write();
    }

}
