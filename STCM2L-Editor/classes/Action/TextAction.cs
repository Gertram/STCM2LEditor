
using STCM2LEditor.classes.Action.Parameters;
using STCM2LEditor.utils;

namespace STCM2LEditor.classes.Action
{
    public partial class TextAction : BaseStringAction
    {
        public TextAction() : base() { }
        
        public TextAction(StringParameter original, StringData translated, int address) : base(original, translated, address)
        {
        }

        internal static TextAction ReadFromFile(byte[] file, ref int seek, IGameSettings settings, ActionHeader header = null) => ReadFromFile<TextAction>(file, ref seek,settings, header);

        public TextAction TextCopy()
        {
            return new TextAction(Original.Copy(), Translated.Copy(), Address);
        }

        public override IStringAction Copy()
        {
            return new TextAction(Original.Copy(), Translated.Copy(), Address);
        }

        public override void SetTranslateAddress(ref int address)
        {
            if (IsTranslated)
            {
                base.SetTranslateAddress(ref address);
            }
        }
        public override uint OpCode => ActionHelpers.ACTION_TEXT;
    }
}
