using MCSM.Core.Utils;
using Newtonsoft.Json;
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

        public async void Create(bool ignoreNotEmpty = false) // 폴더의 빈 여부를 무시/무시하지 않음
        {
            Logger.WriteLog(Logger.LogLv.info, "Creating Server in: " + dir + ".");
            
            // Pre Settings
            this.bukkitVersion.LoadBukkitBuildVersion();
            if (!ignoreNotEmpty && Directory.GetFiles(dir).Length != 0)
            {
                Logger.WriteLog(Logger.LogLv.error, "Selected directory isn't empty in MCSM Core: " + dir + ".");
                Logger.WriteLog(Logger.LogLv.error, "Creating Server Failed.");
                throw new Exception("Selected directory isn't empty");
            }

            Logger.WriteLog(Logger.LogLv.info, $"Bukkit Version: {this.bukkitVersion.ToString()}-{this.bukkitVersion.Build}, RAM: {this.ramAmount}, NoGUI: {this.noGUI}");

            // Eula
            File.WriteAllText(dir + @"\eula.txt", "eula=true");
            
            // Ini
            var lines = new List<string>();
            lines.Add("[bukkit]");
            lines.Add($"argment=-Xms{this.ramAmount}M -Xmx{this.ramAmount}M -jar bukkit-{this.bukkitVersion.ToString()}.jar {(this.noGUI ? "-nogui" : "")}");
            lines.Add($"builds={this.bukkitVersion.Build}");
            File.WriteAllLines(dir + @"\info.ini", lines);
            
            // Plugin
            Directory.CreateDirectory(dir + @"\Plugins");
            this.plugin.Add(Plugin.GetPluginWithVersion(SupportPlugin.WORLDEDIT, this.bukkitVersion));
            foreach (var element in this.plugin)
            {
                if (element.info.type == PluginType.SUPPORT)
                {
                    await Downloader.Download(element.info.url, dir + @"\Plugins\" + element.info.dir);
                } else
                {
                    //TODO file copy
                }
            }
            File.WriteAllText(dir + @"\Plugins\info.json", JsonConvert.SerializeObject(this.plugin));
            
            // Bukkit
            await Downloader.Download($"https://api.papermc.io/v2/projects/paper/versions/{this.bukkitVersion.VER}/builds/{this.bukkitVersion.Build}/downloads/paper-{this.bukkitVersion.VER}-{this.bukkitVersion.Build}.jar", dir + $@"\bukkit-{this.bukkitVersion.ToString()}.jar");

            Logger.WriteLog(Logger.LogLv.info, "DOWNLOAD COMPLETE!");
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
