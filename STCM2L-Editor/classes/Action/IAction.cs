using STCM2LEditor.classes.Action.Parameters;
using System.Collections.Generic;

namespace STCM2LEditor.classes.Action
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
