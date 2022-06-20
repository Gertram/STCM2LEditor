using STCM2LEditor.classes.Actions.Parameters;
using System;
using System.Globalization;
using System.Windows.Data;
namespace STCM2LEditor
{
    internal class ParameterLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return value;
            var param = value as IParameter;
            return $"{param.Length:X}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
