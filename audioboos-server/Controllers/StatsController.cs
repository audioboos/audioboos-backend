using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AudioBoos.Data.Access;
using AudioBoos.Data.Models.DTO;
using AudioBoos.Data.Store;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AudioBoos.Server.Controllers;

[ApiController]
// [Authorize]
[Route("[controller]")]
public class StatsController : ControllerBase {
    private readonly IRepository<TrackPlayLog> _audioPlayRepository;

    public StatsController(ILogger<StatsController> logger, IRepository<TrackPlayLog> audioPlayRepository) {
        _audioPlayRepository = audioPlayRepository;
    }

    [HttpGet("artist/{artistName}/plays")]
    public async Task<ActionResult<List<TrackPlayDto>>> GetArtistPlays(string artistName) {
        var plays = await _audioPlayRepository.GetAll()
            .Where(r => r.Track.Album.Artist.Name.Equals(artistName))
            .Include(r => r.Track)
            .Include(r => r.User)
            .ToListAsync();

        var response = plays.Adapt<List<TrackPlayDto>>();
        return Ok(response);
    }

    [HttpGet("track/{trackName}/plays")]
    public async Task<ActionResult<List<TrackPlayDto>>> GetTrackPlays(string trackName) {
        var plays = await _audioPlayRepository.GetAll()
            .Where(r => r.Track.Name.Equals(trackName))
            .Include(r => r.Track)
            .Include(r => r.User)
            .ToListAsync();

        var response = plays.Adapt<List<TrackPlayDto>>();
        return Ok(response);
    }
}
