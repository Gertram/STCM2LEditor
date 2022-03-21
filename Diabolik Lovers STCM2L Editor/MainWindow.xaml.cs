using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;
using System.ComponentModel;
using System.Configuration;

using Diabolik_Lovers_STCM2L_Editor.classes;
using MahApps.Metro.Controls;

namespace Diabolik_Lovers_STCM2L_Editor
{
    public class WindowCommands
    {
        static WindowCommands()
        {
            ImportFrom = new RoutedCommand("ImportFrom", typeof(MainWindow));
            ImportText = new RoutedCommand("ImportText", typeof(MainWindow));
            ActionsView = new RoutedCommand("ActionsView", typeof(MainWindow));
            PlaceView = new RoutedCommand("PlaceView", typeof(MainWindow));
            Find = new RoutedCommand("Find", typeof(MainWindow));
            Import = new RoutedCommand("Import", typeof(MainWindow));
            Export = new RoutedCommand("Export", typeof(MainWindow));
        }
        public static RoutedCommand ImportFrom { get; set; }
        public static RoutedCommand ImportText { get; set; }
        public static RoutedCommand ActionsView { get; set; }
        public static RoutedCommand PlaceView { get; set; }
        public static RoutedCommand Find { get; set; }
        public static RoutedCommand Import { get; set; }
        public static RoutedCommand Export { get; set; }
    }
    public partial class MainWindow : MetroWindow
    {
        private STCM2L Stcm2l;
        private bool ShouldSave = false;

        public MainWindow()
        {
            InitializeComponent();
            Closing += OnClose;
            //var dir = Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
            try
            {
                var lastFile = ConfigurationManager.AppSettings["lastFile"];
                OpenFile(lastFile);
            }
            catch (Exception)
            {

            }
        }
        static void AddUpdateAppSettings(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error writing app settings");
            }
        }
    
