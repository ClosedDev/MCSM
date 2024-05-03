using MCSM.Core.Utils;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Windows.Threading;
using MCSM.Pages;
using Newtonsoft.Json;

namespace MCSM.Core
{
    public static class JavaManagement
    {
        private static readonly string baseURL = "https://api.adoptium.net/v3";
        public static string[] javaBuildVersions;
        
        public static void LoadJavaVersions()
        {
            List<string> buildVersionsList = new();
            HttpClient client = new();

            var i = 0;
            while (true)
            {
                try
                {
                    var data = APIManager.Get(
                        $"{baseURL}/info/release_names?architecture=x64&heap_size=normal&image_type=jdk&os=windows&" +
                        $"page={i}&page_size=20&project=jdk&release_type=ga&semver=false&sort_method=DEFAULT&sort_order=DESC&vendor=eclipse");

                    if (data == string.Empty) break;
                    
                    var obj = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
                    buildVersionsList.AddRange(JsonConvert.DeserializeObject<string[]>(obj["releases"].ToString()));
                    
                    i++;
                }
                catch (Exception ex)
                {
                    Logger.WriteLog(Logger.LogLv.error, ex.ToString() + ":" + ex.StackTrace);
                    buildVersionsList = [];
                }
            }
            javaBuildVersions = buildVersionsList.ToArray();
        }
        public static String getLatestBuild(int majorVersion)
        {
             if (majorVersion < 8 || majorVersion > 21) return null;

             String latestBuild = null;
             foreach (String version in javaBuildVersions)
             {
                 String prefix = majorVersion == 8 ? "jdk8u" : "jdk-" + majorVersion;
                 if (version.StartsWith(prefix) && (latestBuild == null || version.CompareTo(latestBuild) > 0))
                 {
                     latestBuild = version;
                     break;
                 }
             }
             return latestBuild;
        }
    }

    public class Java
    {
        public string buildVersion { get; set; }
        public int VERSION { get; set; }
        private Process process;

        public Java(int version)
        {
            buildVersion = JavaManagement.getLatestBuild(version);
            if (buildVersion == null) throw new Exception($"Java version {version} is not available.");

            VERSION = version;
        }
        
        /*public event EventHandler<ProcessOnOutPutEventArgs> ProcessOnOutPutEvent;
        
        public class ProcessOnOutPutEventArgs(string text) : EventArgs
        {
            public string Text { get; } = text;
        }

        private void OutputDataReceivedHandler(object sender, DataReceivedEventArgs e)
        {
            this.ProcessOnOutPutEvent -= Core.serverConsole.onProcessOutPut;
            this.ProcessOnOutPutEvent += Core.serverConsole.onProcessOutPut;
            
            ProcessOnOutPutEvent?.Invoke(this, new ProcessOnOutPutEventArgs(e.Data));
        }*/

        public event EventHandler<ProcessOutputEventArgs> ProcessOutputEvent;

        public class ProcessOutputEventArgs(string text) : EventArgs
        {
            public string Text = text;
        }
        
        private void ProcessOutputEventHandler(object sender, DataReceivedEventArgs e)
        {
            ProcessOutputEvent?.Invoke(this, new ProcessOutputEventArgs(e.Data));
        }

        public async Task Run(string argments, string workingDirectory, bool isTest = false)
        {
            if (!CheckAvailableToRun()) throw new Exception("This Java is not available to Run.");

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = Core.MCSMAppdata + @$"jdks\{this.buildVersion}\bin\javaw.exe",
                    Arguments = argments,
                    WorkingDirectory = workingDirectory,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };
            process.OutputDataReceived += ProcessOutputEventHandler;
            process.ErrorDataReceived += ProcessOutputEventHandler;
            ProcessOutputEvent += Core.serverConsole.ProcessOutputEvent;

            await Task.Run(() =>
            {
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            });


            /*ProcessStartInfo psi = new();
            psi.FileName = Core.MCSMAppdata + @$"jdks\{this.buildVersion}\bin\javaw.exe";
            Debug.WriteLine(psi.FileName);
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardInput = true;
            psi.CreateNoWindow = true;
            psi.Arguments = argments;
            Debug.WriteLine(psi.Arguments);
            psi.WorkingDirectory = workingDirectory;

            await Task.Run(() =>
            {
                process = Process.Start(psi);

                process.PriorityClass = ProcessPriorityClass.High;

                process.OutputDataReceived -= OutputDataReceivedHandler;
                process.OutputDataReceived += OutputDataReceivedHandler;

                // if (isTest) InputString("stop");

                process.BeginOutputReadLine();
            });*/
        }

        public void InputString(string input)
        {
            StreamWriter processStreamWriter = process.StandardInput;
            processStreamWriter.WriteLine(input);
        }

        public async Task Download()
        {
            var url = $"https://api.adoptium.net/v3/binary/version/{this.buildVersion}/windows/x64/jdk/hotspot/normal/eclipse?project=jdk";
            await Downloader.Download(url, Core.MCSMAppdata + $@"jdks\{this.buildVersion}.zip", "JAVA", $"JDK-{VERSION}");
            ZipFile.ExtractToDirectory(Core.MCSMAppdata + $@"jdks\{this.buildVersion}.zip", Core.MCSMAppdata + $@"jdks\");
        }
        
        public bool CheckAvailableToRun()
        { 
            if (System.IO.File.Exists(Core.MCSMAppdata + @$"jdks\{this.buildVersion}\bin\javaw.exe")) return true;
            return false;
        }
    }
}