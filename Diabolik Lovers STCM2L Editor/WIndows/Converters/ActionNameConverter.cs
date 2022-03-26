using System;
using System.Collections.Generic;

using System.Windows.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

using STCM2L.classes;

namespace STCM2L
{
    internal class ActionNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return value;
            var action = (classes.DefaultAction)value;
            ProxyData data;
            switch (action.OpCode)
            {
                case classes.ActionHelpers.ACTION_PLACE:
                    data = ((action.Parameters[3] as LocalParameter).ParameterData as ProxyData);
                    if(data.Translate.Text != "")
                        return $"Place {data.Translate.Text}";
                    else
                        return $"Place {data.Original.Text}";
                case classes.ActionHelpers.ACTION_NAME:
                    data = ((action.Parameters[0] as LocalParameter).ParameterData as ProxyData);
                    if (data.Translate.Text != "")
                        return $"Name {data.Translate.Text}";
                    else
                        return $"Name {data.Original.Text}";
                case classes.ActionHelpers.ACTION_TEXT:
                    data = ((action.Parameters[0] as LocalParameter).ParameterData as ProxyData);
                    if (data.Translate.Text != "")
                        return $"Text {data.Translate.Text}";
                    else
                        return $"Text {data.Original.Text}";
                case classes.ActionHelpers.ACTION_DIVIDER:
                    return "Divider";
                case classes.ActionHelpers.ACTION_SHOW_PLACE:
                    return "Show Place";
                case classes.ActionHelpers.ACTION_NEW_PAGE:
                    return "Show New Page";
                case classes.ActionHelpers.ACTION_CHOICE:
                    return "Choice";
                default:
                    return $"Unknown ({action.OpCode})";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
