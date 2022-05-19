using STCM2LEditor.classes.Action;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Collections;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

using STCM2LEditor.classes.Action.Parameters;

namespace STCM2LEditor
{
    internal class TextBoxColorConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var text = values[0] as string;
            var maxlenth = (int)values[1];
            var def = values[2] as Brush;
            if(text.Length > maxlenth)
            {
                return Brushes.LightPink;
            }
            return def;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
