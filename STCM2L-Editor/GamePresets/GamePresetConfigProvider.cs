using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STCM2LEditor.GamePresets
{
    public static class GamePresetConfigProvider
    {
        public readonly static IGamePresetConfig Instance = new GamePresetConfig();
    }
}
