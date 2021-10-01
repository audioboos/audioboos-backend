using System.Threading.Tasks;
using AudioBoos.Data.Access;
using AudioBoos.Data.Persistence.Interfaces;
using AudioBoos.Data.Store;
using Microsoft.AspNetCore.Mvc;

namespace AudioBoos.Server.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class DebugController : ControllerBase {
        private readonly IRepository<AudioFile> _audioFileRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DebugController(IRepository<AudioFile> audioFileRepository, IUnitOfWork unitOfWork) {
            _audioFileRepository = audioFileRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Produces("text/plain")]
        public async Task<IActionResult> Ping() {
            var audioFile = new AudioFile(
                "/tmp/audio.mp3",
                "Artist",
                "Album",
                "Track");
            audioFile.Checksum = "INITIAL";

            var newRecord = await _audioFileRepository.InsertOrUpdate(audioFile);
            await _unitOfWork.Complete();

            newRecord.Checksum = "POST";
            var upserted = await _audioFileRepository.InsertOrUpdate(audioFile);

            var found = await _audioFileRepository.GetByFile(audioFile.PhysicalPath);

            return Ok(found);
        }
    }
}
