using STCM2LEditor.classes.Actions.Parameters;
using System.Collections.Generic;

namespace STCM2LEditor.classes.Actions
{
    public interface IAction
    {
        uint OpCode { get; }
        IAction LocalCall { get; set; }
        int Address { get; set; }
        int Length { get; }
        IReadOnlyList<IParameter> Parameters { get; }
        byte[] ExtraData { get; }
        byte[] Write();
    }
}
