using Microsoft.Win32;
using STCM2LEditor.classes;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;
using STCM2LEditor.utils;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml.Linq;

namespace STCM2LEditor
{
    /// <summary>
    /// Логика взаимодействия для ImportTextWindow.xaml
    /// </summary>
    public partial class ImportWindow : Window
    {
        private MainWindow mainWin;
        public BindingList<TextEntity> TranslatedTexts { get; set; } = new BindingList<TextEntity>();
        public BindingList<Replic> Texts { get; set; }
        public bool ShouldSave { get; private set; } = false;
        internal ImportWindow(MainWindow win)
        {
            InitializeComponent();
            this.mainWin = win;
            Texts = win.Replics;
            TextsList1.DataContext = this;
            TextsList2.DataContext = this;
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
                    var translate= Translator.TranslateText(string.Join(" ", te.Lines.Select(x => x.Text)));
                    return SplitText(translate);
                });
                task.GetAwaiter().OnCompleted(delegate
                {
                    var texts = task.Result;
                    for( int i = 0;i < texts.Count - te.Lines.Count;i++)
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
            if (TextsList2 != null)
                ScrollTo((sender as ListView).SelectedIndex);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (TextsList1.Items.Count != TextsList2.Items.Count)
            {
                if (MessageBox.Show("Количество фрагментов в исходном и конечном текстах не совпадает. Продолжить?", "Внимание", MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    return;
                }
            }
            for (var i = 0; i < mainWin.Replics.Count; i++)
            {
                var fragment = mainWin.Replics[i];
                try
                {
                    var te = TranslatedTexts[i];
                    if (fragment.Lines.Count > te.Lines.Count)
                    {
                        var size = fragment.Lines.Count - te.Lines.Count;
                        for (var j = 0; j < size; j++)
                        {
                            fragment.DeleteLine(mainWin.Stcm2l);
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

                    for (var j = 0; j < fragment.Lines.Count; j++)
                    {
                        fragment.Lines[j].TranslatedText = te.Lines[j].Text;
                    }
                }
                catch (Exception exp)
                {
                    MessageBox.Show(exp.ToString());
                    return;
                }
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
            te.Lines.Add(new TextEntity.MyString { Text = "" });
            if (before)
                TranslatedTexts.Insert(TextsList2.SelectedIndex, te);
            else
                TranslatedTexts.Insert(TextsList2.SelectedIndex + 1, te);
            
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

                    foreach (var line in lines)
                    {
                        text.Lines.Add(new TextEntity.MyString { Text = line.Value });
                        line_num++;
                    }
                    fragment_num++;
                    TranslatedTexts.Add(text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка");
            }
        }
        static char[] splitters =new char[] { ' ' };
        private static List<string> SplitText(string text)
        {
            var list = new List<string>();
            var words = text.Split(splitters);
            var line = "";
            foreach (var word in words)
            {
                if (line.Length + word.Length >= 40)
                {
                    list.Add(line);
                    line = word;
                }
                else
                {
                    line += " " + word;
                }
            }
            if (line.Length > 0)
            {
                list.Add(line);
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
                te.Lines = new BindingList<TextEntity.MyString>(SplitText(text).Select(x => new TextEntity.MyString { Text = x }).ToList());
                TranslatedTexts.Add(te);
            }
        }

        private void TextViewCommand(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {

        }

        private void NewLineCommand(object sender, ExecutedRoutedEventArgs e)
        {
            var txt = e.Parameter as TextBox;
            var str = txt.DataContext as TextEntity.MyString;
            var te = LinesList2.DataContext as TextEntity;
            var ind = te.Lines.IndexOf(str) + 1;
            var text = txt.Text.Substring(txt.CaretIndex) + " ";
            txt.Text = txt.Text.Substring(0, txt.CaretIndex).Trim();
            if (te.Lines.Count <= ind)
            {
                te.AddLine();
            }
            te.Lines[ind].Text = text + te.Lines[ind].Text;
            try
            {
                FocusTextBox(LinesList2, ind, text.Length);
            }
            catch
            {

            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
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
                foreach (var line in reader.ReadLine().Split(new char[] { '|' }))
                {
                    te.Lines.Add(new TextEntity.MyString { Text = line });
                }
                TranslatedTexts.Add(te);
            }
        }

        private void ApplyTranslateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var te = LinesList2.DataContext as TextEntity;
                foreach (var line in te.Lines)
                {
                    line.Text = line.TranslationOption;
                }
            }
            catch
            {

            }
        }
        private void FocusTextBox(ListView list,int ind,int caret)
        {

            var listViewItem = (ListViewItem)(LinesList2.ItemContainerGenerator.
                ContainerFromItem(list.Items[ind]));

            var myContentPresenter = FindVisualChild<ContentPresenter>(listViewItem);

            var myDataTemplate = myContentPresenter.ContentTemplate;
            var myTextBox = (TextBox)myDataTemplate.FindName("textBox", myContentPresenter);
            myTextBox.Focus();
            if(myTextBox.CaretIndex != 0)
            {
                return;
            }
            if(caret > myTextBox.Text.Length)
            {
                myTextBox.CaretIndex = myTextBox.Text.Length;
            }
            else
            {
                myTextBox.CaretIndex = caret;
            }
        }
        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                var textBox = sender as TextBox;
                var ind = (LinesList2.ItemsSource as IList<TextEntity.MyString>).IndexOf(textBox.DataContext as TextEntity.MyString);

                if (e.Key == Key.Down && ind!= LinesList2.Items.Count - 1)
                {
                    FocusTextBox(LinesList2, ind + 1,textBox.CaretIndex);
                    e.Handled = true;
                }
                else if (e.Key == Key.Up && ind != 0)
                {
                    FocusTextBox(LinesList2, ind - 1,textBox.CaretIndex);
                    e.Handled = true;
                }
            }
            catch
            {

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
    }
}
