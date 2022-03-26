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
using STCM2L.classes;

namespace STCM2L
{
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
                if(place.OriginalName.Contains(text)||place.TranslatedName.Contains(text))
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
            public string OriginalName { get => Translate.OriginalText; }
            public string TranslatedName { get => Translate.TranslatedText; set=>Translate.TranslatedText = value; }
            public TranslateData Translate { get; set; }
        }
        private BindingList<Place> places;
        internal PlaceWindow(classes.STCM2L file)
        {
            InitializeComponent();
            this.places = new BindingList<Place>();
            var places = new Dictionary<string,List<ActionProxy>>();
            foreach (var action in file.DefaultActions)
            {
                if(action.OpCode == ActionHelpers.ACTION_PLACE)
                {
                    var param = action.Parameters[3] as LocalParameter;
                    var data = new StringData(param.ParameterData);
                    param.ParameterData = data;
                    var text = data.Text;
                    
                    if (!places.TryGetValue(text, out List<ActionProxy> list))
                    {

                        list = new List<ActionProxy>();
                        places.Add(text, list);
                        //list.Add(new ActionOriginal { Action = action, Original = data });
                    }
                    if (file.Translates.All(x => x.TranslatedText != text))
                    {
                        //list.Add(new ActionOriginal { Action = action, Original = data });
                    }
                }
            }
            var size = file.Translates.Count;
            foreach (var place in places)
            {
                var found = false;
                for(var i = 0;i < size;i++)
                {
                    var translate = file.Translates[i];
                    if (translate.OriginalText == place.Key)
                    {
                        found = true;
                        this.places.Add(new Place { Translate = translate });
                        break;
                    }
                }
                if (!found)
                {
                    if(place.Value.Count == 0)
                    {
                        Console.WriteLine("WTF");
                    }
                    /*var translate = new TranslateData("", place.Value);
                    this.places.Add(new Place { Translate=translate});
                    file.Translates.Add(translate);*/
                }
            }
            
            TextsList.DataContext = this.places;
            TextsList.ItemsSource = this.places;
            TextsList.SelectionChanged += TextsList_SelectionChanged;
        }

        private void SelectItem(int ind)
        {
            if (ind < 0) return;

            var place = places[ind];
            NameBox1.DataContext = place;
            NameBox2.DataContext = place;
            if(places[ind].TranslatedName == "" && (bool)Autotranslate.IsChecked)
            {
                NameBox2.Text = ClassTranslator.TranslateText(places[ind].OriginalName);
                NameBox2.Text = NameBox2.Text.Substring(0, 1).ToUpper() + NameBox2.Text.Substring(1);
            }
            LinesList.ItemsSource = place.Translate.Actions;
            LinesList.DataContext = place.Translate.Actions;
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
