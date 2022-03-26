using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STCM2L.classes {
    static class Global {
        public static Dictionary<uint, List<GlobalParameter>> Calls { get; set; }

        static Global () {
            Calls = new Dictionary<uint, List<GlobalParameter>>();
        }
    }
}
