using System.IO;

namespace MCSM.Core
{
    public class Server
    {
        public string dir;
        public BukkitVersion bv;

        public float ramAmount;
        public bool noGUI;

        public Server(string dir, BukkitVersion bv)
        {
            this.dir = dir;
            this.bv = bv;
        }

        public void Create(bool ignoreNotEmpty) // 폴더의 빈 여부를 무시/무시하지 않음
        {
            Logger.WriteLog(LogLv.info, "Creating Server in: " + dir + ".");
            if (!ignoreNotEmpty && Directory.GetFiles(dir).Length != 0)
            {
                Logger.WriteLog(LogLv.error, "Selected directory isn't empty in MCSM Core: " + dir + ".");
                Logger.WriteLog(LogLv.error, "Creating Server Failed.");
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

        public Server Build()
        {
            return server;
        }
    }
}
