using System.IO;
using System.Threading.Tasks;
using AudioBoos.Server.Helpers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace AudioBoos.Server.Services.Images;

public static class ImageCacher {
    public static async Task<bool> CacheImage(string cacheFile, string imageFile) {
        if (!cacheFile.EndsWith(".jpg"))
            cacheFile = $"{cacheFile}.jpg";

        if (File.Exists(cacheFile)) {
            return true;
        }

        if (!Directory.Exists(Path.GetDirectoryName(cacheFile))) {
            Directory.CreateDirectory(Path.GetDirectoryName(cacheFile));
        }

        var bufferFile = await HttpHelpers.DownloadFile(imageFile, Path.GetTempFileName());
        using var image = await Image.LoadAsync(bufferFile);
        await image.SaveAsJpegAsync(cacheFile);
        File.Delete(bufferFile);

        return File.Exists(cacheFile);
    }
}
