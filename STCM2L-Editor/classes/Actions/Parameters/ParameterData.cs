using STCM2LEditor.utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace STCM2LEditor.classes.Actions.Parameters
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
        public byte[] ExtraData { get; set; }
        public uint Type { get; private set; }
        public uint Value { get; private set; }
        public int Length => ParameterDataHelpers.HEADER_LENGTH + AlignedLength;

        public int DataLength => ExtraData.Length;

        public virtual int AlignedLength => ExtraData.Length;

        internal ParameterData(uint dataType, uint dataVal3, byte[] extraData, int address = 0)
        {
            Type = dataType;
            Value = dataVal3;
            ExtraData = extraData;
            Address = address;
            if (extraData == null)
            {
                throw new Exception();
            }
        }
        protected ParameterData() : this(0, 0, new byte[0]) { }
        internal ParameterData(IParameterData data) : this(data.Type, data.Value, data.ExtraData, data.Address) { }
        internal static ParameterData ReadFromFile(byte[] file, int base_address)
        {
            int address = base_address;
            var type = ByteUtil.ReadUInt32Ref(file, ref address);
            var offset = ByteUtil.ReadInt32Ref(file, ref address) * sizeof(uint);//количество sizeof(int) слов
            var value = ByteUtil.ReadUInt32Ref(file, ref address);
            var strLength = ByteUtil.ReadUInt32Ref(file, ref address);
            var extraData = ByteUtil.ReadBytesRef(file, offset, ref address);
            if(extraData == null)
            {
                return null;
            }
            return new ParameterData(type, value, extraData, base_address);
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

        public IParameterData Copy()
        {
            return new ParameterData
            {
                Type = Type,
                Value = Value,
                Address = Address,
                ExtraData = ExtraData.Clone() as byte[]
            };
        }
    }
}
