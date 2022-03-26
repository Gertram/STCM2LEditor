using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STCM2L.classes
{
    public class ProxyData : IParameterData
    {
        public ProxyData(StringData original, StringData translate)
        {
            Original = original;
            Translate = translate;
        }
        public bool NoInsertTranslate { get; set; } = false;
        public int Address { get => !NoInsertTranslate&&GlobalStatus.Inserting &&Translate.Text!=""? Translate.Address : Original.Address; set => Original.Address = value; }

        public int Length => Original.Length;

        public int DataLength => Original.DataLength;

        public uint Value => Original.Value;

        public int AlignedLength => Original.AlignedLength;

        public uint Type => Original.Type;

        public IReadOnlyList<byte> ExtraData => Original.ExtraData;

        internal StringData Original { get; set; }
        internal StringData Translate { get; set; }

        public byte[] Write()
        {
            return Original.Write();
        }
    }
}
