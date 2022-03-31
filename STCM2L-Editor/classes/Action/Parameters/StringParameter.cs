namespace STCM2LEditor.classes.Action.Parameters
{
    public class StringParameter : IParameter
    {
        private LocalParameter local;
        public StringParameter()
        {
            Data = new StringData();
        }
        public StringParameter(StringData data)
        {
            Data = data;
        }
        private StringParameter(LocalParameter param) : this(new StringData(param.Data))
        {
            local = param;
        }
        public StringParameter Copy()
        {
            return new StringParameter { Data=Data,local=local};
        }
        public static StringParameter ReadFromFile(byte[] file, ref int seek)
        {
            return new StringParameter(LocalParameter.ReadFromFile(file, ref seek));
        }
        public StringData Data { get; set; }
        public uint Value1 => (uint)Data.Address;
        public uint Value2 => local.Value2;
        public uint Value3 => local.Value3;

        public int Length => IParameterHelpers.HEADER_LENGTH + Data.Length;

        public void Write(byte[] buffer, ref int seek)
        {
            new Parameter(Value1, Value2, Value3).Write(buffer, ref seek);
        }
    }
}
