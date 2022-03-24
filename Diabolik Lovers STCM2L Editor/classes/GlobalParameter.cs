using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diabolik_Lovers_STCM2L_Editor.classes
{
    internal class GlobalParameter:Parameter
    {
        public IAction GlobalPointer { get; set; }
        public GlobalParameter (Parameter param):base (param)
        {
            if (!Global.Calls.ContainsKey(Value2))
            {
                Global.Calls.Add(Value2, new List<GlobalParameter>());
            }

            Global.Calls[Value2].Add(this);
            GlobalPointer = null;
        }

        internal override void Write(List<byte> bytes, List<byte> extra)
        {
            bytes.AddRange(BitConverter.GetBytes(Value1)); 
            if (GlobalPointer == null)
            {
                throw new Exception("Lol");
            }
            bytes.AddRange(BitConverter.GetBytes(GlobalPointer.Address));
            bytes.AddRange(BitConverter.GetBytes(Value3));
        }
    }
}
