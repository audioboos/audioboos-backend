using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using AudioBoos.Server.Helpers;
using AudioBoos.Server.Services.Utils;

namespace AudioBoos.Server.Services.Images; 

public static class TextImageGenerator {
    private static async Task<Image> _loadImage() {
        var image = Image.FromStream(await ResourceReader.ReadResourceAsByteArray("default-album.png"));
        return image;
    }

    public static async Task<byte[]> CreateArtistAvatarImage(string artistName) {
        using var bitmap = new Bitmap(50, 50);
        using Graphics g = Graphics.FromImage(bitmap);
        g.Clear(Color.White);
        using (Brush b = new SolidBrush(ColorTranslator.FromHtml("#eeeeee"))) {
            g.FillEllipse(b, 0, 0, 49, 49);
        }

        g.DrawString(artistName.TrimTheFromStart()[..1],
            new Font(FontFamily.GenericSansSerif, 12, FontStyle.Regular),
            new SolidBrush(Color.Black), 10, 15);

        await using var ms = new MemoryStream();
        bitmap.Save(ms, ImageFormat.Png);

        return ms.ToArray();
    }

    public static async Task<byte[]> CreateAlbumImage(string artistName, string albumName) {
        var artistTextRect = new RectangleF(12, 7, 290, 200);
        var albumTextRect = new RectangleF(12, 200, 290, 100);

        var image = await _loadImage();
        using Graphics g = Graphics.FromImage(image);

        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        g.PixelOffsetMode = PixelOffsetMode.HighQuality;
        StringFormat sf = new StringFormat();
        sf.Alignment = StringAlignment.Near;
        sf.LineAlignment = StringAlignment.Center;

        g.DrawString(artistName, new Font(FontFamily.GenericSansSerif, 32, FontStyle.Bold), Brushes.PapayaWhip,
            artistTextRect, sf);
        g.DrawString(albumName, new Font(FontFamily.GenericSansSerif, 16, FontStyle.Bold), Brushes.Black,
            albumTextRect, sf);

        await using var ms = new MemoryStream();
        image.Save(ms, ImageFormat.Png);
        return ms.ToArray();
    }
}
