using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STCM2L.classes
{
    public interface IParameterData
    {
        int Address { get; set; }
        int Length { get; }
        int DataLength { get; }
        uint Value { get; }
        int AlignedLength { get; }
        uint Type { get; }
        IReadOnlyList<byte> ExtraData { get; }

        byte[] Write();
    }
    
}
