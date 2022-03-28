using STCM2LEditor.classes.Action;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace STCM2LEditor
{
    /// <summary>
    /// Логика взаимодействия для PlaceWindow.xaml
    /// </summary>
    public partial class PlaceWindow : Window, Findable
    {
        public bool find(string text)
        {
            for (var i = 0; i < places.Count; i++)
            {
                var place = places[i];
                if (place.OriginalName.Contains(text) || place.TranslatedName.Contains(text))
                {
                    if (i + 10 < places.Count)
                        TextsList.ScrollIntoView(TextsList.Items[i + 10]);
                    else
                        TextsList.ScrollIntoView(place);
                    TextsList.SelectedIndex = i;
                    Focus();
                    return true;
                }
            }
            return false;
        }
        class Place
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
        private readonly BindingList<Place> places = new BindingList<Place>();
        internal PlaceWindow(classes.STCM2L file)
        {
            InitializeComponent();

            foreach (var place in file.PlaceActions)
            {
                places.Add(new Place { Actions = place.Value });
            }

            TextsList.DataContext = places;
            TextsList.ItemsSource = places;
            TextsList.SelectionChanged += TextsList_SelectionChanged;
        }

        private void SelectItem(int ind)
        {
            if (ind < 0) return;

            var place = places[ind];
            NameBox1.DataContext = place;
            NameBox2.DataContext = place;
            if (place.TranslatedName == "" && (bool)Autotranslate.IsChecked)
            {
                NameBox2.Text = ClassTranslator.TranslateText(place.OriginalName);
                NameBox2.Text = NameBox2.Text.Substring(0, 1).ToUpper() + NameBox2.Text.Substring(1);
            }
            LinesList.ItemsSource = place.Actions;
            LinesList.DataContext = place.Actions;
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
    }
}
