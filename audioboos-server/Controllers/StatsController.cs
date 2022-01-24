using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AudioBoos.Data.Access;
using AudioBoos.Data.Models.DTO;
using AudioBoos.Data.Store;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Guid = System.Guid;

namespace AudioBoos.Server.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class StatsController : ControllerBase {
    private readonly IRepository<TrackPlayLog> _audioPlayRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public StatsController(ILogger<StatsController> logger, IRepository<TrackPlayLog> audioPlayRepository,
        IHttpContextAccessor httpContextAccessor) {
        _audioPlayRepository = audioPlayRepository;
        _httpContextAccessor = httpContextAccessor;
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

    [HttpGet("recent")]
    public async Task<ActionResult<TrackPlayDto>> GetRecentPlays() {
        var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var plays = await _audioPlayRepository.GetAll()
            .Where(t => t.User != null)
            .Where(t => t.User.Id.Equals(userId))
            .AsNoTracking()
            .Include(p => p.User)
            .AsNoTracking()
            .Include(r => r.Track)
            .ThenInclude(r => r.Album)
            .ThenInclude(r => r.Artist)
            .AsNoTracking()
            .AsSplitQuery()
            .ToListAsync();


        var response = plays.Adapt<List<TrackPlayDto>>();
        return Ok(response);
    }
}
