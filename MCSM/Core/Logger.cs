using System.Diagnostics;
using System.IO;
using System.Windows;
namespace MCSM.Core
{

    public class Logger
    {
        public enum LogLv
        {
            info = 6,
            warn = 4,
            error = 2,
            fatal = 0
        }

        static Queue<string> logQueue = new Queue<string>();
        private static readonly object writeLock = new object();
        private static readonly string logFile = Core.MCSMAppdata + @"\log.log";

        public static void WriteLog(LogLv lv, string str, string brackets = "")
        {
            var bracket = brackets == string.Empty ? brackets : "[ " + brackets + " ] ";
            var logStr = $"{bracket}{str}";
            
            Debug.WriteLine(logStr);
            logQueue.Enqueue(logStr);
            {
                lock (writeLock)
                {
                    StreamWriter fw;

                    if (!File.Exists(logFile)) fw = File.CreateText(logFile);
                    else fw = File.AppendText(logFile);

                    while (logQueue.Count > 0)
                    {
                        StackFrame stackFrame = new(1, true);
                        string funcName = stackFrame.GetMethod().Name;
                        string fileName = stackFrame.GetFileName();
                        fileName = Path.GetFileNameWithoutExtension(fileName);
                        string fileLine = stackFrame.GetFileLineNumber().ToString();

                        string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

                        string log = logQueue.Dequeue();
                        fw.WriteLine($"[{lv.ToString()}] [{now}] [ at {fileName}.cs line {fileLine} in {funcName}() ] : {logStr}");
                        if (lv == LogLv.fatal) MessageBox.Show("Fatal Error occurred!!!\n" + logStr, "MCSM Core", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    fw.Close();
                }
            }
        }
        
    }
}
