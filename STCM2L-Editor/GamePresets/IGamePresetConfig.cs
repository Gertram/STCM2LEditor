using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Text;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace STCM2LEditor.GamePresets
{
    public interface IGamePresetConfig:INotifyPropertyChanged
    {
        ObservableCollection<GamePreset> Presets { get;}

        GamePreset Selected { get; set; }
    }
}
