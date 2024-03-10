using System.Text;

namespace MCSM.Core
{
    public class ServerProperties
    {
        public enum Properties
        {
            LEVEL_SEED,
            GAMEMODE,
            ENABLE_COMMAND_BLOCK,
            LEVEL_NAME,
            MOTD,
            PVP,
            DIFFICULTY,
            REQUIRE_RESOURCE_PACK,
            MAX_PLAYERS,
            ONLINE_MODE,
            ALLOW_FLIGHT,
            HIDE_ONLINE_PLAYERS,
            RESOURCE_PACK,
            HARDCORE,
            WHITE_LIST,
            SPAWN_NPCS,
            SPAWN_ANIMALS,
            LEVEL_TYPE,
            SPAWN_MOSTERS,
            SPAWN_PROTECTION
        }

        public readonly Dictionary<Properties, string> properties = new() // 주요 Properties와 기타 옵션을 넣어 커스텀 설정할 수 있도록 함
        {
            { Properties.LEVEL_SEED, "level-seed" },
            { Properties.GAMEMODE, "gamemode" },
            { Properties.ENABLE_COMMAND_BLOCK, "enable-command-block" },
            { Properties.LEVEL_NAME, "level-name" },
            { Properties.MOTD, "motd" },
            { Properties.PVP, "pvp" },
            { Properties.DIFFICULTY, "difficulty" },
            { Properties.REQUIRE_RESOURCE_PACK, "require-resource-pack" },
            { Properties.MAX_PLAYERS, "max-players" },
            { Properties.ONLINE_MODE, "online-mode" },
            { Properties.ALLOW_FLIGHT, "allow-flight" },
            { Properties.HIDE_ONLINE_PLAYERS, "hide-online-players" },
            { Properties.RESOURCE_PACK, "resource-pack" },
            { Properties.HARDCORE, "hardcore" },
            { Properties.WHITE_LIST, "white-list" },
            { Properties.SPAWN_NPCS, "spawn-npcs" },
            { Properties.SPAWN_ANIMALS, "spawn-animals" },
            { Properties.LEVEL_TYPE, "level-type" },
            { Properties.SPAWN_MOSTERS, "spawn-monsters" },
            { Properties.SPAWN_PROTECTION, "spawn-protection" }
        };
        
        private readonly Dictionary<string, string> values;
        
        public ServerProperties()
        {
            values = [];
        }

        public ServerProperties(string valueStr)
        {
            values = [];
            
            var lines = valueStr.Split(["\r\n", "\r", "\n"], StringSplitOptions.None);

            foreach (var line in lines)
            {
                if (string.IsNullOrEmpty(line) || line.StartsWith(';')) continue;
                var keyValue = line.Split('=');

                if (keyValue.Length == 2)
                {
                    values.Add(keyValue[0].Trim(), keyValue[1].Trim().Replace("\"", ""));
                }
            }
        }

        public ServerProperties(Dictionary<string, string> preset)
        {
            values = preset;
        }
        

        public string this[string key]
        {
            get => values[key];
            set => values[key] = value;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach (var element in values)
            {
                builder.Append($"{element.Key}={element.Value}\n");
            }
            return builder.ToString();
        }
    }
}
