using System;
using System.IO;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCSM.Core
{
    public class Server
    {
        public string dir;
        public float ramAmount;
        public bool noGUI;


        public Server(string dir)
        {
            this.dir = dir;
        }

        public void Create(bool ignoreNotEmpty)
        {
            if (!ignoreNotEmpty && Directory.GetFiles(dir).Length != 0)
            {
                throw new Exception("Selected directory isn't empty");
            }
        }
    }

    public class ServerBuilder
    {
        private Server server;
        public ServerBuilder(string dir)
        {
            server = new Server(dir);
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
