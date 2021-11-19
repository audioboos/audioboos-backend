using System;
using System.IO;
using System.Threading.Tasks;
using AudioBoos.Server.Helpers;
using AudioBoos.Server.Services.Utils;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

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

    private static IImageProcessingContext WriteText(this IImageProcessingContext processingContext,
        Font font,
        string text,
        Color color,
        float padding) {
        var (width, height) = processingContext.GetCurrentSize();
        float targetWidth = width - (padding * 2);
        float targetHeight = height - (padding * 2);

        float targetMinHeight = height - (padding * 3); // must be with in a margin width of the target height

        // now we are working i 2 dimensions at once and can't just scale because it will cause the text to
        // reflow we need to just try multiple times

        var scaledFont = font;
        FontRectangle s = new FontRectangle(0, 0, float.MaxValue, float.MaxValue);

        float scaleFactor = (scaledFont.Size / 2); // every time we change direction we half this size
        int trapCount = (int)scaledFont.Size * 2;
        if (trapCount < 10) {
            trapCount = 10;
        }

        bool isTooSmall = false;

        while ((s.Height > targetHeight || s.Height < targetMinHeight) && trapCount > 0) {
            if (s.Height > targetHeight) {
                if (isTooSmall) {
                    scaleFactor /= 2;
                }

                scaledFont = new Font(scaledFont, scaledFont.Size - scaleFactor);
                isTooSmall = false;
            }

            if (s.Height < targetMinHeight) {
                if (!isTooSmall) {
                    scaleFactor /= 2;
                }

                scaledFont = new Font(scaledFont, scaledFont.Size + scaleFactor);
                isTooSmall = true;
            }

            trapCount--;

            s = TextMeasurer.Measure(text, new RendererOptions(scaledFont) {
                WrappingWidth = targetWidth
            });
        }

        var center = new PointF(padding, height / 2);
        var textGraphicOptions = new DrawingOptions {
            TextOptions = {
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
                WrapTextWidth = targetWidth
            }
        };
        return processingContext.DrawText(textGraphicOptions, text, scaledFont, color, center);
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
        image.Mutate(x => x.WriteText(
            _loadFont(32),
            artistName,
            Rgba32.ParseHex("#EAC94B"),
            5));

        await using var ms = new MemoryStream();
        await image.SaveAsPngAsync(ms);
        return ms.ToArray();
    }

    public static async Task<byte[]> CreateAlbumImage(string artistName, string albumName) {
        var image = await _loadImage("default-album.png");
        image.Mutate(x => x.WriteText(
            _loadFont(32),
            $"{artistName} - {albumName}",
            Rgba32.ParseHex("#EAC94B"),
            5));

        await using var ms = new MemoryStream();
        await image.SaveAsPngAsync(ms);
        return ms.ToArray();
    }
}
