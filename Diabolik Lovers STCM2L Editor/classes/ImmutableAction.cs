using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;
using System.Text;
using System.Threading.Tasks;

using Diabolik_Lovers_STCM2L_Editor.utils;

namespace Diabolik_Lovers_STCM2L_Editor.classes
{
    class ImmutableAction : IAction
    {
        private const int HEADER_LENGTH = sizeof(uint) * 4;
        private int ParametersHeaderLength => Parameter.HEADER_LENGTH * Parameters.Count;
        public int Length => HEADER_LENGTH + ParametersHeaderLength + ExtraDataLength;
        public int Address
        {
            get => address;
            set
            {
                if (value != address && address != 0)
                {
                    throw new Exception();
                }
                var offset = value - address;
                foreach (var parameter in LocalParameters)
                {
                    parameter.ParameterData.Address -= offset;
                }
                this.address = value;
            }
        }
        private IEnumerable<LocalParameter> LocalParameters => parameters.Where(x => x is LocalParameter).Select(x => x as LocalParameter);
        public UInt32 OpCode { get; private set; } = 0;
        public UInt32 IsLocalCall { get; private set; } = 0;
        public IReadOnlyList<byte> ExtraData { get; private set; }
        private int ExtraDataLength => ExtraData == null ? 0 : ExtraData.Count;
        public IReadOnlyList<IParameter> Parameters => parameters;
        private readonly List<Parameter> parameters = new List<Parameter>();
        private int address = 0;
        public void ReadFromFile(int address, byte[] file)
        {
            this.address = address;

            int seek = (int)address;

            IsLocalCall = ByteUtil.ReadUInt32Ref(file, ref seek);
            OpCode = ByteUtil.ReadUInt32Ref(file, ref seek);
            var parametersCount = ByteUtil.ReadInt32Ref(file, ref seek);
            var length = ByteUtil.ReadInt32Ref(file, ref seek);
            var extraDataLength = length - 16 - parametersCount * 12;
            if (extraDataLength > 0)
            {
                ExtraData = ByteUtil.ReadBytes(file, extraDataLength, seek + parametersCount * Parameter.HEADER_LENGTH);
            }
            for (int i = 0; i < parametersCount; i++)
            {
                parameters.Add(ParseParameter(file, ref seek,parametersCount));
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
        private Parameter ParseParameter(byte[] file, ref int seek,int parametersCount)
        {
            var param = new Parameter(file, ref seek);

            if (param.Value1 == 0xffffff41)
            {
                return new GlobalParameter(param);
            }
            else if (IsLocalParam(param.Value1))
            {
                var link = new LinkParameterData((int)param.Value1 - Address - HEADER_LENGTH - parametersCount * Parameter.HEADER_LENGTH, ExtraData);
                link.Address = (int)param.Value1;
                try
                {
                    //Console.WriteLine(link.ExtraData);
                }catch(Exception e)
                {
                    //Console.WriteLine(e.Message);
                }
                return new LocalParameter(param, link);
            }
            return param;
        }

        public byte[] Write()
        {
            var main = new List<byte>();
            var extra = new List<byte>();

            main.AddRange(BitConverter.GetBytes(IsLocalCall));
            main.AddRange(BitConverter.GetBytes(OpCode));
            main.AddRange(BitConverter.GetBytes(Parameters.Count));
            main.AddRange(BitConverter.GetBytes(Length));

            foreach (var parameter in parameters)
            {
                parameter.Write(main, extra);
            }

            if (ExtraData != null)
            {
                main.AddRange(ExtraData);
            }

            return main.ToArray();
        }
    }
}
