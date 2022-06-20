using STCM2LEditor.utils;
using System.Linq;
namespace STCM2LEditor.classes.Actions.Parameters
{
    public class StringData:IParameterData
    {
        public uint Type { get; set; }
        public uint Value { get; set; }
        public int Address { get; set; }
        private IGameEncoding Encoding { get; set; }
        public StringData(IGameEncoding encoding):this(0,0,new byte[0],encoding){}
        public StringData Copy()
        {
            return new StringData(Type,Value,ExtraData,Encoding,Address);
        }
        public StringData(uint type, uint value, byte[] extraData,IGameEncoding encoding,int address = 0)
        {
            Type = type;
            Value = value;
            ExtraData = extraData;
            Address = address;
            Encoding = encoding;
        }
        public StringData(IParameterData data,IGameEncoding encoding):this(data.Type,data.Value,data.ExtraData,encoding,data.Address){}
        public static StringData TryCreateNew(IParameterData data,IGameEncoding encoding)
        {
            if (data.Address <= 0)
            {
                return null;
            }
            return new StringData(data,encoding);
        }
        public static StringData ReadFromFile(byte[] file, int seek,IGameEncoding encoding)
        {
            var data = ParameterData.ReadFromFile(file, seek);
            if(data == null)
            {
                return null;
            }
            return new StringData(data,encoding);
        }
        public string Text
        {
            get => Encoding.Encoding.GetString(ExtraData.ToArray()).TrimEnd(new char[] { '\0' });
            set
            {
                ExtraData = Encoding.Encoding.GetBytes(value + "\0");
            }
        }
        public byte[] ExtraData { get; set; }
        private int AlignedLength => ExtraData.Length % 4 == 0?ExtraData.Length:ExtraData.Length+4-ExtraData.Length % 4;
        public int Length => ParameterDataHelpers.HEADER_LENGTH + AlignedLength;

        public int DataLength => ExtraData.Length;

        int IParameterData.AlignedLength => AlignedLength;

        public byte[] Write()
        {
            return (new ParameterData(Type, Value, ByteUtil.InsertBytes(new byte[AlignedLength],ExtraData,0))).Write();
        }

        IParameterData IParameterData.Copy()
        {
            return Copy();
        }
    }
}
