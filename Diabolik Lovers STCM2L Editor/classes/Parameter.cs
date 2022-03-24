using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Diabolik_Lovers_STCM2L_Editor.utils;

namespace Diabolik_Lovers_STCM2L_Editor.classes
{
    internal class Parameter:IParameter
    {
        public const int HEADER_LENGTH = sizeof(uint) * 3;
        public UInt32 Value1 { get; set; }
        public UInt32 Value2 { get; set; }
        public UInt32 Value3 { get; set; }
        public virtual int Length => HEADER_LENGTH;
        internal Parameter() 
        {
            Value1 = 0xff000000;
            Value2 = 0xff000000;
            Value3 = 0xff000000;
        }
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
        internal virtual void Write(List<byte> main,List<byte> extra)
        {
            main.AddRange(BitConverter.GetBytes(Value1));
            main.AddRange(BitConverter.GetBytes(Value2));
            main.AddRange(BitConverter.GetBytes(Value3));
        }
        public string TextValue
        {
            get
            {
                return $"{Value1:X} {Value2:X} {Value3:X}";
            }
        }
    }
}
