using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Script.Serialization;
using System.Linq;
using System.Text.RegularExpressions;

using STCM2LEditor.utils;
namespace STCM2LEditor
{
    internal static partial class Translator
    {
        static Translator()
        {
            var language = Config.Get("Auto-translate-language");
            if (language != null)
            {
                translateLanguage = int.Parse(language);
            }
        }
        public static IReadOnlyList<string> Languages = new List<string>
        {
            "en","ru"
        };
        private static int translateLanguage = 1;

        public static int TranslateLanguage
        {
            get => translateLanguage;
            set
            {
                if(value < 0 || value >= Languages.Count)
                {
                    throw new ArgumentOutOfRangeException("translateLanguage");
                }
                Config.Set("Auto-translate-language", value.ToString());
                translateLanguage = value;
            }
        }
        public static string TranslateText(string input)
        {
            if(input == "レイジ")
            {
                return "Рейджи";
            }
            if(input == "ライト")
            {
                return "Лайто";
            }
            try
            {
                input = Regex.Replace(input, @"#KW_.+\[\]", "");
                input = Regex.Replace(input, @"#Name\[1\]", "");
                input = Regex.Replace(input, @"#Name\[2\]", "ゆい");
                // Set the language from/to in the url (or pass it into this function)
                string url = String.Format
                 ("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}",
                  "ja", Languages[TranslateLanguage], Uri.EscapeUriString(input));
                HttpClient httpClient = new HttpClient();
                string result = httpClient.GetStringAsync(url).Result;

                // Get all json data
                var jsonData = new JavaScriptSerializer().Deserialize<List<dynamic>>(result);

                // Extract just the first array element (This is the only data we are interested in)
                var translationItems = jsonData[0];
                if (translationItems == null)
                {
                    return "Wasn't translate";
                }

                // Translation Data
                string translation = "";
                // Loop through the collection extracting the translated objects
                foreach (object item in translationItems)
                {
                    // Convert the item array to IEnumerable
                    IEnumerable translationLineObject = item as IEnumerable;

                    // Convert the IEnumerable translationLineObject to a IEnumerator
                    IEnumerator translationLineString = translationLineObject.GetEnumerator();

                    // Get first object in IEnumerator
                    translationLineString.MoveNext();

                    // Save its value (translated text)
                    translation += string.Format(" {0}", Convert.ToString(translationLineString.Current));
                }


                // Remove first blank character
                if (translation.Length > 1) { translation = translation.Substring(1); };

                // Return translation
                return translation;
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
