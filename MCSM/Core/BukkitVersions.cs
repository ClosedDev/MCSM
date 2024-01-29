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

namespace MCSM.Core
{
    public class BukkitVersions
    {
        public BukkitVersion[] Versions;

        private string baseUrl = "https://api.papermc.io/v2/";

        public BukkitVersion[] getButtkitVersionsWithAPI(bool getBuild = false)
        {
            JToken versions;

            WebRequest request = WebRequest.Create(baseUrl + "projects/paper");
            request.Method = "GET";
            request.ContentType = "application/json";

            using (WebResponse response = request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                string data = reader.ReadToEnd();
                var obj = JObject.Parse(data);
                versions = obj["versions"];
            }

            List<BukkitVersion> versionList = new List<BukkitVersion>();

            foreach (string version in versions)
            {
                if (!(version == "1.13-pre7"))
                {
                    if (!(getBuild)) versionList.Add(new BukkitVersion(version));
                    else versionList.Add(getBukkitBuildVersionWithAPI(version));
                }
            }

            return versionList.ToArray();
        }

        private BukkitVersion getBukkitBuildVersionWithAPI(string bvStr)
        {
            List<string> versions;

            WebRequest request = WebRequest.Create(baseUrl + "projects/paper/versions/" + bvStr);
            request.Method = "GET";
            request.ContentType = "application/json";

            using (WebResponse response = request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                string data = reader.ReadToEnd();
                var obj = JObject.Parse(data);
                versions = obj["builds"].Values<string>().ToList();
            }

            return new BukkitVersion(bvStr, int.Parse(versions.Last()));
        }
    }

    public class BukkitVersion
    {
        public int Ver { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Build { get; set; }
        public string VER;

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
