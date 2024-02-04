using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCSM.Core
{
    public static class Core
    {
        static string EnvironmentAppdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MCSM";
        
        public static void Load()
        {
            if (!Directory.Exists(EnvironmentAppdata))
            {
                Directory.CreateDirectory(EnvironmentAppdata);
                
            }


        }
    }
}
