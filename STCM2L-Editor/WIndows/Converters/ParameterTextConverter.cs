using STCM2LEditor.classes.Action.Parameters;
using System;
using System.Globalization;
using System.Windows.Data;
namespace STCM2LEditor
{
    internal class ParameterTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return value;
            var param = value as IParameter;
            return $"{param.Value1:X} {param.Value2:X} {param.Value3:X}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
