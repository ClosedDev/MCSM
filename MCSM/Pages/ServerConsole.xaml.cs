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

namespace MCSM.Pages
{
    /// <summary>
    /// ServerConsole.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ServerConsole : Page
    {
        public ServerConsole()
        {
            InitializeComponent();
            Core.Core.serverConsole = this;
        }

        public async Task UpdateUI(string newText)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                TextView.Text = TextView.Text + "\n" + newText;
                LoggerScroll.ScrollToBottom();
            });
        }

        public void onProcessOutPut(object sender, Java.ProcessOnOutPutEventArgs e)
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
