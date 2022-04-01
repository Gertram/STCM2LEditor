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

namespace STCM2LEditor.Windows
{
    /// <summary>
    /// Логика взаимодействия для GamePrest.xaml
    /// </summary>
    public partial class GamePresetWindow : Window
    {
        private IList<GamePreset> presets = new List<GamePreset>();
        private GamePreset gamePreset = new GamePreset();

        public GamePresetWindow(IList<GamePreset> presets)
        {
            InitializeComponent();
            this.presets = presets;
        }
        private uint GetValue(TextBox txt)
        {
            if (txt.Text == null || txt.Text.Trim() == "")
            {
                return 0;
            }
            return uint.Parse(txt.Text,System.Globalization.NumberStyles.HexNumber);
        }
        public GamePreset GamePreset
        {
            get => gamePreset; 
            set
            {
                TextActionTB.Text = $"{value.TextAction:X}";
                NameActionTB.Text = $"{value.NameAction:X}";
                PlaceActionTB.Text = $"{value.PlaceAction:X}";
                DividerActionTB.Text = $"{value.DividerAction:X}";
                NameTB.Text = value.Name;
                gamePreset = value;
            }
        }
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                NameTB.Text = NameTB.Text.Trim();
                if (NameTB.Text == "")
                {
                    throw new ArgumentNullException();
                }
                if(presets.Any(x=>x.Name == NameTB.Text))
                {
                    throw new ArgumentException();
                }
                GamePreset.Name = NameTB.Text;
                GamePreset.TextAction = GetValue(TextActionTB);
                GamePreset.NameAction = GetValue(NameActionTB);
                GamePreset.DividerAction = GetValue(DividerActionTB);
                GamePreset.PlaceAction = GetValue(PlaceActionTB);
                DialogResult = true;
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("Name must be not empty");
            }
            catch (ArgumentException)
            {
                MessageBox.Show("Preset with this name already exists");
            }
            catch (Exception exp)
            {
                MessageBox.Show("Incorrect number format");
            }
        }
    }
}
