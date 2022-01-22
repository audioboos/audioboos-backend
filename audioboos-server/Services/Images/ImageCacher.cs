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

        if (imageFile.StartsWith("http")) {
            //image is a remote url
            var bufferFile = await HttpHelpers.DownloadFile(imageFile, Path.GetTempFileName());
            using var image = await Image.LoadAsync(bufferFile);
            await image.SaveAsJpegAsync(cacheFile);
            File.Delete(bufferFile);
        } else {
            //image is a local file in the audio folder
            File.Copy(
                //TODO: hack for using live db in debug
                Constants.DebugMode
                    ? imageFile.Replace("/audio/", "/mnt/frasier/audio/MuziQ/Pedestrian/Store/")
                    : imageFile,
                cacheFile);
        }

        return File.Exists(cacheFile);
    }
}
