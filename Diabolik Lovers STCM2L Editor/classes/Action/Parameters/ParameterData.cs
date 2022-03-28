using STCM2LEditor.utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace STCM2LEditor.classes.Action.Parameters
{
    public partial class ParameterData : IParameterData
    {
        private int address;

        public int Address
        {
            get => address;
            set
            {
                address = value;
            }
        }
        public IReadOnlyList<byte> ExtraData { get; set; }
        public uint Type { get; private set; }
        public uint Value { get; private set; }
        public int Length => ParameterDataHelpers.HEADER_LENGTH + AlignedLength;

        public int DataLength => ExtraData.Count;

        public virtual int AlignedLength => ExtraData.Count;

        internal ParameterData(uint dataType, uint dataVal3, byte[] extraData)
        {
            Type = dataType;
            Value = dataVal3;
            ExtraData = extraData;
        }
        protected ParameterData() { }
        internal ParameterData(IParameterData data)
        {
            address = data.Address;
            Type = data.Type;
            Value = data.Value;
            ExtraData = data.ExtraData;
        }
        internal ParameterData(byte[] file, int address)
        {
            try
            {
                Address = address;
                Type = ByteUtil.ReadUInt32Ref(file, ref address);
                var offset = ByteUtil.ReadInt32Ref(file, ref address) * sizeof(uint);//количество sizeof(int) слов
                Value = ByteUtil.ReadUInt32Ref(file, ref address);
                var strLength = ByteUtil.ReadUInt32Ref(file, ref address);
                ExtraData = ByteUtil.ReadBytesRef(file, offset, ref address);
            }
            catch
            {
                Console.WriteLine($"{address:X}");
            }
        }
        public byte[] Write()
        {
            var buff = new byte[Length];
            var alignedLength = AlignedLength;
            int position = 0;
            buff = ByteUtil.InsertUint32Ref(buff, Type, ref position);
            buff = ByteUtil.InsertInt32Ref(buff, alignedLength / 4, ref position);
            buff = ByteUtil.InsertUint32Ref(buff, Value, ref position);
            buff = ByteUtil.InsertInt32Ref(buff, alignedLength, ref position);
            return ByteUtil.InsertBytesRef(buff, ExtraData.ToArray(), ref position);
        }
    }
}
