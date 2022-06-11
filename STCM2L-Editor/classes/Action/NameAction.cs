
using STCM2LEditor.classes.Action.Parameters;
using System;
using STCM2LEditor.utils;

namespace STCM2LEditor.classes.Action
{
    internal partial class NameAction : BaseStringAction
    {
        public NameAction() : base() { }
        public NameAction(StringParameter original, StringData translated,int address) : base(original, translated,address)
        {
        }
        internal static NameAction ReadFromFile(byte[] file, ref int seek, IGameSettings settings, ActionHeader header = null) => ReadFromFile<NameAction>(file, ref seek,settings, header);

        public override IStringAction Copy()
        {
            return new NameAction(Original.Copy(), Translated.Copy(),Address);
        }

        protected override void WriteParameters(byte[] main, int position)
        {
            try
            {
                if (TranslatedText == null || TranslatedText == "" ||IsReversed)
                    Original.Write(main, ref position);
                else
                {
                    var buf = Original.Copy();
                    buf.Data = Translated;
                    buf.Write(main, ref position);
                }
                ByteUtil.InsertBytes(main, Original.Data.Write(), OriginalDataOffset);
            }
            catch (Exception exp)
            {
                throw exp;
            }
        }
        public override uint OpCode => ActionHelpers.ACTION_NAME;

        public override bool IsTranslated { get => Translated!= null && TranslatedText.Trim().Length > 0; set => base.IsTranslated = value; }
        public override void ReverseStrings()
        {
            if (!string.IsNullOrWhiteSpace(TranslatedText))
            {
                base.ReverseStrings();
            }
        }
    }
}
