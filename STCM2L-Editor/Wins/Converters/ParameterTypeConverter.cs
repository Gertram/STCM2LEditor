using STCM2LEditor.classes.Action.Parameters;
using System;
using System.Globalization;
using System.Windows.Data;

namespace STCM2LEditor
{
    internal class ParameterTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return value;
            if (value is LocalParameter || value is StringParameter)
            {
                return "Локальный";
            }
            if (value is GlobalParameter)
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
