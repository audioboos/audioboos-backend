using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AudioBoos.Server.Helpers;

public static class HttpHelpers {
    public static async Task<MemoryStream> DownloadFileToStream(string url) {
        using var client = new HttpClient();
        await using var stream = await client.GetStreamAsync(url);
        var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        return memoryStream;
    }


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
