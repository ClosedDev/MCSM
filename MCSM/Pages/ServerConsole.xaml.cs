using MCSM.Core;
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
using MCSM.Core.Utils;

namespace MCSM.Pages
{
    /// <summary>
    /// ServerConsole.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ServerConsole : Page
    {
        public ServerConsole()
        {
            Logger.WriteLog(Logger.LogLv.info, "The console 시발ㅇ page is displayed.");
            InitializeComponent();
            Core.Core.serverConsole = this; //TODO 이 씨1발 이거 좆2같ㅇ은거좀 리팩토링 해야할듯
            //TODO 코어에서 모든 인스턴스 관리하는거 너무 불안정함
        }

        public async Task UpdateUI(string newText)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                TextView.Text = TextView.Text + "\n" + newText;
                LoggerScroll.ScrollToBottom();
            });
        }

        public void ProcessOutputEvent(object? sender, Java.ProcessOutputEventArgs e)
        {
            Logger.WriteLog(Logger.LogLv.info, e.Text);
            UpdateUI(e.Text);
        }

        private void TextBox_TextEntered(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                Core.Core.CurrentRunningJava.InputString(tbx.Text);
                tbx.Text = "";
            }
        }
    }
}
