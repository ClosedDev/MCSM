using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace MCSM.Core
{
    public static class BukkitVersions
    {
        static readonly HttpClient client = new();

        public static BukkitVersion[] Versions = [];

        private static string baseUrl = "https://api.papermc.io/v2/";

        public static BukkitVersion[] getButtkitVersionsWithAPI(bool getBuild = false)
        {
            string[] versions;

            using (HttpRequestMessage req = new(HttpMethod.Get, string.Format("{0}projects/paper", baseUrl)))
            using (var response = client.Send(req))
            using (Stream stream = response.Content.ReadAsStream())
            using (StreamReader reader = new(stream))
            {
                string data = reader.ReadToEnd();
                var obj = JObject.Parse(data);
                try
                {
                    versions = obj["versions"].Select(v => (string)v).ToArray();
                } catch (Exception ex)
                {
                    Logger.WriteLog(Logger.LogLv.error, ex.StackTrace);
                    versions = [];
                }
            }

            List<BukkitVersion> versionList = [];

            foreach (string version in versions)
            {
                if (version != "1.13-pre7")
                {
                    if (!getBuild) versionList.Add(new BukkitVersion(version));
                    else versionList.Add(getBukkitBuildVersionWithAPI(version));
                }
            }

            return versionList.ToArray();
        }

        private static BukkitVersion getBukkitBuildVersionWithAPI(string bvStr)
        {
            string[] versions;

            using (HttpRequestMessage req = new(HttpMethod.Get, string.Format("{0}projects/paper/versions/{1}", baseUrl, bvStr)))
            using (var response = client.Send(req))
            using (Stream stream = response.Content.ReadAsStream())
            using (StreamReader reader = new(stream))
            {
                string data = reader.ReadToEnd();
                var obj = JObject.Parse(data);
                try
                {
                    versions = obj["builds"].Select(v => (string)v).ToArray();
                }
                catch (Exception ex)
                {
                    Logger.WriteLog(Logger.LogLv.error, ex.StackTrace);
                    versions = [];
                }
            }

            return new BukkitVersion(bvStr, int.Parse(versions.Last()));
        }

        public static void LoadVersions(bool getBuild = false)
        {
            new Thread(() =>
            {
                Logger.WriteLog(Logger.LogLv.info, "Loading version information from Paper...");
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                Versions = getButtkitVersionsWithAPI(getBuild);

                stopwatch.Stop();
                Logger.WriteLog(Logger.LogLv.info, $"Loaded version information ( Length: {Versions.Length} ) ( {stopwatch.ElapsedMilliseconds}ms )");
            }).Start();
        }
    }

    public class BukkitVersion
    {
        public int Ver { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Build { get; set; }
        public string VER;

        public string GetDownloadURL()
        { return $"https://api.papermc.io/v2/projects/paper/versions/{this.VER}/builds/{this.Build}/downloads/{this.VER}"; }

        public BukkitVersion(string version, int build = 0)
        {
            string[] versionParts = version.Split('.');

            Ver = int.Parse(versionParts[0]);
            Major = int.Parse(versionParts[1]);

            if (!(versionParts.Length == 2)) Minor = int.Parse(versionParts[2]);
            else Minor = 00;

            Build = build;

            VER = version;
        }

        public override string ToString()
        {
            return $"v{Ver:D2}M{Major:D2}m{Minor:D2}";
        }
    }
}
