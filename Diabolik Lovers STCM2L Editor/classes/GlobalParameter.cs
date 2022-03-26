using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using STCM2L.utils;

namespace STCM2L.classes
{
    internal class GlobalParameter : BaseParameter
    {
        public IAction GlobalPointer { get; set; }
        public override uint Value2 { get => (uint)GlobalPointer.Address; set => GlobalPointer.Address = (int)value; }

        public GlobalParameter (Parameter param)
        {
            if (!Global.Calls.ContainsKey(param.Value2))
            {
                Global.Calls.Add(param.Value2, new List<GlobalParameter>());
            }
            Value1 = param.Value1;
            Value3 = param.Value3;
            Global.Calls[param.Value2].Add(this);
            GlobalPointer = null;
        }
    }
}
