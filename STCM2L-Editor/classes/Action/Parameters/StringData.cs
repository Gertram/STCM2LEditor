using STCM2LEditor.utils;
using System.Linq;
namespace STCM2LEditor.classes.Action.Parameters
{
    public class StringData
    {
        public uint Type { get; set; }
        public uint Value { get; set; }
        public int Address { get; set; }
        public StringData()
        {
            Type = 0;
            Value = 0;
            ExtraData = new byte[0];
        }
        public StringData Copy()
        {
            return new StringData();
        }
        public StringData(uint type, uint value, byte[] extraData)
        {
            Type = type;
            Value = value;
            ExtraData = extraData;
        }
        public StringData(IParameterData data)
        {
            Type = data.Type;
            Value = data.Value;
            Address = data.Address;
            ExtraData = data.ExtraData;
        }
        public static StringData TryCreateNew(IParameterData data)
        {
            if (data.Address <= 0)
            {
                return null;
            }
            return new StringData(data);
        }
        public static StringData ReadFromFile(byte[] file, int seek) => new StringData(new ParameterData(file, seek));
        public string Text
        {
            get => EncodingUtil.encoding.GetString(ExtraData.ToArray()).TrimEnd(new char[] { '\0' });
            set
            {
                ExtraData = EncodingUtil.encoding.GetBytes(value + "\0");
            }
        }
        public byte[] ExtraData { get; set; }
        private int AlignedLength => ExtraData.Length % 4 == 0?ExtraData.Length:ExtraData.Length+4-ExtraData.Length % 4;
        public int Length => ParameterDataHelpers.HEADER_LENGTH + AlignedLength;

        public byte[] Write()
        {
            return (new ParameterData(Type, Value, ByteUtil.InsertBytes(new byte[AlignedLength],ExtraData,0))).Write();
        }

    }
}
