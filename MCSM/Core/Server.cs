using MCSM.Core.Utils;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Windows;
using static MCSM.Core.Plugin;

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

        public async void Create(bool ignoreNotEmpty = false)  // 폴더의 빈 여부를 무시/무시하지 않음
        {
            Stopwatch serverStopWatch = new Stopwatch();
            serverStopWatch.Start();

            try
            {
                Logger.WriteLog(Logger.LogLv.info, "Creating Server in: " + this.dir + ".");

                // Pre Settings
                this.bukkitVersion.LoadBukkitBuildVersion();
                if (!ignoreNotEmpty && Directory.GetFiles(this.dir).Length != 0)
                {
                    Logger.WriteLog(Logger.LogLv.error, "Selected directory isn't empty in MCSM Core: " + this.dir + ".");
                    Logger.WriteLog(Logger.LogLv.error, "Creating Server Failed.");
                    throw new Exception("Selected directory isn't empty");
                }

                Logger.WriteLog(Logger.LogLv.info, $"Bukkit Version: {this.bukkitVersion.ToString()}-{this.bukkitVersion.Build}, RAM: {this.ramAmount}, NoGUI: {this.noGUI}");

                // Eula
                File.WriteAllText(this.dir + @"\eula.txt", "eula=true");

                // Ini
                var lines = new List<string>();
                lines.Add("[bukkit]");
                lines.Add($@"argment=-Xms{this.ramAmount}M -Xmx{this.ramAmount}M -jar bukkit-{this.bukkitVersion.ToString()}.jar {(this.noGUI ? "-nogui" : "")}");
                lines.Add($"builds={this.bukkitVersion.Build}");
                File.WriteAllLines(this.dir + @"\info.ini", lines);

                // Plugin
                Directory.CreateDirectory(this.dir + @"\Plugins");
                this.plugin.Add(Plugin.GetPluginWithVersion(SupportPlugin.WORLDEDIT, this.bukkitVersion));
                foreach (var element in this.plugin)
                {
                    if (element.info.type == PluginType.SUPPORT)
                    {
                        await Downloader.Download(element.info.url, this.dir + @"\Plugins\" + element.info.dir);
                    }
                    else
                    {
                        //TODO: 선택되어있는 사용자 지정 플러그인 파일 복사
                    }
                }
                File.WriteAllText(this.dir + @"\Plugins\info.json", JsonConvert.SerializeObject(this.plugin));

                // Bukkit
                var url = $"https://api.papermc.io/v2/projects/paper/versions/{this.bukkitVersion.VER}/builds/{this.bukkitVersion.Build}/downloads/paper-{this.bukkitVersion.VER}-{this.bukkitVersion.Build}.jar";
                Logger.WriteLog(Logger.LogLv.info, $"[ CREATE SERVER ] Downloading bukkit... : {url}");

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                await Downloader.Download(url, this.dir + $@"\bukkit-{this.bukkitVersion.ToString()}.jar");
                stopwatch.Stop();

                Logger.WriteLog(Logger.LogLv.info, $"[ CREATE SERVER ] Download Complete ( {stopwatch.ElapsedMilliseconds}ms )");

                // Checking
                var iniText = File.ReadAllText(this.dir + @"\info.ini");
                IniObject ini = new(iniText);

                Logger.WriteLog(Logger.LogLv.info, $"Running Server with argment: java {ini["bukkit"]["argment"]}");

                Java java = new(17);

                ServerVar.java = java;

                await java.Download();
                java.Run(ini["bukkit"]["argment"], this.dir);

                /*while (true)
                {
                    Logger.WriteLog(Logger.LogLv.info, java.GetJavaPrintLine());
                }*/

            } catch (Exception e)
            {
                serverStopWatch.Stop();

                Logger.WriteLog(Logger.LogLv.error, $"[ CREATE SERVER ] Create Server is ended with error!: {e}");
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

    public static class ServerVar
    {
        public static Java java;
    }
}
