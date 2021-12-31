using System;
using System.Threading.Tasks;
using AudioBoos.Data.Access;
using AudioBoos.Data.Store;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AudioBoos.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class StreamController : ControllerBase {
    private readonly IAudioRepository<Track> _trackRepository;
    private readonly IRepository<TrackPlayLog> _audioPlayRepository;
    private readonly UserManager<AppUser> _userManager;

    public StreamController(IAudioRepository<Track> trackRepository,
        IRepository<TrackPlayLog> audioPlayRepository, UserManager<AppUser> userManager) {
        _trackRepository = trackRepository;
        _audioPlayRepository = audioPlayRepository;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetFileDirect([FromQuery] Guid trackId) {
        var track = await _trackRepository.GetById(trackId);
        if (track is null) {
            return NotFound();
        }

        var res = File(System.IO.File.OpenRead(track.PhysicalPath), "audio/mpeg");
        res.EnableRangeProcessing = true;

        await _trackPlay(track);
        return res;
    }

    private async Task _trackPlay(Track track) {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        var play = new TrackPlayLog {
            User = user,
            Track = track,
            IPAddress = Request.HttpContext.Connection.RemoteIpAddress
        };
        await _audioPlayRepository.InsertOrUpdate(play);
        await _audioPlayRepository.Context.SaveChangesAsync();
    }
}
