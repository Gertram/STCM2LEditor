using STCM2LEditor.utils;
using System.Linq;
namespace STCM2LEditor.classes.Action.Parameters
{
    public class StringData
    {
        private StringData() { }
        public StringData(string text = "")
        {
            Text = text;
        }
        public StringData(IParameterData data)
        {
            Address = data.Address;
            Text = EncodingUtil.encoding.GetString(data.ExtraData.ToArray()).TrimEnd(new char[] { '\0' });
        }
        public static StringData TryCreateNew(IParameterData data)
        {
            if(data.Address <= 0)
            {
                return null;
            }
            return new StringData(data);
        }
        public static StringData ReadFromFile(byte[] file, int seek) => new StringData(new ParameterData(file, seek));
        public int Address { get; set; } = 0;
        public string Text { get; set; }
        public byte[] EncodedBytes => EncodingUtil.encoding.GetBytes(Text);
        private int AlignedLength => EncodedBytes.Length + 4 - EncodedBytes.Length % 4;
        public int Length => ParameterDataHelpers.HEADER_LENGTH + AlignedLength;

        public byte[] Write()
        {
            return new ParameterData(0, 1, ByteUtil.InsertBytes(new byte[AlignedLength], EncodedBytes, 0)).Write();
        }

    }
}
