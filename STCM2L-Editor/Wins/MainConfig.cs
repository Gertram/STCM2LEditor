using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using STCM2LEditor.utils;

using System.Globalization;

namespace STCM2LEditor.Wins
{
    public interface ILanguage
    {
        string Name { get; }
        string Code { get; }
    }
    class Language : ILanguage
    {
        public string Name => "Русский";
        public string Code => "ru-ru";
    }
    public class BoolValue
    {
        public bool Value
        {

            get => bool.Parse(Config.Get("AutotranslateImport") ?? "false");
            set => Config.Set("AutotranslateImport", value.ToString());
        }
    }
    public static partial class MainConfig
    {
        static MainConfig()
        {

        }
        public static ILanguage Language => new Language();
        public static string LastFile
        {
            get => Config.Get("lastFile");
            set => Config.Set("lastFile", value);
        }
        public static string LastGamePreset
        {
            get => Config.Get("LastGamePreset");
            set => Config.Set("LastGamePreset", value);
        }
        public static string WorkDirectory
        {
            get => Config.Get("WorkDirectory");
            set => Config.Set("WorkDirectory", value);
        }
        public static BoolValue AutotranslateImport => new BoolValue();
        public static string EngTextDirectory
        {
            get => Config.Get("EngTextDirectory");
            set => Config.Set("EngTextDirectory", value);
        }
        public static string TranslateBackupFile
        {
            get => Config.Get("TranslateBackup");
            set => Config.Set("TranslateBackup", value);
        }
        public class NameEntries
        {
            public List<NameItem> Names { get; set; }
        }
        public static List<NameItem> Names
        {
            get
            {
                var str = Config.Get("Names");
                if(str == null)
                {
                    return new List<NameItem>();
                }
                try
                {
                    var file = JsonDocument.Parse(str);
                    var entries = file.Deserialize<NameEntries>();
                    if(entries == null)
                    {
                        return new List<NameItem>();
                    }
                    return entries.Names;
                }
                catch
                {
                    return new List<NameItem>();
                }
            }
            set
            {
                var enties = new NameEntries
                {
                    Names = value
                };
                var str = JsonSerializer.Serialize(enties);
                Config.Set("Names", str);
            }
        }
        public static bool NormalClose
        {
            get => bool.Parse(Config.Get("NormalClose") ?? "false");
            set => Config.Set("NormalClose",value.ToString());
        }
    }
}
