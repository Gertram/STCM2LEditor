namespace STCM2LEditor.classes
{
    public class NullString : IString
    {
        public string OriginalText { get => ""; set { } }
        public string TranslatedText { get => ""; set { } }
    }
}
