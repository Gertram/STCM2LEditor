using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Diabolik_Lovers_STCM2L_Editor.utils;

namespace Diabolik_Lovers_STCM2L_Editor.classes
{
    internal class MutableAction : IAction
    {
        private int address;

        internal MutableAction(ImmutableAction action)
        {
            address = action.Address;
            OpCode = action.OpCode;
            IsLocalCall = action.IsLocalCall;
            Params = new List<Parameter>();
            foreach (var par in action.Parameters)
            {
                Params.Add(par as Parameter);
            }
        }
        public MutableAction(int address, uint opCode, uint isLocalCall)
        {
            this.address = address;
            OpCode = opCode;
            IsLocalCall = isLocalCall;
            Params = new List<Parameter>();
        }

        private IEnumerable<LocalParameter> LocalParameters => Parameters.Where(x => x is LocalParameter).Select(x => x as LocalParameter);
        public int Address
        {
            get => address;
            set
            {
                if(value != address && address != 0)
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
        public UInt32 OpCode { get; private set; }
        public UInt32 IsLocalCall { get; private set; }

        private const int HEADER_LENGTH = sizeof(uint) * 4;
        public int Length => HEADER_LENGTH+ Params.Sum(x => x.Length);
        public IReadOnlyList<IParameter> Parameters => Params;
        public List<Parameter> Params { get; set; }
        public byte[] Write()
        {
            var main = new List<byte>();
            var extra = new List<byte>();

            main.AddRange(BitConverter.GetBytes(IsLocalCall));
            main.AddRange(BitConverter.GetBytes(OpCode));
            main.AddRange(BitConverter.GetBytes(Parameters.Count));
            main.AddRange(BitConverter.GetBytes(Length));

            foreach (var parameter in Params)
            {
                parameter.Write(main, extra);
            }
            main.AddRange(extra);
            return main.ToArray();
        }
    }
}
