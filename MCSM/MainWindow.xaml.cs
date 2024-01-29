using MCSM.Core;
using Ookii.Dialogs.Wpf;
using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;

namespace MCSM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Thread t1 = new Thread(new ThreadStart(RunAPI));
            t1.Start();

            InitializeComponent();
        }

        private void mainWindow_Closed(object sender, EventArgs e)
        {
            Logger.WriteLog(LogLv.info, "Application Closed.");
        }

        private void mainWindow_Closing(object sender, CancelEventArgs e)
        {
            Logger.WriteLog(LogLv.info, "Application Closing...");
        }
        private void RunAPI()
        {
            Logger.WriteLog(LogLv.info, "Loading version information from Paper...");
            BukkitVersions bvs = new();
            bvs.Versions = bvs.getButtkitVersionsWithAPI(true);
            Logger.WriteLog(LogLv.info, $"Loaded version information ( Length: {bvs.Versions.Length} )");
        }
    }
}