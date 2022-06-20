using STCM2LEditor.classes.Actions;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using System.Windows.Media;
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
        internal void SetSelected(IAction action)
        {
            var ind = File.Actions.IndexOf(action);
            ActionsList.SelectedIndex = ind;
            FocusTextBlock(ActionsList, ind, "Text");
        }
        private void FocusTextBlock(ListView list, int ind, string name)
        {
            var visualItem = list.ItemContainerGenerator.
                ContainerFromItem(list.Items[ind]);
            var listViewItem = visualItem as ListViewItem;

            var myContentPresenter = FindVisualChild<ContentPresenter>(listViewItem);

            var myDataTemplate = myContentPresenter.ContentTemplate;
            var myTextBox = (TextBlock)myDataTemplate.FindName(name, myContentPresenter);
            myTextBox.Focus();
        }
        private childItem FindVisualChild<childItem>(DependencyObject obj)
    where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                {
                    return (childItem)child;
                }
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }
        private void DeleteActionClick(object sender, RoutedEventArgs e)
        {
            var action = ActionsList.SelectedItem as IAction;
            File.Actions.Remove(action);
            if (action.OpCode == ActionHelpers.ACTION_NAME && action is IStringAction nameAction)
            {
                foreach(var item in File.NameActions)
                {
                    if (item.Value.Contains(nameAction))
                    {
                        item.Value.Remove(nameAction);
                    }
                }
            }
            else if (action.OpCode == ActionHelpers.ACTION_TEXT && action is TextAction textAction)
            {
                File.TextActions.Remove(textAction);
            }
            else if (action.OpCode == ActionHelpers.ACTION_PLACE && action is IStringAction placeAction)
            {

                foreach (var item in File.PlaceActions)
                {
                    if (item.Value.Contains(placeAction))
                    {
                        item.Value.Remove(placeAction);
                    }
                }
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
        public IAction PreLoad { get; set; }
        private void Root_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                int ind = 0;
                if (PreLoad != null)
                {
                    ind = File.Actions.IndexOf(PreLoad);
                }
                else
                {
                    ind = File.Actions.IndexOf(File.Actions.First(x => x.OpCode == ActionHelpers.ACTION_TEXT || x.OpCode == ActionHelpers.ACTION_NAME || x.OpCode == ActionHelpers.ACTION_PLACE));
                }
                if (ind == -1) ind = 0; ;
                ScrollTo(ind);
                
            }
            catch
            {

            }
        }
    }
}
