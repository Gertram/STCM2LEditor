using System.Collections.Generic;

namespace STCM2LEditor.classes.Actions.Parameters
{
    internal class GlobalParameter : BaseParameter
    {
        public IAction GlobalPointer { get; set; }
        public override uint Value2 { get => (uint)GlobalPointer.Address; set => GlobalPointer.Address = (int)value; }

        public GlobalParameter(Parameter param,Global global)
        {
            if (!global.ParameterCalls.ContainsKey(param.Value2))
            {
                global.ParameterCalls.Add(param.Value2, new List<GlobalParameter>());
            }
            Value1 = param.Value1;
            Value3 = param.Value3;
            global.ParameterCalls[param.Value2].Add(this);
            GlobalPointer = null;
        }
    }
}
