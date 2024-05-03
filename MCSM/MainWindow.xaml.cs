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
            Core.Core.mainWindow = this;
        }

        private void mainWindow_Closed(object sender, EventArgs e)
        {
            Logger.WriteLog(Logger.LogLv.info, "Application Closed.");
        }

        private void mainWindow_Closing(object sender, CancelEventArgs e)
        {
            Logger.WriteLog(Logger.LogLv.info, "Application Closing...");
        }

        private async void mainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Wpf.Ui.Appearance.Watcher.Watch(
                this,
                Wpf.Ui.Appearance.BackgroundType.Mica,
                true);

            await Core.Core.Load();
        }

        public void navigatePage(object content)
        {
            main_frame.Navigate(content);
        }
    }
}