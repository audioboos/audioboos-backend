using System.IO;
using System.Threading.Tasks;
using AudioBoos.Server.Helpers;

namespace AudioBoos.Server.Services.Images;

public static class ImageCacher {
    public static async Task<bool> CacheImage(string cacheFile, string imageFile) {
        if (File.Exists(cacheFile)) {
            return true;
        }

        if (!Directory.Exists(Path.GetDirectoryName(cacheFile))) {
            Directory.CreateDirectory(Path.GetDirectoryName(cacheFile));
        }

        var file = await HttpHelpers.DownloadFile(imageFile, cacheFile);
        return !string.IsNullOrEmpty(file) && File.Exists(file);
    }
}
