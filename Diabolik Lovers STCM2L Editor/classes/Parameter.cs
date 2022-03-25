using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Diabolik_Lovers_STCM2L_Editor.utils;

namespace Diabolik_Lovers_STCM2L_Editor.classes
{
    internal partial class Parameter:BaseParameter
    {
        internal Parameter():base() { }
        internal Parameter(Parameter param)
        {
            Value1 = param.Value1;
            Value2 = param.Value2;
            Value3 = param.Value3;
        }
        internal Parameter(byte[] file, ref int seek)
        {
            Value1 = ByteUtil.ReadUInt32Ref(file, ref seek);
            Value2 = ByteUtil.ReadUInt32Ref(file, ref seek);
            Value3 = ByteUtil.ReadUInt32Ref(file, ref seek);
        }
    }
}
