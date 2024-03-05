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
            InitializeComponent();
        }

        private void mainWindow_Closed(object sender, EventArgs e)
        {
            Logger.WriteLog(Logger.LogLv.info, "Application Closed.");
        }

        private void mainWindow_Closing(object sender, CancelEventArgs e)
        {
            Logger.WriteLog(Logger.LogLv.info, "Application Closing...");
        }

        private void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Core.Core.Load();

            Wpf.Ui.Appearance.Watcher.Watch(
                this,
                Wpf.Ui.Appearance.BackgroundType.Mica,
                true);
        }
    }
}