using MCSM.Core;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MCSM.Pages
{
    /// <summary>
    /// MainPage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string dir;

            VistaFolderBrowserDialog vfbd = new() { RootFolder = Environment.SpecialFolder.MyComputer };

            if (vfbd.ShowDialog() == true)
            {
                dir = vfbd.SelectedPath;
            }
            else return;

            ServerBuilder builder = new(dir);
            Server s = builder
                .SetNoGUI(true)
                .SetRAM(4f)
                .Build();

            MessageBox.Show(s.dir, "MCSM Core");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
