using MCSM.Core.Utils;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Windows;
using MCSM.Pages;
using static MCSM.Core.Plugin;
using static System.Net.Mime.MediaTypeNames;

namespace MCSM.Core
{
    public class Server
    {
        public string dir;
        public BukkitVersion bukkitVersion;

        public long ramAmount;
        public bool noGUI;
        public ServerProperties properties;
        public List<Plugin> plugin = new();

        public Server(string dir, BukkitVersion bukkitVersion)
        {
            this.dir = dir;
            this.bukkitVersion = bukkitVersion;
        }

        public async Task Create(bool ignoreNotEmpty = false)  // 폴더의 빈 여부를 무시/무시하지 않음
        {
            Stopwatch serverStopWatch = new Stopwatch();
            serverStopWatch.Start();

            try
            {
                Logger.WriteLog(Logger.LogLv.info, "Creating Server in: " + this.dir + ".");

                // Pre Settings
                var build = BukkitBuild.GetBuild(bukkitVersion);
                if (!ignoreNotEmpty && Directory.GetFiles(this.dir).Length != 0)
                {
                    Logger.WriteLog(Logger.LogLv.error, "Selected directory isn't empty in MCSM Core: " + this.dir + ".");
                    Logger.WriteLog(Logger.LogLv.error, "Creating Server Failed.");
                    throw new Exception("Selected directory isn't empty");
                }

                Logger.WriteLog(Logger.LogLv.info, $"Bukkit Version: {this.bukkitVersion.ToString()}-{build.Build}, RAM: {this.ramAmount}, NoGUI: {this.noGUI}");

                // Eula
                File.WriteAllText(this.dir + @"\eula.txt", "eula=true");

                // Ini
                var lines = new List<string>();
                lines.Add("[bukkit]");
                lines.Add($@"argment=-Xms{this.ramAmount}M -Xmx{this.ramAmount}M -jar bukkit-{this.bukkitVersion.ToString()}.jar {(this.noGUI ? "-nogui" : "")}");
                lines.Add($"builds={build.Build}");
                File.WriteAllLines(this.dir + @"\info.ini", lines);

                // Plugin
                Directory.CreateDirectory(this.dir + @"\Plugins");
                this.plugin.Add(Plugin.GetPluginWithVersion(SupportPlugin.WORLDEDIT, this.bukkitVersion)); //TODO: For Debug
                foreach (var element in this.plugin)
                {
                    if (element.info.type == PluginType.SUPPORT)
                    {
                        await Downloader.Download(element.info.url, this.dir + @"\Plugins\" + element.info.dir, "CREATE SERVER", element.info.name);
                    }
                    else
                    {
                        //TODO: 선택되어있는 사용자 지정 플러그인 파일 복사
                    }
                }
                File.WriteAllText(this.dir + @"\Plugins\info.json", JsonConvert.SerializeObject(this.plugin));

                // Bukkit
                var url = $"https://api.papermc.io/v2/projects/paper/versions/{this.bukkitVersion.VER}/builds/{build.Build}/downloads/paper-{this.bukkitVersion.VER}-{build.Build}.jar";
                await Downloader.Download(url, this.dir + $@"\bukkit-{this.bukkitVersion.ToString()}.jar", "CREATE SERVER", "bukkit");
                
                // Properties
                this.properties = new ServerProperties("so=sow\nthis=fordebug"); //TODO: For Debug
                File.WriteAllText(this.dir + @"\server.properties", this.properties.ToString());

                // Java
                var iniText = File.ReadAllText(this.dir + @"\info.ini");
                IniObject ini = new(iniText);

                // Get Java Version
                Logger.WriteLog(Logger.LogLv.info, "Get java version from bukkit..");
                ZipFile.ExtractToDirectory($@"{this.dir}\bukkit-{this.bukkitVersion.ToString()}.jar", $@"{this.dir}\TEMP-BUKKIT-UNZIPPED-JAR");
                var javaVersion = 0;
                try
                {
                    var obj = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText($@"{this.dir}\TEMP-BUKKIT-UNZIPPED-JAR\version.json"));
                    javaVersion = int.Parse(obj["java_version"].ToString());
                } catch
                {
                    string[] subDirectories = Directory.GetDirectories($@"{this.dir}\TEMP-BUKKIT-UNZIPPED-JAR\META-INF\maven\io.papermc\");

                    foreach (string subDirectory in subDirectories)
                    {
                        Regex regex = new Regex(@"\d+");
                        Match match = regex.Match(subDirectory);
                        if (match.Success) javaVersion = int.Parse(match.Value);
                    }

                    if (javaVersion == 0) throw new Exception("Java version not recognized.");
                }
                Directory.Delete($@"{this.dir}\TEMP-BUKKIT-UNZIPPED-JAR\", true);

                // Running
                Logger.WriteLog(Logger.LogLv.info, $"{javaVersion}");
                Logger.WriteLog(Logger.LogLv.info, $"Testing Server with argment: java {ini["bukkit"]["argment"]}");

                Java java = new(javaVersion);

                Core.CurrentRunningJava = java;
                
                if (!java.CheckAvailableToRun()) await java.Download();
                await java.Run(ini["bukkit"]["argment"], this.dir, true); // 주의: 프로세스 종료까지 await Sleep이 작동되지 않음

                serverStopWatch.Stop();

                Logger.WriteLog(Logger.LogLv.info, "Server Create Ended! ( " + serverStopWatch.ElapsedMilliseconds.ToString() + "ms )");

            } catch (Exception e)
            {
                serverStopWatch.Stop();

                Logger.WriteLog(Logger.LogLv.error, $"Create Server is ended with error!: {e}", "CREATE SERVER");
                MessageBox.Show($"알 수 없는 오류가 발생했습니다. : '{e}'", "MCSM Core", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class ServerBuilder
    {
        private Server server;
        public ServerBuilder(string dir, BukkitVersion bv)
        {
            server = new Server(dir, bv);
        }

        public ServerBuilder SetRAM(long ramAmount)
        {
            server.ramAmount = ramAmount;
            return this;
        }

        public ServerBuilder SetNoGUI(bool noGUI)
        {
            server.noGUI = noGUI;
            return this;
        }

        public ServerBuilder SetProperties(ServerProperties properties)
        {
            server.properties = properties;
            return this;
        }

        public ServerBuilder AddPlugin(Plugin plugin)
        {
            server.plugin.Append(plugin);
            return this;
        }

        public Server Build()
        {
            return server;
        }
    }
}
