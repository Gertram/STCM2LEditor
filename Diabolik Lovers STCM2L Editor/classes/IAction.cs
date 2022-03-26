using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STCM2L.classes
{
    public interface IAction
    {
        uint OpCode { get; }
        uint IsLocalCall { get; }
        int Address { get; set; }
        int Length { get; }
        List<IParameter> Parameters { get; }
        byte[] Write();
    }
}
