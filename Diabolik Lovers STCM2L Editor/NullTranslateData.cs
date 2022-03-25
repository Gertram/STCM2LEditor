using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Diabolik_Lovers_STCM2L_Editor.classes;

namespace Diabolik_Lovers_STCM2L_Editor
{
    internal class NullTranslateData:TranslateData
    {
        public override string OriginalText => "";
        public override string TranslatedText { get => ""; set { } }

        public override void DeleteFrom(STCM2L file)
        {

        }
    }
}
