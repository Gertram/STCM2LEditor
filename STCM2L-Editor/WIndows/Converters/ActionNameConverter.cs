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
            try
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
                    var text = $"Name ({action.OpCode:X})";
                    if (action is IStringAction stringAction)
                    {
                        if (stringAction.TranslatedText != "")
                            text += $" {stringAction.TranslatedText}";
                        else
                            text += $" {stringAction.OriginalText}";
                    }
                    return text;
                }
                else if (action.OpCode == ActionHelpers.ACTION_TEXT)
                {
                    var text = $"Text ({action.OpCode:X})";
                    if (action is IStringAction strinAction)
                    {
                        if (strinAction.TranslatedText != "")
                            text += $" {strinAction.TranslatedText}";
                        else
                            text+=$" {strinAction.OriginalText}";
                    }
                    return text;
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
            }
            catch
            {
                
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
