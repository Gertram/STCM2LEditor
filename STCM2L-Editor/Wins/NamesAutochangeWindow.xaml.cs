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
using System.ComponentModel;

namespace STCM2LEditor.Wins
{
    /// <summary>
    /// Логика взаимодействия для NamesAutochangeWindow.xaml
    /// </summary>
    public class NameItem 
    {
        public string Name { get; set; } = "";
        public string Suffix { get; set; } = "";
        public bool IsEnabled { get; set; } = false;
    }

    public partial class NamesAutochangeWindow : Window, INotifyPropertyChanged
    {
        private BindingList<NameItem> names;

        public NamesAutochangeWindow()
        {
            InitializeComponent();
        }
        public BindingList<NameItem> Names
        {
            get => names; 
            internal set
            {
                names = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Names)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if(Names == null)
            {
                return;
            }
            foreach (var name in Names)
            {
                if(name.Name.Length == 0)
                {
                    MessageBox.Show("Found empty name");
                    e.Cancel = true;
                    return;
                }
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if(sender is TextBox text)
            {
                text.Text = text.Text.Trim();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Names.Add(new NameItem());
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var ind = NamesList.SelectedIndex;
            if(ind == -1)
            {
                return;
            }
            Names.RemoveAt(ind);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(sender is ComboBox combo && combo.DataContext is NameItem name && combo.SelectedItem is ComboBoxItem item)
            {
                name.Suffix = item.Content as string;
            }
        }
    }
}
