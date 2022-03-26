using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STCM2L.classes
{
    public interface IParameter
    {
        uint Value1 { get; set; }
        uint Value2 { get; set; }
        uint Value3 { get; set; }
        int Length { get; }
        void Write(byte[] buffer,ref int seek);
    }
}
