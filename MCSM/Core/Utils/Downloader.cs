using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MCSM.Core.Utils
{
    static class Downloader
    {
        async public static Task Download(string url, string dir, string brackets = "", string naming = "")
        {
            await Task.Run(async () =>
            {
                try
                {
                    var name = naming == string.Empty ? naming : $" {naming}";
                    Logger.WriteLog(Logger.LogLv.info, $"Downloading{name}... : {url}", brackets);
                    
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    
                    using (HttpClient httpClient = new HttpClient())
                    {
                        var result = await httpClient.GetAsync(url);
                        result.EnsureSuccessStatusCode();

                        using (var stream = await result.Content.ReadAsStreamAsync())
                        {
                            using (var fs = new FileStream(dir, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
                            {
                                await stream.CopyToAsync(fs);
                            }
                        }
                    }
                    
                    stopwatch.Stop();
                    Logger.WriteLog(Logger.LogLv.info, $"Download{name} Complete ( {stopwatch.ElapsedMilliseconds}ms )");
                }
                catch (HttpRequestException ex)
                {
                    MessageBox.Show($"다운로드 중 알 수 없는 오류가 발생했습니다. : '{ex}'", "MCSM Core", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }
    }
}
