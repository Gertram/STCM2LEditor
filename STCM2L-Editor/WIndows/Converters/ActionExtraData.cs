using STCM2LEditor.classes.Action;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Collections;
using System.Linq;
using System.Windows.Data;

using STCM2LEditor.classes.Action.Parameters;

namespace STCM2LEditor
{
    internal class ActionExtraData : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return value;
            if(value is DefaultAction action)
            {
                return $"{string.Join("", action.BuildExtraData().Select(x => $"{x:X}"))}";
            }
            return $"{string.Join("",(value as IAction).ExtraData.Select(x => $"{x:X}"))}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
