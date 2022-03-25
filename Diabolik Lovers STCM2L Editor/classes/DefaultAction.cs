using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;
using System.Text;
using System.Threading.Tasks;

using Diabolik_Lovers_STCM2L_Editor.utils;

namespace Diabolik_Lovers_STCM2L_Editor.classes
{
    public class DefaultAction : IAction
    {
        private const int HEADER_LENGTH = sizeof(uint) * 4;
        private int ParametersLength => Parameters.Sum(x => x.Length);
        private int MainLength => HEADER_LENGTH + ParameterHeaderLength;
        private int ParameterHeaderLength => IParameterHelpers.HEADER_LENGTH * Parameters.Count;
        public int Length => MainLength + ExtraDataLength;
        private int ExtraDataLength => ExtraData == null ? LocalParameters.Sum(x => x.ParameterData.Length) : ExtraData.Length;
        public int Address
        {
            get => address;
            set
            {
                var offset = value - address;
                foreach (var parameter in LocalParameters)
                {
                    parameter.ParameterData.Address = MainLength + value + parameter.DataSeek;
                }
                this.address = value;
            }
        }
       private IEnumerable<LocalParameter> LocalParameters => Parameters.Where(x => x is LocalParameter).Select(x => x as LocalParameter);
        public uint OpCode { get; private set; } = 0;
        public uint IsLocalCall { get; private set; } = 0;
        public List<IParameter> Parameters { get; private set; } = new List<IParameter>();
        public byte[] ExtraData { get; private set; }
        private int address = 0;
        public DefaultAction() { }
        public DefaultAction(uint opCode, uint isLocalCall, int paramCount)
        {
            OpCode = opCode;
            IsLocalCall = isLocalCall;

            for (int i = 0; i < paramCount; i++)
            {
                Parameters.Add(new Parameter());
            }
        }
        public void ReadFromFile(int address, byte[] file,IParameterData data = null)
        {
            this.address = address;

            int seek = address;
            IsLocalCall = ByteUtil.ReadUInt32Ref(file, ref seek);
            OpCode = ByteUtil.ReadUInt32Ref(file, ref seek);
            var parametersCount = ByteUtil.ReadInt32Ref(file, ref seek);
            Parameters = new List<IParameter>();
            var length = ByteUtil.ReadInt32Ref(file, ref seek);
            if (data == null)
            {
                for (int i = 0; i < parametersCount; i++)
                {
                    Parameters.Add(ParseParameter(file, ref seek, parametersCount,null));
                }
            }
            else
            {
                for (int i = 0; i < parametersCount; i++)
                {
                    if (i == 0 && OpCode == ActionHelpers.ACTION_TEXT || OpCode == ActionHelpers.ACTION_NAME)
                    {
                        Parameters.Add(ParseParameter(file, ref seek, parametersCount, data));
                    }
                    else if (i == 3 && OpCode == ActionHelpers.ACTION_PLACE)
                    {
                        Parameters.Add(ParseParameter(file, ref seek, parametersCount, data));
                    }
                    else
                    {
                        Parameters.Add(ParseParameter(file, ref seek, parametersCount, null));
                    }
                }
            }
            var extraDataLength = length - HEADER_LENGTH - ParametersLength;
            if (extraDataLength > 0)
            {
                extraDataLength = length - MainLength;
                ExtraData = ByteUtil.ReadBytes(file, extraDataLength, Address + MainLength);
            }
            if (length != Length)
            {
                throw new Exception("Pizda");
            }
        }
        private bool IsLocalParam(uint val)
        {
            return (((val >> 24) & 0xff) != 0xff) && (val > Address) /*&& (val < OldAddress + Length)*/;
        }
        private IParameter ParseParameter(byte[] file, ref int seek, int parametersCount,IParameterData exp)
        {
            var param = new Parameter(file, ref seek);

            if (param.Value1 == 0xffffff41)
            {
                return new GlobalParameter(param);
            }
            else if (IsLocalParam(param.Value1))
            {
                int dataAddress = (int)param.Value1;
                IParameterData data;
                if (exp != null)
                {
                    data = exp;
                }
                else
                {
                    data = new ParameterData(file, ref dataAddress)
                    {
                        Address = (int)param.Value1
                    };
                }
                var parameter = new LocalParameter(param, data)
                {
                    DataSeek = data.Address - Address - HEADER_LENGTH - IParameterHelpers.HEADER_LENGTH * parametersCount
                };
                return parameter;
            }
            return param;
        }
        private byte[] BuildExtraData()
        {
            if (ExtraData != null)
            {
                foreach (var parameter in LocalParameters)
                {
                    var data = parameter.ParameterData.Write();
                    try
                    {
                        ByteUtil.InsertBytes(ExtraData, data, parameter.DataSeek);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
                return ExtraData;
            }
                var extra = new List<byte>();
                foreach (var parameter in LocalParameters)
                {
                    extra.AddRange(parameter.ParameterData.Write());
                }
                return extra.ToArray();
            
        }
        public byte[] Write()
        {
            var main = new byte[Length];
            var position = 0;
            ByteUtil.InsertUint32Ref(main,IsLocalCall,ref position);
            ByteUtil.InsertUint32Ref(main,OpCode,ref position);
            ByteUtil.InsertInt32Ref(main,Parameters.Count,ref position);
            ByteUtil.InsertInt32Ref(main,Length,ref position);

            foreach (var parameter in Parameters)
            {
                parameter.Write(main, ref position);
            }

            return ByteUtil.InsertBytes(main, BuildExtraData(), position);
        }
    }
}
