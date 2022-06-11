using MahApps.Metro.Controls;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Threading;
using System.Windows.Input;

using System.Windows.Media;

using STCM2LEditor.classes;
using STCM2LEditor.utils;
using STCM2LEditor.classes.Action;
using STCM2LEditor.classes.Action.Parameters;
using STCM2LEditor.GamePresets;
using STCM2LEditor.Wins;
using System.Text.Json;

namespace STCM2LEditor
{
    class PackerSettings
    {
        public string INPUT_DIR { get; set; }
        public string ISO_FILE { get; set; }
        public string ULTRAISO_PATH { get; set; }
        public string CPKMAKER_PATH { get; set; }
        public string CSV_PATH { get; set; }
        public string CPK_PATH { get; set; }
        public string EBOOT_PATH { get; set; }
    }
    public partial class MainWindow : MetroWindow, INotifyPropertyChanged,Findable
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void SendChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        public BindingList<Replic> Replics
        {
            get => replics;
            set
            {
                replics = value;
                SendChanged(nameof(Replics));
            }
        }
        internal STCM2L Stcm2l { get; set; }
        private bool ShouldSave = false;
        private BindingList<Replic> replics = new BindingList<Replic>();
        private GamePreset currentPreset;

        public GamePreset CurrentPreset
        {
            get => currentPreset; set
            {
                currentPreset = value;
                SendChanged(nameof(CurrentPreset));
            }
        }
        private void LoadLastFile()
        {
            try
            {
                var lastFile = MainConfig.LastFile;
                if (lastFile != null)
                {
                    OpenFile(lastFile);
                    LoadBackup();
                }
            }
            catch (Exception)
            {

            }
        }
        public MainWindow()
        {
            InitializeComponent();

            LoadGamePresets();

            LoadLastFile();


            aTimer = new System.Timers.Timer
            {
                Interval = MainConfig.AutoSaveTimer.Value
            };
            aTimer.Elapsed += ATimer_Elapsed;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private void ATimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            SaveBackup();
        }

        private void LoadGamePresets()
        {
            GamePresetConfigProvider.Instance.Presets.CollectionChanged += Presets_CollectionChanged;
            foreach (var preset in GamePresetConfigProvider.Instance.Presets)
            {
                AddGamePreset(preset);
            }
            SetGamePreset(GamePresetConfigProvider.Instance.Selected);
        }

        private void Presets_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {

        }



