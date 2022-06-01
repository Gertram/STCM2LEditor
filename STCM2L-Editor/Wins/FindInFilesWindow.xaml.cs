using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Windows;
using Forms = System.Windows.Forms;

namespace STCM2LEditor.Wins
{
    /// <summary>
    /// Логика взаимодействия для FindInFilesWindow.xaml
    /// </summary>
    public partial class FindInFilesWindow : Window
    {
        public FindInFilesWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var text = FinableTextBox.Text;
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }
            var path = MainConfig.WorkDirectory.Value;
            var win = new Forms.FolderBrowserDialog
            {
                RootFolder = Environment.SpecialFolder.Desktop
            };
            win.SelectedPath = path;
            if (win.ShowDialog() != Forms.DialogResult.OK)
            {
                return;
            }
            path = win.SelectedPath;
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }
            
            var dir = new DirectoryInfo(path);
            if (!dir.Exists)
            {
                return;
            }
            var tasks = new List<Task>();
            var files = new BindingList<string>();
            ResultList.DataContext = files;
            ResultList.ItemsSource = files;
            foreach(var filepath in dir.GetFiles())
            {
                var task = Task.Run(delegate ()
                {
                    var file = new classes.STCM2L(filepath.FullName, GamePresets.GamePresetConfigProvider.Instance.Selected);
                    try
                    {
                        file.Load();
                        var replics = file.MakeReplics();
                        return replics.Any(x => x.Lines.Any(y => y.TranslatedText.IndexOf(text, StringComparison.OrdinalIgnoreCase) != -1 || y.OriginalText.IndexOf(text, StringComparison.OrdinalIgnoreCase) != -1));
                    }
                    catch
                    {
                        return false;
                    }
                });
                task.GetAwaiter().OnCompleted(delegate ()
                {
                    if(task.Result){
                        files.Add(filepath.FullName);
                        return;
                    }
                });
            }
            Task.WaitAll(tasks.ToArray());
            MessageBox.Show("Search complete");
        }
    }
}
