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
    private readonly IAudioRepository<Track> _trackRepository;
    private readonly UserManager<AppUser> _userManager;

    public TrackingController(AudioBoosContext context, IAudioRepository<Track> trackRepository,
        UserManager<AppUser> userManager) {
        _context = context;
        _trackRepository = trackRepository;
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<IActionResult> TrackPlay(string trackId) {
        var audioFile = await _trackRepository.GetById(trackId);
        if (audioFile is null) {
            return NotFound();
        }

        var remoteIp = Request.HttpContext.Connection.RemoteIpAddress;
        var user = await _userManager.GetUserAsync(HttpContext.User);
        var play = new TrackPlayLog {
            User = user,
            Track = audioFile,
            IPAddress = remoteIp
        };
        _context.TrackPlayLogs.Add(play);
        await _context.SaveChangesAsync();

        return Ok();
    }
}
