using System;
using System.Collections.Generic;

using System.Windows.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Diabolik_Lovers_STCM2L_Editor.classes;

namespace Diabolik_Lovers_STCM2L_Editor
{
    internal class TranslateConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return value;
            if (!(value is TranslateData))
            {
                return null;
            }
            var translate = value as TranslateData;
            var text = translate.OriginalText;
            if (translate.TranslatedText != "")
            {
                text += $" ({translate.TranslatedText})";
            }

            return text;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
