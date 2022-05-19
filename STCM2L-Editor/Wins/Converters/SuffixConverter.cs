using STCM2LEditor.classes.Action;
using System;
using System.Globalization;
using System.Windows.Data;
using STCM2LEditor.Wins;
namespace STCM2LEditor
{
    internal class SuffixConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is string name && parameter is string key)
            {
                return name == key;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }
    }
}
