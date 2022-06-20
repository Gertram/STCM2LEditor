
using STCM2LEditor.utils;

namespace STCM2LEditor.classes.Actions.Parameters
{

    internal partial class Parameter : BaseParameter
    {
        internal Parameter() : base() { }
        internal Parameter(Parameter param)
        {
            Value1 = param.Value1;
            Value2 = param.Value2;
            Value3 = param.Value3;
        }
        internal Parameter(uint value1, uint value2, uint value3)
        {
            Value1 = value1;
            Value2 = value2;
            Value3 = value3;
        }
        internal static Parameter ReadFromFile(byte[] file, ref int seek)
        {
            if(seek + sizeof(uint)*3 >= file.Length)
            {
                return null;
            }
            return new Parameter
            {
                Value1 = ByteUtil.ReadUInt32Ref(file, ref seek),
                Value2 = ByteUtil.ReadUInt32Ref(file, ref seek),
                Value3 = ByteUtil.ReadUInt32Ref(file, ref seek),
            };
        }
    }
}
