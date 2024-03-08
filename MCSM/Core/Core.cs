using System.IO;
using MCSM.Core.Utils;
using MCSM.Pages;

namespace MCSM.Core
{
    public static class Core
    {
        public static MainPage mainPage { get; set; }

        public static IniObject Settings = null;
        public static string MCSMAppdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\MCSM\";
        private static string SettingsFile = MCSMAppdata + "\\settings.ini";

        public static void Load()
        {
            BukkitVersions.LoadVersions();

            JavaManagement.javaBuildVersions = JavaManagement.getJavaVersions();

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
