using System;
using System.Collections.Generic;

using System.Windows.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;


namespace Diabolik_Lovers_STCM2L_Editor 
{
    internal class ParameterTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return value;
            if (value is classes.LocalParameter)
            {
                return "Локальный";
            }
            if (value is classes.GlobalParameter)
            {
                return "Глобальный";
            }
            return "Значение";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
