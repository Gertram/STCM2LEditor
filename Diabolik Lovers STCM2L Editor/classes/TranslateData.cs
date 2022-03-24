using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Diabolik_Lovers_STCM2L_Editor.utils;

namespace Diabolik_Lovers_STCM2L_Editor.classes
{
    internal class ActionOriginal
    {
        public MutableAction Action { get; set; }
        public StringData Original { get; set; }
    }
    internal class TranslateData
    {
        protected TranslateData() { }
        public readonly BindingList<ActionOriginal> Actions;

        public TranslateData(string translatedText, IList<ActionOriginal> actions)
        {
            Actions = new BindingList<ActionOriginal>(actions);
            Data = new StringData(translatedText);
        }
        public virtual string OriginalText => Actions[0].Original.Text;
        private StringData Data { get; set; }
        public virtual string TranslatedText { get => Data.Text; set => Data.Text = value; }

        public TranslateData(byte[] file, IReadOnlyList<MutableAction> fileActions, ref int seek)
        {
            Data = new StringData(file, ref seek);

            var count = ByteUtil.ReadInt32Ref(file, ref seek);
            Actions = new BindingList<ActionOriginal>();
            for (var i = 0; i < count; i++)
            {
                var actionAddress = ByteUtil.ReadInt32Ref(file, ref seek);
                var originalAddress = ByteUtil.ReadInt32Ref(file, ref seek);
                var buff = originalAddress;
                var original = new StringData(file, ref buff);
                original.Address = originalAddress;
                bool founded = false;
                foreach (var action in fileActions)
                {
                    if (action.Address == actionAddress)
                    {
                        Actions.Add(new ActionOriginal { Action = action, Original = original });
                        founded = true;
                    }
                }
                if (founded == false)
                {
                    throw new Exception("Не удалось найти действие для перевода");
                }
            }
        }
        public int SetAddress(int startAddress)
        {
            Data.Address = startAddress;

            return startAddress + Data.Length+ActionsLength;
        }
        public void InsertTranslate()
        {
            foreach (var action in Actions)
            {
                if (action.Action.OpCode == ActionHelpers.ACTION_PLACE)
                {
                    (action.Action.Parameters[3] as LocalParameter).ParameterData = Data;
                }
                else if (action.Action.OpCode == ActionHelpers.ACTION_NAME || action.Action.OpCode == ActionHelpers.ACTION_TEXT)
                {
                    (action.Action.Parameters[0] as LocalParameter).ParameterData = Data;
                }
            }
        }
        private int ActionsLength => sizeof(int) * (1 + Actions.Count * 2);
        public byte[] Write()
        {
            var ParameDataBytes = new List<byte>();
            Data.Write(ParameDataBytes);

            var bytes = new byte[ParameDataBytes.Count + ActionsLength];

            int position = 0;
            bytes = ByteUtil.InsertBytesRef(bytes, ParameDataBytes.ToArray(), ref position);
            bytes = ByteUtil.InsertInt32Ref(bytes, Actions.Count, ref position);
            foreach (var action in Actions)
            {
                bytes = ByteUtil.InsertInt32Ref(bytes, action.Action.Address, ref position);
                bytes = ByteUtil.InsertInt32Ref(bytes, action.Original.Address, ref position);
            }
            return bytes;
        }
    }
}
