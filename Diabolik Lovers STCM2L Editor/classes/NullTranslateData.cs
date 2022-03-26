using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using STCM2L.classes;

namespace STCM2L
{
    internal class NullTranslateData:TranslateData
    {
        public override string OriginalText => "";
        public override string TranslatedText { get => ""; set { } }

        public override void DeleteFrom(classes.STCM2L file)
        {

        }
    }
}
