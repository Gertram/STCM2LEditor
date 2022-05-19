using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

using STCM2LEditor.Wins;
namespace STCM2LEditor.GamePresets
{
    partial class GamePresetConfig:IGamePresetConfig
    {
        internal GamePresetConfig()
        {
            Presets = new ObservableCollection<GamePreset>();
            var dir = new DirectoryInfo(Environment.CurrentDirectory + "\\GamePresets");
            if (!dir.Exists)
            {
                Selected = null;
                return;
            }
            foreach (var fileInfo in dir.GetFiles())
            {
                if (fileInfo.Extension != ".dat")
                {
                    continue;
                }
                GamePreset preset = GamePreset.Desirealize(fileInfo.FullName);
                if (preset != null)
                {
                    Presets.Add(preset);
                }
            }

            var gamePreset = MainConfig.LastGamePreset;
            if (gamePreset != null && gamePreset != "")
            {
                selected = Presets.FirstOrDefault(x => x.Name == gamePreset);
            }
        }

        public ObservableCollection<GamePreset> Presets { get; private set; }
        private GamePreset selected;

        public event PropertyChangedEventHandler PropertyChanged;

        public GamePreset Selected
        {
            get => selected;
            set
            {
                selected = value;
                PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(nameof(Selected)));
            }
        }
    }
}
