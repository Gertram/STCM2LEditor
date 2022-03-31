using System.Windows;

namespace STCM2LEditor
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
