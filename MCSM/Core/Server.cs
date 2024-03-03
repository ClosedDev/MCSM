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
        public List<Plugin> plugin;

        public Server(string dir, BukkitVersion bukkitVersion)
        {
            this.dir = dir;
            this.bukkitVersion = bukkitVersion;
        }

        public async void Create(bool ignoreNotEmpty = false) // 폴더의 빈 여부를 무시/무시하지 않음
        {
            Logger.WriteLog(Logger.LogLv.info, "Creating Server in: " + dir + ".");
            if (!ignoreNotEmpty && Directory.GetFiles(dir).Length != 0)
            {
                Logger.WriteLog(Logger.LogLv.error, "Selected directory isn't empty in MCSM Core: " + dir + ".");
                Logger.WriteLog(Logger.LogLv.error, "Creating Server Failed.");
                throw new Exception("Selected directory isn't empty");
            }

            this.bukkitVersion.loadBukkitBuildVersion();

            Logger.WriteLog(Logger.LogLv.info, $"Bukkit Version: {this.bukkitVersion.ToString()}-{this.bukkitVersion.Build}, RAM: {this.ramAmount}, NoGUI: {this.noGUI}");

            File.WriteAllText(dir + @"\eula.txt", "eula=true");

            string noGuiFlag = this.noGUI ? "-nogui" : "";

            var lines = new List<string>();
            lines.Add("[bukkit]");
            lines.Add($"argment=-Xms{this.ramAmount}M -Xmx{this.ramAmount}M -jar bukkit-{this.bukkitVersion.ToString()}.jar {noGuiFlag}");
            lines.Add($"builds={this.bukkitVersion.Build}");
            lines.Add("[plugins]");

            /*this.plugin = new();
            this.plugin.Add(new Plugin(new PluginInfo(Plugin.PluginType.SUPPORT, "", "", this.bukkitVersion.VER, "", Plugin.SupportPlugin.WORLDEDIT)));

            foreach (var pl in this.plugin)
            {
                lines.Add($"{pl.info.}.jar");
            }*/


            File.WriteAllLines(dir + @"\info.ini", lines);

            DirectoryInfo di = new(dir + @"\Plugins");
            di.Create();

            await Task.Run(() =>
            {
                try
                {
                    var url = $"https://api.papermc.io/v2/projects/paper/versions/{this.bukkitVersion.VER}/builds/{this.bukkitVersion.Build}/downloads/paper-{this.bukkitVersion.VER}-{this.bukkitVersion.Build}.jar";
                    Logger.WriteLog(Logger.LogLv.info, url);
                    var client = new WebClient();
                    client.DownloadFile(url, dir + $@"\bukkit-{this.bukkitVersion.ToString()}.jar");
                } catch (Exception ex)
                {
                    MessageBox.Show($"다운로드 중 알 수 없는 오류가 발생했습니다. : '{ex}'", "MCSM Core", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });

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
