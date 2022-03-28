using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Script.Serialization;

namespace STCM2LEditor
{
    class ClassTranslator
    {
        public static string TranslateText(string input)
        {
            // Set the language from/to in the url (or pass it into this function)
            string url = String.Format
            ("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}",
             "ja", "ru", Uri.EscapeUriString(input));
            HttpClient httpClient = new HttpClient();
            string result = httpClient.GetStringAsync(url).Result;

            // Get all json data
            var jsonData = new JavaScriptSerializer().Deserialize<List<dynamic>>(result);

            // Extract just the first array element (This is the only data we are interested in)
            var translationItems = jsonData[0];

            // Translation Data
            string translation = "";
            try
            {
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
            }
            catch (Exception)
            {
                return "";
            }

            // Remove first blank character
            if (translation.Length > 1) { translation = translation.Substring(1); };

            // Return translation
            return translation;
        }
    }
}
