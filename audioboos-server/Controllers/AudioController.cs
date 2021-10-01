using System.IO;
using AudioBoos.Data.Models.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AudioBoos.Server.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class AudioController : ControllerBase {
        private readonly SystemSettings _systemSettings;

        public AudioController(IOptions<SystemSettings> systemSettings) {
            _systemSettings = systemSettings.Value;
        }

        [HttpGet("{artistName}/{albumName}/{trackName}")]
        public IActionResult GetFileDirect(string artistName, string albumName, string trackName) {
            var path = Path.Combine(
                _systemSettings.AudioPath,
                artistName,
                albumName,
                trackName);

            if (!System.IO.File.Exists(path)) {
                return NotFound();
            }

            var res = File(System.IO.File.OpenRead(path), "audio/wav");
            res.EnableRangeProcessing = true;
            return res;
        }
    }
}
