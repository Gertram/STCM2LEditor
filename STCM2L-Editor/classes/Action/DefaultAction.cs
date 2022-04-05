using STCM2LEditor.classes.Action.Parameters;
using STCM2LEditor.utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace STCM2LEditor.classes.Action
{
    public class DefaultAction : IAction
    {
        private int ParametersLength => Parameters.Sum(x => x.Length);
        private int MainLength => ActionHelpers.HEADER_LENGTH + ParameterHeaderLength;
        private int ParameterHeaderLength => IParameterHelpers.HEADER_LENGTH * Parameters.Count;
        public int Length => MainLength + ExtraDataLength;
        private int ExtraDataLength => ExtraData == null ? LocalParameters.Sum(x => x.Data.Length) : ExtraData.Length;
        public int Address
        {
            get => address;
            set
            {
                foreach (var parameter in LocalParameters)
                {
                    parameter.Data.Address = MainLength + value + parameter.DataSeek;
                }
                address = value;
            }
        }
        private IEnumerable<LocalParameter> LocalParameters => Parameters.Where(x => x is LocalParameter).Select(x => x as LocalParameter);
        public uint OpCode { get; private set; } = 0;
        public IAction LocalCall { get; set; }
        public List<IParameter> Parameters { get; private set; } = new List<IParameter>();
        public byte[] ExtraData { get; private set; }

        IReadOnlyList<IParameter> IAction.Parameters => Parameters;

        private int address = 0;
        public DefaultAction() { }
        public DefaultAction(uint opCode, IAction localCall, int paramCount)
        {
            OpCode = opCode;
            LocalCall = localCall;

            for (int i = 0; i < paramCount; i++)
            {
                Parameters.Add(new Parameter());
            }
        }
        private static IAction ParseAction(byte[] file,int seek,STCM2L stcm2l,ActionHeader header)
        {
            if (header.OpCode == ActionHelpers.ACTION_NAME)
            {
                var name = NameAction.ReadFromFile(file, ref seek, header);
                if (stcm2l.NameActions.TryGetValue(name.OriginalText, out var list))
                {
                    list.Add(name);
                }
                else
                {
                    list = new System.ComponentModel.BindingList<IStringAction> { name };
                    stcm2l.NameActions.Add(name.OriginalText, list);
                }
                if (header.Length != name.Length)
                {
                    throw new Exception("Action length error");
                }
                return name;
            }
            else if (header.OpCode == ActionHelpers.ACTION_TEXT)
            {
                var text = TextAction.ReadFromFile(file, ref seek, header);
                stcm2l.TextActions.Add(text);
                if (header.Length != text.Length)
                {
                    throw new Exception("Action length error");
                }
                return text;
            }

            else if (header.OpCode == ActionHelpers.ACTION_PLACE)
            {
                var place = PlaceAction.ReadFromFile(file, ref seek, header);
                if (stcm2l.PlaceActions.TryGetValue(place.OriginalText, out var list))
                {
                    list.Add(place);
                }
                else
                {
                    list = new System.ComponentModel.BindingList<IStringAction> { place };
                    stcm2l.PlaceActions.Add(place.OriginalText, list);
                }
                if (header.Length != place.Length)
                {
                    throw new Exception("Action length error");
                }
                return place;
            }

            var action = new DefaultAction();
            action.ReadFromFile(seek, file, header);
            return action;
        }
        internal static IAction ReadFromFile(byte[] file, int seek, STCM2L stcm2l)
        {
            var header = ActionHeader.ReadFromFile(file, ref seek);
            var action = ParseAction(file, seek, stcm2l, header);
            if(header.IsLocalCall == 1)
            {
                if(Global.ActionCalls.TryGetValue(header.OpCode,out var list))
                {
                    list.Add(action);
                }
                else
                {
                    list = new List<IAction> { action };
                    Global.ActionCalls.Add(header.OpCode, list);
                }
            }
            return action;
        }
        private ActionHeader Header;
        private void ReadFromFile(int seek, byte[] file, ActionHeader header)
        {
            address = header.Address;
            OpCode = header.OpCode;
            Parameters = new List<IParameter>();
            Header = header;
            var length = header.Length;
            for (int i = 0; i < header.ParametersCount; i++)
            {
                Parameters.Add(ParseParameter(file, ref seek, header.ParametersCount,length));
            }

            var extraDataLength = length - ActionHelpers.HEADER_LENGTH - ParametersLength;
            if (extraDataLength > 0)
            {
                extraDataLength = length - MainLength;
                ExtraData = ByteUtil.ReadBytes(file, extraDataLength, Address + MainLength);
            }
            if (length != Length)
            {
                throw new Exception("Pizda");
            }
        }
        private bool IsLocalParam(uint val,int length)
        {
            return (((val >> 24) & 0xff) != 0xff) && (val > Address) && (val < Address + length);
        }
        private IParameter ParseParameter(byte[] file, ref int seek, int parametersCount,int length)
        {
            var param = new Parameter(file, ref seek);

            if (param.Value1 == 0xffffff41)
            {
                return new GlobalParameter(param);
            }
            else if (IsLocalParam(param.Value1,length))
            {
                int dataAddress = (int)param.Value1;
                var parameter = new LocalParameter(file, param);
                parameter.DataSeek = parameter.Data.Address - Address - ActionHelpers.HEADER_LENGTH - IParameterHelpers.HEADER_LENGTH * parametersCount;
                return parameter;
            }
            return param;
        }
        public byte[] BuildExtraData()
        {
            if (ExtraData != null)
            {
                foreach (var parameter in LocalParameters)
                {
                    var data = parameter.Data.Write();
                    try
                    {
                        ByteUtil.InsertBytes(ExtraData, data, parameter.DataSeek);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
                return ExtraData;
            }
            var extra = new List<byte>();
            foreach (var parameter in LocalParameters)
            {
                extra.AddRange(parameter.Data.Write());
            }
            return extra.ToArray();

        }
        public byte[] Write()
        {
            var position = 0;
            ActionHeader header;

            if (Header != null && Header.Length != Length)
            {
                throw new Exception("Pizda");
            }
            if (LocalCall == null)
            {
                header = new ActionHeader(0, OpCode, Parameters.Count, Length);
            }
            else
            {
                header = new ActionHeader(1, (uint)LocalCall.Address, Parameters.Count, Length);
            }
            if(header.Length == 8)
            {
                Console.WriteLine("WTF");
            }
            var main = ByteUtil.InsertBytesRef(new byte[Length], header.Write(), ref position);


            foreach (var parameter in Parameters)
            {
                parameter.Write(main, ref position);
            }

            return ByteUtil.InsertBytes(main, BuildExtraData(), position);
        }
    }
}
