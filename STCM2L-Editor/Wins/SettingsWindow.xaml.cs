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

namespace STCM2LEditor.Wins
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
            WorkDirectoryTextBox.Text = MainConfig.WorkDirectory;
            TranslateBackup.Text = MainConfig.TranslateBackupFile;
            EngTextDirectoryTextBox.Text = MainConfig.EngTextDirectory;
        }

        private void TranslateLanguageSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Translator.TranslateLanguage = TranslateLanguageSelect.SelectedIndex;
        }

        private void WorkDirectoryTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && GetText(WorkDirectoryTextBox,out var text))
            {
                MainConfig.WorkDirectory = text;
            }
        }

        private void EngTextDirectoryTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && GetText(EngTextDirectoryTextBox,out var text))
            {
                MainConfig.EngTextDirectory = text;
            }
        }
        private bool GetText(TextBox textBox,out string text)
        {
            text = textBox.Text.Trim();
            return text.Length != 0;
        }

        private void TranslateBackup_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && GetText(TranslateBackup, out var text))
            {
                MainConfig.TranslateBackupFile = text;
            }
        }
    }
}
