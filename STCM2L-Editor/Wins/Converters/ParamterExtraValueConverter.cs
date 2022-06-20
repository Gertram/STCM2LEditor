using STCM2LEditor.classes.Actions.Parameters;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace STCM2LEditor
{
    internal class ParamterExtraValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return value;
            if (!(value is LocalParameter))
            {
                return null;
            }
            var par = value as LocalParameter;
            return string.Join("", par.Data.ExtraData.Select(x => $"{x:X}"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
