using STCM2LEditor.classes.Actions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Collections;
using System.Linq;
using System.Windows.Data;

using STCM2LEditor.classes.Actions.Parameters;

namespace STCM2LEditor
{
    internal class ActionLocalCall : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is IAction action)
            {
                return $"{ action.Address:X}";
            }
            return "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
