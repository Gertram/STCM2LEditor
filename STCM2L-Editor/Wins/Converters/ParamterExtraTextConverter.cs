using STCM2LEditor.classes.Actions.Parameters;
using System;
using System.Globalization;
using System.Windows.Data;

using STCM2LEditor.utils;

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
                    var str = StringData.TryCreateNew(localParameter.Data,EncodingUtil.Current);
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
            if(value is Parameter val)
            {
                var str = StringData.TryCreateNew(new ParameterData(0,0, BitConverter.GetBytes(val.Value1)), EncodingUtil.Current);
                if (str == null)
                {
                    return null;
                }
                return str.Text;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
