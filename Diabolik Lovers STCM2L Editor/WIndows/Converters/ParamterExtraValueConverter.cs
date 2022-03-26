using System;
using System.Collections.Generic;

using System.Windows.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace STCM2L
{
    internal class ParamterExtraValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return value;
            if (!(value is classes.LocalParameter))
            {
                return null;
            }
            var par = value as classes.LocalParameter;
            return string.Join("", par.ParameterData.ExtraData.Select(x => $"{x:X}"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
