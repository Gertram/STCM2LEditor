using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Diabolik_Lovers_STCM2L_Editor.classes;

namespace Diabolik_Lovers_STCM2L_Editor
{
    /// <summary>
    /// Логика взаимодействия для ActionsVIew.xaml
    /// </summary>
    public partial class ActionsView : Window
    {
        private STCM2L file;
        internal ActionsView(STCM2L file)
        {
            InitializeComponent();
            this.file = file;
            this.file = file;
            ActionsList.DataContext = file.Actions;
            ActionsList.ItemsSource = file.Actions;
            var ind = file.Actions.FindIndex(x => x.OpCode == ActionHelpers.ACTION_TEXT || x.OpCode == ActionHelpers.ACTION_NAME);
            ActionsList.SelectionChanged += ParamsList_SelectionChanged;
            if (ind == -1) ind = 0; ;
            var item = ActionsList.Items[ind];
            
            var count = 10;
            if(ind + count >= ActionsList.Items.Count)
            {
                ActionsList.ScrollIntoView(item);
                ActionsList.SelectedItem = item;
                return;
            }
            var topItem = ActionsList.Items[ind + count];
            ActionsList.ScrollIntoView(topItem);
            ActionsList.SelectedItem = item;
        }
        public class ParamValue
        {
            private int idx;
            private classes.ImmutableAction action;

            internal ParamValue(int idx,classes.ImmutableAction action)
            {
                this.idx = idx;
                this.action = action;
            }
            public string Type
            {
                get
                {
                    var par = action.Parameters[idx];
                    if(par is GlobalParameter)
                    {
                        return "Глобальный";
                    }
                    if(par is LocalParameter)
                    {
                        return "Локальный";
                    }
                    return "Значение";

                }
            }
            public string ExtraDataText
            {
                get
                {
                    try
                    {
                        return "";
                        //return action.GetStringFromParameter(idx);
                    }
                    catch
                    {
                        return "";
                    }
                }
            }
            public static string ByteArrayToString(byte[] ba)
            {
                StringBuilder hex = new StringBuilder(ba.Length * 2);
                foreach (byte b in ba)
                    hex.AppendFormat("{0:x2}", b);
                return hex.ToString();
            }
            public string ExtraDataValue
            {
                get
                {
                    try
                    {
                        return "";
                        //return ByteArrayToString(action.GetValueFromParameter(idx));
                    }
                    catch
                    {
                        return "";
                    }
                }
            }
        }
        private void DeleteTextClick(object sender, RoutedEventArgs e)
        {
            foreach(classes.ImmutableAction item in ActionsList.SelectedItems)
            {
                file.Actions.Remove(item);
            }
        }
        private void SelectItem(object sender)
        {
            var item = sender as classes.ImmutableAction;
            var @params = new List<ParamValue>();
            for (var i = 0; i < item.Parameters.Count; i++)
            {
                @params.Add(new ParamValue(i, item));
            }
            ParamsList.DataContext = @params;
            ParamsList.ItemsSource = @params;
            Address.DataContext = item;
            ParamCount.DataContext = item;

           /* if (item.ExtraDataLength > 0)

                ExtraData.Text = utils.EncodingUtil.encoding.GetString(item.ExtraData).TrimEnd(new char[] { '\0' });
            else
                ExtraData.Text = "";*/
        }
        private void TextsListItemClick(object sender, MouseButtonEventArgs e)
        {
            SelectItem(ActionsList.SelectedItem);
        }

        private void ParamsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectItem(ActionsList.SelectedItem);
        }
    }
}
