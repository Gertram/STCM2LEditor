using System.Collections.Generic;

namespace STCM2LEditor.classes.Action.Parameters
{
    internal class GlobalParameter : BaseParameter
    {
        public IAction GlobalPointer { get; set; }
        public override uint Value2 { get => (uint)GlobalPointer.Address; set => GlobalPointer.Address = (int)value; }

        public GlobalParameter(Parameter param)
        {
            if (!Global.ParameterCalls.ContainsKey(param.Value2))
            {
                Global.ParameterCalls.Add(param.Value2, new List<GlobalParameter>());
            }
            Value1 = param.Value1;
            Value3 = param.Value3;
            Global.ParameterCalls[param.Value2].Add(this);
            GlobalPointer = null;
        }
    }
}
