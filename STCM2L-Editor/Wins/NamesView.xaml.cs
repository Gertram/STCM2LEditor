using STCM2LEditor.classes.Actions;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace STCM2LEditor
{

    class Name
    {
        public string OriginalName => Actions.First().OriginalText;
        public string TranslatedName
        {
            get => Actions.First().TranslatedText;
            set
            {
                foreach (var action in Actions)
                {
                    action.TranslatedText = value;
                }
            }
        }
        public BindingList<IStringAction> Actions { get; set; } = new BindingList<IStringAction>();
    }
    /// <summary>
    /// Логика взаимодействия для NamesView.xaml
    /// </summary>

    public partial class NamesView : Window, Findable
    {
        public bool find(string text)
        {
            for (var i = 0; i < names.Count; i++)
            {
                var name = names[i];
                if (name.OriginalName.Contains(text) || name.TranslatedName.Contains(text))
                {
                    if (i + 10 < names.Count)
                        TextsList.ScrollIntoView(TextsList.Items[i + 10]);
                    else
                        TextsList.ScrollIntoView(name);
                    TextsList.SelectedIndex = i;
                    Focus();
                    return true;
                }
            }
            return false;
        }


        private readonly BindingList<Name> names = new BindingList<Name>();
        internal NamesView(classes.STCM2L file)
        {
            InitializeComponent();
            foreach (var name in file.NameActions)
            {
                names.Add(new Name { Actions = name.Value });
            }

            TextsList.DataContext = names;
            TextsList.ItemsSource = names;
            TextsList.SelectionChanged += TextsList_SelectionChanged;
        }

        private void SelectItem(int ind)
        {
            if (ind < 0) return;

            var translate = names[ind];
            NameBox1.DataContext = translate;
            NameBox2.DataContext = translate;
            if (names[ind].TranslatedName == "" && (bool)Autotranslate.IsChecked)
            {
                NameBox2.Text = Translator.TranslateText(names[ind].OriginalName);
                NameBox2.Text = NameBox2.Text.Substring(0, 1).ToUpper() + NameBox2.Text.Substring(1);
            }
            LinesList.ItemsSource = translate.Actions;
            LinesList.DataContext = translate.Actions;
        }
        private void TextsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectItem(TextsList.SelectedIndex);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FindCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var win = new FindDialog(this);
            win.Show();
        }

        private void NameBox2_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
        }
    }
}
