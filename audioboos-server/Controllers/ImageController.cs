using System.IO;
using AudioBoos.Data.Models.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AudioBoos.Server.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class ImageController : ControllerBase {
        private readonly SystemSettings _systemSettings;

        public ImageController(IOptions<SystemSettings> systemSettings) {
            _systemSettings = systemSettings.Value;
        }

        [HttpGet("{artistName}/{albumName}/{imageName}")]
        public IActionResult GetFileDirect(string artistName, string albumName, string imageName) {
            var path = Path.Combine(
                _systemSettings.AudioPath,
                artistName,
                albumName,
                imageName);

            if (!System.IO.File.Exists(path)) {
                return NotFound();
            }

            var res = File(System.IO.File.OpenRead(path), "image/jpeg");
            res.EnableRangeProcessing = true;
            return res;
        }
    }
}
