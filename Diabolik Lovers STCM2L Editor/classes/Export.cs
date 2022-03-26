using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using STCM2L.utils;

namespace STCM2L.classes {
    public class Export {
        public string Name { get; set; }
        public UInt32 Address { get; set; }
        public UInt32 OldAddress { get; set; }
        public IAction ExportedAction { get; set; }

        public byte[] Write() {
            List<byte> bytesExport = new List<byte>();

            bytesExport.AddRange(BitConverter.GetBytes(0));
            bytesExport.AddRange(EncodingUtil.encoding.GetBytes(Name));
            bytesExport.AddRange(BitConverter.GetBytes(ExportedAction.Address));

            return bytesExport.ToArray();
        }
    }
}
