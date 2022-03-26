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
using System.IO;
using System.ComponentModel;
using System.Configuration;

using STCM2L.classes;
using MahApps.Metro.Controls;

namespace STCM2L
{
    public partial class MainWindow : MetroWindow
    {
        internal BindingList<TextTranslate> Translates { get; set; }
        internal classes.STCM2L Stcm2l { get; set; }
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
        internal void DeleteText(int index)
        {
            var translate = Translates[index];
            translate.DeleteFrom(Stcm2l);
            Translates.RemoveAt(index);
        }
        internal void InsertText(int index,bool before)
        {
            var translate = Translates[index];
            if (before)
            {
                Translates.Insert(index,translate.Insert(Stcm2l,before));
            }
            else
            {
                Translates.Insert(index+1,translate.Insert(Stcm2l,before));
            }
            ShouldSave = true;
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
            if (Stcm2l == null)
            {
                return;
            }
            var view = new ActionsView(Stcm2l);
            view.Show();
        }
        private void PlaceWindowCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (Stcm2l == null) return;
            var win = new PlaceWindow(Stcm2l);
            win.ShowDialog();
        }
        private void ImportFromCommand(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                if (Stcm2l == null)
                {
                    return;
                }
                var form = new ImportWindow(this);

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
                try
                {
                    OpenFile(openFileDialog.FileName);
                }catch(Exception exp)
                {
                    MessageBox.Show(exp.ToString());
                }
            }
        }
        private TranslateData MakeTranslate(DefaultAction action,bool insert = false)
        {
            foreach (var translate in Stcm2l.Translates)
            {
                foreach(var _action in translate.Actions)
                {
                    if(_action.Action == action)
                    {
                        return translate;
                    }
                }
            }
            var param = action.Parameters[0] as LocalParameter;
            var data2 = new StringData("");
            var data = new ProxyData(new StringData(param.ParameterData),data2);
            param.ParameterData = data;
            var _actions = new List<ActionProxy>
            {
                new ActionProxy { Action = action, Proxy = data }
            };
            var _translate = new TranslateData(data2, _actions);
            if(insert)
                Stcm2l.Translates.Add(_translate);
            return _translate;
        }
        private void MakeTexts()
        {
            var actions = Stcm2l.Actions;
            TextTranslate text = null;
            Translates = new BindingList<TextTranslate>();
            for (var i = 0; i < actions.Count; i++)
            {
                var action = actions[i];

                while (action.OpCode == ActionHelpers.ACTION_NAME || action.OpCode == ActionHelpers.ACTION_TEXT)
                {
                    if(text == null)
                    {
                        text = new TextTranslate();
                    }
                    if (action.OpCode == ActionHelpers.ACTION_NAME)
                    {
                        if (text.Name.OriginalText != "")
                        {
                            throw new Exception("WTF");
                        }
                        text.Name = MakeTranslate(action as DefaultAction);
                    }
                    else if (action.OpCode == ActionHelpers.ACTION_TEXT)
                    {
                        var temp = MakeTranslate(action as DefaultAction,true);
                        text.Lines.Add(temp);
                    }
                    i++;
                    action = actions[i];
                }
                if(text != null)
                {
                    Translates.Add(text);
                    text = null;
                }
            }
        }

        private void OpenFile(string path)
        {
            Stcm2l = new classes.STCM2L(path);

            Title = Path.GetFileName(path);

            if (Stcm2l.Load())
            {
                AddUpdateAppSettings("lastFile", path);
                MakeTexts();

                TextsList.DataContext = Translates;
                TextsList.ItemsSource = Translates;

                LinesList.DataContext = null;
                LinesList.ItemsSource = null;

                NameBox1.DataContext = null;
                NameBox2.DataContext = null;

                ShouldSave = false;
                TextsList.SelectionChanged += TextsList_SelectionChanged;
            }
            else
            {
                throw new Exception("Invalid File");
            }
        }

        private void TextsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = sender;
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
            try
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
            catch(Exception exp) 
            {
                MessageBox.Show(exp.ToString());
            }
        }
        private void SelectItem(int ind)
        {
            if (ind < 0 || ind >= TextsList.Items.Count) return;
            bool temp = ShouldSave;
            var item = TextsList.Items[ind];

            LinesList.DataContext = item;
            NameBox1.DataContext = item;
            NameBox2.DataContext = item;

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

        private void AddNewLineClick(object sender, RoutedEventArgs e)
        {
            InsertLine();
        }

        private void InsertNewLineBeforeClick(object sender, RoutedEventArgs e)
        {
            InsertLine(LinesList.SelectedIndex);
        }

        private void InsertNewLineAfterClick(object sender, RoutedEventArgs e)
        {
            InsertLine(LinesList.SelectedIndex + 1);
        }

        private void InsertLine(int index = -1)
        {
            var translate = LinesList.DataContext as TextTranslate;
            if (translate != null) {
                translate.AddLine(Stcm2l, index);
                ShouldSave = true;
            }
        }

        private void DeleteLineClick(object sender, RoutedEventArgs e)
        {
            int index = LinesList.SelectedIndex;
            ((sender as MenuItem).DataContext as TextTranslate).DeleteLine(Stcm2l,index);
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
            InsertText(TextsList.SelectedIndex,before);
        }

        private void DeleteTextClick(object sender, RoutedEventArgs e)
        {
            DeleteText(TextsList.SelectedIndex);
            

            LinesList.DataContext = null;
            LinesList.ItemsSource = null;

            NameBox1.DataContext = null;
            NameBox2.DataContext = null;

            ShouldSave = true;
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            ShouldSave = true;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NameWindowCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var win = new NamesView(Stcm2l);
            win.ShowDialog();
        }

        private void TextBlock_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var block = sender as TextBlock;
            if (block.Text.Length > 20)
                block.Text = block.Text.Substring(0, 20);
        }
    }
}
