using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AudioBoos.Server.Helpers;

public static class HttpHelpers {
    public static async Task<string> DownloadFile(string url, string file = "") {
        using var client = new HttpClient();
        using var response = await client.GetAsync(url);
        if (response.StatusCode != HttpStatusCode.OK) {
            return string.Empty;
        }

        using var content = response.Content;
        if (string.IsNullOrEmpty(file))
            file = System.IO.Path.GetTempFileName();
        var result = await content.ReadAsByteArrayAsync();
        await System.IO.File.WriteAllBytesAsync(file, result);

        return file;
    }
}
