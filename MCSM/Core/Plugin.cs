using Newtonsoft.Json.Linq;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;

namespace MCSM.Core
{
    public class PluginList
    {
        private List<Plugin> list;
        public PluginList(List<Plugin> preset = null) {
            list = preset ?? new();
        }
        
        public Plugin this[int i] {
            get
            {
                try
                {
                    return list[i];
                }
                catch (Exception)
                {
                    return null;
                }
            }

            set
            {
                list[i] = value;
            }
        }

        public void Add(Plugin p)
        {
            list.Add(p);
        }
    }
    
    public class Plugin(PluginInfo info)
    {
        public enum PluginType
        {
            SUPPORT,
            OTHER
        }

        public enum SupportPlugin
        {
            WORLDEDIT,
            WORLDGUARD,
            SIMPLE_VOICE_CHAT,
            ESSENTIALSX,
            COREPROTECT
        }

        public PluginInfo info = info;

        private class Result
        {
            public string name { get; set; }
            public string type { get; set; }
            public string version { get; set; }
            public string size { get; set; }
            public string url { get; set; }
            public string date { get; set; }
        }

        private class Root
        {
            public Result[] result { get; set; }
        }


        static readonly HttpClient client = new();
        public static Plugin? GetPluginWithVersion(SupportPlugin type, BukkitVersion bukkitVersion)
        {
            Logger.WriteLog(Logger.LogLv.info, "Loading plugin version information from MCSM Plugin API...");

            string ver = string.Format("{0}.{1}", bukkitVersion.Ver, bukkitVersion.Major);

            using (HttpRequestMessage req = new(HttpMethod.Get, string.Format("{0}plugins/{1}", "http://mcsm.closeddev.kro.kr/", PluginInfo.plugins[type].Item1.ToLower().Replace(" ", "-"))))
            using (var response = client.Send(req))
            using (Stream stream = response.Content.ReadAsStream())
            using (StreamReader reader = new(stream))
            {
                try
                {
                    string data = reader.ReadToEnd();
                    var value = JsonConvert.DeserializeObject<Root>(data) ?? throw new InvalidOperationException();
                    var objects = value.result;

                    for (var i = 0; i < objects.Length; i++)
                    {
                        var element = objects[i];

                        if (element.version.Trim().Contains(ver))
                        {
                            return new Plugin(new PluginInfo(PluginType.SUPPORT, "", "", element.version, element.date, element.url, type));
                        }
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    Logger.WriteLog(Logger.LogLv.error, ex.StackTrace);
                    return null;
                }
            }
        }
    }

    public class PluginInfo
    {
        public static readonly Dictionary<Plugin.SupportPlugin, Tuple<string, string>> plugins = new()
        {
            { Plugin.SupportPlugin.WORLDEDIT, new ("WorldEdit", "각종 명령어로 인게임 내에서 블럭을 생성하거나 없엘 수 있는 플러그인 입니다.") },
            { Plugin.SupportPlugin.WORLDGUARD, new ("WorldGuard", "블럭에 관련해 부수기 / 설치하기와 같이 규칙을 설정할 수 있는 플러그인 입니다.") },
            { Plugin.SupportPlugin.ESSENTIALSX, new ("EssentialsX", "서버 관리에 도움이 되는 100개 이상의 명령어를 추가하는 플러그인 입니다.") },
            { Plugin.SupportPlugin.COREPROTECT, new ("CoreProtect", "플레이어가 상호작용한 블럭의 모든 정보를 보여주고 복구할 수 있는 플러그인 입니다.") },
            { Plugin.SupportPlugin.SIMPLE_VOICE_CHAT, new ("Simple Voice Chat", "다른 플레이어와 마이크로 소통할 수 있는 플러그인 입니다.")}
        };

        public Plugin.PluginType type;
        public string name;
        public string desc;
        public BukkitVersion version;
        public string date;
        public string dir;
        public string url;

        public PluginInfo(Plugin.PluginType type, string name, string desc, string version, string date, string url, Plugin.SupportPlugin? support = null)
        {
            if (type == Plugin.PluginType.SUPPORT && support != null)
            {
                this.name = plugins[(Plugin.SupportPlugin)support].Item1;
                this.desc = plugins[(Plugin.SupportPlugin)support].Item2;
            }
            else
            {
                this.name = name;
                this.desc = desc;
            }

            this.type = type;
            this.version = new BukkitVersion(version);
            this.date = date;
            this.dir = this.name.ToLower().Trim() + ".jar";
            this.url = url;
        }
    }
}
