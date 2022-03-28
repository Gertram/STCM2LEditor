
using STCM2LEditor.utils;

namespace STCM2LEditor.classes.Action
{
    internal partial class ActionHeader
    {
        public ActionHeader(uint isLocalCall, uint opCode, int parametersCount, int length)
            : this(0, isLocalCall, opCode, parametersCount, length)
        {
        }

        private ActionHeader(int address, uint isLocalCall, uint opCode, int parametersCount, int length)
        {
            Address = address;
            OpCode = opCode;
            IsLocalCall = isLocalCall;
            ParametersCount = parametersCount;
            Length = length;
        }

        public int Address { get; set; }
        public uint OpCode { get; set; }
        public uint IsLocalCall { get; set; }
        public int ParametersCount { get; set; }
        public int Length { get; set; }
        public static ActionHeader ReadFromFile(byte[] file, ref int seek) => new ActionHeader
            (seek,
            ByteUtil.ReadUInt32Ref(file, ref seek),
             ByteUtil.ReadUInt32Ref(file, ref seek),
             ByteUtil.ReadInt32Ref(file, ref seek),
             ByteUtil.ReadInt32Ref(file, ref seek));
        public byte[] Write()
        {
            var main = ByteUtil.InsertUint32(new byte[ActionHelpers.HEADER_LENGTH], IsLocalCall, 0);
            ByteUtil.InsertUint32(main, OpCode, 4);
            ByteUtil.InsertInt32(main, ParametersCount, 8);
            return ByteUtil.InsertInt32(main, Length, 12);
        }
    }
}
