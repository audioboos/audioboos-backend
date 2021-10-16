using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AudioBoos.Server.Helpers;

public static class AlbumArtHelper {
    public static string GetSmallAlbumImagePath(string albumPath) {
        //AlbumArt_{7E9897E8-88CD-457E-8A8A-F3DAA893E2A1}_Large.jpg
        //AlbumArtLarge.jpg
        //folder.jpg
        return __getAlbumArtInternal(albumPath, "Small");
    }

    public static string GetLargeAlbumImagePath(string albumPath) {
        //AlbumArt_{7E9897E8-88CD-457E-8A8A-F3DAA893E2A1}_Small.jpg
        //AlbumArtSmall.jpg
        //folder.jpg
        return __getAlbumArtInternal(albumPath, "Large");
    }

    private static string __getAlbumArtInternal(string albumPath, string qualifier) {
        var file = Directory.GetFiles(albumPath)
            .Select(f => new FileInfo(f).Name)
            .Where(f =>
                (f.StartsWith("AlbumArt") && f.EndsWith($"{qualifier}.jpg") ||
                 f.Equals("default.jpg")))
            .Select(f => Path.Join(albumPath, f))
            .FirstOrDefault();
        return file;
    }
}
