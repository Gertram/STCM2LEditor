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

using STCM2L.classes;

namespace STCM2L
{
    /// <summary>
    /// Логика взаимодействия для NamesView.xaml
    /// </summary>
    public partial class NamesView : Window, Findable
    {
        public bool find(string text)
        {
            for (var i = 0; i < namesTranslate.Count; i++)
            {
                var name = namesTranslate[i];
                if (name.OriginalText.Contains(text) || name.TranslatedText.Contains(text))
                {
                    if (i + 10 < namesTranslate.Count)
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
        
        private BindingList<TranslateData> namesTranslate = new BindingList<TranslateData>();
        internal NamesView(classes.STCM2L file)
        {
            InitializeComponent();
            var names = new Dictionary<string, List<DefaultAction>>();
            foreach (var action in file.DefaultActions)
            {
                if (action.OpCode == ActionHelpers.ACTION_NAME)
                {
                    var param = action.Parameters[0] as LocalParameter;
                    var data2 = new StringData("");
                    //var data = new ProxyData(new StringData(param.ParameterData), data2);
                    //param.ParameterData = data;
                    var text = (new StringData(param.ParameterData)).Text;
                    if (!names.TryGetValue(text, out var list))
                    {
                        list = new List<DefaultAction>();
                        names.Add(text, list);
                        list.Add(action);
                    }
                    else if (file.Translates.All(x => x.TranslatedText != text))
                    {
                        list.Add(action);
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
                        namesTranslate.Add(translate);
                        break;
                    }
                }
                if (!found)
                {
                    if (name.Value.Count == 0)
                    {
                        Console.WriteLine("WTF");
                    }
                    var data2 = new StringData("");
                    
                    var _actions = new List<ActionProxy>();
                    foreach (var action in name.Value)
                    {
                        var param = action.Parameters[0] as LocalParameter;
                        var data = new ProxyData(new StringData(param.ParameterData), data2);
                        param.ParameterData = data;
                        _actions.Add(new ActionProxy { Action = action, Proxy = data });
                    }
            
                    var translate = new TranslateData(data2, _actions);

                    namesTranslate.Add(translate);
                    file.Translates.Add(translate);
                }
            }

            TextsList.DataContext = namesTranslate;
            TextsList.ItemsSource = namesTranslate;
            TextsList.SelectionChanged += TextsList_SelectionChanged;
        }

        private void SelectItem(int ind)
        {
            if (ind < 0) return;

            var translate = namesTranslate[ind];
            NameBox1.DataContext = translate;
            NameBox2.DataContext = translate;
            if (namesTranslate[ind].TranslatedText == "" && (bool)Autotranslate.IsChecked)
            {
                NameBox2.Text = ClassTranslator.TranslateText(namesTranslate[ind].OriginalText);
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
