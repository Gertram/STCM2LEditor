using STCM2LEditor.classes.Action.Parameters;
using STCM2LEditor.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace STCM2LEditor.classes.Action
{
    class NullGameEncoding : IGameEncoding
    {
        public Encoding Encoding => throw new NotImplementedException();
    }
    public abstract class BaseStringAction : BasePropertyChanged, IStringAction
    {
        public StringParameter Original { get; set; }
        private ActionHeader Header { get; set; }
        public StringData Translated { get; set; }
        private int address;
        protected BaseStringAction()
        {
            Original = new StringParameter(new NullGameEncoding());
            Translated = new StringData(new NullGameEncoding());
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
        private static bool IsLocalParam(uint val, int length, int address)
        {
            return (((val >> 24) & 0xff) != 0xff) && (val > address);
        }
        protected static T ReadFromFile<T>(byte[] file, ref int seek, IGameSettings settings, ActionHeader header = null) where T : BaseStringAction, new()
        {
            if (header == null)
            {
                header = ActionHeader.ReadFromFile(file, ref seek);
            }
            if (header.Length <= ActionHelpers.HEADER_LENGTH + IParameterHelpers.HEADER_LENGTH)
            {
                return null;
            }
            var param = Parameter.ReadFromFile(file, ref seek);
            if (!IsLocalParam(param.Value1, header.Length, header.Address))
            {
                return null;
            }
            var parameter = LocalParameter.ReadFromFile(file, param);
            if(parameter == null)
            {
                return null;
            }
            var original = new StringParameter(parameter,settings);
            if (original == null)
            {
                return null;
            }
            var action = new T
            {
                Original = original,
                address = header.Address,
                Translated = new StringData(settings),
                Header = header
            };

            action.InsertOriginal(file,settings);

            return action;
        }
        protected void InsertOriginal(byte[] file,IGameEncoding encoding)
        {
            var expectedAddress = Address + OriginalDataOffset;

            if (Original.Data.Address != expectedAddress)
            {
                var originalData = StringData.ReadFromFile(file, expectedAddress,encoding);
                if (originalData == null)
                {
                    return;
                }
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

        public virtual bool IsTranslated { get; set; }

        protected virtual void WriteParameters(byte[] main, int position)
        {
            try
            {
                if (!IsTranslated || IsReversed)
                {
                    Original.Write(main, ref position);
                }
                else
                {
                    var buf = Original.Copy();
                    buf.Data = Translated;
                    buf.Write(main, ref position);
                }
                ByteUtil.InsertBytes(main, Original.Data.Write(), OriginalDataOffset);
            }
            catch (System.Exception exp)
            {
                throw exp;
            }
        }
        public byte[] Write()
        {
            var header = new ActionHeader((uint)(LocalCall == null ? 0 : 1), OpCode, Parameters.Count, Length);
            if (header.Length != header.Length)
            {
                throw new Exception();
            }
            var position = 0;
            var main = ByteUtil.InsertBytesRef(new byte[Length], header.Write(), ref position);
            WriteParameters(main, position);
            return main;
        }
        public virtual byte[] WriteTranslate()
        {
            if (!IsTranslated) return new byte[0];
            return Translated.Write();
        }

        public virtual void SetTranslateAddress(ref int address)
        {
            if (IsTranslated)
            {
                Translated.Address = address;
                address += Translated.Length;
            }
        }

        public abstract IStringAction Copy();
        protected bool IsReversed { get; set; } = false;

        public virtual void ReverseStrings()
        {
            var temp = Original.Data;
            Original.Data = Translated;
            Translated = temp;
            IsReversed = !IsReversed;
        }
    }
}
