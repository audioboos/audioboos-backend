using System.Threading.Tasks;
using AudioBoos.Data.Access;
using AudioBoos.Data.Persistence.Interfaces;
using AudioBoos.Data.Store;
using Microsoft.AspNetCore.Mvc;

namespace AudioBoos.Server.Controllers; 

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
        return await Task.FromResult(Ok("Pong"));
    }
}