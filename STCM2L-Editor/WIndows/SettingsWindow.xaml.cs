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

using STCM2LEditor.classes;
using STCM2LEditor.utils;

namespace STCM2LEditor.Windows
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            TranslateLanguageSelect.ItemsSource = Translator.Languages;
            TranslateLanguageSelect.DataContext = Translator.Languages;
            TranslateLanguageSelect.SelectedIndex = Translator.TranslateLanguage;
            WorkDirectoryTextBox.Text = Config.Get("WorkDirectory");
            EngTextDirectoryTextBox.Text = Config.Get("EngTextDirectory");
        }

        private void TranslateLanguageSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Translator.TranslateLanguage = TranslateLanguageSelect.SelectedIndex;
        }

        private void WorkDirectoryTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                Config.Set("WorkDirectory",WorkDirectoryTextBox.Text);
            }
        }

        private void EngTextDirectoryTextBox_KeyUp(object sender, KeyEventArgs e)
        {
           if(e.Key == Key.Enter)
            {
                Config.Set("EngTextDirectory", EngTextDirectoryTextBox.Text);
            }
        }
    }
}
