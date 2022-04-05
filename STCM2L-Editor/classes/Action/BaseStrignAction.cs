using STCM2LEditor.classes.Action.Parameters;
using STCM2LEditor.utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace STCM2LEditor.classes.Action
{
    internal abstract class BaseStringAction : BasePropertyChanged,IStringAction
    {
        public StringParameter Original { get; set; }
        private ActionHeader Header { get; set; }
        public StringData Translated { get; set; }
        private int address;
        protected BaseStringAction()
        {
            Original = new StringParameter();
            Translated = new StringData();
        }
        public BaseStringAction(StringData original, StringData translated)
            : this(new StringParameter(original), translated, 0)
        {
        }
        protected BaseStringAction(StringParameter original, StringData translated, int address)
        {
            Original = original;
            Translated = translated;
            
            this.address = address;
        }
        public string OriginalText
        {
            get => Original.Data.Text; 
            set
            {
                Original.Data.Text = value;
                Notify(nameof(OriginalText));
            }
        }
        public string TranslatedText
        {
            get => Translated.Text; set
            {
                Translated.Text = value;
                Notify(nameof(TranslatedText));
            }
        }
        protected static T ReadFromFile<T>(byte[] file, ref int seek, ActionHeader header = null) where T : BaseStringAction, new()
        {
            if (header == null)
            {
                header = ActionHeader.ReadFromFile(file, ref seek);
            }
            var action = new T
            {
                Original = StringParameter.ReadFromFile(file, ref seek),
                address = header.Address,
                Translated = new StringData(),
                Header = header
            };

            action.InsertOriginal(file);

            return action;
        }
        protected void InsertOriginal(byte[] file)
        {
            var expectedAddress = Address + OriginalDataOffset;

            if (Original.Data.Address != expectedAddress)
            {
                var originalData = StringData.ReadFromFile(file, expectedAddress);
                Translated = Original.Data;
                Original.Data = originalData;
            }
        }
        public abstract uint OpCode { get; }
        public IAction LocalCall { get; set; }
        protected virtual int ParametersOffset => IParameterHelpers.HEADER_LENGTH;
        protected virtual int OriginalDataOffset => ActionHelpers.HEADER_LENGTH + ParametersOffset;
        public virtual int Address
        {
            get => address; set
            {
                Original.Data.Address = value + OriginalDataOffset;
                address = value;
            }
        }

        public int Length => ActionHelpers.HEADER_LENGTH + Parameters.Sum(x => x.Length);

        public virtual IReadOnlyList<IParameter> Parameters => new List<IParameter> { Original };

        public virtual byte[] ExtraData => Original.Data.ExtraData;

        protected virtual void WriteParameters(byte[] main, int position)
        {
            try
            {
                if (TranslatedText == "")
                    Original.Write(main, ref position);
                else
                {
                    var buf = Original.Copy();
                    buf.Data = Translated;
                    buf.Write(main, ref position);
                }
                ByteUtil.InsertBytes(main, Original.Data.Write(), OriginalDataOffset);
            }catch(Exception exp)
            {
                throw exp;
            }
        }
        public byte[] Write()
        {
            var header = new ActionHeader((uint)(LocalCall ==null?0:1), OpCode, Parameters.Count, Length);
            if (header.Length != header.Length)
            {
                throw new Exception();
            }
            var position = 0;
            var main = ByteUtil.InsertBytesRef(new byte[Length], header.Write(), ref position);
            WriteParameters(main, position);
            return main;
        }
        public byte[] WriteTranslate()
        {
            if (Translated.Text != "")
                return Translated.Write();
            return new byte[0];
        }

        public void SetTranslateAddress(ref int address)
        {
            if (Translated.Text != "")
            {
                Translated.Address = address;
                address += Translated.Length;
            }
        }

        public abstract IStringAction Copy();
    }
}
