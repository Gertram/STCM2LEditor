using Diabolik_Lovers_STCM2L_Editor.utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.ComponentModel;

namespace Diabolik_Lovers_STCM2L_Editor.classes {
    public class STCM2L {
        public const int HEADER_OFFSET = 0x20;
        public const int EXPORT_SIZE = 0x28;
        public const int COLLECTION_LINK_PADDING = 0x38;

        public string FilePath { get; set; }
        public byte[] OriginalFile { get; set; }
        public List<byte> NewFile { get; set; }
        public int StartPosition { get; set; }
        public byte[] StartData { get; set; }

        public int ExportsPosition { get; set; }
        public int ExportsCount { get; set; }
        public List<Export> Exports { get; set; }

        public int CollectionLinkPosition { get; set; }
        public int CollectionLinkOldAddress { get; set; }

        public BindingList<IAction> Actions { get; set; }

        public List<DefaultAction> DefaultActions => Actions.Where(x => x is DefaultAction).Select(x => x as DefaultAction).ToList();

        public ObservableCollection<TranslateData> Translates { get; private set; }
        public STCM2L (string filePath) {
            FilePath = filePath;
            Exports = new List<Export>();
            Actions = new BindingList<IAction>();
            NewFile = new List<byte>();
            Translates = new ObservableCollection<TranslateData>();
        }
        
        public bool Load() {
            try {
                OriginalFile = File.ReadAllBytes(FilePath);
                StartPosition = FindStart();
#if DEBUG
                Console.WriteLine("Start at: 0x{0:X}", StartPosition);
#endif
                if (StartPosition == 0) {
                    return false;
                }

                ReadStartData();
                ReadCollectionLink();

                ReadExports();

                ReadTranslates();
                ReadActions();


                //MakeEntities();

                return true;
            }
            catch (Exception e)
            {
#if DEBUG
                Console.WriteLine(e);
#endif
                return false;
            }
        }
        
        public int FileSize()
        {
            int newExportsAddress = StartPosition + GetActionsLength() + 12;
            int newCollectionLinkAddress = newExportsAddress + ExportsCount * EXPORT_SIZE + 16;
            return newCollectionLinkAddress + 64;
        }
        public bool Save(string filePath) {
            NewFile.Clear();
                /* foreach(TextEntity text in Texts) {
                     text.ReinsertLines();
                 }*/

                int newExportsAddress = StartPosition + GetActionsLength() + 12; // + EXPORT_DATA.Length
                int newCollectionLinkAddress = newExportsAddress + ExportsCount * EXPORT_SIZE + 16; // + COLLECTION_LINK.length
                var extraDataAddress = newCollectionLinkAddress + 64;
                SetAddresses(extraDataAddress);
                GlobalStatus.Inserting = true;
                StartData = ByteUtil.InsertInt32(StartData, newExportsAddress, HEADER_OFFSET);
                StartData = ByteUtil.InsertInt32(StartData, newCollectionLinkAddress, HEADER_OFFSET + 3 * 4);

                NewFile.AddRange(StartData);

                WriteActions();
                WriteExports();
                WriteCollectionLink();

                GlobalStatus.Inserting = false;
/*
                for (int i = 0; i < OriginalFile.Length; i++)
                {
                    if (OriginalFile[i] != NewFile[i])
                    {
                        throw new Exception();
                    }
                }*/

                File.WriteAllBytes(filePath, NewFile.ToArray());
                return true;
        }

        private void WriteActions() {
            
            foreach(var action in Actions) {
                NewFile.AddRange(action.Write());
            }
        }

        private void SetAddresses(int extraDataAddress) {
            int address = StartPosition;


            foreach (var action in Actions) {
                action.Address = address;
                address += action.Length;
            }

            foreach (var translate in Translates)
            {
                if (translate.TranslatedText != null && translate.TranslatedText.Trim() != "")
                {
                    extraDataAddress = translate.SetAddress(extraDataAddress);
                }
            }
        }

