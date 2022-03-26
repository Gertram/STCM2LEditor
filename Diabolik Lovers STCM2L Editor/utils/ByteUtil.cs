using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;

namespace STCM2L.utils {
    public static class ByteUtil {
        public static UInt32 ReadUInt32Ref(byte[] file, ref int seek) {
            return BitConverter.ToUInt32(ReadBytesRef(file, 4, ref seek), 0);
        }
        public static UInt32 ReadUInt32(byte[] file, int seek) {
            return BitConverter.ToUInt32(ReadBytes(file, 4, seek), 0);
        }
        public static UInt32 ReadUInt32(IEnumerable<byte> file, int seek) {
            return BitConverter.ToUInt32(ReadBytes(file, 4, seek), 0);
        }
        public static UInt32 ReadUInt32(ImmutableArray<byte> file, int seek) {
            return BitConverter.ToUInt32(ReadBytes(file, 4, seek), 0);
        }
        public static int ReadInt32(IEnumerable<byte> file, int seek)
        {
            return BitConverter.ToInt32(ReadBytes(file, 4, seek), 0);
        }
        public static int ReadInt32(ImmutableArray<byte> file, int seek) {
            return BitConverter.ToInt32(ReadBytes(file, 4, seek), 0);
        }
        public static int ReadInt32Ref(byte[] file, ref int seek) {
            return BitConverter.ToInt32(ReadBytesRef(file, 4, ref seek), 0);
        }
        public static int ReadInt32(byte[] file, int seek) {
            return BitConverter.ToInt32(ReadBytesRef(file, 4, ref seek), 0);
        }

        public static byte[] ReadBytesRef(byte[] file, int amount, ref int seek) {
            byte[] readValue = new byte[amount];

            Array.Copy(file, seek, readValue, 0, amount);

            seek += (int)amount;

            return readValue;
        }
        public static byte[] ReadBytes(byte[] file, int amount, int seek) {
            byte[] readValue = new byte[amount];

            Array.Copy(file, seek, readValue, 0, amount);

            return readValue;
        }
        public static byte[] ReadBytes(IEnumerable<byte> file, int amount, int seek) {
            return file.Skip(seek).Take(amount).ToArray();
        }
        public static byte[] ReadBytes(ImmutableArray<byte> file, int amount, int seek) {
            byte[] readValue = new byte[amount];

            file.CopyTo(seek, readValue, 0, amount);

            return readValue;
        }
        public static byte[] InsertUint32(byte[] original, UInt32 addition, int position) {
            return InsertBytes(original, BitConverter.GetBytes(addition), position);
        }
        public static byte[] InsertUint32(byte[] original, UInt32 addition, uint position) {
            return InsertBytes(original, BitConverter.GetBytes(addition), position);
        }

        public static byte[] InsertInt32Ref(byte[] original, int addition,ref int position) {
            return InsertBytesRef(original, BitConverter.GetBytes(addition), ref position);
        }
        public static byte[] InsertInt32(byte[] original, int addition,int position) {
            return InsertBytes(original, BitConverter.GetBytes(addition), position);
        }
        public static byte[] InsertUint32Ref(byte[] original, UInt32 addition, ref int position) {
            return InsertBytesRef(original, BitConverter.GetBytes(addition), ref position);
        }

        public static byte[] InsertBytesRef(byte[] original, byte[] addition,ref int position) {
            Array.Copy(addition, 0, original, position, addition.Length);
            position += addition.Length;
            return original;
        }
        public static byte[] InsertBytes(byte[] original, byte[] addition, int position) {
            Array.Copy(addition, 0, original, position, addition.Length);

            return original;
        }
        public static byte[] InsertBytes(byte[] original, byte[] addition, uint position) {
            Array.Copy(addition, 0, original, position, addition.Length);

            return original;
        }
    }
}
