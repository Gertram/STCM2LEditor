using System;
using System.Collections.Generic;

using System.Windows.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace Diabolik_Lovers_STCM2L_Editor
{
    internal class ParameterTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return value;
            var param = value as classes.IParameter;
            return $"{param.Value1:X} {param.Value2:X} {param.Value3:X}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
