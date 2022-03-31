using STCM2LEditor.classes.Action;
using STCM2LEditor.utils;
using System;
using System.Collections.Generic;

namespace STCM2LEditor.classes
{
    public class Export
    {
        public string Name { get; set; }
        public UInt32 Address { get; set; }
        public UInt32 OldAddress { get; set; }
        public IAction ExportedAction { get; set; }

        public byte[] Write()
        {
            List<byte> bytesExport = new List<byte>();

            bytesExport.AddRange(BitConverter.GetBytes(0));
            bytesExport.AddRange(EncodingUtil.encoding.GetBytes(Name));
            bytesExport.AddRange(BitConverter.GetBytes(ExportedAction.Address));

            return bytesExport.ToArray();
        }
    }
}
