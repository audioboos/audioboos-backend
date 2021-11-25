using System.Threading.Tasks;
using AudioBoos.Data.Access;
using AudioBoos.Data;
using AudioBoos.Data.Store;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AudioBoos.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class TrackingController : ControllerBase {
    private readonly AudioBoosContext _context;
    private readonly IRepository<AudioFile> _audioFileRepository;
    private readonly UserManager<AppUser> _userManager;

    public TrackingController(AudioBoosContext context, IRepository<AudioFile> audioFileRepository,
        UserManager<AppUser> userManager) {
        _context = context;
        _audioFileRepository = audioFileRepository;
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> TrackPlay(string trackId) {
        var audioFile = await _audioFileRepository.GetById(trackId);
        if (audioFile is null) {
            return NotFound();
        }

        var remoteIp = Request.HttpContext.Connection.RemoteIpAddress;
        var user = await _userManager.GetUserAsync(HttpContext.User);
        var play = new AudioPlay {
            User = user,
            File = audioFile,
            IPAddress = remoteIp
        };
        _context.AudioPlays.Add(play);
        await _context.SaveChangesAsync();

        return Ok();
    }
}
