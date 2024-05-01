using MCSM.Core;
using Ookii.Dialogs.Wpf;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MCSM.Pages
{
    /// <summary>
    /// MainPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            Logger.WriteLog(Logger.LogLv.info, "The main page is displayed.");
            InitializeComponent();
        }

        private async void Make_Server(object sender, RoutedEventArgs e)
        {
            string dir;

            Logger.WriteLog(Logger.LogLv.info, "Showing folder selection dialog.");
            VistaFolderBrowserDialog vfbd = new() { RootFolder = Environment.SpecialFolder.MyComputer };

            if (vfbd.ShowDialog() == true)
            {
                dir = vfbd.SelectedPath;
                if (!(Directory.GetFiles(dir).Length == 0))
                {
                    Logger.WriteLog(Logger.LogLv.warn, "Selected directory isn't empty: " + dir + ".");
                    MessageBox.Show("비어 있는 경로를 선택해 주세요.", "MCSM", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }
            else
            {
                Logger.WriteLog(Logger.LogLv.warn, "The user didn't specify a server's path.");
                MessageBox.Show("서버를 생성할 경로를 지정해 주세요.", "MCSM", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            ServerBuilder builder = new(dir, new BukkitVersion("1.10.2"));
            Server s = builder
                .SetNoGUI(false)
                .SetRAM(4000)
                .Build();

            MessageBox.Show(s.bukkitVersion.ToString(), "MCSM Core");

            await s.Create(false);
        }

        /*private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ScrollPanel.Children.Add(new TextBlock
            {
                Text = "An Text Block",
                Background = new SolidColorBrush(Color.FromScRgb(1f, 1f, 1f, 1f)),
                Width = 200f,
                Height = 30f,
                TextAlignment = TextAlignment.Center,
                FontSize = 20f
            });
        }*/

        private void Navigate_ServerConsole(object sender, RoutedEventArgs e)
        {
            Core.Core.mainWindow.navigatePage("/Pages/ServerConsole.xaml");
        }
    }
}