    private void OnClose(object sender, CancelEventArgs e)
        {
            if (Stcm2l != null && ShouldSave)
            {
                MessageBoxResult saveWarning = ShowSaveWarning();

                switch (saveWarning)
                {
                    case MessageBoxResult.Yes:
                        SaveAsCommand(null, null);
                        break;
                    case MessageBoxResult.No:
                        break;
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }

        private MessageBoxResult ShowSaveWarning()
        {
            string messageBoxCaption = "Save";
            string messageBoxText = "Do you want to save your changes?";
            MessageBoxButton button = MessageBoxButton.YesNoCancel;
            MessageBoxImage image = MessageBoxImage.Warning;

            return MessageBox.Show(messageBoxText, messageBoxCaption, button, image);
        }

        private void ActionsViewCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if(Stcm2l == null)
            {
                return;
            }
            var view = new ActionsView(Stcm2l);
            view.ShowDialog();
        }
        private void PlaceWindowCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (Stcm2l == null) return;
            var win = new PlaceWindow(Stcm2l);
            win.ShowDialog();
        }
        private void ImportTextCommand(object sender, ExecutedRoutedEventArgs e)
        {

        }
        private void ImportFromCommand(object sender, ExecutedRoutedEventArgs e)
        {
            /*try
            {
                if (Stcm2l == null)
                {
                    MessageBox.Show("Не загружен файл скрипта");
                    return;
                }
                var ofd = new OpenFileDialog();
                ofd.Filter = "XML Files (*.xml)|*.xml";
                if ((bool)!ofd.ShowDialog())
                {
                    return;
                }
                var doc = XDocument.Load(ofd.FileName);
                var root = doc.Root;
                if (root.Name.LocalName.ToUpper() != "SCRIPT")
                {
                    MessageBox.Show("Не был найден корневой элемент \"script\"");
                    return;
                }
                var fragment_num = 0;
                var fragments = root.Elements("fragment");
                if (fragments.Count() == 0)
                {
                    MessageBox.Show("Необнаружена ни одна строка");
                    return;
                }
                if (fragments.Count() != Stcm2l.Texts.Count)
                {
                    MessageBox.Show($"Количество фрагментов не соответсвует исходному\nИсходный {Stcm2l.Texts.Count}\nИмпортируемый {fragments.Count()}");
                    return;
                }
                foreach (var fragment in fragments)
                {
                    var line_num = 0;
                    var lines = fragment.Elements("line");
                    var text = Stcm2l.Texts[fragment_num];

                    if (text.Lines.Count() > lines.Count())
                    {
                        for (int i = text.Lines.Count() - 1; i > lines.Count() - 1; i--)
                        {
                            text.DeleteLine(i);
                        }
                    }
                    else if (text.Lines.Count() < lines.Count())
                    {
                        for (int i = lines.Count(); i != text.Lines.Count; i++)
                        {
                            text.AddLine(true);
                        }
                    }
                    foreach (var line in lines)
                    {
                        text.Lines[line_num] = new Line(line.Value);
                        line_num++;
                    }
                    fragment_num++;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка");
            }*/
            try
            {
                if (Stcm2l == null)
                {
                    return;
                }
                var form = new ImportWindow(Stcm2l);

                if ((bool)!form.ShowDialog())
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка");
            }
        }
        private void OpenFileCommad(object sender, ExecutedRoutedEventArgs e)
        {
            if (Stcm2l != null && ShouldSave)
            {
                MessageBoxResult saveWarning = ShowSaveWarning();

                switch (saveWarning)
                {
                    case MessageBoxResult.Yes:
                        SaveAsCommand(null, null);
                        break;
                    case MessageBoxResult.No:
                        break;
                    case MessageBoxResult.Cancel:
                        return;
                }
            }

            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                OpenFile(openFileDialog.FileName);
            }
        }

        private void OpenFile(string path)
        {
            Stcm2l = new STCM2L(path);

            Title = Path.GetFileName(path);

            if (Stcm2l.Load())
            {
                TextsList.DataContext = Stcm2l.Texts;
                TextsList.ItemsSource = Stcm2l.Texts;

                LinesList.DataContext = null;
                LinesList.ItemsSource = null;

                NameBox.DataContext = null;

                ShouldSave = false;
                AddUpdateAppSettings("lastFile",path);
                TextsList.SelectionChanged += TextsList_SelectionChanged;
            }
            else
            {
                Console.WriteLine("Invalid File");
            }
        }

        private void TextsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectItem(TextsList.SelectedIndex);
        }

        private void SaveAsCommand(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            if (saveFileDialog.ShowDialog() == true)
            {
                if (Stcm2l == null || !Stcm2l.Save(saveFileDialog.FileName))
                {
                   Console.WriteLine("Failed to save.");
                }
                else
                {
                    OpenFile(saveFileDialog.FileName);
                    ShouldSave = false;
                }
            }
        }

        private void SaveCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (Stcm2l == null || !Stcm2l.Save(Stcm2l.FilePath))
            {
                Console.WriteLine("Failed to save.");
            }
            else
            {
                ShouldSave = false;
            }
        }
        private void SelectItem(int ind)
        {
            if (ind < 0 || ind >= TextsList.Items.Count) return;
            bool temp = ShouldSave;
            var item = TextsList.Items[ind];

            LinesList.DataContext = item;
            NameBox.DataContext = item;

            Binding binding = new Binding();
            binding.Path = new PropertyPath("Lines");
            binding.Source = item;
            binding.Mode = BindingMode.TwoWay;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            LinesList.SetBinding(ItemsControl.ItemsSourceProperty, binding);

            if (!temp)
            {
                ShouldSave = false;
            }
        }
        private void TextsListItemClick(object sender, MouseButtonEventArgs e)
        {
            SelectItem(TextsList.SelectedIndex);
        }

        private void ResetLineClick(object sender, RoutedEventArgs e)
        {
            ((sender as MenuItem).DataContext as TextEntity).Lines[LinesList.SelectedIndex].Reset();
        }

        private void ResetNameClick(object sender, RoutedEventArgs e)
        {
            if (NameBox.DataContext as TextEntity != null)
            {
                (NameBox.DataContext as TextEntity).ResetName();
            }
        }

        private void AddNewLineClick(object sender, RoutedEventArgs e)
        {
            if (LinesList.DataContext as TextEntity != null)
            {
                InsertLine();
                Stcm2l.AddLine(TextsList.SelectedIndex, 1);
            }
        }

        private void InsertNewLineBeforeClick(object sender, RoutedEventArgs e)
        {
            if (LinesList.DataContext as TextEntity != null)
            {
                InsertLine(LinesList.SelectedIndex);
                Stcm2l.AddLine(TextsList.SelectedIndex, 1);
            }
        }

        private void InsertNewLineAfterClick(object sender, RoutedEventArgs e)
        {
            if (LinesList.DataContext as TextEntity != null)
            {
                InsertLine(LinesList.SelectedIndex + 1);
                Stcm2l.AddLine(TextsList.SelectedIndex, 1);
            }
        }

        private void InsertLine(int index = -1)
        {
            (LinesList.DataContext as TextEntity).AddLine(false, index);
            ShouldSave = true;
        }

        private void DeleteLineClick(object sender, RoutedEventArgs e)
        {
            int index = LinesList.SelectedIndex;
            ((sender as MenuItem).DataContext as TextEntity).DeleteLine(index);
            Stcm2l.DeleteLine(index, 1);
            ShouldSave = true;
        }

        private void InsertNewTextAfterClick(object sender, RoutedEventArgs e)
        {
            InsertNewText(false);
        }

        private void InsertNewTextBeforeClick(object sender, RoutedEventArgs e)
        {
            InsertNewText(true);
        }

        private void InsertNewText(bool before)
        {
            if (TextsList.SelectedIndex != -1)
            {
                bool newPage = false;
                if (Stcm2l.Texts[TextsList.SelectedIndex].Name == null)
                {
                    string messageBoxCaption = "New page";
                    string messageBoxText = "Do you want to create a new page?";
                    MessageBoxButton button = MessageBoxButton.YesNo;
                    MessageBoxImage image = MessageBoxImage.Question;

                    MessageBoxResult result = MessageBox.Show(messageBoxText, messageBoxCaption, button, image);

                    newPage = result == MessageBoxResult.Yes;
                }
                Stcm2l.InsertText(TextsList.SelectedIndex, before, newPage);
                ShouldSave = true;
            }
        }

        private void DeleteTextClick(object sender, RoutedEventArgs e)
        {
            Stcm2l.DeleteText(TextsList.SelectedIndex);

            LinesList.DataContext = null;
            LinesList.ItemsSource = null;

            NameBox.DataContext = null;

            ShouldSave = true;
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            ShouldSave = true;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ImportFileCommand(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void ExportFileCommand(object sender, ExecutedRoutedEventArgs e)
        {

        }
    }
}
