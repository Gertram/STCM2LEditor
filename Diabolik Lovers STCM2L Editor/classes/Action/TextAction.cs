
using STCM2LEditor.classes.Action.Parameters;

namespace STCM2LEditor.classes.Action
{
    internal partial class TextAction : BaseStringAction
    {
        public TextAction() : base() { }
        public TextAction(string original, string translated) : base(original, translated)
        {
        }
        public TextAction(StringData original, StringData translated) : base(original, translated)
        {
        }

        public static TextAction ReadFromFile(byte[] file, ref int seek, ActionHeader header = null) => ReadFromFile<TextAction>(file, ref seek, header);

        public override uint OpCode => ActionHelpers.ACTION_TEXT;
    }
}
