namespace STCM2LEditor.classes.Actions.Parameters
{
    public class StringParameter : IParameter,ILocalParameter
    {
        private LocalParameter local;
        public StringParameter(IGameEncoding encoding) : this(new StringData(encoding)) { }
        public StringParameter(StringData data)
        {
            if(data.Address == 0)
            {
                data.ExtraData = new byte[0];
            }
            Data = data;
        }
        internal StringParameter(LocalParameter param,IGameEncoding encoding) : this(new StringData(param.Data,encoding))
        {
            local = param;
        }
        public StringParameter Copy()
        {
            return new StringParameter(Data=Data){local=local};
        }
        public static StringParameter ReadFromFile(byte[] file, ref int seek,IGameEncoding encoding)
        {
            var param = LocalParameter.ReadFromFile(file, ref seek);
            if(param == null)
            {
                return null;
            }
            return new StringParameter(param,encoding);
        }
        public StringData Data { get; set; }
        public uint Value1 => (uint)Data.Address;
        public uint Value2 => local.Value2;
        public uint Value3 => local.Value3;

        public int Length => IParameterHelpers.HEADER_LENGTH + Data.Length;

        IParameterData ILocalParameter.Data { get => Data; }

        public void Write(byte[] buffer, ref int seek)
        {
            new Parameter(Value1, Value2, Value3).Write(buffer, ref seek);
        }
    }
}
