using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace STCM2LEditor
{
    internal class Logger
    {
        private const string logfile = "log.txt";
        private const int MAXSIZE = 10 << 10;
        internal static void Error(string message)
        {
            PrintLog(message, "ERROR");
        }
        private static void PrintLog(string message,string code)
        {
            var time = DateTime.Now;
            var stack = System.Environment.StackTrace;
            if (File.Exists(logfile) && new FileInfo(logfile).Length > MAXSIZE)
            {
                File.Delete(logfile);
            }
            File.AppendAllText(logfile, $"{code}: {time.ToUniversalTime()} in {stack} \"{message}\"");
        }
        internal static void Debug(string message)
        {
            PrintLog(message, "DEBUG");
        }
    }
}
