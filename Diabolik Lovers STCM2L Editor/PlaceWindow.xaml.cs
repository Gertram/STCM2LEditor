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

namespace Diabolik_Lovers_STCM2L_Editor
{
    public class Place
    {
        public string Name { get; set; }
        internal List<classes.Action> Actions { get; set; } = new List<classes.Action>();

    }
    /// <summary>
    /// Логика взаимодействия для PlaceWindow.xaml
    /// </summary>
    public partial class PlaceWindow : Window,Findable
    {
        public bool find(string text)
        {
            for (var i = 0; i < places.Count; i++)
            {
                var place = places[i];
                if(place.Name.Contains(text))
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
        private BindingList<Place> places;
        internal PlaceWindow(classes.STCM2L file)
        {
            InitializeComponent();
            places= new BindingList<Place>();
            foreach(var action in file.Actions)
            {
                if(action.OpCode == classes.Action.ACTION_PLACE)
                {
                    var found = false;
                    foreach(var place in places)
                    {
                        var name = action.GetStringFromParameter(3);
                        if (place.Name == name)
                        {
                            place.Actions.Add(action);
                            found = true;
                        }
                    }
                    if (!found)
                    {
                        var place = new Place { Name = action.GetStringFromParameter(3) };
                        place.Actions.Add(action);
                        places.Add(place);
                    }
                }
            }
            TextsList.DataContext = places;
            TextsList.ItemsSource = places;
            TextsList.SelectionChanged += TextsList_SelectionChanged;
        }

        private void SelectItem(int ind)
        {
            if (ind < 0) return;
            NameBox.DataContext = places[ind];
        }
        private void TextsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectItem(TextsList.SelectedIndex);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            classes.Action first = null;
            foreach(var place in places)
            {
                foreach(var action in place.Actions)
                {
                    
                }
            }

        }

        private void FindCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var win = new FindDialog(this);
            win.Show();
        }

        internal static classes.Action action1 = null;
    }
}
