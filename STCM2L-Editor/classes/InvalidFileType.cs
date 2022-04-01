using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STCM2LEditor.classes
{
    internal class InvalidFileTypeException : Exception
    {
        public string FileName { get; private set; }
        internal InvalidFileTypeException(string filename){
            FileName = filename;
        }
    }
}
