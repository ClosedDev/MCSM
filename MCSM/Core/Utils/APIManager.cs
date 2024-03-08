using System.IO;
using System.Net.Http;

namespace MCSM.Core.Utils;

public static class APIManager
{
    static readonly HttpClient client = new();
    
    public static string Get(string url)
    {
        using (HttpRequestMessage req = new(HttpMethod.Get, url))
        using (var response = client.Send(req))
        using (Stream stream = response.Content.ReadAsStream())
        using (StreamReader reader = new(stream))
        {
            return reader.ReadToEnd();
        }
    }

    public async static Task<string> GetAsync(string url)
    {
        using (HttpRequestMessage req = new(HttpMethod.Get, url))
        using (var response = client.Send(req))
        using (Stream stream = response.Content.ReadAsStream())
        using (StreamReader reader = new(stream))
        {
            return await reader.ReadToEndAsync();
        }
    }
}