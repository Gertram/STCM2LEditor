using STCM2LEditor.classes;
using STCM2LEditor.classes.Action;
using System;
using System.Globalization;
using System.Windows.Data;

namespace STCM2LEditor
{
    internal class ActionNameConverter : IValueConverter
    {
        private string GetName(IAction action)
        {
            IString data;
            if (action.OpCode == ActionHelpers.ACTION_PLACE)
            {
                data = action as IString;
                if (data.TranslatedText != "")
                    return $"Place {data.TranslatedText}";
                else
                    return $"Place {data.OriginalText}";
            }
            else if (action.OpCode == ActionHelpers.ACTION_NAME)
            {
                data = action as IString;
                if (data.TranslatedText != "")
                    return $"Name {data.TranslatedText}";
                else
                    return $"Name {data.OriginalText}";
            }
            else if (action.OpCode == ActionHelpers.ACTION_TEXT)
            {
                data = action as IString;
                if (data.TranslatedText != "")
                    return $"Text {data.TranslatedText}";
                else
                    return $"Text {data.OriginalText}";
            }
            else if (action.OpCode == ActionHelpers.ACTION_DIVIDER)
            {
                return "Divider";
            }
            else if (action.OpCode == ActionHelpers.ACTION_SHOW_PLACE)
            {
                return "Show Place";
            }
            else if (action.OpCode == ActionHelpers.ACTION_NEW_PAGE)
            {
                return "Show New Page";
            }
            else if (action.OpCode == ActionHelpers.ACTION_CHOICE)
            {
                return "Choice";
            }
            return $"Unknown ({action.OpCode:X})";
            
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is IAction action)
            {
                var text = GetName(action);
                if (action.LocalCall != null)
                {
                    text += $" LC";
                }
                return text;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
