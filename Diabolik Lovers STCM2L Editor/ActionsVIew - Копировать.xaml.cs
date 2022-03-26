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

using STCM2L.classes;

namespace STCM2L
{
    /// <summary>
    /// Логика взаимодействия для ActionsVIew.xaml
    /// </summary>
    public partial class ActionsView : Window
    {
        private classes.STCM2L file;
        internal ActionsView(classes.STCM2L file)
        {
            InitializeComponent();
            this.file = file;
            this.file = file;
            ActionsList.DataContext = file.Actions;
            ActionsList.ItemsSource = file.Actions;
            var ind = file.Actions.IndexOf(file.Actions.First(x => x.OpCode == ActionHelpers.ACTION_TEXT || x.OpCode == ActionHelpers.ACTION_NAME));
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
        private void DeleteTextClick(object sender, RoutedEventArgs e)
        {
            foreach(classes.DefaultAction item in ActionsList.SelectedItems)
            {
                file.Actions.Remove(item);
            }
        }
        private void SelectItem(object sender)
        {
            if(sender == null)
            {
                
                ParamsList.DataContext = null;
                ParamsList.ItemsSource = null;
                Address.DataContext = null;
                ParamCount.DataContext = null;
                ExtraData.Text = "";
                return;
            }
            var item = sender as classes.DefaultAction;
            ParamsList.DataContext = item.Parameters;
            ParamsList.ItemsSource = item.Parameters;
            Address.DataContext = item;
            ParamCount.DataContext = item;

            if (item.ExtraData != null)

                ExtraData.Text = utils.EncodingUtil.encoding.GetString(item.ExtraData).TrimEnd(new char[] { '\0' });
            else
                ExtraData.Text = "";
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
