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

using STCM2LEditor.GamePresets;
namespace STCM2LEditor.Wins
{
    /// <summary>
    /// Логика взаимодействия для GamePrest.xaml
    /// </summary>
    public partial class GamePresetWindow : Window
    {

        public GamePresetWindow(GamePreset gamePreset = null)
        {
            InitializeComponent();
            this.gamePreset = gamePreset;
            if(gamePreset != null)
            {
                TextActionTB.Text = $"{gamePreset.ACTION_TEXT:X}";
                NameActionTB.Text = $"{gamePreset.ACTION_NAME:X}";
                PlaceActionTB.Text = $"{gamePreset.ACTION_PLACE:X}";
                DividerActionTB.Text = $"{gamePreset.ACTION_DIVIDER:X}";
                NameTB.Text = gamePreset.Name;
                MaxSymsInLineTB.Text = gamePreset.MaxSymsInLine.ToString();
                EncodingComboBox.SelectedIndex = 0;
                foreach(Encoding encoding in EncodingComboBox.Items)
                {
                    if(encoding.EncodingName == gamePreset.Encoding.EncodingName)
                    {
                        EncodingComboBox.SelectedItem = encoding;
                        break;
                    }
                }
            }
        }
        private uint GetValue(TextBox txt, System.Globalization.NumberStyles style = System.Globalization.NumberStyles.HexNumber)
        {
            if (txt.Text == null || txt.Text.Trim() == "")
            {
                return 0;
            }
            return uint.Parse(txt.Text,style);
        }
        private GamePreset gamePreset = null;
        private GamePreset GetPreset()
        {
            var preset = new GamePreset
            {
                Name = NameTB.Text,
                ACTION_TEXT = GetValue(TextActionTB),
                ACTION_NAME = GetValue(NameActionTB),
                ACTION_DIVIDER = GetValue(DividerActionTB),
                ACTION_PLACE = GetValue(PlaceActionTB),
                MaxSymsInLine = (int)GetValue(MaxSymsInLineTB,System.Globalization.NumberStyles.Integer),
                Encoding = EncodingComboBox.SelectedValue as Encoding
            };
            return preset;
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
                if(gamePreset != null && gamePreset.Name != NameTB.Text && GamePresetConfigProvider.Instance.Presets.Any(x=>x.Name == NameTB.Text))
                {
                    throw new ArgumentException();
                }
                var preset = GetPreset();
               
                if(gamePreset == null)
                {
                    GamePresetConfigProvider.Instance.Presets.Add(preset);
                    gamePreset = preset;
                }
                else
                {
                    gamePreset.Update(preset);
                }
                gamePreset.Save();

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
            catch (Exception)
            {
                MessageBox.Show("Incorrect number format");
            }
        }
    }
}
