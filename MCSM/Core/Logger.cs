using System.Diagnostics;
using System.IO;
using System.Windows;
namespace MCSM.Core
{
    public enum LogLv
    {
        info = 6,
        warn = 4,
        error = 2,
        fatal = 0
    }

    public class Logger
    {
        static Queue<string> logQueue = new Queue<string>();
        private static readonly object writeLock = new object();

        public static void WriteLog(LogLv lv, string str)
        {
            Debug.WriteLine(str);
            logQueue.Enqueue(str);
            {
                lock (writeLock)
                {
                    StreamWriter fw;

                    if (!File.Exists("log.log")) fw = File.CreateText("log.log");
                    else fw = File.AppendText("log.log");

                    while (logQueue.Count > 0)
                    {
                        System.Diagnostics.StackFrame stackFrame = new System.Diagnostics.StackFrame(1, true);
                        string funcName = stackFrame.GetMethod().Name;
                        string fileName = stackFrame.GetFileName();
                        fileName = Path.GetFileNameWithoutExtension(fileName);
                        string fileLine = stackFrame.GetFileLineNumber().ToString();

                        string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

                        string log = logQueue.Dequeue();
                        fw.WriteLine($"[{lv.ToString()}] [{now}] [ at {fileName}.cs line {fileLine} in {funcName}() ] : {str}");
                        if (lv == LogLv.fatal) MessageBox.Show("Fatal Error occurred!!!\n" + str, "MCSM Core", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    fw.Close();
                }
            }
        }
        
    }
}
