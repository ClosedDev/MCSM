namespace MCSM.Core
{
    public class Plugin
    {
        public enum Plugins
        {
            WORLDEDIT,
            WORLDGUARD,
            SKRIPT,
            ESSENTIALSX,
            COREPROTECT,
            OTHER
        }

        public readonly Dictionary<Plugins, Tuple<string, string>> plugins = new()
        {
            { Plugins.WORLDEDIT, new ("WorldEdit", "각종 명령어로 인게임 내에서 블럭을 생성하거나 없엘 수 있는 플러그인.") },
            { Plugins.WORLDGUARD, new ("WorldGuard", "블럭에 관련해 부수기 / 설치하기와 같이 규칙을 설정할 수 있는 플러그인.") },
            { Plugins.SKRIPT, new ("Skript", "Skript 언어를 사용할 수 있게 하는 플러그인.") },
            { Plugins.ESSENTIALSX, new ("EssentialsX", "서버 관리에 도움이 되는 100개 이상의 명령어를 추가하는 플러그인.") },
            { Plugins.COREPROTECT, new ("CoreProtect", "플레이어가 상호작용한 블럭의 모든 정보를 보여주고 복구할 수 있는 플러그인.") },
            { Plugins.OTHER, new ("기타", "직접 추가할 수 있습니다.") }
        };
    }
}
