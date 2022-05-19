using Microsoft.Win32;
using STCM2LEditor.classes;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Input;
using STCM2LEditor.utils;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Media;
using System.Xml.Linq;
using STCM2LEditor.Wins;

using STCM2LEditor.GamePresets;

namespace STCM2LEditor
{
    /// <summary>
    /// Логика взаимодействия для ImportTextWindow.xaml
    /// </summary>
    public partial class ImportWindow : Window, INotifyPropertyChanged
    {
        private MainWindow mainWin;
        public event PropertyChangedEventHandler PropertyChanged;
        private void SendChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        public BindingList<TextEntity> TranslatedTexts { get; set; } = new BindingList<TextEntity>();
        public BindingList<Replic> Replics
        {
            get => replics;
            set
            {
                replics = value;
                SendChanged(nameof(Replics));
            }
        }
        public bool ShouldSave { get; private set; } = true;
        private string FileName;
        internal ImportWindow(MainWindow win, string fileName)
        {
            InitializeComponent();
            this.mainWin = win;
            Replics = win.Replics;
            TextsList1.DataContext = this;
            TextsList2.DataContext = this;
            FileName = fileName;
            Names = new BindingList<NameItem>(MainConfig.Names);
        }
        private void ScrollTo(int ind, ListView list, ListView lines)
        {
            if (list.Items.Count == 0)
            {
                return;
            }
            if (ind < 0)
            {
                ind = 0;
            }

            if (ind >= list.Items.Count)
            {
                ind = list.Items.Count - 1;
            }
            const int pad = 10;
            if (list.Items.Count <= ind + pad)
            {
                list.ScrollIntoView(list.Items[ind]);
            }
            else
            {
                list.ScrollIntoView(list.Items[ind + pad]);
            }
            list.SelectedIndex = ind;
            var item = list.Items[ind] as TextEntity;
            lines.DataContext = item;
        }
        private void ScrollTo(int ind)
        {
            if (ind < 0 || ind >= TextsList1.Items.Count)
            {
                return;
            }
            ScrollTo(ind, TextsList2, LinesList2);
            ScrollTo(ind, TextsList1, LinesList1);

            var replic = TextsList1.Items[ind] as Replic;

            NameBox.DataContext = replic;
            LinesList1.DataContext = replic.Lines;
            LinesList1.ItemsSource = replic.Lines;

            if ((bool)AutotranslateCheckbox.IsChecked)
            {
                Autotranslate.Text = "Loading...";
                var task = Task.Run(delegate
                {
                    return Translator.TranslateText(string.Join("", replic.Lines.Select(x => x.OriginalText)));
                });
                task.GetAwaiter().OnCompleted(delegate
                {
                    Autotranslate.Text = task.Result;
                });
            }
            if ((bool)ImportedTrsnalteCheckbox.IsChecked)
            {
                var te = TranslatedTexts[ind];
                foreach (var line in te.Lines)
                {
                    line.TranslationOption = "Loading...";
                }
                var task = Task.Run(delegate
                {
                    var translate = Translator.TranslateText(string.Join(" ", te.Lines.Select(x => x.Text.Trim(new char[] { ' ', '∴' }))));
                    return SplitText(translate);
                });
                task.GetAwaiter().OnCompleted(delegate
                {
                    var texts = task.Result;
                    for (int i = 0; i < texts.Count - te.Lines.Count; i++)
                    {
                        te.AddLine();
                    }
                    for (int i = 0; i < te.Lines.Count; i++)
                    {
                        if (i < texts.Count)
                        {
                            te.Lines[i].TranslationOption = texts[i];
                        }
                        else
                        {
                            te.Lines[i].TranslationOption = "";
                        }
                    }
                });
            }
        }
        private void TextsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (TextsList2 != null)
                {
                    ScrollTo((sender as ListView).SelectedIndex);
                }
            }
            catch
            {

            }
        }


        private void AddNewLineClick(object sender, RoutedEventArgs e)
        {
            if (LinesList2.DataContext as TextEntity != null)
            {
                InsertLine();
            }
        }

        private void InsertNewLineBeforeClick(object sender, RoutedEventArgs e)
        {
            if (LinesList2.DataContext as TextEntity != null)
            {
                InsertLine(LinesList2.SelectedIndex);
            }
        }

        private void InsertNewLineAfterClick(object sender, RoutedEventArgs e)
        {
            if (LinesList2.DataContext as TextEntity != null)
            {
                InsertLine(LinesList2.SelectedIndex + 1);
            }
        }

        private void InsertLine(int index = -1)
        {
            (LinesList2.DataContext as TextEntity).AddLine("", index);
        }

        private void DeleteLineClick(object sender, RoutedEventArgs e)
        {
            ((sender as MenuItem).DataContext as TextEntity).DeleteLine(LinesList2.SelectedIndex);
        }

        private void DeleteTextClick(object sender, RoutedEventArgs e)
        {
            TranslatedTexts.RemoveAt(TextsList2.SelectedIndex);
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
            if (TextsList2.SelectedIndex == -1)
            {
                return;
            }
            var te = new TextEntity();
            te.PropertyChanged += Te_PropertyChanged;
            te.Lines.Add(new TextEntity.MyString { Text = "" });
            if (before)
                TranslatedTexts.Insert(TextsList2.SelectedIndex, te);
            else
                TranslatedTexts.Insert(TextsList2.SelectedIndex + 1, te);

        }

        private void Te_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            WriteCopy();
        }
        private string BackupPath => Path.Combine(MainConfig.TranslateBackupFile, Path.GetFileNameWithoutExtension(FileName) + ".txt");
        private void WriteCopy()
        {
            var copy = TranslatedTexts.Select(x => x.Lines.Select(z => z.Text).ToList()).ToList();
            Task.Run(delegate
            {
                try
                {
                    var dir = new DirectoryInfo(MainConfig.TranslateBackupFile);
                    if (!dir.Exists)
                    {
                        dir.Create();
                    }
                    using (var writer = new StreamWriter(BackupPath, false))
                    {

                        foreach (var te in copy)
                        {
                            writer.WriteLine(string.Join("|", te));
                        }
                        writer.Close();
                    }
                }
                catch
                {

                }
            });

        }
        private void InsertBeforeClick(object sender, RoutedEventArgs e)
        {
            mainWin.InsertText(TextsList1.SelectedIndex, true);
        }

        private void InsertAfterClick(object sender, RoutedEventArgs e)
        {
            mainWin.InsertText(TextsList1.SelectedIndex, false);
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            mainWin.DeleteText(TextsList1.SelectedIndex);
        }
        private void ImportXMLCommand(object sender, ExecutedRoutedEventArgs e)
        {

            try
            {
                var ofd = new OpenFileDialog
                {
                    Filter = "XML Files (*.xml)|*.xml"
                };
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
                TranslatedTexts.Clear();
                foreach (var fragment in fragments)
                {
                    var line_num = 0;
                    var lines = fragment.Elements("line");
                    var text = new TextEntity();
                    text.PropertyChanged += Te_PropertyChanged;
                    foreach (var line in lines)
                    {
                        text.Lines.Add(new TextEntity.MyString { Text = line.Value });
                        line_num++;
                    }
                    fragment_num++;
                    TranslatedTexts.Add(text);
                }
                ShouldSave = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка");
            }
        }
        static char[] splitters = new char[] { ' ' };
        private BindingList<Replic> replics;
        private static void AddLine(IList<string> list, string line)
        {
            if (list.Count == 0)
            {
                for (int i = 0; i < line.Length; i++)
                {
                    if (char.IsLetter(line[i]))
                    {
                        line = line.Substring(0, i) + char.ToUpper(line[i]) + line.Substring(i + 1);
                        break;
                    }
                }
            }
            list.Add(line);
        }
        private static string HandleName(string word,string name, string suffix)
        {
            var pos = word.IndexOf(name);
            if (pos == -1)
            {
                pos = word.IndexOf(name.ToLower());
            }
            var fullname = $"{name}-{suffix}";
            var pos2 = word.IndexOf(fullname);
            if (pos != -1 && pos2 == -1)
            {
                return word.Substring(0,pos)+fullname + word.Substring(pos+name.Length);
            }

            return word;
        }
        private string HandleNames(string word)
        {
            foreach(var name in Names)
            {
                if (name.IsEnabled)
                {
                    word = HandleName(word, name.Name, name.Suffix);
                }
            }
            return word;/*
            word = HandleNames(word, new string[] { "Аято", "Канато", "Субару" }, "-кун");
            return HandleNames(word, new string[] { "Рейджи", "Шу" }, "-сан");*/
        }
        private string AddDotPrev(string text)
        {
            for(int i = 0;i < text.Length;)
            {
                var pos = text.IndexOf(")",i);
                if(pos == -1)
                {
                    return text;
                }
                if (pos != 0 && !new char[] { '!','?','.'}.Contains(text[pos - 1]) && !char.IsLetter(text[pos - 1]))
                {
                    text = text.Substring(0, pos - 1) + "." + text.Substring(pos);
                };
                i = pos + 1;
            }
            return text;
        }
        private List<string> SplitText(string text)
        {
            var list = new List<string>();

            text = text.Replace(" )", ")");
            text = text.Replace("( ", "(");
            for(int i =0;i < text.Length;)
            {
                var pos = text.IndexOf("…",i);
                if(pos == -1)
                {
                    break;
                }
                if (pos != 0 && (text[pos-1] != ' ' && text[pos-1] != '(') && pos != text.Length - 1 && text[pos+1] != '.' &&  char.IsLetter(text[pos + 1]))
                {
                    text = text.Substring(0, pos+1) + " " + text.Substring(pos+1);
                }
                i = pos + 1;
            }
            for(int i =0;i < text.Length;)
            {
                var pos = text.IndexOf("...",i);
                if(pos == -1)
                {
                    break;
                }
                if (pos != 0 && (text[pos - 1] != ' '&& text[pos - 1] != '(') && pos != text.Length - 3 && text[pos+1] != '.' &&  char.IsLetter(text[pos + 3]))
                {
                    text = text.Substring(0, pos+3) + " " + text.Substring(pos+3);
                }
                i = pos + 3;
            }
            
            var words = text.Split(splitters);
            var line = "";
            foreach (var item in words)
            {
                var word = HandleNames(item);
                word = word.Replace("?..", "...?");
                word = word.Replace("?...", "...?");
                word = word.Replace("!..", "...!");
                word = word.Replace("!...", "...!");
                word = word.Replace("…", "...");
                word = word.Replace("Райто", "Лайто");
                word = word.Replace("Рёджи", "Рейджи");
                word = word.Replace("Пуси-гёрл", "Стервочка");
                //word = AddDotPrev(word);

                if (line.Length + word.Length >= GamePresetConfigProvider.Instance.Selected.MaxSymsInLine)
                {
                    AddLine(list, line);
                    line = word;
                }
                else
                {
                    line += " " + word;
                }
            }
            if (line.Length > 0)
            {
                AddLine(list, line);
            }
            var last = list.Last();
            if(!new char[] {'?','!','.',')',',' }.Contains(last.Last()))
            {
                list[list.Count - 1] = last + ".";
            }
            return list;
        }
        private void ImportTextCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var win = new ImportTextWindow();
            win.Texts = TranslatedTexts.Select(x => string.Join(" ", x.Lines.Select(y => y.Text))).ToList();
            win.ShowDialog();
            TranslatedTexts.Clear();
            foreach (var text in win.Texts)
            {
                var te = new TextEntity();
                te.PropertyChanged += Te_PropertyChanged;
                te.AddRange(SplitText(text));
                TranslatedTexts.Add(te);
            }
        }

        private void TextViewCommand(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(sender is TextBox txt && e.Key == Key.D && txt.SelectionLength > 0
                && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                var text = txt.Text;
                var selected = txt.SelectedText;
                var start = txt.SelectionStart;
                txt.Text = text.Substring(0, start) + selected + text.Substring(start);
                txt.SelectionStart = start;
                txt.SelectionLength = selected.Length;
            }
        }
        private void ToPrevLine(TextBox txt)
        {
            var str = txt.DataContext as TextEntity.MyString;
            var te = LinesList2.DataContext as TextEntity;
            var ind = te.Lines.IndexOf(str)-1;
            var text = " " + txt.Text.Substring(0,txt.CaretIndex).Trim();
            txt.Text = txt.Text.Substring(txt.CaretIndex).Trim();
            if (ind == -1)
            {
                te.AddLine("",0);
                ind = 0;
            }
            if (txt.Name == "textBox")
            {
                te.Lines[ind].Text = te.Lines[ind].Text+ text;
            }
            else
            {
                te.Lines[ind].TranslationOption = te.Lines[ind].TranslationOption + text;
            }
            try
            {
                FocusTextBox(LinesList2, ind, te.Lines[ind].Text.Length, txt.Name);
            }
            catch
            {

            }
        }
        private void ToNextLine(TextBox txt)
        {
            var str = txt.DataContext as TextEntity.MyString;
            var te = LinesList2.DataContext as TextEntity;
            var ind = te.Lines.IndexOf(str) + 1;
            var text = txt.Text.Substring(txt.CaretIndex) + " ";
            txt.Text = txt.Text.Substring(0, txt.CaretIndex).Trim();
            if (te.Lines.Count <= ind)
            {
                te.AddLine();
            }
            if (txt.Name == "textBox")
            {
                te.Lines[ind].Text = text + te.Lines[ind].Text;
            }
            else
            {
                te.Lines[ind].TranslationOption = text + te.Lines[ind].TranslationOption;
            }
            try
            {
                FocusTextBox(LinesList2, ind, text.Length, txt.Name);
            }
            catch
            {

            }
        }
        private void NewLineCommand(object sender, ExecutedRoutedEventArgs e)
        {
            ToNextLine(e.OriginalSource as TextBox);
        }

        private void ImportFormattedTextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            ofd.InitialDirectory = Config.Get("EngTextDirectory");
            if (!(bool)ofd.ShowDialog())
            {
                return;
            }
            var reader = new StreamReader(ofd.FileName);
            TranslatedTexts.Clear();
            while (!reader.EndOfStream)
            {
                var te = new TextEntity();
                te.PropertyChanged += Te_PropertyChanged;
                var temp = reader.ReadLine().Trim();
                var pos = temp.IndexOf(':');
                if (pos < temp.IndexOf(' '))
                {
                    temp = temp.Substring(pos + 1).Trim();
                }
                foreach (var line in temp.Split(new char[] { '|' }))
                {
                    temp = line;
                    for (int i = 0; i < temp.Length; i++)
                    {
                        if (char.IsLetter(temp[i]))
                        {
                            temp = temp.Substring(0, i) + temp.Substring(i, 1).ToUpper() + temp.Substring(i + 1);
                            break;
                        }
                    }
                    te.Lines.Add(new TextEntity.MyString { Text = temp });
                }
                TranslatedTexts.Add(te);
            }
        }
        private void FocusBlock(ListView list, int ind)
        {
            var visualItem = list.ItemContainerGenerator.
               ContainerFromItem(list.Items[ind]);
            var listViewItem = visualItem as ListViewItem;

            var myContentPresenter = FindVisualChild<ContentPresenter>(listViewItem);
            var myDataTemplate = myContentPresenter.ContentTemplate;
            var myTextBlock = (TextBox)myDataTemplate.FindName("wrap", myContentPresenter);
            myTextBlock.Focus();
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

        private void ChangeSelectedCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is string direction)
            {
                if ((direction == "Up" || direction == "Down"))
                {
                    var textBox = e.OriginalSource as TextBox;
                    var list = e.Source as ListView;
                    var ind = (list.ItemsSource as IList<TextEntity.MyString>).IndexOf(textBox.DataContext as TextEntity.MyString);

                    if (direction == "Down" && ind != LinesList2.Items.Count - 1)
                    {
                        FocusTextBox(LinesList2, ind + 1, textBox.CaretIndex, textBox.Name);
                        e.Handled = true;
                    }
                    else if (direction == "Up" && ind != 0)
                    {
                        FocusTextBox(LinesList2, ind - 1, textBox.CaretIndex, textBox.Name);
                        e.Handled = true;
                    }
                }
                else if (direction == "UpReplic" && TextsList2.SelectedIndex > 0)
                {
                    TextsList2.SelectedIndex--;
                    Focus();
                    e.Handled = true;
                }
                else if (direction == "DownReplic" && TextsList2.SelectedIndex + 1 != TextsList2.Items.Count)
                {
                    TextsList2.SelectedIndex++;
                    Focus();
                    e.Handled = true;
                }
                else if ((direction == "Right" || direction == "Left") && e.OriginalSource is TextBox textBox)
                {
                    Console.WriteLine(e.Source is null);
                    textBox.Focus();
                }
            }
        }

        private void QuickTranslateCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (!(bool)ImportedTrsnalteCheckbox.IsChecked)
            {
                return;
            }
            try
            {
                var te = LinesList2.DataContext as TextEntity;
                if (te.Lines.All(x => x.Text.Trim().Length == 0) || te.Lines.All(x => x.Text.Trim() == "Loading..."))
                {
                    return;
                }
                foreach (var line in te.Lines)
                {
                    line.Text = line.TranslationOption.Trim();
                }
            }
            catch
            {

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
        private void root_Closing(object sender, CancelEventArgs e)
        {
            if (!ShouldSave && TranslatedTexts != null && TranslatedTexts.Count != 0)
            {
                return;
            }
            MessageBoxResult saveWarning = ShowSaveWarning();

            switch (saveWarning)
            {
                case MessageBoxResult.Yes:
                    SaveCommand(null, null);
                    break;
                case MessageBoxResult.No:
                    break;
                case MessageBoxResult.Cancel:
                    e.Cancel = true;
                    break;
            }
        }

        private void SaveCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (TextsList1.Items.Count != TextsList2.Items.Count)
            {
                if (MessageBox.Show("Количество фрагментов в исходном и конечном текстах не совпадает. Продолжить?", "Внимание", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    return;
                }
            }
            try
            {
                for (var i = 0; i < mainWin.Replics.Count; i++)
                {
                    var fragment = mainWin.Replics[i];
                    try
                    {
                        var te = TranslatedTexts[i];
                        if (fragment.Lines.Count > te.Lines.Count)
                        {
                            var size = fragment.Lines.Count - te.Lines.Count;
                            for (var j = te.Lines.Count; j < fragment.Lines.Count; j++)
                            {
                                fragment.Lines[j].TranslatedText = "";
                            }
                        }
                        else if (fragment.Lines.Count < te.Lines.Count)
                        {
                            var size = te.Lines.Count - fragment.Lines.Count;
                            for (var j = 0; j < size; j++)
                            {
                                fragment.AddLine(mainWin.Stcm2l);
                            }
                        }

                        for (var j = 0; j < fragment.Lines.Count && j < te.Lines.Count; j++)
                        {
                            fragment.Lines[j].TranslatedText = te.Lines[j].Text;
                        }
                        ShouldSave = false;
                    }
                    catch (Exception exp)
                    {
                        MessageBox.Show(exp.ToString());
                        return;
                    }
                }

                DialogResult = true;
            }
            catch
            {

            }
        }
        private void LoadBackup()
        {
            var reader = new StreamReader(BackupPath);
            TranslatedTexts.Clear();
            while (!reader.EndOfStream)
            {
                var te = new TextEntity(reader.ReadLine().Split(new char[] { '|' }));
                te.PropertyChanged += Te_PropertyChanged;
                TranslatedTexts.Add(te);
            }
            reader.Close();
        }
        private void root_Loaded(object sender, RoutedEventArgs e)
        {
            var backupPath = BackupPath;
            if (backupPath == null)
            {
                MainConfig.TranslateBackupFile = "TranslateBackup";
                backupPath = MainConfig.TranslateBackupFile;
            }
            if (!File.Exists(backupPath))
            {
                return;
            }
            var res = MessageBox.Show("Finded backup file. Do you want to load it?", "Attention", MessageBoxButton.YesNo);
            if (res != MessageBoxResult.Yes)
            {
                return;
            }
            LoadBackup();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {

            try
            {
                var win = new SelectPresetWindow();
                if (!(bool)win.ShowDialog())
                {
                    return;
                }
                var ofd = new OpenFileDialog();
                if (!(bool)ofd.ShowDialog())
                {
                    return;
                }
                var file = new STCM2L(ofd.FileName, win.SelectedPreset);
                if (!file.Load())
                {
                    return;
                }
                var replics = file.MakeReplics();
                TranslatedTexts.Clear();
                foreach (var replic in replics)
                {
                    var te = new TextEntity(replic.Lines.Select(x => x.OriginalText));
                    TranslatedTexts.Add(te);
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show($"An error when reading file {exp}", "Error");
            }
        }
        private void ToPrevText(TextEntity te,int line_num)
        {
            var lines = te.Lines.Take(line_num).ToList();
            var temp = new TextEntity();
            foreach(var line in lines)
            {
                temp.Lines.Add(line);
                te.DeleteLine(0);
            }
            TranslatedTexts.Insert(TranslatedTexts.IndexOf(te), temp);
        }
        private void ToNextText(TextEntity te, int line_ind)
        {
            var lines = te.Lines.Skip(line_ind).ToList();
            var temp = new TextEntity();
            foreach (var line in lines)
            {
                temp.Lines.Add(line);
                te.DeleteLine(line_ind);
            }
            TranslatedTexts.Insert(TranslatedTexts.IndexOf(te)+1, temp);
        }
        private void textBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBox txt)
            {
                if (e.XButton1 == MouseButtonState.Pressed && txt.IsFocused)
                {
                    if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                    {
                        var temp = txt.DataContext as TextEntity.MyString;
                        var te = LinesList2.DataContext as TextEntity;
                        var ind = te.Lines.IndexOf(temp);
                        if(ind == 0)
                        {
                            if(te.Lines.Count == 1)
                            {
                                e.Handled = true;
                                return;
                            }
                                ind = 1;
                            
                        }
                        ToNextText(te,ind);
                    }else if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                    {
                        var temp = txt.DataContext as TextEntity.MyString;
                        var te = LinesList2.DataContext as TextEntity;
                        var ind = te.Lines.IndexOf(temp)+1;
                        te.AddLine("", ind);
                    }
                    else
                    {
                        ToNextLine(txt);
                    }
                    e.Handled = true;
                }
                else if (e.XButton2 == MouseButtonState.Pressed && txt.IsFocused)
                {
                    if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                    {
                        var temp = txt.DataContext as TextEntity.MyString;
                        var te = LinesList2.DataContext as TextEntity;
                        var ind = te.Lines.IndexOf(temp);
                        if (ind == te.Lines.Count-1)
                        {
                            if(ind == 0)
                            {
                                e.Handled = true;
                                return;
                            }
                            ind--;
                        }
                        ToPrevText(te,ind);
                    }
                    else if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
                    {
                        var temp = txt.DataContext as TextEntity.MyString;
                        var te = LinesList2.DataContext as TextEntity;
                        var ind = te.Lines.IndexOf(temp);
                        te.AddLine("", ind);
                    }
                    else
                    {
                        ToPrevLine(txt);
                    }
                    e.Handled = true;
                }
            }
        }
        private BindingList<NameItem> Names { get; set; }
        private void AutoChangeNameMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var win = new NamesAutochangeWindow
            {
                Names = Names
            };
            win.ShowDialog();
            MainConfig.Names = win.Names.ToList();
        }

        private void LinesList2_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(sender is ListView list && e.Key == Key.V && Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (!Clipboard.ContainsText() || list.SelectedIndex == -1)
                {
                    return;
                }

                var text = Clipboard.GetText();

                var lines = SplitText(text);
                var te = new TextEntity(lines);
                foreach(var line in te.Lines)
                {
                    line.PropertyChanged += Te_PropertyChanged;
                }
                if (Keyboard.IsKeyDown(Key.LeftShift))
                {
                    TranslatedTexts.Insert(list.SelectedIndex,te);
                }
                else
                {
                    TranslatedTexts.Insert(list.SelectedIndex+1, te);
                }

                e.Handled = true;
            }
        }
    }
}
