using System;
using System.Collections.Generic;

using System.Windows.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using STCM2L.classes;

namespace STCM2L
{
    internal class ParamterExtraTextConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return value;
            if (!(value is classes.LocalParameter))
            {
                return null;
            }
            var proxy = (value as classes.LocalParameter).ParameterData as ProxyData;
            var text = proxy.Original.Text;
            if(proxy.Translate.Text != "")
            {
                text += $" ({proxy.Translate.Text})";
            }

            return text;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