        private void WriteExports() {
            NewFile.AddRange(EncodingUtil.encoding.GetBytes("EXPORT_DATA"));
            NewFile.Add(new byte());

            foreach (Export export in Exports) {
                NewFile.AddRange(export.Write());
            }
        }

        private void WriteCollectionLink() {
            NewFile.AddRange(EncodingUtil.encoding.GetBytes("COLLECTION_LINK"));
            NewFile.Add(new byte());
            NewFile.AddRange(BitConverter.GetBytes(0));

            var extra = new List<byte[]>();
            foreach (var translate in Translates)
            {
                if (translate.TranslatedText != null && translate.TranslatedText.Trim() != "")
                {
                    extra.Add(translate.Write());
                }
            }

            var sum = (uint)extra.Sum(x => x.Length);

            UInt32 newCollectionLinkAddress = (UInt32) NewFile.Count + 4 + COLLECTION_LINK_PADDING+sum;

            NewFile.AddRange(BitConverter.GetBytes(newCollectionLinkAddress));
            NewFile.AddRange(new byte[COLLECTION_LINK_PADDING]);
            foreach(var data in extra)
            {
                NewFile.AddRange(data);
            }
        }

        private int GetActionsLength() {
            int length = 0;

            foreach(var action in Actions) {
                length += action.Length;
            }

            return length;
        }

        private void ReadStartData () {
            int seek = 0;
            StartData = ByteUtil.ReadBytesRef(OriginalFile, StartPosition, ref seek);

            seek = HEADER_OFFSET;
            ExportsPosition = ByteUtil.ReadInt32Ref(StartData, ref seek);

            seek += 2 * 4;
            CollectionLinkPosition = ByteUtil.ReadInt32Ref(StartData, ref seek);
        }

        private int FindStart() {
            byte[] start = EncodingUtil.encoding.GetBytes("CODE_START_");

            for (int i = 0; i < 2000; i++) {
                if (OriginalFile[i] == start[0]) {
                    for (int j = 0; j < start.Length; j++) {
                        if (OriginalFile[i + j] != start[j]) {
                            break;
                        }
                        else if (j + 1 == start.Length) {
                            return  i + 0x0c;
                        }
                    }
                }
            }

            return 0;
        }

        private void ReadCollectionLink() {
            int seek = (int)CollectionLinkPosition + 4;
            CollectionLinkOldAddress = ByteUtil.ReadInt32(OriginalFile, seek);
        }

