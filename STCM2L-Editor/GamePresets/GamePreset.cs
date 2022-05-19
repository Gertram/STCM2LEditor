using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

namespace STCM2LEditor.GamePresets
{
    public partial class GamePreset : BasePropertyChanged, classes.IGameSettings
    {
        public string FileName { get; set; } = null;
        private string name;
        private uint aCTION_TEXT;
        private uint aCTION_NAME;
        private uint aCTION_PLACE;
        private uint aCTION_DIVIDER;
        private int maxSymsInLine = 40;

        public uint MAX_SCREEN_LINES { get; set; } = 14;
        public int MaxSymsInLine
        {
            get => maxSymsInLine; set
            {
                maxSymsInLine = value;
                Notify(nameof(MaxSymsInLine));
            }
        }
        public string Name
        {
            get => name; set
            {
                name = value;
                Notify(nameof(Name));
            }
        }
        public uint ACTION_TEXT
        {
            get => aCTION_TEXT; set
            {
                aCTION_TEXT = value;
                Notify(nameof(ACTION_TEXT));
            }
        }
        public uint ACTION_NAME
        {
            get => aCTION_NAME; set
            {
                aCTION_NAME = value;
                Notify(nameof(ACTION_NAME));
            }
        }
        public uint ACTION_PLACE
        {
            get => aCTION_PLACE; set
            {
                aCTION_PLACE = value;
                Notify(nameof(ACTION_PLACE));
            }
        }
        public uint ACTION_DIVIDER
        {
            get => aCTION_DIVIDER; set
            {
                aCTION_DIVIDER = value;
                Notify(nameof(ACTION_DIVIDER));
            }
        }

        public uint ACTION_CHOICE => classes.Action.ActionHelpers.ACTION_CHOICE;

        public uint ACTION_NEW_PAGE => classes.Action.ActionHelpers.ACTION_NEW_PAGE;

        public uint ACTION_SHOW_PLACE => classes.Action.ActionHelpers.ACTION_SHOW_PLACE;

        public Encoding Encoding { get; set; } = Encoding.GetEncoding("shift-jis");

        public void Update(GamePreset temp)
        {
            Name = temp.Name;
            ACTION_NAME = temp.ACTION_NAME;
            ACTION_TEXT = temp.ACTION_TEXT;
            ACTION_PLACE = temp.ACTION_PLACE;
            ACTION_DIVIDER = temp.ACTION_DIVIDER;
            Encoding = temp.Encoding;
            MaxSymsInLine = temp.MaxSymsInLine;
            /*ACTION_SHOW_PLACE = temp.ACTION_SHOW_PLACE;
            ACTION_NEW_PAGE = temp.ACTION_NEW_PAGE;
            ACTION_CHOICE = temp.ACTION_CHOICE;*/
        }
        class PresetInfo
        {
            public string Name { get; set; }
            public uint ACTION_TEXT { get; set; }
            public uint ACTION_NAME { get; set; }
            public uint ACTION_PLACE { get; set; }
            public uint ACTION_DIVIDER { get; set; }
            public int MAX_SYMS_IN_LINE { get; set; } = 40;

            public int Encoding { get; set; } = -1;
        }
        public static GamePreset Desirealize(string filename)
        {
            var file = JsonDocument.Parse(File.OpenRead(filename));
            var info = file.Deserialize<PresetInfo>();
            var preset = new GamePreset
            {
                ACTION_TEXT = info.ACTION_TEXT,
                ACTION_NAME = info.ACTION_NAME,
                ACTION_PLACE = info.ACTION_PLACE,
                ACTION_DIVIDER = info.ACTION_DIVIDER,
                Name = info.Name,
                FileName = Path.GetFileNameWithoutExtension(filename),
                MaxSymsInLine = info.MAX_SYMS_IN_LINE
            };
            if (info.Encoding != -1)
            {
                preset.Encoding = Encoding.GetEncoding(info.Encoding);
            }

            return preset;
        }
        public void Save()
        {
            var dir = new DirectoryInfo(Environment.CurrentDirectory + "\\GamePresets");
            if (!dir.Exists)
            {
                dir.Create();
            }
            var filename = dir.FullName + "\\" + Name + ".dat";
            if (FileName != null && FileName != Name)
            {
                File.Move(dir.FullName + "\\" + FileName + ".dat", filename);
                FileName = Name;
            }
            var info = new PresetInfo
            {
                Name = Name,
                ACTION_TEXT = ACTION_TEXT,
                ACTION_NAME = ACTION_NAME,
                ACTION_DIVIDER = ACTION_DIVIDER,
                ACTION_PLACE = ACTION_PLACE,
                Encoding = Encoding.CodePage,
                MAX_SYMS_IN_LINE = MaxSymsInLine
            };

            File.WriteAllText(filename, JsonSerializer.Serialize(info));
        }
        public void Delete()
        {
            var dir = new DirectoryInfo(Environment.CurrentDirectory + "\\GamePresets");
            var filename = dir.FullName + "\\" + Name + ".dat";
            File.Delete(filename);
        }
    }
}
