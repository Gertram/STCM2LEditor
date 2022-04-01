using STCM2LEditor.classes.Action;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using System.ComponentModel;
using STCM2LEditor.classes;

namespace STCM2LEditor
{
    /// <summary>
    /// Логика взаимодействия для ActionsVIew.xaml
    /// </summary>
    public partial class ActionsView : Window
    {
        public STCM2L File { get; set; }
        internal ActionsView(STCM2L file)
        {

            InitializeComponent();
            File = file;
            ActionsContainer.DataContext = File;
            
        }
        private void DeleteActionClick(object sender, RoutedEventArgs e)
        {
            foreach (IAction item in ActionsList.SelectedItems)
            {
                File.Actions.Remove(item);
            }
        }
        private void SelectItem(object sender)
        {
            if (sender == null)
            {
                ActionData.DataContext = null;
                return;
            }
           
            ActionData.DataContext = sender as IAction;
            ActionsList.ScrollIntoView(sender);
            /*if (item.ExtraData != null)

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

        private void GotoCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var address = int.Parse(ActionAddress.Text, System.Globalization.NumberStyles.HexNumber);
            try
            {
                var ind = File.Actions.IndexOf(File.Actions.First(x => x.Address == address));
                ScrollTo(ind);
            }
            catch
            {
                
            }
        }
        private void ScrollTo(int ind)
        {
            var item = ActionsList.Items[ind];
            var count = -10;
            if (ind + count >= ActionsList.Items.Count)
            {
                ActionsList.ScrollIntoView(item);
                ActionsList.SelectedItem = item;
                return;
            }
            var topItem = ActionsList.Items[ind + count];
            ActionsList.SelectedItem = item;
            ActionsList.ScrollIntoView(topItem);
        }
        private void Root_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var ind = File.Actions.IndexOf(File.Actions.First(x => x.OpCode == ActionHelpers.ACTION_TEXT || x.OpCode == ActionHelpers.ACTION_NAME || x.OpCode == ActionHelpers.ACTION_PLACE));

                if (ind == -1) ind = 0; ;
                ScrollTo(ind);
                
            }
            catch
            {

            }
        }
    }
}
