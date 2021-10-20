using System.IO;
using System.Threading.Tasks;
using AudioBoos.Data.Access;
using AudioBoos.Data.Models.Settings;
using AudioBoos.Data.Store;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

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
            var album = await _albumRepository.GetById(albumId);
            if (album is null) return NotFound();

            var image = type.Equals("small") ? album.SmallImage : album.LargeImage;

            return string.IsNullOrEmpty(image)
                ? Ok("https://live.staticflickr.com/7062/6979126489_cd9b8bc323_b.jpg")
                : GetFileDirect(image);
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
