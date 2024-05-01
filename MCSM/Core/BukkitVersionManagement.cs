using MCSM.Core.Utils;
using Newtonsoft.Json;
using System.Diagnostics;

namespace MCSM.Core {
    public static class BukkitVersionManagement
    {
        public static BukkitVersion[] BukkitVersions;
        
        public static readonly string baseUrl = "https://api.papermc.io/v2/";

        public static void LoadBukkitVersions()
        {
            string[] versions;
            var data = APIManager.Get(string.Format("{0}projects/paper", baseUrl));
            var obj = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
            try
            {
                versions = JsonConvert.DeserializeObject<string[]>(obj["versions"].ToString());
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Logger.LogLv.error, ex.StackTrace);
                versions = [];
            }

            BukkitVersions = versions
                .Where(version => version != "1.13-pre7")
                .Select(version => new BukkitVersion(version)).ToArray();
        }
    }
    
    public class BukkitVersion
    {
        public int Ver;
        public int Major;
        public int Minor;
        public string VER;

        public BukkitVersion(string version)
        {
            var versionParts = version.Split('.');

            this.Ver = int.Parse(versionParts[0]);
            this.Major = int.Parse(versionParts[1]);

            if (!(versionParts.Length == 2)) this.Minor = int.Parse(versionParts[2]);
            else this.Minor = 00;

            this.VER = version;
        }
    
        public override string ToString()
        {
            return $"v{this.Ver:D2}M{this.Major:D2}m{this.Minor:D2}";
        }

        public string ToVersionString()
        {
            return $"{this.Ver}.{this.Major}.{this.Minor}";
        }
    }

    public class BukkitBuild : BukkitVersion
    {
        public int Build;
    
        public BukkitBuild(string version, int build) : base(version)
        {
            this.Build = build;
        }

        public static BukkitBuild GetBuild(BukkitVersion version)
        {
            int[] versions;
            
            var data = APIManager.Get(string.Format("{0}projects/paper/versions/{1}", BukkitVersionManagement.baseUrl, version.ToVersionString()));

            var obj = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
            try
            {
                versions = JsonConvert.DeserializeObject<int[]>(obj["builds"].ToString());
            }
            catch (Exception ex)
            {
                Logger.WriteLog(Logger.LogLv.error, ex.StackTrace);
                versions = [];
            }

            return new BukkitBuild(version.ToVersionString(), versions.Last());
        }
    }
}