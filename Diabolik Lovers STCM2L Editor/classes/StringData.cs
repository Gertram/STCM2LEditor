using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Diabolik_Lovers_STCM2L_Editor.utils;
namespace Diabolik_Lovers_STCM2L_Editor.classes
{
    public class StringData : ParameterData
    {
        public string Text
        {
            get => EncodingUtil.encoding.GetString(ExtraData.ToArray()).TrimEnd(new char[] { '\0' });
            set => ExtraData = EncodingUtil.encoding.GetBytes(value);
        }
        public override int AlignedLength => base.AlignedLength + (4 - ExtraData.Count % 4);
        internal StringData(string text) : base(0, 1, EncodingUtil.encoding.GetBytes(text)) {
            
        }
        internal StringData(IParameterData data):base(data.Type,data.Value,data.ExtraData.TakeWhile(x => x!= 0).ToArray())
        {
            Address = data.Address;

            if(data.Length != Length)
            {
                throw new Exception();
            }
        }
        internal StringData(byte[] file, ref int seek) : base(file, ref seek) 
        {
            ExtraData = ExtraData.TakeWhile(x => x != 0).ToArray();
        }
    }
}
