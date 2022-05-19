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
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using STCM2LEditor.GamePresets;

namespace STCM2LEditor.Wins
{
    public class GamePresetWrap : BasePropertyChanged
    {
        private bool isNotSelected = true;

        internal GamePreset preset { get; private set; }

        public GamePresetWrap(GamePreset preset)
        {
            this.preset = preset;
        }

        public string Name => preset.Name;
        public bool IsNotSelected
        {
            get => isNotSelected; set
            {
                isNotSelected = value;
                Notify(nameof(IsNotSelected));
            }
        }
    }
    /// <summary>
    /// Логика взаимодействия для SelectPresetWindow.xaml
    /// </summary>
    public partial class SelectPresetWindow : Window, INotifyPropertyChanged
    {
        public BindingList<GamePresetWrap> Presets
        {
            get => presets; set
            {
                presets = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Presets)));
            }
        }
        private GamePreset selectedPreset;
        private BindingList<GamePresetWrap> presets;

        public event PropertyChangedEventHandler PropertyChanged;

        public GamePreset SelectedPreset
        {
            get => selectedPreset;
            set
            {
                foreach (var preset in Presets)
                {
                    preset.IsNotSelected = preset.preset != value;
                }
                selectedPreset = value;
            }
        }
        public SelectPresetWindow()
        {
            InitializeComponent();
            Presets = new BindingList<GamePresetWrap>(GamePresetConfigProvider.Instance.Presets.Select(x => new GamePresetWrap(x)).ToList());
            SelectedPreset = GamePresetConfigProvider.Instance.Selected;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var wrap = button.DataContext as GamePresetWrap;
            SelectedPreset = wrap.preset;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
