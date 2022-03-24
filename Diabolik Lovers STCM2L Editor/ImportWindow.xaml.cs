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
using System.IO;
using System.Linq.Expressions;
using System.Xml;
using Microsoft.Win32;
using System.Xml.Linq;
using System.ComponentModel;

using MahApps.Metro.Controls;
using Diabolik_Lovers_STCM2L_Editor.classes;

namespace Diabolik_Lovers_STCM2L_Editor
{
    /// <summary>
    /// Логика взаимодействия для ImportTextWindow.xaml
    /// </summary>
    public partial class ImportWindow : Window
    {
        private readonly classes.STCM2L stcm2l;
        private readonly BindingList<classes.TextEntity> texts = new BindingList<classes.TextEntity>();
        internal ImportWindow(classes.STCM2L file)
        {
            InitializeComponent();
            stcm2l = file;
            //TextsList1.ItemsSource = file.Texts;
            //TextsList1.DataContext = file.Texts;
        }

        private void OpenFileCommad(object sender, ExecutedRoutedEventArgs e)
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
                texts.Clear();
                foreach (var fragment in fragments)
                {
                    var line_num = 0;
                    var lines = fragment.Elements("line");
                    var text = new classes.TextEntity();

                    foreach (var line in lines)
                    {
                        text.Lines.Add(new classes.Line(line.Value));
                        line_num++;
                    }
                    fragment_num++;
                    texts.Add(text);
                }
                TextsList2.DataContext = texts;
                TextsList2.ItemsSource = texts;

                TextsList1.SelectionChanged += TextsList_SelectionChanged;
                TextsList2.SelectionChanged += TextsList_SelectionChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка");
            }
        }
        private void ScrollTo(int ind,ListView list,ListView lines)
        {
            if(ind >= list.Items.Count)
            {
                ind = list.Items.Count-1;
            }
            const int pad = 10;
            if(list.Items.Count <= ind + pad)
            {
                list.ScrollIntoView(list.Items[ind]);
            }
            else
            {
                list.ScrollIntoView(list.Items[ind+pad]);
            }
            list.SelectedIndex = ind;
            var item = list.Items[ind] as classes.TextEntity;
            lines.DataContext = item;

            Binding binding = new Binding();
            binding.Path = new PropertyPath("Lines");
            binding.Source = item;
            binding.Mode = BindingMode.TwoWay;
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;

            lines.SetBinding(ItemsControl.ItemsSourceProperty, binding);
        }
        private void ScrollTo(int ind)
        {
            if(ind < 0 || ind >= TextsList1.Items.Count)
            {
                return;
            }
            ScrollTo(ind, TextsList2, LinesList2);
            ScrollTo(ind, TextsList1,LinesList1);

            var te = TextsList1.Items[ind] as classes.TextEntity;

            NameBox.DataContext = te;

            if ((bool)AutotranslateCheckbox.IsChecked)
            {
                Autotranslate.Text = ClassTranslator.TranslateText(string.Join("", te.Lines.Select(x => x.LineText)));
            }
        }
        private void TextsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ScrollTo((sender as ListView).SelectedIndex);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (TextsList1.Items.Count != TextsList2.Items.Count)
            {
                if(MessageBox.Show("Количество фрагментов в исходном и конечном текстах не совпадает. Продолжить?","Внимание",MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    return;
                }
            }/*
            var fragments = stcm2l.Texts;
            for(var i = 0;i < fragments.Count; i++)
            {
                var fragment = fragments[i];
                try
                {
                    var te = texts[i];
                    if (fragment.Lines.Count > te.Lines.Count)
                    {
                        var size = fragment.Lines.Count - te.Lines.Count;
                        for (var j = 0; j < size; j++)
                        {
                            fragment.DeleteLine(0);
                        }
                        stcm2l.DeleteLine(i, size);
                    }
                    else if (fragment.Lines.Count < te.Lines.Count)
                    {
                        var size = te.Lines.Count - fragment.Lines.Count;
                        for (var j = 0; j < size; j++)
                        {
                            fragment.AddLine(false, 0);
                        }
                        stcm2l.AddLine(i, size);
                    }

                    for (var j = 0; j < te.Lines.Count; j++)
                    {
                        fragment.Lines[j] = te.Lines[j];
                    }
                }
                catch
                {
                    return;
                }
            }*/
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
            //(LinesList2.DataContext as TextEntity).AddLine(false, index);
        }

        private void DeleteLineClick(object sender, RoutedEventArgs e)
        {
            /*int index = LinesList2.SelectedIndex;
            ((sender as MenuItem).DataContext as TextEntity).DeleteLine(index);*/
        }

        private void DeleteTextClick(object sender, RoutedEventArgs e)
        {
            try
            {
                texts.RemoveAt(TextsList2.SelectedIndex);
            }
            catch
            {

            }
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
            if (TextsList2.SelectedIndex != -1)
            {
                var te = new TextEntity();
                te.Lines.Add(new classes.Line(""));
                if (before)
                    texts.Insert(TextsList2.SelectedIndex, te);
                else
                    texts.Insert(TextsList2.SelectedIndex+1,te);
            }
        }
    }
}
