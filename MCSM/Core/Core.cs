using System.Diagnostics;
using System.IO;
using MCSM.Core.Utils;
using MCSM.Pages;

namespace MCSM.Core
{
    public static class Core
    {
        public static ServerConsole serverConsole { get; set; }
        public static MainWindow mainWindow { get; set; }
        public static Java CurrentRunningJava { get; set; }

        public static IniObject Settings = null;
        public static string MCSMAppdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\MCSM\";
        private static string SettingsFile = MCSMAppdata + "settings.ini";

        public async static Task Load()
        {
            await Task.Run(() =>
            {
                Logger.WriteLog(Logger.LogLv.info, "Loading version information from Paper...");
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                
                BukkitVersionManagement.LoadBukkitVersions();
                
                stopwatch.Stop();
                Logger.WriteLog(Logger.LogLv.info,
                    $"Loaded Paper version information ( Length: {BukkitVersionManagement.BukkitVersions.Length} ) ( {stopwatch.ElapsedMilliseconds}ms )");
            });

            await Task.Run(() =>
            {
                Logger.WriteLog(Logger.LogLv.info, "Loading version information from Adoptium...");
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                
                JavaManagement.LoadJavaVersions();

                stopwatch.Stop();
                Logger.WriteLog(Logger.LogLv.info,
                    $"Loaded Java version information ( Length: {JavaManagement.javaBuildVersions.Length} ) ( {stopwatch.ElapsedMilliseconds}ms )");
            });

            

            if (!Directory.Exists(MCSMAppdata))
            {
                Directory.CreateDirectory(MCSMAppdata);
            }

            if (!File.Exists(SettingsFile))
            {
                Settings = new();
                File.WriteAllText(SettingsFile, string.Empty);
            }

            Settings = new IniObject(File.ReadAllText(SettingsFile));
        }
    }
}
