namespace STCM2LEditor.classes.Actions.Parameters
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
        internal static LocalParameter ReadFromFile(byte[] file, Parameter parameter)
        {
            var data = ParameterData.ReadFromFile(file, (int)parameter.Value1);
            if(data != null)
            {
                return new LocalParameter(parameter, data);
            }
            return null;
        }
        public static LocalParameter ReadFromFile(byte[] file, ref int seek)
        {
            var par = Parameter.ReadFromFile(file, ref seek);
            if(par == null)
            {
                return null;
            }
            return LocalParameter.ReadFromFile(file, par);
        }
        internal LocalParameter(Parameter param, IParameterData data)
        {
            Data = data;
            Value2 = param.Value2;
            Value3 = param.Value3;
        }
    }
}
