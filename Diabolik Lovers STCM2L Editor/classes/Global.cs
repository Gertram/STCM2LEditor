using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diabolik_Lovers_STCM2L_Editor.classes {
    static class Global {
        public static Dictionary<uint, List<GlobalParameter>> Calls { get; set; }

        static Global () {
            Calls = new Dictionary<uint, List<GlobalParameter>>();
        }
    }
}
