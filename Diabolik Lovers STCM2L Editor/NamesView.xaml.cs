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
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Diabolik_Lovers_STCM2L_Editor.classes;

namespace Diabolik_Lovers_STCM2L_Editor
{
    class Name
    {
        public string OriginalName { get => Translate.OriginalText; }
        public string TranslatedName { get => Translate.TranslatedText; set => Translate.TranslatedText = value; }
        public TranslateData Translate { get; set; }
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
                var place = names[i];
                if (place.OriginalName.Contains(text) || place.TranslatedName.Contains(text))
                {
                    if (i + 10 < names.Count)
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
        
        private BindingList<Name> names;
        internal NamesView(STCM2L file)
        {
            InitializeComponent();
            this.names = new BindingList<Name>();
            var names = new Dictionary<string, List<ActionOriginal>>();
            foreach (var action in file.MutableActions)
            {
                if (action.OpCode == ActionHelpers.ACTION_NAME)
                {
                    var param = action.Parameters[0] as LocalParameter;
                    var data = new StringData(param.ParameterData);
                    param.ParameterData = data;
                    var text = data.Text;

                    if (!names.TryGetValue(text, out List<ActionOriginal> list))
                    {

                        list = new List<ActionOriginal>();
                        names.Add(text, list);
                        list.Add(new ActionOriginal { Action = action, Original = data });
                    }
                    if (file.Translates.All(x => x.TranslatedText != text))
                    {
                        list.Add(new ActionOriginal { Action = action, Original = data });
                    }
                }
            }
            var size = file.Translates.Count;
            foreach (var name in names)
            {
                var found = false;
                for (var i = 0; i < size; i++)
                {
                    var translate = file.Translates[i];
                    if (translate.OriginalText == name.Key)
                    {
                        found = true;
                        this.names.Add(new Name { Translate = translate });
                        break;
                    }
                }
                if (!found)
                {
                    if (name.Value.Count == 0)
                    {
                        Console.WriteLine("WTF");
                    }
                    var translate = new TranslateData("", name.Value);
                    this.names.Add(new Name { Translate = translate });
                    file.Translates.Add(translate);
                }
            }

            TextsList.DataContext = this.names;
            TextsList.ItemsSource = this.names;
            TextsList.SelectionChanged += TextsList_SelectionChanged;
        }

        private void SelectItem(int ind)
        {
            if (ind < 0) return;

            var place = names[ind];
            NameBox1.DataContext = place;
            NameBox2.DataContext = place;
            if (names[ind].TranslatedName == "" && (bool)Autotranslate.IsChecked)
            {
                NameBox2.Text = ClassTranslator.TranslateText(names[ind].OriginalName);
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
