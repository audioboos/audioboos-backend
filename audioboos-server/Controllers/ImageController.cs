using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AudioBoos.Data.Access;
using AudioBoos.Data.Models.Settings;
using AudioBoos.Data.Store;
using AudioBoos.Server.Services.Images;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;

namespace AudioBoos.Server.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class ImageController : ControllerBase {
        private readonly IRepository<Artist> _artistRepository;
        private readonly IRepository<Album> _albumRepository;
        private readonly SystemSettings _systemSettings;

        public ImageController(IOptions<SystemSettings> systemSettings, IRepository<Artist> artistRepository,
            IRepository<Album> albumRepository) {
            _artistRepository = artistRepository;
            _albumRepository = albumRepository;
            _systemSettings = systemSettings.Value;
        }

        [HttpGet("album/{albumId}")]
        public async Task<IActionResult> GetAlbumImage(string albumId, [FromQuery] string type) {
            var album = await _albumRepository.GetAll()
                .Where(a => a.Id.Equals(Guid.Parse(albumId)))
                .Include(a => a.Artist)
                .FirstOrDefaultAsync();
            if (album is null) return NotFound();

            var image = type.Equals("small") ? album.SmallImage : album.LargeImage;

            return string.IsNullOrEmpty(image)
                ? await GeneratePlaceholderImage(album.Artist.Name, album.Name)
                : GetFileDirect(image);
        }

        public async Task<IActionResult> GeneratePlaceholderImage(string artistName, string albumName) {
            var image = await TextImageGenerator.CreateAlbumImage(artistName, albumName, 300, 300);
            return File(image, "image/png");
        }

        public IActionResult GetFileDirect(string path) {
            if (!System.IO.File.Exists(path)) {
                return NotFound();
            }

            var res = File(System.IO.File.OpenRead(path), "image/jpeg");
            res.EnableRangeProcessing = true;
            return res;
        }
    }
}
