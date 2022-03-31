using MahApps.Metro.Controls;
using Microsoft.Win32;
using STCM2LEditor.classes.Action;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace STCM2LEditor
{
    public partial class MainWindow : MetroWindow
    {
        public BindingList<Replic> Replics { get; set; } = new BindingList<Replic>();
        internal classes.STCM2L Stcm2l { get; set; }
        private bool ShouldSave = false;

        public MainWindow()
        {
            InitializeComponent();
            Closing += OnClose;
            
            //var dir = Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
            try
            {
                var gamePreset = ConfigurationManager.AppSettings["LastGamePreset"];
                if (gamePreset != null && gamePreset != "")
                {
                    foreach(var item in GamesPresetMenu.FindChildren<MenuItem>())
                    {
                        if((string)item.CommandParameter == gamePreset)
                        {
                            SetGamePreset(item,gamePreset);
                            break;
                        }
                    }
                    
                }
                var lastFile = ConfigurationManager.AppSettings["lastFile"];
                OpenFile(lastFile);
            }
            catch (Exception)
            {

            }
        }
        internal void DeleteText(int index)
        {
            var replic = Replics[index];
            Stcm2l.DeleteReplic(replic);
            Replics.RemoveAt(index);
        }
        internal void InsertText(int index, bool before)
        {
            var replic = Replics[index];
            if (before)
            {
                Replics.Insert(index, Stcm2l.NewReplic(replic, before));
            }
            else
            {
                Replics.Insert(index + 1, Stcm2l.NewReplic(replic, before));
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
                }
                catch (Exception exp)
                {
                    MessageBox.Show(exp.ToString());
                }
            }
        }
        private void NakeReplics()
        {
            var actions = Stcm2l.Actions;
            Replic text = null;
            Replics.Clear();
            for (var i = 0; i < actions.Count; i++)
            {
                var action = actions[i];

                while (action.OpCode == ActionHelpers.ACTION_NAME || action.OpCode == ActionHelpers.ACTION_TEXT)
                {
                    if (text == null)
                    {
                        text = new Replic();
                    }
                    if (action.OpCode == ActionHelpers.ACTION_NAME)
                    {
                        if (text.Name.OriginalText != "")
                        {
                            throw new Exception("WTF");
                        }
                        text.Name = action as IStringAction;
                    }
                    else if (action.OpCode == ActionHelpers.ACTION_TEXT)
                    {
                        text.Lines.Add(action as IStringAction);
                    }
                    i++;
                    action = actions[i];
                }
                if (text != null)
                {
                    if(text.Lines.Count == 0)
                    {
                        text.AddLine(Stcm2l);
                    }
                    Replics.Add(text);
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
                NakeReplics();
                TextsList.DataContext = this;

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
            catch (Exception exp)
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
            var translate = LinesList.DataContext as Replic;
            if (translate != null)
            {
                translate.AddLine(Stcm2l, index);
                ShouldSave = true;
            }
        }

        private void DeleteLineClick(object sender, RoutedEventArgs e)
        {
            int index = LinesList.SelectedIndex;
            ((sender as MenuItem).DataContext as Replic).DeleteLine(Stcm2l, index);
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
            InsertText(TextsList.SelectedIndex, before);
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

        private void PackCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var inputDir = ConfigurationManager.AppSettings["INPUT_DIR"];
            var csvPath = ConfigurationManager.AppSettings["CSV_PATH"];
            var cpkMakerPath = ConfigurationManager.AppSettings["CPKMAKER_PATH"];
            var cpkPath = ConfigurationManager.AppSettings["CPK_PATH"];
            var ebootPath = ConfigurationManager.AppSettings["EBOOT_PATH"];
            var output = ConfigurationManager.AppSettings["ISO_FILE"];
            var ultraISOPath = ConfigurationManager.AppSettings["ULTRAISO_PATH"];
            var writer = new StreamWriter(csvPath, false);
            int i = 0;
            foreach (var file in Directory.EnumerateFiles(inputDir, "*", SearchOption.AllDirectories))
            {
                var localName = file.Substring(inputDir.Length + 1).Replace("\\", "/");
                var fileName = file.Replace("\\", "/");
                writer.WriteLine(@"{0},{1},{2},Uncompress", fileName, localName, i);
                i++;
            }
            writer.Close();

            var cmd = string.Format("\"{0}\" \"{1}\" -mode=FILENAME", csvPath, cpkPath);

            CMD(cpkMakerPath, cmd);

            cmd = string.Format(" -in \"{0}\" -chdir \"/PSP_GAME/INSDIR\" -f \"{1}\" -chdir \"/PSP_GAME/SYSDIR\" -f \"{2}\"", output, cpkPath, ebootPath);

            CMD(ultraISOPath, cmd);
            //MessageBox.Show("Done");
        }
        private void CMD(string filename, string arguments)
        {
            
            var processInfo = new ProcessStartInfo(filename, arguments)
            {
                RedirectStandardOutput = false,
                WindowStyle = ProcessWindowStyle.Normal,
                UseShellExecute = true,
                CreateNoWindow = false
            };
            var process = Process.Start(processInfo);
            process.WaitForExit();
        }
        private void SetGamePreset(MenuItem menuItem, string id)
        {
            if (id == "0")
            {
                ActionHelpers.ACTION_NAME = 0xd4;
                ActionHelpers.ACTION_TEXT = 0xd2;
                ActionHelpers.ACTION_CHOICE = 0xe7;
                ActionHelpers.ACTION_DIVIDER = 0xd3;
                ActionHelpers.ACTION_NEW_PAGE = 0x1c1;
                ActionHelpers.ACTION_PLACE = 0x79d0;
                ActionHelpers.ACTION_SHOW_PLACE = 0x227c;
            }
            else if (id == "1")
            {
                ActionHelpers.ACTION_NAME = 0x4B074;
                ActionHelpers.ACTION_TEXT = 0x4A29C;
                //ActionHelpers.ACTION_CHOICE = 0xe7;
                ActionHelpers.ACTION_DIVIDER = 0x4AF94;
               // ActionHelpers.ACTION_NEW_PAGE = 0x1c1;
                //ActionHelpers.ACTION_PLACE = 0x79d0;
                //ActionHelpers.ACTION_SHOW_PLACE = 0x227c;
            }
            else if (id == "2")
            {
                //ActionHelpers.ACTION_NAME = 0x4B074;
                ActionHelpers.ACTION_TEXT = 0x4ba;
                //ActionHelpers.ACTION_CHOICE = 0xe7;
                ActionHelpers.ACTION_DIVIDER = 0x4bb;
               // ActionHelpers.ACTION_NEW_PAGE = 0x1c1;
                //ActionHelpers.ACTION_PLACE = 0x79d0;
                //ActionHelpers.ACTION_SHOW_PLACE = 0x227c;
            }
            else if (id == "3")
            {
                //ActionHelpers.ACTION_NAME = 0x4B074;
                ActionHelpers.ACTION_TEXT = 0xcdc4;
                //ActionHelpers.ACTION_CHOICE = 0xe7;
                ActionHelpers.ACTION_DIVIDER = 0x1f4;
               // ActionHelpers.ACTION_NEW_PAGE = 0x1c1;
                //ActionHelpers.ACTION_PLACE = 0x79d0;
                //ActionHelpers.ACTION_SHOW_PLACE = 0x227c;
            }
            if(Stcm2l != null)
            {
                OpenFile(Stcm2l.FilePath);
            }
            AddUpdateAppSettings("LastGamePreset", id);
            foreach (var item in GamesPresetMenu.FindChildren<MenuItem>())
            {
                if (item != menuItem)
                {
                    item.IsChecked = false;
                }
                else
                {
                    item.IsChecked = true;
                }
            }
        }
        private void GamePresetCommand(object sender, ExecutedRoutedEventArgs e)
        {
            SetGamePreset(e.OriginalSource as MenuItem,e.Parameter as string);
            
        }

        private void InsertTrashCommand(object sender, ExecutedRoutedEventArgs e)
        {
            foreach(var replic in Replics)
            {
                foreach(var line in replic.Lines)
                {
                    line.TranslatedText = "Some trash translated text here";
                }
            }
        }

        private void MenuItemExportOriginalText_Click(object sender, RoutedEventArgs e)
        {
            if (Stcm2l == null)
            {
                return;
            }
            var sfd = new SaveFileDialog();
            if (!(bool)sfd.ShowDialog())
            {
                return;
            }
            var writer = new StreamWriter(sfd.FileName, false);
            foreach (var item in Replics)
            {
                if (item.Name.OriginalText != "")
                    writer.Write($"{item.Name.OriginalText}:");
                writer.WriteLine(string.Join("|",item.Lines.Select(x => x.OriginalText)));
            }
            writer.Close();
        }

        private void MenuItemExportTranslatedText_Click(object sender, RoutedEventArgs e)
        {
            if (Stcm2l == null)
            {
                return;
            }
            var sfd = new SaveFileDialog();
            if (!(bool)sfd.ShowDialog())
            {
                return;
            }
            var writer = new StreamWriter(sfd.FileName, false);
            foreach (var item in Replics)
            {
                if(item.Name != null)
                    if (item.Name.TranslatedText != "")
                        writer.Write($"{item.Name.TranslatedText}:");
                writer.WriteLine(string.Join("|", item.Lines.Select(x => x.TranslatedText)));
            }
            writer.Close();
        }

        private void MenuItemImportText_Click(object sender, RoutedEventArgs e)
        {
            if (Stcm2l == null)
            {
                return;
            }
            var ofd = new OpenFileDialog();
            if (!(bool)ofd.ShowDialog())
            {
                return;
            }
            var reader = new StreamReader(ofd.FileName);
            var list = new List<string[]>();
            while (!reader.EndOfStream)
            {
                var text = reader.ReadLine().Trim();
                int index = text.IndexOf(':');
                if(index >= 0)
                {
                    text = text.Substring(0, index + 1).Trim();
                }
                list.Add(text.Split(new char[] { '|' }));
            }
            reader.Close();
            if(list.Count != Replics.Count)
            {
                MessageBox.Show("Replice count not eq sourc replic count");
                return;
            }
            for(int i = 0;i < list.Count; i++)
            {
                var item = list[i];
                var replic = Replics[i];
                if(item.Length > replic.Lines.Count)
                {
                    for(int j = 0; j < item.Length - replic.Lines.Count; j++)
                    {
                        replic.AddLine(Stcm2l);
                    }
                }
                for(int j = 0;j < replic.Lines.Count; j++)
                {
                    replic.Lines[j].TranslatedText = item[j];
                }
            }
            
        }
    }
}
