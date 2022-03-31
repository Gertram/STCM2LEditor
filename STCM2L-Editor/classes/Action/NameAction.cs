
using STCM2LEditor.classes.Action.Parameters;

namespace STCM2LEditor.classes.Action
{
    internal partial class NameAction : BaseStringAction
    {
        public NameAction() : base() { }
        public NameAction(StringParameter original, StringData translated,int address) : base(original, translated,address)
        {
        }
        public static NameAction ReadFromFile(byte[] file, ref int seek, ActionHeader header = null) => ReadFromFile<NameAction>(file, ref seek, header);

        public override IStringAction Copy()
        {
            return new NameAction(Original.Copy(), Translated.Copy(),Address);
        }

        public override uint OpCode => ActionHelpers.ACTION_NAME;
    }
}
