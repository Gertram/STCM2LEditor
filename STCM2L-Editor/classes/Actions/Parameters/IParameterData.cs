using System.Collections.Generic;

namespace STCM2LEditor.classes.Actions.Parameters
{
    public interface IParameterData
    {
        int Address { get; set; }
        int Length { get; }
        int DataLength { get; }
        uint Value { get; }
        int AlignedLength { get; }
        uint Type { get; }
        byte[] ExtraData { get; set; }
        IParameterData Copy();

        byte[] Write();
    }

}
