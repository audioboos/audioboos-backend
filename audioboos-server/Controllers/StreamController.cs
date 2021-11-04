using System;
using System.Threading.Tasks;
using AudioBoos.Data.Access;
using AudioBoos.Data.Models.Settings;
using AudioBoos.Data.Store;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AudioBoos.Server.Controllers; 

[ApiController]
[Route("[controller]")]
public class StreamController : ControllerBase {
    private readonly IRepository<Track> _trackRepository;
    private readonly SystemSettings _systemSettings;

    public StreamController(IOptions<SystemSettings> systemSettings, IRepository<Track> trackRepository) {
        _trackRepository = trackRepository;
        _systemSettings = systemSettings.Value;
    }

    [HttpGet]
    public async Task<IActionResult> GetFileDirect([FromQuery] Guid trackId) {
        var track = await _trackRepository.GetById(trackId);
        if (track is null) {
            return NotFound();
        }

        var res = File(System.IO.File.OpenRead(track.PhysicalPath), "audio/wav");
        res.EnableRangeProcessing = true;
        return res;
    }
}