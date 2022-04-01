namespace STCM2LEditor.classes.Action.Parameters
{
    internal partial class LocalParameter : BaseParameter,ILocalParameter
    {
        public IParameterData Data { get; set; }
        public int DataSeek { get; set; } = 0;

        public override int Length => base.Length + Data.Length;
        public override uint Value1 { get => (uint)Data.Address; set => Data.Address = (int)value; }

        internal LocalParameter(IParameterData data)
        {
            Data = data;
        }
        internal LocalParameter(byte[] file, Parameter parameter) : this(parameter, new ParameterData(file, (int)parameter.Value1))
        {

        }
        public static LocalParameter ReadFromFile(byte[] file, ref int seek)
        {
            return new LocalParameter(file, new Parameter(file, ref seek));
        }
        internal LocalParameter(Parameter param, IParameterData data)
        {
            Data = data;
            Value2 = param.Value2;
            Value3 = param.Value3;
        }
    }
}