        internal void DeleteText(int index)
        {
            var replic = Replics[index];
            Stcm2l.DeleteReplic(replic);
            Replics.RemoveAt(index);
            ShouldSave = true;
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

        private void OnClosing(object sender, CancelEventArgs e)
        {
            if (Stcm2l != null && ShouldSave)
            {
                MessageBoxResult saveWarning = ShowSaveWarning();

                switch (saveWarning)
                {
                    case MessageBoxResult.Yes:
                        if(!SaveAs())
                        {
                            e.Cancel = true;
                        }
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
                var form = new ImportWindow(this,Stcm2l.FilePath);

                if ((bool)!form.ShowDialog())
                {
                    return;
                }
                ShouldSave = true;
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

            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = MainConfig.WorkDirectory.Value
            };
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
        private static System.Timers.Timer aTimer; 

        private void OpenFile(string path)
        {
            try
            {
                var temp = new STCM2L(path, CurrentPreset);

                Title = Path.GetFileName(path);

                if (temp.Load())
                {
                    MainConfig.LastFile = path;
                    Stcm2l = temp;
                    Title = Path.GetFileName(path);
                    Replics = new BindingList<Replic>(Stcm2l.MakeReplics());
                    ReplicWrap.DataContext = null;
                    ShouldSave = false;

                }
                else
                {
                    throw new Exception("Invalid File");
                }
            }
            catch (InvalidFileTypeException exp)
            {
                MessageBox.Show($"The\"{exp.FileName}\"  file is in an unsupported format", "Error");
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString());
            }
        }

        private void TextsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ReplicWrap.DataContext = TextsList.SelectedItem;
        }

        private void SaveAsCommand(object sender, ExecutedRoutedEventArgs e) => SaveAs();
        private bool SaveAs()
        {

            SaveFileDialog saveFileDialog = new SaveFileDialog();

            if (!(bool)saveFileDialog.ShowDialog())
            {
                return false;
            }
            if (Stcm2l == null || !Stcm2l.Save(saveFileDialog.FileName))
            {
                Console.WriteLine("Failed to save.");
                return false;
            }
            OpenFile(saveFileDialog.FileName);
            ShouldSave = false;

            return true;
        }

        private void SaveCommand(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {

                if (Stcm2l == null)
                {
                    Console.WriteLine("Failed to save.");
                }
                else
                {
                    foreach (var replic in Replics)
                    {
                        replic.InsertTranslates();
                    }
                    if (Stcm2l.Save(Stcm2l.FilePath))
                    {
                        ShouldSave = false;
                    }
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString());
            }
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
            if (LinesList.DataContext is Replic translate)
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
            ReplicWrap.DataContext = null;
        }
        private void LoadBackup()
        {
            if (!File.Exists("main_backup.txt") || MainConfig.NormalClose)
            {
                MainConfig.NormalClose = false;
                return;
            }
            var res = MessageBox.Show("Обнаружено аварийное завершений работы приложения. Загрузить бэкап?", "Внимание", MessageBoxButton.YesNo);
            if (res != MessageBoxResult.Yes)
            {
                return;
            }

            var list = File.ReadAllText("main_backup.txt").Split('\n').Select(x => x.Split('|')).ToList();
            for (int i = 0; i < Replics.Count; i++)
            {
                var replic = Replics[i];
                var temp = list[i];
                for (int j = replic.Lines.Count; j < temp.Length; j++)
                {
                    replic.Lines[j].TranslatedText = temp[j];
                }
            }
        }
        private void SaveBackup()
        {
            if(Stcm2l == null || Replics == null)
            {
                return;
            }
            File.WriteAllText("main_backup.txt",
            string.Join("\n", Replics.Select(x => string.Join("|", x.Lines.Select(y => y.TranslatedText)))));
        }
        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            ShouldSave = true;
        }

