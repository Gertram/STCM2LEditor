
using STCM2LEditor.classes.Action.Parameters;

namespace STCM2LEditor.classes.Action
{
    internal partial class NameAction : BaseStringAction
    {
        public NameAction() : base() { }
        public NameAction(string original, string translated) : base(original, translated)
        {
        }
        public NameAction(StringData original, StringData translated) : base(original, translated)
        {
        }
        public static NameAction ReadFromFile(byte[] file, ref int seek, ActionHeader header = null) => ReadFromFile<NameAction>(file, ref seek, header);
        public override uint OpCode => ActionHelpers.ACTION_NAME;
    }
}
