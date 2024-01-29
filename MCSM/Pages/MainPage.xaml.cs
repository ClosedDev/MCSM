using MCSM.Core;
using Ookii.Dialogs.Wpf;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace MCSM.Pages
{
    /// <summary>
    /// MainPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            Logger.WriteLog(LogLv.info, "The main page is displayed.");
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string dir;

            Logger.WriteLog(LogLv.info, "Showing folder selection dialog.");
            VistaFolderBrowserDialog vfbd = new() { RootFolder = Environment.SpecialFolder.MyComputer };

            if (vfbd.ShowDialog() == true)
            {
                dir = vfbd.SelectedPath;
                if (!(Directory.GetFiles(dir).Length == 0))
                {
                    Logger.WriteLog(LogLv.warn, "Selected directory isn't empty: " + dir + ".");
                    MessageBox.Show("비어 있는 경로를 선택해 주세요.", "MCSM", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }
            else
            {
                Logger.WriteLog(LogLv.warn, "The user didn't specify a server's path.");
                MessageBox.Show("서버를 생성할 경로를 지정해 주세요.", "MCSM", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            ServerBuilder builder = new(dir, new BukkitVersion("1.20.4"));
            Server s = builder
                .SetNoGUI(true)
                .SetRAM(4f)
                .Build();

            MessageBox.Show(s.bv.ToString(), "MCSM Core");

            try
            {
                s.Create(false);
            } catch (Exception ex)
            {
                MessageBox.Show($"알 수 없는 오류가 발생했습니다. : '{ex}'", "MCSM Core", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }
    }
}
