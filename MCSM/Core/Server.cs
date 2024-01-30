using System.IO;

namespace MCSM.Core
{
    public class Server
    {
        public string dir;
        public BukkitVersion bukkitVersion;

        public float ramAmount;
        public bool noGUI;
        public ServerProperties properties;

        public Server(string dir, BukkitVersion bukkitVersion)
        {
            this.dir = dir;
            this.bukkitVersion = bukkitVersion;
        }

        public void Create(bool ignoreNotEmpty) // 폴더의 빈 여부를 무시/무시하지 않음
        {
            Logger.WriteLog(Logger.LogLv.info, "Creating Server in: " + dir + ".");
            if (!ignoreNotEmpty && Directory.GetFiles(dir).Length != 0)
            {
                Logger.WriteLog(Logger.LogLv.error, "Selected directory isn't empty in MCSM Core: " + dir + ".");
                Logger.WriteLog(Logger.LogLv.error, "Creating Server Failed.");
                throw new Exception("Selected directory isn't empty");
            }
        }
    }

    public class ServerBuilder
    {
        private Server server;
        public ServerBuilder(string dir, BukkitVersion bv)
        {
            server = new Server(dir, bv);
        }

        public ServerBuilder SetRAM(float ramAmount)
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

        public Server Build()
        {
            return server;
        }
    }
}
