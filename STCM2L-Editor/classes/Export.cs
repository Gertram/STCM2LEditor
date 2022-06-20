﻿using STCM2LEditor.classes.Actions;
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
        public IGameEncoding Encoding { get; set; }
        public byte[] Write()
        {
            List<byte> bytesExport = new List<byte>();

            bytesExport.AddRange(BitConverter.GetBytes(0));
            bytesExport.AddRange(Encoding.Encoding.GetBytes(Name));
            bytesExport.AddRange(BitConverter.GetBytes(ExportedAction.Address));

            return bytesExport.ToArray();
        }
    }
}
