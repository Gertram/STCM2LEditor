using STCM2LEditor.classes;
using System;
using System.Globalization;
using System.Windows.Data;

namespace STCM2LEditor
{
    internal class TranslateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IString param)
            {
                var text = param.OriginalText;
                if (param.TranslatedText != "")
                {
                    text += $" ({param.TranslatedText})";
                }

                return text;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
