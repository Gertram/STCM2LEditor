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
using Microsoft.Win32;
using Forms = System.Windows.Forms;
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
            int i = 0;
            foreach(ComboBoxItem item in FontSizeComboBox.Items)
            {
                var str = item.Content as string;
                if (int.Parse(str) == MainConfig.FontSize.Value)
                {
                    FontSizeComboBox.SelectedIndex = i;
                }
                i++;
            }
            TranslateBackup.Text = MainConfig.TranslateBackupFile;
            EngTextDirectoryTextBox.Text = MainConfig.EngTextDirectory;
        }

        private void TranslateLanguageSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Translator.TranslateLanguage = TranslateLanguageSelect.SelectedIndex;
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

        private void WordDirectoryButton_Click(object sender, RoutedEventArgs e)
        {
            var win = new Forms.FolderBrowserDialog();
            
            if (win.ShowDialog() != Forms.DialogResult.OK || string.IsNullOrWhiteSpace(win.SelectedPath))
            {
                return;
            }
            WorkDirectoryTextBox.Text = win.SelectedPath;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem item && int.TryParse(item.Content as string, out var value))
            {
                MainConfig.FontSize.Value = value;
            }
        }
    }
}
