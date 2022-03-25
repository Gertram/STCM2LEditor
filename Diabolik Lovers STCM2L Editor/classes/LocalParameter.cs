using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Diabolik_Lovers_STCM2L_Editor.utils;

namespace Diabolik_Lovers_STCM2L_Editor.classes
{
    internal partial class LocalParameter : BaseParameter
    {
        public IParameterData ParameterData { get; set; }
        public int DataSeek { get; set; } = 0;

        public override int Length => base.Length + ParameterData.Length;
        public override uint Value1 { get => (uint)ParameterData.Address; set => ParameterData.Address = (int)value; }

        internal LocalParameter(IParameterData data)
        {
            ParameterData = data;
        }
        internal LocalParameter(Parameter param, IParameterData data)
        {
            ParameterData = data;
            Value2 = param.Value2;
            Value3 = param.Value3;
        }
    }
}