        private void ReadExports () {
            int exportsLength = CollectionLinkPosition - ExportsPosition;
            ExportsCount = exportsLength / EXPORT_SIZE;

            int seek = (int)ExportsPosition;

            for (int i = 0; i < ExportsCount; i++) {
                Export export = new Export();

                seek += 4;

                export.Name = EncodingUtil.encoding.GetString(ByteUtil.ReadBytesRef(OriginalFile, 32, ref seek));
                export.OldAddress = ByteUtil.ReadUInt32Ref(OriginalFile, ref seek);

                Exports.Add(export);
            }
        }
        private void ReadTranslates()
        {
            var seek = (int)(CollectionLinkPosition + 64);
            Translates = new ObservableCollection<TranslateData>();
            while(seek != CollectionLinkOldAddress)
            {
                Translates.Add(new TranslateData(OriginalFile,ref seek));
            }
            /*foreach(var translate in Translates)
            {
                foreach(var item in translate.Actions)
                {
                    if (item.Action.OpCode == ActionHelpers.ACTION_PLACE) {
                        (item.Action.Parameters[3] as LocalParameter).ParameterData = item.Original;
                    }
                    else if (item.Action.OpCode == ActionHelpers.ACTION_NAME || item.Action.OpCode == ActionHelpers.ACTION_TEXT) {
                        (item.Action.Parameters[0] as LocalParameter).ParameterData = item.Original;
                    }
                }
            }*/
        }
        private bool IsDefaultAction(IAction action)
        {
            return action.OpCode == ActionHelpers.ACTION_PLACE
                || action.OpCode == ActionHelpers.ACTION_NAME
                || action.OpCode == ActionHelpers.ACTION_TEXT;
        }
        private void ReadActions () {
            int currentAddress = StartPosition;
            int maxAddress = ExportsPosition - 12; // Before EXPORT_DATA
            int currentExport = 0;
            int i = 0;

            do {
                var action = new DefaultAction();
                i++;
                var founded = false;
                foreach(var translate in Translates){
                    foreach(var actionExp in translate.ActionsExport)
                    {
                        if(actionExp.Key == currentAddress)
                        {
                            founded = true;
                            var proxy = new ProxyData(actionExp.Value, translate.Data);
                            action.ReadFromFile(currentAddress, OriginalFile,proxy);
                            translate.Actions.Add(new ActionProxy { Action = action, Proxy = proxy});
                        }
                    }
                }
                if (!founded)
                {
                    action.ReadFromFile(currentAddress, OriginalFile);
                }

                if(currentExport < Exports.Count && Exports[currentExport].OldAddress == currentAddress) {
                    Exports[currentExport].ExportedAction = action;
                    currentExport++;
                }

                currentAddress += action.Length;

                 Actions.Add(action);
            }
            while (currentAddress < maxAddress);

            foreach(var translate in Translates)
            {
                foreach (var export in translate.ActionsExport)
                {
                    bool founded = false;
                    foreach(var action in translate.Actions)
                    {
                        if(action.Action.Address == export.Key)
                        {
                            founded = true;
                            break;
                        }
                    }
                    if (!founded)
                    {
                        throw new Exception("Не все действия были найдены");
                    }
                }
                translate.ActionsExport = null;
            }
            RecoverGlobalCalls();
#if DEBUG
            Console.WriteLine("Found {0} actions.", Actions.Count);
#endif
        }
        
        private void RecoverGlobalCalls () {

            foreach(var action in Actions) {
                if (Global.Calls.ContainsKey((uint)action.Address))
                {
                    var list = Global.Calls[(uint)action.Address];

                    foreach (var parameter in list)
                    {
                        parameter.GlobalPointer = action;
                    }
                   /* Console.WriteLine($"action in {currentAddress:X}");
                    for (var i = 0; i < action.ParameterCount; i++)
                    {
                        Console.Write($"par{i}={action.Parameters[i].Text}");
                        try
                        {
                            Console.WriteLine($"par{i}={action.GetStringFromParameter(i)}");
                        }
                        catch { }
                    }*/
                }
            }
        }
/*
        private void MakeEntities() {
            for (int i = 0; i < Actions.Count; i++) { 
                if (
                    (Actions[i].OpCode == GeneralAction.ACTION_NAME || Actions[i].OpCode == GeneralAction.ACTION_TEXT || Actions[i].OpCode == GeneralAction.ACTION_PLACE) &&
                    Actions[i].ExtraDataLength > 0
                ) {
                    TextEntity textEntity = new TextEntity();
                    textEntity.SetConversation(ref i, Actions);

                    Texts.Add(textEntity);
                }
                else if (Actions[i].OpCode == GeneralAction.ACTION_CHOICE) {
                    TextEntity textEntity = new TextEntity();
                    textEntity.SetAnswer(ref i, Actions);
                    Texts.Add(textEntity);
                }
            }
#if DEBUG
            Console.WriteLine("Read {0} texts.", Texts.Count);
#endif
        }*/

/*
        public void DeleteText(int index) {
            Texts[index].DeleteText();
            DeleteLine(index, Texts[index].AmountInserted);
            Texts.Remove(Texts[index]);
        }

        public void AddLine(int index, int amount) {
            for(int i = index; i < Texts.Count; i++) {
                Texts[i].ActionsEnd += amount;
            }
        }

        public void DeleteLine(int index, int amount) {
            for(int i = index; i < Texts.Count; i++) {
                Texts[i].ActionsEnd -= amount;
            }
        }*/

    }
}