        private void NameWindowCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var win = new NamesView(Stcm2l);
            win.ShowDialog();
        }

        private void PackCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var pack = JsonSerializer.Deserialize<PackerSettings>(File.ReadAllText("pack.json"));/*
            pack.INPUT_DIR = "D:\\Diabolik lovers\\Work\\DL";
            pack.ISO_FILE = "D:\\Diabolik lovers\\Work\\dl.ISO";
            pack.ULTRAISO_PATH = "C:\\Program Files (x86)\\UltraISO\\UltraISO.exe";
            pack.CPKMAKER_PATH = "D:\\Downloads\\CRI2.40.13.0\\crifilesystem\\cpkmakec.exe";
            pack.CSV_PATH = "D:\\Diabolik lovers\\Work\\DL.CSV";
            pack.CPK_PATH = "D:\\Diabolik lovers\\Work\\INSTALL.DNS";
            pack.EBOOT_PATH = "D:\\Diabolik lovers\\Work\\EBOOT.BIN";
            File.WriteAllText("pack.json", JsonSerializer.Serialize(pack));*/

            var inputDir = pack.INPUT_DIR;//ConfigurationManager.AppSettings["INPUT_DIR"];

            var csvPath = pack.CSV_PATH;// ConfigurationManager.AppSettings["CSV_PATH"];
            var cpkMakerPath = pack.CPKMAKER_PATH;// ConfigurationManager.AppSettings["CPKMAKER_PATH"];
            var cpkPath = pack.CPK_PATH; //ConfigurationManager.AppSettings["CPK_PATH"];
            var ebootPath = pack.EBOOT_PATH;// ConfigurationManager.AppSettings["EBOOT_PATH"];
            var output = pack.ISO_FILE;// ConfigurationManager.AppSettings["ISO_FILE"];
            var ultraISOPath = pack.ULTRAISO_PATH;// ConfigurationManager.AppSettings["ULTRAISO_PATH"];
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
        private void SetGamePreset(GamePreset preset)
        {
            if (preset == null)
            {
                return;
            }
            CurrentPreset = preset;
            var id = GamePresetConfigProvider.Instance.Presets.IndexOf(preset);

            if (preset.ACTION_NAME != 0)
            {
                ActionHelpers.ACTION_NAME = preset.ACTION_NAME;
            }
            if (preset.ACTION_TEXT != 0)
            {
                ActionHelpers.ACTION_TEXT = preset.ACTION_TEXT;
            }
            if (preset.ACTION_PLACE != 0)
            {
                ActionHelpers.ACTION_PLACE = preset.ACTION_PLACE;
            }
            if (preset.ACTION_DIVIDER != 0)
            {
                ActionHelpers.ACTION_DIVIDER = preset.ACTION_DIVIDER;
            }
            EncodingUtil.Current = preset;
            GamePresetConfigProvider.Instance.Selected = preset;

            if (Stcm2l != null)
            {
                OpenFile(Stcm2l.FilePath);
            }
            Config.Set("LastGamePreset", preset.Name);
            foreach (MenuItem item in GamesPresetMenu.Items)
            {
                if (item != GamesPresetMenu.Items[id])
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
            SetGamePreset(e.Parameter as GamePreset);
        }

        private void InsertTrashCommand(object sender, ExecutedRoutedEventArgs e)
        {
            foreach (var replic in Replics)
            {
                foreach (var line in replic.Lines)
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
                if (item.Name != null && item.Name.OriginalText != "")
                    writer.Write($"{item.Name.OriginalText}:");
                writer.WriteLine(string.Join("|", item.Lines.Select(x => x.OriginalText)));
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
                if (item.Name != null)
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
                if (index >= 0)
                {
                    text = text.Substring(index + 1).Trim();
                }
                list.Add(text.Split(new char[] { '|' }));
            }
            reader.Close();
            if (list.Count != Replics.Count)
            {
                MessageBox.Show("Replice count not eq sourc replic count");
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                var replic = Replics[i];
                for (int j = 0; j < item.Length - replic.Lines.Count; j++)
                {
                    replic.AddLine(Stcm2l);
                }
                for (int j = 0; j < replic.Lines.Count - item.Length; j++)
                {
                    replic.DeleteLine(Stcm2l);
                }
                for (int j = 0; j < item.Count(); j++)
                {
                    replic.Lines[j].TranslatedText = item[j];
                }
            }

        }

        private void SettingsCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var win = new Wins.SettingsWindow();
            win.ShowDialog();
        }
        class Group
        {
            public List<IAction> Actions { get; set; } = new List<IAction>();
            public int CharCount { get; set; } = 0;
        }
        private int CharCount(IAction action)
        {
            if (action.Parameters.Count > 0 && action.Parameters[0] is ILocalParameter param)
            {
                var str = CurrentPreset.Encoding.GetString(param.Data.ExtraData);
                if (str.Length > 0 && action.OpCode == 22516)
                {
                    Console.WriteLine(str);
                }

                return str.Length;
            }
            return 0;
        }
        private void TextAnalyzeMenuItme_Click(object sender, RoutedEventArgs e)
        {
            if (ShouldSave)
            {
                var res = MessageBox.Show("All you changes will be delete. Continue?", "Attention!", MessageBoxButton.YesNo);
                if (res != MessageBoxResult.Yes)
                {
                    return;
                }
            }
            if (Stcm2l == null || Stcm2l.Actions.Count == 0)
            {
                return;
            }
            var groups = new Dictionary<uint, Group>();
            foreach (var action in Stcm2l.Actions)
            {
                if (groups.TryGetValue(action.OpCode, out var group))
                {
                    group.Actions.Add(action);
                }
                else
                {
                    group = new Group();
                    group.Actions.Add(action);
                    groups.Add(action.OpCode, group);
                }
                group.CharCount += CharCount(action);
            }
            var item = groups.Aggregate((a, b) => a.Value.CharCount < b.Value.CharCount ? b : a);
            var result = MessageBox.Show($" Elements with id {item.Key:X} may be is text. Continue?", "Attention!", MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes)
            {
                return;
            }
            ActionHelpers.ACTION_TEXT = item.Key;
            for (var i = Stcm2l.Actions.IndexOf(item.Value.Actions[0]); i < Stcm2l.Actions.Count; i++)
            {
                var action = Stcm2l.Actions[i];
                if (action.OpCode != item.Key)
                {
                    ActionHelpers.ACTION_DIVIDER = action.OpCode;

                    break;
                }
            }
            OpenFile(Stcm2l.FilePath);
            foreach (MenuItem menu in GamesPresetMenu.Items)
            {
                menu.IsChecked = false;
            }
        }

        private void AddPresetMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var win = new GamePresetWindow();
            win.ShowDialog();
        }
        private void AddChildMenuItem(MenuItem parent, string name, ICommand command, object parameter = null)
        {
            var child = new MenuItem
            {
                Header = name,
                Command = command,
                CommandParameter = parameter
            };
            parent.Items.Add(child);
        }
        private void AddGamePreset(GamePreset preset)
        {
            var menu = new MenuItem();
            menu.SetBinding(MenuItem.HeaderProperty, new Binding(nameof(preset.Name)) { Source = preset });

            AddChildMenuItem(menu, "_Open", WindowCommands.GamePreset, preset);
            AddChildMenuItem(menu, "_Edit", WindowCommands.EditGamePreset, preset);
            AddChildMenuItem(menu, "_Delete", WindowCommands.DeleteGamePreset, preset);
            GamesPresetMenu.Items.Insert(GamesPresetMenu.Items.Count - 2, menu);
        }

        private void AddPresetByCurrent_Click(object sender, RoutedEventArgs e)
        {
            var preset = new GamePreset
            {
                ACTION_NAME = ActionHelpers.ACTION_NAME,
                ACTION_TEXT = ActionHelpers.ACTION_TEXT,
                ACTION_PLACE = ActionHelpers.ACTION_PLACE,
                ACTION_DIVIDER = ActionHelpers.ACTION_DIVIDER,
            };
            var win = new GamePresetWindow(preset);
            win.ShowDialog();
        }

        private void ExportFilesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var win = new SelectPresetWindow();
            if (!(bool)win.ShowDialog())
            {
                return;
            }
            var ofd = new OpenFileDialog
            {
                Multiselect = true
            };
            if (!(bool)ofd.ShowDialog())
            {
                return;
            }

            var fbd = new System.Windows.Forms.FolderBrowserDialog
            {
                ShowNewFolderButton = true
            };
            if (fbd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            var dir = new DirectoryInfo(fbd.SelectedPath);
            if (!dir.Exists)
            {
                dir.Create();
            }
            foreach (var filepath in ofd.FileNames)
            {
                var file = new STCM2L(filepath, win.SelectedPreset);
                file.Load();

                var newpath = Path.Combine(dir.FullName, Path.GetFileNameWithoutExtension(filepath) + ".txt");
                var outfile = new StreamWriter(newpath, false);
                var replics = file.MakeReplics();

                foreach (var replic in replics)
                {
                    var text = string.Join(" ", replic.Lines.Select(x => x.TranslatedText));
                    outfile.WriteLine(text);
                }
                outfile.Close();
            }
        }

        private void ResizeTextBoxCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.OriginalSource is TextBox textBox)
            {
                textBox.MaxLength += 10;
            }
        }

        private void SwitchTextCommand(object sender, ExecutedRoutedEventArgs e)
        {
            foreach (var replic in Replics)
            {
                foreach (var line in replic.Lines)
                {
                    line.TranslatedText = line.OriginalText;
                }
            }
        }

        private void DeleteGamePresetCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (MessageBox.Show("Game preset will be deleted!\nContinue?", "Attenttion!", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }
            var preset = e.Parameter as GamePreset as GamePreset;

            var ind = GamePresetConfigProvider.Instance.Presets.IndexOf(preset);
            GamesPresetMenu.Items.RemoveAt(ind);
            GamePresetConfigProvider.Instance.Presets.Remove(preset);
            preset.Delete();
        }

        private void EditGamePresetCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var win = new GamePresetWindow(e.Parameter as GamePreset);
            win.ShowDialog();
        }

        private void MetroWindow_Drop(object sender, DragEventArgs e)
        {
            base.OnDrop(e);
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var obj = e.Data.GetData(DataFormats.FileDrop);
                var files = obj as string[];
                var path = files[0];
                OpenFile(path);
            }
            e.Handled = true;
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            MainConfig.NormalClose = true;
        }

        private void OpenInActionView_Click(object sender, RoutedEventArgs e)
        {
            var replic = TextsList.SelectedItem as Replic;
            var win = new ActionsView(Stcm2l);
            if (replic.Name != null)
            {
                win.PreLoad = replic.Name;
            }
            else
            {
                win.PreLoad = replic.Lines.First();
            }
            win.Show();
        }

        private void ReplaceDotsCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            foreach(var te in Replics)
            {
                TextAction prev = null;
                foreach(var line in te.Lines)
                {
                    line.TranslatedText = line.TranslatedText.Replace("…","...");
                    line.TranslatedText = line.TranslatedText.Replace("!..","...!");
                    line.TranslatedText = line.TranslatedText.Replace("?..","...?");
                    line.TranslatedText = line.TranslatedText.Replace("≪", "\"");
                    line.TranslatedText = line.TranslatedText.Replace("≫", "\"");
                    line.TranslatedText = line.TranslatedText.Replace("『", "\"");
                    line.TranslatedText = line.TranslatedText.Replace("』", "\"");
                    line.TranslatedText = line.TranslatedText.Replace("канато", "Канато");
                    var eval = new MatchEvaluator(delegate(Match match){
                        var main = match.Groups["main"];
                        var value = match.Value;
                        return value.Substring(0,1) + main + " " + value.Substring(value.Length - 1);
                    });
                    var eval2 = new MatchEvaluator(delegate (Match match)
                    {
                        var main = match.Groups["main"];
                        var value = match.Value;
                        
                        return value.Substring(0, main.Index- match.Index) + "." + main;
                    });
                    line.TranslatedText = Regex.Replace(line.TranslatedText, @"\w(?<main>(\.\.\.)+)\w",eval);
                    line.TranslatedText = Regex.Replace(line.TranslatedText, @"(\w(?<main>(\.\.[\!\?]])))|(\w(?<main>(\.\.))$)|(^(?<main>(\.\.))$)", eval2);

                    if(prev != null && !string.IsNullOrWhiteSpace(line.TranslatedText))
                    {
                        if((new char[] {','}.Contains(prev.TranslatedText.Last()) || char.IsLetter(prev.TranslatedText.Last())) && char.IsUpper(line.TranslatedText.First()))
                        {
                            line.TranslatedText = line.TranslatedText.Substring(0, 1).ToLower() + line.TranslatedText.Substring(1);
                        }
                    }
                    prev = line;
                }
            }
        }

        private void MoveLineCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var txt = e.OriginalSource as TextBox;
            if(!bool.TryParse(e.Parameter as string, out var direction))
            {
                return;
            }
            if (direction)
            {
                ToNextLine(txt);
            }
            else
            {
                ToPrevLine(txt);
            }
        }

        private void ToPrevLine(TextBox txt)
        {
            var str = txt.DataContext as TextAction;
            var te = LinesList.DataContext as Replic;
            var ind = te.Lines.IndexOf(str) - 1;
            var text = " " + txt.Text.Substring(0, txt.CaretIndex).Trim();
            txt.Text = txt.Text.Substring(txt.CaretIndex).Trim();
            if (ind == -1)
            {
                te.AddLine(Stcm2l, 0);
                ind = 0;
            }
            te.Lines[ind].TranslatedText = (te.Lines[ind].TranslatedText + text).Trim();
            try
            {
                FocusTextBox(LinesList, ind, te.Lines[ind].TranslatedText.Length, txt.Name);
            }
            catch
            {

            }
        }
        private void ToNextLine(TextBox txt)
        {
            var str = txt.DataContext as TextAction;
            var te = LinesList.DataContext as Replic;
            var ind = te.Lines.IndexOf(str) + 1;
            var text = txt.Text.Substring(txt.CaretIndex) + " ";
            txt.Text = txt.Text.Substring(0, txt.CaretIndex).Trim();
            if (te.Lines.Count <= ind)
            {
                te.AddLine(Stcm2l);
            }
            te.Lines[ind].TranslatedText = (text + te.Lines[ind].TranslatedText).Trim();
            try
            {
                FocusTextBox(LinesList, ind, text.Length, txt.Name);
            }
            catch
            {

            }
        }

        private void FocusTextBox(ListView list, int ind, int caret, string name)
        {
            var visualItem = list.ItemContainerGenerator.
                ContainerFromItem(list.Items[ind]);
            var listViewItem = visualItem as ListViewItem;

            var myContentPresenter = FindVisualChild<ContentPresenter>(listViewItem);

            var myDataTemplate = myContentPresenter.ContentTemplate;
            var myTextBox = (TextBox)myDataTemplate.FindName(name, myContentPresenter);
            myTextBox.Focus();
            if (myTextBox.CaretIndex != 0)
            {
                return;
            }
            if (caret > myTextBox.Text.Length)
            {
                myTextBox.CaretIndex = myTextBox.Text.Length;
            }
            else
            {
                myTextBox.CaretIndex = caret;
            }
        }
        private childItem FindVisualChild<childItem>(DependencyObject obj)
    where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                {
                    return (childItem)child;
                }
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        private void TextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox txt)
            {
                if (e.XButton2 == MouseButtonState.Pressed && txt.IsFocused)
                {
                    /*if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                    {
                        var temp = txt.DataContext as TextEntity.MyString;
                        var te = LinesList2.DataContext as TextEntity;
                        var ind = te.Lines.IndexOf(temp);
                        if (ind == 0)
                        {
                            if (te.Lines.Count == 1)
                            {
                                e.Handled = true;
                                return;
                            }
                            ind = 1;

                        }
                        ToNextText(te, ind);
                    }
                    else */if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                    {
                        var temp = txt.DataContext as TextAction;
                        var te = LinesList.DataContext as Replic;
                        var ind = te.Lines.IndexOf(temp) + 1;
                        te.AddLine(Stcm2l, ind);
                    }
                    else
                    {
                        ToNextLine(txt);
                    }
                    e.Handled = true;
                }
                else if (e.XButton1 == MouseButtonState.Pressed && txt.IsFocused)
                {
                    /*if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                    {
                        var temp = txt.DataContext as TextEntity.MyString;
                        var te = LinesList2.DataContext as TextEntity;
                        var ind = te.Lines.IndexOf(temp);
                        if (ind == te.Lines.Count - 1)
                        {
                            if (ind == 0)
                            {
                                e.Handled = true;
                                return;
                            }
                            ind--;
                        }
                        ToPrevText(te, ind);
                    }
                    else*/ if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                    {
                        var temp = txt.DataContext as TextAction;
                        var te = LinesList.DataContext as Replic;
                        var ind = te.Lines.IndexOf(temp);
                        te.AddLine(Stcm2l, ind);
                    }
                    else
                    {
                        ToPrevLine(txt);
                    }
                    e.Handled = true;
                }
            }
        }

        private void FindCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var win = new FindDialog(this);
            win.Show();
        }

        public bool find(string text)
        {
            if(Stcm2l == null)
            {
                return false;
            }
            foreach(var replic in Replics)
            {
                if(replic.Lines.Any(x => x.TranslatedText.Contains(text)))
                {
                    TextsList.ScrollIntoView(replic);
                    TextsList.SelectedItem = replic;
                    return true;
                }
            }
            return false;
        }

        private void FindInFilesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var win = new FindInFilesWindow();
            win.ShowDialog();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if(Stcm2l != null)
            {
                Stcm2l.DirectInsert = true;
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if(Stcm2l != null)
            {
                Stcm2l.DirectInsert = false;
            }
        }
    }
}
