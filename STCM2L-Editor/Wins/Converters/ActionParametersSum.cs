using STCM2LEditor.classes.Actions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

using STCM2LEditor.classes.Actions.Parameters;

namespace STCM2LEditor
{
    internal class ActionParametersSum : IValueConverter
    {
        private int Sum(IReadOnlyList<IParameter> list)
        {
            int i = 0;
            foreach(var item in list)
            {
                i += item.Length;
            }
            return i;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return value;
            return $"{Sum(value as IReadOnlyList<IParameter>):X}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
