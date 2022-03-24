using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diabolik_Lovers_STCM2L_Editor.classes
{
    internal class NullTranslateData:TranslateData
    {
        public override string OriginalText => "";
        public override string TranslatedText { get => ""; set { } }
    }
}
