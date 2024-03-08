using MCSM.Core.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace MCSM.Core
{
    public static class JavaManagement
    {
        static readonly HttpClient client = new();
        static readonly string baseUrl = "https://api.adoptium.net/v3/";

        public static List<string> javaBuildVersions = new();
       
        public static List<string> getJavaVersions()
        {
            List<string> buildVersionsList = new();

            var i = 0;

            while (true)
            {
                try
                {
                    using (HttpRequestMessage req = new(HttpMethod.Get, string.Format("{0}info/release_names?architecture=x64&heap_size=normal&image_type=jdk&os=windows&page={1}&page_size=20&project=jdk&release_type=ga&semver=false&sort_method=DEFAULT&sort_order=DESC&vendor=eclipse", baseUrl, i)))
                    using (var response = client.Send(req))
                    using (Stream stream = response.Content.ReadAsStream())
                    using (StreamReader reader = new(stream))
                    {
                        string data = reader.ReadToEnd();
                        var obj = JObject.Parse(data);
                        try
                        {
                            buildVersionsList.AddRange(obj["releases"].Values<string>().ToList<string>());
                        }
                        catch (Exception ex)
                        {
                            Logger.WriteLog(Logger.LogLv.error, ex.StackTrace);
                            buildVersionsList = [];
                        }
                    }

                    i++;
                } catch
                {
                    break;
                }
            }

            return buildVersionsList;
        }

        public static String getLatestBuild(int majorVersion)
        {
            if (majorVersion < 8 || majorVersion > 21)
            {
                return null;
            }

            String latestBuild = null;
            foreach (String version in javaBuildVersions)
            {
                String prefix = majorVersion == 8 ? "jdk8u" : "jdk-" + majorVersion;
                if (version.StartsWith(prefix))
                {
                    if (latestBuild == null || version.CompareTo(latestBuild) > 0)
                    {
                        latestBuild = version;
                    }
                }
            }
            return latestBuild;
        }
    }

    public class Java
    {
        public string BuildVer { get; set; }
        public int version { get; set; }
        private Process p;

        public class ProcessOnOutputEventArgs : EventArgs
        {
            public string Text { get; }

            public ProcessOnOutputEventArgs(string text)
            {
                Text = text;
            }
        }

        public event EventHandler<ProcessOnOutputEventArgs> ProcessOnOutput; 

        public Java(int ver) // ver 형식: 17 / 8 / 21... 등
        {
            BuildVer = JavaManagement.getLatestBuild(ver);
            if (BuildVer == null) throw new Exception($"Java version {ver} is not available.");
            
            version = ver;
        }

        private void OutputDataReceivedHandler(object sender, DataReceivedEventArgs e)
        {
            // 이벤트 핸들러 메서드를 이벤트에 등록
            this.ProcessOnOutput -= Core.mainPage.onProcessOutPut;
            this.ProcessOnOutput += Core.mainPage.onProcessOutPut;

            ProcessOnOutput?.Invoke(this, new ProcessOnOutputEventArgs(e.Data));
        }

        public async Task Run(string argments, string dir)
        {
            if (isAvailableToRun())
            {
                ProcessStartInfo start = new ProcessStartInfo();
                start.FileName = Core.MCSMAppdata + @$"jdks\{this.BuildVer}\bin\javaw.exe";
                start.RedirectStandardOutput = true;
                start.RedirectStandardInput = true;
                start.CreateNoWindow = true;
                start.Arguments = argments;
                start.WorkingDirectory = dir;

                await Task.Run(() =>
                {
                    p = Process.Start(start);

                    p.OutputDataReceived -= OutputDataReceivedHandler;
                    p.OutputDataReceived += OutputDataReceivedHandler;

                    p.BeginOutputReadLine();
                });
                
            }
            else throw new Exception("This Java is not available to Run.");
        }

        public async Task<string> GetJavaPrintLineAsync()
        {
            StreamReader reader = p.StandardOutput;
            return await reader.ReadToEndAsync();
        }

        public void InputString(string input)
        {
            StreamWriter myStreamWriter = p.StandardInput;
            myStreamWriter.WriteLine(input);
        }

        public async Task Download()
        {
            var url = $"https://api.adoptium.net/v3/binary/version/{this.BuildVer}/windows/x64/jdk/hotspot/normal/eclipse?project=jdk";
            await Downloader.Download(url, Core.MCSMAppdata + $@"jdks\{this.BuildVer}.zip");
            ZipFile.ExtractToDirectory(Core.MCSMAppdata + $@"jdks\{this.BuildVer}.zip", Core.MCSMAppdata + $@"jdks\");
        }

        public bool isAvailableToRun()
        {
            Logger.WriteLog(Logger.LogLv.info, $"{Core.MCSMAppdata + @$"jdks\{this.BuildVer}\bin\javaw.exe"}");
            if (System.IO.File.Exists(Core.MCSMAppdata + @$"jdks\{this.BuildVer}\bin\javaw.exe")) return true;
            else return false;
        }
    }
}
