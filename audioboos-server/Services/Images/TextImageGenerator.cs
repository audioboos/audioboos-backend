using System;
using System.IO;
using System.Threading.Tasks;
using AudioBoos.Server.Helpers;
using AudioBoos.Server.Services.Utils;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Path = System.IO.Path;

namespace AudioBoos.Server.Services.Images;

public static class TextImageGenerator {
    private static async Task<Image> _loadImage(string imageName) {
        var image = await Image.LoadAsync(await ResourceReader.ReadResourceAsByteArray(imageName));
        return image;
    }

    private static Font _loadFont(float fontSize = 36f, string fontName = "Roboto-Regular", string fontType = "ttf",
        FontStyle fontStyle = FontStyle.Regular) {
        FontCollection collection = new FontCollection();
        FontFamily family = collection.Install(Path.Combine("./Resources/Fonts/", $"{fontName}.{fontType}"));
        Font font = family.CreateFont(fontSize, FontStyle.Italic);
        return font;
    }

    private static async Task<IImageProcessingContext> WriteTextInBounds(this IImageProcessingContext processingContext,
        string text,
        Size targetSize) {
        var font = _loadFont(100);
        RendererOptions
            style = new RendererOptions(font, 72); // again dpi doesn't overlay matter as this code genreates a vector

        // this is the important line, where we render the glyphs to a vector instead of directly to the image
        // this allows further vector manipulation (scaling, translating) etc without the expensive pixel operations.
        IPathCollection glyphs = TextBuilder.GenerateGlyphs(text, style);

        var widthScale = (targetSize.Width / glyphs.Bounds.Width);
        var heightScale = (targetSize.Height / glyphs.Bounds.Height);
        var minScale = Math.Min(widthScale, heightScale);

        // scale so that it will fit exactly in image shape once rendered
        glyphs = glyphs.Scale(minScale);

        // move the vectorised glyph so that it touchs top and left edges 
        // could be tweeked to center horizontaly & vertically here
        glyphs = glyphs.Translate(-glyphs.Bounds.Location);

        return processingContext.Fill(Color.Black, glyphs);
    }


    public static async Task<byte[]> CreateArtistAvatarImage(string artistName) {
        using var image = new Image<Rgba32>(50, 50);
        var text = artistName.TrimTheFromStart()[..1];
        var font = _loadFont(32);

        image.Mutate(x => x.Fill(Rgba32.ParseHex("#96BC99")));
        image.Mutate(x => x.DrawText(
            text,
            font,
            Rgba32.ParseHex("#EAC94B"),
            new PointF(15, 6)));

        await using var ms = new MemoryStream();
        await image.SaveAsPngAsync(ms);
        return ms.ToArray();
    }

    public static async Task<byte[]> CreateArtistImage(string artistName) {
        var image = await _loadImage("default-artist.png");
        image.Mutate(x => 
            x.WriteTextInBounds(
                artistName,
                new Size(300, 200)));

        await using var ms = new MemoryStream();
        await image.SaveAsPngAsync(ms);
        return ms.ToArray();
    }

    public static async Task<byte[]> CreateAlbumImage(string artistName, string albumName) {

        var image = await _loadImage("default-album.png");
        image.Mutate(x => 
            x.WriteTextInBounds(
                artistName,
                new Size(300, 200)));

        // image.Mutate(x => x.WriteTextInRect(
        //     _loadFont(16),
        //     $"{albumName}",
        //     Rgba32.ParseHex("#EAC94B"),
        //     new Rectangle(0, 200, 300, 100),
        //     5));

        await using var ms = new MemoryStream();
        await image.SaveAsPngAsync(ms);
        return ms.ToArray();
    }
}
