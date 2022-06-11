
using STCM2LEditor.utils;

namespace STCM2LEditor.classes.Action.Parameters
{
    internal abstract class BaseParameter : IParameter
    {
        public virtual uint Value1 { get; set; } = 0xff000000;
        public virtual uint Value2 { get; set; } = 0xff000000;
        public virtual uint Value3 { get; set; } = 0xff000000;
        public virtual int Length => IParameterHelpers.HEADER_LENGTH;

        public void Write(byte[] main, ref int seek)
        {
            ByteUtil.InsertUint32Ref(main, Value1, ref seek);
            ByteUtil.InsertUint32Ref(main, Value2, ref seek);
            ByteUtil.InsertUint32Ref(main, Value3, ref seek);
        }
    }
}
