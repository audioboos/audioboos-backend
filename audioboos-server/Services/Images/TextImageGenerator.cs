using System;
using System.IO;
using System.Threading.Tasks;
using AudioBoos.Server.Services.Utils;
using ImageMagick;

namespace AudioBoos.Server.Services.Images;

public static class TextImageGenerator {
    private static async Task<MagickImage> _loadImage(string imageName) {
        var image = new MagickImage(await ResourceReader.ReadResourceAsByteArray(imageName));
        return image;
    }

    private static async Task _addTextToImage(this MagickImage image, string text, int yOffset, int width, int height,
        MagickColor color, int padding = 5) {
        var readSettings = new MagickReadSettings {
            Font = "Calibri",
            TextGravity = Gravity.Center,
            FillColor = color,
            BackgroundColor = MagickColors.Transparent,
            Height = height,
            Width = width - (padding * 2)
        };

        using var caption = new MagickImage($"caption:{text}", readSettings);

        image.Composite(caption, 0 + (padding / 2), yOffset, CompositeOperator.Atop);
    }


    public static async Task<byte[]> CreateArtistAvatarImage(string artistName) {
        throw new Exception();
    }

    public static async Task<byte[]> CreateArtistImage(string artistName) {
        using var image = await _loadImage("default-artist.png");
        await image._addTextToImage(artistName, 0, 300, 200, MagickColors.Azure);

        await using var ms = new MemoryStream();
        await image.WriteAsync(ms);
        return ms.ToArray();
    }

    public static async Task<byte[]> CreateAlbumImage(string artistName, string albumName) {
        using var image = await _loadImage("default-album.png");
        await image._addTextToImage(artistName, 0, 300, 200, MagickColors.Azure);
        await image._addTextToImage(albumName, 200, 300, 100, MagickColors.DarkGray);

        await using var ms = new MemoryStream();
        await image.WriteAsync(ms);
        return ms.ToArray();
    }
}
