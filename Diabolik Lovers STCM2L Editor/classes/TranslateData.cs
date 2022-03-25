using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Diabolik_Lovers_STCM2L_Editor.utils;

namespace Diabolik_Lovers_STCM2L_Editor.classes
{
    public class ActionProxy
    {
        public DefaultAction Action { get; set; }
        public ProxyData Proxy { get; set; }
    }
    public class TranslateData
    {
        protected TranslateData() { }
        public BindingList<ActionProxy> Actions { get; set; }
        public Dictionary<int, StringData> ActionsExport { get; set; } = new Dictionary<int, StringData>();

        public TranslateData(StringData data, IList<ActionProxy> actions)
        {
            Actions = new BindingList<ActionProxy>(actions);
            
            Data = data;
        }
        public virtual string OriginalText => Actions[0].Proxy.Original.Text;
        public StringData Data { get; set; }
        public virtual string TranslatedText { get => Data.Text; set => Data.Text = value; }

        public TranslateData(byte[] file, ref int seek)
        {
            Data = new StringData(file, ref seek);

            var count = ByteUtil.ReadInt32Ref(file, ref seek);
            Actions = new BindingList<ActionProxy>();
            for (var i = 0; i < count; i++)
            {
                var actionAddress = ByteUtil.ReadInt32Ref(file, ref seek);
                var originalAddress = ByteUtil.ReadInt32Ref(file, ref seek);
                var buff = originalAddress;
                var original = new StringData(file, ref buff)
                {
                    Address = originalAddress
                };
                ActionsExport.Add(actionAddress,original);
            }
        }
        public int SetAddress(int startAddress)
        {
            Data.Address = startAddress;

            return startAddress + Data.Length+ActionsLength;
        }

        
        private int ActionsLength => sizeof(int) * (1 + Actions.Count * 2);
        public byte[] Write()
        {
            var ParameDataBytes = Data.Write();

            var bytes = new byte[ParameDataBytes.Length + ActionsLength];

            int position = 0;
            bytes = ByteUtil.InsertBytesRef(bytes, ParameDataBytes.ToArray(), ref position);
            bytes = ByteUtil.InsertInt32Ref(bytes, Actions.Count, ref position);
            foreach (var action in Actions)
            {
                bytes = ByteUtil.InsertInt32Ref(bytes, action.Action.Address, ref position);
                bytes = ByteUtil.InsertInt32Ref(bytes, action.Proxy.Original.Address, ref position);
            }
            return bytes;
        }

        public virtual void DeleteFrom(STCM2L file)
        {
            foreach(var item in Actions)
            {
                file.Actions.Remove(item.Action);
            }
            file.Translates.Remove(this);
        }
    }
}
