using STCM2LEditor.classes.Action.Parameters;
using System;
using System.Globalization;
using System.Windows.Data;

namespace STCM2LEditor
{
    internal class ParamterExtraTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is StringParameter stringParameter)
            {
                return stringParameter.Data.Text;
            }
            if (value is LocalParameter localParameter)
            {
                try
                {
                    var str = StringData.TryCreateNew(localParameter.Data);
                    if (str == null)
                    {
                        return null;
                    }
                    return str.Text;
                }
                catch
                {

                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
