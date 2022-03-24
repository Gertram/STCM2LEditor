using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Diabolik_Lovers_STCM2L_Editor.utils;

namespace Diabolik_Lovers_STCM2L_Editor.classes
{
    internal partial class LocalParameter : Parameter
    {
        public IParameterData ParameterData { get; set; }

        public override int Length => base.Length + ParameterData.Length;

        internal LocalParameter(IParameterData data) : base()
        {
            ParameterData = data;
        }
        internal LocalParameter(Parameter param, IParameterData data) : base(param)
        {
            ParameterData = data;
        }
        internal override void Write(List<byte> bytes,List<byte> extra)
        {
            bytes.AddRange(BitConverter.GetBytes(ParameterData.Address));
            bytes.AddRange(BitConverter.GetBytes(Value2));
            bytes.AddRange(BitConverter.GetBytes(Value3));
            ParameterData.Write(extra);
        }
    }
}
