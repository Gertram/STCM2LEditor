using System.Text;
using System.ComponentModel;
using System.Collections.Generic;
using STCM2LEditor.classes;

namespace STCM2LEditor.utils
{
    public static class EncodingUtil
    {
        //public static Encoding encoding = Encoding.GetEncoding("shift-jis");
        public static IGameEncoding Current { get; set; }
        public static List<Encoding> Encodings = new List<Encoding>
        {
            Encoding.GetEncoding("shift-jis"),
            Encoding.UTF8
        };
    }
}
