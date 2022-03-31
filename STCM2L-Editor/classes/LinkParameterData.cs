using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Immutable;
using System.Threading.Tasks;

using Diabolik_Lovers_STCM2L_Editor.utils;

namespace Diabolik_Lovers_STCM2L_Editor.classes
{
    internal class LinkParameterData
    {
        private const int TYPE_OFFSET = 0;
        private const int OFFSET_OFFSET = sizeof(uint);
        private const int VAL_OFFSET = sizeof(uint) * 2;
        private const int LENGTH_OFFSET = sizeof(uint) * 3;
        private const int DATA_OFFSET = sizeof(uint) * 4;
        public int DataLength => ByteUtil.ReadInt32(extraData, seek + LENGTH_OFFSET);
        public uint Value => ByteUtil.ReadUInt32(extraData, seek + VAL_OFFSET);
        public int AlignedLength => ByteUtil.ReadInt32(extraData, seek + OFFSET_OFFSET) * 4;
        public uint Type => ByteUtil.ReadUInt32(extraData, seek + TYPE_OFFSET);
        public int Length => DATA_OFFSET + AlignedLength;
        public IReadOnlyList<byte> ExtraData => ByteUtil.ReadBytes(extraData, DataLength, seek + DATA_OFFSET);
        public int Address
        {
            get => address;
            set
            {
                if (value != address && address != 0)
                {
                    throw new Exception();
                }
                address = value;
            }
        }

        private readonly int seek;
        private readonly IReadOnlyList<byte> extraData;
        private int address;

        public LinkParameterData(int seek, IReadOnlyList<byte> extraData)
        {
            this.seek = seek;
            this.extraData = extraData;
        }

        public void Write(List<byte> bytes)
        {
            var buff = new byte[Length];
            var alignedLength = AlignedLength;
            int position = 0;
            try
            {
                buff = ByteUtil.InsertUint32Ref(buff, Type, ref position);
                buff = ByteUtil.InsertInt32Ref(buff, alignedLength / 4, ref position);
                buff = ByteUtil.InsertUint32Ref(buff, Value, ref position);
                buff = ByteUtil.InsertInt32Ref(buff, alignedLength, ref position);
                buff = ByteUtil.InsertBytesRef(buff, ExtraData.ToArray(), ref position);
                bytes.AddRange(buff);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
