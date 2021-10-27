using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace AudioBoos.Server.Services.Images {
    public static class TextImageGenerator {
        private const string FONT_NAME = "Bubblegum";

        // private static Font _loadFonts() {
        //     FontCollection collection = new FontCollection();
        //     FontFamily family = collection.Install("./Resources/Fonts/Roboto-Regular.ttf");
        //     return family.CreateFont(48, FontStyle.Regular);
        // }

        private static async Task<Image> _loadImage() {
            var image = Image.FromStream(await Utils.ResourceReader.ReadResourceAsByteArray("default-album.png"));
            return image;
        }

        public static async Task<byte[]> CreateAlbumImage(string artistName, string albumName, int width, int height) {
            var artistRect = new RectangleF(12, 7, 290, 200); //rectf for My Text
            var albumRect = new RectangleF(12, 200, 290, 100); //rectf for My Text

            var image = await _loadImage();
            using Graphics g = Graphics.FromImage(image);

            //g.DrawRectangle(new Pen(Color.Red, 2), 655, 460, 535, 90); 
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Near;
            sf.LineAlignment = StringAlignment.Center;
            g.DrawString(artistName, new Font("Arial", 32, FontStyle.Bold), Brushes.PapayaWhip, artistRect, sf);
            g.DrawString(albumName, new Font("Arial", 16, FontStyle.Bold), Brushes.Black, albumRect, sf);

            await using var ms = new MemoryStream();
            image.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }

        //     public static async Task<byte[]> _createAlbumImage(string artistName, string albumName, int width, int height) {
        //         var image = await Image.LoadAsync(
        //             await Utils.ResourceReader.ReadResourceAsByteArray("default-album.png")
        //         );
        //
        //         var font = _loadFonts();
        //         image.Mutate(x => x.DrawText(
        //             artistName,
        //             font,
        //             Color.NavajoWhite,
        //             new PointF(10, 10)));
        //
        //         image.Mutate(x => x.DrawText(
        //             albumName,
        //             font,
        //             Color.Black,
        //             new PointF(10, 210)));
        //
        //         await using var outputStream = new MemoryStream();
        //         await image.SaveAsPngAsync(outputStream);
        //
        //         return outputStream.ToArray();
        //     }
    }
}
