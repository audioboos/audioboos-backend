using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Threading.Tasks;
using AudioBoos.Server.Helpers;
using AudioBoos.Server.Services.Utils;
using Color = System.Drawing.Color;
using RectangleF = System.Drawing.RectangleF;

namespace AudioBoos.Server.Services.Images;

public static class TextImageGenerator {
    private static async Task<Image> _loadImage(string imageName) {
        var image = Image.FromStream(await ResourceReader.ReadResourceAsByteArray(imageName));
        return image;
    }

    private static Font _loadFont(float fontSize = 36f, string fontName = "Roboto-Regular", string fontType = "ttf",
        FontStyle fontStyle = FontStyle.Regular) {
        var collection = new PrivateFontCollection();
        collection.AddFontFile(Path.Combine("./Resources/Fonts/", $"{fontName}.{fontType}"));
        return new Font(collection.Families[0], fontSize, fontStyle);
    }

    public static async Task<byte[]> CreateArtistAvatarImage(string artistName) {
        using var bitmap = new Bitmap(50, 50);
        using Graphics g = Graphics.FromImage(bitmap);
        g.Clear(Color.Transparent);
        using (Brush b = new SolidBrush(ColorTranslator.FromHtml("#96BC99"))) {
            g.FillEllipse(b, 0, 0, 49, 49);
        }

        g.DrawString(artistName.TrimTheFromStart()[..1],
            new Font(FontFamily.GenericSansSerif, 24, FontStyle.Regular),
            new SolidBrush(ColorTranslator.FromHtml("#EAC94B")), 13, 5);

        await using var ms = new MemoryStream();
        bitmap.Save(ms, ImageFormat.Png);

        return ms.ToArray();
    }

    public static async Task<byte[]> CreateArtistImage(string artistName) {
        var artistTextRect = new RectangleF(7, 7, 290, 200);

        var image = await _loadImage("default-artist.png");
        using Graphics g = Graphics.FromImage(image);

        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        StringFormat sf = new StringFormat();
        sf.Alignment = StringAlignment.Center;
        sf.LineAlignment = StringAlignment.Center;

        g.DrawString(
            artistName,
            _loadFont(32),
            Brushes.PapayaWhip,
            artistTextRect, sf
        );


        await using var ms = new MemoryStream();
        image.Save(ms, ImageFormat.Png);
        return ms.ToArray();
    }

    public static async Task<byte[]> CreateAlbumImage(string artistName, string albumName) {
        var artistTextRect = new RectangleF(12, 7, 260, 200);
        var albumTextRect = new RectangleF(12, 200, 260, 100);

        var image = await _loadImage("default-album.png");
        using Graphics g = Graphics.FromImage(image);

        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        StringFormat sf = new StringFormat();
        sf.Alignment = StringAlignment.Near;
        sf.LineAlignment = StringAlignment.Near;

        g.DrawString(
            artistName,
            _loadFont(32),
            Brushes.PapayaWhip,
            artistTextRect,
            sf
        );
        sf.LineAlignment = StringAlignment.Center;
        g.DrawString(
            albumName,
            _loadFont(16),
            new SolidBrush(ColorTranslator.FromHtml("#EB4557")),
            albumTextRect,
            sf
        );

        await using var ms = new MemoryStream();
        image.Save(ms, ImageFormat.Png);
        return ms.ToArray();
    }
}
