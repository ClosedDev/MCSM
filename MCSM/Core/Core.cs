﻿using System.IO;
using MCSM.Core.Utils;

namespace MCSM.Core
{
    public static class Core
    {
        public static IniObject Settings = null;
        private static string MCSMAppdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\MCSM";
        private static string SettingsFile = MCSMAppdata + "\\settings.ini";

        public static void Load()
        {
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