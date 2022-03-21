using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diabolik_Lovers_STCM2L_Editor.classes
{
    internal partial class ActionPlace : Action
    {
        public string Background { get; set; }
        public string OriginalName { get; set; }
        public string TranslateName { get; set; }

        private Parameter param1;
        private Parameter param2;
        
        internal ActionPlace(string Background,string Name,Parameter param1,Parameter param2)
        {
            this.Background = Background;
            this.param1 = param1;
            this.param2 = param2;
        }
        protected override string GetName()
        {
            return "Place ";
        }
    }
}
