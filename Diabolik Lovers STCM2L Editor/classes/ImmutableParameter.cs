using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Diabolik_Lovers_STCM2L_Editor.classes
{
    internal class ImmutableParameter : IParameter
    {
        private readonly uint value1;
        private readonly uint value2;
        private readonly uint value3;

        public ImmutableParameter(uint value1, uint value2, uint value3)
        {
            this.value1 = value1;
            this.value2 = value2;
            this.value3 = value3;
        }

        uint IParameter.Value1 => value1;

        uint IParameter.Value2 => value2;

        uint IParameter.Value3 => value3;
    }
}
