
using STCM2LEditor.classes.Action.Parameters;

namespace STCM2LEditor.classes.Action
{
    internal partial class TextAction : BaseStringAction
    {
        public TextAction() : base() { }
        public TextAction(StringParameter original, StringData translated,int address) : base(original, translated,address)
        {
        }

        public static TextAction ReadFromFile(byte[] file, ref int seek, ActionHeader header = null) => ReadFromFile<TextAction>(file, ref seek, header);

        public override IStringAction Copy()
        {
            return new TextAction(Original.Copy(), Translated.Copy(),Address);
        }

        public override uint OpCode => ActionHelpers.ACTION_TEXT;
    }
}
