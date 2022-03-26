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

namespace STCM2L
{
    public interface Findable
    {
        bool find(string text);
    }
    /// <summary>
    /// Логика взаимодействия для FindDialog.xaml
    /// </summary>
    public partial class FindDialog : Window
    {
        Findable findable;
        public FindDialog(Findable findable)
        {
            InitializeComponent();
            this.findable = findable;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var text = Findable.Text.Trim();
            if (text.Length == 0)
            {
                return;
            }
            if (!findable.find(text))
            {
                MessageBox.Show("Ничего не нашлось");
            }
        }
    }
}
